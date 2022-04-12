namespace Jakar.Extensions.Models.Collections;


public class ObservableDictionary<TKey, TValue> : ObservableClass, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ICollectionAlerts
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    protected readonly Dictionary<TKey, TValue>           _dictionary;
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


    public ObservableDictionary() : this(new Dictionary<TKey, TValue>()) { }
    protected ObservableDictionary( Dictionary<TKey, TValue>             dictionary ) => _dictionary = dictionary;
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary ) : this(new Dictionary<TKey, TValue>(dictionary)) { }
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) : this(new Dictionary<TKey, TValue>(dictionary, comparer)) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) : this(new Dictionary<TKey, TValue>(collection)) { }
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) : this(new Dictionary<TKey, TValue>(collection, comparer)) { }
    public ObservableDictionary( IEqualityComparer<TKey>                 comparer ) : this(new Dictionary<TKey, TValue>(comparer)) { }
    public ObservableDictionary( int                                     capacity ) : this(new Dictionary<TKey, TValue>(capacity)) { }
    public ObservableDictionary( int                                     capacity, IEqualityComparer<TKey> comparer ) : this(new Dictionary<TKey, TValue>(capacity, comparer)) { }


    public bool TryGetValue( TKey key, out TValue value ) => _dictionary.TryGetValue(key, out value);

    public bool ContainsKey( TKey key ) => _dictionary.ContainsKey(key);

    public bool ContainsValue( TValue value ) => _dictionary.ContainsValue(value);

    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey(item.Key) && ContainsValue(item.Value);


    public void Add( KeyValuePair<TKey, TValue> item ) => Add(item.Key, item.Value);

    public void Add( TKey key, TValue value )
    {
        _dictionary.Add(key, value);
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        Added(pair);
        OnCountChanged();
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove(item.Key);

    public bool Remove( TKey key )
    {
        if ( !_dictionary.ContainsKey(key) ) { return false; }

        TValue value = _dictionary[key];
        _dictionary.Remove(key);
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        Removed(pair);
        OnCountChanged();
        return true;
    }

    public void Clear()
    {
        _dictionary.Clear();
        Reset();
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
