namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see>
///     </para>
/// </summary>
/// <typeparam name="T"> </typeparam>
[Serializable]
public class ConcurrentObservableCollection<T>( IComparer<T> comparer, IEqualityComparer<T> equalityComparer, int capacity = DEFAULT_CAPACITY ) : ObservableCollection<T>( comparer, equalityComparer, capacity ), IList, ILockedCollection<T, AsyncLockerEnumerator<T>, LockerEnumerator<T>>
{
    protected internal readonly Locker locker = Locker.Default;


    public AsyncLockerEnumerator<T> AsyncValues    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }
    public Locker                   Lock           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; init => locker = value; }
    object ICollection.             SyncRoot       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; }
    bool ICollection.               IsSynchronized { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => true; }
    public LockerEnumerator<T>      Values         { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }


    public ConcurrentObservableCollection() : this( Comparer<T>.Default, EqualityComparer<T>.Default ) { }
    public ConcurrentObservableCollection( int                       capacity ) : this( Comparer<T>.Default, EqualityComparer<T>.Default, capacity ) { }
    public ConcurrentObservableCollection( scoped in ReadOnlySpan<T> values ) : this( values.Length ) => InternalAdd( values );
    public ConcurrentObservableCollection( scoped in ReadOnlySpan<T> values, IComparer<T> comparer, IEqualityComparer<T> equalityComparer ) : this( comparer, equalityComparer, values.Length ) => InternalAdd( values );
    public ConcurrentObservableCollection( IEnumerable<T>            values ) : this( values, Comparer<T>.Default, EqualityComparer<T>.Default ) { }
    public ConcurrentObservableCollection( IEnumerable<T>            values, IComparer<T> comparer, IEqualityComparer<T> equalityComparer ) : this( comparer, equalityComparer ) => InternalAdd( values );
    public ConcurrentObservableCollection( T[]                       values ) : this( values.Length ) => InternalAdd( values.AsSpan() );
    public ConcurrentObservableCollection( T[]                       values, IComparer<T> comparer, IEqualityComparer<T> equalityComparer ) : this( values.AsSpan(), comparer, equalityComparer ) { }


    public override void Dispose()
    {
        locker.Dispose();
        GC.SuppressFinalize( this );
    }

    // public static implicit operator ConcurrentObservableCollection<T>( MemoryBuffer<T>                                        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( List<T>                                                values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( HashSet<T>                                             values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( ConcurrentBag<T>                                       values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( System.Collections.ObjectModel.ObservableCollection<T> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( Collection<T>                                          values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( T[]                                                    values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( ReadOnlyMemory<T>                                      values ) => new(values.Span);
    public static implicit operator ConcurrentObservableCollection<T>( ReadOnlySpan<T>                                        values ) => new(values);


    public override void Set( int index, T value )
    {
        using ( AcquireLock() ) { InternalSet( index, value ); }
    }
    public override ref T Get( int index )
    {
        using ( AcquireLock() ) { return ref InternalGet( index ); }
    }


    public override bool Exists( Predicate<T> match )
    {
        using ( AcquireLock() ) { return buffer.FindIndex( match ) >= 0; }
    }
    public async ValueTask<bool> ExistsAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( match ) >= 0; }
    }


    public override int FindIndex( Predicate<T> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return buffer.FindIndex( start, endInclusive, match ); }
    }
    public override int FindIndex( Predicate<T> match, int start = 0 )
    {
        using ( AcquireLock() ) { return buffer.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, Predicate<T> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( start, count, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Predicate<T> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindIndex( match ); }
    }


    public override int FindLastIndex( Predicate<T> match, int start, int count )
    {
        using ( AcquireLock() ) { return buffer.FindLastIndex( start, count, match ); }
    }
    public override int FindLastIndex( Predicate<T> match, int start = 0 )
    {
        using ( AcquireLock() ) { return buffer.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<T> match, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLastIndex( start, count, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<T> match, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLastIndex( match ); }
    }


    public override int IndexOf( T value, int start )
    {
        using ( AcquireLock() ) { return buffer.IndexOf( value, start ); }
    }
    public override int IndexOf( T value, int start, int count )
    {
        using ( AcquireLock() ) { return buffer.IndexOf( value, start, count ); }
    }
    public async ValueTask<int> IndexOfAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( T value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( T value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.IndexOf( value, start, count ); }
    }


    public override int LastIndexOf( T value )
    {
        using ( AcquireLock() ) { return buffer.LastIndexOf( value ); }
    }
    public override int LastIndexOf( T value, int start )
    {
        using ( AcquireLock() ) { return buffer.LastIndexOf( value, start ); }
    }
    public override int LastIndexOf( T value, int start, int count )
    {
        using ( AcquireLock() ) { return buffer.LastIndexOf( value, start, count ); }
    }
    public async ValueTask<int> LastIndexOfAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( T value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( T value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.LastIndexOf( value, start, count ); }
    }


    public override T[] FindAll( Predicate<T> match )
    {
        using ( AcquireLock() ) { return [..buffer.FindAll( match )]; }
    }
    public async ValueTask<T[]> FindAllAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return [..buffer.FindAll( match )]; }
    }
    public override T? Find( Predicate<T> match )
    {
        using ( AcquireLock() ) { return buffer.Find( match ); }
    }
    public async ValueTask<T?> FindAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.Find( match ); }
    }
    public override T? FindLast( Predicate<T> match )
    {
        using ( AcquireLock() ) { return buffer.FindLast( match ); }
    }
    public async ValueTask<T?> FindLastAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.FindLast( match ); }
    }


    public override bool TryAdd( T value )
    {
        using ( AcquireLock() ) { return InternalTryAdd( value ); }
    }
    public override void Add( params T[] values ) => Add( new ReadOnlySpan<T>( values ) );
    public override void Add( IEnumerable<T> values )
    {
        using ( AcquireLock() ) { InternalAdd( values ); }
    }
    public override void Add( SpanEnumerable<T, EnumerableProducer<T>> values )
    {
        using ( AcquireLock() )
        {
            foreach ( T value in values ) { InternalAdd( value ); }
        }
    }
    public override void Add( scoped in ReadOnlySpan<T> values )
    {
        using ( AcquireLock() ) { InternalAdd( values ); }
    }


    public override void AddOrUpdate( T value )
    {
        using ( AcquireLock() ) { InternalAddOrUpdate( value ); }
    }
    public override void AddOrUpdate( IEnumerable<T> values )
    {
        using ( AcquireLock() )
        {
            foreach ( T value in values ) { InternalAddOrUpdate( value ); }
        }
    }
    public override void AddOrUpdate( scoped in ReadOnlySpan<T> values )
    {
        using ( AcquireLock() )
        {
            foreach ( T value in values ) { InternalAddOrUpdate( value ); }
        }
    }
    public virtual async ValueTask AddOrUpdate( IAsyncEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( T value in values.WithCancellation( token ) ) { InternalAddOrUpdate( value ); }
        }
    }


    public virtual async ValueTask<bool> TryAddAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalTryAdd( value ); }
    }
    public virtual async ValueTask TryAddAsync( IEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            foreach ( T value in values ) { InternalTryAdd( value ); }
        }
    }
    public virtual async ValueTask TryAddAsync( IAsyncEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( T value in values.WithCancellation( token ) ) { InternalTryAdd( value ); }
        }
    }
    public virtual async ValueTask AddAsync( ReadOnlyMemory<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalAdd( values ); }
    }
    public virtual async ValueTask AddAsync( IAsyncEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( T value in values.WithCancellation( token ) ) { InternalAdd( value ); }
        }
    }


    public override void CopyTo( T[] array )
    {
        using ( AcquireLock() ) { buffer.CopyTo( array ); }
    }
    public override void CopyTo( T[] array, int destinationStartIndex )
    {
        using ( AcquireLock() ) { buffer.CopyTo( array, destinationStartIndex ); }
    }
    public override void CopyTo( T[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 )
    {
        using ( AcquireLock() ) { buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex ); }
    }
    public async ValueTask CopyToAsync( T[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { buffer.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( T[] array, int destinationStartIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { buffer.CopyTo( array, destinationStartIndex ); }
    }
    public async ValueTask CopyToAsync( T[] array, int destinationStartIndex, int length, int sourceStartIndex = 0, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex ); }
    }


    public override void InsertRange( int index, IEnumerable<T> collection )
    {
        using ( AcquireLock() ) { InternalInsert( index, collection ); }
    }
    public override void InsertRange( int index, scoped in ReadOnlySpan<T> collection )
    {
        using ( AcquireLock() ) { InternalInsert( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<T> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IAsyncEnumerable<T> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( (int i, T value) in collection.Enumerate( index ).WithCancellation( token ) ) { InternalInsert( i, value ); }
        }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<T> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, collection.AsSpan() ); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<T> collection, CancellationToken token = default )
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


    public override int Remove( Func<T, bool> match )
    {
        using ( AcquireLock() ) { return InternalRemove( match ); }
    }
    public override int Remove( IEnumerable<T> values )
    {
        using ( AcquireLock() ) { return InternalRemove( values ); }
    }
    public override bool Remove( T value )
    {
        using ( AcquireLock() ) { return InternalRemove( value ); }
    }


    public virtual async ValueTask<bool> RemoveAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( value ); }
    }
    public virtual async ValueTask<int> RemoveAsync( Func<T, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( buffer.Where( match ) ); }
    }
    public virtual async ValueTask<int> RemoveAsync( IEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( values ); }
    }
    public virtual async ValueTask RemoveAsync( IAsyncEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( T value in values.WithCancellation( token ) ) { InternalRemove( value ); }
        }
    }
    public virtual async ValueTask<int> RemoveAsync( ReadOnlyMemory<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( values.Span ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ImmutableArray<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return InternalRemove( values.AsSpan() ); }
    }


    public override void RemoveAt( int index )
    {
        using ( AcquireLock() ) { InternalRemoveAt( index, out _ ); }
    }
    public override bool RemoveAt( int index, [NotNullWhen( true )] out T? value )
    {
        using ( AcquireLock() ) { return InternalRemoveAt( index, out value ); }
    }
    public async ValueTask<T?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            return InternalRemoveAt( index, out T? value )
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


    public override void Sort( IComparer<T> compare )
    {
        using ( AcquireLock() ) { InternalSort( compare ); }
    }
    public override void Sort( Comparison<T> compare )
    {
        using ( AcquireLock() ) { InternalSort( compare ); }
    }
    public override void Sort( int start, int count ) => Sort( start, count, comparer );
    public override void Sort( int start, int count, IComparer<T> compare )
    {
        using ( AcquireLock() ) { InternalSort( start, count, compare ); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync( comparer, token );
    public async ValueTask SortAsync( IComparer<T> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalSort( compare ); }
    }
    public virtual async ValueTask SortAsync( Comparison<T> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalSort( compare ); }
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<T> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalSort( start, count, compare ); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() )
        {
            if ( array is T[] x ) { buffer.CopyTo( x, start ); }
        }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is T x ) { buffer.Remove( x ); }
        }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is not T x ) { return NOT_FOUND; }

            buffer.Add( x );
            return Count;
        }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return value is T x && buffer.Contains( x ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() )
        {
            return value is T x
                       ? buffer.IndexOf( x )
                       : NOT_FOUND;
        }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is T x ) { buffer.Insert( index, x ); }
        }
    }


    public override bool Contains( T value )
    {
        using ( AcquireLock() ) { return buffer.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return buffer.Contains( value ); }
    }


    public override void Add( T value )
    {
        using ( AcquireLock() ) { InternalAdd( value ); }
    }
    public virtual async ValueTask AddAsync( T value, CancellationToken token = default )
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


    public override void Insert( int index, T value )
    {
        using ( AcquireLock() ) { InternalInsert( index, value ); }
    }
    public async ValueTask InsertAsync( int index, T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { InternalInsert( index, value ); }
    }


    public          IAsyncEnumerator<T> GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator( token );
    public override IEnumerator<T>      GetEnumerator()                               => Values;
    IEnumerator IEnumerable.            GetEnumerator()                               => GetEnumerator();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock()                                                                                     => locker.Enter();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( scoped in CancellationToken token )                                                  => locker.Enter( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( ref       bool              lockTaken, scoped in CancellationToken token = default ) => locker.Enter( ref lockTaken, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask<Closer> AcquireLockAsync( CancellationToken      token ) => locker.EnterAsync( token );


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<T> AsSpan( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return buffer.ToArray(); }
    }
    protected internal ReadOnlyMemory<T> AsMemory( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return buffer.ToArray(); }
    }
#endif


    protected internal ReadOnlyMemory<T> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    ReadOnlyMemory<T> ILockedCollection<T>.                              Copy()                               => Copy();
    ConfiguredValueTaskAwaitable<ReadOnlyMemory<T>> ILockedCollection<T>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );
    protected async ValueTask<ReadOnlyMemory<T>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return FilteredValues(); }
    }
}
