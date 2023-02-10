// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  9:29 AM

namespace Jakar.Extensions;


/// <summary>
///     A wrapper around various locking methods.
///     <para>
///         <list type="bullet">
///             <item>
///                 <see cref="ReaderWriterLockSlim"/>
///             </item>
///             <item>
///                 <see cref="SemaphoreSlim"/>
///             </item>
///             <item>
///                 <see cref="Semaphore"/>
///             </item>
///             <item>
///                 <see langword="lock"/>
///             </item>
///         </list>
///     </para>
/// </summary>
[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public sealed class Locker : IAsyncDisposable, IDisposable
{
    private readonly object   _lock;
    public           TimeSpan TimeOut { get; init; }


    public Locker() : this( new object() ) { }
    public Locker( object               locker ) => _lock = locker;
    public Locker( Semaphore            locker ) => _lock = locker;
    public Locker( SemaphoreSlim        locker ) => _lock = locker;
    public Locker( ReaderWriterLockSlim locker ) => _lock = locker;


    public static implicit operator Locker( Semaphore            value ) => new(value);
    public static implicit operator Locker( SemaphoreSlim        value ) => new(value);
    public static implicit operator Locker( ReaderWriterLockSlim value ) => new(value);


    public void Enter( CancellationToken token = default )
    {
        switch ( _lock )
        {
            case ReaderWriterLockSlim value:
                value.EnterReadLock();
                return;

            case SemaphoreSlim value:
                value.Wait( token );
                return;

            case Semaphore value:
                value.WaitOne();
                return;

            default:
                Monitor.Enter( _lock );
                return;
        }
    }
    public async ValueTask EnterAsync( CancellationToken token = default )
    {
        switch ( _lock )
        {
            case ReaderWriterLockSlim value:
                value.EnterWriteLock();
                break;

            case SemaphoreSlim value:
                await value.WaitAsync( token );
                break;

            case Semaphore value:
                value.WaitOne();
                break;

            default:
                Monitor.Enter( _lock );
                break;
        }
    }
    public void Exit()
    {
        switch ( _lock )
        {
            case ReaderWriterLockSlim value:
                value.ExitWriteLock();
                break;

            case SemaphoreSlim value:
                value.Release();
                break;

            case Semaphore locker:
                locker.Release();
                break;

            default:
                Monitor.Exit( _lock );
                Monitor.PulseAll( _lock );
                break;
        }
    }


    public void Dispose()
    {
        Exit();
        (_lock as IDisposable)?.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        Exit();

        switch ( _lock )
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync();
                break;

            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
}



public readonly struct Locker<T> : IEnumerator<T>, IAsyncEnumerator<T>
{
    private readonly Locker               _locker;
    private readonly IEnumerator<T>?      _enumerator      = default;
    private readonly IAsyncEnumerator<T>? _asyncEnumerator = default;


    public T Current
    {
        get
        {
            if ( _enumerator is not null ) { return _enumerator.Current; }

            if ( _asyncEnumerator is not null ) { return _asyncEnumerator.Current; }

            throw new InvalidOperationException();
        }
    }
    object? IEnumerator.Current => Current;


    /// <summary> Creates the collection Locker </summary>
    /// <param name="collection"> </param>
    /// <param name="locker">
    ///     Can be any of the following:
    ///     <para>
    ///         <list type="bullet">
    ///             <item>
    ///                 <see cref="ReaderWriterLockSlim"/>
    ///             </item>
    ///             <item>
    ///                 <see cref="SemaphoreSlim"/>
    ///             </item>
    ///             <item>
    ///                 <see cref="Semaphore"/>
    ///             </item>
    ///             <item>
    ///                 <see cref="object"/>
    ///             </item>
    ///             <item>
    ///                 <see langword="null"/>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </param>
    public Locker( IEnumerable<T> collection, Locker? locker = default )
    {
        _enumerator = collection.GetEnumerator();
        _locker     = locker ?? new Locker();
    }
    public Locker( IAsyncEnumerable<T> collection, Locker? locker = default )
    {
        _asyncEnumerator = collection.GetAsyncEnumerator();
        _locker          = locker ?? new Locker();
    }


    public void Enter( CancellationToken           token = default ) => _locker.Enter( token );
    public ValueTask EnterAsync( CancellationToken token = default ) => _locker.EnterAsync( token );
    public void Exit() => _locker.Exit();


    public void Dispose()
    {
        _locker.Dispose();
        _enumerator?.Dispose();

        _asyncEnumerator?.DisposeAsync()
                         .CallSynchronously();
    }
    public async ValueTask DisposeAsync()
    {
        await _locker.DisposeAsync();
        _enumerator?.Dispose();
        if ( _asyncEnumerator is not null ) { await _asyncEnumerator.DisposeAsync(); }
    }


    public bool MoveNext() => _enumerator?.MoveNext() ?? false;
    public async ValueTask<bool> MoveNextAsync() => _asyncEnumerator is not null && await _asyncEnumerator.MoveNextAsync();
    public void Reset() => _enumerator?.Reset();
}
