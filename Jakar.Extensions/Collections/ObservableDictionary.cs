namespace Jakar.Extensions;


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).



public sealed class ObservableDictionary<TKey, TValue> : ObservableDictionary<ObservableDictionary<TKey, TValue>, TKey, TValue>, ICollectionAlerts<ObservableDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public ObservableDictionary() : this(DEFAULT_CAPACITY) { }
    public ObservableDictionary( IDictionary<TKey, TValue>                       dictionary ) : base(dictionary) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>>         collection ) : base(collection) { }
    public ObservableDictionary( params ReadOnlySpan<KeyValuePair<TKey, TValue>> collection ) : base(collection) { }
    public ObservableDictionary( int                                             capacity ) : base(capacity) { }


    public static implicit operator ObservableDictionary<TKey, TValue>( List<KeyValuePair<TKey, TValue>>           values ) => new(values);
    public static implicit operator ObservableDictionary<TKey, TValue>( HashSet<KeyValuePair<TKey, TValue>>        values ) => new(values);
    public static implicit operator ObservableDictionary<TKey, TValue>( ConcurrentBag<KeyValuePair<TKey, TValue>>  values ) => new(values);
    public static implicit operator ObservableDictionary<TKey, TValue>( Collection<KeyValuePair<TKey, TValue>>     values ) => new(values);
    public static implicit operator ObservableDictionary<TKey, TValue>( KeyValuePair<TKey, TValue>[]               values ) => new(values.AsSpan());
    public static implicit operator ObservableDictionary<TKey, TValue>( ImmutableArray<KeyValuePair<TKey, TValue>> values ) => new(values.AsSpan());
    public static implicit operator ObservableDictionary<TKey, TValue>( ReadOnlyMemory<KeyValuePair<TKey, TValue>> values ) => new(values.Span);
    public static implicit operator ObservableDictionary<TKey, TValue>( ReadOnlySpan<KeyValuePair<TKey, TValue>>   values ) => new(values);


    public override int  GetHashCode()                                                                                      => base.GetHashCode();
    public override bool Equals( object?                                  other )                                           => ReferenceEquals(this, other) || ( other is ObservableDictionary<TKey, TValue> x && Equals(x) );
    public static   bool operator ==( ObservableDictionary<TKey, TValue>? left, ObservableDictionary<TKey, TValue>? right ) => EqualityComparer<ObservableDictionary<TKey, TValue>>.Default.Equals(left, right);
    public static   bool operator !=( ObservableDictionary<TKey, TValue>? left, ObservableDictionary<TKey, TValue>? right ) => !EqualityComparer<ObservableDictionary<TKey, TValue>>.Default.Equals(left, right);
    public static   bool operator >( ObservableDictionary<TKey, TValue>   left, ObservableDictionary<TKey, TValue>  right ) => Comparer<ObservableDictionary<TKey, TValue>>.Default.Compare(left, right) > 0;
    public static   bool operator >=( ObservableDictionary<TKey, TValue>  left, ObservableDictionary<TKey, TValue>  right ) => Comparer<ObservableDictionary<TKey, TValue>>.Default.Compare(left, right) >= 0;
    public static   bool operator <( ObservableDictionary<TKey, TValue>   left, ObservableDictionary<TKey, TValue>  right ) => Comparer<ObservableDictionary<TKey, TValue>>.Default.Compare(left, right) < 0;
    public static   bool operator <=( ObservableDictionary<TKey, TValue>  left, ObservableDictionary<TKey, TValue>  right ) => Comparer<ObservableDictionary<TKey, TValue>>.Default.Compare(left, right) <= 0;
}



public abstract class ObservableDictionary<TSelf, TKey, TValue>( Dictionary<TKey, TValue> dictionary ) : CollectionAlerts<TSelf, KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TSelf : ObservableDictionary<TSelf, TKey, TValue>, ICollectionAlerts<TSelf, KeyValuePair<TKey, TValue>>
{
    protected internal readonly Dictionary<TKey, TValue> buffer = dictionary;

    
    public override        int  Capacity   => buffer.Capacity;
    public sealed override int  Count      => buffer.Count;
    public                 bool IsReadOnly => ( (IDictionary)buffer ).IsReadOnly;

    public TValue this[ TKey key ]
    {
        get => buffer[key];
        set
        {
            bool exists = ContainsKey(key);

            TValue? old = exists
                              ? buffer[key]
                              : default;

            buffer[key] = value;
            KeyValuePair<TKey, TValue> pair = new(key, value);

            if ( exists )
            {
                KeyValuePair<TKey, TValue> oldPair = new(key, old!);
                Replaced(in oldPair, in pair, -1);
            }
            else { Added(in pair, -1); }
        }
    }

    public ICollection<TKey>                              Keys   => buffer.Keys;
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   => buffer.Keys;
    public ICollection<TValue>                            Values => buffer.Values;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => buffer.Values;


    protected ObservableDictionary() : this(DEFAULT_CAPACITY) { }
    protected ObservableDictionary( IDictionary<TKey, TValue>                       dictionary ) : this(new Dictionary<TKey, TValue>(dictionary,        EqualityComparer<TKey>.Default)) { }
    protected ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>>         collection ) : this(new Dictionary<TKey, TValue>(collection,        EqualityComparer<TKey>.Default)) { }
    protected ObservableDictionary( params ReadOnlySpan<KeyValuePair<TKey, TValue>> collection ) : this(new Dictionary<TKey, TValue>(collection.Length, EqualityComparer<TKey>.Default)) { Add(collection); }
    protected ObservableDictionary( int                                             capacity ) : this(new Dictionary<TKey, TValue>(capacity,            EqualityComparer<TKey>.Default)) { }


    public bool ContainsValue( TValue value ) => buffer.ContainsValue(value);


    public bool TryGetValue( TKey                    key, [NotNullWhen(true)] out TValue? value ) => buffer.TryGetValue(key, out value) && value is not null;
    public bool ContainsKey( TKey                    key )   => buffer.ContainsKey(key);
    public bool Contains( KeyValuePair<TKey, TValue> value ) => ContainsKey(value.Key) && ContainsValue(value.Value);


    public virtual void Add( KeyValuePair<TKey, TValue> value ) => Add(value.Key, value.Value);
    public virtual void Add( TKey key, TValue value )
    {
        if ( !buffer.TryAdd(key, value) ) { return; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Added(in pair, -1);
    }
    public void Add( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add(pair); }
    }
    public void Add( params ReadOnlySpan<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add(pair); }
    }


    public bool Remove( KeyValuePair<TKey, TValue> value ) => Remove(value.Key);
    public bool Remove( TKey key )
    {
        if ( !buffer.ContainsKey(key) ) { return false; }

        if ( !buffer.Remove(key, out TValue? value) ) { return false; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Removed(in pair, -1);
        OnCountChanged();
        return true;
    }


    public void Clear()
    {
        buffer.Clear();
        Reset();
    }


    public void CopyTo( KeyValuePair<TKey, TValue>[] array, int startIndex )
    {
        foreach ( ( int index, KeyValuePair<TKey, TValue> pair ) in this.EnumeratePairs(0) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }


    [Pure] [MustDisposeResource] [SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
    protected internal override ArrayBuffer<KeyValuePair<TKey, TValue>> FilteredValues()
    {
        int                                        count  = buffer.Count;
        ArrayBuffer<KeyValuePair<TKey, TValue>>   values = new(count);
        FilterDelegate<KeyValuePair<TKey, TValue>> filter = GetFilter();
        int                                        index  = 0;


        foreach ( KeyValuePair<TKey, TValue> pair in buffer )
        {
            if ( filter(index++, in pair) ) { values.Add(in pair); }
        }

        return values;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
