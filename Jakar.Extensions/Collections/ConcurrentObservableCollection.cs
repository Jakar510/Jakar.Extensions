#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href = "https://stackoverflow.com/a/54733415/9530917" > This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see>
///     </para>
///     <para>
///         <see href = "https://stackoverflow.com/a/14602121/9530917" > How do I update an ObservableCollection via a worker thread? </see>
///     </para>
/// </summary>
/// <typeparam name = "T" > </typeparam>
[Serializable]
public class ConcurrentObservableCollection<T> : CollectionAlerts<T>, IList<T>, IReadOnlyList<T>, IList
{
    protected readonly IComparer<T> _comparer;


    protected readonly List<T> _items;
    protected readonly object _lock = new();
    public object Lock => _lock;


    public sealed override int Count
    {
        get
        {
            lock (_lock) { return _items.Count; }
        }
    }
    bool ICollection<T>.IsReadOnly
    {
        get
        {
            lock (_lock) { return ((IList)_items).IsReadOnly; }
        }
    }

    public T this[int index]
    {
        get
        {
            lock (_lock) { return _items[index]; }
        }
        set
        {
            lock (_lock)
            {
                T old = _items[index];
                _items[index] = value;
                Replaced(old, value, index);
            }
        }
    }


    object? IList.this[int index]

    {
        get
        {
            lock (_lock) { return ((IList)_items)[index]; }
        }
        set
        {
            lock (_lock) { ((IList)_items)[index] = value; }
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            lock (_lock) { return ((IList)_items).IsSynchronized; }
        }
    }
    object ICollection.SyncRoot
    {
        get
        {
            lock (_lock) { return ((IList)_items).SyncRoot; }
        }
    }
    bool IList.IsFixedSize
    {
        get
        {
            lock (_lock) { return ((IList)_items).IsFixedSize; }
        }
    }
    bool IList.IsReadOnly
    {
        get
        {
            lock (_lock) { return ((IList)_items).IsReadOnly; }
        }
    }


    public ConcurrentObservableCollection() : this(new List<T>()) { }
    protected ConcurrentObservableCollection(List<T> items) : this(Comparer<T>.Default, items) { }
    protected ConcurrentObservableCollection(IComparer<T> comparer, List<T> items)
    {
        _comparer = comparer;
        _items = items;
    }
    public ConcurrentObservableCollection(IComparer<T> comparer) : this(comparer, new List<T>()) { }
    public ConcurrentObservableCollection(int capacity) : this(Comparer<T>.Default, new List<T>(capacity)) { }
    public ConcurrentObservableCollection(int capacity, IComparer<T> comparer) : this(comparer, new List<T>(capacity)) { }
    public ConcurrentObservableCollection(IEnumerable<T> items) : this(new List<T>(items)) { }
    public ConcurrentObservableCollection(IEnumerable<T> items, IComparer<T> comparer) : this(comparer, new List<T>(items)) { }


    public static implicit operator ConcurrentObservableCollection<T>(List<T> items) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>(HashSet<T> items) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>(ConcurrentBag<T> items) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>(ObservableCollection<T> items) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>(Collection<T> items) => new(items);
    public static implicit operator ConcurrentObservableCollection<T>(T[] items) => new(items);


    public virtual void Add(params T[] items)
    {
        lock (_lock)
        {
            foreach (T item in items)
            {
                _items.Add(item);
                Added(item);
            }
        }
    }
    public virtual void Add(IEnumerable<T> items)
    {
        lock (_lock)
        {
            foreach (T item in items)
            {
                _items.Add(item);
                Added(item);
            }
        }
    }

    public virtual bool TryAdd(T item)
    {
        lock (_lock)
        {
            if (_items.Contains(item)) { return false; }

            _items.Add(item);
            Added(item);
            return true;
        }
    }
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        lock (_lock)
        {
            foreach ((int i, T? item) in collection.Enumerate(index))
            {
                _items.Insert(i, item);
                Added(item, i);
            }
        }
    }

    public void RemoveAt(int index, out T? item)
    {
        lock (_lock)
        {
            item = this[index];
            _items.RemoveAt(index);
            Removed(item, index);
        }
    }

    public int RemoveAll(Predicate<T> match)
    {
        lock (_lock)
        {
            var results = 0;

            foreach (T item in _items.Where(item => match(item)))
            {
                _items.Remove(item);
                Removed(item);
                results++;
            }

            return results;
        }
    }
    public void RemoveRange(in int start, in int count)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            Guard.IsInRangeFor(count, (ICollection<T>)_items, nameof(count));

            for (int x = start; x < start + count; x++)
            {
                _items.RemoveAt(x);
                Removed(x);
            }
        }
    }


    public void CopyTo(T[] array)
    {
        lock (_lock) { _items.CopyTo(array, 0); }
    }


    public bool Exists(Predicate<T> match)
    {
        lock (_lock) { return _items.Exists(match); }
    }
    public T? Find(Predicate<T> match)
    {
        lock (_lock) { return _items.Find(match); }
    }
    public List<T> FindAll(Predicate<T> match)
    {
        lock (_lock) { return _items.FindAll(match); }
    }
    public int FindIndex(in int start, in int count, Predicate<T> match)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            Guard.IsInRangeFor(count, (ICollection<T>)_items, nameof(count));
            return _items.FindIndex(start, count, match);
        }
    }
    public int FindIndex(int start, Predicate<T> match)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            return _items.FindIndex(start, match);
        }
    }
    public int FindIndex(Predicate<T> match)
    {
        lock (_lock) { return _items.FindIndex(match); }
    }
    public T? FindLast(Predicate<T> match)
    {
        lock (_lock) { return _items.FindLast(match); }
    }
    public int FindLastIndex(in int start, in int count, Predicate<T> match)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            Guard.IsInRangeFor(count, (ICollection<T>)_items, nameof(count));
            return _items.FindLastIndex(start, count, match);
        }
    }
    public int FindLastIndex(in int start, Predicate<T> match)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            return _items.FindLastIndex(start, match);
        }
    }
    public int FindLastIndex(Predicate<T> match)
    {
        lock (_lock) { return _items.FindLastIndex(match); }
    }
    public int IndexOf(T value, in int start)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            return _items.IndexOf(value, start);
        }
    }
    public int IndexOf(T value, in int start, in int count)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            return _items.IndexOf(value, start, count);
        }
    }
    public int LastIndexOf(T value)
    {
        lock (_lock) { return _items.LastIndexOf(value); }
    }
    public int LastIndexOf(T value, in int start)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            return _items.LastIndexOf(value, start);
        }
    }
    public int LastIndexOf(T value, in int start, in int count)
    {
        lock (_lock)
        {
            Guard.IsInRangeFor(start, (ICollection<T>)_items, nameof(start));
            Guard.IsInRangeFor(count, (ICollection<T>)_items, nameof(count));
            return _items.LastIndexOf(value, start, count);
        }
    }


    public void Reverse()
    {
        lock (_lock)
        {
            _items.Reverse();
            Reset();
        }
    }
    public void Reverse(int start, int count)
    {
        lock (_lock)
        {
            _items.Reverse(start, count);
            Reset();
        }
    }


    public virtual void Sort() => Sort(_comparer);
    public virtual void Sort(in IComparer<T> comparer) => Sort(comparer.Compare);
    public virtual void Sort(in Comparison<T> compare)
    {
        lock (_lock)
        {
            if (_items.Count == 0) { return; }

            _items.Sort(compare);
            Reset();
        }
    }
    public virtual void Sort(in int start, in int count) => Sort(start, count, _comparer);
    public virtual void Sort(in int start, in int count, IComparer<T> comparer)
    {
        lock (_lock)
        {
            if (_items.Count == 0) { return; }

            _items.Sort(start, count, comparer);
            Reset();
        }
    }


    void ICollection.CopyTo(Array array, int start)
    {
        lock (_lock) { ((IList)_items).CopyTo(array, start); }
    }
    void IList.Remove(object? value)
    {
        lock (_lock) { ((IList)_items).Remove(value); }
    }
    int IList.Add(object? value)
    {
        lock (_lock) { return ((IList)_items).Add(value); }
    }
    bool IList.Contains(object? value)
    {
        lock (_lock) { return ((IList)_items).Contains(value); }
    }
    int IList.IndexOf(object? value)
    {
        lock (_lock) { return ((IList)_items).IndexOf(value); }
    }
    void IList.Insert(int index, object? value)
    {
        lock (_lock) { ((IList)_items).Insert(index, value); }
    }


    public virtual bool Contains(T item)
    {
        lock (_lock) { return _items.Contains(item); }
    }
    public virtual void Add(T item)
    {
        lock (_lock)
        {
            _items.Add(item);
            Added(item);
        }
    }

    public virtual void Clear()
    {
        lock (_lock)
        {
            _items.Clear();
            Reset();
        }
    }

    public void Insert(int index, T item)
    {
        lock (_lock)
        {
            _items.Insert(index, item);
            Added(item);
        }
    }

    public virtual bool Remove(T item)
    {
        lock (_lock)
        {
            bool result = _items.Remove(item);
            if (result) { Removed(item); }

            return result;
        }
    }
    public void RemoveAt(int index)
    {
        lock (_lock)
        {
            T item = this[index];
            _items.RemoveAt(index);
            Removed(item, index);
        }
    }


    public void CopyTo(T[] array, int start)
    {
        lock (_lock) { _items.CopyTo(array, start); }
    }
    public int IndexOf(T value)
    {
        lock (_lock) { return _items.IndexOf(value); }
    }


    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock) { return _items.GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
