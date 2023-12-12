// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  9:29 AM


// using LockerContext =
//     OneOf.OneOf<object, System.Threading.SemaphoreSlim, System.Threading.Semaphore, System.Threading.ReaderWriterLockSlim, System.Threading.Mutex, System.Threading.SpinLock, System.Threading.EventWaitHandle, System.Threading.AutoResetEvent,
//         System.Threading.ManualResetEvent, System.Threading.ManualResetEventSlim, System.Threading.Barrier, System.Threading.CountdownEvent>;


namespace Jakar.Extensions;


public interface ILockedCollection<TCloser, TValue> : IReadOnlyCollection<TValue>
    where TCloser : IDisposable
{
    TCloser            AcquireLock();
    TCloser            AcquireLock( in CancellationToken   token );
    ValueTask<TCloser> AcquireLockAsync( CancellationToken token );


    ReadOnlyMemory<TValue>                               Copy();
    ConfiguredValueTaskAwaitable<ReadOnlyMemory<TValue>> CopyAsync( CancellationToken token );


    public static bool MoveNext( ref int index, in ReadOnlySpan<TValue> span, out TValue? current )
    {
        int i = Interlocked.Add( ref index, 1 );

        current = i < span.Length
                      ? span[i]
                      : default;

        return i < span.Length;
    }
}



public interface ILocker<TCloser>
    where TCloser : IDisposable
{
    bool               IsTaken { get; }
    TimeSpan?          TimeOut { get; }
    TCloser            Enter( CancellationToken      token = default );
    ValueTask<TCloser> EnterAsync( CancellationToken token = default );
    void               Exit();
}



public interface ILocker : ILocker<ILocker.Closer>
{
    public readonly record struct Closer( ILocker Locker ) : IDisposable
    {
        public void Dispose() => Locker.Exit();
    }
}



[ SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" ) ]
public sealed class Locker : ILocker, IEquatable<Locker>, IAsyncDisposable, IDisposable
{
    private const byte AUTO_RESET_EVENT        = 0;
    private const byte BARRIER                 = 1;
    private const byte COUNTDOWN_EVENT         = 2;
    private const byte EVENT_WAIT_HANDLE       = 3;
    private const byte MANUAL_RESET_EVENT      = 4;
    private const byte MANUAL_RESET_EVENT_SLIM = 5;
    private const byte MUTEX                   = 6;
    private const byte OBJECT                  = 7;
    private const byte READER_WRITER_LOCK_SLIM = 8;
    private const byte SEMAPHORE               = 9;
    private const byte SEMAPHORE_SLIM          = 10;
    private const byte SPIN_LOCK               = 11;


    private readonly AutoResetEvent?       _autoResetEvent;
    private readonly Barrier?              _barrier;
    private readonly byte                  _index;
    private readonly CountdownEvent?       _countdownEvent;
    private readonly EventWaitHandle?      _eventWaitHandle;
    private readonly ManualResetEvent?     _manualResetEvent;
    private readonly ManualResetEventSlim? _manualResetEventSlim;
    private readonly Mutex?                _mutex;
    private readonly ReaderWriterLockSlim? _readerWriterLockSlim;
    private readonly Semaphore?            _semaphore;
    private readonly SemaphoreSlim?        _semaphoreSlim;
    private readonly SpinLock?             _spinLock;
    private          bool                  _isTaken;


    public static Locker Default
    {
        [ Pure, MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => new(new SemaphoreSlim( 1 ));
    }
    public bool IsTaken
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _isTaken;
    }
    public TimeSpan? TimeOut { [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get; init; }


    public Locker() => _index = OBJECT;
    public Locker( SemaphoreSlim value )
    {
        _index         = SEMAPHORE_SLIM;
        _semaphoreSlim = value;
    }
    public Locker( Semaphore value )
    {
        _index     = SEMAPHORE;
        _semaphore = value;
    }
    public Locker( ReaderWriterLockSlim value )
    {
        _index                = READER_WRITER_LOCK_SLIM;
        _readerWriterLockSlim = value;
    }
    public Locker( Mutex value )
    {
        _index = MUTEX;
        _mutex = value;
    }
    public Locker( SpinLock value )
    {
        _index    = SPIN_LOCK;
        _spinLock = value;
    }
    public Locker( EventWaitHandle value )
    {
        _index           = EVENT_WAIT_HANDLE;
        _eventWaitHandle = value;
    }
    public Locker( AutoResetEvent value )
    {
        _index          = AUTO_RESET_EVENT;
        _autoResetEvent = value;
    }
    public Locker( ManualResetEvent value )
    {
        _index            = MANUAL_RESET_EVENT;
        _manualResetEvent = value;
    }
    public Locker( ManualResetEventSlim value )
    {
        _index                = MANUAL_RESET_EVENT_SLIM;
        _manualResetEventSlim = value;
    }
    public Locker( Barrier value )
    {
        _index   = BARRIER;
        _barrier = value;
    }
    public Locker( CountdownEvent value )
    {
        _index          = COUNTDOWN_EVENT;
        _countdownEvent = value;
    }


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


    public ILocker.Closer Enter( CancellationToken token = default ) => Enter( ref _isTaken, token );
    public ILocker.Closer Enter( ref bool lockTaken, CancellationToken token = default )
    {
        switch ( _index )
        {
            case OBJECT:
                lockTaken = false;
                Monitor.Enter( this, ref lockTaken );
                break;

            case SEMAPHORE_SLIM:
            {
                Debug.Assert( _semaphoreSlim != null, nameof(_semaphoreSlim) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _semaphoreSlim.Wait( TimeOut.Value,    token )
                                : _semaphoreSlim.Wait( Timeout.Infinite, token );

                break;
            }

            case SEMAPHORE:
            {
                Debug.Assert( _semaphore != null, nameof(_semaphore) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _semaphore.WaitOne( TimeOut.Value )
                                : _semaphore.WaitOne( Timeout.Infinite );

                break;
            }

            case READER_WRITER_LOCK_SLIM:
                Debug.Assert( _readerWriterLockSlim != null, nameof(_readerWriterLockSlim) + " != null" );
                _readerWriterLockSlim.EnterWriteLock();
                lockTaken = true;
                break;

            case MUTEX:
            {
                Debug.Assert( _mutex != null, nameof(_mutex) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _mutex.WaitOne( TimeOut.Value )
                                : _mutex.WaitOne( Timeout.Infinite );

                break;
            }

            case SPIN_LOCK:
                Debug.Assert( _spinLock != null, nameof(_spinLock) + " != null" );
                _spinLock.Value.Enter( ref lockTaken );
                break;

            case EVENT_WAIT_HANDLE:
            {
                Debug.Assert( _eventWaitHandle != null, nameof(_eventWaitHandle) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _eventWaitHandle.WaitOne( TimeOut.Value )
                                : _eventWaitHandle.WaitOne( Timeout.Infinite );

                break;
            }

            case AUTO_RESET_EVENT:
            {
                Debug.Assert( _autoResetEvent != null, nameof(_autoResetEvent) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _autoResetEvent.WaitOne( TimeOut.Value )
                                : _autoResetEvent.WaitOne( Timeout.Infinite );

                break;
            }

            case MANUAL_RESET_EVENT:
            {
                Debug.Assert( _manualResetEvent != null, nameof(_manualResetEvent) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _manualResetEvent.WaitOne( TimeOut.Value )
                                : _manualResetEvent.WaitOne( Timeout.Infinite );

                break;
            }

            case MANUAL_RESET_EVENT_SLIM:
            {
                Debug.Assert( _manualResetEventSlim != null, nameof(_manualResetEventSlim) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _manualResetEventSlim.Wait( TimeOut.Value,    token )
                                : _manualResetEventSlim.Wait( Timeout.Infinite, token );

                break;
            }

            case BARRIER:
            {
                Debug.Assert( _barrier != null, nameof(_barrier) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _barrier.SignalAndWait( TimeOut.Value,    token )
                                : _barrier.SignalAndWait( Timeout.Infinite, token );

                break;
            }

            case COUNTDOWN_EVENT:
            {
                Debug.Assert( _countdownEvent != null, nameof(_countdownEvent) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _countdownEvent.Wait( TimeOut.Value,    token )
                                : _countdownEvent.Wait( Timeout.Infinite, token );

                break;
            }

            default: throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" );
        }

        _isTaken = lockTaken;
        return new ILocker.Closer( this );
    }
    public async ValueTask<ILocker.Closer> EnterAsync( CancellationToken token = default )
    {
        bool lockTaken = false;

        switch ( _index )
        {
            case OBJECT:
                lockTaken = false;
                Monitor.Enter( this, ref lockTaken );
                break;

            case SEMAPHORE_SLIM:
            {
                Debug.Assert( _semaphoreSlim != null, nameof(_semaphoreSlim) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? await _semaphoreSlim.WaitAsync( TimeOut.Value,    token )
                                : await _semaphoreSlim.WaitAsync( Timeout.Infinite, token );

                break;
            }

            case SEMAPHORE:
            {
                Debug.Assert( _semaphore != null, nameof(_semaphore) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _semaphore.WaitOne( TimeOut.Value )
                                : _semaphore.WaitOne( Timeout.Infinite );

                break;
            }

            case READER_WRITER_LOCK_SLIM:
                Debug.Assert( _readerWriterLockSlim != null, nameof(_readerWriterLockSlim) + " != null" );
                _readerWriterLockSlim.EnterWriteLock();
                lockTaken = true;
                break;

            case MUTEX:
            {
                Debug.Assert( _mutex != null, nameof(_mutex) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _mutex.WaitOne( TimeOut.Value )
                                : _mutex.WaitOne( Timeout.Infinite );

                break;
            }

            case SPIN_LOCK:
                Debug.Assert( _spinLock != null, nameof(_spinLock) + " != null" );
                _spinLock.Value.Enter( ref lockTaken );
                break;

            case EVENT_WAIT_HANDLE:
            {
                Debug.Assert( _eventWaitHandle != null, nameof(_eventWaitHandle) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _eventWaitHandle.WaitOne( TimeOut.Value )
                                : _eventWaitHandle.WaitOne( Timeout.Infinite );

                break;
            }

            case AUTO_RESET_EVENT:
            {
                Debug.Assert( _autoResetEvent != null, nameof(_autoResetEvent) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _autoResetEvent.WaitOne( TimeOut.Value )
                                : _autoResetEvent.WaitOne( Timeout.Infinite );

                break;
            }

            case MANUAL_RESET_EVENT:
            {
                Debug.Assert( _manualResetEvent != null, nameof(_manualResetEvent) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _manualResetEvent.WaitOne( TimeOut.Value )
                                : _manualResetEvent.WaitOne( Timeout.Infinite );

                break;
            }

            case MANUAL_RESET_EVENT_SLIM:
            {
                Debug.Assert( _manualResetEventSlim != null, nameof(_manualResetEventSlim) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _manualResetEventSlim.Wait( TimeOut.Value,    token )
                                : _manualResetEventSlim.Wait( Timeout.Infinite, token );

                break;
            }

            case BARRIER:
            {
                Debug.Assert( _barrier != null, nameof(_barrier) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _barrier.SignalAndWait( TimeOut.Value,    token )
                                : _barrier.SignalAndWait( Timeout.Infinite, token );

                break;
            }

            case COUNTDOWN_EVENT:
            {
                Debug.Assert( _countdownEvent != null, nameof(_countdownEvent) + " != null" );

                lockTaken = TimeOut.HasValue
                                ? _countdownEvent.Wait( TimeOut.Value,    token )
                                : _countdownEvent.Wait( Timeout.Infinite, token );

                break;
            }

            default: throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" );
        }

        _isTaken = lockTaken;
        return new ILocker.Closer( this );
    }


    public void Dispose() => Exit();
    public ValueTask DisposeAsync()
    {
        Exit();
        return default;
    }
    public void Exit()
    {
        _isTaken = false;

        switch ( _index )
        {
            case OBJECT:
            {
                Monitor.Exit( this );
                Monitor.PulseAll( this );
                return;
            }

            case SEMAPHORE_SLIM:
                Debug.Assert( _semaphoreSlim is not null, nameof(_semaphoreSlim) + " is not null" );
                _semaphoreSlim.Release();
                return;

            case SEMAPHORE:
                Debug.Assert( _semaphore is not null, nameof(_semaphore) + " is not null" );
                _semaphore.Release();
                return;

            case READER_WRITER_LOCK_SLIM:
                Debug.Assert( _readerWriterLockSlim is not null, nameof(_readerWriterLockSlim) + " is not null" );
                _readerWriterLockSlim.ExitWriteLock();
                return;

            case MUTEX:
                Debug.Assert( _mutex is not null, nameof(_mutex) + " is not null" );
                _mutex.ReleaseMutex();
                return;

            case SPIN_LOCK:
                Debug.Assert( _spinLock is not null, nameof(_spinLock) + " is not null" );
                _spinLock.Value.Exit();
                return;

            case EVENT_WAIT_HANDLE:
                Debug.Assert( _eventWaitHandle is not null, nameof(_eventWaitHandle) + " is not null" );
                _eventWaitHandle.Set();
                return;

            case AUTO_RESET_EVENT:
                Debug.Assert( _autoResetEvent is not null, nameof(_autoResetEvent) + " is not null" );
                _autoResetEvent.Set();
                return;

            case MANUAL_RESET_EVENT:
                Debug.Assert( _manualResetEvent is not null, nameof(_manualResetEvent) + " is not null" );
                _manualResetEvent.Set();
                return;

            case MANUAL_RESET_EVENT_SLIM:
                Debug.Assert( _manualResetEventSlim is not null, nameof(_manualResetEventSlim) + " is not null" );
                _manualResetEventSlim.Set();
                return;

            case BARRIER:
                Debug.Assert( _barrier is not null, nameof(_barrier) + " is not null" );
                _barrier.SignalAndWait();
                return;

            case COUNTDOWN_EVENT:
                Debug.Assert( _countdownEvent is not null, nameof(_countdownEvent) + " is not null" );
                _countdownEvent.Reset();
                return;
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
        _index switch
        {
            OBJECT                  => base.ToString(),
            SEMAPHORE_SLIM          => _semaphoreSlim?.ToString(),
            SEMAPHORE               => _semaphore?.ToString(),
            READER_WRITER_LOCK_SLIM => _readerWriterLockSlim?.ToString(),
            MUTEX                   => _mutex?.ToString(),
            SPIN_LOCK               => _spinLock?.ToString(),
            EVENT_WAIT_HANDLE       => _eventWaitHandle?.ToString(),
            AUTO_RESET_EVENT        => _autoResetEvent?.ToString(),
            MANUAL_RESET_EVENT      => _manualResetEvent?.ToString(),
            MANUAL_RESET_EVENT_SLIM => _manualResetEventSlim?.ToString(),
            BARRIER                 => _barrier?.ToString(),
            COUNTDOWN_EVENT         => _countdownEvent?.ToString(),
            _                       => throw new OutOfRangeException( nameof(_index), _index )
        } ??
        string.Empty;
    public override int GetHashCode()
    {
        int? nullable = _index switch
                        {
                            SEMAPHORE_SLIM          => _semaphoreSlim?.GetHashCode(),
                            SEMAPHORE               => _semaphore?.GetHashCode(),
                            READER_WRITER_LOCK_SLIM => _readerWriterLockSlim?.GetHashCode(),
                            MUTEX                   => _mutex?.GetHashCode(),
                            SPIN_LOCK               => _spinLock?.GetHashCode(),
                            EVENT_WAIT_HANDLE       => _eventWaitHandle?.GetHashCode(),
                            AUTO_RESET_EVENT        => _autoResetEvent?.GetHashCode(),
                            MANUAL_RESET_EVENT      => _manualResetEvent?.GetHashCode(),
                            MANUAL_RESET_EVENT_SLIM => _manualResetEventSlim?.GetHashCode(),
                            BARRIER                 => _barrier?.GetHashCode(),
                            COUNTDOWN_EVENT         => _countdownEvent?.GetHashCode(),
                            _                       => null
                        };

        return (nullable.GetValueOrDefault( 0 ) * 397) ^ _index;
    }
    public override bool Equals( object? other ) => Equals( other as Locker );
    public bool Equals( Locker? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        bool flag1 = _index == other._index;
        if ( !flag1 ) { return flag1; }

        bool flag2 = _index switch
                     {
                         SEMAPHORE_SLIM          => Equals( _semaphoreSlim,        other._semaphoreSlim ),
                         SEMAPHORE               => Equals( _semaphore,            other._semaphore ),
                         READER_WRITER_LOCK_SLIM => Equals( _readerWriterLockSlim, other._readerWriterLockSlim ),
                         MUTEX                   => Equals( _mutex,                other._mutex ),
                         SPIN_LOCK               => Nullable.Equals( _spinLock, other._spinLock ),
                         EVENT_WAIT_HANDLE       => Equals( _eventWaitHandle,      other._eventWaitHandle ),
                         AUTO_RESET_EVENT        => Equals( _autoResetEvent,       other._autoResetEvent ),
                         MANUAL_RESET_EVENT      => Equals( _manualResetEvent,     other._manualResetEvent ),
                         MANUAL_RESET_EVENT_SLIM => Equals( _manualResetEventSlim, other._manualResetEventSlim ),
                         BARRIER                 => Equals( _barrier,              other._barrier ),
                         COUNTDOWN_EVENT         => Equals( _countdownEvent,       other._countdownEvent ),
                         _                       => false
                     };

        flag1 = flag2;

        return flag1;
    }
}
