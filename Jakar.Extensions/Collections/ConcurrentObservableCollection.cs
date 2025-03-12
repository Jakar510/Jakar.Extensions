using ZXing.Aztec.Internal;



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
public class ConcurrentObservableCollection<TValue> : ObservableCollection<TValue>, IList, ILockedCollection<TValue, LockCloser, AsyncLockerEnumerator<TValue, LockCloser>, LockerEnumerator<TValue, LockCloser>>
    where TValue : IEquatable<TValue>
{
    protected internal readonly Lock locker = new();


    public AsyncLockerEnumerator<TValue, LockCloser> AsyncValues    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }
    bool ICollection.                                IsSynchronized { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => true; }
    public Lock                                      Lock           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; init => locker = value; }

#pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
    object ICollection.SyncRoot { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; }
#pragma warning restore CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
    public LockerEnumerator<TValue, LockCloser> Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }

    public ConcurrentObservableCollection() : base() { }
    public ConcurrentObservableCollection( IComparer<TValue>                   comparer, int capacity = DEFAULT_CAPACITY ) : base( comparer, capacity ) { }
    public ConcurrentObservableCollection( int                                 capacity ) : base( capacity ) { }
    public ConcurrentObservableCollection( ref readonly Buffer<TValue>         values ) : base( in values ) { }
    public ConcurrentObservableCollection( ref readonly Buffer<TValue>         values, IComparer<TValue> comparer ) : base( in values, comparer ) { }
    public ConcurrentObservableCollection( ref readonly ImmutableArray<TValue> values ) : base( in values ) { }
    public ConcurrentObservableCollection( ref readonly ImmutableArray<TValue> values, IComparer<TValue> comparer ) : base( in values, comparer ) { }
    public ConcurrentObservableCollection( ref readonly ReadOnlyMemory<TValue> values ) : base( in values ) { }
    public ConcurrentObservableCollection( ref readonly ReadOnlyMemory<TValue> values, IComparer<TValue> comparer ) : base( in values, comparer ) { }
    public ConcurrentObservableCollection( params       ReadOnlySpan<TValue>   values ) : base( values ) { }
    public ConcurrentObservableCollection( IComparer<TValue>                   comparer, params ReadOnlySpan<TValue> values ) : base( comparer, values ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue>                 values ) : base( values ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue>                 values, IComparer<TValue> comparer ) : base( comparer ) { }
    public ConcurrentObservableCollection( TValue[]                            values ) : base( values ) { }
    public ConcurrentObservableCollection( TValue[]                            values, IComparer<TValue> comparer ) : base( values, comparer ) { }


    public static implicit operator ConcurrentObservableCollection<TValue>( Buffer<TValue>         values ) => new(in values);
    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue>           values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue>        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue>  values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue>     values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[]               values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ImmutableArray<TValue> values ) => new(in values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlyMemory<TValue> values ) => new(in values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlySpan<TValue>   values ) => new(values);


    public override void Set( int index, TValue value )
    {
        using ( AcquireLock() ) { base.Set( index, value ); }
    }
    public override TValue Get( int index )
    {
        using ( AcquireLock() ) { return base.Get( index ); }
    }


    public override bool Exists( Func<TValue, bool> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( match ) >= 0; }
    }
    public async ValueTask<bool> ExistsAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindIndex( match ) >= 0; }
    }


    public override int FindIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return base.FindIndex( match, start, endInclusive ); }
    }
    public override int FindIndex( Func<TValue, bool> match, int start = 0 )
    {
        using ( AcquireLock() ) { return base.FindIndex( match, start ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindIndex( match, start, count ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindIndex( match, start ); }
    }
    public async ValueTask<int> FindIndexAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindIndex( match ); }
    }


    public override int FindLastIndex( Func<TValue, bool> match, int start, int count )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( match, start, count ); }
    }
    public override int FindLastIndex( Func<TValue, bool> match, int start = 0 )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( match, start ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Func<TValue, bool> match, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindLastIndex( match, start, count ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Func<TValue, bool> match, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindLastIndex( match, start ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindLastIndex( match ); }
    }


    public override int IndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return base.IndexOf( value, start ); }
    }
    public override int IndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return base.IndexOf( value, start, count ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.IndexOf( value, start, count ); }
    }


    public override int LastIndexOf( TValue value )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value ); }
    }
    public override int LastIndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value, start ); }
    }
    public override int LastIndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value, start, count ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.LastIndexOf( value, start, count ); }
    }


    public override TValue[] FindAll( Func<TValue, bool> match )
    {
        using ( AcquireLock() ) { return base.FindAll( match ); }
    }
    public async ValueTask<TValue[]> FindAllAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindAll( match ); }
    }
    public override TValue? Find( Func<TValue, bool> match )
    {
        using ( AcquireLock() ) { return base.Find( match ); }
    }
    public async ValueTask<TValue?> FindAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Find( match ); }
    }
    public override TValue? FindLast( Func<TValue, bool> match )
    {
        using ( AcquireLock() ) { return base.FindLast( match ); }
    }
    public async ValueTask<TValue?> FindLastAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.FindLast( match ); }
    }


    public override bool TryAdd( TValue value )
    {
        using ( AcquireLock() ) { return base.TryAdd( value ); }
    }
    public override void Add( params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { base.Add( values ); }
    }
    public override void Add( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { base.Add( values ); }
    }
    public override void Add( ref readonly SpanEnumerable<TValue, EnumerableProducer<TValue>> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { base.Add( value ); }
        }
    }


    public override void AddOrUpdate( TValue value )
    {
        using ( AcquireLock() ) { base.AddOrUpdate( value ); }
    }
    public override void AddOrUpdate( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { base.AddOrUpdate( value ); }
        }
    }
    public override void AddOrUpdate( params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { base.AddOrUpdate( value ); }
        }
    }
    public virtual async ValueTask AddOrUpdate( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { base.AddOrUpdate( value ); }
        }
    }


    public virtual async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.TryAdd( value ); }
    }
    public virtual async ValueTask TryAddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            foreach ( TValue value in values ) { base.TryAdd( value ); }
        }
    }
    public virtual async ValueTask TryAddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { base.TryAdd( value ); }
        }
    }
    public virtual async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Add( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Add( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Add( values ); }
    }
    public virtual async ValueTask AddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { base.Add( value ); }
        }
    }


    public override void CopyTo( TValue[] array )
    {
        using ( AcquireLock() ) { base.CopyTo( array ); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex )
    {
        using ( AcquireLock() ) { base.CopyTo( array, destinationStartIndex ); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 )
    {
        using ( AcquireLock() ) { base.CopyTo( array, sourceStartIndex, length, destinationStartIndex ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.CopyTo( array, destinationStartIndex ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.CopyTo( array, sourceStartIndex, length, destinationStartIndex ); }
    }


    public override void Insert( int index, IEnumerable<TValue> collection )
    {
        using ( AcquireLock() ) { base.Insert( index, collection ); }
    }
    public override void Insert( int index, params ReadOnlySpan<TValue> collection )
    {
        using ( AcquireLock() ) { base.Insert( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Insert( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IAsyncEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( (int i, TValue value) in collection.Enumerate( index ).WithCancellation( token ) ) { base.Insert( i, value ); }
        }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Insert( index, collection.AsSpan() ); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Insert( index, collection.Span ); }
    }


    public override void RemoveRange( int start, int count )
    {
        using ( AcquireLock() ) { base.RemoveRange( start, count ); }
    }
    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.RemoveRange( start, count ); }
    }


    public override int Remove( Func<TValue, bool> match )
    {
        using ( AcquireLock() ) { return base.Remove( match ); }
    }
    public override int Remove( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { return base.Remove( values ); }
    }
    public override bool Remove( TValue value )
    {
        using ( AcquireLock() ) { return base.Remove( value ); }
    }


    public virtual async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Remove( value ); }
    }
    public virtual async ValueTask<int> RemoveAsync( Func<TValue, bool> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Remove( buffer.Where( match ) ); }
    }
    public virtual async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Remove( values ); }
    }
    public virtual async ValueTask RemoveAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            await foreach ( TValue value in values.WithCancellation( token ) ) { base.Remove( value ); }
        }
    }
    public virtual async ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Remove( values.Span ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Remove( values.AsSpan() ); }
    }


    public override bool RemoveAt( int index )
    {
        using ( AcquireLock() ) { return base.RemoveAt( index, out _ ); }
    }
    public override bool RemoveAt( int index, [NotNullWhen( true )] out TValue? value )
    {
        using ( AcquireLock() ) { return base.RemoveAt( index, out value ); }
    }
    public async ValueTask<TValue?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) )
        {
            return base.RemoveAt( index, out TValue? value )
                       ? value
                       : default;
        }
    }


    public override void Reverse()
    {
        using ( AcquireLock() ) { base.Reverse(); }
    }
    public override void Reverse( int start, int count )
    {
        using ( AcquireLock() ) { base.Reverse( start, count ); }
    }
    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Reverse(); }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Reverse( start, count ); }
    }


    public override void Sort()
    {
        using ( AcquireLock() ) { base.Sort(); }
    }
    public override void Sort( IComparer<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( Comparison<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( int start, int count, IComparer<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( start, count, compare ); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync( comparer, token );
    public async ValueTask SortAsync( IComparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Sort( compare ); }
    }
    public virtual async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Sort( compare ); }
    }
    public ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Sort( start, count, compare ); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() )
        {
            if ( array is TValue[] x ) { base.CopyTo( x, start ); }
        }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is TValue x ) { base.Remove( x ); }
        }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is not TValue x ) { return NOT_FOUND; }

            base.Add( x );
            return Count;
        }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return value is TValue x && base.Contains( x ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() )
        {
            return value is TValue x
                       ? base.IndexOf( x )
                       : NOT_FOUND;
        }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is TValue x ) { base.Insert( index, x ); }
        }
    }


    public override bool Contains( TValue value )
    {
        using ( AcquireLock() ) { return base.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return base.Contains( value ); }
    }


    public override void Add( TValue value )
    {
        using ( AcquireLock() ) { base.Add( value ); }
    }
    public virtual async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Add( value ); }
    }


    public override void Clear()
    {
        using ( AcquireLock() ) { base.Clear(); }
    }
    public virtual async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Clear(); }
    }


    public override void Insert( int index, TValue value )
    {
        using ( AcquireLock() ) { base.Insert( index, value ); }
    }
    public async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { base.Insert( index, value ); }
    }


    public AsyncLockerEnumerator<TValue, LockCloser>     GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator( token );
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.   GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public override LockerEnumerator<TValue, LockCloser> GetEnumerator()                               => Values;
    IEnumerator IEnumerable.                             GetEnumerator()                               => GetEnumerator();


    [MustDisposeResource, MethodImpl( MethodImplOptions.AggressiveInlining )] public Lock.Scope            AcquireLock()                               => locker.EnterScope();
    [MustDisposeResource, MethodImpl( MethodImplOptions.AggressiveInlining )] public LockCloser            AcquireLock( CancellationToken      token ) => LockCloser.Enter( locker, token );
    [MustDisposeResource, MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask<LockCloser> AcquireLockAsync( CancellationToken token ) => LockCloser.EnterAsync( locker, token );


    public sealed override void TrimExcess()
    {
        if ( locker.TryEnter() is false )
        {
            using ( AcquireLock() ) { base.TrimExcess(); }
        }
        else { base.TrimExcess(); }
    }
    public sealed override void EnsureCapacity( int capacity )
    {
        if ( locker.IsHeldByCurrentThread is false )
        {
            using ( AcquireLock() ) { base.EnsureCapacity( capacity ); }
        }
        else { base.EnsureCapacity( capacity ); }
    }


    [Pure, MustDisposeResource]
    protected internal FilterBuffer<TValue> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    [Pure, MustDisposeResource] FilterBuffer<TValue> ILockedCollection<TValue, LockCloser>.                              Copy()                               => Copy();
    [Pure, MustDisposeResource] ConfiguredValueTaskAwaitable<FilterBuffer<TValue>> ILockedCollection<TValue, LockCloser>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );
    [Pure, MustDisposeResource]
    protected async ValueTask<FilterBuffer<TValue>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ).ConfigureAwait( false ) ) { return FilteredValues(); }
    }
}
