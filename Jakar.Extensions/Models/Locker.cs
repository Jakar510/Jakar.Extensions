// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  9:29 AM


// using LockerContext =
//     OneOf.OneOf<object, System.Threading.SemaphoreSlim, System.Threading.Semaphore, System.Threading.ReaderWriterLockSlim, System.Threading.Mutex, System.Threading.SpinLock, System.Threading.EventWaitHandle, System.Threading.AutoResetEvent,
//         System.Threading.ManualResetEvent, System.Threading.ManualResetEventSlim, System.Threading.Barrier, System.Threading.CountdownEvent>;


namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" ) ]
public readonly struct LockContext : IEquatable<LockContext>, IAsyncDisposable, IDisposable
{
    private readonly object?               _value0  = null;
    private readonly SemaphoreSlim?        _value1  = null;
    private readonly Semaphore?            _value2  = null;
    private readonly ReaderWriterLockSlim? _value3  = null;
    private readonly Mutex?                _value4  = null;
    private readonly SpinLock?             _value5  = null;
    private readonly EventWaitHandle?      _value6  = null;
    private readonly AutoResetEvent?       _value7  = null;
    private readonly ManualResetEvent?     _value8  = null;
    private readonly ManualResetEventSlim? _value9  = null;
    private readonly Barrier?              _value10 = null;
    private readonly CountdownEvent?       _value11 = null;


    public int  Index                  { get; }
    public bool IsObject               => Index == 0;
    public bool IsSemaphoreSlim        => Index == 1;
    public bool IsSemaphore            => Index == 2;
    public bool IsReaderWriterLockSlim => Index == 3;
    public bool IsMutex                => Index == 4;
    public bool IsSpinLock             => Index == 5;
    public bool IsEventWaitHandle      => Index == 6;
    public bool IsAutoResetEvent       => Index == 7;
    public bool IsManualResetEvent     => Index == 8;
    public bool IsManualResetEventSlim => Index == 9;
    public bool IsSemaphoreSlim0       => Index == 10;
    public bool IsSemaphoreSlim1       => Index == 11;


    public object AsObject =>
        Index != 0
            ? throw new InvalidOperationException( $"Cannot return as object as result is T{Index}" )
            : _value0!;

    public SemaphoreSlim AsSemaphoreSlim =>
        Index != 1
            ? throw new InvalidOperationException( $"Cannot return as SemaphoreSlim as result is T{Index}" )
            : _value1!;

    public Semaphore AsSemaphore =>
        Index != 2
            ? throw new InvalidOperationException( $"Cannot return as Semaphore as result is T{Index}" )
            : _value2!;

    public ReaderWriterLockSlim AsReaderWriterLockSlim =>
        Index != 3
            ? throw new InvalidOperationException( $"Cannot return as ReaderWriterLockSlim as result is T{Index}" )
            : _value3!;

    public Mutex AsMutex =>
        Index != 4
            ? throw new InvalidOperationException( $"Cannot return as Mutex as result is T{Index}" )
            : _value4!;

    public SpinLock AsSpinLock =>
        Index != 5
            ? throw new InvalidOperationException( $"Cannot return as SpinLock as result is T{Index}" )
            : _value5!.Value;

    public EventWaitHandle AsEventWaitHandle =>
        Index != 6
            ? throw new InvalidOperationException( $"Cannot return as EventWaitHandle as result is T{Index}" )
            : _value6!;

    public AutoResetEvent AsAutoResetEvent =>
        Index != 7
            ? throw new InvalidOperationException( $"Cannot return as AutoResetEvent as result is T{Index}" )
            : _value7!;

    public ManualResetEvent AsManualResetEvent =>
        Index != 8
            ? throw new InvalidOperationException( $"Cannot return as ManualResetEvent as result is T{Index}" )
            : _value8!;

    public ManualResetEventSlim AsManualResetEventSlim =>
        Index != 9
            ? throw new InvalidOperationException( $"Cannot return as ManualResetEventSlim as result is T{Index}" )
            : _value9!;

    public Barrier AsSemaphoreSlim0 =>
        Index != 10
            ? throw new InvalidOperationException( $"Cannot return as Barrier as result is T{Index}" )
            : _value10!;

    public CountdownEvent AsSemaphoreSlim1 =>
        Index != 11
            ? throw new InvalidOperationException( $"Cannot return as CountdownEvent as result is T{Index}" )
            : _value11!;


    public LockContext() : this( new SemaphoreSlim( 1 ) ) { }
    public LockContext( object value )
    {
        Index   = 0;
        _value0 = value;
    }
    public LockContext( SemaphoreSlim value )
    {
        Index   = 1;
        _value1 = value;
    }
    public LockContext( Semaphore value )
    {
        Index   = 2;
        _value2 = value;
    }
    public LockContext( ReaderWriterLockSlim value )
    {
        Index   = 3;
        _value3 = value;
    }
    public LockContext( Mutex value )
    {
        Index   = 4;
        _value4 = value;
    }
    public LockContext( SpinLock value )
    {
        Index   = 5;
        _value5 = value;
    }
    public LockContext( EventWaitHandle value )
    {
        Index   = 6;
        _value6 = value;
    }
    public LockContext( AutoResetEvent value )
    {
        Index   = 7;
        _value7 = value;
    }
    public LockContext( ManualResetEvent value )
    {
        Index   = 8;
        _value8 = value;
    }
    public LockContext( ManualResetEventSlim value )
    {
        Index   = 9;
        _value9 = value;
    }
    public LockContext( Barrier value )
    {
        Index    = 10;
        _value10 = value;
    }
    public LockContext( CountdownEvent value )
    {
        Index    = 11;
        _value11 = value;
    }


    public static implicit operator LockContext( SemaphoreSlim        value ) => new(value);
    public static implicit operator LockContext( Semaphore            value ) => new(value);
    public static implicit operator LockContext( ReaderWriterLockSlim value ) => new(value);
    public static implicit operator LockContext( Mutex                value ) => new(value);
    public static implicit operator LockContext( SpinLock             value ) => new(value);
    public static implicit operator LockContext( EventWaitHandle      value ) => new(value);
    public static implicit operator LockContext( AutoResetEvent       value ) => new(value);
    public static implicit operator LockContext( ManualResetEvent     value ) => new(value);
    public static implicit operator LockContext( ManualResetEventSlim value ) => new(value);
    public static implicit operator LockContext( Barrier              value ) => new(value);
    public static implicit operator LockContext( CountdownEvent       value ) => new(value);


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
        if ( Index      == 0  && f0 is not null ) { f0( _value0 ); }
        else if ( Index == 1  && f1 is not null ) { f1( _value1 ); }
        else if ( Index == 2  && f2 is not null ) { f2( _value2 ); }
        else if ( Index == 3  && f3 is not null ) { f3( _value3 ); }
        else if ( Index == 4  && f4 is not null ) { f4( _value4 ); }
        else if ( Index == 5  && f5 is not null ) { f5( _value5 ); }
        else if ( Index == 6  && f6 is not null ) { f6( _value6 ); }
        else if ( Index == 7  && f7 is not null ) { f7( _value7 ); }
        else if ( Index == 8  && f8 is not null ) { f8( _value8 ); }
        else if ( Index == 9  && f9 is not null ) { f9( _value9 ); }
        else if ( Index == 10 && f10 is not null ) { f10( _value10 ); }
        else
        {
            if ( Index != 11 || f11 == default ) { throw new InvalidOperationException(); }

            f11( _value11 );
        }
    }
    */

    public bool Equals( LockContext other )
    {
        bool flag1 = Index == other.Index;

        if ( flag1 )
        {
            bool flag2 = Index switch
                         {
                             0  => Equals( _value0, other._value0 ),
                             1  => Equals( _value1, other._value1 ),
                             2  => Equals( _value2, other._value2 ),
                             3  => Equals( _value3, other._value3 ),
                             4  => Equals( _value4, other._value4 ),
                             5  => Nullable.Equals( _value5, other._value5 ),
                             6  => Equals( _value6,  other._value6 ),
                             7  => Equals( _value7,  other._value7 ),
                             8  => Equals( _value8,  other._value8 ),
                             9  => Equals( _value9,  other._value9 ),
                             10 => Equals( _value10, other._value10 ),
                             11 => Equals( _value11, other._value11 ),
                             _  => false
                         };

            flag1 = flag2;
        }

        return flag1;
    }


    public override string ToString() =>
        Index switch
        {
            0  => _value0?.ToString(),
            1  => _value1?.ToString(),
            2  => _value2?.ToString(),
            3  => _value3?.ToString(),
            4  => _value4?.ToString(),
            5  => _value5?.ToString(),
            6  => _value6?.ToString(),
            7  => _value7?.ToString(),
            8  => _value8?.ToString(),
            9  => _value9?.ToString(),
            10 => _value10?.ToString(),
            11 => _value11?.ToString(),
            _  => throw new OutOfRangeException( nameof(Index), Index )
        } ??
        string.Empty;

    public override int GetHashCode()
    {
        int? nullable = Index switch
                        {
                            0  => _value0?.GetHashCode(),
                            1  => _value1?.GetHashCode(),
                            2  => _value2?.GetHashCode(),
                            3  => _value3?.GetHashCode(),
                            4  => _value4?.GetHashCode(),
                            5  => _value5?.GetHashCode(),
                            6  => _value6?.GetHashCode(),
                            7  => _value7?.GetHashCode(),
                            8  => _value8?.GetHashCode(),
                            9  => _value9?.GetHashCode(),
                            10 => _value10?.GetHashCode(),
                            11 => _value11?.GetHashCode(),
                            _  => 0
                        };

        return (nullable.GetValueOrDefault() * 397) ^ Index;
    }


    public void Dispose()
    {
        _value1?.Dispose();
        _value2?.Dispose();
        _value3?.Dispose();
        _value4?.Dispose();
        _value6?.Dispose();
        _value7?.Dispose();
        _value8?.Dispose();
        _value9?.Dispose();
        _value10?.Dispose();
        _value11?.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        if ( _value1 is not null ) { await CastAndDispose( _value1 ); }

        if ( _value2 is not null ) { await CastAndDispose( _value2 ); }

        if ( _value3 is not null ) { await CastAndDispose( _value3 ); }

        if ( _value4 is not null ) { await CastAndDispose( _value4 ); }

        if ( _value6 is not null ) { await CastAndDispose( _value6 ); }

        if ( _value7 is not null ) { await CastAndDispose( _value7 ); }

        if ( _value8 is not null ) { await CastAndDispose( _value8 ); }

        if ( _value9 is not null ) { await CastAndDispose( _value9 ); }

        if ( _value10 is not null ) { await CastAndDispose( _value10 ); }

        if ( _value11 is not null ) { await CastAndDispose( _value11 ); }

        return;

        static async ValueTask CastAndDispose( IDisposable resource )
        {
            if ( resource is IAsyncDisposable asyncDisposable ) { await asyncDisposable.DisposeAsync(); }
            else { resource.Dispose(); }
        }
    }
}



[ SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" ) ]
public sealed class Locker : IAsyncDisposable, IDisposable
{
    private readonly LockContext _context;
    private          bool        _lockTaken;


    public bool      IsTaken => _lockTaken;
    public TimeSpan? TimeOut { get; init; }


    public Locker( LockContext context ) => _context = context;


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


    public Context Enter( CancellationToken token = default )
    {
        if ( _context.IsObject ) { Monitor.Enter( _context.AsMutex, ref _lockTaken ); }

        else if ( _context.IsSemaphoreSlim )
        {
            SemaphoreSlim value = _context.AsSemaphoreSlim;

            _lockTaken = TimeOut.HasValue
                             ? value.Wait( TimeOut.Value,    token )
                             : value.Wait( Timeout.Infinite, token );
        }

        else if ( _context.IsSemaphore )
        {
            Semaphore value = _context.AsSemaphore;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsReaderWriterLockSlim )
        {
            _context.AsReaderWriterLockSlim.EnterWriteLock();
            _lockTaken = true;
        }

        else if ( _context.IsMutex )
        {
            Mutex value = _context.AsMutex;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsSpinLock ) { _context.AsSpinLock.Enter( ref _lockTaken ); }

        else if ( _context.IsEventWaitHandle )
        {
            AutoResetEvent value = _context.AsAutoResetEvent;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsAutoResetEvent )
        {
            AutoResetEvent value = _context.AsAutoResetEvent;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsManualResetEvent )
        {
            ManualResetEvent value = _context.AsManualResetEvent;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsManualResetEventSlim )
        {
            ManualResetEventSlim value = _context.AsManualResetEventSlim;

            _lockTaken = TimeOut.HasValue
                             ? value.Wait( TimeOut.Value,    token )
                             : value.Wait( Timeout.Infinite, token );
        }

        else if ( _context.IsSemaphoreSlim0 )
        {
            Barrier value = _context.AsSemaphoreSlim0;

            _lockTaken = TimeOut.HasValue
                             ? value.SignalAndWait( TimeOut.Value,    token )
                             : value.SignalAndWait( Timeout.Infinite, token );
        }

        else if ( _context.IsSemaphoreSlim1 )
        {
            CountdownEvent value = _context.AsSemaphoreSlim1;

            _lockTaken = TimeOut.HasValue
                             ? value.Wait( TimeOut.Value,    token )
                             : value.Wait( Timeout.Infinite, token );
        }

        else { throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" ); }

        // Interlocked.Exchange( ref _lockTaken, 1 );
        return this;
    }
    public async ValueTask<Context> EnterAsync( CancellationToken token = default )
    {
        if ( _context.IsObject ) { Monitor.Enter( _context.AsMutex, ref _lockTaken ); }

        else if ( _context.IsSemaphoreSlim )
        {
            SemaphoreSlim value = _context.AsSemaphoreSlim;

            _lockTaken = TimeOut.HasValue
                             ? await value.WaitAsync( TimeOut.Value,    token )
                             : await value.WaitAsync( Timeout.Infinite, token );
        }

        else if ( _context.IsSemaphore )
        {
            Semaphore value = _context.AsSemaphore;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsReaderWriterLockSlim )
        {
            _context.AsReaderWriterLockSlim.EnterWriteLock();
            _lockTaken = true;
        }

        else if ( _context.IsMutex )
        {
            Mutex value = _context.AsMutex;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsSpinLock ) { _context.AsSpinLock.Enter( ref _lockTaken ); }

        else if ( _context.IsEventWaitHandle )
        {
            AutoResetEvent value = _context.AsAutoResetEvent;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsAutoResetEvent )
        {
            AutoResetEvent value = _context.AsAutoResetEvent;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsManualResetEvent )
        {
            ManualResetEvent value = _context.AsManualResetEvent;

            _lockTaken = TimeOut.HasValue
                             ? value.WaitOne( TimeOut.Value )
                             : value.WaitOne( Timeout.Infinite );
        }

        else if ( _context.IsManualResetEventSlim )
        {
            ManualResetEventSlim value = _context.AsManualResetEventSlim;

            _lockTaken = TimeOut.HasValue
                             ? value.Wait( TimeOut.Value,    token )
                             : value.Wait( Timeout.Infinite, token );
        }

        else if ( _context.IsSemaphoreSlim0 )
        {
            Barrier value = _context.AsSemaphoreSlim0;

            _lockTaken = TimeOut.HasValue
                             ? value.SignalAndWait( TimeOut.Value,    token )
                             : value.SignalAndWait( Timeout.Infinite, token );
        }

        else if ( _context.IsSemaphoreSlim1 )
        {
            CountdownEvent value = _context.AsSemaphoreSlim1;

            _lockTaken = TimeOut.HasValue
                             ? value.Wait( TimeOut.Value,    token )
                             : value.Wait( Timeout.Infinite, token );
        }

        else { throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" ); }

        // Interlocked.Exchange( ref _lockTaken, 1 );
        return this;
    }
    public void Exit()
    {
        if ( _context.IsObject )
        {
            Mutex value = _context.AsMutex;
            Monitor.Exit( value );
            Monitor.PulseAll( value );
        }

        else if ( _context.IsSemaphoreSlim ) { _context.AsSemaphoreSlim.Release(); }

        else if ( _context.IsSemaphore ) { _context.AsSemaphore.Release(); }

        else if ( _context.IsReaderWriterLockSlim ) { _context.AsReaderWriterLockSlim.ExitWriteLock(); }

        else if ( _context.IsMutex ) { _context.AsMutex.ReleaseMutex(); }

        else if ( _context.IsSpinLock ) { _context.AsSpinLock.Exit(); }

        else if ( _context.IsEventWaitHandle ) { _context.AsEventWaitHandle.Set(); }

        else if ( _context.IsAutoResetEvent ) { _context.AsAutoResetEvent.Set(); }

        else if ( _context.IsManualResetEvent ) { _context.AsManualResetEvent.Set(); }

        else if ( _context.IsManualResetEventSlim ) { _context.AsManualResetEventSlim.Set(); }

        else if ( _context.IsSemaphoreSlim0 ) { _context.AsSemaphoreSlim0.SignalAndWait(); }

        else if ( _context.IsSemaphoreSlim1 ) { _context.AsSemaphoreSlim1.Reset(); }

        else { throw new InvalidOperationException( $"{nameof(Locker)} is not initialized" ); }
    }


    public void Dispose()
    {
        Exit();
        _context.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        Exit();
        await _context.DisposeAsync();
    }



    public readonly struct Context : IDisposable
    {
        private readonly Locker _locker;
        public Context( Locker                          locker ) => _locker = locker;
        public static implicit operator Context( Locker value ) => new(value);

        public void Dispose() => _locker.Exit();
    }
}



public sealed class Locker<T> : IEnumerator<T>, IAsyncEnumerator<T>
{
    private readonly IAsyncEnumerator<T>? _asyncEnumerator;
    private readonly IEnumerator<T>?      _enumerator;
    private readonly Locker               _locker;


    public T Current => _enumerator is not null
                            ? _enumerator.Current
                            : _asyncEnumerator is not null
                                ? _asyncEnumerator.Current
                                : throw new InvalidOperationException();

    object? IEnumerator.Current => Current;


    public Locker( IEnumerable<T> collection, Locker? locker = default )
    {
        _enumerator = collection.GetEnumerator();
        _locker     = locker ?? new Locker( new LockContext() );
    }
    public Locker( IAsyncEnumerable<T> collection, Locker? locker = default )
    {
        _asyncEnumerator = collection.GetAsyncEnumerator();
        _locker          = locker ?? new Locker( new LockContext() );
    }


    public       Locker.Context            Enter( CancellationToken      token = default ) => _locker.Enter( token );
    public async ValueTask<Locker.Context> EnterAsync( CancellationToken token = default ) => await _locker.EnterAsync( token );
    public       void                      Exit()                                          => _locker.Exit();


    public void Dispose() => DisposeAsync().WaitSynchronously();
    public async ValueTask DisposeAsync()
    {
        await _locker.DisposeAsync();
        _enumerator?.Dispose();
        if ( _asyncEnumerator is not null ) { await _asyncEnumerator.DisposeAsync(); }
    }


    public bool MoveNext()
    {
        Debug.Assert( _enumerator is not null );
        return _enumerator.MoveNext();
    }
    public async ValueTask<bool> MoveNextAsync()
    {
        Debug.Assert( _asyncEnumerator is not null );
        return await _asyncEnumerator.MoveNextAsync();
    }
    void IEnumerator.Reset() => _enumerator?.Reset();
}
