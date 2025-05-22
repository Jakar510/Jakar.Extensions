// Jakar.Extensions :: Jakar.Extensions
// 03/20/2025  10:03

using System.Threading.Channels;



namespace Jakar.Extensions;


public sealed class AsyncChannel<TValue> : IDisposable
{
    public readonly  AsyncReader                Reader;
    public readonly  AsyncWriter                Writer;
    private readonly ConcurrentQueue<TValue>    _values     = [];
    private readonly TaskCompletionSource<bool> _completion = new();


    public int  Count   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Reader.Count; }
    public bool IsEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Reader.Count == 0; }


    public AsyncChannel()
    {
        Writer = new AsyncWriter( this );
        Reader = new AsyncReader( this );
    }
    public void Dispose() => Writer.TryComplete();


    public static implicit operator AsyncReader( AsyncChannel<TValue> channel ) => channel.Reader;
    public static implicit operator AsyncWriter( AsyncChannel<TValue> channel ) => channel.Writer;



    public class AsyncReader( AsyncChannel<TValue> parent ) : ChannelReader<TValue?>
    {
        private readonly AsyncChannel<TValue>       _parent = parent;
        private          TaskCompletionSource<bool> _Completion => _parent._completion;
        private          ConcurrentQueue<TValue>    _Values     => _parent._values;
        public override  bool                       CanCount    => true;
        public override  bool                       CanPeek     => true;
        public override  Task                       Completion  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Completion.Task; }
        public override  int                        Count       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Values.Count; }
        public           bool                       IsEmpty     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _Values.IsEmpty; }


        public TValue? Read() => _Values.TryDequeue( out TValue? first )
                                     ? first
                                     : default;
        public override bool TryRead( [NotNullWhen( true )] out TValue? value )
        {
            value = Read();
            return value is not null;
        }
        public override bool               TryPeek( [NotNullWhen( true )] out TValue? value )           => _Values.TryPeek( out value ) && value is not null;
        public override ValueTask<TValue?> ReadAsync( CancellationToken               token = default ) => new(Read());
        public override async IAsyncEnumerable<TValue> ReadAllAsync( [EnumeratorCancellation] CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create( "AsyncChannel.ReadAllAsync" );

            while ( token.ShouldContinue() && Completion.IsCompleted is false )
            {
                if ( Count > 0 )
                {
                    while ( TryRead( out TValue? value ) ) { yield return value; }
                }

                await telemetrySpan.Delay( 5, token ).ConfigureAwait( false );
            }
        }
        public override ValueTask<bool> WaitToReadAsync( CancellationToken token = default ) => new(_Values.IsEmpty is false);
    }



    public class AsyncWriter( AsyncChannel<TValue> parent ) : ChannelWriter<TValue>
    {
        private readonly AsyncChannel<TValue>       _parent = parent;
        private          TaskCompletionSource<bool> _Completion => _parent._completion;
        private          ConcurrentQueue<TValue>    _Values     => _parent._values;


        public override bool TryComplete( Exception? error = null ) => error is not null
                                                                           ? _Completion.TrySetException( error )
                                                                           : _Completion.TrySetResult( true );
        public override ValueTask<bool> WaitToWriteAsync( CancellationToken token = default ) => token.IsCancellationRequested
                                                                                                     ? ValueTask.FromCanceled<bool>( token )
                                                                                                     : new ValueTask<bool>( true );
        public override bool TryWrite( TValue value )
        {
            _Values.Enqueue( Validate.ThrowIfNull( value ) );
            return true;
        }
        public override ValueTask WriteAsync( TValue value, CancellationToken token = default )
        {
            _Values.Enqueue( Validate.ThrowIfNull( value ) );
            return ValueTask.CompletedTask;
        }
    }
}
