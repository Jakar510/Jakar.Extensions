// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<T>
{
    private T[]?                  _arrayToReturnToPool = default;
    private Span<T>               _span                = default;
    private int                   _index               = 0;
    private IEqualityComparer<T>? _comparer;

    public readonly bool    IsEmpty    => Length == 0;
    public readonly bool    IsNotEmpty => Length > 0;
    public readonly int     Capacity   => _span.Length;
    public readonly int     Length     => _index;
    public readonly Span<T> Next       => _span[_index..];
    public readonly Span<T> Span       => _span[.._index];
    public          bool    IsReadOnly { get; init; } = false;
    public int Index
    {
        readonly get => _index;
        set => _index = Math.Max( Math.Min( Capacity, value ), 0 );
    }


    public ref T this[ int              index ] => ref _span[index];
    public ref T this[ Index            index ] => ref _span[index];
    public readonly Span<T> this[ Range range ] => _span[range];
    public readonly Span<T> this[ int   start, int length ] => _span.Slice( start, length );


    public Buffer() : this( 64 ) { }
    public Buffer( int initialCapacity ) : this( initialCapacity, EqualityComparer<T>.Default ) { }
    public Buffer( int initialCapacity, IEqualityComparer<T> comparer )
    {
        _comparer = comparer;
        _span     = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );
    }
    public Buffer( ReadOnlySpan<T> span ) : this( span, EqualityComparer<T>.Default ) => Append( span );
    public Buffer( ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( span.Length, comparer ) => Append( span );


    public void Dispose()
    {
        T[]? toReturn = _arrayToReturnToPool;
        _comparer = default;
        this      = default; // For safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return( toReturn ); }
    }


    public readonly override string     ToString()      => $"{nameof(Buffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(_index)}: {_index}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    public readonly          Enumerator GetEnumerator() => new(this);


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    private readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(Buffer<T>)} is read only" ); }
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalCapacityBeyondPos"/> to <see cref="_index"/> whichever is greater. </summary>
    /// <param name="additionalCapacityBeyondPos"> Number of chars requested beyond current position. </param>
    private void Grow( in int additionalCapacityBeyondPos )
    {
        ThrowIfReadOnly();
        Debug.Assert( additionalCapacityBeyondPos          > 0 );
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
    public void EnsureCapacity( in int capacity )
    {
        if ( _index + capacity > Capacity ) { Grow( capacity ); }
    }


    public readonly ref T GetPinnableReference() => ref _span.GetPinnableReference();
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


    [ Pure ]
    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return _span[.._index]; }

        EnsureCapacity( ++_index );
        _span[_index] = terminate;
        return Span;
    }
    [ Pure ] public readonly ReadOnlySpan<T> Slice( int start )             => Span[start..];
    [ Pure ] public readonly ReadOnlySpan<T> Slice( int start, int length ) => Span.Slice( start, length );
    public readonly int IndexOf( T value )
    {
        Debug.Assert( _comparer is not null );

        for ( int i = 0; i < _span.Length; i++ )
        {
            if ( _comparer.Equals( _span[i], value ) ) { return i; }
        }

        return -1;
    }
    public readonly int LastIndexOf( T value, int end = 0 )
    {
        Debug.Assert( _comparer is not null );

        for ( int i = Length; i < end; i-- )
        {
            if ( _comparer.Equals( _span[i], value ) ) { return i; }
        }

        return -1;
    }


    public readonly bool Contains( T value )
    {
        Debug.Assert( _comparer is not null );

        foreach ( T x in _span )
        {
            if ( _comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }
    public readonly bool Contains( ReadOnlySpan<T> value )
    {
        Debug.Assert( _comparer is not null );
        if ( value.Length > _span.Length ) { return false; }
    #if NET6_0_OR_GREATER
        if ( value.Length == _span.Length ) { return _span.SequenceEqual( value, _comparer ); }
    #endif

        for ( int i = 0; i < _span.Length || i + value.Length < _span.Length; i++ )
        {
            ReadOnlySpan<T> span = _span.Slice( i, value.Length );

        #if NET6_0_OR_GREATER
            if ( span.SequenceEqual( value, _comparer ) ) { return true; }
        #else
            for ( int j = 0; j < span.Length; j++ )
            {
                if ( _comparer.Equals( span[j], value[j] ) ) { return true; }
            }

        #endif
        }

        return false;
    }


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


    public Buffer<T> Replace( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        if ( _index + count > _span.Length ) { Grow( count ); }

        _span.Slice( index, count ).Fill( value );

        return this;
    }
    public Buffer<T> Replace( int index, ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( _index + span.Length > _span.Length ) { Grow( span.Length ); }

        span.CopyTo( _span.Slice( index, span.Length ) );
        return this;
    }


    public Buffer<T> Insert( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        if ( _index + count > _span.Length ) { Grow( count ); }

        int remaining = _index - index;

        _span.Slice( index, remaining ).CopyTo( _span[(index + count)..] );

        _span.Slice( index, count ).Fill( value );

        _index += count;
        return this;
    }
    public Buffer<T> Insert( int index, ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( _index + span.Length > _span.Length ) { Grow( span.Length ); }

        int remaining = _index - index;

        _span.Slice( index, remaining ).CopyTo( _span[(index + span.Length)..] );

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



    public ref struct Enumerator
    {
        private readonly    Buffer<T> _buffer;
        private             int       _index = 0;
        public readonly ref T         Current => ref _buffer[_index];

        internal Enumerator( Buffer<T> buffer ) => _buffer = buffer;

        public void Reset()    => _index = 0;
        public bool MoveNext() => ++_index < _buffer._index;
    }
}
