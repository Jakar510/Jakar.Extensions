﻿// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<T> where T : unmanaged, IEquatable<T>
{
    private T[]?    _arrayToReturnToPool = default;
    private Span<T> _span                = default;

    public          bool    IsEmpty  => Length == 0;
    public readonly int     Capacity => _span.Length;
    public          int     Index    { get; set; } = 0;
    public          Span<T> Next     => _span[Index..];


    /// <summary> Returns the underlying storage of the builder. </summary>
    public Span<T> RawChars => _span;

    public Span<T> this[ Range range ] => _span[range];
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


    public Buffer() : this(64) { }
    public Buffer( int                initialCapacity ) => _span = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
    public Buffer( in ReadOnlySpan<T> buffer ) : this(buffer.Length * 2) => Append(buffer);
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



    public Buffer<T> Reset( in T value )
    {
        Index = 0;
        _span.Fill(value);
        return this;
    }
    public Buffer<T> Init( in ReadOnlySpan<T> value )
    {
        if ( _arrayToReturnToPool is not null ) { ArrayPool<T>.Shared.Return(_arrayToReturnToPool); }

        Index = 0;
        _span = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(value.Length * 2);
        value.CopyTo(_span);
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
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(Index > _span.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(Index + additionalCapacityBeyondPos, _span.Length * 2));

        _span.CopyTo(poolArray);

        T[]? toReturn                = _arrayToReturnToPool;
        _span = _arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return(toReturn); }
    }
    private void GrowAndAppend( T c )
    {
        Grow(1);
        Append(c);
    }
    public void EnsureCapacity( int capacity )
    {
        if ( capacity > _span.Length ) { Grow(capacity - Index); }
    }


    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null T after <see cref="Length"/>.
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (T* c = builder)"
    /// </summary>
    public ref T GetPinnableReference() => ref _span.GetPinnableReference();

    /// <summary> Get a pinnable reference to the builder. </summary>
    /// <param name="terminate">Ensures that the builder has a null T after <see cref="Length"/></param>
    public ref T GetPinnableReference( in T? terminate )
    {
        if ( !terminate.HasValue ) { return ref GetPinnableReference(); }

        EnsureCapacity(Index + 1);
        _span[Index + 1] = terminate.Value;

        return ref GetPinnableReference();
    }


    public override string ToString() => AsSpan()
       .ToString();


    [Pure] public readonly ReadOnlySpan<T> AsSpan() => _span[..Index];
    [Pure]
    public ReadOnlySpan<T> AsSpan( in T? terminate )
    {
        if ( terminate is null ) { return _span[..Index]; }

        EnsureCapacity(Index + 1);
        _span[Index + 1] = terminate.Value;
        return _span[..Index];
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
        if ( Index + count > _span.Length ) { Grow(count); }

        _span.Slice(index, count)
             .Fill(value);

        return this;
    }
    public Buffer<T> Replace( in int index, in ReadOnlySpan<T> span )
    {
        if ( Index + span.Length > _span.Length ) { Grow(span.Length); }

        span.CopyTo(_span.Slice(index, span.Length));
        return this;
    }


    public Buffer<T> Insert( in int index, in T value ) => Insert(index, value, 1);
    public Buffer<T> Insert( in int index, in T value, in int count )
    {
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

            default: { return AppendSlow(span); }
        }
    }

    private Buffer<T> AppendSlow( in ReadOnlySpan<T> span )
    {
        if ( Index + span.Length >= Capacity ) { Grow(span.Length); }

        span.CopyTo(_span[Index..]);
        Index += span.Length;
        return this;
    }

    public Buffer<T> Append( in T c, in int count )
    {
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
