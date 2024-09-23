// Jakar.Extensions :: Jakar.Extensions
// 06/12/2022  10:15 AM

namespace Jakar.Extensions;


/*
public ref struct Buffer<TValue>
{
    private  TValue[]                   _arrayToReturnToPool;
    internal Span<TValue>               buffer = Span<TValue>.Empty;
    private  IEqualityComparer<TValue>? _comparer;
    private  int                        _length;


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
    public Buffer( int                            initialCapacity ) : this( EqualityComparer<TValue>.Default, initialCapacity ) { }
    public Buffer( scoped in ReadOnlySpan<TValue> span ) : this( span, EqualityComparer<TValue>.Default ) => Append( span );
    public Buffer( scoped in ReadOnlySpan<TValue> span, IEqualityComparer<TValue> comparer ) : this( comparer, span.Length ) => Append( span );
    public Buffer( IEqualityComparer<TValue> comparer, int initialCapacity = DEFAULT_CAPACITY )
    {
        _comparer = comparer;
        buffer    = _arrayToReturnToPool = ArrayPool<TValue>.Shared.Rent( initialCapacity );
    }
    public void Dispose()
    {
        _comparer = null;
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
        int      minimumLength = GetLength( capacity, requestedCapacity );
        TValue[] array         = ArrayPool<TValue>.Shared.Rent( minimumLength );
        new Span<TValue>( _arrayToReturnToPool ).CopyTo( array );
        ArrayPool<TValue>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = array;
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
        Debug.Assert( _comparer is not null );

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
        Debug.Assert( _comparer is not null );

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

        if ( value.Length == buffer.Length ) { return buffer.SequenceEqual( value, _comparer ); }

        for ( int i = 0; i < buffer.Length || i + value.Length < buffer.Length; i++ )
        {
            ReadOnlySpan<TValue> span = buffer.Slice( i, value.Length );
            if ( span.SequenceEqual( value, _comparer ) ) { return true; }
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Append( TValue                         value )        => Add( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Append( IEnumerable<TValue>            value )        => Add( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Append( scoped in ReadOnlySpan<TValue> span )         => Add( span );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Buffer<TValue> Append( TValue                         c, int count ) => Add( c, count );


    public Buffer<TValue> Add( TValue value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        buffer[Length++] = value;
        return this;
    }
    public Buffer<TValue> Add( TValue                         value, int count ) => AddRange( value, count );
    public Buffer<TValue> Add( scoped in ReadOnlySpan<TValue> values )     => AddRange( values );
    public Buffer<TValue> Add( IEnumerable<TValue>            enumerable ) => AddRange( enumerable );
    public Buffer<TValue> AddRange( TValue value, int count )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        ThrowIfReadOnly();
        EnsureCapacity( count );
        Span<TValue> span = Next;

        for ( int i = 0; i < count; i++ ) { span[i] = value; }

        Length += count;
        return this;
    }
    public Buffer<TValue> AddRange( scoped in ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Span<TValue> span = Next;

        switch ( values.Length )
        {
            case 0: return this;

            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when span.Length > 1:
            {
                span[0] =  values[0];
                Length  += 1;
                return this;
            }

            case 2 when span.Length > 2:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                Length  += 2;
                return this;
            }

            case 3 when span.Length > 3:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                span[2] =  values[2];
                Length  += 3;
                return this;
            }

            case > 0 when span.Length > values.Length:
            {
                values.CopyTo( span );
                Length += values.Length;
                return this;
            }

            default:
            {
                EnsureCapacity( values.Length );
                values.CopyTo( Next );
                Length += values.Length;
                return this;
            }
        }
    }
    public Buffer<TValue> AddRange( IEnumerable<TValue> enumerable )
    {
        ThrowIfReadOnly();

        switch ( enumerable )
        {
            case TValue[] values:
            {
                int count = values.Length;
                if ( count <= 0 ) { return this; }

                EnsureCapacity( count );
                new ReadOnlySpan<TValue>( values ).CopyTo( Next );
                Length += count;
                return this;
            }

            case List<TValue> list:
            {
                int count = list.Count;
                if ( count <= 0 ) { return this; }

                EnsureCapacity( count );
                CollectionsMarshal.AsSpan( list ).CopyTo( Next );
                Length += count;
                return this;
            }

            case ICollection<TValue> collection:
            {
                int count = collection.Count;
                if ( count <= 0 ) { return this; }

                EnsureCapacity( count );
                TValue[] array = _arrayToReturnToPool;
                collection.CopyTo( array, Length );
                Length += count;
                return this;
            }

            default:
            {
                foreach ( TValue value in enumerable ) { Add( value ); }

                return this;
            }
        }
    }


    public void Sort( IComparer<TValue>  compare )                                         { }
    public void Sort( Comparison<TValue> compare )                                         { }
    public void Sort( int                compare, int length, IComparer<TValue> comparer ) { }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref struct Enumerator( scoped in Buffer<TValue> buffer )
    {
        private readonly Buffer<TValue> _buffer = buffer;
        private          int            _index  = NOT_FOUND;

        public readonly ref TValue Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer[_index]; }

        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => (uint)++_index < (uint)_buffer.Length;
    }
}
*/
