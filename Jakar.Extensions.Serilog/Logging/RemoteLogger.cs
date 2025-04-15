// TrueLogic :: TrueLogic.Common
// 01/13/2025  08:01

using System.Threading.Channels;
using Newtonsoft.Json;
using Serilog.Debugging;



namespace Jakar.Extensions.Serilog;


/// <summary> A <see cref="Api"/> to send log event batches to. Batches and empty batch notifications will not be sent concurrently. When the <see cref="RemoteLogger"/> is disposed, it will dispose this object if possible. </summary>
[Experimental( nameof(RemoteLogger) )]
public sealed class RemoteLogger : BackgroundService, ILogEventSink, ISetLoggingFailureListener, IAsyncDisposable
{
    private const    int                        BATCH_SIZE_LIMIT = 1000;
    private readonly Api                        _targetSink;
    private readonly Channel<LogEvent>          _queue;                   // Needed because the read loop needs to observe shutdown even when the target batched (remote) sink is unable to accept events (preventing the queue from being drained and completion being observed).
    private readonly FailureAwareBatchScheduler _batchScheduler  = new(); // Timer to coordinate batch sending
    private readonly Lock                       _stateLock       = new(); // These fields are used by the write side to signal shutdown. A mutex is required because the queue writer `Complete()` call is not idempotent and will throw if called multiple times, e.g. via multiple `Dispose()` calls on this sink.
    private readonly Queue<LogEvent>            _currentBatch    = new(BATCH_SIZE_LIMIT);
    private          CancellationToken          _shutdownSignal  = CancellationToken.None;
    private          ILoggingFailureListener    _failureListener = SelfLog.FailureListener;
    private          Task<bool>?                _cachedWaitToRead;


    private RemoteLogger( Uri loggingServer ) : base()
    {
        _targetSink = new Api( loggingServer );

        _queue = Channel.CreateBounded<LogEvent>( new BoundedChannelOptions( 25000 )
                                                  {
                                                      SingleReader                  = true,
                                                      SingleWriter                  = false,
                                                      AllowSynchronousContinuations = true
                                                  } );
    }
    public static RemoteLogger Create( Uri loggingServer ) => new(loggingServer);

    public async ValueTask DisposeAsync()
    {
        try { await StopAsync( CancellationToken.None ).ConfigureAwait( false ); }
        catch ( Exception ex )
        {
            // E.g. the task was canceled before ever being run, or internally failed and threw an unexpected exception.
            _failureListener.OnLoggingFailed( this, LoggingFailureKind.Final, "caught exception during async disposal", null, ex );
        }
        finally { Dispose(); }

        await _targetSink.DisposeAsync().ConfigureAwait( false );
    }
    public override void Dispose()
    {
        lock (_stateLock)
        {
            // Relies on synchronization via `_stateLock`: once the writer is completed, subsequent attempts to complete it will throw.
            _queue.Writer.Complete();
        }

        base.Dispose();
    }


    /// <summary> Emit the provided log event to the sink. If the sink is being disposed or the app domain unloaded, then the event is ignored. </summary>
    /// <param name="logEvent"> Log event to emit. </param>
    /// <exception cref="ArgumentNullException"> The event is null. </exception>
    /// <remarks> The sink implements the contract that any events whose Emit() method has completed at the time of sink disposal will be flushed (or attempted to, depending on app domain state). </remarks>
    public void Emit( LogEvent logEvent )
    {
        ArgumentNullException.ThrowIfNull( logEvent );

        if ( _shutdownSignal.IsCancellationRequested )
        {
            ;
            return;
        }

        _queue.Writer.TryWrite( logEvent );
    }


    public Task StartAsync() => StartAsync( CancellationToken.None );
    protected override async Task ExecuteAsync( CancellationToken token )
    {
        _shutdownSignal = token;
        Task waitForShutdownSignal = Task.Delay( Timeout.InfiniteTimeSpan, token ).ContinueWith( static e => e.Exception, TaskContinuationOptions.OnlyOnFaulted );

        do
        {
            // Code from here through to the `try` block is expected to be infallible.
            // It's structured this way because any failure modes within it haven't been accounted for in the rest of the sink design, and would need consideration in order for the sink to function robustly (i.e. to avoid hot/infinite looping).
            Task fillBatch = Task.Delay( _batchScheduler.NextInterval, token );

            do
            {
                while ( _currentBatch.Count < BATCH_SIZE_LIMIT && token.ShouldContinue() && _queue.Reader.TryRead( out LogEvent? next ) )
                {
                    _currentBatch.Enqueue( next );
                    ;
                }
            }
            while ( _currentBatch.Count == 0 && token.ShouldContinue() && await TryWaitToReadAsync( _queue.Reader, fillBatch, token ).ConfigureAwait( false ) );

            try
            {
                if ( _currentBatch.Count == 0 ) { await _targetSink.OnEmptyBatchAsync().ConfigureAwait( false ); }
                else
                {
                    await _targetSink.EmitBatchAsync( _currentBatch.ToArray(), token ).ConfigureAwait( false );

                    _currentBatch.Clear();
                    _batchScheduler.MarkSuccess();
                }
            }
            catch ( Exception ex )
            {
                _failureListener.OnLoggingFailed( this, LoggingFailureKind.Temporary, "failed emitting a batch", _currentBatch, ex );
                _batchScheduler.MarkFailure( out bool shouldDropBatch, out bool shouldDropQueue );

                if ( shouldDropBatch )
                {
                    _failureListener.OnLoggingFailed( this, LoggingFailureKind.Permanent, "dropping the current batch", _currentBatch, ex );
                    _currentBatch.Clear();
                }

                if ( shouldDropQueue ) { DrainOnFailure( LoggingFailureKind.Permanent, "dropping all queued events", ex ); }

                // Wait out the remainder of the batch fill time so that we don't overwhelm the server.
                // With each successive failure the interval will increase.
                // Needs special handling so that we don't need to make `fillBatch` cancellable (and thus fallible).
                await Task.WhenAny( fillBatch, waitForShutdownSignal ).ConfigureAwait( false );
            }
        }
        while ( token.ShouldContinue() );

        // At this point:
        //  - The sink is being disposed
        //  - The queue has been completed
        //  - The queue may or may not be empty
        //  - The waiting batch may or may not be empty
        //  - The target sink may or may not be accepting events

        // Try flushing the rest of the queue, but bail out on any failure.
        // Shutdown time is unbounded, but it doesn't make sense to pick an arbitrary limit - a future version might add a new option to control this.
        try
        {
            while ( _queue.Reader.TryPeek( out _ ) )
            {
                while ( _currentBatch.Count < BATCH_SIZE_LIMIT && _queue.Reader.TryRead( out LogEvent? next ) ) { _currentBatch.Enqueue( next ); }

                if ( _currentBatch.Count == 0 ) { continue; }

                await _targetSink.EmitBatchAsync( _currentBatch.ToArray(), token ).ConfigureAwait( false );
                _currentBatch.Clear();
            }
        }
        catch ( Exception ex )
        {
            _failureListener.OnLoggingFailed( this, LoggingFailureKind.Permanent, "dropping the current batch", _currentBatch, ex );
            DrainOnFailure( LoggingFailureKind.Final, "failed emitting a batch during shutdown; dropping remaining queued events", ex, true );
        }
    }
    private async Task<bool> TryWaitToReadAsync( ChannelReader<LogEvent> reader, Task timeout, CancellationToken cancellationToken )
    {
        // Wait until `reader` has items to read. Returns `false` if the `timeout` task completes, or if the reader is cancelled.
        Task<bool> waitToRead = _cachedWaitToRead ?? reader.WaitToReadAsync( cancellationToken ).AsTask();
        _cachedWaitToRead = null;

        Task completed = await Task.WhenAny( timeout, waitToRead ).ConfigureAwait( false );

        // Avoid unobserved task exceptions in the cancellation and failure cases. Note that we may not end up observing read task cancellation exceptions during shutdown, may be some room to improve.
        if ( completed is { Exception: not null, IsCanceled: false } ) { _failureListener.OnLoggingFailed( this, LoggingFailureKind.Temporary, "could not read from queue", null, completed.Exception ); }

        if ( completed == timeout )
        {
            // Dropping references to `waitToRead` will cause it and some supporting objects to leak; disposing it will break the channel and cause future attempts to read to fail.
            // So, we cache and reuse it next time around the loop.
            _cachedWaitToRead = waitToRead;
            return false;
        }

        if ( waitToRead.Status is not TaskStatus.RanToCompletion ) { return false; }

        return await waitToRead;
    }


    private void DrainOnFailure( LoggingFailureKind kind, string message, Exception? exception, bool ignoreShutdownSignal = false )
    {
        const int      BUFFER_LIMIT = 1024;
        List<LogEvent> buffer       = new(BUFFER_LIMIT + 1);

        // Not ideal, uses some CPU capacity unnecessarily and doesn't complete in bounded time.
        // The goal is to reduce memory pressure on the client if the server is offline for extended periods.
        // May be worth reviewing and possibly abandoning this.
        while ( _queue.Reader.TryRead( out LogEvent? logEvent ) && (ignoreShutdownSignal || _shutdownSignal.IsCancellationRequested is false) )
        {
            buffer.Add( logEvent );
            if ( buffer.Count != BUFFER_LIMIT ) { continue; }

            _failureListener.OnLoggingFailed( this, kind, message, buffer, exception );
            buffer.Clear();
        }

        _failureListener.OnLoggingFailed( this, kind, message, buffer, exception );
    }
    public void SetFailureListener( ILoggingFailureListener failureListener )
    {
        ArgumentNullException.ThrowIfNull( failureListener );
        _failureListener = failureListener;
    }



    /// <summary> Buffers log events into batches for background flushing. </summary>
    public sealed class Api : IAsyncDisposable
    {
        public const     int          PORT   = 10753;
        public const     string       INGEST = "ingest/logs";
        private readonly string       _targetString;
        private          bool         _disposed;
        private readonly WebRequester _requester;

        /// <summary> Buffers log events into batches for background flushing. </summary>
        public Api( Uri loggingServer )
        {
            Uri target = new(loggingServer, INGEST);
            _targetString = target.ToString();
            _requester    = WebRequester.Builder.Create( target ).Build();
        }
        public async ValueTask DisposeAsync()
        {
            if ( _disposed ) { return; }

            _disposed = true;
            await ValueTask.CompletedTask;
        }
        public override string ToString() => _targetString;


        public async ValueTask OnEmptyBatchAsync()
        {
            ObjectDisposedException.ThrowIf( _disposed, this );

            const string LINE = $@"[{nameof(RemoteLogger)}] {nameof(OnEmptyBatchAsync)}";
            await Console.Error.WriteLineAsync( LINE );
        }
        public async ValueTask<ErrorOrResult<LogResponse>> EmitBatchAsync( LogEvent[] batch, CancellationToken token = default )
        {
            ObjectDisposedException.ThrowIf( _disposed, this );
            Log[]                    logs     = batch.Select( Log.Create ).ToArray( batch.Length );
            WebResponse<LogResponse> response = await _requester.Post( INGEST, logs, token ).AsJson<LogResponse>();

            if ( response.IsSuccessStatusCode ) { return response.Payload; }

            Exception? e      = response.Exception;
            string?    error  = response.ErrorMessage;
            string     detail = $"Failed to send {logs.Length} logs due to '{error}'";
            return new Error( response.StatusCode, e?.GetType().Name ?? response.StatusDescription, e?.Message, detail, _targetString, error );
        }
    }



    /// <summary> Manages reconnection period and transient fault response for <see cref="RemoteLogger"/>. During normal operation an object of this type will simply echo the configured batch transmission period. When availability fluctuates, the class tracks the number of failed attempts, each time increasing the interval before reconnection is attempted (up to a set maximum) and at predefined points indicating that either the current batch, or entire waiting queue, should be dropped. This Serves two purposes - first, a loaded receiver may need a temporary reduction in traffic while coming back online. Second, the sender needs to account for both bad batches (the first fault response) and also overproduction (the second, queue-dropping response). In combination these should provide a reasonable delivery effort but ultimately protect the sender from memory exhaustion. </summary>
    private class FailureAwareBatchScheduler
    {
        private const           int          DROPPED_BATCHES_BEFORE_DROPPING_QUEUE = 10;
        private static readonly TimeSpan     _bufferingTimeLimit                   = TimeSpan.FromSeconds( 2 );
        private static readonly TimeSpan     _maximumBackoffInterval               = TimeSpan.FromMinutes( 1 );
        private static readonly TimeSpan     _minimumBackoffPeriod                 = TimeSpan.FromSeconds( 5 );
        private static readonly TimeSpan     _retryTimeLimit                       = TimeSpan.FromMinutes( 10 );
        private readonly        TimeProvider _timeProvider                         = TimeProvider.System;
        private                 int          _consecutiveDroppedBatches;
        private                 int          _failuresSinceSuccessfulBatch;
        private                 long?        _firstFailureTimestamp;


        public TimeSpan NextInterval
        {
            get
            {
                // Available, and first failure, just try the batch interval
                if ( _failuresSinceSuccessfulBatch <= 1 ) { return _bufferingTimeLimit; }

                double backoffFactor = Math.Pow( 2, _failuresSinceSuccessfulBatch - 1 );                   // Second failure, start ramping up the interval - first 2x, then 4x, ...
                long   backoffPeriod = Math.Max( _bufferingTimeLimit.Ticks, _minimumBackoffPeriod.Ticks ); // If the period is ridiculously short, give it a boost so we get some visible backoff.
                long   backedOff     = (long)(backoffPeriod * backoffFactor);                              // The "ideal" interval
                long   cappedBackoff = Math.Min( _maximumBackoffInterval.Ticks, backedOff );               // Capped to the maximum interval
                long   actual        = Math.Max( _bufferingTimeLimit.Ticks, cappedBackoff );               // Unless that's shorter than the period, in which case we'll just apply the period

                return TimeSpan.FromTicks( actual );
            }
        }

        public void MarkSuccess()
        {
            _failuresSinceSuccessfulBatch = 0;
            _consecutiveDroppedBatches    = 0;
            _firstFailureTimestamp        = null;
        }

        public void MarkFailure( out bool shouldDropBatch, out bool shouldDropQueue )
        {
            ++_failuresSinceSuccessfulBatch;
            _firstFailureTimestamp ??= _timeProvider.GetTimestamp();

            // Once we're up against the time limit, we'll try each subsequent batch once and then drop it.
            TimeSpan now          = _timeProvider.GetElapsedTime( _firstFailureTimestamp.Value );
            TimeSpan wouldRetryAt = now.Add( NextInterval );
            shouldDropBatch = wouldRetryAt >= _retryTimeLimit;

            if ( shouldDropBatch ) { ++_consecutiveDroppedBatches; }

            // After trying and dropping enough batches consecutively, we'll try to get out of the way and just drop everything after each subsequent failure.
            // Each time a batch is tried and fails, we'll drop it and drain the whole queue.
            shouldDropQueue = _consecutiveDroppedBatches >= DROPPED_BATCHES_BEFORE_DROPPING_QUEUE;
        }
    }



    public readonly record struct LogResponse( bool Success );



    public sealed class Log : BaseClass, JsonModels.IJsonModel
    {
        private static readonly    JsonSerializer                _serializer = JsonSerializer.CreateDefault();
        public                     ExceptionDetails?             Exception       { get; init; }
        public                     LogEventLevel                 Level           { get; init; }
        public                     string                        Message         { get; init; }
        public                     string                        MessageTemplate { get; init; }
        public                     ActivitySpanId?               SpanId          { get; init; }
        public                     DateTimeOffset                Timestamp       { get; init; }
        public                     ActivityTraceId?              TraceId         { get; init; }
        [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData  { get; set; }


        public Log( LogEvent log )
        {
            Timestamp       = log.Timestamp;
            Level           = log.Level;
            TraceId         = log.TraceId;
            SpanId          = log.SpanId;
            Message         = log.RenderMessage( CultureInfo.InvariantCulture );
            MessageTemplate = log.MessageTemplate.Text;
            Exception       = log.Exception?.FullDetails();
            if ( log.Properties.Count <= 0 ) { return; }

            IDictionary<string, JToken?> data = AdditionalData ??= new Dictionary<string, JToken?>( log.Properties.Count );
            foreach ( (string key, LogEventPropertyValue value) in log.Properties ) { data[key] = Convert( value ); }
        }
        public static Log Create( LogEvent log ) => new(log);


        private static JObject Convert( IReadOnlyDictionary<ScalarValue, LogEventPropertyValue> elements )
        {
            JObject result = new();

            foreach ( (ScalarValue key, LogEventPropertyValue logEventPropertyValue) in elements )
            {
                string? keyString = key.Value?.ToString();
                if ( string.IsNullOrWhiteSpace( keyString ) ) { continue; }

                result[keyString] = Convert( logEventPropertyValue );
            }

            return result;
        }
        private static JObject Convert( IReadOnlyList<LogEventProperty> properties )
        {
            JObject result = new();

            foreach ( LogEventProperty property in properties )
            {
                string? keyString = property.Name;
                if ( string.IsNullOrWhiteSpace( keyString ) ) { continue; }

                result[keyString] = Convert( property.Value );
            }

            return result;
        }
        private static JArray Convert( IReadOnlyList<LogEventPropertyValue> elements )
        {
            JArray result = new();

            foreach ( LogEventPropertyValue element in elements )
            {
                JToken? token = Convert( element );
                if ( token is not null ) { result.Add( token ); }
            }

            return result;
        }
        private static JToken? Convert( ScalarValue value ) => value.Value is null
                                                                   ? null
                                                                   : JToken.FromObject( value.Value, _serializer );
        private static JToken? Convert( LogEventPropertyValue value ) => value switch
                                                                         {
                                                                             ScalarValue sv                                      => Convert( sv ),
                                                                             DictionaryValue { Elements.Count : > 0 } dictionary => Convert( dictionary.Elements ),
                                                                             StructureValue { Properties.Count: > 0 } structure  => Convert( structure.Properties ),
                                                                             SequenceValue { Elements.Count   : > 0 } sequence   => Convert( sequence.Elements ),
                                                                             _                                                   => JToken.FromObject( value.ToString(), _serializer )
                                                                         };
    }
}
