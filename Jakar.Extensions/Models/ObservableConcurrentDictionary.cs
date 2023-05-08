// Jakar.Extensions :: Jakar.Extensions
// 04/10/2022  6:24 PM

#nullable enable
namespace Jakar.Extensions;


public class ObservableConcurrentDictionary<TKey, TValue> : CollectionAlerts<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    protected readonly     ConcurrentDictionary<TKey, TValue> _dictionary;
    public sealed override int                                Count      => _dictionary.Count;
    public                 bool                               IsReadOnly => ((IDictionary)_dictionary).IsReadOnly;


    public TValue this[ TKey key ]
    {
        get => _dictionary[key];
        set
        {
            bool exists = ContainsKey( key );

            TValue? old = exists
                              ? _dictionary[key]
                              : default;

            _dictionary[key] = value;

            // ReSharper disable once NullableWarningSuppressionIsUsed
            if ( exists ) { Replaced( new KeyValuePair<TKey, TValue>( key, old! ), new KeyValuePair<TKey, TValue>( key, value ) ); }
            else { Added( new KeyValuePair<TKey, TValue>( key,             value ) ); }
        }
    }


    public ICollection<TKey>                              Keys   => _dictionary.Keys;
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   => _dictionary.Keys;
    public ICollection<TValue>                            Values => _dictionary.Values;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;


    public ObservableConcurrentDictionary() : this( new ConcurrentDictionary<TKey, TValue>() ) { }
    protected ObservableConcurrentDictionary( ConcurrentDictionary<TKey, TValue>   dictionary ) => _dictionary = dictionary;
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary ) : this( new ConcurrentDictionary<TKey, TValue>( dictionary ) ) { }
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) : this( new ConcurrentDictionary<TKey, TValue>( dictionary, comparer ) ) { }
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) : this( new ConcurrentDictionary<TKey, TValue>( collection ) ) { }
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) : this( new ConcurrentDictionary<TKey, TValue>( collection, comparer ) ) { }
    public ObservableConcurrentDictionary( IEqualityComparer<TKey>                 comparer ) : this( new ConcurrentDictionary<TKey, TValue>( comparer ) ) { }
    public ObservableConcurrentDictionary( int                                     concurrencyLevel, int capacity ) : this( new ConcurrentDictionary<TKey, TValue>( concurrencyLevel,                                   capacity ) ) { }
    public ObservableConcurrentDictionary( int                                     concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer ) : this( new ConcurrentDictionary<TKey, TValue>( concurrencyLevel, capacity, comparer ) ) { }


    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( List<KeyValuePair<TKey, TValue>>                 items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( HashSet<KeyValuePair<TKey, TValue>>              items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ConcurrentBag<KeyValuePair<TKey, TValue>>        items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ObservableCollection<KeyValuePair<TKey, TValue>> items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( Collection<KeyValuePair<TKey, TValue>>           items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( KeyValuePair<TKey, TValue>[]                     items ) => new(items);


    public bool ContainsValue( TValue              value ) => _dictionary.Values.Contains( value );
    public bool TryAdd( KeyValuePair<TKey, TValue> pair ) => TryAdd( pair.Key, pair.Value );
    public bool TryAdd( TKey key, TValue value )
    {
        if ( !_dictionary.TryAdd( key, value ) ) { return false; }

        Added( new KeyValuePair<TKey, TValue>( key, value ) );
        return true;
    }


    public void Add( params KeyValuePair<TKey, TValue>[] pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add( pair ); }
    }
    public void Add( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add( pair ); }
    }


    public bool TryGetValue( TKey key, [NotNullWhen( true )] out TValue? value ) => _dictionary.TryGetValue( key, out value );


    public bool ContainsKey( TKey key ) => _dictionary.ContainsKey( key );

    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey( item.Key ) && ContainsValue( item.Value );
    public void Add( KeyValuePair<TKey, TValue>      item ) => TryAdd( item );
    public void Add( TKey                            key, TValue value ) => TryAdd( key, value );


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove( item.Key );

    public bool Remove( TKey key )
    {
        if ( !_dictionary.ContainsKey( key ) ) { return false; }

        if ( !_dictionary.TryRemove( key, out TValue? value ) ) { return false; }

        Removed( new KeyValuePair<TKey, TValue>( key, value ) );
        OnCountChanged();
        return true;
    }

    public void Clear()
    {
        _dictionary.Clear();
        Reset();
        OnCountChanged();
    }


    public void CopyTo( KeyValuePair<TKey, TValue>[] array, int startIndex )
    {
        foreach ( (int index, KeyValuePair<TKey, TValue> pair) in this.EnumeratePairs( 0 ) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }


    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.Where( Filter )
                          .GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
