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
public sealed class ConcurrentObservableCollection<TValue> : ConcurrentObservableCollection<ConcurrentObservableCollection<TValue>, TValue>, ICollectionAlerts<ConcurrentObservableCollection<TValue>, TValue>
    where TValue : IEquatable<TValue>
{
    private static JsonTypeInfo<ConcurrentObservableCollection<TValue>[]>? __JsonArrayInfo;
    private static JsonSerializerContext?                                  __jsonContext;
    private static JsonTypeInfo<ConcurrentObservableCollection<TValue>>?   __jsonTypeInfo;
    public static  JsonSerializerContext                                   JsonContext   { get => Validate.ThrowIfNull(__jsonContext);   set => __jsonContext = value; }
    public static  JsonTypeInfo<ConcurrentObservableCollection<TValue>>    JsonTypeInfo  { get => Validate.ThrowIfNull(__jsonTypeInfo);  set => __jsonTypeInfo = value; }
    public static  JsonTypeInfo<ConcurrentObservableCollection<TValue>[]>  JsonArrayInfo { get => Validate.ThrowIfNull(__JsonArrayInfo); set => __JsonArrayInfo = value; }


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
    public ConcurrentObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : base(comparer) { }
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
}



[Serializable]
public abstract class ConcurrentObservableCollection<TClass, TValue> : ObservableCollection<TClass, TValue>, IList, ILockedCollection<TValue, LockCloser, AsyncLockerEnumerator<TValue, LockCloser>, LockerEnumerator<TValue, LockCloser>>
    where TValue : IEquatable<TValue>
    where TClass : ConcurrentObservableCollection<TClass, TValue>, ICollectionAlerts<TClass, TValue>
{
    protected internal readonly Lock locker = new();


    public AsyncLockerEnumerator<TValue, LockCloser> AsyncValues    { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(this); }
    bool ICollection.                                IsSynchronized { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => true; }
    public Lock                                      Lock           { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => locker; init => locker = value; }

#pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
    object ICollection.SyncRoot { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => locker; }
#pragma warning restore CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement
    public LockerEnumerator<TValue, LockCloser> Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(this); }


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
    public ConcurrentObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : base(comparer) { }
    public ConcurrentObservableCollection( TValue[]                            values ) : base(values) { }
    public ConcurrentObservableCollection( TValue[]                            values, Comparer<TValue> comparer ) : base(values, comparer) { }


    public override void Set( int index, TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalSet(index, in value); }
    }
    public override TValue Get( int index )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalGet(index); }
    }


    public override bool Exists( RefCheck<TValue> match )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindIndex(match) >= 0; }
    }
    public async ValueTask<bool> ExistsAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match) >= 0; }
    }


    public override int FindIndex( RefCheck<TValue> match, int start, int endInclusive )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindIndex(match, start, endInclusive); }
    }
    public override int FindIndex( RefCheck<TValue> match, int start = 0 )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindIndex(match, start); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match, start, count); }
    }
    public async ValueTask<int> FindIndexAsync( int start, RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match, start); }
    }
    public async ValueTask<int> FindIndexAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindIndex(match); }
    }


    public override int FindLastIndex( RefCheck<TValue> match, int start, int count )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindLastIndex(match, start, count); }
    }
    public override int FindLastIndex( RefCheck<TValue> match, int start = 0 )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindLastIndex(match, start); }
    }
    public async ValueTask<int> FindLastIndexAsync( RefCheck<TValue> match, int start, int count, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLastIndex(match, start, count); }
    }
    public async ValueTask<int> FindLastIndexAsync( RefCheck<TValue> match, int start, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLastIndex(match, start); }
    }
    public async ValueTask<int> FindLastIndexAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLastIndex(match); }
    }


    public override int IndexOf( TValue value, int start )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.IndexOf(value, start); }
    }
    public override int IndexOf( TValue value, int start, int count )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.IndexOf(value, start, count); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.IndexOf(value); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.IndexOf(value, start); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.IndexOf(value, start, count); }
    }


    public override int LastIndexOf( TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.LastIndexOf(value); }
    }
    public override int LastIndexOf( TValue value, int start )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.LastIndexOf(value, start); }
    }
    public override int LastIndexOf( TValue value, int start, int count )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.LastIndexOf(value, start, count); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.LastIndexOf(value); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.LastIndexOf(value, start); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.LastIndexOf(value, start, count); }
    }


    public override TValue[] FindAll( RefCheck<TValue> match )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindAll(match); }
    }
    public async ValueTask<TValue[]> FindAllAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindAll(match); }
    }
    public override TValue? Find( RefCheck<TValue> match )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.Find(match); }
    }
    public async ValueTask<TValue?> FindAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.Find(match); }
    }
    public override TValue? FindLast( RefCheck<TValue> match )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.FindLast(match); }
    }
    public async ValueTask<TValue?> FindLastAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return base.FindLast(match); }
    }


    public override bool TryAdd( TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalTryAdd(in value); }
    }
    public override void Add( params ReadOnlySpan<TValue> values )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalAdd(values); }
    }
    public override void Add( IEnumerable<TValue> values )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalAdd(values); }
    }


    public override void AddOrUpdate( TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalAddOrUpdate(in value); }
    }
    public override void AddOrUpdate( IEnumerable<TValue> values )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAddOrUpdate(in value); }
        }
    }
    public override void AddOrUpdate( params ReadOnlySpan<TValue> values )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { InternalAddOrUpdate(in value); }
        }
    }
    public override async ValueTask AddOrUpdate( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token) ) { InternalAddOrUpdate(in value); }
        }
    }


    public override async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalTryAdd(in value); }
    }
    public override async ValueTask TryAddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            foreach ( TValue value in values ) { InternalTryAdd(in value); }
        }
    }
    public override async ValueTask TryAddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token) ) { InternalTryAdd(in value); }
        }
    }
    public override async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(values.Span); }
    }
    public override async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(values.AsSpan()); }
    }
    public override async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(values); }
    }
    public override async ValueTask AddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token) ) { InternalAdd(in value); }
        }
    }


    public override void CopyTo( TValue[] array )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { base.CopyTo(array); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { base.CopyTo(array, destinationStartIndex); }
    }
    public override void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { base.CopyTo(array, sourceStartIndex, length, destinationStartIndex); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { base.CopyTo(array); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { base.CopyTo(array, destinationStartIndex); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { base.CopyTo(array, sourceStartIndex, length, destinationStartIndex); }
    }


    public override void Insert( int index, IEnumerable<TValue> collection )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalInsert(index, collection); }
    }
    public override void Insert( int index, params ReadOnlySpan<TValue> collection )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalInsert(index, collection); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, collection); }
    }
    public async ValueTask InsertRangeAsync( int index, IAsyncEnumerable<TValue> collection, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( ( int i, TValue value ) in collection.Enumerate(index).WithCancellation(token) ) { InternalInsert(i, in value); }
        }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<TValue> collection, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, collection.AsSpan()); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<TValue> collection, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, collection.Span); }
    }


    public override void RemoveRange( int start, int count )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalRemove(start, count); }
    }
    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalRemove(start, count); }
    }


    public override int Remove( RefCheck<TValue> match )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalRemove(match); }
    }
    public override int Remove( IEnumerable<TValue> values )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalRemove(values); }
    }
    public override bool Remove( TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalRemove(in value); }
    }


    public override async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(in value); }
    }
    public override async ValueTask<int> RemoveAsync( RefCheck<TValue> match, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(match); }
    }
    public override async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(values); }
    }
    public override async ValueTask RemoveAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            await foreach ( TValue value in values.WithCancellation(token) ) { InternalRemove(in value); }
        }
    }
    public override async ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(values.Span); }
    }
    public override async ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalRemove(values.AsSpan()); }
    }


    public override bool RemoveAt( int index )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalRemoveAt(index, out _); }
    }
    public override bool RemoveAt( int index, [NotNullWhen(true)] out TValue? value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return InternalRemoveAt(index, out value); }
    }
    public async ValueTask<TValue?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( await AcquireLockAsync(token).ConfigureAwait(false) )
        {
            return InternalRemoveAt(index, out TValue? value)
                       ? value
                       : default;
        }
    }


    public override void Reverse()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalReverse(); }
    }
    public override void Reverse( int start, int count )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalReverse(start, count); }
    }
    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalReverse(); }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalReverse(start, count); }
    }


    public override void Sort()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalSort(comparer); }
    }
    public override void Sort( Comparer<TValue> compare )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalSort(compare); }
    }
    public override void Sort( Comparison<TValue> compare )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalSort(compare); }
    }
    public override void Sort( int start, int count, Comparer<TValue> compare )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalSort(start, count, compare); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync(comparer, token);
    public async ValueTask SortAsync( Comparer<TValue> compare, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalSort(compare); }
    }
    public override async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalSort(compare); }
    }
    public override ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync(start, count, comparer, token);
    public override async ValueTask SortAsync( int start, int count, Comparer<TValue> compare, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalSort(start, count, compare); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            if ( array is TValue[] x ) { base.CopyTo(x, start); }
        }
    }
    void IList.Remove( object? value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            if ( value is TValue x ) { InternalRemove(in x); }
        }
    }
    int IList.Add( object? value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            if ( value is not TValue x ) { return NOT_FOUND; }

            InternalAdd(in x);
            return Count;
        }
    }
    bool IList.Contains( object? value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return value is TValue x && base.Contains(x); }
    }
    int IList.IndexOf( object? value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            return value is TValue x
                       ? base.IndexOf(x)
                       : NOT_FOUND;
        }
    }
    void IList.Insert( int index, object? value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( AcquireLock() )
        {
            if ( value is TValue x ) { InternalInsert(index, in x); }
        }
    }


    public override bool Contains( TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return base.Contains(value); }
    }
    public override async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return InternalContains(in value); }
    }


    public override void Add( TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalAdd(in value); }
    }
    public override async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalAdd(in value); }
    }


    public override void Clear()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { base.Clear(); }
    }
    public override async ValueTask ClearAsync( CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalClear(); }
    }


    public override void Insert( int index, TValue value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { InternalInsert(index, in value); }
    }
    public override async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { InternalInsert(index, in value); }
    }


    public AsyncLockerEnumerator<TValue, LockCloser>     GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator(token);
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.   GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator(token);
    public override LockerEnumerator<TValue, LockCloser> GetEnumerator()                               => Values;
    IEnumerator IEnumerable.                             GetEnumerator()                               => GetEnumerator();


    [MustDisposeResource]
    public Lock.Scope AcquireLock()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return locker.EnterScope();
    }
    [MustDisposeResource]
    public LockCloser AcquireLock( CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return LockCloser.Enter(locker, token);
    }
    [MustDisposeResource]
    public async ValueTask<LockCloser> AcquireLockAsync( CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return await LockCloser.EnterAsync(locker, token);
    }


    public sealed override void TrimExcess()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        if ( !locker.TryEnter() )
        {
            using ( AcquireLock() ) { base.TrimExcess(); }
        }
        else { base.TrimExcess(); }
    }
    public sealed override void EnsureCapacity( int capacity )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        if ( !locker.IsHeldByCurrentThread )
        {
            using ( AcquireLock() ) { base.EnsureCapacity(capacity); }
        }
        else { base.EnsureCapacity(capacity); }
    }


    [Pure, MustDisposeResource]
    protected internal FilterBuffer<TValue> Copy()
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    [Pure, MustDisposeResource] FilterBuffer<TValue> ILockedCollection<TValue, LockCloser>.                              Copy()                               => Copy();
    [Pure, MustDisposeResource] ConfiguredValueTaskAwaitable<FilterBuffer<TValue>> ILockedCollection<TValue, LockCloser>.CopyAsync( CancellationToken token ) => CopyAsync(token).ConfigureAwait(false);
    [Pure, MustDisposeResource]
    protected async ValueTask<FilterBuffer<TValue>> CopyAsync( CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        using ( await AcquireLockAsync(token).ConfigureAwait(false) ) { return FilteredValues(); }
    }
}
