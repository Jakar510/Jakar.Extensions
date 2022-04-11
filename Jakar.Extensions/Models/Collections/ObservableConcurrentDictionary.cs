// Jakar.Extensions :: Jakar.Extensions
// 04/10/2022  6:24 PM

namespace Jakar.Extensions.Models.Collections;


public class ObservableConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
    protected ConcurrentDictionary<TKey, TValue> _Dictionary { get; }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler?         PropertyChanged;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.  Keys   => _Dictionary.Keys;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _Dictionary.Values;

    public ICollection<TKey>   Keys   => _Dictionary.Keys;
    public ICollection<TValue> Values => _Dictionary.Values;


    public int  Count      => _Dictionary.Count;
    public bool IsReadOnly => false;

    public TValue this[ TKey key ]
    {
        get => _Dictionary[key];
        set
        {
            TValue? old = ContainsKey(key)
                              ? _Dictionary[key]
                              : default;

            _Dictionary[key] = value;

            OnCollectionChanged(NotifyCollectionChangedAction.Replace, key, old, value);
            OnCountPropertyChanged();
        }
    }


    public ObservableConcurrentDictionary() => _Dictionary = new ConcurrentDictionary<TKey, TValue>();
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary ) => _Dictionary = new ConcurrentDictionary<TKey, TValue>(dictionary);
    public ObservableConcurrentDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) => _Dictionary = new ConcurrentDictionary<TKey, TValue>(dictionary, comparer);
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) => _Dictionary = new ConcurrentDictionary<TKey, TValue>(collection);
    public ObservableConcurrentDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) => _Dictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
    public ObservableConcurrentDictionary( IEqualityComparer<TKey>                 comparer ) => _Dictionary = new ConcurrentDictionary<TKey, TValue>(comparer);


    public bool TryGetValue( TKey key, out TValue value ) => _Dictionary.TryGetValue(key, out value);

    public bool ContainsKey( TKey key ) => _Dictionary.ContainsKey(key);

    public bool ContainsValue( TValue value ) => _Dictionary.Values.Contains(value);

    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey(item.Key) && ContainsValue(item.Value);


    public void Add( KeyValuePair<TKey, TValue> item ) => Add(item.Key, item.Value);
    public void Add( TKey                       key, TValue value ) => TryAdd(key, value);
    public bool TryAdd( TKey key, TValue value )
    {
        if ( !_Dictionary.TryAdd(key, value) ) { return false; }

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, pair);
        OnCountPropertyChanged();
        return true;
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove(item.Key);

    public bool Remove( TKey key )
    {
        if ( !_Dictionary.ContainsKey(key) ) { return false; }

        if ( !_Dictionary.TryRemove(key, out TValue value) ) { return false; }

        var pair = new KeyValuePair<TKey, TValue>(key, value);
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, pair);
        OnCountPropertyChanged();
        return true;
    }

    public void Clear()
    {
        _Dictionary.Clear();
        OnCollectionReset();
        OnCountPropertyChanged();
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


    protected void OnCountPropertyChanged() => OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
    protected virtual void OnPropertyChanged( [CallerMemberName] string property = "" ) => OnPropertyChanged(new PropertyChangedEventArgs(property));
    protected virtual void OnPropertyChanged( PropertyChangedEventArgs  e ) => PropertyChanged?.Invoke(this, e);


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
