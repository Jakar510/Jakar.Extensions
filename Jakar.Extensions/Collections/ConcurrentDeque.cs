namespace Jakar.Extensions;


public class ConcurrentDeque<TValue>( int capacity = DEFAULT_CAPACITY ) : IQueue<TValue>
{
    protected readonly Deque<TValue> _values = new(capacity);
    protected readonly Lock          _lock   = new();


    public int Count
    {
        get
        {
            lock ( _lock ) { return _values.Count; }
        }
    }
    public bool IsEmpty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Count == 0; }
    public TValue Next
    {
        get
        {
            lock ( _lock ) { return _values[^1]; }
        }
    }


    public ConcurrentDeque( params ReadOnlySpan<TValue> values ) : this(values.Length)
    {
        foreach ( TValue x in values ) { _values.AddToBack(x); }
    }


    public virtual void Add( TValue value )
    {
        lock ( _lock ) { _values.AddToBack(value); }
    }
    public virtual ValueTask AddAsync( TValue value )
    {
        lock ( _lock ) { _values.AddToBack(value); }

        return ValueTask.CompletedTask;
    }


    public virtual bool Remove( [NotNullWhen(true)] out TValue? value )
    {
        lock ( _lock )
        {
            if ( _values.Count > 0 )
            {
                TValue x = _values.RemoveFromBack();

                if ( x is not null )
                {
                    value = x;
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

        lock ( _lock )
        {
            if ( _values.Count > 0 )
            {
                TValue t = _values.RemoveFromBack();

                if ( t is not null ) { value = t; }
            }
        }

        return ValueTask.FromResult(value);
    }


    public virtual void Clear()
    {
        lock ( _lock ) { _values.Clear(); }
    }
    public virtual ValueTask ClearAsync()
    {
        lock ( _lock )
        {
            _values.Clear();
            return ValueTask.CompletedTask;
        }
    }


    public virtual bool Contains( TValue obj )
    {
        lock ( _lock ) { return _values.Contains(obj); }
    }
    public virtual ValueTask<bool> ContainsAsync( TValue obj )
    {
        lock ( _lock ) { return ValueTask.FromResult(_values.Contains(obj)); }
    }


    public IEnumerator<TValue> GetEnumerator()
    {
        TValue[] values;
        lock ( _lock ) { values = _values.ToArray(); }

        foreach ( TValue value in values ) { yield return value; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
