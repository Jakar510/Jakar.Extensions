// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

using System;



namespace Jakar.Extensions;


public ref struct Buffer<T>
{
    private          T[]                   _arrayToReturnToPool;
    internal         Span<T>               buffer = default;
    private readonly IEqualityComparer<T>? _comparer;
    private          int                   _length;


    public readonly bool    IsEmpty    { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public readonly bool    IsNotEmpty { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public readonly int     Capacity   { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer.Length; }
    public          int     Length     { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => _length; set => _length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public readonly Span<T> Next       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[Length..]; }
    public readonly Span<T> Span       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[..Length]; }
    public          bool    IsReadOnly { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get;                    init; } = false;
    public          int     Index      { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => Length; set => Length = Math.Max( val1: Math.Min( val1: Capacity, val2: value ), val2: 0 ); }
    public readonly ref T this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index: index]; }
    public readonly ref T this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly Span<T> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => buffer[range]; }
    public readonly Span<T> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Slice( start: start, length: length ); }


    public Buffer() : this( initialCapacity: 64 ) { }
    public Buffer( int initialCapacity ) : this( initialCapacity: initialCapacity, comparer: EqualityComparer<T>.Default ) { }
    public Buffer( int initialCapacity, IEqualityComparer<T> comparer )
    {
        _comparer = comparer;
        buffer    = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( minimumLength: initialCapacity );
    }
    public Buffer( scoped in ReadOnlySpan<T> span ) : this( span: span, comparer: EqualityComparer<T>.Default ) => Append( span: span );
    public Buffer( scoped in ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( initialCapacity: span.Length, comparer: comparer ) => Append( span: span );


    public void Dispose()
    {
        T[]? toReturn = _arrayToReturnToPool;
        this = default; // For safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return( array: toReturn ); }
    }


    public readonly override string     ToString()      => $"{nameof(Buffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    public readonly          Enumerator GetEnumerator() => new(buffer: this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( message: $"{nameof(Buffer<T>)} is read only" ); }
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

        int minimumLength = checked ((int)Math.Min( Math.Max( requestedCapacity, capacity * 2 ), int.MaxValue ));
        T[] poolArray     = ArrayPool<T>.Shared.Rent( minimumLength );
        _arrayToReturnToPool.AsSpan().CopyTo( poolArray );
        ArrayPool<T>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = poolArray;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly ref T GetPinnableReference() => ref buffer.GetPinnableReference();
    [Pure]
    public ref T GetPinnableReference( T terminate )
    {
        Append( value: terminate );
        return ref GetPinnableReference();
    }


    public T[] ToArray()
    {
        T[] array = Span.ToArray();
        Dispose();
        return array;
    }


    public Buffer<T> Clear()
    {
        Length = 0;
        buffer.Clear();
        return this;
    }
    public Buffer<T> Reset( T value )
    {
        Length = 0;
        buffer.Fill( value: value );
        return this;
    }


    [Pure]
    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return Span; }

        Append( value: terminate );
        return Span;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly Span<T> Slice( int start )             => Span[start..];
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly Span<T> Slice( int start, int length ) => Span.Slice( start: start, length: length );


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int IndexOf( T value, in int start = 0 ) => IndexOf( value: value, start: start, end: Length );
    public readonly int IndexOf( T value, in int start, in int end )
    {
        Trace.Assert( condition: start >= 0 && start <= Length );
        Trace.Assert( condition: end   >= 0 && end   <= Length );
        Trace.Assert( condition: end >= start );

        if ( _comparer is null ) { return -1; }

        for ( int i = start; i < end; i++ )
        {
            if ( _comparer.Equals( x: buffer[index: i], y: value ) ) { return i; }
        }

        return -1;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int LastIndexOf( T value, in int end = 0 ) => LastIndexOf( value: value, start: Length, end: end );
    [Pure]
    public readonly int LastIndexOf( T value, in int start, in int end )
    {
        Trace.Assert( condition: start >= 0 && start <= Length );
        Trace.Assert( condition: end   >= 0 && end   <= Length );
        Trace.Assert( condition: start >= end );

        if ( _comparer is null ) { return -1; }

        for ( int i = start; i < end; i-- )
        {
            if ( _comparer.Equals( x: buffer[index: i], y: value ) ) { return i; }
        }

        return -1;
    }


    [Pure]
    public readonly bool Contains( T value )
    {
        Debug.Assert( condition: _comparer is not null );

        foreach ( T x in buffer )
        {
            if ( _comparer.Equals( x: x, y: value ) ) { return true; }
        }

        return false;
    }
    [Pure]
    public readonly bool Contains( scoped in ReadOnlySpan<T> value )
    {
        Debug.Assert( condition: _comparer is not null );
        if ( value.Length > buffer.Length ) { return false; }
    #if NET6_0_OR_GREATER
        if ( value.Length == buffer.Length ) { return buffer.SequenceEqual( other: value, comparer: _comparer ); }
    #endif

        for ( int i = 0; i < buffer.Length || i + value.Length < buffer.Length; i++ )
        {
            ReadOnlySpan<T> span = buffer.Slice( start: i, length: value.Length );

        #if NET6_0_OR_GREATER
            if ( span.SequenceEqual( other: value, comparer: _comparer ) ) { return true; }
        #else
            for ( int j = 0; j < span.Length; j++ )
            {
                if ( _comparer.Equals( x: span[index: j], y: value[index: j] ) ) { return true; }
            }

        #endif
        }

        return false;
    }


    [Pure]
    public readonly bool TryCopyTo( scoped in Span<T> destination, out int length )
    {
        if ( Span.TryCopyTo( destination: destination ) )
        {
            length = Length;
            return true;
        }

        length = 0;
        return false;
    }


    public Buffer<T> Replace( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        buffer.Slice( start: index, length: count ).Fill( value: value );

        return this;
    }
    public Buffer<T> Replace( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );

        span.CopyTo( destination: buffer.Slice( start: index, length: span.Length ) );
        return this;
    }


    public Buffer<T> Insert( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        int remaining = Length - index;

        buffer.Slice( start: index, length: remaining ).CopyTo( destination: buffer[(index + count)..] );

        buffer.Slice( start: index, length: count ).Fill( value: value );

        Length += count;
        return this;
    }
    public Buffer<T> Insert( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );

        int remaining = Length - index;

        buffer.Slice( start: index, length: remaining ).CopyTo( destination: buffer[(index + span.Length)..] );

        span.CopyTo( destination: buffer[index..] );

        Length += span.Length;
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Add( T                         value )        => Append( value: value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Add( scoped in ReadOnlySpan<T> span )         => Append( span: span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Add( T                         c, int count ) => Append( c: c, count: count );
    public Buffer<T> Append( T value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        buffer[Length++] = value;
        return this;
    }
    public Buffer<T> Append( scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();

        switch ( span.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when Length + 1 < Capacity:
            {
                buffer[index: Length++] = span[index: 0];
                return this;
            }

            case 2 when Length + 2 < Capacity:
            {
                buffer[index: Length++] = span[index: 0];
                buffer[index: Length++] = span[index: 1];
                return this;
            }

            default:
            {
                EnsureCapacity( span.Length );

                span.CopyTo( destination: Next );
                Length += span.Length;
                return this;
            }
        }
    }
    public Buffer<T> Append( T c, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        for ( int i = Length; i < Length + count; i++ ) { buffer[index: i] = c; }

        Length += count;
        return this;
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref struct Enumerator( scoped in Buffer<T> buffer )
    {
        private readonly Buffer<T> _buffer = buffer;
        private          int       _index  = 0;

        public readonly ref T Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer[index: _index]; }

        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
    }
}



public static class BufferExtensions
{
    public static Buffer<T> AsBuffer<T>( this ReadOnlySpan<T> span ) => new(span: span);
    public static void Trim<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.Trim( trimElement: value );
        buffer.Length = span.Length;
        span.CopyTo( destination: buffer.buffer );
    }
    public static void Trim<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.Trim( trimElements: value );
        buffer.Length = span.Length;
        span.CopyTo( destination: buffer.buffer );
    }
    public static void TrimStart<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimStart( trimElement: value );
        buffer.Length = span.Length;
        span.CopyTo( destination: buffer.buffer );
    }
    public static void TrimStart<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimStart( trimElements: value );
        buffer.Length = span.Length;
        span.CopyTo( destination: buffer.buffer );
    }
    public static void TrimEnd<T>( this Buffer<T> buffer, scoped in T value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimEnd( trimElement: value );
        buffer.Length = span.Length;
        span.CopyTo( destination: buffer.buffer );
    }
    public static void TrimEnd<T>( this Buffer<T> buffer, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> span = buffer.buffer.TrimEnd( trimElements: value );
        buffer.Length = span.Length;
        span.CopyTo( destination: buffer.buffer );
    }
}
