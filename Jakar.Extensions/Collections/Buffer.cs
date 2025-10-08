﻿// Jakar.Extensions :: Jakar.Extensions
// 09/21/2024  10:09


using ZLinq;
using ZLinq.Internal;
using ZLinq.Linq;



namespace Jakar.Extensions;


[StructLayout(LayoutKind.Auto)]
[method: MustDisposeResource]
public ref struct Buffer<TValue>( int capacity ) : IMemoryOwner<TValue>
    where TValue : IEquatable<TValue>
{
    private static readonly TValue[] _empty = [];
    private readonly        TValue[] _array = ArrayPool<TValue>.Shared.Rent(Math.Max(capacity, DEFAULT_CAPACITY));
    private                 int      _length;


    public readonly int  Capacity     => _array.Length;
    public readonly int  FreeCapacity => Capacity - (int)Length;
    public          int  Length       { [Pure] readonly get => _length; set => _length = Math.Clamp(value, 0, Capacity); }
    public readonly bool IsEmpty      { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Length == 0; }
    public readonly bool IsNotEmpty   { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Length > 0; }
    public          bool IsReadOnly   { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get; init; } = false;
    public readonly ref TValue this[ int     index ] { [Pure] get => ref Values[index]; }
    public readonly ref TValue this[ Index   index ] { [Pure] get => ref Values[index]; }
    public readonly Span<TValue> this[ Range range ] { [Pure] get => Values[range]; }
    public readonly Span<TValue> this[ int   start, int length ] { [Pure] get => Values.Slice(start, length); }
    public readonly Memory<TValue> Memory { [Pure] get => _array; }
    public readonly Span<TValue>   Next   { [Pure] get => Span[Length..]; }
    public readonly Span<TValue>   Span   { [Pure] get => _array; }
    public readonly Span<TValue>   Values { [Pure] get => new(_array, 0, Length); }


    [MustDisposeResource] public Buffer() : this(DEFAULT_CAPACITY) { }
    [MustDisposeResource] public Buffer( params ReadOnlySpan<TValue> span ) : this(span.Length) => span.CopyTo(Span);
    public void Dispose() { ArrayPool<TValue>.Shared.Return(_array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()); }


    public TValue[] ToArray()
    {
        TValue[] array = Values.ToArray();
        Dispose();
        return array;
    }
    public void Clear()
    {
        Span.Clear();
        Length = 0;
    }
    public void Fill( TValue value ) => Span.Fill(value);


    public void CopyTo( Span<TValue> array ) => Values.CopyTo(array);
    public bool TryCopyTo( Span<TValue> destination, out int length )
    {
        if ( Values.TryCopyTo(destination) )
        {
            length = Length;
            return true;
        }

        length = 0;
        return false;
    }


    public void Reverse( int start, int length ) => Values.Slice(start, length)
                                                          .Reverse();
    public void Reverse() => Values.Reverse();


    public ReadOnlySpan<TValue> AsSpan( TValue? terminate )
    {
        if ( terminate is not null ) { Add(terminate); }

        return Values;
    }
    public readonly Span<TValue> Slice( int start )             => Slice(start, Capacity - start);
    public readonly Span<TValue> Slice( int start, int length ) => Span.Slice(start, length);


    public readonly int IndexOf( TValue value )            => IndexOf(value, 0);
    public readonly int IndexOf( TValue value, int start ) => IndexOf(value, start, Length - 1);
    public readonly int IndexOf( TValue value, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( value.Equals(span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }
    public readonly int FindIndex( Func<TValue, bool> match )            => FindIndex(match, 0);
    public readonly int FindIndex( Func<TValue, bool> match, int start ) => FindIndex(match, start, Length - 1);
    public readonly int FindIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match(span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }
    public readonly int LastIndexOf( TValue value )                   => LastIndexOf(value, 0);
    public readonly int LastIndexOf( TValue value, int endInclusive ) => LastIndexOf(value, Length - 1, endInclusive);
    public readonly int LastIndexOf( TValue value, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(start, endInclusive);
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( value.Equals(span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }


    public readonly int FindLastIndex( Func<TValue, bool> match )                   => FindLastIndex(match, 0);
    public readonly int FindLastIndex( Func<TValue, bool> match, int endInclusive ) => FindLastIndex(match, Length - 1, endInclusive);
    public readonly int FindLastIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(start, endInclusive);

        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match(span[i]) ) { return i; }
        }

        return NOT_FOUND;
    }
    public readonly TValue? FindLast( Func<TValue, bool> match )                   => FindLast(match, 0);
    public readonly TValue? FindLast( Func<TValue, bool> match, int endInclusive ) => FindLast(match, Length - 1, endInclusive);
    public readonly TValue? FindLast( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(start, endInclusive);

        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match(span[i]) ) { return span[i]; }
        }

        return default;
    }
    public readonly TValue? Find( Func<TValue, bool> match )            => Find(match, 0);
    public readonly TValue? Find( Func<TValue, bool> match, int start ) => Find(match, start, Length - 1);
    public readonly TValue? Find( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);

        Span<TValue> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match(span[i]) ) { return span[i]; }
        }

        return default;
    }
    [Pure] [MustDisposeResource] public readonly FilterBuffer<TValue> FindAll( Func<TValue, bool> match )            => FindAll(match, 0);
    [Pure] [MustDisposeResource] public readonly FilterBuffer<TValue> FindAll( Func<TValue, bool> match, int start ) => FindAll(match, start, Length - 1);
    [Pure] [MustDisposeResource] public readonly FilterBuffer<TValue> FindAll( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange(start,        0, Length);
        Guard.IsInRange(endInclusive, 0, Length);
        Guard.IsGreaterThanOrEqualTo(endInclusive, start);
        FilterBuffer<TValue> buffer = new(Length);
        ReadOnlySpan<TValue> span   = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match(span[i]) ) { buffer.Add(in span[i]); }
        }

        return buffer;
    }


    public readonly bool Contains( TValue value )
    {
        ReadOnlySpan<TValue> values = Values;
        return values.Contains(value);
    }
    public readonly bool Contains( params ReadOnlySpan<TValue> value )
    {
        ReadOnlySpan<TValue> values = Values;
        return values.Contains(in value, EqualityComparer<TValue>.Default);
    }


    public bool RemoveAt( int index )
    {
        if ( index < 0 || index >= Length ) { return false; }

        Span<TValue> span      = Span;
        int          moveCount = _length - index - 1;
        if ( moveCount <= 0 ) { return false; }

        span.Slice(index + 1, moveCount)
            .CopyTo(span.Slice(index));

        Length--;
        return true;
    }
    public bool Remove( TValue value ) => RemoveAt(IndexOf(value, 0));


    public void Replace( int start, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo(count, 0);
        Guard.IsInRange(start + count, 0, Capacity);

        Values.Slice(start, count)
              .Fill(value);
    }
    public void Replace( int start, params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange(start,                 0, Length);
        Guard.IsInRange(start + values.Length, 0, Capacity);

        values.CopyTo(Span.Slice(start, values.Length));
    }


    public void Insert( int start, TValue value, int count = 1 )
    {
        Guard.IsGreaterThanOrEqualTo(count, 0);
        using IMemoryOwner<TValue> owner = MemoryPool<TValue>.Shared.Rent(count);
        owner.Memory.Span.Fill(value);
        Insert(start, owner.Memory.Span);
    }
    public void Insert( int start, params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange(start,                 0, Length);
        Guard.IsInRange(start + values.Length, 0, Capacity);
        if ( values.IsEmpty ) { return; }

        int index  = start    - values.Length;
        int length = Capacity - index;

        Span.Slice(start, length)
            .CopyTo(Span[index..]);

        values.CopyTo(Span.Slice(start, values.Length));
        Length += values.Length;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Add( TValue value ) => Span[Length++] = value;
    public void Add( TValue value, int count )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo(count, 0);
        Guard.IsInRange(Length + count, 0, Capacity);
        Span<TValue> span = Next;

        for ( int i = 0; i < count; i++ ) { span[i] = value; }

        Length += count;
    }
    public void Add( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Span<TValue> next = Next;

        switch ( values.Length )
        {
            case 0: { return; }

            case > 0 when next.Length >= values.Length:
            {
                values.CopyTo(next);
                Length += values.Length;
                return;
            }

            default:
            {
                EnsureCapacity(values.Length + Length);
                values.CopyTo(Next);
                Length += values.Length;
                return;
            }
        }
    }
    public void AddRange( IEnumerable<TValue> enumerable )
    {
        ThrowIfReadOnly();

        switch ( enumerable )
        {
            case TValue[] array:
            {
                int count = array.Length;
                if ( count <= 0 ) { return; }

                Guard.IsInRange(array.Length + Length, 0, Capacity);
                new ReadOnlySpan<TValue>(array).CopyTo(Next);
                Length += count;
                return;
            }

            case List<TValue> list:
            {
                int count = list.Count;
                if ( count <= 0 ) { return; }

                Guard.IsInRange(list.Count + Length, 0, Capacity);

                CollectionsMarshal.AsSpan(list)
                                  .CopyTo(Next);

                Length += count;
                return;
            }

            case ICollection<TValue> collection:
            {
                int count = collection.Count;
                if ( count <= 0 ) { return; }

                Guard.IsInRange(collection.Count + Length, 0, Capacity);
                TValue[] array = ArrayPool<TValue>.Shared.Rent(count);

                try
                {
                    collection.CopyTo(array, Length);
                    array.CopyTo(Next);
                    Length += count;
                }
                finally { ArrayPool<TValue>.Shared.Return(array); }

                return;
            }

            default:
            {
                foreach ( TValue value in enumerable ) { Add(value); }

                return;
            }
        }
    }


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
        Span<TValue> span  = Span;
        int          start = 0;

        // Skip matching values at the beginning
        while ( start < _length && EqualityComparer<TValue>.Default.Equals(span[start], value) ) { start++; }

        if ( start <= 0 ) { return; }

        span[start.._length]
           .CopyTo(span);

        Length = _length - start;
    }
    public void TrimStart( params ReadOnlySpan<TValue> values )
    {
        Span<TValue> span  = Span;
        int          start = 0;

        while ( start < _length && values.Contains(span[start], EqualityComparer<TValue>.Default) ) { start++; }

        if ( start <= 0 ) { return; }

        span[start.._length]
           .CopyTo(span);

        Length = _length - start;
    }


    public void TrimEnd( TValue value )
    {
        Span<TValue> span   = Span;
        int          length = _length - 1;

        while ( length >= 0 && EqualityComparer<TValue>.Default.Equals(span[length], value) ) { length--; }

        Length = length;
    }
    public void TrimEnd( params ReadOnlySpan<TValue> values )
    {
        Span<TValue> span   = Span;
        int          length = _length - 1;

        while ( length >= 0 && values.Contains(span[length]) ) { length--; }

        Length = length;
    }


    public readonly void Sort()                              => Sort(Comparer<TValue>.Default);
    public readonly void Sort( Comparer<TValue>   comparer ) => Span.Sort(comparer);
    public readonly void Sort( Comparison<TValue> comparer ) => Span.Sort(comparer);
    public readonly void Sort( int start, int length, IComparer<TValue> comparer ) => Span.Slice(start, length)
                                                                                          .Sort(comparer);


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalRequestedCapacity"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalRequestedCapacity"> the requested new size of the buffer. </param>
    public Buffer<TValue> Grow( uint additionalRequestedCapacity )
    {
        ThrowIfReadOnly();
        Guard.IsInRange(additionalRequestedCapacity, 1, int.MaxValue);
        int capacity = GetLength((uint)Capacity, additionalRequestedCapacity);

        // ReSharper disable once NotDisposedResource
        Buffer<TValue> buffer = new(capacity);
        Values.CopyTo(buffer.Span);
        Dispose();
        this = buffer;
        return this;
    }
    public void EnsureCapacity( int min )
    {
        if ( min <= Capacity ) { return; }

        Grow((uint)( min - Capacity ));
    }


    public readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException($"Buffer<{typeof(TValue).Name}> is read only"); }
    }


    public ValueEnumerable<FromSpan<TValue>, TValue> AsValueEnumerable() => new(new FromSpan<TValue>(Values));
    public readonly ReadOnlySpan<TValue>.Enumerator GetEnumerator()
    {
        ReadOnlySpan<TValue> span = Values;
        return span.GetEnumerator();
    }
}
