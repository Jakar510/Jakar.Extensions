// Jakar.Extensions :: Jakar.Extensions
// 04/10/2022  6:24 PM

namespace Jakar.Extensions;


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).



public sealed partial class ObservableConcurrentDictionary<TKey, TValue> : ObservableConcurrentDictionary<ObservableConcurrentDictionary<TKey, TValue>, TKey, TValue>, ICollectionAlerts<ObservableConcurrentDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public ObservableConcurrentDictionary() : this(DEFAULT_CAPACITY) { }
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>>         collection ) : this() => Add(collection);
    public ObservableConcurrentDictionary( params ReadOnlySpan<KeyValuePair<TKey, TValue>> collection ) : this() => Add(collection);
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>                       dictionary ) : this(new ConcurrentDictionary<TKey, TValue>(dictionary)) { }
    public ObservableConcurrentDictionary( int                                             capacity ) : this(new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, capacity, EqualityComparer<TKey>.Default)) { }
    private ObservableConcurrentDictionary( ConcurrentDictionary<TKey, TValue>             dictionary ) : base(dictionary) { }


    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( List<KeyValuePair<TKey, TValue>>           values ) => new(values);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( HashSet<KeyValuePair<TKey, TValue>>        values ) => new(values);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ConcurrentBag<KeyValuePair<TKey, TValue>>  values ) => new(values);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( Collection<KeyValuePair<TKey, TValue>>     values ) => new(values);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( KeyValuePair<TKey, TValue>[]               values ) => new(values.AsSpan());
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ImmutableArray<KeyValuePair<TKey, TValue>> values ) => new(values.AsSpan());
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ReadOnlyMemory<KeyValuePair<TKey, TValue>> values ) => new(values.Span);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ReadOnlySpan<KeyValuePair<TKey, TValue>>   values ) => new(values);


    public override int  GetHashCode()                                                                                                          => RuntimeHelpers.GetHashCode(this);
    public override bool Equals( object?                                            other )                                                     => ReferenceEquals(this, other) || ( other is ObservableConcurrentDictionary<TKey, TValue> x && Equals(x) );
    public static   bool operator ==( ObservableConcurrentDictionary<TKey, TValue>? left, ObservableConcurrentDictionary<TKey, TValue>? right ) => EqualityComparer<ObservableConcurrentDictionary<TKey, TValue>>.Default.Equals(left, right);
    public static   bool operator !=( ObservableConcurrentDictionary<TKey, TValue>? left, ObservableConcurrentDictionary<TKey, TValue>? right ) => !EqualityComparer<ObservableConcurrentDictionary<TKey, TValue>>.Default.Equals(left, right);
    public static   bool operator >( ObservableConcurrentDictionary<TKey, TValue>   left, ObservableConcurrentDictionary<TKey, TValue>  right ) => Comparer<ObservableConcurrentDictionary<TKey, TValue>>.Default.Compare(left, right) > 0;
    public static   bool operator >=( ObservableConcurrentDictionary<TKey, TValue>  left, ObservableConcurrentDictionary<TKey, TValue>  right ) => Comparer<ObservableConcurrentDictionary<TKey, TValue>>.Default.Compare(left, right) >= 0;
    public static   bool operator <( ObservableConcurrentDictionary<TKey, TValue>   left, ObservableConcurrentDictionary<TKey, TValue>  right ) => Comparer<ObservableConcurrentDictionary<TKey, TValue>>.Default.Compare(left, right) < 0;
    public static   bool operator <=( ObservableConcurrentDictionary<TKey, TValue>  left, ObservableConcurrentDictionary<TKey, TValue>  right ) => Comparer<ObservableConcurrentDictionary<TKey, TValue>>.Default.Compare(left, right) <= 0;
}



public abstract class ObservableConcurrentDictionary<TSelf, TKey, TValue>( ConcurrentDictionary<TKey, TValue> dictionary ) : CollectionAlerts<TSelf, KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TSelf : ObservableConcurrentDictionary<TSelf, TKey, TValue>, ICollectionAlerts<TSelf, KeyValuePair<TKey, TValue>>
{
    protected internal readonly ConcurrentDictionary<TKey, TValue> buffer = dictionary;


    public override        int  Capacity   => buffer.Count;
    public sealed override int  Count      => buffer.Count;
    public                 bool IsReadOnly => ( (IDictionary)buffer ).IsReadOnly;

    public TValue this[ TKey key ]
    {
        get => buffer[key];
        set
        {
            // atomic: the notification always reports the value that was actually replaced
            while ( true )
            {
                if ( buffer.TryGetValue(key, out TValue? old) )
                {
                    if ( !buffer.TryUpdate(key, value, old) ) { continue; }

                    KeyValuePair<TKey, TValue> oldPair = new(key, old);
                    KeyValuePair<TKey, TValue> pair    = new(key, value);
                    Replaced(in oldPair, in pair, -1);
                    return;
                }

                if ( buffer.TryAdd(key, value) )
                {
                    KeyValuePair<TKey, TValue> pair = new(key, value);
                    Added(in pair, -1);
                    return;
                }
            }
        }
    }

    public ICollection<TKey>                              Keys   => buffer.Keys;
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   => buffer.Keys;
    public ICollection<TValue>                            Values => buffer.Values;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => buffer.Values;


    protected ObservableConcurrentDictionary() : this(DEFAULT_CAPACITY) { }
    protected ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>>         collection ) : this() => Add(collection);
    protected ObservableConcurrentDictionary( params ReadOnlySpan<KeyValuePair<TKey, TValue>> collection ) : this() => Add(collection);
    protected ObservableConcurrentDictionary( IDictionary<TKey, TValue>                       dictionary ) : this(new ConcurrentDictionary<TKey, TValue>(dictionary)) { }
    protected ObservableConcurrentDictionary( int                                             capacity ) : this(new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, capacity, EqualityComparer<TKey>.Default)) { }


    public void Clear()
    {
        buffer.Clear();
        Reset();
    }


    public bool TryAdd( KeyValuePair<TKey, TValue> pair ) => TryAdd(pair.Key, pair.Value);
    public bool TryAdd( TKey key, TValue value )
    {
        if ( !buffer.TryAdd(key, value) ) { return false; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Added(in pair, -1);
        return true;
    }


    public void Add( KeyValuePair<TKey, TValue> item )              => TryAdd(item);
    public void Add( TKey                       key, TValue value ) => TryAdd(key, value);
    public void Add( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add(pair); }
    }
    public void Add( params ReadOnlySpan<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add(pair); }
    }


    public bool TryGetValue( TKey key, [NotNullWhen(true)] out TValue? value ) => buffer.TryGetValue(key, out value) && value is not null;


    /// <remarks> Lock free, without the snapshot of <see cref="ConcurrentDictionary{TKey,TValue}.Values"/>. </remarks>
    public bool ContainsValue( TValue value )
    {
        EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;

        foreach ( KeyValuePair<TKey, TValue> pair in buffer )
        {
            if ( comparer.Equals(pair.Value, value) ) { return true; }
        }

        return false;
    }
    public bool ContainsKey( TKey                    key )  => buffer.ContainsKey(key);
    public bool Contains( KeyValuePair<TKey, TValue> item ) => buffer.TryGetValue(item.Key, out TValue? x) && EqualityComparer<TValue>.Default.Equals(x, item.Value);


    /// <summary> Removes <paramref name="item"/> only when both its key and value match (atomically). </summary>
    public bool Remove( KeyValuePair<TKey, TValue> item )
    {
        if ( !buffer.TryRemove(item) ) { return false; }

        Removed(in item, -1);
        return true;
    }
    public bool Remove( TKey key )
    {
        if ( !buffer.TryRemove(key, out TValue? value) ) { return false; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Removed(in pair, -1);
        return true;
    }


    /// <remarks> An atomic snapshot. </remarks>
    public void CopyTo( KeyValuePair<TKey, TValue>[] array, int startIndex ) => ( (ICollection<KeyValuePair<TKey, TValue>>)buffer ).CopyTo(array, startIndex);
    /// <remarks> Not atomic: items added or removed while copying may or may not be included. </remarks>
    public void CopyTo( Span<KeyValuePair<TKey, TValue>> array, int startIndex )
    {
        Span<KeyValuePair<TKey, TValue>> destination = array[startIndex..];
        int                              index       = 0;

        foreach ( KeyValuePair<TKey, TValue> pair in buffer ) { destination[index++] = pair; }
    }


    [Pure] [MustDisposeResource] [SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
    protected internal override ArrayBuffer<KeyValuePair<TKey, TValue>> FilteredValues()
    {
        // items may be added while enumerating, so the buffer grows when needed (previously it overflowed)
        ArrayBuffer<KeyValuePair<TKey, TValue>>     values = new(buffer.Count + 4);
        FilterDelegate<KeyValuePair<TKey, TValue>>? filter = HasFilter
                                                                 ? GetFilter()
                                                                 : null;

        int index = 0;

        foreach ( KeyValuePair<TKey, TValue> pair in buffer )
        {
            if ( filter is not null && !filter(index++, in pair) ) { continue; }

            if ( values.Length == values.Capacity )
            {
                ArrayBuffer<KeyValuePair<TKey, TValue>> larger = new(values.Capacity * 2);
                larger.Add(values.Values);
                values.Dispose();
                values = larger;
            }

            values.Add(in pair);
        }

        return values;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
