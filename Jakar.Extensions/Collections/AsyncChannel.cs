// Jakar.Extensions :: Jakar.Extensions
// 03/20/2025  10:03

using System.Threading.Channels;



namespace Jakar.Extensions;


public sealed class AsyncChannel<TValue> : IDisposable
{
    public readonly  AsyncReader                Reader;
    public readonly  AsyncWriter                Writer;
    private readonly ConcurrentQueue<TValue>    __values     = [];
    private readonly TaskCompletionSource<bool> __completion = new();


    public int  Count   { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Reader.Count; }
    public bool IsEmpty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Reader.Count == 0; }


    public AsyncChannel()
    {
        Writer = new AsyncWriter(this);
        Reader = new AsyncReader(this);
    }
    public void Dispose() => Writer.TryComplete();


    public static implicit operator AsyncReader( AsyncChannel<TValue> channel ) => channel.Reader;
    public static implicit operator AsyncWriter( AsyncChannel<TValue> channel ) => channel.Writer;


    public void OnNext( TValue value ) => __values.Enqueue(value);
    internal bool TryComplete( Exception? error = null ) => error is not null
                                                                ? __completion.TrySetException(error)
                                                                : __completion.TrySetResult(true);



    public sealed class AsyncReader( AsyncChannel<TValue> parent ) : ChannelReader<TValue?>
    {
        private readonly AsyncChannel<TValue> __parent = parent;
        public override  bool                 CanCount   => true;
        public override  bool                 CanPeek    => true;
        public override  Task                 Completion { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __parent.__completion.Task; }
        public override  int                  Count      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __parent.__values.Count; }
        public           bool                 IsEmpty    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => __parent.__values.IsEmpty; }


        public TValue? Read() => __parent.__values.TryDequeue(out TValue? first)
                                     ? first
                                     : default;
        public override bool TryRead( [NotNullWhen(true)] out TValue? value )
        {
            value = Read();
            return value is not null;
        }
        public override bool               TryPeek( [NotNullWhen(true)] out TValue? value )           => __parent.__values.TryPeek(out value) && value is not null;
        public override ValueTask<TValue?> ReadAsync( CancellationToken             token = default ) => new(Read());
        public override async IAsyncEnumerable<TValue> ReadAllAsync( [EnumeratorCancellation] CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create("AsyncChannel.ReadAllAsync");

            while ( token.ShouldContinue() && !Completion.IsCompleted )
            {
                if ( Count > 0 )
                {
                    while ( TryRead(out TValue? value) ) { yield return value; }
                }

                await telemetrySpan.Delay(5, token).ConfigureAwait(false);
            }
        }
        public override ValueTask<bool> WaitToReadAsync( CancellationToken token = default ) => new(!__parent.__values.IsEmpty);
    }



    public sealed class AsyncWriter( AsyncChannel<TValue> parent ) : ChannelWriter<TValue>
    {
        private readonly AsyncChannel<TValue> __parent = parent;


        public override bool TryComplete( Exception? error = null ) => __parent.TryComplete(error);
        public override ValueTask<bool> WaitToWriteAsync( CancellationToken token = default ) => token.IsCancellationRequested
                                                                                                     ? ValueTask.FromCanceled<bool>(token)
                                                                                                     : new ValueTask<bool>(true);
        public override bool TryWrite( TValue value )
        {
            __parent.OnNext(value);
            return true;
        }
        public override ValueTask WriteAsync( TValue value, CancellationToken token = default )
        {
            __parent.OnNext(value);
            return ValueTask.CompletedTask;
        }
    }
}