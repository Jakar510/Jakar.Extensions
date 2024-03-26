// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<T>
{
    private          T[]?                  _arrayToReturnToPool = default;
    internal         Span<T>               buffer               = default;
    private readonly IEqualityComparer<T>? _comparer;


    public readonly bool    IsEmpty    { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public readonly bool    IsNotEmpty { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public readonly int     Capacity   { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer.Length; }
    public          int     Length     { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get; internal set; } = 0;
    public readonly Span<T> Next       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[Length..]; }
    public readonly Span<T> Span       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[..Length]; }
    public          bool    IsReadOnly { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get;                    init; } = false;
    public          int     Index      { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => Length; set => Length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public readonly ref T this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly ref T this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly Span<T> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => buffer[range]; }
    public readonly Span<T> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Slice( start, length ); }


    public Buffer() : this( 64 ) { }
    public Buffer( int initialCapacity ) : this( initialCapacity, EqualityComparer<T>.Default ) { }
    public Buffer( int initialCapacity, IEqualityComparer<T> comparer )
    {
        _comparer = comparer;
        buffer    = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );
    }
    public Buffer( scoped in ReadOnlySpan<T> span ) : this( span, EqualityComparer<T>.Default ) => Append( span );
    public Buffer( scoped in ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( span.Length, comparer ) => Append( span );


    public void Dispose()
    {
        T[]? toReturn = _arrayToReturnToPool;
        this = default; // For safety, to avoid using pooled array if this instance is erroneously appended to again
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return( toReturn ); }
    }


    public readonly override string     ToString()      => $"{nameof(Buffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    public readonly          Enumerator GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private readonly void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(Buffer<T>)} is read only" ); }
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalCapacityBeyondPos"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalCapacityBeyondPos"> Number of chars requested beyond current position. </param>
    private void Grow( in int additionalCapacityBeyondPos )
    {
        ThrowIfReadOnly();
        Debug.Assert( additionalCapacityBeyondPos          > 0 );
        Debug.Assert( Length + additionalCapacityBeyondPos >= Capacity, "Grow called incorrectly, no resize is needed." );

        T[] poolArray = ArrayPool<T>.Shared.Rent( Math.Max( Length + additionalCapacityBeyondPos, Capacity * 2 ) );
        buffer.CopyTo( poolArray );

        T[]? toReturn                 = _arrayToReturnToPool;
        buffer = _arrayToReturnToPool = poolArray;
        if ( toReturn is not null ) { ArrayPool<T>.Shared.Return( toReturn ); }
    }
    private void GrowAndAppend( T c )
    {
        Grow( Length * 2 );
        Append( c );
    }
    public void EnsureCapacity( in int capacity )
    {
        if ( Length + capacity > Capacity ) { Grow( capacity ); }
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly ref T GetPinnableReference() => ref buffer.GetPinnableReference();
    [Pure]
    public ref T GetPinnableReference( T terminate )
    {
        Append( terminate );
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
        buffer.Fill( value );
        return this;
    }


    [Pure]
    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return Span; }

        Append( terminate );
        return Span;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly Span<T> Slice( int start )             => Span[start..];
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly Span<T> Slice( int start, int length ) => Span.Slice( start, length );


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int IndexOf( T value, in int start = 0 ) => IndexOf( value, start, Length );
    public readonly int IndexOf( T value, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( end >= start );

        if ( _comparer is null ) { return -1; }

        for ( int i = start; i < end; i++ )
        {
            if ( _comparer.Equals( buffer[i], value ) ) { return i; }
        }

        return -1;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int LastIndexOf( T value, in int end = 0 ) => LastIndexOf( value, Length, end );
    [Pure]
    public readonly int LastIndexOf( T value, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );

        if ( _comparer is null ) { return -1; }

        for ( int i = start; i < end; i-- )
        {
            if ( _comparer.Equals( buffer[i], value ) ) { return i; }
        }

        return -1;
    }


    [Pure]
    public readonly bool Contains( T value )
    {
        Debug.Assert( _comparer is not null );

        foreach ( T x in buffer )
        {
            if ( _comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }
    [Pure]
    public readonly bool Contains( scoped in ReadOnlySpan<T> value )
    {
        Debug.Assert( _comparer is not null );
        if ( value.Length > buffer.Length ) { return false; }
    #if NET6_0_OR_GREATER
        if ( value.Length == buffer.Length ) { return buffer.SequenceEqual( value, _comparer ); }
    #endif

        for ( int i = 0; i < buffer.Length || i + value.Length < buffer.Length; i++ )
        {
            ReadOnlySpan<T> span = this.buffer.Slice( i, value.Length );

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
    public readonly bool TryCopyTo( scoped in Span<T> destination, out int charsWritten )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            charsWritten = Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }


    public Buffer<T> Replace( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        if ( Length + count > buffer.Length ) { Grow( count ); }

        buffer.Slice( index, count ).Fill( value );

        return this;
    }
    public Buffer<T> Replace( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( Length + span.Length > this.buffer.Length ) { Grow( span.Length ); }

        span.CopyTo( this.buffer.Slice( index, span.Length ) );
        return this;
    }


    public Buffer<T> Insert( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        if ( Length + count > buffer.Length ) { Grow( count ); }

        int remaining = Length - index;

        buffer.Slice( index, remaining ).CopyTo( buffer[(index + count)..] );

        buffer.Slice( index, count ).Fill( value );

        Length += count;
        return this;
    }
    public Buffer<T> Insert( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( Length + span.Length > this.buffer.Length ) { Grow( span.Length ); }

        int remaining = Length - index;

        this.buffer.Slice( index, remaining ).CopyTo( this.buffer[(index + span.Length)..] );

        span.CopyTo( this.buffer[index..] );

        Length += span.Length;
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Add( T                         value )        => Append( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Add( scoped in ReadOnlySpan<T> span )         => Append( span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Add( T                         c, int count ) => Append( c, count );
    public Buffer<T> Append( T value )
    {
        ThrowIfReadOnly();

        if ( (uint)Length < (uint)buffer.Length ) { buffer[Length++] = value; }
        else { GrowAndAppend( value ); }

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
                this.buffer[Length++] = span[0];
                return this;
            }

            case 2 when Length + 2 < Capacity:
            {
                this.buffer[Length++] = span[0];
                this.buffer[Length++] = span[1];
                return this;
            }

            default:
            {
                if ( Length + span.Length >= Capacity ) { Grow( span.Length ); }

                span.CopyTo( Next );
                Length += span.Length;
                return this;
            }
        }
    }
    public Buffer<T> Append( T c, int count )
    {
        ThrowIfReadOnly();
        if ( Length + count >= Capacity ) { Grow( count ); }

        for ( int i = Length; i < Length + count; i++ ) { buffer[i] = c; }

        Length += count;
        return this;
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref struct Enumerator( scoped in Buffer<T> buffer )
    {
        private readonly Buffer<T> _buffer = buffer;
        private          int       _index  = 0;

        public readonly ref T Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer[_index]; }

        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
    }
}



public static class BufferExtensions
{
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
