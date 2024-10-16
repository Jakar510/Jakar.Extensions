// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  9:29 AM


// using LockerContext =
//     OneOf.OneOf<object, System.Threading.SemaphoreSlim, System.Threading.Semaphore, System.Threading.ReaderWriterLockSlim, System.Threading.Mutex, System.Threading.SpinLock, System.Threading.EventWaitHandle, System.Threading.AutoResetEvent,
//         System.Threading.ManualResetEvent, System.Threading.ManualResetEventSlim, System.Threading.Barrier, System.Threading.CountdownEvent>;


namespace Jakar.Extensions;


public interface ILockedCollection<TValue> : IReadOnlyCollection<TValue>, IAsyncEnumerable<TValue>
{
    Closer            AcquireLock();
    Closer            AcquireLock( CancellationToken      token );
    ValueTask<Closer> AcquireLockAsync( CancellationToken token );


    [Pure, MustDisposeResource] FilterBuffer<TValue>                               Copy();
    [Pure, MustDisposeResource] ConfiguredValueTaskAwaitable<FilterBuffer<TValue>> CopyAsync( CancellationToken token );
}



public interface ILockedCollection<TValue, out TAsyncLockerEnumerator, out TLockerEnumerator> : ILockedCollection<TValue>
    where TAsyncLockerEnumerator : IAsyncDisposable
    where TLockerEnumerator : IDisposable
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



public readonly record struct Closer( ILocker Locker ) : IDisposable
{
    public void Dispose() => Locker.Exit();
}



[SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" )]
public sealed class Locker : ILocker, IEquatable<Locker>, IAsyncDisposable, IDisposable
{
    private readonly AutoResetEvent?       _autoResetEvent;
    private readonly Barrier?              _barrier;
    private readonly CountdownEvent?       _countdownEvent;
    private readonly EventWaitHandle?      _eventWaitHandle;
    private readonly ManualResetEvent?     _manualResetEvent;
    private readonly ManualResetEventSlim? _manualResetEventSlim;
    private readonly Mutex?                _mutex;
    private readonly ReaderWriterLockSlim? _readerWriterLockSlim;
    private readonly Semaphore?            _semaphore;
    private readonly SemaphoreSlim?        _semaphoreSlim;
    private readonly SpinLock?             _spinLock;
    private readonly Type                  _index;
    private          bool                  _isTaken;


    public static Locker    Default { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(new SemaphoreSlim( 1, 1 )); }
    public        bool      IsTaken { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _isTaken; }
    public        TimeSpan? TimeOut { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }


    public Locker() : this( Type.Object ) { }
    private Locker( Type                type ) => _index = type;
    public Locker( SemaphoreSlim        value ) : this( Type.SemaphoreSlim ) => _semaphoreSlim = value;
    public Locker( Semaphore            value ) : this( Type.Semaphore ) => _semaphore = value;
    public Locker( ReaderWriterLockSlim value ) : this( Type.ReaderWriterLockSlim ) => _readerWriterLockSlim = value;
    public Locker( Mutex                value ) : this( Type.Mutex ) => _mutex = value;
    public Locker( SpinLock             value ) : this( Type.SpinLock ) => _spinLock = value;
    public Locker( EventWaitHandle      value ) : this( Type.EventWaitHandle ) => _eventWaitHandle = value;
    public Locker( AutoResetEvent       value ) : this( Type.AutoResetEvent ) => _autoResetEvent = value;
    public Locker( ManualResetEvent     value ) : this( Type.ManualResetEvent ) => _manualResetEvent = value;
    public Locker( ManualResetEventSlim value ) : this( Type.ManualResetEventSlim ) => _manualResetEventSlim = value;
    public Locker( Barrier              value ) : this( Type.Barrier ) => _barrier = value;
    public Locker( CountdownEvent       value ) : this( Type.CountdownEvent ) => _countdownEvent = value;


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
        _autoResetEvent?.Dispose();
        _barrier?.Dispose();
        _countdownEvent?.Dispose();
        _eventWaitHandle?.Dispose();
        _manualResetEvent?.Dispose();
        _manualResetEventSlim?.Dispose();
        _mutex?.Dispose();
        _readerWriterLockSlim?.Dispose();
        _semaphore?.Dispose();
        _semaphoreSlim?.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        Exit();
        if ( _autoResetEvent is not null ) { await CastAndDispose( _autoResetEvent ); }

        if ( _barrier is not null ) { await CastAndDispose( _barrier ); }

        if ( _countdownEvent is not null ) { await CastAndDispose( _countdownEvent ); }

        if ( _eventWaitHandle is not null ) { await CastAndDispose( _eventWaitHandle ); }

        if ( _manualResetEvent is not null ) { await CastAndDispose( _manualResetEvent ); }

        if ( _manualResetEventSlim is not null ) { await CastAndDispose( _manualResetEventSlim ); }

        if ( _mutex is not null ) { await CastAndDispose( _mutex ); }

        if ( _readerWriterLockSlim is not null ) { await CastAndDispose( _readerWriterLockSlim ); }

        if ( _semaphore is not null ) { await CastAndDispose( _semaphore ); }

        if ( _semaphoreSlim is not null ) { await CastAndDispose( _semaphoreSlim ); }

        return;

        static async ValueTask CastAndDispose( IDisposable resource )
        {
            if ( resource is IAsyncDisposable resourceAsyncDisposable ) { await resourceAsyncDisposable.DisposeAsync(); }
            else { resource.Dispose(); }
        }
    }


    public Closer Enter( CancellationToken token = default )
    {
        switch ( _index )
        {
            case Type.Object:
            {
                Monitor.Enter( this, ref _isTaken );
                return new Closer( this );
            }

            case Type.SemaphoreSlim:
            {
                Debug.Assert( _semaphoreSlim is not null, $"{nameof(_semaphoreSlim)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _semaphoreSlim.Wait( TimeOut.Value,    token )
                               : _semaphoreSlim.Wait( Timeout.Infinite, token );

                return new Closer( this );
            }

            case Type.Semaphore:
            {
                Debug.Assert( _semaphore is not null, $"{nameof(_semaphore)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _semaphore.WaitOne( TimeOut.Value )
                               : _semaphore.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.ReaderWriterLockSlim:
            {
                Debug.Assert( _readerWriterLockSlim is not null, $"{nameof(_readerWriterLockSlim)} is not null" );
                _readerWriterLockSlim.EnterWriteLock();
                _isTaken = true;
                return new Closer( this );
            }

            case Type.Mutex:
            {
                Debug.Assert( _mutex is not null, $"{nameof(_mutex)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _mutex.WaitOne( TimeOut.Value )
                               : _mutex.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.SpinLock:
            {
                Debug.Assert( _spinLock is not null, $"{nameof(_spinLock)} is not null" );
                _spinLock.Value.Enter( ref _isTaken );
                return new Closer( this );
            }

            case Type.EventWaitHandle:
            {
                Debug.Assert( _eventWaitHandle is not null, $"{nameof(_eventWaitHandle)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _eventWaitHandle.WaitOne( TimeOut.Value )
                               : _eventWaitHandle.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.AutoResetEvent:
            {
                Debug.Assert( _autoResetEvent is not null, $"{nameof(_autoResetEvent)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _autoResetEvent.WaitOne( TimeOut.Value )
                               : _autoResetEvent.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.ManualResetEvent:
            {
                Debug.Assert( _manualResetEvent is not null, $"{nameof(_manualResetEvent)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _manualResetEvent.WaitOne( TimeOut.Value )
                               : _manualResetEvent.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.ManualResetEventSlim:
            {
                Debug.Assert( _manualResetEventSlim is not null, $"{nameof(_manualResetEventSlim)}  null" );

                _isTaken = TimeOut.HasValue
                               ? _manualResetEventSlim.Wait( TimeOut.Value,    token )
                               : _manualResetEventSlim.Wait( Timeout.Infinite, token );

                return new Closer( this );
            }

            case Type.Barrier:
            {
                Debug.Assert( _barrier is not null, $"{nameof(_barrier)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _barrier.SignalAndWait( TimeOut.Value,    token )
                               : _barrier.SignalAndWait( Timeout.Infinite, token );

                return new Closer( this );
            }

            case Type.CountdownEvent:
            {
                Debug.Assert( _countdownEvent is not null, $"{nameof(_countdownEvent)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _countdownEvent.Wait( TimeOut.Value,    token )
                               : _countdownEvent.Wait( Timeout.Infinite, token );

                return new Closer( this );
            }

            default: throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" );
        }
    }
    public async ValueTask<Closer> EnterAsync( CancellationToken token = default )
    {
        switch ( _index )
        {
            case Type.Object:
            {
                Monitor.Enter( this, ref _isTaken );
                return new Closer( this );
            }

            case Type.SemaphoreSlim:
            {
                Debug.Assert( _semaphoreSlim is not null, $"{nameof(_semaphoreSlim)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? await _semaphoreSlim.WaitAsync( TimeOut.Value,    token )
                               : await _semaphoreSlim.WaitAsync( Timeout.Infinite, token );

                return new Closer( this );
            }

            case Type.Semaphore:
            {
                Debug.Assert( _semaphore is not null, $"{nameof(_semaphore)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _semaphore.WaitOne( TimeOut.Value )
                               : _semaphore.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.ReaderWriterLockSlim:
            {
                Debug.Assert( _readerWriterLockSlim is not null, $"{nameof(_readerWriterLockSlim)} is not null" );
                _readerWriterLockSlim.EnterWriteLock();
                _isTaken = true;
                return new Closer( this );
            }

            case Type.Mutex:
            {
                Debug.Assert( _mutex is not null, $"{nameof(_mutex)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _mutex.WaitOne( TimeOut.Value )
                               : _mutex.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.SpinLock:
            {
                Debug.Assert( _spinLock is not null, $"{nameof(_spinLock)} is not null" );
                _spinLock.Value.Enter( ref _isTaken );
                return new Closer( this );
            }

            case Type.EventWaitHandle:
            {
                Debug.Assert( _eventWaitHandle is not null, $"{nameof(_eventWaitHandle)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _eventWaitHandle.WaitOne( TimeOut.Value )
                               : _eventWaitHandle.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.AutoResetEvent:
            {
                Debug.Assert( _autoResetEvent is not null, $"{nameof(_autoResetEvent)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _autoResetEvent.WaitOne( TimeOut.Value )
                               : _autoResetEvent.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.ManualResetEvent:
            {
                Debug.Assert( _manualResetEvent is not null, $"{nameof(_manualResetEvent)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _manualResetEvent.WaitOne( TimeOut.Value )
                               : _manualResetEvent.WaitOne( Timeout.Infinite );

                return new Closer( this );
            }

            case Type.ManualResetEventSlim:
            {
                Debug.Assert( _manualResetEventSlim is not null, $"{nameof(_manualResetEventSlim)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _manualResetEventSlim.Wait( TimeOut.Value,    token )
                               : _manualResetEventSlim.Wait( Timeout.Infinite, token );

                return new Closer( this );
            }

            case Type.Barrier:
            {
                Debug.Assert( _barrier is not null, $"{nameof(_barrier)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _barrier.SignalAndWait( TimeOut.Value,    token )
                               : _barrier.SignalAndWait( Timeout.Infinite, token );

                return new Closer( this );
            }

            case Type.CountdownEvent:
            {
                Debug.Assert( _countdownEvent is not null, $"{nameof(_countdownEvent)} is not null" );

                _isTaken = TimeOut.HasValue
                               ? _countdownEvent.Wait( TimeOut.Value,    token )
                               : _countdownEvent.Wait( Timeout.Infinite, token );

                return new Closer( this );
            }

            default: throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" );
        }
    }


    public void Exit()
    {
        _isTaken = false;

        switch ( _index )
        {
            case Type.Object:
            {
                Monitor.Exit( this );
                Monitor.PulseAll( this );
                return;
            }

            case Type.SemaphoreSlim:
                Debug.Assert( _semaphoreSlim is not null, nameof(_semaphoreSlim) + " is not null" );
                _semaphoreSlim.Release();
                return;

            case Type.Semaphore:
                Debug.Assert( _semaphore is not null, nameof(_semaphore) + " is not null" );
                _semaphore.Release();
                return;

            case Type.ReaderWriterLockSlim:
                Debug.Assert( _readerWriterLockSlim is not null, nameof(_readerWriterLockSlim) + " is not null" );
                _readerWriterLockSlim.ExitWriteLock();
                return;

            case Type.Mutex:
                Debug.Assert( _mutex is not null, nameof(_mutex) + " is not null" );
                _mutex.ReleaseMutex();
                return;

            case Type.SpinLock:
                Debug.Assert( _spinLock is not null, nameof(_spinLock) + " is not null" );
                _spinLock.Value.Exit();
                return;

            case Type.EventWaitHandle:
                Debug.Assert( _eventWaitHandle is not null, nameof(_eventWaitHandle) + " is not null" );
                _eventWaitHandle.Set();
                return;

            case Type.AutoResetEvent:
                Debug.Assert( _autoResetEvent is not null, nameof(_autoResetEvent) + " is not null" );
                _autoResetEvent.Set();
                return;

            case Type.ManualResetEvent:
                Debug.Assert( _manualResetEvent is not null, nameof(_manualResetEvent) + " is not null" );
                _manualResetEvent.Set();
                return;

            case Type.ManualResetEventSlim:
                Debug.Assert( _manualResetEventSlim is not null, nameof(_manualResetEventSlim) + " is not null" );
                _manualResetEventSlim.Set();
                return;

            case Type.Barrier:
                Debug.Assert( _barrier is not null, nameof(_barrier) + " is not null" );
                _barrier.SignalAndWait();
                return;

            case Type.CountdownEvent:
                Debug.Assert( _countdownEvent is not null, nameof(_countdownEvent) + " is not null" );
                _countdownEvent.Reset();
                return;

            default: throw new OutOfRangeException( _index );
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
            Type.Object               => base.ToString(),
            Type.SemaphoreSlim        => _semaphoreSlim?.ToString(),
            Type.Semaphore            => _semaphore?.ToString(),
            Type.ReaderWriterLockSlim => _readerWriterLockSlim?.ToString(),
            Type.Mutex                => _mutex?.ToString(),
            Type.SpinLock             => _spinLock?.ToString(),
            Type.EventWaitHandle      => _eventWaitHandle?.ToString(),
            Type.AutoResetEvent       => _autoResetEvent?.ToString(),
            Type.ManualResetEvent     => _manualResetEvent?.ToString(),
            Type.ManualResetEventSlim => _manualResetEventSlim?.ToString(),
            Type.Barrier              => _barrier?.ToString(),
            Type.CountdownEvent       => _countdownEvent?.ToString(),
            _                         => throw new OutOfRangeException( _index )
        } ??
        string.Empty;
    public override int GetHashCode()
    {
        int? nullable = _index switch
                        {
                            Type.SemaphoreSlim        => _semaphoreSlim?.GetHashCode(),
                            Type.Semaphore            => _semaphore?.GetHashCode(),
                            Type.ReaderWriterLockSlim => _readerWriterLockSlim?.GetHashCode(),
                            Type.Mutex                => _mutex?.GetHashCode(),
                            Type.SpinLock             => _spinLock?.GetHashCode(),
                            Type.EventWaitHandle      => _eventWaitHandle?.GetHashCode(),
                            Type.AutoResetEvent       => _autoResetEvent?.GetHashCode(),
                            Type.ManualResetEvent     => _manualResetEvent?.GetHashCode(),
                            Type.ManualResetEventSlim => _manualResetEventSlim?.GetHashCode(),
                            Type.Barrier              => _barrier?.GetHashCode(),
                            Type.CountdownEvent       => _countdownEvent?.GetHashCode(),
                            _                         => null
                        };

        return (nullable.GetValueOrDefault( 0 ) * 397) ^ _index.GetHashCode();
    }
    public override bool Equals( object? other ) => Equals( other as Locker );
    public bool Equals( Locker? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        if ( _index != other._index ) { return false; }

        bool check = _index switch
                     {
                         Type.SemaphoreSlim        => Equals( _semaphoreSlim,        other._semaphoreSlim ),
                         Type.Semaphore            => Equals( _semaphore,            other._semaphore ),
                         Type.ReaderWriterLockSlim => Equals( _readerWriterLockSlim, other._readerWriterLockSlim ),
                         Type.Mutex                => Equals( _mutex,                other._mutex ),
                         Type.SpinLock             => Nullable.Equals( _spinLock, other._spinLock ),
                         Type.EventWaitHandle      => Equals( _eventWaitHandle,      other._eventWaitHandle ),
                         Type.AutoResetEvent       => Equals( _autoResetEvent,       other._autoResetEvent ),
                         Type.ManualResetEvent     => Equals( _manualResetEvent,     other._manualResetEvent ),
                         Type.ManualResetEventSlim => Equals( _manualResetEventSlim, other._manualResetEventSlim ),
                         Type.Barrier              => Equals( _barrier,              other._barrier ),
                         Type.CountdownEvent       => Equals( _countdownEvent,       other._countdownEvent ),
                         _                         => false
                     };

        return check;
    }



    private enum Type : byte
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
        CountdownEvent
    }
}
