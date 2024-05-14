namespace Jakar.Extensions;


public class ConcurrentDeque<T>( int capacity, Locker? locker = null ) : IQueue<T>
{
    protected readonly Deque<T> _queue = new(capacity);
    protected readonly Locker   _lock  = locker ?? Locker.Default;


    public int  Count   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _queue.Count; }
    public bool IsEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _queue.IsEmpty(); }
    public T Next
    {
        get
        {
            using ( _lock.Enter() ) { return _queue[^1]; }
        }
    }


    public ConcurrentDeque( Locker? locker = null ) : this( 10, locker ) { }
    public ConcurrentDeque( IEnumerable<T> enumerable, Locker? locker = null ) : this( 10, locker )
    {
        foreach ( T x in enumerable ) { _queue.AddToBack( x ); }
    }


    public virtual void Add( T value )
    {
        using ( _lock.Enter() ) { _queue.AddToBack( value ); }
    }
    public virtual async ValueTask AddAsync( T value )
    {
        using ( await _lock.EnterAsync().ConfigureAwait( false ) ) { _queue.AddToBack( value ); }
    }


    public virtual bool Remove( [NotNullWhen( true )] out T? value )
    {
        using ( _lock.Enter() )
        {
            if ( _queue.Count > 0 )
            {
                T t = _queue.RemoveFromBack();

                if ( t is not null )
                {
                    value = t;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
    public virtual async ValueTask<T?> RemoveAsync()
    {
        using ( await _lock.EnterAsync().ConfigureAwait( false ) )
        {
            if ( _queue.Count <= 0 ) { return default; }

            T t = _queue.RemoveFromBack();

            if ( t is not null ) { return t; }
        }

        return default;
    }


    public virtual void Clear()
    {
        using ( _lock.Enter() ) { _queue.Clear(); }
    }
    public virtual async ValueTask ClearAsync()
    {
        using ( await _lock.EnterAsync().ConfigureAwait( false ) ) { _queue.Clear(); }
    }


    public virtual bool Contains( T obj )
    {
        using ( _lock.Enter() ) { return _queue.Contains( obj ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( T obj )
    {
        using ( await _lock.EnterAsync().ConfigureAwait( false ) ) { return _queue.Contains( obj ); }
    }


    public IEnumerator<T> GetEnumerator()
    {
        using ( _lock.Enter() ) { return _queue.GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
