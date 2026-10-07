using ZLinq;



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
public sealed partial class ConcurrentObservableCollection<TValue> : ConcurrentObservableCollection<ConcurrentObservableCollection<TValue>, TValue>, ICollectionAlerts<ConcurrentObservableCollection<TValue>, TValue>
    where TValue : IEquatable<TValue>
{
    public ConcurrentObservableCollection() : base() { }
    public ConcurrentObservableCollection( Comparer<TValue>                    comparer, int capacity = DEFAULT_CAPACITY ) : base(comparer, capacity) { }
    public ConcurrentObservableCollection( int                                 capacity ) : base(capacity) { }
    public ConcurrentObservableCollection( ref readonly Buffer<TValue>         values ) : base(in values) { }
    public ConcurrentObservableCollection( ref readonly Buffer<TValue>         values, Comparer<TValue> comparer ) : base(in values, comparer) { }
    public ConcurrentObservableCollection( ref readonly ImmutableArray<TValue> values ) : base(in values) { }
    public ConcurrentObservableCollection( ref readonly ImmutableArray<TValue> values, Comparer<TValue> comparer ) : base(in values, comparer) { }
    public ConcurrentObservableCollection( ref readonly ReadOnlyMemory<TValue> values ) : base(in values) { }
    public ConcurrentObservableCollection( ref readonly ReadOnlyMemory<TValue> values, Comparer<TValue> comparer ) : base(in values, comparer) { }
    public ConcurrentObservableCollection( params       ReadOnlySpan<TValue>   values ) : base(values) { }
    public ConcurrentObservableCollection( Comparer<TValue>                    comparer, params ReadOnlySpan<TValue> values ) : base(comparer, values) { }
    public ConcurrentObservableCollection( IEnumerable<TValue>                 values ) : base(values) { }
    public ConcurrentObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : base(values, comparer) { }
    public ConcurrentObservableCollection( TValue[]                            values ) : base(values) { }
    public ConcurrentObservableCollection( TValue[]                            values, Comparer<TValue> comparer ) : base(values, comparer) { }


    public static implicit operator ConcurrentObservableCollection<TValue>( Buffer<TValue>         values ) => new(in values);
    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue>           values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue>        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue>  values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue>     values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[]               values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ImmutableArray<TValue> values ) => new(in values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlyMemory<TValue> values ) => new(in values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlySpan<TValue>   values ) => new(values);


    public override int  GetHashCode()                                                                                              => RuntimeHelpers.GetHashCode(this);
    public override bool Equals( object?                                      other )                                               => ReferenceEquals(this, other) || ( other is ConcurrentObservableCollection<TValue> x && Equals(x) );
    public static   bool operator ==( ConcurrentObservableCollection<TValue>? left, ConcurrentObservableCollection<TValue>? right ) => EqualityComparer<ConcurrentObservableCollection<TValue>>.Default.Equals(left, right);
    public static   bool operator !=( ConcurrentObservableCollection<TValue>? left, ConcurrentObservableCollection<TValue>? right ) => !EqualityComparer<ConcurrentObservableCollection<TValue>>.Default.Equals(left, right);
    public static   bool operator >( ConcurrentObservableCollection<TValue>   left, ConcurrentObservableCollection<TValue>  right ) => Comparer<ConcurrentObservableCollection<TValue>>.Default.Compare(left, right) > 0;
    public static   bool operator >=( ConcurrentObservableCollection<TValue>  left, ConcurrentObservableCollection<TValue>  right ) => Comparer<ConcurrentObservableCollection<TValue>>.Default.Compare(left, right) >= 0;
    public static   bool operator <( ConcurrentObservableCollection<TValue>   left, ConcurrentObservableCollection<TValue>  right ) => Comparer<ConcurrentObservableCollection<TValue>>.Default.Compare(left, right) < 0;
    public static   bool operator <=( ConcurrentObservableCollection<TValue>  left, ConcurrentObservableCollection<TValue>  right ) => Comparer<ConcurrentObservableCollection<TValue>>.Default.Compare(left, right) <= 0;
}



[Serializable]
public abstract partial class ConcurrentObservableCollection<TSelf, TValue> : ObservableCollection<TSelf, TValue>, IList, ILockedCollection<TValue, LockCloser, AsyncLockerEnumerator<TValue, LockCloser>, LockerEnumerator<TValue, LockCloser>>
    where TSelf : ConcurrentObservableCollection<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>
    where TValue : IEquatable<TValue>
{
    protected internal readonly Lock locker = new();


    public AsyncLockerEnumerator<TValue, LockCloser> AsyncValues    => new(this);
    bool ICollection.                                IsSynchronized => true;
    public Lock                                      Lock           { get => locker; init => locker = value; }

#pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
    object ICollection.SyncRoot => locker;
#pragma warning restore CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
    public LockerEnumerator<TValue, LockCloser> Values => new(this);


    protected ConcurrentObservableCollection() : base() { }
    protected ConcurrentObservableCollection( Comparer<TValue>                    comparer, int capacity = DEFAULT_CAPACITY ) : base(comparer, capacity) { }
    protected ConcurrentObservableCollection( int                                 capacity ) : base(capacity) { }
    protected ConcurrentObservableCollection( ref readonly Buffer<TValue>         values ) : base(in values) { }
    protected ConcurrentObservableCollection( ref readonly Buffer<TValue>         values, Comparer<TValue> comparer ) : base(in values, comparer) { }
    protected ConcurrentObservableCollection( ref readonly ImmutableArray<TValue> values ) : base(in values) { }
    protected ConcurrentObservableCollection( ref readonly ImmutableArray<TValue> values, Comparer<TValue> comparer ) : base(in values, comparer) { }
    protected ConcurrentObservableCollection( ref readonly ReadOnlyMemory<TValue> values ) : base(in values) { }
    protected ConcurrentObservableCollection( ref readonly ReadOnlyMemory<TValue> values, Comparer<TValue> comparer ) : base(in values, comparer) { }
    protected ConcurrentObservableCollection( params       ReadOnlySpan<TValue>   values ) : base(values) { }
    protected ConcurrentObservableCollection( Comparer<TValue>                    comparer, params ReadOnlySpan<TValue> values ) : base(comparer, values) { }
    protected ConcurrentObservableCollection( IEnumerable<TValue>                 values ) : base(values) { }
    protected ConcurrentObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : base(values, comparer) { }
    protected ConcurrentObservableCollection( TValue[]                            values ) : base(values) { }
    protected ConcurrentObservableCollection( TValue[]                            values, Comparer<TValue> comparer ) : base(values, comparer) { }


    public override void Set( int index, TValue value )
    {
        using ( AcquireLock() ) { InternalSet(index, in value); }
    }
    public override TValue Get( int index )
    {
        using ( AcquireLock() ) { return InternalGet(index); }
    }


    public override bool Exists( RefCheck<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindIndex(match) >= 0; }
    }
    public async ValueTask<bool> ExistsAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match) >= 0; }
    }


    public override int FindIndex( RefCheck<TValue> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return base.FindIndex(match, start, endInclusive); }
    }
    public override int FindIndex( RefCheck<TValue> match, int start = 0 )
    {
        using ( AcquireLock() ) { return base.FindIndex(match, start); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match, start, count); }
    }
    public async ValueTask<int> FindIndexAsync( int start, RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match, start); }
    }
    public async ValueTask<int> FindIndexAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match); }
    }


    public override int FindLastIndex( RefCheck<TValue> match, int start, int count )
    {
        using ( AcquireLock() ) { return base.FindLastIndex(match, start, count); }
    }
    public override int FindLastIndex( RefCheck<TValue> match, int start = 0 )
    {
        using ( AcquireLock() ) { return base.FindLastIndex(match, start); }
    }
    public async ValueTask<int> FindLastIndexAsync( RefCheck<TValue> match, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLastIndex(match, start, count); }
    }
    public async ValueTask<int> FindLastIndexAsync( RefCheck<TValue> match, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLastIndex(match, start); }
    }
    public async ValueTask<int> FindLastIndexAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLastIndex(match); }
    }


    public override int IndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return base.IndexOf(value, start); }
    }
    public override int IndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return base.IndexOf(value, start, count); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.IndexOf(value); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.IndexOf(value, start); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.IndexOf(value, start, count); }
    }


    public override int LastIndexOf( TValue value )
    {
        using ( AcquireLock() ) { return base.LastIndexOf(value); }
    }
    public override int LastIndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return base.LastIndexOf(value, start); }
    }
    public override int LastIndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return base.LastIndexOf(value, start, count); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.LastIndexOf(value); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.LastIndexOf(value, start); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.LastIndexOf(value, start, count); }
    }


    public override TValue[] FindAll( RefCheck<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindAll(match); }
    }
    public async ValueTask<TValue[]> FindAllAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindAll(match); }
    }
    public override TValue? Find( RefCheck<TValue> match )
    {
        using ( AcquireLock() ) { return base.Find(match); }
    }
    public async ValueTask<TValue?> FindAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.Find(match); }
    }
    public override TValue? FindLast( RefCheck<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindLast(match); }
    }
    public async ValueTask<TValue?> FindLastAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLast(match); }
    }


    public override void Add( TValue value )
    {
        using ( AcquireLock() ) { InternalAdd(in value); }
    }
    public override void Add( params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd(values); }
    }
    public override void Add( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd(values); }
    }
    public override void Add<TEnumerator>( ValueEnumerable<TEnumerator, TValue> values )
    {
        using ( AcquireLock() ) { InternalAdd(values); }
    }


    public override async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(values.Span); }
    }
    public override async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(values.AsSpan()); }
    }
    public override async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(values); }
    }
    public override async ValueTask AddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalAdd(in value); }
        }
    }
    public override async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(in value); }
    }


    public override bool TryAdd( TValue value )
    {
        using ( AcquireLock() ) { return InternalTryAdd(in value); }
    }
    public override async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalTryAdd(in value); }
    }
    public override async ValueTask TryAddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            foreach ( TValue value in values ) { InternalTryAdd(in value); }
        }
    }
    public override async ValueTask TryAddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalTryAdd(in value); }
        }
    }


    public override void AddOrUpdate( TValue value )
    {
        using ( AcquireLock() ) { InternalAddOrUpdate(in value); }
    }
    public override void AddOrUpdate( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAddOrUpdate(in value); }
        }
    }
    public override void AddOrUpdate( params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAddOrUpdate(in value); }
        }
    }
    public override async ValueTask AddOrUpdate( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalAddOrUpdate(in value); }
        }
    }


    public override void CopyTo( TValue[] array )
    {
        using ( AcquireLock() ) { base.CopyTo(array); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex )
    {
        using ( AcquireLock() ) { base.CopyTo(array, destinationStartIndex); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 )
    {
        using ( AcquireLock() ) { base.CopyTo(array, destinationStartIndex, length, sourceStartIndex); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { base.CopyTo(array); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { base.CopyTo(array, destinationStartIndex); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { base.CopyTo(array, destinationStartIndex, length, sourceStartIndex); }
    }


    public override void Insert( int index, IEnumerable<TValue> collection )
    {
        using ( AcquireLock() ) { InternalInsert(index, collection); }
    }
    public override void Insert( int index, params ReadOnlySpan<TValue> collection )
    {
        using ( AcquireLock() ) { InternalInsert(index, collection); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, collection); }
    }
    public async ValueTask InsertRangeAsync( int index, IAsyncEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( ( int i, TValue value ) in collection.Enumerate(index).WithCancellation(token).ConfigureAwait(false) ) { InternalInsert(i, in value); }
        }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, collection.AsSpan()); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, collection.Span); }
    }


    public override void RemoveRange( int start, int count )
    {
        using ( AcquireLock() ) { InternalRemove(start, count); }
    }
    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalRemove(start, count); }
    }


    public override int Remove( RefCheck<TValue> match )
    {
        using ( AcquireLock() ) { return InternalRemove(match); }
    }
    public override int Remove( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { return InternalRemove(values); }
    }
    public override bool Remove( TValue value )
    {
        using ( AcquireLock() ) { return InternalRemove(in value); }
    }


    public override async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(in value); }
    }
    public override async ValueTask<int> RemoveAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(match); }
    }
    public override async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(values); }
    }
    public override async ValueTask RemoveAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalRemove(in value); }
        }
    }
    public override async ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(values.Span); }
    }
    public override async ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(values.AsSpan()); }
    }


    public override bool RemoveAt( int index )
    {
        using ( AcquireLock() ) { return InternalRemoveAt(index, out _); }
    }
    public override bool RemoveAt( int index, [NotNullWhen(true)] out TValue? value )
    {
        using ( AcquireLock() ) { return InternalRemoveAt(index, out value); }
    }
    public async ValueTask<TValue?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            return InternalRemoveAt(index, out TValue? value)
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
        using ( AcquireLock() ) { InternalReverse(start, count); }
    }
    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalReverse(); }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalReverse(start, count); }
    }


    public override void Sort()
    {
        using ( AcquireLock() ) { InternalSort(comparer); }
    }
    public override void Sort( Comparer<TValue> compare )
    {
        using ( AcquireLock() ) { InternalSort(compare); }
    }
    public override void Sort( Comparison<TValue> compare )
    {
        using ( AcquireLock() ) { InternalSort(compare); }
    }
    public override void Sort( int start, int count, Comparer<TValue> compare )
    {
        using ( AcquireLock() ) { InternalSort(start, count, compare); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync(comparer, token);
    public async ValueTask SortAsync( Comparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalSort(compare); }
    }
    public override async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalSort(compare); }
    }
    public override ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync(start, count, comparer, token);
    public override async ValueTask SortAsync( int start, int count, Comparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalSort(start, count, compare); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() )
        {
            if ( array is TValue[] x ) { base.CopyTo(x, start); }
            else { ( (ICollection)buffer ).CopyTo(array, start); }
        }
    }
    protected override int AddAndGetIndex( TValue value )
    {
        using ( AcquireLock() ) { return base.AddAndGetIndex(value); }
    }


    public override bool Contains( TValue value )
    {
        using ( AcquireLock() ) { return base.Contains(value); }
    }
    public override async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalContains(in value); }
    }


    public override void Clear()
    {
        using ( AcquireLock() ) { base.Clear(); }
    }
    public override async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalClear(); }
    }


    public override void Insert( int index, TValue value )
    {
        using ( AcquireLock() ) { InternalInsert(index, in value); }
    }
    public override async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, in value); }
    }


    public override TValue[] ToArray()
    {
        using ( AcquireLock() ) { return [.. buffer]; }
    }
    public override int IndexOf( TValue value )
    {
        using ( AcquireLock() ) { return base.IndexOf(value); }
    }
    public override int FindCount( RefCheck<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindCount(match); }
    }
    public override TValue? Find( RefCheck<TValue> match, int start )
    {
        using ( AcquireLock() ) { return base.Find(match, start); }
    }
    public override TValue? Find( RefCheck<TValue> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return base.Find(match, start, endInclusive); }
    }
    public override TValue? FindLast( RefCheck<TValue> match, int start )
    {
        using ( AcquireLock() ) { return base.FindLast(match, start); }
    }
    public override TValue? FindLast( RefCheck<TValue> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return base.FindLast(match, start, endInclusive); }
    }
    public override TValue[] FindAll( RefCheck<TValue> match, int start )
    {
        using ( AcquireLock() ) { return base.FindAll(match, start); }
    }
    public override TValue[] FindAll( RefCheck<TValue> match, int start, int endInclusive )
    {
        using ( AcquireLock() ) { return base.FindAll(match, start, endInclusive); }
    }
    public override bool Contains( params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { return base.Contains(values); }
    }
    public override void Add( TValue value, int count )
    {
        using ( AcquireLock() ) { InternalAdd(in value, count); }
    }
    public override void Insert( int startIndex, TValue value, int count )
    {
        using ( AcquireLock() ) { InternalInsert(startIndex, in value, count); }
    }
    public override void Replace( int startIndex, TValue value, int count = 1 )
    {
        using ( AcquireLock() ) { InternalReplace(startIndex, in value, count); }
    }
    public override void Replace( int startIndex, params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { InternalReplace(startIndex, values); }
    }
    public override int Remove( params ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { return InternalRemove(values); }
    }


    public AsyncLockerEnumerator<TValue, LockCloser>     GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator(token);
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.   GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator(token);
    public override LockerEnumerator<TValue, LockCloser> GetEnumerator()                               => Values;
    IEnumerator IEnumerable.                             GetEnumerator()                               => GetEnumerator();


    [Pure] [MustDisposeResource] protected internal override ArrayBuffer<TValue> FilteredValues()
    {
        using ( AcquireLock() ) { return base.FilteredValues(); }
    }
    [MustDisposeResource] [MethodImpl(MethodImplOptions.AggressiveInlining)] public Lock.Scope            AcquireLock()                               => locker.EnterScope();
    [MustDisposeResource]                                                    public LockCloser            AcquireLock( CancellationToken      token ) => LockCloser.Enter(locker, token);
    [MustDisposeResource]                                                    public ValueTask<LockCloser> AcquireLockAsync( CancellationToken token ) => LockCloser.EnterAsync(locker, token);


    public sealed override void TrimExcess()
    {
        using ( AcquireLock() ) { base.TrimExcess(); }
    }
    public sealed override void EnsureCapacity( int capacity )
    {
        using ( AcquireLock() ) { base.EnsureCapacity(capacity); }
    }


    [Pure] [MustDisposeResource] protected internal ArrayBuffer<TValue>                                                                     Copy()                               => FilteredValues();
    [Pure] [MustDisposeResource]                    ArrayBuffer<TValue> ILockedCollection<TValue, LockCloser>.                              Copy()                               => Copy();
    [Pure] [MustDisposeResource]                    ConfiguredValueTaskAwaitable<ArrayBuffer<TValue>> ILockedCollection<TValue, LockCloser>.CopyAsync( CancellationToken token ) => CopyAsync(token).ConfigureAwait(false);
    [Pure] [MustDisposeResource] protected async ValueTask<ArrayBuffer<TValue>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FilteredValues(); }
    }
}
