namespace Jakar.Extensions;


public class ConcurrentDeque<TValue> : IQueue<TValue>
{
    protected readonly Deque<TValue> _queue;
#if NET8_0
    protected readonly object _lock;
#else
    protected readonly Lock _lock;
#endif


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
    public TValue Next
    {
        get
        {
            lock (_lock) { return _queue[^1]; }
        }
    }

    public ConcurrentDeque() : this( null ) { }
    public ConcurrentDeque( IEnumerable<TValue>? enumerable,
                            int                  capacity = DEFAULT_CAPACITY,
                        #if NET8_0
                            object? locker = null
                        #else
                            Lock? locker = null
#endif
    )
    {
        _queue = new Deque<TValue>( capacity );

        _lock = locker ??
            #if NET8_0
                new object();
            #else
                new Lock();
    #endif

        if ( enumerable is not null )
        {
            foreach ( TValue x in enumerable ) { _queue.AddToBack( x ); }
        }
    }


    public virtual void Add( TValue value )
    {
        lock (_lock) { _queue.AddToBack( value ); }
    }
    public virtual ValueTask AddAsync( TValue value )
    {
        lock (_lock) { _queue.AddToBack( value ); }

        return ValueTask.CompletedTask;
    }


    public virtual bool Remove( [NotNullWhen( true )] out TValue? value )
    {
        lock (_lock)
        {
            if ( _queue.Count > 0 )
            {
                TValue t = _queue.RemoveFromBack();

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
    public virtual ValueTask<TValue?> RemoveAsync()
    {
        TValue? value = default;

        lock (_lock)
        {
            if ( _queue.Count > 0 )
            {
                TValue t = _queue.RemoveFromBack();

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


    public virtual bool Contains( TValue obj )
    {
        lock (_lock) { return _queue.Contains( obj ); }
    }
    public virtual ValueTask<bool> ContainsAsync( TValue obj )
    {
        lock (_lock) { return ValueTask.FromResult( _queue.Contains( obj ) ); }
    }


    public IEnumerator<TValue> GetEnumerator()
    {
        lock (_lock) { return _queue.GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
