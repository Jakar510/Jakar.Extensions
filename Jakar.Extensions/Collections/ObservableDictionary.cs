﻿namespace Jakar.Extensions;


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableDictionary<TKey, TValue> : CollectionAlerts<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    protected readonly Dictionary<TKey, TValue> _dictionary;


    public sealed override int  Count      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Count; }
    public                 bool IsReadOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ((IDictionary)_dictionary).IsReadOnly; }

    public TValue this[ TKey key ]
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary[key];
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

    public ICollection<TKey>                              Keys   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Keys; }
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Keys; }
    public ICollection<TValue>                            Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Values; }
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _dictionary.Values; }


    public ObservableDictionary() : this( DEFAULT_CAPACITY ) { }
    protected ObservableDictionary( Dictionary<TKey, TValue>             dictionary ) => _dictionary = dictionary;
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary ) : this( new Dictionary<TKey, TValue>( dictionary ) ) { }
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) : this( new Dictionary<TKey, TValue>( dictionary, comparer ) ) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) : this( new Dictionary<TKey, TValue>( collection ) ) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) : this( new Dictionary<TKey, TValue>( collection, comparer ) ) { }
    public ObservableDictionary( IEqualityComparer<TKey>                 comparer ) : this( DEFAULT_CAPACITY, comparer ) { }
    public ObservableDictionary( int                                     capacity ) : this( new Dictionary<TKey, TValue>( capacity ) ) { }
    public ObservableDictionary( int                                     capacity, IEqualityComparer<TKey> comparer ) : this( new Dictionary<TKey, TValue>( capacity, comparer ) ) { }


    public static implicit operator ObservableDictionary<TKey, TValue>( List<KeyValuePair<TKey, TValue>>                 items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( HashSet<KeyValuePair<TKey, TValue>>              items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( ConcurrentBag<KeyValuePair<TKey, TValue>>        items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( ObservableCollection<KeyValuePair<TKey, TValue>> items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( Collection<KeyValuePair<TKey, TValue>>           items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( KeyValuePair<TKey, TValue>[]                     items ) => new(items);


    public bool ContainsValue( TValue value ) => _dictionary.ContainsValue( value );


    public bool TryGetValue( TKey                    key, [NotNullWhen( true )] out TValue? value ) => _dictionary.TryGetValue( key, out value ) && value is not null;
    public bool ContainsKey( TKey                    key )  => _dictionary.ContainsKey( key );
    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey( item.Key ) && ContainsValue( item.Value );


    public virtual void Add( KeyValuePair<TKey, TValue> item ) => Add( item.Key, item.Value );
    public virtual void Add( TKey key, TValue value )
    {
        if ( !_dictionary.TryAdd( key, value ) ) { return; }

        Added( new KeyValuePair<TKey, TValue>( key, value ) );
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove( item.Key );
    public bool Remove( TKey key )
    {
        if ( !_dictionary.ContainsKey( key ) ) { return false; }

        TValue value = _dictionary[key];
        _dictionary.Remove( key );
        Removed( new KeyValuePair<TKey, TValue>( key, value ) );
        return true;
    }


    public void Clear()
    {
        _dictionary.Clear();
        Reset();
    }


    public void CopyTo( KeyValuePair<TKey, TValue>[] array, int startIndex )
    {
        foreach ( (int index, KeyValuePair<TKey, TValue> pair) in this.EnumeratePairs( 0 ) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal override ReadOnlyMemory<KeyValuePair<TKey, TValue>> FilteredValues() => FilteredValues( _dictionary.ToSpanEnumerable() );
    IEnumerator IEnumerable.                                                                                                    GetEnumerator()  => GetEnumerator();
}