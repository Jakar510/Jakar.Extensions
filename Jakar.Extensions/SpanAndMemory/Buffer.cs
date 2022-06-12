// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

using System.Buffers;



namespace Jakar.Extensions.SpanAndMemory;


public ref struct Buffer<T> where T : struct, IEquatable<T>
{
    private T[]?    _arrayToReturnToPool;
    private Span<T> _span;
    private int     _pos;


    public int Length
    {
        readonly get => _pos;
        set
        {
            Debug.Assert(value >= 0,            "Value must be zero or greater");
            Debug.Assert(value <= _span.Length, $"Value must be less than '{_span.Length}'");
            _pos = value;
        }
    }

    public readonly int Capacity => _span.Length;

    public ref T this[ int index ]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _span[index];
        }
    }


    public Buffer() : this(64) { }
    public Buffer( int initialCapacity )
    {
        _arrayToReturnToPool = ArrayPool<T>.Shared.Rent(initialCapacity);
        _span                = _arrayToReturnToPool;
        _pos                 = 0;
    }
    public Buffer( in Span<T> initialBuffer )
    {
        _arrayToReturnToPool = null;
        _span                = initialBuffer;
        _pos                 = 0;
    }
    public void Dispose()
    {
        T[]? toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return(toReturn); }
    }


    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow( int additionalCapacityBeyondPos )
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _span.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        T[] poolArray = ArrayPool<T>.Shared.Rent(Math.Max(_pos + additionalCapacityBeyondPos, _span.Length * 2));

        _span.CopyTo(poolArray);
        T[]? toReturn                = _arrayToReturnToPool;
        _span = _arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return(toReturn); }
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend( T c )
    {
        Grow(1);
        Append(c);
    }
    public void EnsureCapacity( int capacity )
    {
        if ( capacity > _span.Length ) { Grow(capacity - _pos); }
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

        EnsureCapacity(_pos + 1);
        _span[_pos + 1] = terminate.Value;

        return ref GetPinnableReference();
    }


    public override string ToString() => AsSpan().ToString();


    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<T> RawChars => _span;


    [Pure] public readonly ReadOnlySpan<T> AsSpan() => _span[.._pos];
    [Pure]
    public ReadOnlySpan<T> AsSpan( in T? terminate )
    {
        if ( !terminate.HasValue ) { return _span[.._pos]; }

        EnsureCapacity(_pos + 1);
        _span[_pos + 1] = terminate.Value;
        return _span[.._pos];
    }
    [Pure] public readonly ReadOnlySpan<T> AsSpan( int start ) => _span.Slice(start,             _pos - start);
    [Pure] public readonly ReadOnlySpan<T> AsSpan( int start, int length ) => _span.Slice(start, length);


    public void Contains( T                  value ) => _span.Contains(value);
    public void Contains( in Span<T>         value ) => _span.Contains(value);
    public void Contains( in ReadOnlySpan<T> value ) => _span.Contains(value);


    public bool TryCopyTo( in Span<T> destination, out int charsWritten )
    {
        if ( _span[.._pos].TryCopyTo(destination) )
        {
            charsWritten = _pos;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    public void Insert( int index, T value, int count )
    {
        if ( _pos > _span.Length - count ) { Grow(count); }

        int remaining = _pos - index;
        _span.Slice(index, remaining).CopyTo(_span[( index + count )..]);
        _span.Slice(index, count).Fill(value);
        _pos += count;
    }
    public void Insert( int index, ReadOnlySpan<T> s )
    {
        int count = s.Length;

        if ( _pos > _span.Length - count ) { Grow(count); }

        int remaining = _pos - index;
        _span.Slice(index, remaining).CopyTo(_span[( index + count )..]);
        s.AsSpan().CopyTo(_span[index..]);
        _pos += count;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append( T c )
    {
        int pos = _pos;

        if ( (uint)pos < (uint)_span.Length )
        {
            _span[pos] = c;
            _pos       = pos + 1;
        }
        else { GrowAndAppend(c); }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append( ReadOnlySpan<T> s )
    {
        int pos = _pos;

        switch ( s.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when (uint)pos < (uint)_span.Length:
            {
                _span[pos] =  s[0];
                _pos       += 1;
                return;
            }

            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 2 when (uint)pos < (uint)_span.Length:
            {
                _span[pos] =  s[0];
                _pos       += 1;
                _span[pos] =  s[1];
                _pos       += 1;
                return;
            }

            default:
            {
                AppendSlow(s);
                return;
            }
        }
    }

    private void AppendSlow( ReadOnlySpan<T> s )
    {
        int pos = _pos;
        if ( pos > _span.Length - s.Length ) { Grow(s.Length); }

        s.CopyTo(_span[pos..]);
        _pos += s.Length;
    }

    public void Append( T c, int count )
    {
        if ( _pos > _span.Length - count ) { Grow(count); }

        Span<T> dst = _span.Slice(_pos, count);
        for ( var i = 0; i < dst.Length; i++ ) { dst[i] = c; }

        _pos += count;
    }


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

    public void Append( in ReadOnlySpan<T> value )
    {
        int pos = _pos;
        if ( pos > _span.Length - value.Length ) { Grow(value.Length); }

        value.CopyTo(_span[_pos..]);
        _pos += value.Length;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AppendSpan( int length )
    {
        int origPos = _pos;
        if ( origPos > _span.Length - length ) { Grow(length); }

        _pos = origPos + length;
        return _span.Slice(origPos, length);
    }
}
