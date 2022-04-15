// Jakar.Extensions :: Jakar.Extensions
// 04/10/2022  6:24 PM

namespace Jakar.Extensions.Models.Collections;


public class ObservableConcurrentDictionary<TKey, TValue> : ObservableClass, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ICollectionAlerts
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    protected readonly ConcurrentDictionary<TKey, TValue> _dictionary;
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys       => _dictionary.Keys;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values     => _dictionary.Values;
    public ICollection<TKey>                              Keys       => _dictionary.Keys;
    public ICollection<TValue>                            Values     => _dictionary.Values;
    public int                                            Count      => _dictionary.Count;
    public bool                                           IsReadOnly => ( (IDictionary)_dictionary ).IsReadOnly;

    public TValue this[ TKey key ]
    {
        get => _dictionary[key];
        set
        {
            bool exists = ContainsKey(key);

            TValue? old = exists
                              ? _dictionary[key]
                              : default;

            _dictionary[key] = value;

            if ( exists ) { Replaced(key, old, value); }
            else { Added(key, value); }
        }
    }


    public ObservableConcurrentDictionary() : this(new ConcurrentDictionary<TKey, TValue>()) { }
    protected ObservableConcurrentDictionary( ConcurrentDictionary<TKey, TValue>   dictionary ) => _dictionary = dictionary;
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary ) : this(new ConcurrentDictionary<TKey, TValue>(dictionary)) { }
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) : this(new ConcurrentDictionary<TKey, TValue>(dictionary, comparer)) { }
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) : this(new ConcurrentDictionary<TKey, TValue>(collection)) { }
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) : this(new ConcurrentDictionary<TKey, TValue>(collection, comparer)) { }
    public ObservableConcurrentDictionary( IEqualityComparer<TKey>                 comparer ) : this(new ConcurrentDictionary<TKey, TValue>(comparer)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
    /// class that is empty, has the specified concurrency level, has the specified initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer{TKey}"/>.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="ConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="ConcurrentDictionary{TKey,TValue}"/> can contain.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="concurrencyLevel"/> is less than 1. -or- <paramref name="capacity"/> is less than 0.
    /// </exception>
    public ObservableConcurrentDictionary( int concurrencyLevel, int capacity ) : this(new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}"/>
    /// class that is empty, has the specified concurrency level, has the specified initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer{TKey}"/>.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="ConcurrentDictionary{TKey,TValue}"/> concurrently.</param>
    /// <param name="capacity">The initial number of elements that the <see cref="ConcurrentDictionary{TKey,TValue}"/> can contain.</param>
    /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer{TKey}"/> implementation to use when comparing keys.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="concurrencyLevel"/> is less than 1. -or- <paramref name="capacity"/> is less than 0.
    /// </exception>
    public ObservableConcurrentDictionary( int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer ) : this(new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer)) { }


    public bool TryGetValue( TKey key, out TValue value ) => _dictionary.TryGetValue(key, out value);

    public bool ContainsKey( TKey key ) => _dictionary.ContainsKey(key);

    public bool ContainsValue( TValue value ) => _dictionary.Values.Contains(value);

    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey(item.Key) && ContainsValue(item.Value);


    public void Add( params KeyValuePair<TKey, TValue>[] pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add(pair); }
    }
    public void Add( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
    {
        foreach ( KeyValuePair<TKey, TValue> pair in pairs ) { Add(pair); }
    }
    public void Add( KeyValuePair<TKey, TValue>    item ) => TryAdd(item);
    public void Add( TKey                          key, TValue value ) => TryAdd(key, value);
    public bool TryAdd( KeyValuePair<TKey, TValue> pair ) => TryAdd(pair.Key, pair.Value);
    public bool TryAdd( TKey key, TValue value )
    {
        if ( !_dictionary.TryAdd(key, value) ) { return false; }

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        Added(pair);
        OnCountChanged();
        return true;
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove(item.Key);

    public bool Remove( TKey key )
    {
        if ( !_dictionary.ContainsKey(key) ) { return false; }

        if ( !_dictionary.TryRemove(key, out TValue value) ) { return false; }

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        Removed(pair);
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
        foreach ( ( int index, KeyValuePair<TKey, TValue> pair ) in this.EnumeratePairs() )
        {
            if ( index < startIndex ) { continue; }

            array[index] = pair;
        }
    }

    protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e ) => CollectionChanged?.Invoke(this, e);
    protected void Removed( KeyValuePair<TKey, TValue> item )
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        OnCountChanged();
    }
    protected void Added( TKey key, TValue item ) => Added(new KeyValuePair<TKey, TValue>(key, item));
    protected void Added( KeyValuePair<TKey, TValue> item )
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        OnCountChanged();
    }

    protected void Replaced( TKey key, TValue? oldItem, TValue? newItem ) => OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue?>(key, oldItem), new KeyValuePair<TKey, TValue?>(key, newItem));
    protected void OnCollectionChanged( NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue?> oldItem, KeyValuePair<TKey, TValue?> newItem ) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
    protected void Reset()
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnCountChanged();
    }
    void ICollectionAlerts.SendOnChanged( NotifyCollectionChangedEventArgs e ) => OnCollectionChanged(e);
    protected void OnCountChanged() => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
