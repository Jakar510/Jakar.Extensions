namespace Jakar.Extensions;


/// <summary>
///     <para> <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see> </para>
///     <para> <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see> </para>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
[ Serializable ]
public class ConcurrentObservableCollection<TValue> : CollectionAlerts<TValue>, ILockedCollection<TValue>, IAsyncEnumerable<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList, IDisposable
{
    protected readonly List<TValue>      _values;
    protected readonly Locker            _lock = Locker.Default;
    protected          IComparer<TValue> _comparer;

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


    public AsyncLockerEnumerator AsyncValues => new(this);

    public sealed override int Count
    {
        get
        {
            using ( AcquireLock() ) { return _values.Count; }
        }
    }

    public Locker Lock
    {
        get => _lock;
        init => _lock = value;
    }

    public LockerEnumerator<TValue> Values => new(this);

    object ICollection.SyncRoot
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)_values).SyncRoot; }
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


    public ConcurrentObservableCollection() : this( Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IComparer<TValue>   comparer ) : this( [], comparer ) { }
    public ConcurrentObservableCollection( int                 capacity ) : this( capacity, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( int                 capacity, IComparer<TValue> comparer ) : this( new List<TValue>( capacity ), comparer ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue> values ) : this( values, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue> values, IComparer<TValue> comparer ) : this( [..values], comparer ) { }
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

    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue> values ) => new(values);

    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue> values ) => new(values);

    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue> values ) => new(values);

    public static implicit operator ConcurrentObservableCollection<TValue>( ObservableCollection<TValue> values ) => new(values);

    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue> values ) => new(values);

    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[] values ) => new(values);

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

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected bool InternalTryAdd( in TValue value )
    {
        if ( _values.Contains( value ) ) { return false; }

        InternalAdd( value );
        return true;
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected void InternalAdd( in TValue value )
    {
        _values.Add( value );
        Added( value );
    }

    protected void InternalAdd( in ReadOnlySpan<TValue> values )
    {
        foreach ( TValue value in values ) { InternalAdd( value ); }
    }

    public virtual bool TryAdd( TValue value )
    {
        using ( AcquireLock() ) { return InternalTryAdd( value ); }
    }

    public virtual void Add( params TValue[] values ) => Add( new ReadOnlySpan<TValue>( values ) );

    public virtual void Add( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAdd( value ); }
        }
    }

    public void Add( ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd( values ); }
    }

    public void Add( ReadOnlyMemory<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd( values.Span ); }
    }

    public void Add( ImmutableArray<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd( values.AsSpan() ); }
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
        using ( await AcquireLockAsync( token ) ) { InternalAdd( values.Span ); }
    }

    public virtual async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalAdd( values.AsSpan() ); }
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

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected void InternalInsertRange( int i, TValue value )
    {
        _values.Insert( i, value );
        Added( value, i );
    }

    protected void InternalInsertRange( int index, IEnumerable<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsertRange( i, value ); }
    }

    protected void InternalInsertRange( int index, in ReadOnlySpan<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsertRange( i, value ); }
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

    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalInsertRange( index, collection.Span ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected void InternalRemoveRange( int start, int count )
    {
        Guard.IsInRangeFor( start, _values, nameof(start) );
        Guard.IsInRangeFor( count, _values, nameof(count) );

        for ( int x = start; x < start + count; x++ )
        {
            _values.RemoveAt( x );
            Removed( x );
        }
    }

    public void RemoveRange( int start, int count )
    {
        using ( AcquireLock() ) { InternalRemoveRange( start, count ); }
    }

    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalRemoveRange( start, count ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected bool InternalRemove( TValue value )
    {
        bool result = _values.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }

    protected int InternalRemove( IEnumerable<TValue> values )
    {
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( InternalRemove( value ) is false ) { continue; }

            results++;
        }

        return results;
    }

    protected int InternalRemove( in ReadOnlySpan<TValue> values )
    {
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( InternalRemove( value ) is false ) { continue; }

            results++;
        }

        return results;
    }

    public virtual int Remove( Func<TValue, bool> match ) => Remove( _values.Where( match ) );

    public virtual int Remove( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { return InternalRemove( values ); }
    }

    public virtual bool Remove( TValue value )
    {
        using ( AcquireLock() ) { return InternalRemove( value ); }
    }

    public virtual async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return InternalRemove( value ); }
    }

    public virtual ValueTask<int> RemoveAsync( Func<TValue, bool> match, CancellationToken token = default ) => RemoveAsync( _values.Where( match ), token );

    public virtual async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return InternalRemove( values ); }
    }

    public virtual async ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return InternalRemove( values.Span ); }
    }

    public virtual async ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return InternalRemove( values.AsSpan() ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected bool InternalRemoveAt( int index, [ NotNullWhen( true ) ] out TValue? value )
    {
        if ( index < 0 || index >= _values.Count )
        {
            value = default;
            return false;
        }

        value = _values[index];
        _values.RemoveAt( index );
        Removed( value, index );
        return value is not null;
    }

    public void RemoveAt( int index )
    {
        using ( AcquireLock() ) { InternalRemoveAt( index, out _ ); }
    }

    public void RemoveAt( int index, [ NotNullWhen( true ) ] out TValue? value )
    {
        using ( AcquireLock() ) { InternalRemoveAt( index, out value ); }
    }

    public async ValueTask<TValue?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            return InternalRemoveAt( index, out TValue? value )
                       ? value
                       : default;
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

    public virtual void Sort() => Sort( _comparer );

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

    public virtual ValueTask SortAsync( CancellationToken token = default ) => SortAsync( _comparer, token );

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

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected void InternalAdd( TValue value )
    {
        _values.Add( value );
        Added( value );
    }

    public virtual void Add( TValue value )
    {
        using ( AcquireLock() ) { InternalAdd( value ); }
    }

    public virtual async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalAdd( value ); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected void InternalClear()
    {
        _values.Clear();
        Reset();
    }

    public virtual void Clear()
    {
        using ( AcquireLock() ) { InternalClear(); }
    }

    public virtual async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalClear(); }
    }

    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    protected void InternalInsert( int index, TValue value )
    {
        _values.Insert( index, value );
        Added( value );
    }

    public void Insert( int index, TValue value )
    {
        using ( AcquireLock() ) { InternalInsert( index, value ); }
    }

    public async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { InternalInsert( index, value ); }
    }

    public IAsyncEnumerator<TValue> GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator( token );

    public override IEnumerator<TValue> GetEnumerator() => Values;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public IDisposable            AcquireLock()                               => _lock.Enter();
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public IDisposable            AcquireLock( in CancellationToken   token ) => _lock.Enter( token );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public ValueTask<IDisposable> AcquireLockAsync( CancellationToken token ) => _lock.EnterAsync( token );


    protected ReadOnlyMemory<TValue>                 FilteredValues() => _values.Where( Filter ).ToArray();
    ReadOnlyMemory<TValue> ILockedCollection<TValue>.Copy()           => Copy();

    protected ReadOnlyMemory<TValue> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }

    ConfiguredValueTaskAwaitable<ReadOnlyMemory<TValue>> ILockedCollection<TValue>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );

    protected async ValueTask<ReadOnlyMemory<TValue>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ) ) { return FilteredValues(); }
    }



    public sealed class AsyncLockerEnumerator( ConcurrentObservableCollection<TValue> collection ) : AsyncLockerEnumerator<TValue>( collection );
}
