using System;
using NoAlloq;



namespace Jakar.Extensions;


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableDictionary<TKey, TValue> : CollectionAlerts<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    protected internal readonly Dictionary<TKey, TValue> buffer;


    public sealed override int  Count      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    public                 bool IsReadOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ((IDictionary)buffer).IsReadOnly; }

    public TValue this[ TKey key ]
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer[key];
        set
        {
            bool exists = ContainsKey( key );

            TValue? old = exists
                              ? buffer[key]
                              : default;

            buffer[key] = value;
            KeyValuePair<TKey, TValue> pair = new(key, value);

            if ( exists )
            {
                KeyValuePair<TKey, TValue> oldPair = new(key, old!);
                Replaced( in oldPair, in pair, -1 );
            }
            else { Added( in pair, -1 ); }
        }
    }

    public ICollection<TKey>                              Keys   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Keys; }
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Keys; }
    public ICollection<TValue>                            Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Values; }
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Values; }


    public ObservableDictionary() : this( DEFAULT_CAPACITY ) { }
    protected ObservableDictionary( Dictionary<TKey, TValue>             dictionary ) => buffer = dictionary;
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary ) : this( new Dictionary<TKey, TValue>( dictionary ) ) { }
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) : this( new Dictionary<TKey, TValue>( dictionary, comparer ) ) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) : this( new Dictionary<TKey, TValue>( collection ) ) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) : this( new Dictionary<TKey, TValue>( collection, comparer ) ) { }
    public ObservableDictionary( IEqualityComparer<TKey>                 comparer ) : this( DEFAULT_CAPACITY, comparer ) { }
    public ObservableDictionary( int                                     capacity ) : this( new Dictionary<TKey, TValue>( capacity ) ) { }
    public ObservableDictionary( int                                     capacity, IEqualityComparer<TKey> comparer ) : this( new Dictionary<TKey, TValue>( capacity, comparer ) ) { }


    public static implicit operator ObservableDictionary<TKey, TValue>( List<KeyValuePair<TKey, TValue>>          items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( HashSet<KeyValuePair<TKey, TValue>>       items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( ConcurrentBag<KeyValuePair<TKey, TValue>> items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( Collection<KeyValuePair<TKey, TValue>>    items ) => new(items);
    public static implicit operator ObservableDictionary<TKey, TValue>( KeyValuePair<TKey, TValue>[]              items ) => new(items);


    public bool ContainsValue( TValue value ) => buffer.ContainsValue( value );


    public bool TryGetValue( TKey                    key, [NotNullWhen( true )] out TValue? value ) => buffer.TryGetValue( key, out value ) && value is not null;
    public bool ContainsKey( TKey                    key )  => buffer.ContainsKey( key );
    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey( item.Key ) && ContainsValue( item.Value );


    public virtual void Add( KeyValuePair<TKey, TValue> item ) => Add( item.Key, item.Value );
    public virtual void Add( TKey key, TValue value )
    {
        if ( buffer.TryAdd( key, value ) is false ) { return; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Added( in pair, -1 );
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove( item.Key );
    public bool Remove( TKey key )
    {
        if ( buffer.ContainsKey( key ) is false ) { return false; }

        if ( buffer.Remove( key, out TValue? value ) is false ) { return false; }

        KeyValuePair<TKey, TValue> pair = new(key, value);
        Removed( in pair, -1 );
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
        foreach ( (int index, KeyValuePair<TKey, TValue> pair) in this.EnumeratePairs( 0 ) )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }


    [Pure, MustDisposeResource]
    [SuppressMessage( "ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator" )]
    protected internal override FilterBuffer<KeyValuePair<TKey, TValue>> FilteredValues()
    {
        int                                      count  = buffer.Count;
        FilterBuffer<KeyValuePair<TKey, TValue>> values = new(count);
        int                                      index  = 0;

        foreach ( KeyValuePair<TKey, TValue> pair in buffer )
        {
            if ( Filter( index++, in pair ) ) { values.Add( in pair ); }
        }

        return values;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
