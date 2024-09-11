namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see>
///     </para>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
[Serializable]
public class ConcurrentObservableCollection<TValue>( IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer, int capacity = DEFAULT_CAPACITY ) : ObservableCollection<TValue>( comparer, equalityComparer, capacity ), IList, ILockedCollection<TValue, AsyncLockerEnumerator<TValue>, LockerEnumerator<TValue>>
{
    protected internal readonly Locker locker = Locker.Default;


    public AsyncLockerEnumerator<TValue> AsyncValues    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }
    public Locker                        Lock           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; init => locker = value; }
    object ICollection.                  SyncRoot       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; }
    bool ICollection.                    IsSynchronized { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => true; }
    public LockerEnumerator<TValue>      Values         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }


    public ConcurrentObservableCollection() : this( Comparer<TValue>.Default, EqualityComparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( int                            capacity ) : this( Comparer<TValue>.Default, EqualityComparer<TValue>.Default, capacity ) { }
    public ConcurrentObservableCollection( scoped in ReadOnlySpan<TValue> values ) : this( values.Length ) => InternalAdd( values );
    public ConcurrentObservableCollection( scoped in ReadOnlySpan<TValue> values, IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer ) : this( comparer, equalityComparer, values.Length ) => InternalAdd( values );
    public ConcurrentObservableCollection( IEnumerable<TValue>            values ) : this( values, Comparer<TValue>.Default, EqualityComparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue>            values, IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer ) : this( comparer, equalityComparer ) => InternalAdd( values );
    public ConcurrentObservableCollection( TValue[]                       values ) : this( values.Length ) => InternalAdd( values.AsSpan() );
    public ConcurrentObservableCollection( TValue[]                       values, IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer ) : this( values.AsSpan(), comparer, equalityComparer ) { }


    public override void Dispose()
    {
        locker.Dispose();
        GC.SuppressFinalize( this );
    }

    // public static implicit operator ConcurrentObservableCollection<T>( MemoryBuffer<T>                                        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue>                                                values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue>                                             values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue>                                       values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( System.Collections.ObjectModel.ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue>                                          values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[]                                                    values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlyMemory<TValue>                                      values ) => new(values.Span);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlySpan<TValue>                                        values ) => new(values);


    public override void Set( int index, TValue value )
    {
        using ( AcquireLock() ) { InternalSet( index, value ); }
    }
    public override TValue Get( int index )
    {
        using ( AcquireLock() ) { return InternalGet( index ); }
    }


    public override bool Exists( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return buffer.FindIndex( match ) >= 0; }
    }
    public async ValueTask<bool> ExistsAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( match ) >= 0; }
    }


    public override int FindIndex( Predicate<TValue> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return buffer.FindIndex( start, endInclusive, match ); }
    }
    public override int FindIndex( Predicate<TValue> match, int start = 0 )
    {
        using ( AcquireLock() ) { return buffer.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( start, count, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( match ); }
    }


    public override int FindLastIndex( Predicate<TValue> match, int start, int count )
    {
        using ( AcquireLock() ) { return buffer.FindLastIndex( start, count, match ); }
    }
    public override int FindLastIndex( Predicate<TValue> match, int start = 0 )
    {
        using ( AcquireLock() ) { return buffer.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<TValue> match, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLastIndex( start, count, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<TValue> match, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLastIndex( match ); }
    }


    public override int IndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return buffer.IndexOf( value, start ); }
    }
    public override int IndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return buffer.IndexOf( value, start, count ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.IndexOf( value, start, count ); }
    }


    public override int LastIndexOf( TValue value )
    {
        using ( AcquireLock() ) { return buffer.LastIndexOf( value ); }
    }
    public override int LastIndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return buffer.LastIndexOf( value, start ); }
    }
    public override int LastIndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return buffer.LastIndexOf( value, start, count ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.LastIndexOf( value, start, count ); }
    }


    public override TValue[] FindAll( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return [..buffer.FindAll( match )]; }
    }
    public async ValueTask<TValue[]> FindAllAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return [..buffer.FindAll( match )]; }
    }
    public override TValue? Find( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return buffer.Find( match ); }
    }
    public async ValueTask<TValue?> FindAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.Find( match ); }
    }
    public override TValue? FindLast( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return buffer.FindLast( match ); }
    }
    public async ValueTask<TValue?> FindLastAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLast( match ); }
    }


    public override bool TryAdd( TValue value )
    {
        using ( AcquireLock() ) { return InternalTryAdd( value ); }
    }
    public override void Add( params TValue[] values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public override void Add( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd( values ); }
    }
    public override void Add( SpanEnumerable<TValue, EnumerableProducer<TValue>> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAdd( value ); }
        }
    }
    public override void Add( scoped in ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd( values ); }
    }


    public override void AddOrUpdate( TValue value )
    {
        using ( AcquireLock() ) { InternalAddOrUpdate( value ); }
    }
    public override void AddOrUpdate( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAddOrUpdate( value ); }
        }
    }
    public override void AddOrUpdate( scoped in ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAddOrUpdate( value ); }
        }
    }
    public virtual async ValueTask AddOrUpdate( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { InternalAddOrUpdate( value ); }
        }
    }


    public virtual async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalTryAdd( value ); }
    }
    public virtual async ValueTask TryAddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            foreach ( TValue value in values ) { InternalTryAdd( value ); }
        }
    }
    public virtual async ValueTask TryAddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { InternalTryAdd( value ); }
        }
    }
    public virtual async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( values ); }
    }
    public virtual async ValueTask AddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { InternalAdd( value ); }
        }
    }


    public override void CopyTo( TValue[] array )
    {
        using ( AcquireLock() ) { buffer.CopyTo( array ); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex )
    {
        using ( AcquireLock() ) { buffer.CopyTo( array, destinationStartIndex ); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 )
    {
        using ( AcquireLock() ) { buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { buffer.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { buffer.CopyTo( array, destinationStartIndex ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex ); }
    }


    public override void InsertRange( int index, IEnumerable<TValue> collection )
    {
        using ( AcquireLock() ) { InternalInsert( index, collection ); }
    }
    public override void InsertRange( int index, scoped in ReadOnlySpan<TValue> collection )
    {
        using ( AcquireLock() ) { InternalInsert( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IAsyncEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( (int i, TValue value) in collection.Enumerate( index ).WithCancellation( token ) ) { InternalInsert( i, value ); }
        }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, collection.AsSpan() ); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, collection.Span ); }
    }


    public override void RemoveRange( int start, int count )
    {
        using ( AcquireLock() ) { InternalRemoveRange( start, count ); }
    }
    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalRemoveRange( start, count ); }
    }


    public override int Remove( Func<TValue, bool> match )
    {
        using ( AcquireLock() ) { return InternalRemove( match ); }
    }
    public override int Remove( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { return InternalRemove( values ); }
    }
    public override bool Remove( TValue value )
    {
        using ( AcquireLock() ) { return InternalRemove( value ); }
    }


    public virtual async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( value ); }
    }
    public virtual async ValueTask<int> RemoveAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( buffer.Where( match ) ); }
    }
    public virtual async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( values ); }
    }
    public virtual async ValueTask RemoveAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { InternalRemove( value ); }
        }
    }
    public virtual async ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( values.Span ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( values.AsSpan() ); }
    }


    public override void RemoveAt( int index )
    {
        using ( AcquireLock() ) { InternalRemoveAt( index, out _ ); }
    }
    public override bool RemoveAt( int index, [NotNullWhen( true )] out TValue? value )
    {
        using ( AcquireLock() ) { return InternalRemoveAt( index, out value ); }
    }
    public async ValueTask<TValue?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            return InternalRemoveAt( index, out TValue? value )
                       ? value
                       : default;
        }
    }


    public override void Reverse()
    {
        using ( AcquireLock() ) { InternalReverse(); }
    }
    public override void Reverse( int start, int count )
    {
        using ( AcquireLock() ) { InternalReverse( start, count ); }
    }
    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalReverse(); }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalReverse( start, count ); }
    }


    public override void Sort( IComparer<TValue> compare )
    {
        using ( AcquireLock() ) { InternalSort( compare ); }
    }
    public override void Sort( Comparison<TValue> compare )
    {
        using ( AcquireLock() ) { InternalSort( compare ); }
    }
    public override void Sort( int start, int count ) => Sort( start, count, comparer );
    public override void Sort( int start, int count, IComparer<TValue> compare )
    {
        using ( AcquireLock() ) { InternalSort( start, count, compare ); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync( comparer, token );
    public async ValueTask SortAsync( IComparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalSort( compare ); }
    }
    public virtual async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalSort( compare ); }
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalSort( start, count, compare ); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() )
        {
            if ( array is TValue[] x ) { buffer.CopyTo( x, start ); }
        }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is TValue x ) { buffer.Remove( x ); }
        }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is not TValue x ) { return NOT_FOUND; }

            buffer.Add( x );
            return Count;
        }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return value is TValue x && buffer.Contains( x ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() )
        {
            return value is TValue x
                       ? buffer.IndexOf( x )
                       : NOT_FOUND;
        }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is TValue x ) { buffer.Insert( index, x ); }
        }
    }


    public override bool Contains( TValue value )
    {
        using ( AcquireLock() ) { return buffer.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.Contains( value ); }
    }


    public override void Add( TValue value )
    {
        using ( AcquireLock() ) { InternalAdd( value ); }
    }
    public virtual async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( value ); }
    }


    public override void Clear()
    {
        using ( AcquireLock() ) { InternalClear(); }
    }
    public virtual async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalClear(); }
    }


    public override void Insert( int index, TValue value )
    {
        using ( AcquireLock() ) { InternalInsert( index, value ); }
    }
    public async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, value ); }
    }


    public AsyncLockerEnumerator<TValue>              GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator( token );
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public override LockerEnumerator<TValue>          GetEnumerator()                               => Values;
    IEnumerator IEnumerable.                          GetEnumerator()                               => GetEnumerator();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock()                                                                                     => locker.Enter();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( scoped in CancellationToken token )                                                  => locker.Enter( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( ref       bool              lockTaken, scoped in CancellationToken token = default ) => locker.Enter( ref lockTaken, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask<Closer> AcquireLockAsync( CancellationToken      token ) => locker.EnterAsync( token );


    protected internal ReadOnlySpan<TValue> AsSpan( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return buffer.ToArray(); }
    }
    protected internal ReadOnlyMemory<TValue> AsMemory( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return buffer.ToArray(); }
    }


    protected internal FilterBuffer<TValue> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    FilterBuffer<TValue> ILockedCollection<TValue>.                              Copy()                               => Copy();
    ConfiguredValueTaskAwaitable<FilterBuffer<TValue>> ILockedCollection<TValue>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );
    protected async ValueTask<FilterBuffer<TValue>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return FilteredValues(); }
    }
}
