namespace Jakar.Extensions;


public class ConcurrentDeque<T>( int capacity, Lock? locker = null ) : IQueue<T>
{
    protected readonly Deque<T> _queue = new(capacity);
    protected readonly Lock     _lock  = locker ?? new Lock();


    public int Count
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            lock (_lock) { return _queue.Count; }
        }
    }
    public bool IsEmpty
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            lock (_lock) { return _queue.IsEmpty(); }
        }
    }
    public T Next
    {
        get
        {
            lock (_lock) { return _queue[^1]; }
        }
    }


    public ConcurrentDeque( Lock? locker = null ) : this( DEFAULT_CAPACITY, locker ) { }
    public ConcurrentDeque( IEnumerable<T> enumerable, Lock? locker = null ) : this( DEFAULT_CAPACITY, locker )
    {
        foreach ( T x in enumerable ) { _queue.AddToBack( x ); }
    }


    public virtual void Add( T value )
    {
        lock (_lock) { _queue.AddToBack( value ); }
    }
    public virtual ValueTask AddAsync( T value )
    {
        lock (_lock) { _queue.AddToBack( value ); }

        return ValueTask.CompletedTask;
    }


    public virtual bool Remove( [NotNullWhen( true )] out T? value )
    {
        lock (_lock)
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
    public virtual ValueTask<T?> RemoveAsync()
    {
        T? value = default;

        lock (_lock)
        {
            if ( _queue.Count > 0 )
            {
                T t = _queue.RemoveFromBack();

                if ( t is not null ) { value = t; }
            }
        }

        return ValueTask.FromResult( value );
    }


    public virtual void Clear()
    {
        lock (_lock) { _queue.Clear(); }
    }
    public virtual ValueTask ClearAsync()
    {
        lock (_lock) { _queue.Clear(); }

        return ValueTask.CompletedTask;
    }


    public virtual bool Contains( T obj )
    {
        lock (_lock) { return _queue.Contains( obj ); }
    }
    public virtual ValueTask<bool> ContainsAsync( T obj )
    {
        lock (_lock) { return ValueTask.FromResult( _queue.Contains( obj ) ); }
    }


    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock) { return _queue.GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
