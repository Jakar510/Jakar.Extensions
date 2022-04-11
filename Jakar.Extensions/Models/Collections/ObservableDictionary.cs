namespace Jakar.Extensions.Models.Collections;


public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler?         PropertyChanged;


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
            TValue? old = ContainsKey(key)
                              ? _dictionary[key]
                              : default;

            _dictionary[key] = value;

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, key, old, value);
            OnCountChanged();
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
        OnCollectionChanged(NotifyCollectionChangedAction.Add, pair);
        OnCountChanged();
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove(item.Key);

    public bool Remove( TKey key )
    {
        if ( !_dictionary.ContainsKey(key) ) { return false; }

        TValue value = _dictionary[key];
        _dictionary.Remove(key);
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, pair);
        OnCountChanged();
        return true;
    }

    public void Clear()
    {
        _dictionary.Clear();
        OnCollectionReset();
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

    protected void OnCollectionChanged( NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> item ) =>
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(action,
                                                                 new List<KeyValuePair<TKey, TValue>>()
                                                                 {
                                                                     item
                                                                 }));

    protected void OnCollectionChanged( NotifyCollectionChangedAction action, TKey key, TValue? oldItem, TValue? newItem ) =>
        OnCollectionChanged(action, new KeyValuePair<TKey, TValue?>(key, oldItem), new KeyValuePair<TKey, TValue?>(key, newItem));

    protected void OnCollectionChanged( NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue?> oldItem, KeyValuePair<TKey, TValue?> newItem ) =>
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem));

    protected void OnCollectionReset() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));


    protected void OnCountChanged() => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    protected virtual void OnPropertyChanged( [CallerMemberName] string property = "" ) => OnPropertyChanged(new PropertyChangedEventArgs(property));
    protected virtual void OnPropertyChanged( PropertyChangedEventArgs  e ) => PropertyChanged?.Invoke(this, e);


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
