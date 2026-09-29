// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  9:29 AM


// using LockerContext =
//     OneOf.OneOf<object, System.Threading.SemaphoreSlim, System.Threading.Semaphore, System.Threading.ReaderWriterLockSlim, System.Threading.Mutex, System.Threading.SpinLock, System.Threading.EventWaitHandle, System.Threading.AutoResetEvent,
//         System.Threading.ManualResetEvent, System.Threading.ManualResetEventSlim, System.Threading.Barrier, System.Threading.CountdownEvent>;


namespace Jakar.Extensions;


public interface ILockedCollection<TValue, TCloser> : IReadOnlyCollection<TValue>, IAsyncEnumerable<TValue>
    where TCloser : IDisposable
{
    Lock.Scope         AcquireLock();
    TCloser            AcquireLock( CancellationToken      token );
    ValueTask<TCloser> AcquireLockAsync( CancellationToken token );


    [Pure] [MustDisposeResource] ArrayBuffer<TValue>                               Copy();
    [Pure] [MustDisposeResource] ConfiguredValueTaskAwaitable<ArrayBuffer<TValue>> CopyAsync( CancellationToken token );
}



public interface ILockedCollection<TValue, TCloser, out TAsyncLockerEnumerator, out TLockerEnumerator> : ILockedCollection<TValue, TCloser>
    where TAsyncLockerEnumerator : IAsyncDisposable
    where TLockerEnumerator : IDisposable
    where TCloser : IDisposable
{
    TAsyncLockerEnumerator AsyncValues { get; }
    TLockerEnumerator      Values      { get; }
}



public interface ILocker
{
    bool              IsTaken { get; }
    TimeSpan?         TimeOut { get; }
    Closer            Enter( CancellationToken      token = default );
    ValueTask<Closer> EnterAsync( CancellationToken token = default );
    void              Exit();
}



[DefaultValue(nameof(Empty))]
public readonly struct LockCloser( Lock? locker ) : IDisposable
{
    private static readonly TimeSpan __timeOut = TimeSpan.FromMicroseconds(100);
    public static readonly  Closer   Empty     = new(null);
    private readonly        Lock?    __locker  = locker;
    public                  void     Dispose() => __locker?.Exit();


    [Pure] [MustDisposeResource] public static LockCloser Enter( Lock locker, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        locker.Enter();
        while ( token.ShouldContinue() && !locker.TryEnter(__timeOut) ) { }

        return new LockCloser(locker);
    }
    [Pure] [MustDisposeResource] public static async ValueTask<LockCloser> EnterAsync( Lock locker, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        while ( token.ShouldContinue() && !locker.TryEnter(__timeOut) ) { await Task.Delay(1, token).ConfigureAwait(false); }

        return new LockCloser(locker);
    }
}



[DefaultValue(nameof(Empty))]
public readonly struct Closer( ILocker? locker ) : IDisposable
{
    public static readonly Closer   Empty    = new(null);
    private readonly       ILocker? __locker = locker;
    public                 void     Dispose() => __locker?.Exit();
}



[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public sealed class Locker : ILocker, IEquatable<Locker>, IAsyncDisposable, IDisposable
{
    private readonly AutoResetEvent?  __autoResetEvent;
    private readonly Barrier?         __barrier;
    private readonly CountdownEvent?  __countdownEvent;
    private readonly EventWaitHandle? __eventWaitHandle;
#if NET9_0_OR_GREATER
    private readonly Lock? __lock;
#endif
    private readonly ManualResetEvent?     __manualResetEvent;
    private readonly ManualResetEventSlim? __manualResetEventSlim;
    private readonly Mutex?                __mutex;
    private readonly ReaderWriterLockSlim? __readerWriterLockSlim;
    private readonly Semaphore?            __semaphore;
    private readonly SemaphoreSlim?        __semaphoreSlim;
    private readonly SpinLock?             __spinLock;
    private readonly Type                  __index;
    private          bool                  __isTaken;

    public static Locker    Default { [Pure] get => new SemaphoreSlim(1, 1); }
    public        bool      IsTaken { [Pure] get => __isTaken; }
    public        TimeSpan? TimeOut { [Pure] get; init; }


    public Locker() : this(Type.Object) { }
    private Locker( Type                type ) => __index = type;
    public Locker( SemaphoreSlim        value ) : this(Type.SemaphoreSlim) => __semaphoreSlim = value;
    public Locker( Semaphore            value ) : this(Type.Semaphore) => __semaphore = value;
    public Locker( ReaderWriterLockSlim value ) : this(Type.ReaderWriterLockSlim) => __readerWriterLockSlim = value;
    public Locker( Mutex                value ) : this(Type.Mutex) => __mutex = value;
    public Locker( SpinLock             value ) : this(Type.SpinLock) => __spinLock = value;
    public Locker( EventWaitHandle      value ) : this(Type.EventWaitHandle) => __eventWaitHandle = value;
    public Locker( AutoResetEvent       value ) : this(Type.AutoResetEvent) => __autoResetEvent = value;
    public Locker( ManualResetEvent     value ) : this(Type.ManualResetEvent) => __manualResetEvent = value;
    public Locker( ManualResetEventSlim value ) : this(Type.ManualResetEventSlim) => __manualResetEventSlim = value;
    public Locker( Barrier              value ) : this(Type.Barrier) => __barrier = value;
    public Locker( CountdownEvent       value ) : this(Type.CountdownEvent) => __countdownEvent = value;
#if NET9_0_OR_GREATER
    public Locker( Lock value ) : this(Type.ThreadingLock) => __lock = value;
#endif


    public static implicit operator Locker( SemaphoreSlim        value ) => new(value);
    public static implicit operator Locker( Semaphore            value ) => new(value);
    public static implicit operator Locker( ReaderWriterLockSlim value ) => new(value);
    public static implicit operator Locker( Mutex                value ) => new(value);
    public static implicit operator Locker( SpinLock             value ) => new(value);
    public static implicit operator Locker( EventWaitHandle      value ) => new(value);
    public static implicit operator Locker( AutoResetEvent       value ) => new(value);
    public static implicit operator Locker( ManualResetEvent     value ) => new(value);
    public static implicit operator Locker( ManualResetEventSlim value ) => new(value);
    public static implicit operator Locker( Barrier              value ) => new(value);
    public static implicit operator Locker( CountdownEvent       value ) => new(value);

    public void Dispose()
    {
        Exit();
        __autoResetEvent?.Dispose();
        __barrier?.Dispose();
        __countdownEvent?.Dispose();
        __eventWaitHandle?.Dispose();
        __manualResetEvent?.Dispose();
        __manualResetEventSlim?.Dispose();
        __mutex?.Dispose();
        __readerWriterLockSlim?.Dispose();
        __semaphore?.Dispose();
        __semaphoreSlim?.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        Exit();
        if ( __autoResetEvent is not null ) { await Disposables.CastAndDisposeAsync(__autoResetEvent).ConfigureAwait(false); }

        if ( __barrier is not null ) { await Disposables.CastAndDisposeAsync(__barrier).ConfigureAwait(false); }

        if ( __countdownEvent is not null ) { await Disposables.CastAndDisposeAsync(__countdownEvent).ConfigureAwait(false); }

        if ( __eventWaitHandle is not null ) { await Disposables.CastAndDisposeAsync(__eventWaitHandle).ConfigureAwait(false); }

        if ( __manualResetEvent is not null ) { await Disposables.CastAndDisposeAsync(__manualResetEvent).ConfigureAwait(false); }

        if ( __manualResetEventSlim is not null ) { await Disposables.CastAndDisposeAsync(__manualResetEventSlim).ConfigureAwait(false); }

        if ( __mutex is not null ) { await Disposables.CastAndDisposeAsync(__mutex).ConfigureAwait(false); }

        if ( __readerWriterLockSlim is not null ) { await Disposables.CastAndDisposeAsync(__readerWriterLockSlim).ConfigureAwait(false); }

        if ( __semaphore is not null ) { await Disposables.CastAndDisposeAsync(__semaphore).ConfigureAwait(false); }

        if ( __semaphoreSlim is not null ) { await Disposables.CastAndDisposeAsync(__semaphoreSlim).ConfigureAwait(false); }
    }


    public Closer Enter( CancellationToken token = default )
    {
        switch ( __index )
        {
            case Type.Object:
            {
                Monitor.Enter(this, ref __isTaken);
                return new Closer(this);
            }

            case Type.SemaphoreSlim:
            {
                Debug.Assert(__semaphoreSlim is not null, $"{nameof(__semaphoreSlim)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __semaphoreSlim.Wait(TimeOut.Value,    token)
                                : __semaphoreSlim.Wait(Timeout.Infinite, token);

                return new Closer(this);
            }

            case Type.Semaphore:
            {
                Debug.Assert(__semaphore is not null, $"{nameof(__semaphore)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __semaphore.WaitOne(TimeOut.Value)
                                : __semaphore.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.ReaderWriterLockSlim:
            {
                Debug.Assert(__readerWriterLockSlim is not null, $"{nameof(__readerWriterLockSlim)} is not null");
                __readerWriterLockSlim.EnterWriteLock();
                __isTaken = true;
                return new Closer(this);
            }

            case Type.Mutex:
            {
                Debug.Assert(__mutex is not null, $"{nameof(__mutex)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __mutex.WaitOne(TimeOut.Value)
                                : __mutex.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.SpinLock:
            {
                Debug.Assert(__spinLock is not null, $"{nameof(__spinLock)} is not null");
                __spinLock.Value.Enter(ref __isTaken);
                return new Closer(this);
            }

            case Type.EventWaitHandle:
            {
                Debug.Assert(__eventWaitHandle is not null, $"{nameof(__eventWaitHandle)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __eventWaitHandle.WaitOne(TimeOut.Value)
                                : __eventWaitHandle.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.AutoResetEvent:
            {
                Debug.Assert(__autoResetEvent is not null, $"{nameof(__autoResetEvent)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __autoResetEvent.WaitOne(TimeOut.Value)
                                : __autoResetEvent.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.ManualResetEvent:
            {
                Debug.Assert(__manualResetEvent is not null, $"{nameof(__manualResetEvent)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __manualResetEvent.WaitOne(TimeOut.Value)
                                : __manualResetEvent.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.ManualResetEventSlim:
            {
                Debug.Assert(__manualResetEventSlim is not null, $"{nameof(__manualResetEventSlim)}  null");

                __isTaken = TimeOut.HasValue
                                ? __manualResetEventSlim.Wait(TimeOut.Value,    token)
                                : __manualResetEventSlim.Wait(Timeout.Infinite, token);

                return new Closer(this);
            }

            case Type.Barrier:
            {
                Debug.Assert(__barrier is not null, $"{nameof(__barrier)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __barrier.SignalAndWait(TimeOut.Value,    token)
                                : __barrier.SignalAndWait(Timeout.Infinite, token);

                return new Closer(this);
            }

            case Type.CountdownEvent:
            {
                Debug.Assert(__countdownEvent is not null, $"{nameof(__countdownEvent)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __countdownEvent.Wait(TimeOut.Value,    token)
                                : __countdownEvent.Wait(Timeout.Infinite, token);

                return new Closer(this);
            }

            default:
                throw new InvalidOperationException($"{nameof(Locker)} is not initialized");
        }
    }
    public async ValueTask<Closer> EnterAsync( CancellationToken token = default )
    {
        switch ( __index )
        {
            case Type.Object:
            {
                Monitor.Enter(this, ref __isTaken);
                return new Closer(this);
            }

            case Type.SemaphoreSlim:
            {
                Debug.Assert(__semaphoreSlim is not null, $"{nameof(__semaphoreSlim)} is not null");

                __isTaken = TimeOut.HasValue
                                ? await __semaphoreSlim.WaitAsync(TimeOut.Value,    token).ConfigureAwait(false)
                                : await __semaphoreSlim.WaitAsync(Timeout.Infinite, token).ConfigureAwait(false);

                return new Closer(this);
            }

            case Type.Semaphore:
            {
                Debug.Assert(__semaphore is not null, $"{nameof(__semaphore)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __semaphore.WaitOne(TimeOut.Value)
                                : __semaphore.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.ReaderWriterLockSlim:
            {
                Debug.Assert(__readerWriterLockSlim is not null, $"{nameof(__readerWriterLockSlim)} is not null");
                __readerWriterLockSlim.EnterWriteLock();
                __isTaken = true;
                return new Closer(this);
            }

            case Type.Mutex:
            {
                Debug.Assert(__mutex is not null, $"{nameof(__mutex)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __mutex.WaitOne(TimeOut.Value)
                                : __mutex.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.SpinLock:
            {
                Debug.Assert(__spinLock is not null, $"{nameof(__spinLock)} is not null");
                __spinLock.Value.Enter(ref __isTaken);
                return new Closer(this);
            }

            case Type.EventWaitHandle:
            {
                Debug.Assert(__eventWaitHandle is not null, $"{nameof(__eventWaitHandle)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __eventWaitHandle.WaitOne(TimeOut.Value)
                                : __eventWaitHandle.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.AutoResetEvent:
            {
                Debug.Assert(__autoResetEvent is not null, $"{nameof(__autoResetEvent)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __autoResetEvent.WaitOne(TimeOut.Value)
                                : __autoResetEvent.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.ManualResetEvent:
            {
                Debug.Assert(__manualResetEvent is not null, $"{nameof(__manualResetEvent)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __manualResetEvent.WaitOne(TimeOut.Value)
                                : __manualResetEvent.WaitOne(Timeout.Infinite);

                return new Closer(this);
            }

            case Type.ManualResetEventSlim:
            {
                Debug.Assert(__manualResetEventSlim is not null, $"{nameof(__manualResetEventSlim)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __manualResetEventSlim.Wait(TimeOut.Value,    token)
                                : __manualResetEventSlim.Wait(Timeout.Infinite, token);

                return new Closer(this);
            }

            case Type.Barrier:
            {
                Debug.Assert(__barrier is not null, $"{nameof(__barrier)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __barrier.SignalAndWait(TimeOut.Value,    token)
                                : __barrier.SignalAndWait(Timeout.Infinite, token);

                return new Closer(this);
            }

            case Type.CountdownEvent:
            {
                Debug.Assert(__countdownEvent is not null, $"{nameof(__countdownEvent)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __countdownEvent.Wait(TimeOut.Value,    token)
                                : __countdownEvent.Wait(Timeout.Infinite, token);

                return new Closer(this);
            }

        #if NET9_0_OR_GREATER
            case Type.ThreadingLock:
            {
                Debug.Assert(__lock is not null, $"{nameof(__lock)} is not null");

                __isTaken = TimeOut.HasValue
                                ? __lock.TryEnter(TimeOut.Value)
                                : __lock.TryEnter(Timeout.Infinite);

                return new Closer(this);
            }
        #endif
            default:
                throw new InvalidOperationException($"{nameof(Locker)} is not initialized");
        }
    }


    public void Exit()
    {
        __isTaken = false;

        switch ( __index )
        {
            case Type.Object:
            {
                Monitor.Exit(this);
                Monitor.PulseAll(this);
                return;
            }

            case Type.SemaphoreSlim:
                Debug.Assert(__semaphoreSlim is not null, nameof(__semaphoreSlim) + " is not null");
                __semaphoreSlim.Release();
                return;

            case Type.Semaphore:
                Debug.Assert(__semaphore is not null, nameof(__semaphore) + " is not null");
                __semaphore.Release();
                return;

            case Type.ReaderWriterLockSlim:
                Debug.Assert(__readerWriterLockSlim is not null, nameof(__readerWriterLockSlim) + " is not null");
                __readerWriterLockSlim.ExitWriteLock();
                return;

            case Type.Mutex:
                Debug.Assert(__mutex is not null, nameof(__mutex) + " is not null");
                __mutex.ReleaseMutex();
                return;

            case Type.SpinLock:
                Debug.Assert(__spinLock is not null, nameof(__spinLock) + " is not null");
                __spinLock.Value.Exit();
                return;

            case Type.EventWaitHandle:
                Debug.Assert(__eventWaitHandle is not null, nameof(__eventWaitHandle) + " is not null");
                __eventWaitHandle.Set();
                return;

            case Type.AutoResetEvent:
                Debug.Assert(__autoResetEvent is not null, nameof(__autoResetEvent) + " is not null");
                __autoResetEvent.Set();
                return;

            case Type.ManualResetEvent:
                Debug.Assert(__manualResetEvent is not null, nameof(__manualResetEvent) + " is not null");
                __manualResetEvent.Set();
                return;

            case Type.ManualResetEventSlim:
                Debug.Assert(__manualResetEventSlim is not null, nameof(__manualResetEventSlim) + " is not null");
                __manualResetEventSlim.Set();
                return;

            case Type.Barrier:
                Debug.Assert(__barrier is not null, nameof(__barrier) + " is not null");
                __barrier.SignalAndWait();
                return;

            case Type.CountdownEvent:
                Debug.Assert(__countdownEvent is not null, nameof(__countdownEvent) + " is not null");
                __countdownEvent.Reset();
                return;

        #if NET9_0_OR_GREATER
            case Type.ThreadingLock:
                Debug.Assert(__lock is not null, nameof(__lock) + " is not null");
                __lock.Exit();
                return;
        #endif
            default:
                throw new OutOfRangeException(__index);
        }
    }


    /*
    public void Switch( Action<object>               f0,
                        Action<SemaphoreSlim>        f1,
                        Action<Semaphore>            f2,
                        Action<ReaderWriterLockSlim> f3,
                        Action<Mutex>                f4,
                        Action<SpinLock>             f5,
                        Action<EventWaitHandle>      f6,
                        Action<AutoResetEvent>       f7,
                        Action<ManualResetEvent>     f8,
                        Action<ManualResetEventSlim> f9,
                        Action<Barrier>              f10,
                        Action<CountdownEvent>       f11
    )
    {
        switch ( _index )
        {
            case OBJECT:
                f0( this! );
                return;

            case SEMAPHORE_SLIM:
                f1( _semaphoreSlim! );
                return;

            case SEMAPHORE:
                f2( _semaphore! );
                return;

            case READER_WRITER_LOCK_SLIM:
                f3( _readerWriterLockSlim! );
                return;

            case MUTEX:
                f4( _mutex! );
                return;

            case SPIN_LOCK:
                f5( _spinLock!.Value );
                return;

            case EVENT_WAIT_HANDLE:
                f6( _eventWaitHandle! );
                return;

            case AUTO_RESET_EVENT:
                f7( _autoResetEvent! );
                return;

            case MANUAL_RESET_EVENT:
                f8( _manualResetEvent! );
                return;

            case MANUAL_RESET_EVENT_SLIM:
                f9( _manualResetEventSlim! );
                return;

            case BARRIER:
                f10( _barrier! );
                return;

            default:
            {
                if ( _index != COUNTDOWN_EVENT || f11 == default ) { throw new InvalidOperationException(); }

                f11( _countdownEvent! );
                return;
            }
        }
    }
    */


    public override string ToString() =>
        __index switch
        {
            Type.Object               => base.ToString(),
            Type.SemaphoreSlim        => __semaphoreSlim?.ToString(),
            Type.Semaphore            => __semaphore?.ToString(),
            Type.ReaderWriterLockSlim => __readerWriterLockSlim?.ToString(),
            Type.Mutex                => __mutex?.ToString(),
            Type.SpinLock             => __spinLock?.ToString(),
            Type.EventWaitHandle      => __eventWaitHandle?.ToString(),
            Type.AutoResetEvent       => __autoResetEvent?.ToString(),
            Type.ManualResetEvent     => __manualResetEvent?.ToString(),
            Type.ManualResetEventSlim => __manualResetEventSlim?.ToString(),
            Type.Barrier              => __barrier?.ToString(),
            Type.CountdownEvent       => __countdownEvent?.ToString(),
        #if NET9_0_OR_GREATER
            Type.ThreadingLock => __lock?.ToString(),
        #endif
            _ => throw new OutOfRangeException(__index)
        } ??
        EMPTY;
    public override int GetHashCode()
    {
        int? nullable = __index switch
                        {
                            Type.SemaphoreSlim        => __semaphoreSlim?.GetHashCode(),
                            Type.Semaphore            => __semaphore?.GetHashCode(),
                            Type.ReaderWriterLockSlim => __readerWriterLockSlim?.GetHashCode(),
                            Type.Mutex                => __mutex?.GetHashCode(),
                            Type.SpinLock             => __spinLock?.GetHashCode(),
                            Type.EventWaitHandle      => __eventWaitHandle?.GetHashCode(),
                            Type.AutoResetEvent       => __autoResetEvent?.GetHashCode(),
                            Type.ManualResetEvent     => __manualResetEvent?.GetHashCode(),
                            Type.ManualResetEventSlim => __manualResetEventSlim?.GetHashCode(),
                            Type.Barrier              => __barrier?.GetHashCode(),
                            Type.CountdownEvent       => __countdownEvent?.GetHashCode(),
                        #if NET9_0_OR_GREATER
                            Type.ThreadingLock => __lock?.GetHashCode(),
                        #endif
                            _ => null
                        };

        return ( nullable.GetValueOrDefault(0) * 397 ) ^ __index.GetHashCode();
    }
    public override bool Equals( object? other ) => Equals(other as Locker);
    public bool Equals( Locker? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        if ( __index != other.__index ) { return false; }

        bool check = __index switch
                     {
                         Type.SemaphoreSlim        => Equals(__semaphoreSlim,        other.__semaphoreSlim),
                         Type.Semaphore            => Equals(__semaphore,            other.__semaphore),
                         Type.ReaderWriterLockSlim => Equals(__readerWriterLockSlim, other.__readerWriterLockSlim),
                         Type.Mutex                => Equals(__mutex,                other.__mutex),
                         Type.SpinLock             => Nullable.Equals(__spinLock, other.__spinLock),
                         Type.EventWaitHandle      => Equals(__eventWaitHandle,      other.__eventWaitHandle),
                         Type.AutoResetEvent       => Equals(__autoResetEvent,       other.__autoResetEvent),
                         Type.ManualResetEvent     => Equals(__manualResetEvent,     other.__manualResetEvent),
                         Type.ManualResetEventSlim => Equals(__manualResetEventSlim, other.__manualResetEventSlim),
                         Type.Barrier              => Equals(__barrier,              other.__barrier),
                         Type.CountdownEvent       => Equals(__countdownEvent,       other.__countdownEvent),
                     #if NET9_0_OR_GREATER
                     #pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
                         Type.ThreadingLock => Equals(__lock, other.__lock),
                     #pragma warning restore CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
                     #endif
                         _ => false
                     };

        return check;
    }



    private enum Type
    {
        Object,
        SemaphoreSlim,
        Semaphore,
        ReaderWriterLockSlim,
        Mutex,
        SpinLock,
        EventWaitHandle,
        AutoResetEvent,
        ManualResetEvent,
        ManualResetEventSlim,
        Barrier,
        CountdownEvent,
    #if NET9_0_OR_GREATER
        ThreadingLock
    #endif
    }
}
