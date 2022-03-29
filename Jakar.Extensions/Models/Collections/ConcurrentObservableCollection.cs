namespace Jakar.Extensions.Models.Collections;


/// <summary>
/// <para><see href="https://stackoverflow.com/a/54733415/9530917">This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread</see></para>
/// <para><see href="https://stackoverflow.com/a/14602121/9530917">How do I update an ObservableCollection via a worker thread?</see></para>
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class ConcurrentObservableCollection<T> : IList<T>, IList, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler?         PropertyChanged;


    protected readonly List<T>       _items;
    protected readonly IComparer<T>? _comparer;
    public             object        Lock => _lock;
    protected readonly object        _lock = new();


    public int Count
    {
        get
        {
            lock ( _lock ) { return _items.Count; }
        }
    }


    protected IList _Items
    {
        get
        {
            lock ( _lock ) { return _items; }
        }
    }

    bool ICollection.   IsSynchronized => _Items.IsSynchronized;
    object ICollection. SyncRoot       => _Items.SyncRoot;
    bool ICollection<T>.IsReadOnly     => _Items.IsReadOnly;
    bool IList.         IsFixedSize    => _Items.IsFixedSize;
    bool IList.         IsReadOnly     => _Items.IsReadOnly;


    public ConcurrentObservableCollection() : this(new List<T>()) { }
    protected ConcurrentObservableCollection( List<T> items ) : this(Comparer<T>.Default, items) { }
    protected ConcurrentObservableCollection( IComparer<T> comparer, List<T> items )
    {
        _comparer = comparer;
        _items    = items;
    }
    public ConcurrentObservableCollection( IComparer<T>   comparer ) : this(comparer, new List<T>()) { }
    public ConcurrentObservableCollection( int            capacity ) : this(new List<T>(capacity)) { }
    public ConcurrentObservableCollection( int            capacity, IComparer<T> comparer ) : this(comparer, new List<T>(capacity)) { }
    public ConcurrentObservableCollection( IEnumerable<T> items ) : this(new List<T>(items)) { }
    public ConcurrentObservableCollection( IEnumerable<T> items, IComparer<T> comparer ) : this(comparer, new List<T>(items)) { }


    public static implicit operator ConcurrentObservableCollection<T>( List<T>                 items ) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>( HashSet<T>              items ) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>( ConcurrentBag<T>        items ) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>( ObservableCollection<T> items ) => new(items);


    public virtual bool Contains( T item )
    {
        lock ( _lock ) { return _items.Contains(item); }
    }

    public virtual void Add( params T[] items )
    {
        lock ( _lock )
        {
            foreach ( T item in items )
            {
                _items.Add(item);
                Added(item);
            }
        }
    }
    public virtual void Add( IEnumerable<T> items )
    {
        lock ( _lock )
        {
            foreach ( T item in items )
            {
                _items.Add(item);
                Added(item);
            }
        }
    }
    public virtual void Add( T item )
    {
        lock ( _lock )
        {
            _items.Add(item);
            Added(item);
        }
    }

    public virtual bool TryAdd( T item )
    {
        lock ( _lock )
        {
            if ( _items.Contains(item) ) { return false; }

            _items.Add(item);
            Added(item);
            return true;
        }
    }

    public virtual void Clear()
    {
        lock ( _lock )
        {
            _items.Clear();
            Reset();
        }
    }

    public void Insert( int index, T item )
    {
        lock ( _lock )
        {
            _items.Insert(index, item);
            Added(item);
        }
    }
    public void InsertRange( int index, IEnumerable<T> collection )
    {
        lock ( _lock )
        {
            foreach ( ( int i, T? item ) in collection.Enumerate(index) )
            {
                _items.Insert(i, item);
                Added(item, i);
            }
        }
    }

    public virtual bool Remove( T item )
    {
        lock ( _lock )
        {
            bool result = _items.Remove(item);
            if ( result ) { Removed(item); }

            return result;
        }
    }

    public void RemoveAt( int index, out T item )
    {
        lock ( _lock )
        {
            item = this[index];
            _items.RemoveAt(index);
            Removed(item, index);
        }
    }
    public void RemoveAt( int index )
    {
        lock ( _lock )
        {
            T item = this[index];
            _items.RemoveAt(index);
            Removed(item, index);
        }
    }

    public int RemoveAll( Predicate<T> match )
    {
        lock ( _lock )
        {
            var results = 0;

            foreach ( T item in _items.Where(item => match(item)) )
            {
                _items.Remove(item);
                Removed(item);
                results++;
            }

            return results;
        }
    }
    public void RemoveRange( int index, int count )
    {
        lock ( _lock )
        {
            for ( int x = index; x < index + count; x++ )
            {
                _Items.RemoveAt(x);
                Removed(x);
            }
        }
    }


    public T this[ int index ]
    {
        get
        {
            lock ( _lock ) { return _items[index]; }
        }
        set
        {
            lock ( _lock )
            {
                T old = _items[index];
                _items[index] = value;
                Replaced(old, value, index);
            }
        }
    }


    public void CopyTo( int index, T[] array, int arrayIndex, int count )
    {
        lock ( _lock ) { _items.CopyTo(index, array, arrayIndex, count); }
    }
    public void CopyTo( T[] array )
    {
        lock ( _lock ) { _items.CopyTo(array); }
    }
    public void CopyTo( T[] array, int arrayIndex )
    {
        lock ( _lock ) { _items.CopyTo(array, arrayIndex); }
    }


    public bool Exists( Predicate<T> match )
    {
        lock ( _lock ) { return _items.Exists(match); }
    }
    public T? Find( Predicate<T> match )
    {
        lock ( _lock ) { return _items.Find(match); }
    }
    public List<T> FindAll( Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindAll(match); }
    }
    public int FindIndex( int startIndex, int count, Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindIndex(startIndex, count, match); }
    }
    public int FindIndex( int startIndex, Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindIndex(startIndex, match); }
    }
    public int FindIndex( Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindIndex(match); }
    }
    public T? FindLast( Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindLast(match); }
    }
    public int FindLastIndex( int startIndex, int count, Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindLastIndex(startIndex, count, match); }
    }
    public int FindLastIndex( int startIndex, Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindLastIndex(startIndex, match); }
    }
    public int FindLastIndex( Predicate<T> match )
    {
        lock ( _lock ) { return _items.FindLastIndex(match); }
    }
    public int IndexOf( T item, int index )
    {
        lock ( _lock ) { return _items.IndexOf(item, index); }
    }
    public int IndexOf( T item, int index, int count )
    {
        lock ( _lock ) { return _items.IndexOf(item, index, count); }
    }
    public int IndexOf( T item )
    {
        lock ( _lock ) { return _items.IndexOf(item); }
    }
    public int LastIndexOf( T item )
    {
        lock ( _lock ) { return _items.LastIndexOf(item); }
    }
    public int LastIndexOf( T item, int index )
    {
        lock ( _lock ) { return _items.LastIndexOf(item, index); }
    }
    public int LastIndexOf( T item, int index, int count )
    {
        lock ( _lock ) { return _items.LastIndexOf(item, index, count); }
    }


    public void Reverse()
    {
        lock ( _lock )
        {
            _items.Reverse();
            Reset();
        }
    }
    public void Reverse( int index, int count )
    {
        lock ( _lock )
        {
            _items.Reverse(index, count);
            Reset();
        }
    }


    public virtual void Sort()
    {
        lock ( _lock )
        {
            if ( _comparer is null ) { _items.Sort(); }
            else { _items.Sort(_comparer); }

            Reset();
        }
    }
    public virtual void Sort( IComparer<T> comparer )
    {
        lock ( _lock )
        {
            _items.Sort(comparer);

            Reset();
        }
    }
    public virtual void Sort( Comparison<T> comparison )
    {
        lock ( _lock )
        {
            _items.Sort(comparison);
            Reset();
        }
    }
    public virtual void Sort( int index, int count, IComparer<T> comparer )
    {
        lock ( _lock )
        {
            _items.Sort(index, count, comparer);
            Reset();
        }
    }


    void ICollection.CopyTo( Array    array, int index ) => _Items.CopyTo(array, index);
    void IList.      Remove( object   value )               => _Items.Remove(value);
    int IList.       Add( object      value )               => _Items.Add(value);
    bool IList.      Contains( object value )               => _Items.Contains(value);
    int IList.       IndexOf( object  value )               => _Items.IndexOf(value);
    void IList.      Insert( int      index, object value ) => _Items.Insert(index, value);

    object IList.this[ int index ]

    {
        get => _Items[index];
        set => _Items[index] = value;
    }


    public IEnumerator<T> GetEnumerator()
    {
        lock ( _lock ) { return _items.GetEnumerator(); }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    protected void Added( in List<T> items )
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        OnPropertyChanged(nameof(Count));
    }
    protected void Added( in T item )
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        OnPropertyChanged(nameof(Count));
    }
    protected void Added( in T item, in int index )
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        OnPropertyChanged(nameof(Count));
    }
    protected void Removed( in T item )
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        OnPropertyChanged(nameof(Count));
    }
    protected void Removed( in T item, in int index )
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        OnPropertyChanged(nameof(Count));
    }
    protected void Removed( in int index )
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, index));
        OnPropertyChanged(nameof(Count));
    }
    protected void Replaced( in T old,  in T   @new )                   => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, @new, old));
    protected void Replaced( in T old,  in T   @new,  in int index )    => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, @new, old, index));
    protected void Move( in     T item, in int index, in int oldIndex ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, index, oldIndex));
    protected void Reset()
    {
        OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(nameof(Count));
    }
    protected void OnChanged( NotifyCollectionChangedEventArgs   e )                => CollectionChanged?.Invoke(this, e);
    protected void OnPropertyChanged( [CallerMemberName] string? caller = default ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
}
