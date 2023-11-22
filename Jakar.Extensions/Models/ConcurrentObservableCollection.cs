using Newtonsoft.Json.Linq;



namespace Jakar.Extensions;


/// <summary>
///     <para> <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see> </para>
///     <para> <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see> </para>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
[ Serializable ]
public class ConcurrentObservableCollection<TValue> : CollectionAlerts<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList, IAsyncDisposable, IDisposable
{
    protected readonly List<TValue>      _values;
    protected readonly Locker            _lock;
    protected          IComparer<TValue> _comparer;


    public sealed override int Count
    {
        get
        {
            using ( _lock.Enter() ) { return _values.Count; }
        }
    }
    bool IList.IsFixedSize
    {
        get
        {
            using ( _lock.Enter() ) { return ((IList)_values).IsFixedSize; }
        }
    }
    bool IList.IsReadOnly
    {
        get
        {
            using ( _lock.Enter() ) { return ((IList)_values).IsReadOnly; }
        }
    }
    bool ICollection<TValue>.IsReadOnly
    {
        get
        {
            using ( _lock.Enter() ) { return ((IList)_values).IsReadOnly; }
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            using ( _lock.Enter() ) { return ((IList)_values).IsSynchronized; }
        }
    }

    object? IList.this[ int index ]

    {
        get
        {
            using ( _lock.Enter() ) { return ((IList)_values)[index]; }
        }
        set
        {
            using ( _lock.Enter() ) { ((IList)_values)[index] = value; }
        }
    }

    public TValue this[ int index ]
    {
        get
        {
            using ( _lock.Enter() ) { return _values[index]; }
        }
        set
        {
            using ( _lock.Enter() )
            {
                TValue old = _values[index];
                _values[index] = value;
                Replaced( old, value, index );
            }
        }
    }
    public Locker Lock => _lock;
    object ICollection.SyncRoot
    {
        get
        {
            using ( _lock.Enter() ) { return ((IList)_values).SyncRoot; }
        }
    }


    public ConcurrentObservableCollection() : this( Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IComparer<TValue>   comparer ) : this( new List<TValue>(), comparer ) { }
    public ConcurrentObservableCollection( int                 capacity ) : this( capacity, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( int                 capacity, IComparer<TValue> comparer ) : this( new List<TValue>( capacity ), comparer ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue> values ) : this( values, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue> values, IComparer<TValue> comparer ) : this( new List<TValue>( values ), comparer ) { }
    private ConcurrentObservableCollection( List<TValue>       values ) : this( values, Comparer<TValue>.Default ) { }
    protected ConcurrentObservableCollection( List<TValue> values, IComparer<TValue> comparer )
    {
        _comparer = comparer;
        _values   = values;
        _lock     = new Locker( new SemaphoreSlim( 1 ) );
    }


    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue>                 values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue>              values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue>        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue>           values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[]                     values ) => new(values);


    public bool Exists( Predicate<TValue> match )
    {
        using ( _lock.Enter() ) { return _values.Exists( match ); }
    }

    public virtual bool TryAdd( TValue item )
    {
        using ( _lock.Enter() )
        {
            if ( _values.Contains( item ) ) { return false; }

            _values.Add( item );
            Added( item );
            return true;
        }
    }
    public int FindIndex( int start, int count, Predicate<TValue> match )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
            return _values.FindIndex( start, count, match );
        }
    }
    public int FindIndex( int start, Predicate<TValue> match )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            return _values.FindIndex( start, match );
        }
    }
    public int FindIndex( Predicate<TValue> match )
    {
        using ( _lock.Enter() ) { return _values.FindIndex( match ); }
    }
    public int FindLastIndex( int start, int count, Predicate<TValue> match )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
            return _values.FindLastIndex( start, count, match );
        }
    }
    public int FindLastIndex( int start, Predicate<TValue> match )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            return _values.FindLastIndex( start, match );
        }
    }
    public int FindLastIndex( Predicate<TValue> match )
    {
        using ( _lock.Enter() ) { return _values.FindLastIndex( match ); }
    }
    public int IndexOf( TValue value, int start )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            return _values.IndexOf( value, start );
        }
    }
    public int IndexOf( TValue value, int start, int count )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            return _values.IndexOf( value, start, count );
        }
    }
    public int LastIndexOf( TValue value )
    {
        using ( _lock.Enter() ) { return _values.LastIndexOf( value ); }
    }
    public int LastIndexOf( TValue value, int start )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            return _values.LastIndexOf( value, start );
        }
    }
    public int LastIndexOf( TValue value, int start, int count )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
            return _values.LastIndexOf( value, start, count );
        }
    }

    public int RemoveAll( Predicate<TValue> match )
    {
        using ( _lock.Enter() )
        {
            int results = 0;

            foreach ( TValue item in _values.Where( item => match( item ) ) )
            {
                _values.Remove( item );
                Removed( item );
                results++;
            }

            return results;
        }
    }
    public List<TValue> FindAll( Predicate<TValue> match )
    {
        using ( _lock.Enter() ) { return _values.FindAll( match ); }
    }
    public TValue? Find( Predicate<TValue> match )
    {
        using ( _lock.Enter() ) { return _values.Find( match ); }
    }
    public TValue? FindLast( Predicate<TValue> match )
    {
        using ( _lock.Enter() ) { return _values.FindLast( match ); }
    }


    public virtual void Add( params TValue[] values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public virtual void Add( IEnumerable<TValue> values )
    {
        using ( _lock.Enter() )
        {
            foreach ( TValue item in values )
            {
                _values.Add( item );
                Added( item );
            }
        }
    }
    public virtual void Add( ReadOnlySpan<TValue> values )
    {
        using ( _lock.Enter() ) { AddRange( values ); }
    }
    public virtual void Add( ImmutableArray<TValue> values )
    {
        using ( _lock.Enter() ) { AddRange( values.AsSpan() ); }
    }
    protected void AddRange( in ReadOnlySpan<TValue> values )
    {
        foreach ( TValue item in values )
        {
            _values.Add( item );
            Added( item );
        }
    }


    public virtual async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) ) { AddRange( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) ) { AddRange( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) )
        {
            foreach ( TValue item in values )
            {
                _values.Add( item );
                Added( item );
            }
        }
    }


    public void CopyTo( TValue[] array )
    {
        using ( _lock.Enter() ) { _values.CopyTo( array, 0 ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) ) { _values.CopyTo( array, 0 ); }
    }


    public void InsertRange( int index, IEnumerable<TValue> collection )
    {
        using ( _lock.Enter() )
        {
            foreach ( (int i, TValue? item) in collection.Enumerate( index ) )
            {
                _values.Insert( i, item );
                Added( item, i );
            }
        }
    }
    public void InsertRange( int index, ReadOnlySpan<TValue> collection )
    {
        using ( _lock.Enter() )
        {
            foreach ( (int i, TValue? item) in collection.Enumerate( index ) )
            {
                _values.Insert( i, item );
                Added( item, i );
            }
        }
    }
    public void InsertRange( int index, ReadOnlyMemory<TValue> collection ) => InsertRange( index, collection.Span );
    public void InsertRange( int index, ImmutableArray<TValue> collection ) => InsertRange( index, collection.AsSpan() );
    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) )
        {
            foreach ( (int i, TValue? item) in collection.Enumerate( index ) )
            {
                _values.Insert( i, item );
                Added( item, i );
            }
        }
    }


    public void RemoveAt( int index, out TValue? item )
    {
        using ( _lock.Enter() )
        {
            item = this[index];
            _values.RemoveAt( index );
            Removed( item, index );
        }
    }
    public void RemoveRange( int start, int count )
    {
        using ( _lock.Enter() )
        {
            Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
            Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );

            for ( int x = start; x < start + count; x++ )
            {
                _values.RemoveAt( x );
                Removed( x );
            }
        }
    }


    public void Reverse()
    {
        using ( _lock.Enter() )
        {
            _values.Reverse();
            Reset();
        }
    }
    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) )
        {
            _values.Reverse();
            Reset();
        }
    }
    public void Reverse( int start, int count )
    {
        using ( _lock.Enter() )
        {
            _values.Reverse( start, count );
            Reset();
        }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) )
        {
            _values.Reverse( start, count );
            Reset();
        }
    }


    public virtual void Sort()                             => Sort( _comparer );
    public virtual void Sort( IComparer<TValue> comparer ) => Sort( comparer.Compare );
    public virtual void Sort( Comparison<TValue> compare )
    {
        using ( _lock.Enter() )
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( compare );
            Reset();
        }
    }
    public virtual void Sort( int start, int count ) => Sort( start, count, _comparer );
    public virtual void Sort( int start, int count, IComparer<TValue> comparer )
    {
        using ( _lock.Enter() )
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( start, count, comparer );
            Reset();
        }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( _lock.Enter() ) { ((IList)_values).CopyTo( array, start ); }
    }
    void IList.Remove( object? value )
    {
        using ( _lock.Enter() ) { ((IList)_values).Remove( value ); }
    }
    int IList.Add( object? value )
    {
        using ( _lock.Enter() ) { return ((IList)_values).Add( value ); }
    }
    bool IList.Contains( object? value )
    {
        using ( _lock.Enter() ) { return ((IList)_values).Contains( value ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( _lock.Enter() ) { return ((IList)_values).IndexOf( value ); }
    }
    void IList.Insert( int index, object? value )
    {
        using ( _lock.Enter() ) { ((IList)_values).Insert( index, value ); }
    }


    public virtual bool Contains( TValue item )
    {
        using ( _lock.Enter() ) { return _values.Contains( item ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( TValue item, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) ) { return _values.Contains( item ); }
    }
    public virtual void Add( TValue item )
    {
        using ( _lock.Enter() )
        {
            _values.Add( item );
            Added( item );
        }
    }
    public virtual async ValueTask AddAsync( TValue item, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) )
        {
            _values.Add( item );
            Added( item );
        }
    }


    public virtual void Clear()
    {
        using ( _lock.Enter() )
        {
            _values.Clear();
            Reset();
        }
    }
    public virtual async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ) )
        {
            _values.Clear();
            Reset();
        }
    }

    public void Insert( int index, TValue item )
    {
        using ( _lock.Enter() )
        {
            _values.Insert( index, item );
            Added( item );
        }
    }

    public virtual bool Remove( TValue item )
    {
        using ( _lock.Enter() )
        {
            bool result = _values.Remove( item );
            if ( result ) { Removed( item ); }

            return result;
        }
    }
    public void RemoveAt( int index )
    {
        using ( _lock.Enter() )
        {
            TValue item = this[index];
            _values.RemoveAt( index );
            Removed( item, index );
        }
    }


    public void CopyTo( TValue[] array, int start )
    {
        using ( _lock.Enter() ) { _values.CopyTo( array, start ); }
    }
    public int IndexOf( TValue value )
    {
        using ( _lock.Enter() ) { return _values.IndexOf( value ); }
    }


    public override IEnumerator<TValue> GetEnumerator()
    {
        using ( _lock.Enter() ) { return _values.Where( Filter ).GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Dispose()
    {
        _lock.Dispose();
        GC.SuppressFinalize( this );
    }
    public async ValueTask DisposeAsync()
    {
        await _lock.DisposeAsync();
        GC.SuppressFinalize( this );
    }
}
