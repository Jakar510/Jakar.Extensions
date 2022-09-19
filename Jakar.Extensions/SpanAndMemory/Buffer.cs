// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<T> where T : unmanaged, IEquatable<T>
{
    private T[]?    _arrayToReturnToPool = default;
    private Span<T> _span                = default;

    public          bool            IsEmpty    => Length == 0;
    public readonly int             Capacity   => _span.Length;
    internal        int             Index      { get; set; } = 0;
    public          bool            IsReadOnly { get; init; }
    public readonly Span<T>         Next       => _span[Index..];
    public readonly ReadOnlySpan<T> Span       => _span[..Index];
    public readonly Span<T>         RawSpan    => _span;


    public readonly Span<T> this[ in Range range ] => _span[range];
    public ref T this[ int index ]
    {
        get
        {
            Debug.Assert(index < Length);
            return ref _span[index];
        }
    }


    public int Length
    {
        readonly get => Index;
        set => Index = Math.Max(Math.Min(Capacity, value), 0);
    }


    public Buffer() : this(64, false) { }
    public Buffer( int initialCapacity, in bool isReadOnly )
    {
        _span      = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
        IsReadOnly = isReadOnly;
    }
    public void Dispose()
    {
        T[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return(toReturn); }
    }
    public readonly Enumerator GetEnumerator() => new(this);



    public ref struct Enumerator
    {
        private readonly Buffer<T> _buffer;
        private          int       _index = 0;
        public readonly  T         Current => _buffer[_index];


        internal Enumerator( in Buffer<T> buffer ) => _buffer = buffer;


        public void Reset() => _index = 0;
        public bool MoveNext() => ++_index < _buffer.Length;
    }



    private void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException($"{nameof(Buffer<T>)} is read only"); }
    }
    private static int GetLength( in ReadOnlySpan<T> span, in bool isReadOnly ) => span.Length * ( isReadOnly
                                                                                                       ? 1
                                                                                                       : 2 );


    [Pure] public static Buffer<T> Create( in ReadOnlySpan<T> span, in bool isReadOnly ) => new Buffer<T>().Init(span, isReadOnly);
    [Pure]
    public Buffer<T> Init( in ReadOnlySpan<T> span, in bool isReadOnly )
    {
        if ( _arrayToReturnToPool is not null ) { ArrayPool<T>.Shared.Return(_arrayToReturnToPool); }

        Index = 0;

        _span = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(GetLength(span, isReadOnly));

        span.CopyTo(_span);

        return this with
               {
                   IsReadOnly = isReadOnly
               };
    }
    public Buffer<T> Reset( in T value )
    {
        Index = 0;
        _span.Fill(value);
        return this;
    }


    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="Index"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    private void Grow( in int additionalCapacityBeyondPos )
    {
        ThrowIfReadOnly();
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(Index + additionalCapacityBeyondPos >= Capacity, "Grow called incorrectly, no resize is needed.");

        T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(Index + additionalCapacityBeyondPos, Capacity * 2));
        _span.CopyTo(poolArray);

        T[]? toReturn                = _arrayToReturnToPool;
        _span = _arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return(toReturn); }
    }
    private void GrowAndAppend( in T c )
    {
        Grow(1);
        Append(c);
    }
    public void EnsureCapacity( in int capacity )
    {
        if ( Index + capacity > Capacity ) { Grow(capacity); }
    }


    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null T after <see cref="Length"/>.
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (T* c = builder)"
    /// </summary>
    public ref T GetPinnableReference() => ref _span.GetPinnableReference();

    /// <summary> Get a pinnable reference to the builder. Ensures that the builder has a <paramref name="terminate"/> value after <see cref="Length"/> </summary>
    /// <param name="terminate"></param>
    public ref T GetPinnableReference( in T terminate )
    {
        EnsureCapacity(Length + 1);
        _span[++Length] = terminate;

        return ref GetPinnableReference();
    }


    public override string ToString() => $"{nameof(Buffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Index)}: {Index}, {nameof(IsReadOnly)}: {IsReadOnly} )";

    [Pure]
    public ReadOnlySpan<T> AsSpan( in T? terminate )
    {
        if ( terminate is null ) { return _span[..Index]; }

        EnsureCapacity(++Index);
        _span[Index] = terminate.Value;
        return Span;
    }
    [Pure] public readonly ReadOnlySpan<T> Slice( int start ) => _span.Slice(start,             Index - start);
    [Pure] public readonly ReadOnlySpan<T> Slice( int start, int length ) => _span.Slice(start, length);
    public int IndexOf( in     T                      value ) => Next.IndexOf(value);
    public int LastIndexOf( in T                      value, in int end ) => Next.LastIndexOf(value, end);


    public bool Contains( in T               value ) => _span.Contains(value);
    public bool Contains( in Span<T>         value ) => _span.Contains(value);
    public bool Contains( in ReadOnlySpan<T> value ) => _span.Contains(value);


    public bool TryCopyTo( in Span<T> destination, out int charsWritten )
    {
        if ( _span[..Index]
           .TryCopyTo(destination) )
        {
            charsWritten = Index;
            return true;
        }

        charsWritten = 0;
        return false;
    }


    public Buffer<T> Replace( in int index, in T value ) => Replace(index, value, 1);
    public Buffer<T> Replace( in int index, in T value, in int count )
    {
        ThrowIfReadOnly();
        if ( Index + count > _span.Length ) { Grow(count); }

        _span.Slice(index, count)
             .Fill(value);

        return this;
    }
    public Buffer<T> Replace( in int index, in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( Index + span.Length > _span.Length ) { Grow(span.Length); }

        span.CopyTo(_span.Slice(index, span.Length));
        return this;
    }


    public Buffer<T> Insert( in int index, in T value ) => Insert(index, value, 1);
    public Buffer<T> Insert( in int index, in T value, in int count )
    {
        ThrowIfReadOnly();
        if ( Index + count > _span.Length ) { Grow(count); }

        int remaining = Index - index;

        _span.Slice(index, remaining)
             .CopyTo(_span[( index + count )..]);

        _span.Slice(index, count)
             .Fill(value);

        Index += count;
        return this;
    }
    public Buffer<T> Insert( in int index, in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( Index + span.Length > _span.Length ) { Grow(span.Length); }

        int remaining = Index - index;

        _span.Slice(index, remaining)
             .CopyTo(_span[( index + span.Length )..]);

        span.CopyTo(_span[index..]);

        Index += span.Length;
        return this;
    }


    public Buffer<T> Append( in T value )
    {
        ThrowIfReadOnly();
        int pos = Index;

        if ( (uint)pos < (uint)_span.Length )
        {
            _span[pos] = value;
            Index      = pos + 1;
        }
        else { GrowAndAppend(value); }

        return this;
    }
    public Buffer<T> Append( in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();

        switch ( span.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when Index + 1 < Capacity:
            {
                _span[Index++] = span[0];
                return this;
            }

            case 2 when Index + 2 < Capacity:
            {
                _span[Index++] = span[0];
                _span[Index++] = span[1];
                return this;
            }

            default:
            {
                if ( Index + span.Length >= Capacity ) { Grow(span.Length); }

                span.CopyTo(_span[Index..]);
                Index += span.Length;
                return this;
            }
        }
    }


    public Buffer<T> Append( in T c, in int count )
    {
        ThrowIfReadOnly();
        if ( Index + count >= Capacity ) { Grow(count); }

        for ( int i = Index; i < Index + count; i++ ) { _span[i] = c; }

        Index += count;
        return this;
    }


    // public Span<T> AppendSpan( int length )
    // {
    //     int origPos = Index;
    //     if ( origPos > Capacity - length ) { Grow(length); }
    //
    //     Index = origPos + length;
    //     return _span.Slice(origPos, length);
    // }


    // public unsafe void Append( T* value, int length )
    // {
    //     int pos = _pos;
    //     if ( pos > _chars.Length - length ) { Grow(length); }
    //
    //     Span<T> dst = _chars.Slice(_pos, length);
    //     for ( var i = 0; i < dst.Length; i++ ) { dst[i] = *value++; }
    //
    //     _pos += length;
    // }
}
