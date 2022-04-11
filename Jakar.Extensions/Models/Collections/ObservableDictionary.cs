﻿namespace Jakar.Extensions.Models.Collections;


public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
    protected Dictionary<TKey, TValue> _Dictionary { get; }

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


    public ObservableDictionary() => _Dictionary = new Dictionary<TKey, TValue>();
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary ) => _Dictionary = new Dictionary<TKey, TValue>(dictionary);
    public ObservableDictionary( IDictionary<TKey, TValue>               dictionary, IEqualityComparer<TKey> comparer ) => _Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection ) => _Dictionary = new Dictionary<TKey, TValue>(collection);
    public ObservableDictionary( IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer ) => _Dictionary = new Dictionary<TKey, TValue>(collection, comparer);
    public ObservableDictionary( IEqualityComparer<TKey>                 comparer ) => _Dictionary = new Dictionary<TKey, TValue>(comparer);
    public ObservableDictionary( int                                     capacity ) => _Dictionary = new Dictionary<TKey, TValue>(capacity);
    public ObservableDictionary( int                                     capacity, IEqualityComparer<TKey> comparer ) => _Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);


    public bool TryGetValue( TKey key, out TValue value ) => _Dictionary.TryGetValue(key, out value);

    public bool ContainsKey( TKey key ) => _Dictionary.ContainsKey(key);

    public bool ContainsValue( TValue value ) => _Dictionary.ContainsValue(value);

    public bool Contains( KeyValuePair<TKey, TValue> item ) => ContainsKey(item.Key) && ContainsValue(item.Value);


    public void Add( KeyValuePair<TKey, TValue> item ) => Add(item.Key, item.Value);

    public void Add( TKey key, TValue value )
    {
        _Dictionary.Add(key, value);
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, pair);
        OnCountPropertyChanged();
    }


    public bool Remove( KeyValuePair<TKey, TValue> item ) => Remove(item.Key);

    public bool Remove( TKey key )
    {
        if ( !_Dictionary.ContainsKey(key) ) { return false; }

        TValue value = _Dictionary[key];
        _Dictionary.Remove(key);
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