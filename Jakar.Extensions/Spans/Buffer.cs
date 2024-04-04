// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<TValue>
{
    private          TValue[]                   _arrayToReturnToPool;
    internal         Span<TValue>               buffer = default;
    private readonly IEqualityComparer<TValue>? _comparer;
    private          int                        _length;


    public readonly bool         IsEmpty    { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public readonly bool         IsNotEmpty { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public readonly int          Capacity   { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer.Length; }
    public          int          Length     { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => _length; set => _length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public readonly Span<TValue> Next       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[Length..]; }
    public readonly Span<TValue> Span       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[..Length]; }
    public          bool         IsReadOnly { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get;                    init; } = false;
    public          int          Index      { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => Length; set => Length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public readonly ref TValue this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly ref TValue this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly Span<TValue> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => buffer[range]; }
    public readonly Span<TValue> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Slice( start, length ); }


    public Buffer() : this( DEFAULT_CAPACITY ) { }
    public Buffer( int initialCapacity ) : this( initialCapacity, EqualityComparer<TValue>.Default ) { }
    public Buffer( int initialCapacity, IEqualityComparer<TValue> comparer )
    {
        _comparer = comparer;
        buffer    = _arrayToReturnToPool = ArrayPool<TValue>.Shared.Rent( initialCapacity );
    }
    public Buffer( scoped in ReadOnlySpan<TValue> span ) : this( span, EqualityComparer<TValue>.Default ) => Append( span );
    public Buffer( scoped in ReadOnlySpan<TValue> span, IEqualityComparer<TValue> comparer ) : this( span.Length, comparer ) => Append( span );


    public void Dispose()
    {
        TValue[]? toReturn = _arrayToReturnToPool;
        this = default; // For safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<TValue>.Shared.Return( toReturn ); }
    }


    public readonly override string     ToString()      => $"{nameof(Buffer<TValue>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    public readonly          Enumerator GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(Buffer<TValue>)} is read only" ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void EnsureCapacity( in int capacity ) => EnsureCapacity( (uint)capacity );
    public void EnsureCapacity( in uint capacity )
    {
        if ( Length + capacity > Capacity ) { Grow( capacity ); }
    }

    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalCapacityBeyondPos"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalCapacityBeyondPos"> Number of chars requested beyond current position. </param>
    private void Grow( in uint additionalCapacityBeyondPos ) => Grow( (uint)Capacity, (uint)_length + additionalCapacityBeyondPos );
    private void Grow( in uint capacity, in uint requestedCapacity )
    {
        ThrowIfReadOnly();
        Debug.Assert( requestedCapacity > 0 );
        Debug.Assert( requestedCapacity >= capacity, "Grow called incorrectly, no resize is needed." );

        int      minimumLength = checked ((int)Math.Min( Math.Max( requestedCapacity, capacity * 2 ), int.MaxValue ));
        TValue[] poolArray     = ArrayPool<TValue>.Shared.Rent( minimumLength );
        _arrayToReturnToPool.AsSpan().CopyTo( poolArray );
        ArrayPool<TValue>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = poolArray;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly ref TValue GetPinnableReference() => ref buffer.GetPinnableReference();
    [Pure]
    public ref TValue GetPinnableReference( TValue terminate )
    {
        Append( terminate );
        return ref GetPinnableReference();
    }


    public TValue[] ToArray()
    {
        TValue[] array = Span.ToArray();
        Dispose();
        return array;
    }


    public Buffer<TValue> Clear()
    {
        Length = 0;
        buffer.Clear();
        return this;
    }
    public Buffer<TValue> Reset( TValue value )
    {
        Length = 0;
        buffer.Fill( value );
        return this;
    }


    [Pure]
    public ReadOnlySpan<TValue> AsSpan( TValue? terminate )
    {
        if ( terminate is null ) { return Span; }

        Append( terminate );
        return Span;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly Span<TValue> Slice( int start )             => Span[start..];
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly Span<TValue> Slice( int start, int length ) => Span.Slice( start, length );


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int IndexOf( TValue value, in int start = 0 ) => IndexOf( value, start, Length );
    public readonly int IndexOf( TValue value, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( end >= start );

        if ( _comparer is null ) { return NOT_FOUND; }

        for ( int i = start; i < end; i++ )
        {
            if ( _comparer.Equals( buffer[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int LastIndexOf( TValue value, in int end = 0 ) => LastIndexOf( value, Length, end );
    [Pure]
    public readonly int LastIndexOf( TValue value, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );

        if ( _comparer is null ) { return NOT_FOUND; }

        for ( int i = start; i < end; i-- )
        {
            if ( _comparer.Equals( buffer[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [Pure]
    public readonly bool Contains( TValue value )
    {
        Debug.Assert( _comparer is not null );

        foreach ( TValue x in buffer )
        {
            if ( _comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }
    [Pure]
    public readonly bool Contains( scoped in ReadOnlySpan<TValue> value )
    {
        Debug.Assert( _comparer is not null );
        if ( value.Length > buffer.Length ) { return false; }
    #if NET6_0_OR_GREATER
        if ( value.Length == buffer.Length ) { return buffer.SequenceEqual( value, _comparer ); }
    #endif

        for ( int i = 0; i < buffer.Length || i + value.Length < buffer.Length; i++ )
        {
            ReadOnlySpan<TValue> span = buffer.Slice( i, value.Length );

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


    [Pure]
    public readonly bool TryCopyTo( scoped in Span<TValue> destination, out int length )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            length = Length;
            return true;
        }

        length = 0;
        return false;
    }


    public Buffer<TValue> Replace( int index, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        buffer.Slice( index, count ).Fill( value );

        return this;
    }
    public Buffer<TValue> Replace( int index, scoped in ReadOnlySpan<TValue> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );

        span.CopyTo( buffer.Slice( index, span.Length ) );
        return this;
    }


    public Buffer<TValue> Insert( int index, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        int remaining = Length - index;

        buffer.Slice( index, remaining ).CopyTo( buffer[(index + count)..] );

        buffer.Slice( index, count ).Fill( value );

        Length += count;
        return this;
    }
    public Buffer<TValue> Insert( int index, scoped in ReadOnlySpan<TValue> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );

        int remaining = Length - index;

        buffer.Slice( index, remaining ).CopyTo( buffer[(index + span.Length)..] );

        span.CopyTo( buffer[index..] );

        Length += span.Length;
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Add( TValue                         value )        => Append( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Add( scoped in ReadOnlySpan<TValue> span )         => Append( span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Add( TValue                         c, int count ) => Append( c, count );
    public Buffer<TValue> Append( TValue value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        buffer[Length++] = value;
        return this;
    }
    public Buffer<TValue> Append( scoped in ReadOnlySpan<TValue> span )
    {
        ThrowIfReadOnly();

        switch ( span.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when Length + 1 < Capacity:
            {
                buffer[Length++] = span[0];
                return this;
            }

            case 2 when Length + 2 < Capacity:
            {
                buffer[Length++] = span[0];
                buffer[Length++] = span[1];
                return this;
            }

            default:
            {
                EnsureCapacity( span.Length );

                span.CopyTo( Next );
                Length += span.Length;
                return this;
            }
        }
    }
    public Buffer<TValue> Append( TValue c, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        for ( int i = Length; i < Length + count; i++ ) { buffer[i] = c; }

        Length += count;
        return this;
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref struct Enumerator( scoped in Buffer<TValue> buffer )
    {
        private readonly Buffer<TValue> _buffer = buffer;
        private          int            _index  = 0;

        public readonly ref TValue Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer[_index]; }

        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
    }
}



public static class BufferExtensions
{
    public const int NOT_FOUND        = -1;
    public const int DEFAULT_CAPACITY = 64;


    public static Buffer<T> AsBuffer<T>( this ReadOnlySpan<T> span ) => new(span);
    public static void Trim<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.Trim( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void Trim<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.Trim( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimStart<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimStart( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimStart<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimStart( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimEnd<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimEnd( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
    public static void TrimEnd<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimEnd( value );
        buffer.Length = span.Length;
        span.CopyTo( buffer.buffer );
    }
}
