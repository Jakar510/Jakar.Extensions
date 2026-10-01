// Jakar.Extensions :: Jakar.Extensions
// 09/21/2024  10:09


using ZLinq;
using ZLinq.Linq;



namespace Jakar.Extensions;


/// <summary> A growable buffer backed by an <see cref="ArrayPool{T}"/> array. </summary>
/// <remarks>
///     This is a mutable struct: pass it by <see langword="ref"/>. Copies share the same array, so only one copy may grow or be disposed.
///     <see cref="Dispose"/> is idempotent per instance and a <see langword="default"/> buffer is valid (empty, zero capacity).
/// </remarks>
[StructLayout(LayoutKind.Auto)]
public struct Buffer<TValue> : IMemoryOwner<TValue>, IBufferWriter<TValue>
{
    private TValue[]? _array;
    private int       _length;


    public readonly int  Capacity     { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _array?.Length ?? 0; }
    public readonly int  FreeCapacity { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Capacity - _length; }
    public          int  Length       { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => _length; set => _length = Math.Clamp(value, 0, Capacity); }
    public readonly bool IsEmpty      { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _length == 0; }
    public readonly bool IsNotEmpty   { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _length > 0; }
    public          bool IsReadOnly   { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get; init; } = false;
    public readonly ref TValue this[ int     index ] { [Pure] get => ref Values[index]; }
    public readonly ref TValue this[ Index   index ] { [Pure] get => ref Values[index]; }
    public readonly Span<TValue> this[ Range range ] { [Pure] get => Values[range]; }
    public readonly Span<TValue> this[ int   start, int length ] { [Pure] get => Values.Slice(start, length); }
    public readonly Memory<TValue> Memory { [Pure] get => new(_array, 0, _length); }
    /// <summary> The whole backing storage (<see cref="Capacity"/> elements). </summary>
    public readonly Span<TValue> Span { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(_array); }
    /// <summary> The unused storage after <see cref="Length"/>. </summary>
    public readonly Span<TValue> Next { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(_array, _length, Capacity - _length); }
    /// <summary> The unused storage after <see cref="Length"/>, as <see cref="Memory{T}"/>. </summary>
    internal readonly Memory<TValue> FreeMemory => new(_array, _length, Capacity - _length);
    /// <summary> The written elements (<see cref="Length"/> elements). </summary>
    public readonly Span<TValue> Values { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(_array, 0, _length); }


    [MustDisposeResource] public Buffer() { }
    /// <summary> Starts with a copy of <paramref name="span"/> (<see cref="Length"/> = <paramref name="span"/>.Length). </summary>
    [MustDisposeResource] public Buffer( params ReadOnlySpan<TValue> span ) : this(span.Length)
    {
        span.CopyTo(Span);
        _length = span.Length;
    }
    [MustDisposeResource] public Buffer( int capacity )
    {
        _array  = ArrayPool<TValue>.Shared.Rent(capacity);
        _length = 0;
    }
    /// <summary> Returns the array to the pool. Safe to call more than once on the same instance. </summary>
    public void Dispose()
    {
        TValue[]? array = _array;
        _array  = null;
        _length = 0;
        if ( array is not null ) { ArrayPool<TValue>.Shared.Return(array, RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()); }
    }


    [HandlesResourceDisposal] public TValue[] ToArray()
    {
        TValue[] array = Values.ToArray();
        Dispose();
        return array;
    }
    [Pure] [MustDisposeResource] [HandlesResourceDisposal] public ArrayBuffer<TValue> ToArrayBuffer()
    {
        ArrayBuffer<TValue> array = new(Values);
        Dispose();
        return array;
    }
    public void Clear()
    {
        Span.Clear();
        _length = 0;
    }
    public readonly void Fill( TValue value ) => Span.Fill(value);


    public readonly void CopyTo( Span<TValue> array ) => Values.CopyTo(array);
    public readonly bool TryCopyTo( Span<TValue> destination, out int length )
    {
        if ( Values.TryCopyTo(destination) )
        {
            length = _length;
            return true;
        }

        length = 0;
        return false;
    }


    public readonly void Reverse( int start, int length ) => Values.Slice(start, length).Reverse();
    public readonly void Reverse()                        => Values.Reverse();


    public ReadOnlySpan<TValue> AsSpan( TValue? terminate )
    {
        if ( terminate is not null ) { Append(terminate); }

        return Values;
    }
    public readonly Span<TValue> Slice( int start )             => Slice(start, Capacity - start);
    public readonly Span<TValue> Slice( int start, int length ) => Span.Slice(start, length);


    // ─── Search ──────────────────────────────────────────────────────────────
    // All searches return NOT_FOUND on an empty buffer instead of throwing.

    public readonly int IndexOf( TValue value ) => IndexOf(Values, value);
    public readonly int IndexOf( TValue value, int start )
    {
        Guard.IsInRange(start, 0, _length + 1);
        int index = IndexOf(Values[start..], value);

        return index < 0
                   ? NOT_FOUND
                   : index + start;
    }
    public readonly int IndexOf( TValue value, int start, int endInclusive )
    {
        if ( _length == 0 ) { return NOT_FOUND; }

        Guard.IsInRange(start,        0, _length);
        Guard.IsInRange(endInclusive, 0, _length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);
        int index = IndexOf(Values[start..( endInclusive + 1 )], value);

        return index < 0
                   ? NOT_FOUND
                   : index + start;
    }
    public readonly int FindIndex( Func<TValue, bool> match ) => _length == 0
                                                                     ? NOT_FOUND
                                                                     : FindIndex(match, 0);
    public readonly int FindIndex( Func<TValue, bool> match, int start ) => _length == 0
                                                                                ? NOT_FOUND
                                                                                : FindIndex(match, start, _length - 1);
    public readonly int FindIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        if ( _length == 0 ) { return NOT_FOUND; }

        Guard.IsInRange(start,        0, _length);
        Guard.IsInRange(endInclusive, 0, _length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match(span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }
    public readonly int LastIndexOf( TValue value ) => LastIndexOf(Values, value);
    public readonly int LastIndexOf( TValue value, int endInclusive ) => _length == 0
                                                                             ? NOT_FOUND
                                                                             : LastIndexOf(value, _length - 1, endInclusive);
    public readonly int LastIndexOf( TValue value, int start, int endInclusive )
    {
        if ( _length == 0 ) { return NOT_FOUND; }

        Guard.IsInRange(start,        0, _length);
        Guard.IsInRange(endInclusive, 0, _length);
        Guard.IsGreaterThanOrEqualTo(start, endInclusive);
        int index = LastIndexOf(Values[endInclusive..( start + 1 )], value);

        return index < 0
                   ? NOT_FOUND
                   : index + endInclusive;
    }


    public readonly int FindLastIndex( Func<TValue, bool> match ) => _length == 0
                                                                         ? NOT_FOUND
                                                                         : FindLastIndex(match, _length - 1, 0);
    public readonly int FindLastIndex( Func<TValue, bool> match, int endInclusive ) => _length == 0
                                                                                           ? NOT_FOUND
                                                                                           : FindLastIndex(match, _length - 1, endInclusive);
    public readonly int FindLastIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        if ( _length == 0 ) { return NOT_FOUND; }

        Guard.IsInRange(start,        0, _length);
        Guard.IsInRange(endInclusive, 0, _length);
        Guard.IsGreaterThanOrEqualTo(start, endInclusive);
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match(span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }
    public readonly TValue? FindLast( Func<TValue, bool> match ) => _length == 0
                                                                        ? default
                                                                        : FindLast(match, _length - 1, 0);
    public readonly TValue? FindLast( Func<TValue, bool> match, int endInclusive ) => _length == 0
                                                                                          ? default
                                                                                          : FindLast(match, _length - 1, endInclusive);
    public readonly TValue? FindLast( Func<TValue, bool> match, int start, int endInclusive )
    {
        int index = FindLastIndex(match, start, endInclusive);

        return index < 0
                   ? default
                   : Values[index];
    }
    public readonly TValue? Find( Func<TValue, bool> match ) => _length == 0
                                                                    ? default
                                                                    : Find(match, 0);
    public readonly TValue? Find( Func<TValue, bool> match, int start ) => _length == 0
                                                                               ? default
                                                                               : Find(match, start, _length - 1);
    public readonly TValue? Find( Func<TValue, bool> match, int start, int endInclusive )
    {
        int index = FindIndex(match, start, endInclusive);

        return index < 0
                   ? default
                   : Values[index];
    }
    [Pure] [MustDisposeResource] public readonly ArrayBuffer<TValue> FindAll( Func<TValue, bool> match ) => ArrayBuffer<TValue>.Create(AsValueEnumerable().Where(match));
    [Pure] [MustDisposeResource] public readonly ArrayBuffer<TValue> FindAll( Func<TValue, bool> match, int start ) => _length == 0
                                                                                                                       ? new ArrayBuffer<TValue>()
                                                                                                                       : FindAll(match, start, _length - 1);
    [Pure] [MustDisposeResource] public readonly ArrayBuffer<TValue> FindAll( Func<TValue, bool> match, int start, int endInclusive ) => ArrayBuffer<TValue>.Create(AsValueEnumerable(start, endInclusive).Where(match));


    public readonly bool Contains( TValue                      value ) => IndexOf(Values, value) >= 0;
    public readonly bool Contains( params ReadOnlySpan<TValue> value ) => Values.ContainsAll(EqualityComparer<TValue>.Default, value);


    // ─── Mutation ────────────────────────────────────────────────────────────

    /// <summary> Removes the element at <paramref name="index"/>, shifting the rest down. Returns <see langword="false"/> if <paramref name="index"/> is out of range. </summary>
    public bool RemoveAt( int index )
    {
        if ( (uint)index >= (uint)_length ) { return false; }

        Span<TValue> span = Span;
        span.Slice(index + 1, _length - index - 1).CopyTo(span[index..]);
        _length--;
        if ( RuntimeHelpers.IsReferenceOrContainsReferences<TValue>() ) { span[_length] = default!; }

        return true;
    }
    public bool Remove( TValue value ) => RemoveAt(IndexOf(Values, value));


    public readonly void Replace( int start, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo(count, 0);
        Guard.IsInRange(start + count, 0, Capacity + 1);

        Values.Slice(start, count).Fill(value);
    }
    public readonly void Replace( int start, params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange(start,                 0, _length);
        Guard.IsInRange(start + values.Length, 0, Capacity + 1);

        values.CopyTo(Span.Slice(start, values.Length));
    }


    /// <summary> Inserts <paramref name="count"/> copies of <paramref name="value"/> at <paramref name="start"/> (0..<see cref="Length"/>), shifting later elements up. Capacity must already be sufficient. </summary>
    internal void InsertInternal( int start, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo(count, 0);
        Guard.IsInRange(start,           0, _length + 1);
        Guard.IsInRange(_length + count, 0, Capacity + 1);
        if ( count == 0 ) { return; }

        Span<TValue> span = Span;
        span[start.._length].CopyTo(span[( start + count )..]);
        span.Slice(start, count).Fill(value);
        _length += count;
    }
    /// <summary> Inserts <paramref name="values"/> at <paramref name="start"/> (0..<see cref="Length"/>), shifting later elements up. Capacity must already be sufficient. </summary>
    internal void InsertInternal( int start, params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange(start,                   0, _length + 1);
        Guard.IsInRange(_length + values.Length, 0, Capacity + 1);
        if ( values.IsEmpty ) { return; }

        Span<TValue> span = Span;
        span[start.._length].CopyTo(span[( start + values.Length )..]);
        values.CopyTo(span[start..]);
        _length += values.Length;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] internal void AddInternal( TValue value ) => Span[_length++] = value;
    internal void AddInternal( TValue value, int count )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo(count, 0);
        Guard.IsInRange(_length + count, 0, Capacity + 1);

        Next[..count].Fill(value);
        _length += count;
    }
    internal void AddInternal( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        if ( values.IsEmpty ) { return; }

        values.CopyTo(Next);
        _length += values.Length;
    }
    internal void AddRangeInternal( IEnumerable<TValue> enumerable )
    {
        ThrowIfReadOnly();

        switch ( enumerable )
        {
            case TValue[] array:
            {
                EnsureCapacityFor(array.Length);
                array.AsSpan().CopyTo(Next);
                _length += array.Length;
                return;
            }

            case List<TValue> list:
            {
                EnsureCapacityFor(list.Count);
                list.AsSpan().CopyTo(Next);
                _length += list.Count;
                return;
            }

            case ICollection<TValue> collection:
            {
                int count = collection.Count;
                if ( count <= 0 ) { return; }

                EnsureCapacityFor(count);

                // ICollection<T>.CopyTo needs an array; copy straight into ours at the current length.
                collection.CopyTo(_array!, _length);
                _length += count;
                return;
            }

            default:
            {
                foreach ( TValue value in enumerable ) { Append(value); }

                return;
            }
        }
    }


    // ─── Growth ──────────────────────────────────────────────────────────────

    /// <summary> Appends <paramref name="value"/>, growing if needed. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Append( TValue value )
    {
        TValue[]? array  = _array;
        int       length = _length;

        if ( array is not null && (uint)length < (uint)array.Length && !IsReadOnly )
        {
            array[length] = value;
            _length       = length + 1;
        }
        else { GrowAndAppend(value); }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend( TValue value )
    {
        ThrowIfReadOnly();
        Grow(1);
        _array![_length++] = value;
    }


    /// <summary> Ensures room for <paramref name="additional"/> more elements after <see cref="Length"/>. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EnsureCapacityFor( int additional )
    {
        if ( (uint)( _length + additional ) > (uint)Capacity ) { Grow(additional); }
    }


    /// <summary> Grows to at least <see cref="Length"/> + <paramref name="additional"/>, doubling when that is larger, keeping the written elements. </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void Grow( int additional )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo(additional, 0);

        int       required = checked(_length + additional);
        int       capacity = Math.Max(required, (int)Math.Min((uint)Math.Max(Capacity, 4) * 2, (uint)Array.MaxLength));
        TValue[]  array    = ArrayPool<TValue>.Shared.Rent(capacity);
        TValue[]? previous = _array;

        if ( previous is not null )
        {
            previous.AsSpan(0, _length).CopyTo(array);
            ArrayPool<TValue>.Shared.Return(previous, RuntimeHelpers.IsReferenceOrContainsReferences<TValue>());
        }

        _array = array;
    }


    // ─── Trim ────────────────────────────────────────────────────────────────

    public void Trim( TValue value )
    {
        TrimEnd(value);
        TrimStart(value);
    }
    public void Trim( params ReadOnlySpan<TValue> value )
    {
        TrimEnd(value);
        TrimStart(value);
    }


    public void TrimStart( TValue value )
    {
        Span<TValue> span  = Values;
        int          start = 0;

        while ( start < span.Length && EqualityComparer<TValue>.Default.Equals(span[start], value) ) { start++; }

        RemoveStart(start);
    }
    public void TrimStart( params ReadOnlySpan<TValue> values )
    {
        Span<TValue> span  = Values;
        int          start = 0;

        while ( start < span.Length && IndexOf(values, span[start]) >= 0 ) { start++; }

        RemoveStart(start);
    }


    public void TrimEnd( TValue value )
    {
        ReadOnlySpan<TValue> span   = Values;
        int                  length = span.Length;

        while ( length > 0 && EqualityComparer<TValue>.Default.Equals(span[length - 1], value) ) { length--; }

        _length = length;
    }
    public void TrimEnd( params ReadOnlySpan<TValue> values )
    {
        ReadOnlySpan<TValue> span   = Values;
        int                  length = span.Length;

        while ( length > 0 && IndexOf(values, span[length - 1]) >= 0 ) { length--; }

        _length = length;
    }


    private void RemoveStart( int count )
    {
        if ( count <= 0 ) { return; }

        Span<TValue> span = Span;
        span[count.._length].CopyTo(span);
        _length -= count;
    }


    public readonly void Sort()                                                                   => Sort(Comparer<TValue>.Default);
    public readonly void Sort( Comparer<TValue>   comparer )                                      => Values.Sort(comparer);
    public readonly void Sort( Comparison<TValue> comparer )                                      => Values.Sort(comparer);
    public readonly void Sort( int                start, int length, IComparer<TValue> comparer ) => Values.Slice(start, length).Sort(comparer);


    public readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { ThrowReadOnly(); }
    }
    [DoesNotReturn] private static void ThrowReadOnly() => throw new InvalidOperationException($"Buffer<{typeof(TValue).Name}> is read only");


    public readonly ValueEnumerable<FromSpan<TValue>, TValue> AsValueEnumerable() => new(new FromSpan<TValue>(Values));
    public readonly ValueEnumerable<FromSpan<TValue>, TValue> AsValueEnumerable( int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, _length);
        Guard.IsInRange(endInclusive, 0, _length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);
        return new ValueEnumerable<FromSpan<TValue>, TValue>(new FromSpan<TValue>(Values.Slice(start, endInclusive - start + 1)));
    }


    public readonly ReadOnlySpan<TValue>.Enumerator GetEnumerator()
    {
        ReadOnlySpan<TValue> span = Values;
        return span.GetEnumerator();
    }


    void IBufferWriter<TValue>.Advance( int count )
    {
        Guard.IsInRange(count, 0, FreeCapacity + 1);
        _length += count;
    }
    Memory<TValue> IBufferWriter<TValue>.GetMemory( int sizeHint )
    {
        EnsureCapacityFor(Math.Max(sizeHint, 1));
        return FreeMemory;
    }
    Span<TValue> IBufferWriter<TValue>.GetSpan( int sizeHint )
    {
        EnsureCapacityFor(Math.Max(sizeHint, 1));
        return Next;
    }


    // ─── Equality search with vectorized paths for common primitive types ────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOf( ReadOnlySpan<TValue> span, TValue value )
    {
        if ( typeof(TValue) == typeof(char) ) { return As<char>(span).IndexOf(Unsafe.As<TValue, char>(ref value)); }

        if ( typeof(TValue) == typeof(byte) ) { return As<byte>(span).IndexOf(Unsafe.As<TValue, byte>(ref value)); }

        if ( typeof(TValue) == typeof(int) ) { return As<int>(span).IndexOf(Unsafe.As<TValue, int>(ref value)); }

        if ( typeof(TValue) == typeof(long) ) { return As<long>(span).IndexOf(Unsafe.As<TValue, long>(ref value)); }

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( EqualityComparer<TValue>.Default.Equals(span[i], value) ) { return i; }
        }

        return NOT_FOUND;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LastIndexOf( ReadOnlySpan<TValue> span, TValue value )
    {
        if ( typeof(TValue) == typeof(char) ) { return As<char>(span).LastIndexOf(Unsafe.As<TValue, char>(ref value)); }

        if ( typeof(TValue) == typeof(byte) ) { return As<byte>(span).LastIndexOf(Unsafe.As<TValue, byte>(ref value)); }

        if ( typeof(TValue) == typeof(int) ) { return As<int>(span).LastIndexOf(Unsafe.As<TValue, int>(ref value)); }

        if ( typeof(TValue) == typeof(long) ) { return As<long>(span).LastIndexOf(Unsafe.As<TValue, long>(ref value)); }

        for ( int i = span.Length - 1; i >= 0; i-- )
        {
            if ( EqualityComparer<TValue>.Default.Equals(span[i], value) ) { return i; }
        }

        return NOT_FOUND;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<TOther> As<TOther>( ReadOnlySpan<TValue> span ) => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<TValue, TOther>(ref MemoryMarshal.GetReference(span)), span.Length);
}
