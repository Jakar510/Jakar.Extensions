// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<T> where T : IEquatable<T>
{
    private T[]?    _arrayToReturnToPool = default;
    private Span<T> _span                = default;
    private int     _index               = 0;


    public readonly bool    IsEmpty    => Length == 0;
    public readonly bool    IsNotEmpty => Length > 0;
    public readonly int     Capacity   => _span.Length;
    public readonly int     Length     => _index;
    public readonly Span<T> Next       => _span[_index..];
    public readonly Span<T> Span       => _span[.._index];
    public          bool    IsReadOnly { get; init; }
    public int Index
    {
        readonly get => _index;
        set => _index = Math.Max( Math.Min( Capacity, value ), 0 );
    }


    public readonly Span<T> this[ Range range ] => _span[range];
    public ref T this[ int index ]
    {
        get
        {
            Debug.Assert( index < Index );
            return ref _span[index];
        }
    }


    public Buffer() : this( 64 ) { }
    public Buffer( int initialCapacity )
    {
        // var sb = new StringBuilder( "", 0, 0, initialCapacity );
        // GC.AllocateUninitializedArray<T>( initialCapacity );

        _span      = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );
        IsReadOnly = false;
    }
    public Buffer( ReadOnlySpan<T> span ) : this( span.Length ) => Append( span );
    public void Dispose()
    {
        T[]? toReturn = _arrayToReturnToPool;
        this = default; // For safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return( toReturn ); }
    }


    public readonly override string ToString() => $"{nameof(Buffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(_index)}: {_index}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    public readonly Enumerator GetEnumerator() => new(this);


    private readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(Buffer<T>)} is read only" ); }
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalCapacityBeyondPos"/> to <see cref="_index"/> whichever is greater. </summary>
    /// <param name="additionalCapacityBeyondPos"> Number of chars requested beyond current position. </param>
    private void Grow( int additionalCapacityBeyondPos )
    {
        ThrowIfReadOnly();
        Debug.Assert( additionalCapacityBeyondPos > 0 );
        Debug.Assert( _index + additionalCapacityBeyondPos >= Capacity, "Grow called incorrectly, no resize is needed." );

        T[] poolArray = ArrayPool<T>.Shared.Rent( Math.Max( _index + additionalCapacityBeyondPos, Capacity * 2 ) );
        _span.CopyTo( poolArray );

        T[]? toReturn                = _arrayToReturnToPool;
        _span = _arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return( toReturn ); }
    }
    private void GrowAndAppend( T c )
    {
        Grow( 1 );
        Append( c );
    }
    public void EnsureCapacity( int capacity )
    {
        if ( _index + capacity > Capacity ) { Grow( capacity ); }
    }


    /// <summary> Get a pinnable reference to the builder. Does not ensure there is a null T after <see cref="Index"/> . This overload is pattern matched in the C# 7.3+ compiler so you can omit the explicit method call, and write eg "fixed (T* c = builder)" </summary>
    public readonly ref T GetPinnableReference() => ref _span.GetPinnableReference();

    /// <summary> Get a pinnable reference to the builder. Ensures that the builder has a <paramref name="terminate"/> value after <see cref="Index"/> </summary>
    /// <param name="terminate"> </param>
    public ref T GetPinnableReference( T terminate )
    {
        Append( terminate );
        return ref GetPinnableReference();
    }


    public Buffer<T> Clear()
    {
        _index = 0;
        _span.Clear();
        return this;
    }
    public Buffer<T> Reset( T value )
    {
        _index = 0;
        _span.Fill( value );
        return this;
    }


    [Pure]
    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return _span[.._index]; }

        EnsureCapacity( ++_index );
        _span[_index] = terminate;
        return Span;
    }
    [Pure] public readonly ReadOnlySpan<T> Slice( int start ) => _span.Slice( start,             _index - start );
    [Pure] public readonly ReadOnlySpan<T> Slice( int start, int length ) => _span.Slice( start, length );
    public readonly int IndexOf( T                    value ) => Next.IndexOf( value );
    public readonly int LastIndexOf( T                value, int end ) => Next.LastIndexOf( value, end );


    public readonly bool Contains( T               value ) => _span.Contains( value );
    public readonly bool Contains( Span<T>         value ) => _span.Contains( value );
    public readonly bool Contains( ReadOnlySpan<T> value ) => _span.Contains( value );


    public readonly bool TryCopyTo( Span<T> destination, out int charsWritten )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            charsWritten = _index;
            return true;
        }

        charsWritten = 0;
        return false;
    }


    public Buffer<T> Replace( int index, T value ) => Replace( index, value, 1 );
    public Buffer<T> Replace( int index, T value, int count )
    {
        ThrowIfReadOnly();
        if ( _index + count > _span.Length ) { Grow( count ); }

        _span.Slice( index, count )
             .Fill( value );

        return this;
    }
    public Buffer<T> Replace( int index, ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( _index + span.Length > _span.Length ) { Grow( span.Length ); }

        span.CopyTo( _span.Slice( index, span.Length ) );
        return this;
    }


    public Buffer<T> Insert( int index, T value ) => Insert( index, value, 1 );
    public Buffer<T> Insert( int index, T value, int count )
    {
        ThrowIfReadOnly();
        if ( _index + count > _span.Length ) { Grow( count ); }

        int remaining = _index - index;

        _span.Slice( index, remaining )
             .CopyTo( _span[(index + count)..] );

        _span.Slice( index, count )
             .Fill( value );

        _index += count;
        return this;
    }
    public Buffer<T> Insert( int index, ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( _index + span.Length > _span.Length ) { Grow( span.Length ); }

        int remaining = _index - index;

        _span.Slice( index, remaining )
             .CopyTo( _span[(index + span.Length)..] );

        span.CopyTo( _span[index..] );

        _index += span.Length;
        return this;
    }


    public Buffer<T> Append( T value )
    {
        ThrowIfReadOnly();

        if ( (uint)_index < (uint)_span.Length ) { _span[_index++] = value; }
        else { GrowAndAppend( value ); }

        return this;
    }
    public Buffer<T> Append( ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();

        switch ( span.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when _index + 1 < Capacity:
            {
                _span[_index++] = span[0];
                return this;
            }

            case 2 when _index + 2 < Capacity:
            {
                _span[_index++] = span[0];
                _span[_index++] = span[1];
                return this;
            }

            default:
            {
                if ( _index + span.Length >= Capacity ) { Grow( span.Length ); }

                span.CopyTo( Next );
                _index += span.Length;
                return this;
            }
        }
    }
    public Buffer<T> Append( T c, int count )
    {
        ThrowIfReadOnly();
        if ( _index + count >= Capacity ) { Grow( count ); }

        for ( int i = _index; i < _index + count; i++ ) { _span[i] = c; }

        _index += count;
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



    public ref struct Enumerator
    {
        private readonly    Buffer<T> _buffer;
        private             int       _index = 0;
        public readonly ref T         Current => ref _buffer[_index];

        internal Enumerator( Buffer<T> buffer ) => _buffer = buffer;

        public void Reset() => _index = 0;
        public bool MoveNext() => ++_index < _buffer._index;
    }
}
