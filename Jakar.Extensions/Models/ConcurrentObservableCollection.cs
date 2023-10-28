namespace Jakar.Extensions;


/// <summary>
///     <para> <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see> </para>
///     <para> <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see> </para>
/// </summary>
/// <typeparam name="T"> </typeparam>
[ Serializable ]
public class ConcurrentObservableCollection<T> : CollectionAlerts<T>, IList<T>, IReadOnlyList<T>, IList
{
    protected readonly List<T>      _values;
    protected readonly object       _lock = new();
    protected          IComparer<T> _comparer;


    public sealed override int Count
    {
        get
        {
            lock (_lock) { return _values.Count; }
        }
    }
    bool IList.IsFixedSize
    {
        get
        {
            lock (_lock) { return ((IList)_values).IsFixedSize; }
        }
    }
    bool IList.IsReadOnly
    {
        get
        {
            lock (_lock) { return ((IList)_values).IsReadOnly; }
        }
    }
    bool ICollection<T>.IsReadOnly
    {
        get
        {
            lock (_lock) { return ((IList)_values).IsReadOnly; }
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            lock (_lock) { return ((IList)_values).IsSynchronized; }
        }
    }


    object? IList.this[ int index ]

    {
        get
        {
            lock (_lock) { return ((IList)_values)[index]; }
        }
        set
        {
            lock (_lock) { ((IList)_values)[index] = value; }
        }
    }

    public T this[ int index ]
    {
        get
        {
            lock (_lock) { return _values[index]; }
        }
        set
        {
            lock (_lock)
            {
                T old = _values[index];
                _values[index] = value;
                Replaced( old, value, index );
            }
        }
    }
    public object Lock => _lock;
    object ICollection.SyncRoot
    {
        get
        {
            lock (_lock) { return ((IList)_values).SyncRoot; }
        }
    }


    public ConcurrentObservableCollection() : this( Comparer<T>.Default ) { }
    public ConcurrentObservableCollection( IComparer<T>   comparer ) : this( new List<T>(), comparer ) { }
    public ConcurrentObservableCollection( int            capacity ) : this( capacity, Comparer<T>.Default ) { }
    public ConcurrentObservableCollection( int            capacity, IComparer<T> comparer ) : this( new List<T>( capacity ), comparer ) { }
    public ConcurrentObservableCollection( IEnumerable<T> values ) : this( values, Comparer<T>.Default ) { }
    public ConcurrentObservableCollection( IEnumerable<T> values, IComparer<T> comparer ) : this( new List<T>( values ), comparer ) { }
    private ConcurrentObservableCollection( List<T>       values ) : this( values, Comparer<T>.Default ) { }
    protected ConcurrentObservableCollection( List<T> values, IComparer<T> comparer )
    {
        _comparer = comparer;
        _values   = values;
    }


    public static implicit operator ConcurrentObservableCollection<T>( List<T>                 values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( HashSet<T>              values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( ConcurrentBag<T>        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( ObservableCollection<T> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( Collection<T>           values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( T[]                     values ) => new(values);


    public bool Exists( Predicate<T> match )
    {
        lock (_lock) { return _values.Exists( match ); }
    }

    public virtual bool TryAdd( T item )
    {
        lock (_lock)
        {
            if ( _values.Contains( item ) ) { return false; }

            _values.Add( item );
            Added( item );
            return true;
        }
    }
    public int FindIndex( int start, int count, Predicate<T> match )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<T>)_values, nameof(count) );
            return _values.FindIndex( start, count, match );
        }
    }
    public int FindIndex( int start, Predicate<T> match )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            return _values.FindIndex( start, match );
        }
    }
    public int FindIndex( Predicate<T> match )
    {
        lock (_lock) { return _values.FindIndex( match ); }
    }
    public int FindLastIndex( int start, int count, Predicate<T> match )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<T>)_values, nameof(count) );
            return _values.FindLastIndex( start, count, match );
        }
    }
    public int FindLastIndex( int start, Predicate<T> match )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            return _values.FindLastIndex( start, match );
        }
    }
    public int FindLastIndex( Predicate<T> match )
    {
        lock (_lock) { return _values.FindLastIndex( match ); }
    }
    public int IndexOf( T value, int start )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            return _values.IndexOf( value, start );
        }
    }
    public int IndexOf( T value, int start, int count )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            return _values.IndexOf( value, start, count );
        }
    }
    public int LastIndexOf( T value )
    {
        lock (_lock) { return _values.LastIndexOf( value ); }
    }
    public int LastIndexOf( T value, int start )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            return _values.LastIndexOf( value, start );
        }
    }
    public int LastIndexOf( T value, int start, int count )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<T>)_values, nameof(count) );
            return _values.LastIndexOf( value, start, count );
        }
    }

    public int RemoveAll( Predicate<T> match )
    {
        lock (_lock)
        {
            int results = 0;

            foreach ( T item in _values.Where( item => match( item ) ) )
            {
                _values.Remove( item );
                Removed( item );
                results++;
            }

            return results;
        }
    }
    public List<T> FindAll( Predicate<T> match )
    {
        lock (_lock) { return _values.FindAll( match ); }
    }
    public T? Find( Predicate<T> match )
    {
        lock (_lock) { return _values.Find( match ); }
    }
    public T? FindLast( Predicate<T> match )
    {
        lock (_lock) { return _values.FindLast( match ); }
    }


    public virtual void Add( params T[] values )
    {
        lock (_lock)
        {
            foreach ( T item in values )
            {
                _values.Add( item );
                Added( item );
            }
        }
    }
    public virtual void Add( IEnumerable<T> values )
    {
        lock (_lock)
        {
            foreach ( T item in values )
            {
                _values.Add( item );
                Added( item );
            }
        }
    }


    public void CopyTo( T[] array )
    {
        lock (_lock) { _values.CopyTo( array, 0 ); }
    }
    public void InsertRange( int index, IEnumerable<T> collection )
    {
        lock (_lock)
        {
            foreach ( (int i, T? item) in collection.Enumerate( index ) )
            {
                _values.Insert( i, item );
                Added( item, i );
            }
        }
    }

    public void RemoveAt( int index, out T? item )
    {
        lock (_lock)
        {
            item = this[index];
            _values.RemoveAt( index );
            Removed( item, index );
        }
    }
    public void RemoveRange( int start, int count )
    {
        lock (_lock)
        {
            Guard.IsInRangeFor( start, (ICollection<T>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<T>)_values, nameof(count) );

            for ( int x = start; x < start + count; x++ )
            {
                _values.RemoveAt( x );
                Removed( x );
            }
        }
    }


    public void Reverse()
    {
        lock (_lock)
        {
            _values.Reverse();
            Reset();
        }
    }
    public void Reverse( int start, int count )
    {
        lock (_lock)
        {
            _values.Reverse( start, count );
            Reset();
        }
    }


    public virtual void Sort()                        => Sort( _comparer );
    public virtual void Sort( IComparer<T> comparer ) => Sort( comparer.Compare );
    public virtual void Sort( Comparison<T> compare )
    {
        lock (_lock)
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( compare );
            Reset();
        }
    }
    public virtual void Sort( int start, int count ) => Sort( start, count, _comparer );
    public virtual void Sort( int start, int count, IComparer<T> comparer )
    {
        lock (_lock)
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( start, count, comparer );
            Reset();
        }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        lock (_lock) { ((IList)_values).CopyTo( array, start ); }
    }
    void IList.Remove( object? value )
    {
        lock (_lock) { ((IList)_values).Remove( value ); }
    }
    int IList.Add( object? value )
    {
        lock (_lock) { return ((IList)_values).Add( value ); }
    }
    bool IList.Contains( object? value )
    {
        lock (_lock) { return ((IList)_values).Contains( value ); }
    }
    int IList.IndexOf( object? value )
    {
        lock (_lock) { return ((IList)_values).IndexOf( value ); }
    }
    void IList.Insert( int index, object? value )
    {
        lock (_lock) { ((IList)_values).Insert( index, value ); }
    }


    public virtual bool Contains( T item )
    {
        lock (_lock) { return _values.Contains( item ); }
    }
    public virtual void Add( T item )
    {
        lock (_lock)
        {
            _values.Add( item );
            Added( item );
        }
    }

    public virtual void Clear()
    {
        lock (_lock)
        {
            _values.Clear();
            Reset();
        }
    }

    public void Insert( int index, T item )
    {
        lock (_lock)
        {
            _values.Insert( index, item );
            Added( item );
        }
    }

    public virtual bool Remove( T item )
    {
        lock (_lock)
        {
            bool result = _values.Remove( item );
            if ( result ) { Removed( item ); }

            return result;
        }
    }
    public void RemoveAt( int index )
    {
        lock (_lock)
        {
            T item = this[index];
            _values.RemoveAt( index );
            Removed( item, index );
        }
    }


    public void CopyTo( T[] array, int start )
    {
        lock (_lock) { _values.CopyTo( array, start ); }
    }
    public int IndexOf( T value )
    {
        lock (_lock) { return _values.IndexOf( value ); }
    }


    public override IEnumerator<T> GetEnumerator()
    {
        lock (_lock) { return _values.Where( Filter ).GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
