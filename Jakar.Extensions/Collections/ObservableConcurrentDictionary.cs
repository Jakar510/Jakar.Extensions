// Jakar.Extensions :: Jakar.Extensions
// 04/10/2022  6:24 PM

namespace Jakar.Extensions;


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).



public class ObservableConcurrentDictionary<TKey, TValue> : CollectionAlerts<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    protected internal readonly ConcurrentDictionary<TKey, TValue> buffer;


    public sealed override int  Count      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    public                 bool IsReadOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ((IDictionary)buffer).IsReadOnly; }

    public TValue this[ TKey key ]
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer[key];
        set
        {
            bool exists = TryGetValue( key, out TValue? old );
            buffer[key] = value;

            // ReSharper disable once NullableWarningSuppressionIsUsed
            if ( exists ) { Replaced( new KeyValuePair<TKey, TValue>( key, old! ), new KeyValuePair<TKey, TValue>( key, value ) ); }
            else { Added( new KeyValuePair<TKey, TValue>( key,             value ) ); }
        }
    }

    public ICollection<TKey>                              Keys   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Keys; }
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Keys; }
    public ICollection<TValue>                            Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Values; }
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Values; }


    public ObservableConcurrentDictionary() : this( DEFAULT_CAPACITY ) { }
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary ) : this( dictionary, EqualityComparer<TKey>.Default ) { }
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) : this( dictionary.Count, comparer ) => Add( dictionary );
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) : this( EqualityComparer<TKey>.Default ) => Add( collection );
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) : this( comparer ) => Add( collection );
    public ObservableConcurrentDictionary( IEqualityComparer<TKey>                 comparer ) : this( Environment.ProcessorCount, DEFAULT_CAPACITY, comparer ) { }
    public ObservableConcurrentDictionary( int                                     capacity ) : this( capacity, EqualityComparer<TKey>.Default ) { }
    public ObservableConcurrentDictionary( int                                     capacity,         IEqualityComparer<TKey> comparer ) : this( Environment.ProcessorCount, capacity, comparer ) { }
    public ObservableConcurrentDictionary( int                                     concurrencyLevel, int                     capacity ) : this( concurrencyLevel, capacity, EqualityComparer<TKey>.Default ) { }
    public ObservableConcurrentDictionary( int                                     concurrencyLevel, int                     capacity, IEqualityComparer<TKey> comparer ) : this( new ConcurrentDictionary<TKey, TValue>( concurrencyLevel, capacity, comparer ) ) { }
    protected ObservableConcurrentDictionary( ConcurrentDictionary<TKey, TValue>   dictionary ) => buffer = dictionary;


    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( List<KeyValuePair<TKey, TValue>>          items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( HashSet<KeyValuePair<TKey, TValue>>       items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( ConcurrentBag<KeyValuePair<TKey, TValue>> items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( Collection<KeyValuePair<TKey, TValue>>    items ) => new(items);
    public static implicit operator ObservableConcurrentDictionary<TKey, TValue>( KeyValuePair<TKey, TValue>[]              items ) => new(items);


    public void Clear()
    {
        buffer.Clear();
        Reset();
        OnCountChanged();
    }


    public bool TryAdd( KeyValuePair<TKey, TValue> pair ) => TryAdd( pair.Key, pair.Value );
    public bool TryAdd( TKey key, TValue value )
    {
        if ( !buffer.TryAdd( key, value ) ) { return false; }

        Added( new KeyValuePair<TKey, TValue>( key, value ) );
        return true;
    }


    public void Add( KeyValuePair<TKey, TValue> item )              => TryAdd( item );
    public void Add( TKey                       key, TValue value ) => TryAdd( key, value );
    public void Add( params KeyValuePair<TKey, TValue>[] pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add( pair ); }
    }
    public void Add( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add( pair ); }
    }


    public bool TryGetValue( TKey key, [NotNullWhen( true )] out TValue? value ) => buffer.TryGetValue( key, out value ) && value is not null;


    public bool ContainsValue( TValue                value ) => buffer.Values.Contains( value );
    public bool ContainsKey( TKey                    key )   => buffer.ContainsKey( key );
    public bool Contains( KeyValuePair<TKey, TValue> item )  => ContainsKey( item.Key ) && ContainsValue( item.Value );


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove( item.Key );
    public bool Remove( TKey key )
    {
        if ( !buffer.ContainsKey( key ) ) { return false; }

        if ( !buffer.TryRemove( key, out TValue? value ) ) { return false; }

        Removed( new KeyValuePair<TKey, TValue>( key, value ) );
        OnCountChanged();
        return true;
    }


    public void CopyTo( KeyValuePair<TKey, TValue>[] array, int startIndex )
    {
        foreach ( (int index, KeyValuePair<TKey, TValue> pair) in this.EnumeratePairs( 0 ) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }
    public void CopyTo( Span<KeyValuePair<TKey, TValue>> array, int startIndex )
    {
        foreach ( (int index, KeyValuePair<TKey, TValue> pair) in this.EnumeratePairs( 0 ) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }


    [Pure, MustDisposeResource]
    protected internal override FilterBuffer<KeyValuePair<TKey, TValue>> FilteredValues()
    {
        int                                      count  = buffer.Count;
        FilterBuffer<KeyValuePair<TKey, TValue>> values = new(count);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( KeyValuePair<TKey, TValue> pair in buffer )
        {
            if ( Filter( in pair ) ) { values.Add( in pair ); }
        }

        return values;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
