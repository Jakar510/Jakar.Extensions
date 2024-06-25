// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


public ref struct Buffer<T>
{
    private          T[]                   _arrayToReturnToPool;
    internal         Span<T>               buffer = Span<T>.Empty;
    private readonly IEqualityComparer<T>? _comparer;
    private          int                   _length;


    public readonly bool    IsEmpty    { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public readonly bool    IsNotEmpty { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public readonly int     Capacity   { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer.Length; }
    public          int     Length     { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => _length; set => _length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public readonly Span<T> Next       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[Length..]; }
    public readonly Span<T> Span       { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get => buffer[..Length]; }
    public          bool    IsReadOnly { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] get;                    init; } = false;
    public          int     Index      { [Pure, MethodImpl(                      MethodImplOptions.AggressiveInlining )] readonly get => Length; set => Length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public readonly ref T this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly ref T this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref buffer[index]; }
    public readonly Span<T> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => buffer[range]; }
    public readonly Span<T> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Slice( start, length ); }


    public Buffer() : this( DEFAULT_CAPACITY ) { }
    public Buffer( int                       initialCapacity ) : this( EqualityComparer<T>.Default, initialCapacity ) { }
    public Buffer( scoped in ReadOnlySpan<T> span ) : this( span, EqualityComparer<T>.Default ) => Append( span );
    public Buffer( scoped in ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( comparer, span.Length ) => Append( span );
    public Buffer( IEqualityComparer<T> comparer, int initialCapacity = DEFAULT_CAPACITY )
    {
        _comparer = comparer;
        buffer    = _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );
    }
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void EnsureCapacity( in int additionalCapacityBeyondPos ) => EnsureCapacity( (uint)additionalCapacityBeyondPos );
    public void EnsureCapacity( in uint additionalCapacityBeyondPos )
    {
        Guard.IsInRange( additionalCapacityBeyondPos, 1, int.MaxValue );
        uint capacity = (uint)Capacity;
        if ( Length + additionalCapacityBeyondPos > capacity ) { Grow( capacity, (uint)_length + additionalCapacityBeyondPos ); }
    }

    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalCapacityBeyondPos"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalCapacityBeyondPos"> Number of chars requested beyond current position. </param>
    private void Grow( in uint additionalCapacityBeyondPos ) => Grow( (uint)Capacity, (uint)_length + additionalCapacityBeyondPos );
    private void Grow( in uint capacity, in uint requestedCapacity )
    {
        ThrowIfReadOnly();
        int minimumLength = GetLength( capacity, requestedCapacity );
        T[] array         = ArrayPool<T>.Shared.Rent( minimumLength );
        new Span<T>( _arrayToReturnToPool ).CopyTo( array );
        ArrayPool<T>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = array;
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

        if ( _comparer is null ) { return NOT_FOUND; }

        for ( int i = start; i < end; i++ )
        {
            if ( _comparer.Equals( buffer[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public readonly int LastIndexOf( T value, in int end = 0 ) => LastIndexOf( value, Length, end );
    [Pure]
    public readonly int LastIndexOf( T value, in int start, in int end )
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
            ReadOnlySpan<T> span = buffer.Slice( i, value.Length );

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
    public readonly bool TryCopyTo( scoped in Span<T> destination, out int length )
    {
        if ( Span.TryCopyTo( destination ) )
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

        buffer.Slice( index, count ).Fill( value );

        return this;
    }
    public Buffer<T> Replace( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );

        span.CopyTo( buffer.Slice( index, span.Length ) );
        return this;
    }


    public Buffer<T> Insert( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        int remaining = Length - index;

        buffer.Slice( index, remaining ).CopyTo( buffer[(index + count)..] );

        buffer.Slice( index, count ).Fill( value );

        Length += count;
        return this;
    }
    public Buffer<T> Insert( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );

        int remaining = Length - index;

        buffer.Slice( index, remaining ).CopyTo( buffer[(index + span.Length)..] );

        span.CopyTo( buffer[index..] );

        Length += span.Length;
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Append( T                         value )        => Add( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Append( IEnumerable<T>            value )        => Add( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Append( scoped in ReadOnlySpan<T> span )         => Add( span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<T> Append( T                         c, int count ) => Add( c, count );


    public Buffer<T> Add( T value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        buffer[Length++] = value;
        return this;
    }
    public Buffer<T> Add( IEnumerable<T> values )
    {
        ThrowIfReadOnly();

        if ( values is ICollection<T> collection )
        {
            int count = collection.Count;
            if ( count <= 0 ) { return this; }

            EnsureCapacity( count );
            collection.CopyTo( _arrayToReturnToPool, Length );
            Length += count;
        }
        else
        {
            foreach ( T obj in values ) { Add( obj ); }
        }

        return this;
    }
    public Buffer<T> Add( scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();

        switch ( span.Length )
        {
            case 0: return this;

            case > 0 when Length + span.Length < Capacity:
            {
                Span<T> next = Next;
                span.CopyTo( next );
                Length += span.Length;
                return this;
            }

            default:
            {
                EnsureCapacity( span.Length );

                Span<T> next = Next;
                span.CopyTo( next );
                Length += span.Length;
                return this;
            }
        }
    }
    public Buffer<T> Add( T c, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );
        Span<T> span = Next;
        for ( int i = 0; i < count; i++ ) { span[i] = c; }

        Length += count;
        return this;
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref struct Enumerator( scoped in Buffer<T> buffer )
    {
        private readonly Buffer<T> _buffer = buffer;
        private          int       _index  = NOT_FOUND;

        public readonly ref T Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer[_index]; }

        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
    }
}
