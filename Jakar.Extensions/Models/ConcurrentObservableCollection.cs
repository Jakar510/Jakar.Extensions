namespace Jakar.Extensions;


/// <summary>
///     <para> <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see> </para>
///     <para> <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see> </para>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
[ Serializable ]
public class ConcurrentObservableCollection<TValue> : CollectionAlerts<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList, IDisposable, IAsyncEnumerable<TValue>
{
    protected readonly List<TValue>      _values;
    protected readonly SemaphoreSlim     _lock = new(1);
    protected          IComparer<TValue> _comparer;


    public sealed override int Count
    {
        get
        {
            using ( AcquireLock() ) { return _values.Count; }
        }
    }
    bool IList.IsFixedSize
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values).IsFixedSize; }
        }
    }
    bool IList.IsReadOnly
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values).IsReadOnly; }
        }
    }
    bool ICollection<TValue>.IsReadOnly
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values).IsReadOnly; }
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values).IsSynchronized; }
        }
    }

    object? IList.this[ int index ]

    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values)[index]; }
        }
        set
        {
            using ( AcquireLock() ) { ((IList)_values)[index] = value; }
        }
    }

    public TValue this[ int index ]
    {
        get
        {
            using ( AcquireLock() ) { return _values[index]; }
        }
        set
        {
            using ( AcquireLock() )
            {
                TValue old = _values[index];
                _values[index] = value;
                Replaced( old, value, index );
            }
        }
    }
    public SemaphoreSlim Lock => _lock;
    object ICollection.SyncRoot
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values).SyncRoot; }
        }
    }
    public IEnumerable<TValue> Values => new Enumerator( this );


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
    }
    public virtual void Dispose()
    {
        _lock.Dispose();
        GC.SuppressFinalize( this );
    }


    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue>                 values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue>              values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue>        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue>           values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[]                     values ) => new(values);


    public bool Exists( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return _values.Exists( match ); }
    }
    public async ValueTask<bool> ExistsAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.Exists( match ); }
    }


    public int FindIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
        using ( AcquireLock() ) { return _values.FindIndex( start, count, match ); }
    }
    public int FindIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( AcquireLock() ) { return _values.FindIndex( start, match ); }
    }
    public int FindIndex( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return _values.FindIndex( match ); }
    }


    public async ValueTask<int> FindIndexAsync( int start, int count, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return _values.FindIndex( start, count, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return _values.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.FindIndex( match ); }
    }


    public int FindLastIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
        using ( AcquireLock() ) { return _values.FindLastIndex( start, count, match ); }
    }
    public int FindLastIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( AcquireLock() ) { return _values.FindLastIndex( start, match ); }
    }
    public int FindLastIndex( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return _values.FindLastIndex( match ); }
    }


    public async ValueTask<int> FindLastIndexAsync( int start, int count, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return _values.FindLastIndex( start, count, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( int start, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return _values.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.FindLastIndex( match ); }
    }


    public int IndexOf( TValue value )
    {
        using ( AcquireLock() ) { return _values.IndexOf( value ); }
    }
    public int IndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( AcquireLock() ) { return _values.IndexOf( value, start ); }
    }
    public int IndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( AcquireLock() ) { return _values.IndexOf( value, start, count ); }
    }


    public async ValueTask<int> IndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return _values.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return _values.IndexOf( value, start, count ); }
    }


    public int LastIndexOf( TValue value )
    {
        using ( AcquireLock() ) { return _values.LastIndexOf( value ); }
    }
    public int LastIndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( AcquireLock() ) { return _values.LastIndexOf( value, start ); }
    }
    public int LastIndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
        using ( AcquireLock() ) { return _values.LastIndexOf( value, start, count ); }
    }


    public async ValueTask<int> LastIndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return _values.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)_values, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)_values, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return _values.LastIndexOf( value, start, count ); }
    }


    public List<TValue> FindAll( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return _values.FindAll( match ); }
    }
    public async ValueTask<List<TValue>> FindAllAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.FindAll( match ); }
    }
    public TValue? Find( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return _values.Find( match ); }
    }
    public async ValueTask<TValue?> FindAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.Find( match ); }
    }
    public TValue? FindLast( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return _values.FindLast( match ); }
    }
    public async ValueTask<TValue?> FindLastAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.FindLast( match ); }
    }


    public virtual bool TryAdd( TValue value )
    {
        using ( AcquireLock() )
        {
            if ( _values.Contains( value ) ) { return false; }

            _values.Add( value );
            Added( value );
            return true;
        }
    }
    public virtual void Add( params TValue[] values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public virtual void Add( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values )
            {
                _values.Add( value );
                Added( value );
            }
        }
    }
    public virtual void Add( ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { AddRange( values ); }
    }
    public virtual void Add( ImmutableArray<TValue> values )
    {
        using ( AcquireLock() ) { AddRange( values.AsSpan() ); }
    }
    protected void AddRange( in ReadOnlySpan<TValue> values )
    {
        foreach ( TValue value in values )
        {
            _values.Add( value );
            Added( value );
        }
    }


    public virtual async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            if ( _values.Contains( value ) ) { return false; }

            _values.Add( value );
            Added( value );
            return true;
        }
    }
    public virtual async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { AddRange( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { AddRange( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            foreach ( TValue value in values )
            {
                _values.Add( value );
                Added( value );
            }
        }
    }


    public void CopyTo( TValue[] array )
    {
        using ( AcquireLock() ) { _values.CopyTo( array ); }
    }
    public void CopyTo( TValue[] array, int arrayIndex )
    {
        using ( AcquireLock() ) { _values.CopyTo( array, arrayIndex ); }
    }
    public void CopyTo( int index, TValue[] array, int arrayIndex, int count )
    {
        using ( AcquireLock() ) { _values.CopyTo( index, array, arrayIndex, count ); }
    }


    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { _values.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int arrayIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { _values.CopyTo( array, arrayIndex ); }
    }
    public async ValueTask CopyToAsync( int index, TValue[] array, int arrayIndex, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { _values.CopyTo( index, array, arrayIndex, count ); }
    }


    protected void InternalInsertRange( int index, IEnumerable<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) )
        {
            _values.Insert( i, value );
            Added( value, i );
        }
    }
    protected void InternalInsertRange( int index, ReadOnlySpan<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) )
        {
            _values.Insert( i, value );
            Added( value, i );
        }
    }


    public void InsertRange( int index, IEnumerable<TValue> collection )
    {
        using ( AcquireLock() ) { InternalInsertRange( index, collection ); }
    }
    public void InsertRange( int index, ReadOnlySpan<TValue> collection )
    {
        using ( AcquireLock() ) { InternalInsertRange( index, collection ); }
    }
    public void InsertRange( int index, ReadOnlyMemory<TValue> collection ) => InsertRange( index, collection.Span );
    public void InsertRange( int index, ImmutableArray<TValue> collection ) => InsertRange( index, collection.AsSpan() );


    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalInsertRange( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalInsertRange( index, collection.AsSpan() ); }
    }


    public void RemoveRange( int start, int count )
    {
        using ( AcquireLock() )
        {
            Guard.IsInRangeFor( start, _values, nameof(start) );
            Guard.IsInRangeFor( count, _values, nameof(count) );

            for ( int x = start; x < start + count; x++ )
            {
                _values.RemoveAt( x );
                Removed( x );
            }
        }
    }
    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            Guard.IsInRangeFor( start, _values, nameof(start) );
            Guard.IsInRangeFor( count, _values, nameof(count) );

            for ( int x = start; x < start + count; x++ )
            {
                _values.RemoveAt( x );
                Removed( x );
            }
        }
    }


    public virtual int Remove( Predicate<TValue> match ) => Remove( _values.Where( value => match( value ) ) );
    public virtual int Remove( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            int results = 0;

            foreach ( TValue value in values )
            {
                if ( _values.Remove( value ) is false ) { continue; }

                Removed( value );
                results++;
            }

            return results;
        }
    }
    public virtual bool Remove( TValue value )
    {
        using ( AcquireLock() )
        {
            bool result = _values.Remove( value );
            if ( result ) { Removed( value ); }

            return result;
        }
    }


    public virtual async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            bool result = _values.Remove( value );
            if ( result ) { Removed( value ); }

            return result;
        }
    }
    public virtual ValueTask<int> RemoveAsync( Predicate<TValue> match, CancellationToken token = default ) => RemoveAsync( _values.Where( value => match( value ) ), token );
    public virtual async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            int results = 0;

            foreach ( TValue value in values )
            {
                if ( _values.Remove( value ) is false ) { continue; }

                Removed( value );
                results++;
            }

            return results;
        }
    }


    public void RemoveAt( int index )
    {
        using ( AcquireLock() )
        {
            TValue value = this[index];
            _values.RemoveAt( index );
            Removed( value, index );
        }
    }
    public void RemoveAt( int index, out TValue value )
    {
        using ( AcquireLock() )
        {
            value = this[index];
            _values.RemoveAt( index );
            Removed( value, index );
        }
    }
    public async ValueTask<TValue> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            TValue value = this[index];
            _values.RemoveAt( index );
            Removed( value, index );
            return value;
        }
    }


    public void Reverse()
    {
        using ( AcquireLock() )
        {
            _values.Reverse();
            Reset();
        }
    }
    public void Reverse( int start, int count )
    {
        using ( AcquireLock() )
        {
            _values.Reverse( start, count );
            Reset();
        }
    }


    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            _values.Reverse();
            Reset();
        }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            _values.Reverse( start, count );
            Reset();
        }
    }


    public virtual void Sort()                             => Sort( _comparer );
    public virtual void Sort( IComparer<TValue> comparer ) => Sort( comparer.Compare );
    public virtual void Sort( Comparison<TValue> compare )
    {
        using ( AcquireLock() )
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( compare );
            Reset();
        }
    }
    public virtual void Sort( int start, int count ) => Sort( start, count, _comparer );
    public virtual void Sort( int start, int count, IComparer<TValue> comparer )
    {
        using ( AcquireLock() )
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( start, count, comparer );
            Reset();
        }
    }


    public virtual ValueTask SortAsync( CancellationToken token                             = default ) => SortAsync( _comparer,        token );
    public virtual ValueTask SortAsync( IComparer<TValue> comparer, CancellationToken token = default ) => SortAsync( comparer.Compare, token );
    public virtual async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( compare );
            Reset();
        }
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, _comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<TValue> comparer, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            if ( _values.Count == 0 ) { return; }

            _values.Sort( start, count, comparer );
            Reset();
        }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() ) { ((IList)_values).CopyTo( array, start ); }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() ) { ((IList)_values).Remove( value ); }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() ) { return ((IList)_values).Add( value ); }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return ((IList)_values).Contains( value ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() ) { return ((IList)_values).IndexOf( value ); }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() ) { ((IList)_values).Insert( index, value ); }
    }


    public virtual bool Contains( TValue value )
    {
        using ( AcquireLock() ) { return _values.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return _values.Contains( value ); }
    }
    public virtual void Add( TValue value )
    {
        using ( AcquireLock() )
        {
            _values.Add( value );
            Added( value );
        }
    }
    public virtual async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            _values.Add( value );
            Added( value );
        }
    }


    public virtual void Clear()
    {
        using ( AcquireLock() )
        {
            _values.Clear();
            Reset();
        }
    }
    public virtual async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            _values.Clear();
            Reset();
        }
    }


    public void Insert( int index, TValue value )
    {
        using ( AcquireLock() )
        {
            _values.Insert( index, value );
            Added( value );
        }
    }
    public async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            _values.Insert( index, value );
            Added( value );
        }
    }


    public AsyncEnumerator                            GetAsyncEnumerator( CancellationToken token = default ) => new(this, token);
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token )           => GetAsyncEnumerator( token );
    public override IEnumerator<TValue>               GetEnumerator()                                         => new Enumerator( this );
    IEnumerator IEnumerable.                          GetEnumerator()                                         => GetEnumerator();


    protected internal LockContext AcquireLock()
    {
        _lock.Wait();
        return new LockContext( _lock );
    }
    protected internal LockContext AcquireLock( in CancellationToken token )
    {
        _lock.Wait( token );
        return new LockContext( _lock );
    }
    protected internal LockContext AcquireLock( in TimeSpan span )
    {
        _lock.Wait( span );
        return new LockContext( _lock );
    }
    protected internal LockContext AcquireLock( in TimeSpan span, in CancellationToken token )
    {
        _lock.Wait( span, token );
        return new LockContext( _lock );
    }


    protected internal async ValueTask<LockContext> AcquireLockAsync( CancellationToken token )
    {
        await _lock.WaitAsync( token );
        return new LockContext( _lock );
    }
    protected internal async ValueTask<LockContext> AcquireLockAsync( TimeSpan span, CancellationToken token )
    {
        await _lock.WaitAsync( span, token );
        return new LockContext( _lock );
    }


    protected override bool Filter( TValue? value ) => value is not null;
    internal bool Filter( ref int index, out TValue? current )
    {
        do
        {
            index++;

            current = index < _values.Count
                          ? _values[index]
                          : default;
        } while ( Filter( current ) );

        return index < _values.Count;
    }


#pragma warning disable IDE0064 // Make readonly fields writable
    public struct AsyncEnumerator : IAsyncEnumerator<TValue>
    {
        private readonly ConcurrentObservableCollection<TValue> _collection;
        private readonly CancellationToken                      _token;
        private          int                                    _index   = -1;
        private          TValue?                                _current = default;

        public readonly TValue Current => _current ?? throw new NullReferenceException( nameof(_current) );


        public AsyncEnumerator( ConcurrentObservableCollection<TValue> collection, CancellationToken token )
        {
            _collection = collection;
            _token      = token;
        }
        public ValueTask DisposeAsync()
        {
            this = default;
            return default;
        }


        public async ValueTask<bool> MoveNextAsync()
        {
            using ( await _collection.AcquireLockAsync( _token ) ) { return _collection.Filter( ref _index, out _current ); }
        }
        public void Reset() => _index = -1;
    }



    public struct Enumerator : IEnumerator<TValue>, IEnumerable<TValue>
    {
        private readonly ConcurrentObservableCollection<TValue> _collection;
        private          int                                    _index   = -1;
        private          TValue?                                _current = default;

        public readonly TValue             Current => _current ?? throw new NullReferenceException( nameof(_current) );
        readonly        object IEnumerator.Current => Current  ?? throw new NullReferenceException( nameof(_current) );


        public Enumerator( ConcurrentObservableCollection<TValue> collection ) => _collection = collection;


        public void Dispose() => this = default;
        public bool MoveNext()
        {
            using ( _collection.AcquireLock() ) { return _collection.Filter( ref _index, out _current ); }
        }
        public void Reset() => _index = -1;


        readonly IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;
        readonly IEnumerator IEnumerable.                GetEnumerator() => this;
    }
#pragma warning restore IDE0064 // Make readonly fields writable



    protected internal readonly record struct LockContext( SemaphoreSlim Locker ) : IDisposable
    {
        public void Dispose() => Locker.Release();
    }
}
