// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  15:41

using Jakar.Extensions.UserGuid;



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
public sealed class ObservableCollection<TValue>( Comparer<TValue> comparer, int capacity = DEFAULT_CAPACITY ) : ObservableCollection<ObservableCollection<TValue>, TValue>(comparer, capacity), ICollectionAlerts<ObservableCollection<TValue>, TValue>
    where TValue : IEquatable<TValue>
{
    private static JsonTypeInfo<ObservableCollection<TValue>[]>? __JsonArrayInfo;
    private static JsonSerializerContext?                        __jsonContext;
    private static JsonTypeInfo<ObservableCollection<TValue>>?   __jsonTypeInfo;
    public static  JsonSerializerContext                         JsonContext   { get => Validate.ThrowIfNull(__jsonContext);   set => __jsonContext = value; }
    public static  JsonTypeInfo<ObservableCollection<TValue>>    JsonTypeInfo  { get => Validate.ThrowIfNull(__jsonTypeInfo);  set => __jsonTypeInfo = value; }
    public static  JsonTypeInfo<ObservableCollection<TValue>[]>  JsonArrayInfo { get => Validate.ThrowIfNull(__JsonArrayInfo); set => __JsonArrayInfo = value; }


    public ObservableCollection() : this(Comparer<TValue>.Default) { }
    public ObservableCollection( int                                 capacity ) : this(Comparer<TValue>.Default, capacity) { }
    public ObservableCollection( ref readonly Buffer<TValue>         values ) : this(values.Length) => InternalAdd(values.Values);
    public ObservableCollection( ref readonly Buffer<TValue>         values, Comparer<TValue> comparer ) : this(comparer, values.Length) => InternalAdd(values.Values);
    public ObservableCollection( ref readonly ImmutableArray<TValue> values ) : this(values.Length) => InternalAdd(values.AsSpan());
    public ObservableCollection( ref readonly ImmutableArray<TValue> values, Comparer<TValue> comparer ) : this(comparer, values.Length) => InternalAdd(values.AsSpan());
    public ObservableCollection( ref readonly ReadOnlyMemory<TValue> values ) : this(values.Length) => InternalAdd(values.Span);
    public ObservableCollection( ref readonly ReadOnlyMemory<TValue> values, Comparer<TValue> comparer ) : this(comparer, values.Length) => InternalAdd(values.Span);
    public ObservableCollection( params       ReadOnlySpan<TValue>   values ) : this(values.Length) => InternalAdd(values);
    public ObservableCollection( Comparer<TValue>                    comparer, params ReadOnlySpan<TValue> values ) : this(comparer, values.Length) => InternalAdd(values);
    public ObservableCollection( TValue[]                            values ) : this(values.Length) => InternalAdd();
    public ObservableCollection( TValue[]                            values, Comparer<TValue> comparer ) : this(comparer, new ReadOnlySpan<TValue>(values)) { }
    public ObservableCollection( IEnumerable<TValue>                 values ) : this(values, Comparer<TValue>.Default) { }
    public ObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : this(comparer) => InternalAdd(values);


    public static implicit operator ObservableCollection<TValue>( List<TValue>                                                values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( HashSet<TValue>                                             values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ConcurrentBag<TValue>                                       values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( Collection<TValue>                                          values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( System.Collections.ObjectModel.ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( TValue[]                                                    values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ImmutableArray<TValue>                                      values ) => new(in values);
    public static implicit operator ObservableCollection<TValue>( ReadOnlyMemory<TValue>                                      values ) => new(in values);
    public static implicit operator ObservableCollection<TValue>( ReadOnlySpan<TValue>                                        values ) => new(values);


    public override int  GetHashCode()                                                                          => RuntimeHelpers.GetHashCode(this);
    public override bool Equals( object?                            other )                                     => ReferenceEquals(this, other) || other is ObservableCollection<TValue> x && Equals(x);
    public static   bool operator ==( ObservableCollection<TValue>? left, ObservableCollection<TValue>? right ) => EqualityComparer<ObservableCollection<TValue>>.Default.Equals(left, right);
    public static   bool operator !=( ObservableCollection<TValue>? left, ObservableCollection<TValue>? right ) => !EqualityComparer<ObservableCollection<TValue>>.Default.Equals(left, right);
    public static   bool operator >( ObservableCollection<TValue>   left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) > 0;
    public static   bool operator >=( ObservableCollection<TValue>  left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) >= 0;
    public static   bool operator <( ObservableCollection<TValue>   left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) < 0;
    public static   bool operator <=( ObservableCollection<TValue>  left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) <= 0;
}



[Serializable]
public abstract class ObservableCollection<TSelf, TValue>( Comparer<TValue> comparer, int capacity = DEFAULT_CAPACITY ) : CollectionAlerts<TSelf, TValue>, IIObservableCollection<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList
    where TValue : IEquatable<TValue>
    where TSelf : ObservableCollection<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>
{
    protected internal readonly Comparer<TValue> comparer = comparer;
    protected internal readonly List<TValue>     buffer   = new(capacity);


    public          int  Capacity       { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => buffer.Capacity; }
    public override int  Count          { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => buffer.Count; }
    public          bool IsEmpty        { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Count == 0; }
    bool IList.          IsFixedSize    { [MethodImpl(       MethodImplOptions.AggressiveInlining)] get => ( (IList)buffer ).IsFixedSize; }
    public bool          IsNotEmpty     { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Count > 0; }
    public bool          IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining)] get; init; }
    bool ICollection.    IsSynchronized { [MethodImpl(       MethodImplOptions.AggressiveInlining)] get => false; }
    object? IList.this[ int                index ] { get => Get(index); set => Set(index, (TValue)value!); }
    public TValue this[ int                index ] { get => Get(index); set => Set(index, value); }
    TValue IReadOnlyList<TValue>.this[ int index ] { get => Get(index); }
    object ICollection.SyncRoot { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => buffer; }


    protected ObservableCollection() : this(Comparer<TValue>.Default) { }
    protected ObservableCollection( int                                 capacity ) : this(Comparer<TValue>.Default, capacity) { }
    protected ObservableCollection( ref readonly Buffer<TValue>         values ) : this(values.Length) => InternalAdd(values.Values);
    protected ObservableCollection( ref readonly Buffer<TValue>         values, Comparer<TValue> comparer ) : this(comparer, values.Length) => InternalAdd(values.Values);
    protected ObservableCollection( ref readonly ImmutableArray<TValue> values ) : this(values.Length) => InternalAdd(values.AsSpan());
    protected ObservableCollection( ref readonly ImmutableArray<TValue> values, Comparer<TValue> comparer ) : this(comparer, values.Length) => InternalAdd(values.AsSpan());
    protected ObservableCollection( ref readonly ReadOnlyMemory<TValue> values ) : this(values.Length) => InternalAdd(values.Span);
    protected ObservableCollection( ref readonly ReadOnlyMemory<TValue> values, Comparer<TValue> comparer ) : this(comparer, values.Length) => InternalAdd(values.Span);
    protected ObservableCollection( params       ReadOnlySpan<TValue>   values ) : this(values.Length) => InternalAdd(values);
    protected ObservableCollection( Comparer<TValue>                    comparer, params ReadOnlySpan<TValue> values ) : this(comparer, values.Length) => InternalAdd(values);
    protected ObservableCollection( TValue[]                            values ) : this(values.Length) => InternalAdd();
    protected ObservableCollection( TValue[]                            values, Comparer<TValue> comparer ) : this(comparer, new ReadOnlySpan<TValue>(values)) { }
    protected ObservableCollection( IEnumerable<TValue>                 values ) : this(values, Comparer<TValue>.Default) { }
    protected ObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : this(comparer) => InternalAdd(values);
    public virtual void Dispose()
    {
        buffer.Clear();
        GC.SuppressFinalize(this);
    }


    public TValue[] ToArray() => buffer.ToArray();


    protected internal virtual void InternalInsert( int i, ref readonly TValue value )
    {
        ThrowIfReadOnly();
        EnsureCapacity(1);
        buffer.Insert(i, value);
        Added(in value, i);
    }
    protected internal virtual void InternalInsert( int startIndex, IEnumerable<TValue> collection )
    {
        ThrowIfReadOnly();
        foreach ( ( int i, TValue? value ) in collection.Enumerate(startIndex) ) { InternalInsert(i, in value); }
    }
    protected internal virtual void InternalInsert( int startIndex, params ReadOnlySpan<TValue> collection )
    {
        ThrowIfReadOnly();
        EnsureCapacity(startIndex + collection.Length);
        foreach ( ( int i, TValue? value ) in collection.Enumerate(startIndex) ) { InternalInsert(i, in value); }
    }
    protected internal virtual void InternalInsert( int index, ref readonly TValue value, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity(count);
        for ( int i = 0; i < count; i++ ) { InternalInsert(index + i, in value); }
    }


    protected internal virtual void InternalReplace( int startIndex, ref readonly TValue value, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity(count);
        for ( int i = 0; i < count; i++ ) { buffer[i + startIndex] = value; }

        Reset();
    }
    protected internal virtual void InternalReplace( int startIndex, params ReadOnlySpan<TValue> value )
    {
        ThrowIfReadOnly();
        EnsureCapacity(value.Length);
        for ( int i = 0; i < value.Length; i++ ) { buffer[i + startIndex] = value[i]; }

        Reset();
    }


    protected internal virtual void InternalRemove( int start, int count )
    {
        ThrowIfReadOnly();
        Guard.IsInRangeFor(start,         buffer, nameof(start));
        Guard.IsInRangeFor(start + count, buffer, nameof(count));

        for ( int x = start; x < start + count; x++ )
        {
            buffer.RemoveAt(x);
            Removed(x);
        }
    }


    protected internal virtual bool InternalRemove( ref readonly TValue value )
    {
        ThrowIfReadOnly();
        int index = buffer.IndexOf(value);
        if ( index < 0 ) { return false; }

        buffer.RemoveAt(index);
        Removed(in value, index);
        return true;
    }
    protected internal virtual int InternalRemove( RefCheck<TValue> match )
    {
        ThrowIfReadOnly();
        ReadOnlySpan<TValue> span  = AsSpan();
        int                  count = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in span )
        {
            if ( match(in value) && InternalRemove(in value) ) { count++; }
        }

        return count;
    }


    protected internal virtual bool InternalContains( ref readonly TValue value ) => buffer.Contains(value);
    protected internal virtual void InternalClear()
    {
        ThrowIfReadOnly();
        buffer.Clear();
        Reset();
    }


    protected internal virtual bool InternalRemoveAt( int index, [NotNullWhen(true)] out TValue? value )
    {
        ThrowIfReadOnly();

        if ( index < 0 || index >= Count )
        {
            value = default;
            return false;
        }

        value = buffer[index];
        buffer.RemoveAt(index);
        Removed(in value, index);
        return true;
    }
    protected internal virtual bool InternalTryAdd( ref readonly TValue value )
    {
        ThrowIfReadOnly();
        if ( buffer.Contains(value) ) { return false; }

        InternalAdd(in value);
        return true;
    }


    protected internal virtual void InternalAdd( ref readonly TValue value )
    {
        ThrowIfReadOnly();
        buffer.Add(value);
        Added(in value, Count - 1);
    }
    protected internal virtual void InternalAdd( ref readonly TValue value, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity(count);
        for ( int i = 0; i < count; i++ ) { InternalAdd(in value); }
    }
    protected internal virtual void InternalAdd( IEnumerable<TValue> values )
    {
        ThrowIfReadOnly();
        buffer.AddRange(values);
        Reset();
    }
    protected internal virtual void InternalAdd( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        EnsureCapacity(values.Length);
        buffer.AddRange(values);
        Reset();
    }


    protected internal virtual void InternalAddOrUpdate( ref readonly TValue value )
    {
        ThrowIfReadOnly();
        int index = buffer.IndexOf(value);

        if ( index >= 0 ) { InternalSet(index, in value); }
        else { InternalAdd(in value); }
    }


    protected internal virtual void InternalSort( Comparer<TValue> compare )
    {
        ThrowIfReadOnly();

        CollectionsMarshal.AsSpan(buffer)
                          .Sort(compare);

        Reset();
    }
    protected internal virtual void InternalSort( Comparison<TValue> compare )
    {
        ThrowIfReadOnly();

        CollectionsMarshal.AsSpan(buffer)
                          .Sort(compare);

        Reset();
    }
    protected internal virtual void InternalSort( int start, int length, Comparer<TValue> compare )
    {
        ThrowIfReadOnly();

        CollectionsMarshal.AsSpan(buffer)
                          .Slice(start, length)
                          .Sort(compare);

        Reset();
    }


    protected internal virtual int InternalRemove( IEnumerable<TValue> values )
    {
        ThrowIfReadOnly();
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( !InternalRemove(in value) ) { continue; }

            results++;
        }

        return results;
    }
    protected internal virtual int InternalRemove( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( !InternalRemove(in value) ) { continue; }

            results++;
        }

        return results;
    }


    protected internal virtual void InternalReverse()
    {
        ThrowIfReadOnly();
        buffer.Reverse();
        Reset();
    }
    protected internal virtual void InternalReverse( int start, int length )
    {
        ThrowIfReadOnly();
        Guard.IsInRangeFor(start,          buffer, nameof(start));
        Guard.IsInRangeFor(start + length, buffer, nameof(length));
        buffer.Reverse(start, length);
        Reset();
    }


    protected void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new NotSupportedException("Collection is read-only."); }
    }


    protected internal virtual TValue InternalGet( int index )
    {
        Guard.IsInRangeFor(index, buffer, nameof(index));
        TValue result = buffer[index];
        return result;
    }
    protected internal virtual void InternalSet( int index, ref readonly TValue value )
    {
        ThrowIfReadOnly();

        TValue? old = index < buffer.Count && index >= 0
                          ? buffer[index]
                          : default;

        Guard.IsInRangeFor(index, buffer, nameof(index));
        buffer[index] = value;
        Replaced(in old, in value, index);
    }


    public virtual TValue Get( int index )               => InternalGet(index);
    public virtual void   Set( int index, TValue value ) => InternalSet(index, in value);


    public virtual bool Exists( RefCheck<TValue> match ) => FindIndex(match) >= 0;


    public virtual int FindIndex( RefCheck<TValue> match, int start = 0 )
    {
        Guard.IsInRangeFor(start, buffer, nameof(start));
        return FindIndex(match, start, Count - 1);
    }
    public virtual int FindIndex( RefCheck<TValue> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor(start,        buffer, nameof(start));
        Guard.IsInRangeFor(endInclusive, buffer, nameof(endInclusive));
        ReadOnlySpan<TValue> span = AsSpan();

        for ( int i = start; i < endInclusive; i++ )
        {
            if ( match(in span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }


    public virtual int FindLastIndex( RefCheck<TValue> match, int start = 0 )
    {
        Guard.IsInRangeFor(start, buffer, nameof(start));
        return FindLastIndex(match, Count - 1, start);
    }
    public virtual int FindLastIndex( RefCheck<TValue> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor(start,        buffer, nameof(start));
        Guard.IsInRangeFor(endInclusive, buffer, nameof(endInclusive));
        ReadOnlySpan<TValue> span = AsSpan();

        for ( int i = start; i < endInclusive; i-- )
        {
            if ( match(in span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }


    public virtual int IndexOf( TValue value ) => buffer.IndexOf(value);
    public virtual int IndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor(start, buffer, nameof(start));
        return buffer.IndexOf(value, start);
    }
    public virtual int IndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor(start,         buffer, nameof(start));
        Guard.IsInRangeFor(start + count, buffer, nameof(count));
        return buffer.IndexOf(value, start, count);
    }


    public virtual int LastIndexOf( TValue value ) => buffer.LastIndexOf(value);
    public virtual int LastIndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor(start, buffer, nameof(start));
        return buffer.LastIndexOf(value, start);
    }
    public virtual int LastIndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor(start,         buffer, nameof(start));
        Guard.IsInRangeFor(start + count, buffer, nameof(count));
        return buffer.LastIndexOf(value, start, count);
    }


    public virtual int FindCount( RefCheck<TValue> match )
    {
        ReadOnlySpan<TValue> span = AsSpan();
        return span.Count(match);
    }
    public virtual TValue? Find( RefCheck<TValue> match )            => Find(match, 0);
    public virtual TValue? Find( RefCheck<TValue> match, int start ) => Find(match, start, Count - 1);
    public virtual TValue? Find( RefCheck<TValue> match, int start, int endInclusive )
    {
        Guard.IsLessThanOrEqualTo(start, endInclusive);
        Guard.IsInRangeFor(start,        buffer, nameof(start));
        Guard.IsInRangeFor(endInclusive, buffer, nameof(endInclusive));
        ReadOnlySpan<TValue> span = AsSpan(start, endInclusive - start);
        return span.FirstOrDefault(match);
    }
    public virtual TValue? FindLast( RefCheck<TValue> match )            => FindLast(match, 0);
    public virtual TValue? FindLast( RefCheck<TValue> match, int start ) => FindLast(match, start, Count - 1);
    public virtual TValue? FindLast( RefCheck<TValue> match, int start, int endInclusive )
    {
        Guard.IsLessThanOrEqualTo(start, endInclusive);
        Guard.IsInRangeFor(start,        buffer, nameof(start));
        Guard.IsInRangeFor(endInclusive, buffer, nameof(endInclusive));

        ReadOnlySpan<TValue> span = AsSpan(start, endInclusive - start);
        return span.LastOrDefault(match);
    }
    public virtual TValue[] FindAll( RefCheck<TValue> match )            => FindAll(match, 0);
    public virtual TValue[] FindAll( RefCheck<TValue> match, int start ) => FindAll(match, start, Count - 1);
    public virtual TValue[] FindAll( RefCheck<TValue> match, int start, int endInclusive )
    {
        Guard.IsLessThanOrEqualTo(start, endInclusive);
        Guard.IsInRangeFor(start,        buffer, nameof(start));
        Guard.IsInRangeFor(endInclusive, buffer, nameof(endInclusive));
        List<TValue>         list = new(Count);
        ReadOnlySpan<TValue> span = AsSpan(start, endInclusive - start);

        foreach ( TValue value in span )
        {
            if ( match(in value) ) { list.Add(value); }
        }

        return list.ToArray();
    }


    public virtual bool TryAdd( TValue                           value )            => InternalTryAdd(in value);
    public virtual void Add( TValue                              value )            => InternalAdd(in value);
    public virtual void Add( TValue                              value, int count ) => InternalAdd(in value, count);
    public virtual void Add( params ReadOnlySpan<TValue>         values ) => InternalAdd(values);
    public virtual void Add( IEnumerable<TValue>                 values ) => InternalAdd(values);
    public virtual void Add( ref readonly ReadOnlyMemory<TValue> values ) => InternalAdd(values.Span);
    public virtual void Add( ref readonly ImmutableArray<TValue> values ) => InternalAdd(values.AsSpan());
    public virtual ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        InternalAdd(in value);
        return ValueTask.CompletedTask;
    }


    public virtual ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default ) { return ValueTask.FromResult(InternalTryAdd(in value)); }
    public virtual ValueTask TryAddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        foreach ( TValue value in values ) { InternalTryAdd(in value); }

        return ValueTask.CompletedTask;
    }
    public virtual async ValueTask TryAddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await foreach ( TValue value in values.WithCancellation(token) ) { InternalTryAdd(in value); }
    }
    public virtual ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        InternalAdd(values.Span);
        return ValueTask.CompletedTask;
    }
    public virtual ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        InternalAdd(values.AsSpan());
        return ValueTask.CompletedTask;
    }
    public virtual ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        InternalAdd(values);
        return ValueTask.CompletedTask;
    }
    public virtual async ValueTask AddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await foreach ( TValue value in values.WithCancellation(token) ) { InternalAdd(in value); }
    }


    public virtual async ValueTask AddOrUpdate( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await foreach ( TValue value in values.WithCancellation(token) ) { InternalAddOrUpdate(in value); }
    }
    public virtual void AddOrUpdate( TValue value ) => InternalAddOrUpdate(in value);
    public virtual void AddOrUpdate( IEnumerable<TValue> values )
    {
        foreach ( TValue value in values ) { InternalAddOrUpdate(in value); }
    }
    public void AddOrUpdate( ref readonly ReadOnlyMemory<TValue> values ) => AddOrUpdate(values.Span);
    public void AddOrUpdate( ref readonly ImmutableArray<TValue> values ) => AddOrUpdate(values.AsSpan());
    public virtual void AddOrUpdate( params ReadOnlySpan<TValue> values )
    {
        Sort();
        foreach ( TValue value in values ) { InternalAddOrUpdate(in value); }
    }
    public virtual void AddRange( TValue                      value, int count ) => InternalAdd(in value, count);
    public virtual void AddRange( params ReadOnlySpan<TValue> values )     => InternalAdd(values);
    public virtual void AddRange( IEnumerable<TValue>         enumerable ) => InternalAdd(enumerable);


    public virtual void CopyTo( TValue[] array )                                                                  => buffer.CopyTo(array);
    public virtual void CopyTo( TValue[] array, int destinationStartIndex )                                       => buffer.CopyTo(array,            destinationStartIndex);
    public virtual void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 ) => buffer.CopyTo(sourceStartIndex, array, length, destinationStartIndex);


    public virtual void Insert( int index,      TValue                              value )            => InternalInsert(index,      in value);
    public virtual void Insert( int startIndex, TValue                              value, int count ) => InternalInsert(startIndex, in value, count);
    public virtual void Insert( int startIndex, IEnumerable<TValue>                 collection ) => InternalInsert(startIndex, collection);
    public virtual void Insert( int startIndex, params       ReadOnlySpan<TValue>   values )     => InternalInsert(startIndex, values);
    public virtual void Insert( int startIndex, ref readonly ReadOnlyMemory<TValue> collection ) => Insert(startIndex, collection.Span);
    public virtual void Insert( int startIndex, ref readonly ImmutableArray<TValue> collection ) => Insert(startIndex, collection.AsSpan());
    public virtual ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        InternalInsert(index, in value);
        return ValueTask.CompletedTask;
    }


    public         void Replace( int     startIndex, TValue                      value, int count = 1 ) => InternalReplace(startIndex, in value, count);
    public         void Replace( int     startIndex, params ReadOnlySpan<TValue> values ) => InternalReplace(startIndex, values);
    public virtual void RemoveRange( int startIndex, int                         count )  => InternalRemove(startIndex, count);


    public virtual ValueTask<bool> RemoveAsync( TValue              value,  CancellationToken token = default ) { return ValueTask.FromResult(InternalRemove(in value)); }
    public virtual ValueTask<int>  RemoveAsync( RefCheck<TValue>    match,  CancellationToken token = default ) { return ValueTask.FromResult(InternalRemove(match)); }
    public virtual ValueTask<int>  RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default ) { return ValueTask.FromResult(InternalRemove(values)); }
    public virtual async ValueTask RemoveAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        await foreach ( TValue value in values.WithCancellation(token) ) { InternalRemove(in value); }
    }
    public virtual ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default ) { return ValueTask.FromResult(InternalRemove(values.Span)); }
    public virtual ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default ) { return ValueTask.FromResult(InternalRemove(values.AsSpan())); }

    public virtual int  Remove( RefCheck<TValue>                    match )                                        => InternalRemove(match);
    public virtual bool Remove( TValue                              value )                                        => InternalRemove(in value);
    public virtual int  Remove( IEnumerable<TValue>                 values )                                       => InternalRemove(values);
    public virtual int  Remove( params       ReadOnlySpan<TValue>   values )                                       => InternalRemove(values);
    public         int  Remove( ref readonly ReadOnlyMemory<TValue> values )                                       => Remove(values.Span);
    public         int  Remove( ref readonly ImmutableArray<TValue> values )                                       => Remove(values.AsSpan());
    void IList<TValue>. RemoveAt( int                               index )                                        => RemoveAt(index);
    void IList.         RemoveAt( int                               index )                                        => RemoveAt(index);
    public virtual bool RemoveAt( int                               index )                                        => InternalRemoveAt(index, out _);
    public virtual bool RemoveAt( int                               index, [NotNullWhen(true)] out TValue? value ) => InternalRemoveAt(index, out value);


    public virtual void Reverse()                       => InternalReverse();
    public virtual void Reverse( int start, int count ) => InternalReverse(start, count);


    public virtual void Sort()                                                                => InternalSort(comparer);
    public virtual void Sort( Comparer<TValue>   compare )                                    => InternalSort(compare);
    public virtual void Sort( Comparison<TValue> compare )                                    => InternalSort(compare);
    public virtual void Sort( int                start, int count )                           => InternalSort(start, count, comparer);
    public virtual void Sort( int                start, int count, Comparer<TValue> compare ) => InternalSort(start, count, compare);
    public virtual ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        InternalSort(compare);
        return ValueTask.CompletedTask;
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync(start, count, comparer, token);
    public virtual ValueTask SortAsync( int start, int count, Comparer<TValue> compare, CancellationToken token = default )
    {
        InternalSort(start, count, compare);
        return ValueTask.CompletedTask;
    }


    void ICollection.CopyTo( Array array, int start )
    {
        if ( array is TValue[] values ) { CopyTo(values, start); }
    }
    void IList.Remove( object? value )
    {
        if ( value is TValue x ) { buffer.Remove(x); }
    }
    int IList.Add( object? value )
    {
        if ( value is not TValue x ) { return NOT_FOUND; }

        buffer.Add(x);
        return buffer.Count;
    }
    bool IList.Contains( object? value ) => value is TValue x && buffer.Contains(x);
    int IList.IndexOf( object? value ) => value is TValue x
                                              ? buffer.IndexOf(x)
                                              : NOT_FOUND;
    void IList.Insert( int index, object? value )
    {
        if ( value is TValue x ) { buffer[index] = x; }
    }


    public virtual bool Contains( TValue value ) => InternalContains(in value);
    public virtual bool Contains( params ReadOnlySpan<TValue> values )
    {
        ReadOnlySpan<TValue> span = AsSpan();
        return span.ContainsAny(values);
    }
    public virtual ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default ) { return ValueTask.FromResult(InternalContains(in value)); }


    public virtual void Clear() => InternalClear();
    public virtual ValueTask ClearAsync( CancellationToken token = default )
    {
        InternalClear();
        return ValueTask.CompletedTask;
    }


    [Pure] [MustDisposeResource] protected internal override FilterBuffer<TValue> FilteredValues()
    {
        ReadOnlySpan<TValue>   span   = AsSpan();
        FilterBuffer<TValue>   values = new(span.Length);
        FilterDelegate<TValue> filter = GetFilter(); 

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( filter(i, in span[i]) ) { values.Add(in span[i]); }
        }

        return values;
    }


    /// <summary> Use With Caution -- Do not modify the <see cref="buffer"/> while the span is being used. </summary>
    public virtual ReadOnlySpan<TValue> AsSpan() => CollectionsMarshal.AsSpan(buffer);


    /// <summary> Use With Caution -- Do not modify the <see cref="buffer"/> while the span is being used. </summary>
    public virtual ReadOnlySpan<TValue> AsSpan( int start, int length ) => AsSpan()
       .Slice(start, length);


    public virtual void EnsureCapacity( int capacity ) => buffer.EnsureCapacity(buffer.Count + capacity);
    public virtual void TrimExcess()                   => buffer.TrimExcess();
}
