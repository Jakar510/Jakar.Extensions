// Jakar.Extensions :: Jakar.Extensions
// 04/10/2022  6:24 PM

namespace Jakar.Extensions;


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).



public sealed class ObservableConcurrentDictionary<TKey, TValue> : ObservableConcurrentDictionary<ObservableConcurrentDictionary<TKey, TValue>, TKey, TValue>, ICollectionAlerts<ObservableConcurrentDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private static JsonTypeInfo<ObservableConcurrentDictionary<TKey, TValue>[]>? __JsonArrayInfo;
    private static JsonSerializerContext?                                        __jsonContext;
    private static JsonTypeInfo<ObservableConcurrentDictionary<TKey, TValue>>?   __jsonTypeInfo;
    public static  JsonSerializerContext                                         JsonContext   { get => Validate.ThrowIfNull(__jsonContext);   set => __jsonContext = value; }
    public static  JsonTypeInfo<ObservableConcurrentDictionary<TKey, TValue>>    JsonTypeInfo  { get => Validate.ThrowIfNull(__jsonTypeInfo);  set => __jsonTypeInfo = value; }
    public static  JsonTypeInfo<ObservableConcurrentDictionary<TKey, TValue>[]>  JsonArrayInfo { get => Validate.ThrowIfNull(__JsonArrayInfo); set => __JsonArrayInfo = value; }


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
    public override bool Equals( object?                                            other )                                                     => ReferenceEquals(this, other) || other is ObservableConcurrentDictionary<TKey, TValue> x && Equals(x);
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


    public sealed override int  Count      { get => buffer.Count; }
    public                 bool IsReadOnly { get => ( (IDictionary)buffer ).IsReadOnly; }

    public TValue this[ TKey key ]
    {
        get => buffer[key];
        set
        {
            bool exists = TryGetValue(key, out TValue? old);
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

    public ICollection<TKey>                              Keys   { get => buffer.Keys; }
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   { get => buffer.Keys; }
    public ICollection<TValue>                            Values { get => buffer.Values; }
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { get => buffer.Values; }


    protected ObservableConcurrentDictionary() : this(DEFAULT_CAPACITY) { }
    protected ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>>         collection ) : this() => Add(collection);
    protected ObservableConcurrentDictionary( params ReadOnlySpan<KeyValuePair<TKey, TValue>> collection ) : this() => Add(collection);
    protected ObservableConcurrentDictionary( IDictionary<TKey, TValue>                       dictionary ) : this(new ConcurrentDictionary<TKey, TValue>(dictionary)) { }
    protected ObservableConcurrentDictionary( int                                             capacity ) : this(new ConcurrentDictionary<TKey, TValue>(Environment.ProcessorCount, capacity, EqualityComparer<TKey>.Default)) { }


    public void Clear()
    {
        buffer.Clear();
        Reset();
        OnCountChanged();
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


    public bool ContainsValue( TValue                value ) => buffer.Values.Contains(value);
    public bool ContainsKey( TKey                    key )   => buffer.ContainsKey(key);
    public bool Contains( KeyValuePair<TKey, TValue> item )  => ContainsKey(item.Key) && ContainsValue(item.Value);


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove(item.Key);
    public bool Remove( TKey key )
    {
        if ( !buffer.ContainsKey(key) ) { return false; }

        if ( !buffer.TryRemove(key, out TValue? value) ) { return false; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Removed(in pair, -1);
        OnCountChanged();
        return true;
    }


    public void CopyTo( KeyValuePair<TKey, TValue>[] array, int startIndex )
    {
        foreach ( ( int index, KeyValuePair<TKey, TValue> pair ) in this.EnumeratePairs(0) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }
    public void CopyTo( Span<KeyValuePair<TKey, TValue>> array, int startIndex )
    {
        foreach ( ( int index, KeyValuePair<TKey, TValue> pair ) in this.EnumeratePairs(0) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }


    [Pure] [MustDisposeResource] [SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
    protected internal override FilterBuffer<KeyValuePair<TKey, TValue>> FilteredValues()
    {
        int                                        count  = buffer.Count;
        FilterBuffer<KeyValuePair<TKey, TValue>>   values = new(count);
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
