// Jakar.Extensions :: Jakar.Extensions
// 10/18/2022  3:37 PM

using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;



namespace Console.Experiments;


public static class DelayValueTaskExtensions
{
    // public static DelayValueTask Delay( this TimeSpan time, CancellationToken token = default ) => new( time, token );
}



public readonly struct DelayValueTask : IDisposable
{
    private readonly ValueTask    _task;
    private readonly DelayPromise _promise;


    public DelayValueTask( in TimeSpan time, in CancellationToken token = default )
    {
        _promise = new DelayPromise( time, token );
        _task    = new ValueTask( _promise, (short)time.Ticks );
    }

    // Task.Delay( time, token );
    // var tasks   = new Task( _ => { }, token );
    // var _source = new TaskCompletionSource();
    public ValueTaskAwaiter GetAwaiter()
    {
        _promise.Reset();
        return _task.GetAwaiter();
    }
    public void Dispose() => _promise.Dispose();



    private sealed class DelayPromise : IValueTaskSource, IDisposable
    {
        private readonly CancellationToken                       _token;
        private readonly CancellationTokenRegistration           _registration;
        private readonly Timer                                   _timer;
        private          ManualResetValueTaskSourceCore<object?> _logic; // mutable struct; do not make this readonly


        public bool RunContinuationsAsynchronously
        {
            get => _logic.RunContinuationsAsynchronously;
            set => _logic.RunContinuationsAsynchronously = value;
        }
        public DelayPromise( in TimeSpan time, CancellationToken token = default )
        {
            _token        = token;
            _timer        = new Timer( Completed, this, time, TimeSpan.Zero );
            _registration = token.Register( Cancel );
        }
        public void Dispose()
        {
            _timer.Dispose();
            _registration.Dispose();
        }


        internal void Completed() => _logic.SetResult( null );
        private void Completed( object? _ ) => _logic.SetResult( null );
        private void Cancel() => _logic.SetException( _token.IsCancellationRequested
                                                          ? new TaskCanceledException()
                                                          : new TimeoutException() );


        public void Reset() => _logic.Reset();
        public void SetException( Exception error ) => _logic.SetException( error );


        void IValueTaskSource.GetResult( short                  token ) => _logic.GetResult( token );
        ValueTaskSourceStatus IValueTaskSource.GetStatus( short token ) => _logic.GetStatus( token );
        void IValueTaskSource.OnCompleted( Action<object?>      continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags ) => _logic.OnCompleted( continuation, state, token, flags );
    }
}
