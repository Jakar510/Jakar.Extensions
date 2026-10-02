// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  15:41

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
public sealed partial class ObservableCollection<TValue>( Comparer<TValue> comparer, int capacity = DEFAULT_CAPACITY ) : ObservableCollection<ObservableCollection<TValue>, TValue>(comparer, capacity), ICollectionAlerts<ObservableCollection<TValue>, TValue>
    where TValue : IEquatable<TValue>
{
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
    public ObservableCollection( TValue[]                            values ) : this(Comparer<TValue>.Default, new ReadOnlySpan<TValue>(values)) { }
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
    public override bool Equals( object?                            other )                                     => ReferenceEquals(this, other) || ( other is ObservableCollection<TValue> x && Equals(x) );
    public static   bool operator ==( ObservableCollection<TValue>? left, ObservableCollection<TValue>? right ) => EqualityComparer<ObservableCollection<TValue>>.Default.Equals(left, right);
    public static   bool operator !=( ObservableCollection<TValue>? left, ObservableCollection<TValue>? right ) => !EqualityComparer<ObservableCollection<TValue>>.Default.Equals(left, right);
    public static   bool operator >( ObservableCollection<TValue>   left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) > 0;
    public static   bool operator >=( ObservableCollection<TValue>  left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) >= 0;
    public static   bool operator <( ObservableCollection<TValue>   left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) < 0;
    public static   bool operator <=( ObservableCollection<TValue>  left, ObservableCollection<TValue>  right ) => Comparer<ObservableCollection<TValue>>.Default.Compare(left, right) <= 0;
}



[Serializable]
public abstract partial class ObservableCollection<TSelf, TValue>( Comparer<TValue> comparer, int capacity = DEFAULT_CAPACITY ) : CollectionAlerts<TSelf, TValue>, IObservableCollection<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList
    where TValue : IEquatable<TValue>
    where TSelf : ObservableCollection<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>
{
    protected internal readonly Comparer<TValue> comparer = comparer;
    protected internal readonly List<TValue>     buffer   = new(capacity);


    public override int Capacity       { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => buffer.Capacity; }
    public override int Count          { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => buffer.Count; }
    bool IList.         IsFixedSize    { [MethodImpl(       MethodImplOptions.AggressiveInlining)] get => false; }
    public bool         IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining)] get; init; }
    bool ICollection.   IsSynchronized { [MethodImpl(       MethodImplOptions.AggressiveInlining)] get => false; }
    object? IList.this[ int                index ] { get => Get(index); set => Set(index, Cast(value)); }
    public TValue this[ int                index ] { get => Get(index); set => Set(index, value); }
    TValue IReadOnlyList<TValue>.this[ int index ] => Get(index);
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
    protected ObservableCollection( TValue[]                            values ) : this(Comparer<TValue>.Default, new ReadOnlySpan<TValue>(values)) { }
    protected ObservableCollection( TValue[]                            values, Comparer<TValue> comparer ) : this(comparer, new ReadOnlySpan<TValue>(values)) { }
    protected ObservableCollection( IEnumerable<TValue>                 values ) : this(values, Comparer<TValue>.Default) { }
    protected ObservableCollection( IEnumerable<TValue>                 values, Comparer<TValue> comparer ) : this(comparer) => InternalAdd(values);
    protected override void Dispose( bool disposing )
    {
        base.Dispose(disposing);
        if ( disposing ) { buffer.Clear(); }
    }


    public virtual TValue[] ToArray() => buffer.ToArray();


    // ─── Change notifications ────────────────────────────────────────────────
    // Single item changes raise their specific action; multi item changes raise Reset, because WPF (and most UI frameworks) don't support range actions.

    private void ItemsAdded( int index, int count )
    {
        switch ( count )
        {
            case <= 0: return;

            case 1:
                Added(in CollectionsMarshal.AsSpan(buffer)[index], index);
                return;

            default:
                Reset();
                return;
        }
    }


    // ─── Validation ──────────────────────────────────────────────────────────

    private static TValue Cast( object? value )
    {
        if ( value is TValue x ) { return x; }

        if ( value is null && default(TValue) is null ) { return default!; }

        throw new ArgumentException($"Value must be of type {typeof(TValue).Name}", nameof(value));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void ThrowIfOutOfRange( int index, [CallerArgumentExpression(nameof(index))] string? name = null )
    {
        if ( (uint)index >= (uint)buffer.Count ) { ThrowOutOfRange(index, name); }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void ThrowIfInvalidRange( int start, int count )
    {
        if ( start < 0 || count < 0 || (uint)( start + count ) > (uint)buffer.Count ) { ThrowOutOfRange(start, nameof(start), count); }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void ThrowIfInvalidInsert( int index )
    {
        if ( (uint)index > (uint)buffer.Count ) { ThrowOutOfRange(index, nameof(index)); }
    }
    [DoesNotReturn] private void ThrowOutOfRange( int index, string? name, int count = 1 ) => throw new ArgumentOutOfRangeException(name, index, $"[{index}, {index + count}) is outside of [0, {buffer.Count})");
    /// <summary> The inclusive range <c>[<paramref name="start"/>, <paramref name="endInclusive"/>]</c>; empty when the collection is empty. </summary>
    private ReadOnlySpan<TValue> Range( int start, int endInclusive )
    {
        int length = endInclusive - start + 1;
        if ( buffer.Count == 0 && start == 0 && length <= 0 ) { return default; }

        ThrowIfOutOfRange(start);
        ThrowIfOutOfRange(endInclusive);
        if ( length < 0 ) { throw new ArgumentOutOfRangeException(nameof(endInclusive), endInclusive, $"must be >= {nameof(start)} ({start})"); }

        return CollectionsMarshal.AsSpan(buffer).Slice(start, length);
    }


    // ─── Insert ──────────────────────────────────────────────────────────────

    protected internal virtual void InternalInsert( int i, ref readonly TValue value )
    {
        ThrowIfReadOnly();
        buffer.Insert(i, value);
        Added(in value, i);
    }
    protected internal virtual void InternalInsert( int startIndex, IEnumerable<TValue> collection )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidInsert(startIndex);
        int count = buffer.Count;
        buffer.InsertRange(startIndex, collection);
        ItemsAdded(startIndex, buffer.Count - count);
    }
    protected internal virtual void InternalInsert( int startIndex, params ReadOnlySpan<TValue> collection )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidInsert(startIndex);
        buffer.InsertRange(startIndex, collection);
        ItemsAdded(startIndex, collection.Length);
    }
    protected internal virtual void InternalInsert( int index, ref readonly TValue value, int count )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidInsert(index);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if ( count == 0 ) { return; }

        int old = buffer.Count;
        CollectionsMarshal.SetCount(buffer, old + count);
        Span<TValue> span = CollectionsMarshal.AsSpan(buffer);
        span[index..old].CopyTo(span[( index + count )..]);
        span.Slice(index, count).Fill(value);
        ItemsAdded(index, count);
    }


    // ─── Replace ─────────────────────────────────────────────────────────────

    protected internal virtual void InternalReplace( int startIndex, ref readonly TValue value, int count )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidRange(startIndex, count);

        if ( count == 1 )
        {
            InternalSet(startIndex, in value);
            return;
        }

        CollectionsMarshal.AsSpan(buffer).Slice(startIndex, count).Fill(value);
        if ( count > 0 ) { Reset(); }
    }
    protected internal virtual void InternalReplace( int startIndex, params ReadOnlySpan<TValue> value )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidRange(startIndex, value.Length);

        if ( value.Length == 1 )
        {
            InternalSet(startIndex, in value[0]);
            return;
        }

        value.CopyTo(CollectionsMarshal.AsSpan(buffer)[startIndex..]);
        if ( value.Length > 0 ) { Reset(); }
    }


    // ─── Remove ──────────────────────────────────────────────────────────────

    protected internal virtual void InternalRemove( int start, int count )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidRange(start, count);

        switch ( count )
        {
            case 0: return;

            case 1:
                InternalRemoveAt(start, out _);
                return;

            default:
                buffer.RemoveRange(start, count);
                Reset();
                return;
        }
    }
    protected internal virtual bool InternalRemove( ref readonly TValue value )
    {
        ThrowIfReadOnly();
        int index = buffer.IndexOf(value);
        if ( index < 0 ) { return false; }

        TValue removed = buffer[index];
        buffer.RemoveAt(index);
        Removed(in removed, index);
        return true;
    }
    /// <summary> Removes every item matching <paramref name="match"/> in a single pass. </summary>
    protected internal virtual int InternalRemove( RefCheck<TValue> match )
    {
        ThrowIfReadOnly();
        Span<TValue> span  = CollectionsMarshal.AsSpan(buffer);
        int          write = 0;
        int          first = NOT_FOUND;
        TValue?      item  = default;

        for ( int read = 0; read < span.Length; read++ )
        {
            if ( match(in span[read]) )
            {
                if ( first < 0 )
                {
                    first = read;
                    item  = span[read];
                }

                continue;
            }

            if ( write != read ) { span[write] = span[read]; }

            write++;
        }

        int removed = span.Length - write;
        if ( removed == 0 ) { return 0; }

        buffer.RemoveRange(write, removed); // also clears the vacated slots

        if ( removed == 1 ) { Removed(in item!, first); }
        else { Reset(); }

        return removed;
    }
    protected internal virtual int InternalRemove( IEnumerable<TValue> values )
    {
        ThrowIfReadOnly();
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( InternalRemove(in value) ) { results++; }
        }

        return results;
    }
    protected internal virtual int InternalRemove( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( ref readonly TValue value in values )
        {
            if ( InternalRemove(in value) ) { results++; }
        }

        return results;
    }
    protected internal virtual bool InternalRemoveAt( int index, [NotNullWhen(true)] out TValue? value )
    {
        ThrowIfReadOnly();

        if ( (uint)index >= (uint)buffer.Count )
        {
            value = default;
            return false;
        }

        value = buffer[index];
        buffer.RemoveAt(index);
        Removed(in value, index);
        return true;
    }
    protected internal virtual void InternalClear()
    {
        ThrowIfReadOnly();
        buffer.Clear();
        Reset();
    }


    protected internal virtual bool InternalContains( ref readonly TValue value ) => buffer.Contains(value);


    // ─── Add ─────────────────────────────────────────────────────────────────

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
        Added(in value, buffer.Count - 1);
    }
    protected internal virtual void InternalAdd( ref readonly TValue value, int count ) => InternalInsert(buffer.Count, in value, count);
    protected internal virtual void InternalAdd( IEnumerable<TValue> values )
    {
        ThrowIfReadOnly();
        int count = buffer.Count;
        buffer.AddRange(values);
        ItemsAdded(count, buffer.Count - count);
    }
    protected internal virtual void InternalAdd( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        int count = buffer.Count;
        buffer.AddRange(values);
        ItemsAdded(count, values.Length);
    }
    protected internal virtual void InternalAdd<TEnumerator>( ValueEnumerable<TEnumerator, TValue> values )
        where TEnumerator : struct, IValueEnumerator<TValue>, allows ref struct
    {
        using PooledArray<TValue> enumerator = values.ToArrayPool();
        InternalAdd(enumerator.Span);
    }
    protected internal virtual void InternalAddOrUpdate( ref readonly TValue value )
    {
        ThrowIfReadOnly();
        int index = buffer.IndexOf(value);

        if ( index >= 0 ) { InternalSet(index, in value); }
        else { InternalAdd(in value); }
    }


    // ─── Sort / Reverse ──────────────────────────────────────────────────────

    protected internal virtual void InternalSort( Comparer<TValue> compare )
    {
        ThrowIfReadOnly();
        CollectionsMarshal.AsSpan(buffer).Sort(compare);
        Reset();
    }
    protected internal virtual void InternalSort( Comparison<TValue> compare )
    {
        ThrowIfReadOnly();
        CollectionsMarshal.AsSpan(buffer).Sort(compare);
        Reset();
    }
    protected internal virtual void InternalSort( int start, int length, Comparer<TValue> compare )
    {
        ThrowIfReadOnly();
        ThrowIfInvalidRange(start, length);
        CollectionsMarshal.AsSpan(buffer).Slice(start, length).Sort(compare);
        Reset();
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
        ThrowIfInvalidRange(start, length);
        buffer.Reverse(start, length);
        Reset();
    }


    protected void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new NotSupportedException("Collection is read-only."); }
    }


    // ─── Get / Set ───────────────────────────────────────────────────────────

    protected internal virtual TValue InternalGet( int index )
    {
        ThrowIfOutOfRange(index);
        return buffer[index];
    }
    protected internal virtual void InternalSet( int index, ref readonly TValue value )
    {
        ThrowIfReadOnly();
        ThrowIfOutOfRange(index);
        ref TValue slot = ref CollectionsMarshal.AsSpan(buffer)[index];
        TValue     old  = slot;
        slot = value;
        Replaced(in old, in value, index);
    }


    public virtual TValue Get( int index )               => InternalGet(index);
    public virtual void   Set( int index, TValue value ) => InternalSet(index, in value);


    // ─── Search ──────────────────────────────────────────────────────────────

    public virtual bool Exists( RefCheck<TValue> match ) => FindIndex(match) >= 0;


    /// <summary> First index matching <paramref name="match"/> at or after <paramref name="start"/>, or <see cref="NOT_FOUND"/>. </summary>
    public virtual int FindIndex( RefCheck<TValue> match, int start = 0 ) => FindIndex(match, start, buffer.Count - 1);
    /// <summary> First index matching <paramref name="match"/> in <c>[<paramref name="start"/>, <paramref name="endInclusive"/>]</c>, or <see cref="NOT_FOUND"/>. </summary>
    public virtual int FindIndex( RefCheck<TValue> match, int start, int endInclusive ) => FindIndexCore(match, start, endInclusive);


    /// <summary> Last index matching <paramref name="match"/> at or after <paramref name="start"/>, or <see cref="NOT_FOUND"/>. </summary>
    public virtual int FindLastIndex( RefCheck<TValue> match, int start = 0 ) => FindLastIndex(match, start, buffer.Count - 1);
    /// <summary> Last index matching <paramref name="match"/> in <c>[<paramref name="start"/>, <paramref name="endInclusive"/>]</c>, or <see cref="NOT_FOUND"/>. </summary>
    public virtual int FindLastIndex( RefCheck<TValue> match, int start, int endInclusive ) => FindLastIndexCore(match, start, endInclusive);


    public virtual int IndexOf( TValue value ) => buffer.IndexOf(value);
    public virtual int IndexOf( TValue value, int start )
    {
        ThrowIfInvalidInsert(start);
        return buffer.IndexOf(value, start);
    }
    /// <summary> First index of <paramref name="value"/> in the <paramref name="count"/> items starting at <paramref name="start"/>. </summary>
    public virtual int IndexOf( TValue value, int start, int count )
    {
        ThrowIfInvalidRange(start, count);
        return buffer.IndexOf(value, start, count);
    }


    public virtual int LastIndexOf( TValue value ) => buffer.LastIndexOf(value);
    /// <summary> Last index of <paramref name="value"/> searching backwards from <paramref name="start"/>. </summary>
    public virtual int LastIndexOf( TValue value, int start )
    {
        if ( buffer.Count == 0 ) { return NOT_FOUND; }

        ThrowIfOutOfRange(start);
        return buffer.LastIndexOf(value, start);
    }
    /// <summary> Last index of <paramref name="value"/> in the <paramref name="count"/> items ending at <paramref name="start"/>, searching backwards. </summary>
    public virtual int LastIndexOf( TValue value, int start, int count )
    {
        if ( buffer.Count == 0 ) { return NOT_FOUND; }

        ThrowIfOutOfRange(start);
        ThrowIfInvalidRange(start - count + 1, count);
        return buffer.LastIndexOf(value, start, count);
    }


    public virtual int FindCount( RefCheck<TValue> match )
    {
        ReadOnlySpan<TValue> span  = CollectionsMarshal.AsSpan(buffer);
        int                  count = 0;

        foreach ( ref readonly TValue value in span )
        {
            if ( match(in value) ) { count++; }
        }

        return count;
    }
    public virtual TValue? Find( RefCheck<TValue> match )            => Find(match, 0);
    public virtual TValue? Find( RefCheck<TValue> match, int start ) => Find(match, start, buffer.Count - 1);
    public virtual TValue? Find( RefCheck<TValue> match, int start, int endInclusive )
    {
        int index = FindIndexCore(match, start, endInclusive);

        return index >= 0
                   ? buffer[index]
                   : default;
    }
    public virtual TValue? FindLast( RefCheck<TValue> match )            => FindLast(match, 0);
    public virtual TValue? FindLast( RefCheck<TValue> match, int start ) => FindLast(match, start, buffer.Count - 1);
    public virtual TValue? FindLast( RefCheck<TValue> match, int start, int endInclusive )
    {
        int index = FindLastIndexCore(match, start, endInclusive);

        return index >= 0
                   ? buffer[index]
                   : default;
    }
    public virtual TValue[] FindAll( RefCheck<TValue> match )            => FindAll(match, 0);
    public virtual TValue[] FindAll( RefCheck<TValue> match, int start ) => FindAll(match, start, buffer.Count - 1);
    public virtual TValue[] FindAll( RefCheck<TValue> match, int start, int endInclusive )
    {
        ReadOnlySpan<TValue> span = Range(start, endInclusive);
        if ( span.IsEmpty ) { return []; }

        ArrayBuffer<TValue> values = new(span.Length);

        foreach ( ref readonly TValue value in span )
        {
            if ( match(in value) ) { values.Add(in value); }
        }

        return values.ToArray();
    }
    private int FindIndexCore( RefCheck<TValue> match, int start, int endInclusive )
    {
        ReadOnlySpan<TValue> span = Range(start, endInclusive);

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( match(in span[i]) ) { return start + i; }
        }

        return NOT_FOUND;
    }
    private int FindLastIndexCore( RefCheck<TValue> match, int start, int endInclusive )
    {
        ReadOnlySpan<TValue> span = Range(start, endInclusive);

        for ( int i = span.Length - 1; i >= 0; i-- )
        {
            if ( match(in span[i]) ) { return start + i; }
        }

        return NOT_FOUND;
    }


    // ─── Public mutation API ─────────────────────────────────────────────────

    public virtual void Add( TValue                      value )            => InternalAdd(in value);
    public virtual void Add( TValue                      value, int count ) => InternalAdd(in value, count);
    public virtual void Add( params ReadOnlySpan<TValue> values ) => InternalAdd(values);
    public virtual void Add( IEnumerable<TValue>         values ) => InternalAdd(values);
    public virtual void Add<TEnumerator>( ValueEnumerable<TEnumerator, TValue> values )
        where TEnumerator : struct, IValueEnumerator<TValue>, allows ref struct => InternalAdd(values);
    public virtual void Add( ref readonly ReadOnlyMemory<TValue> values ) => Add(values.Span);
    public virtual void Add( ref readonly ImmutableArray<TValue> values ) => Add(values.AsSpan());


    public virtual bool            TryAdd( TValue      value )                                    => InternalTryAdd(in value);
    public virtual ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default ) => ValueTask.FromResult(TryAdd(value));
    public virtual ValueTask TryAddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        foreach ( TValue value in values ) { InternalTryAdd(in value); }

        return ValueTask.CompletedTask;
    }
    public virtual async ValueTask TryAddAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalTryAdd(in value); }
    }


    public virtual ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        InternalAdd(in value);
        return ValueTask.CompletedTask;
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
        await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalAdd(in value); }
    }


    public virtual async ValueTask AddOrUpdate( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalAddOrUpdate(in value); }
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
        foreach ( ref readonly TValue value in values ) { InternalAddOrUpdate(in value); }
    }
    public virtual void AddRange( TValue                      value, int count ) => Add(value, count);
    public virtual void AddRange( params ReadOnlySpan<TValue> values )     => Add(values);
    public virtual void AddRange( IEnumerable<TValue>         enumerable ) => Add(enumerable);
    public virtual void AddRange<TEnumerator>( ValueEnumerable<TEnumerator, TValue> values )
        where TEnumerator : struct, IValueEnumerator<TValue>, allows ref struct => Add(values);


    public virtual void CopyTo( TValue[] array )                                                                  => buffer.CopyTo(array);
    public virtual void CopyTo( TValue[] array, int destinationStartIndex )                                       => buffer.CopyTo(array,            destinationStartIndex);
    public virtual void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 ) => buffer.CopyTo(sourceStartIndex, array, destinationStartIndex, length);


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


    public virtual void Replace( int     startIndex, TValue                      value, int count = 1 ) => InternalReplace(startIndex, in value, count);
    public virtual void Replace( int     startIndex, params ReadOnlySpan<TValue> values ) => InternalReplace(startIndex, values);
    public virtual void RemoveRange( int startIndex, int                         count )  => InternalRemove(startIndex, count);


    public virtual ValueTask<bool> RemoveAsync( TValue              value,  CancellationToken token = default ) => ValueTask.FromResult(InternalRemove(in value));
    public virtual ValueTask<int>  RemoveAsync( RefCheck<TValue>    match,  CancellationToken token = default ) => ValueTask.FromResult(InternalRemove(match));
    public virtual ValueTask<int>  RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default ) => ValueTask.FromResult(InternalRemove(values));
    public virtual async ValueTask RemoveAsync( IAsyncEnumerable<TValue> values, CancellationToken token = default )
    {
        await foreach ( TValue value in values.WithCancellation(token).ConfigureAwait(false) ) { InternalRemove(in value); }
    }
    public virtual ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default ) => ValueTask.FromResult(InternalRemove(values.Span));
    public virtual ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default ) => ValueTask.FromResult(InternalRemove(values.AsSpan()));

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
    public virtual void Sort( int                start, int count )                           => Sort(start, count, comparer);
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


    // ─── IList (non generic): routed through the public API, so events, read-only and locking apply ───

    void ICollection.CopyTo( Array array, int start )
    {
        if ( array is TValue[] values ) { CopyTo(values, start); }
        else { ( (ICollection)buffer ).CopyTo(array, start); }
    }
    void IList.Remove( object?   value ) => Remove(Cast(value));
    int IList. Add( object?      value ) => AddAndGetIndex(Cast(value));
    bool IList.Contains( object? value ) => value is TValue x && Contains(x);
    int IList. IndexOf( object?  value ) => value is TValue x
                                                ? IndexOf(x)
                                                : NOT_FOUND;
    void IList.Insert( int index, object? value ) => Insert(index, Cast(value));
    /// <summary> Adds <paramref name="value"/> and returns its index (<see cref="IList.Add"/>). </summary>
    protected virtual int AddAndGetIndex( TValue value )
    {
        InternalAdd(in value);
        return buffer.Count - 1;
    }


    public virtual bool Contains( TValue value ) => InternalContains(in value);
    public virtual bool Contains( params ReadOnlySpan<TValue> values )
    {
        ReadOnlySpan<TValue> span = CollectionsMarshal.AsSpan(buffer);
        return span.ContainsAny(values);
    }
    public virtual ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default ) => ValueTask.FromResult(Contains(value));


    public virtual void Clear() => InternalClear();
    public virtual ValueTask ClearAsync( CancellationToken token = default )
    {
        InternalClear();
        return ValueTask.CompletedTask;
    }


    /// <remarks> Without a filter the items are block-copied; otherwise the filter delegate is created once and invoked per item. </remarks>
    [Pure] [MustDisposeResource] protected internal override ArrayBuffer<TValue> FilteredValues()
    {
        ReadOnlySpan<TValue> span = CollectionsMarshal.AsSpan(buffer);
        if ( !HasFilter ) { return new ArrayBuffer<TValue>(span); }

        ArrayBuffer<TValue>    values = new(span.Length);
        FilterDelegate<TValue> filter = GetFilter();

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( filter(i, in span[i]) ) { values.Add(in span[i]); }
        }

        return values;
    }


    /// <summary> Use With Caution -- Do not modify the <see cref="buffer"/> while the span is being used. </summary>
    [UseWithCaution] public virtual ReadOnlySpan<TValue> AsSpan() => CollectionsMarshal.AsSpan(buffer);


    /// <summary> Use With Caution -- Do not modify the <see cref="buffer"/> while the span is being used. </summary>
    [UseWithCaution] public virtual ReadOnlySpan<TValue> AsSpan( int start, int length ) => CollectionsMarshal.AsSpan(buffer).Slice(start, length);


    /// <summary> Ensures room for <paramref name="capacity"/> more items. </summary>
    public virtual void EnsureCapacity( int capacity ) => buffer.EnsureCapacity(buffer.Count + capacity);
    public virtual void TrimExcess()                   => buffer.TrimExcess();
}
