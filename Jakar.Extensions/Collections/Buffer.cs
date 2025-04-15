// Jakar.Extensions :: Jakar.Extensions
// 09/21/2024  10:09

using System;



namespace Jakar.Extensions;


public ref struct Buffer<TValue>( int capacity )
    where TValue : IEquatable<TValue>
{
    private readonly TValue[] _array = ArrayPool<TValue>.Shared.Rent( Math.Max( capacity, DEFAULT_CAPACITY ) );
    private          long     _length;


    public int Capacity { [Pure] get => _array.Length; }
    public int Length
    {
        [Pure] get => (int)Interlocked.Read( ref _length );
        set
        {
            Guard.IsInRange( value, 0, Capacity );
            Interlocked.Exchange( ref _length, value );
        }
    }
    public bool IsEmpty    { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public bool IsNotEmpty { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public bool IsReadOnly { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref TValue this[ int     index ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Values[index]; }
    public ref TValue this[ Index   index ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Values[index]; }
    public Span<TValue> this[ Range range ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Values[range]; }
    public Span<TValue> this[ int   start, int length ] { [Pure] get => Values.Slice( start, length ); }
    public Memory<TValue> Memory { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _array; }
    public Span<TValue> Next
    {
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
        get
        {
            int length = Length;
            return new Span<TValue>( _array, length, Capacity - length );
        }
    }
    public Span<TValue> Span   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _array; }
    public Span<TValue> Values { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_array, 0, Length); }


    public Buffer() : this( DEFAULT_CAPACITY ) { }
    public Buffer( params ReadOnlySpan<TValue> span ) : this( span.Length ) => span.CopyTo( _array );
    public void Dispose() => ArrayPool<TValue>.Shared.Return( _array );


    public TValue[] ToArray()
    {
        TValue[] array = Values.ToArray();
        Dispose();
        return array;
    }


    public void Clear()
    {
        Span.Clear();
        Length = 0;
    }
    public void Fill( TValue value ) => Span.Fill( value );


    public void CopyTo( Span<TValue> array ) => Values.CopyTo( array );
    public bool TryCopyTo( Span<TValue> destination, out int length )
    {
        if ( Values.TryCopyTo( destination ) )
        {
            length = Length;
            return true;
        }

        length = 0;
        return false;
    }


    public void Reverse( int start, int length ) => Values.Slice( start, length ).Reverse();
    public void Reverse() => Values.Reverse();


    public ReadOnlySpan<TValue> AsSpan( TValue? terminate )
    {
        if ( terminate is not null ) { Add( terminate ); }

        return Values;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<TValue> Slice( int start )             => Slice( start, Capacity - start );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<TValue> Slice( int start, int length ) => new(_array, start, length);


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( TValue value )            => IndexOf( value, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( TValue value, int start ) => IndexOf( value, start, Length - 1 );
    public int IndexOf( TValue value, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( value.Equals( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( Func<TValue, bool> match )            => FindIndex( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( Func<TValue, bool> match, int start ) => FindIndex( match, start, Length - 1 );
    public int FindIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( TValue value )                   => LastIndexOf( value, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( TValue value, int endInclusive ) => LastIndexOf( value, Length - 1, endInclusive );
    public int LastIndexOf( TValue value, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );
        ReadOnlySpan<TValue> span = Values;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( value.Equals( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( Func<TValue, bool> match )                   => FindLastIndex( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( Func<TValue, bool> match, int endInclusive ) => FindLastIndex( match, Length - 1, endInclusive );
    public int FindLastIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? FindLast( Func<TValue, bool> match )                   => FindLast( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? FindLast( Func<TValue, bool> match, int endInclusive ) => FindLast( match, Length - 1, endInclusive );
    public TValue? FindLast( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? Find( Func<TValue, bool> match )            => Find( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? Find( Func<TValue, bool> match, int start ) => Find( match, start, Length - 1 );
    public TValue? Find( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<TValue> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue[] FindAll( Func<TValue, bool> match )            => FindAll( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue[] FindAll( Func<TValue, bool> match, int start ) => FindAll( match, start, Length - 1 );
    public TValue[] FindAll( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        using Buffer<TValue> buffer = new(Length);
        ReadOnlySpan<TValue> span   = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { buffer.Add( span[i] ); }
        }

        return buffer.ToArray();
    }


    public bool Contains( TValue value )
    {
        ReadOnlySpan<TValue> values = Values;
        return values.Contains( value );
    }
    public bool Contains( params ReadOnlySpan<TValue> value )
    {
        ReadOnlySpan<TValue> values = Values;
        return values.Contains( in value, EqualityComparer<TValue>.Default );
    }


    public bool RemoveAt( int index )
    {
        if ( index < 0 || index >= Length ) { return false; }

        Array.Copy( _array, index + 1, _array, index, _array.Length - index - 1 );
        Length--;
        return true;
    }
    public bool Remove( TValue value ) => RemoveAt( IndexOf( value, 0 ) );


    public void Replace( int start, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        Guard.IsInRange( start + count, 0, Capacity );

        Values.Slice( start, count ).Fill( value );
    }
    public void Replace( int start, params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( start,                 0, Length );
        Guard.IsInRange( start + values.Length, 0, Capacity );

        values.CopyTo( _array.AsSpan( start, values.Length ) );
    }


    public void Insert( int start, TValue value, int count = 1 )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        using IMemoryOwner<TValue> owner = MemoryPool<TValue>.Shared.Rent( count );
        owner.Memory.Span.Fill( value );
        Insert( start, owner.Memory.Span );
    }
    public void Insert( int start, params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( start,                 0, Length );
        Guard.IsInRange( start + values.Length, 0, Capacity );
        if ( values.IsEmpty ) { return; }

        Array.Copy( _array, start, _array, values.Length + start, Capacity - start );
        values.CopyTo( _array.AsSpan( start, values.Length ) );
        Length += values.Length;
    }


    public void Add( TValue value ) => _array[Length++] = value;
    public void Add( TValue value, int count )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        Guard.IsInRange( Length + count, 0, Capacity );
        Span<TValue> span = Next;

        for ( int i = 0; i < count; i++ ) { span[i] = value; }

        Length += count;
    }
    public void Add( params ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Span<TValue> span = Next;

        switch ( values.Length )
        {
            case 0: return;

            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when span.Length > 1:
            {
                span[0] =  values[0];
                Length  += 1;
                return;
            }

            case 2 when span.Length > 2:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                Length  += 2;
                return;
            }

            case 3 when span.Length > 3:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                span[2] =  values[2];
                Length  += 3;
                return;
            }

            case > 0 when span.Length > values.Length:
            {
                values.CopyTo( span );
                Length += values.Length;
                return;
            }

            default:
            {
                Guard.IsInRange( values.Length + Length, 0, Capacity );
                values.CopyTo( Next );
                Length += values.Length;
                return;
            }
        }
    }
    public void AddRange( IEnumerable<TValue> enumerable )
    {
        ThrowIfReadOnly();

        switch ( enumerable )
        {
            case TValue[] array:
            {
                int count = array.Length;
                if ( count <= 0 ) { return; }

                Guard.IsInRange( array.Length + Length, 0, Capacity );
                new ReadOnlySpan<TValue>( array ).CopyTo( Next );
                Length += count;
                return;
            }

            case List<TValue> list:
            {
                int count = list.Count;
                if ( count <= 0 ) { return; }

                Guard.IsInRange( list.Count + Length, 0, Capacity );
                CollectionsMarshal.AsSpan( list ).CopyTo( Next );
                Length += count;
                return;
            }

            case ICollection<TValue> collection:
            {
                int count = collection.Count;
                if ( count <= 0 ) { return; }

                Guard.IsInRange( collection.Count + Length, 0, Capacity );
                collection.CopyTo( _array, Length );
                Length += count;
                return;
            }

            default:
            {
                foreach ( TValue value in enumerable ) { Add( value ); }

                return;
            }
        }
    }


    public void Trim( TValue value )
    {
        Span<TValue>         values = Span;
        ReadOnlySpan<TValue> span   = values.Trim( value );
        Length = span.Length;
        span.CopyTo( values );
    }
    public void Trim( params ReadOnlySpan<TValue> value )
    {
        Span<TValue>         values = Span;
        ReadOnlySpan<TValue> span   = values.Trim( value );
        Length = span.Length;
        span.CopyTo( values );
    }

    public void TrimStart( TValue value )
    {
        Span<TValue>         values = Span;
        ReadOnlySpan<TValue> span   = values.TrimStart( value );
        Length = span.Length;
        span.CopyTo( values );
    }
    public void TrimStart( params ReadOnlySpan<TValue> value )
    {
        Span<TValue>         values = Span;
        ReadOnlySpan<TValue> span   = values.TrimStart( value );
        Length = span.Length;
        span.CopyTo( values );
    }

    public void TrimEnd( TValue value )
    {
        Span<TValue>         values = Span;
        ReadOnlySpan<TValue> span   = values.TrimEnd( value );
        Length = span.Length;
        span.CopyTo( values );
    }
    public void TrimEnd( params ReadOnlySpan<TValue> value )
    {
        Span<TValue>         values = Span;
        ReadOnlySpan<TValue> span   = values.TrimEnd( value );
        Length = span.Length;
        span.CopyTo( values );
    }


    public void Sort()                                                                   => Sort( Comparer<TValue>.Default );
    public void Sort( IComparer<TValue>  comparer )                                      => Span.Sort( comparer );
    public void Sort( Comparison<TValue> comparer )                                      => Span.Sort( comparer );
    public void Sort( int                start, int length, IComparer<TValue> comparer ) => Span.Slice( start, length ).Sort( comparer );


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalRequestedCapacity"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalRequestedCapacity"> the requested new size of the buffer. </param>
    [Pure]
    public Buffer<TValue> Grow( uint additionalRequestedCapacity )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( additionalRequestedCapacity, 1, int.MaxValue );
        Buffer<TValue> buffer = new(GetLength( (uint)Capacity, additionalRequestedCapacity ));
        Values.CopyTo( buffer.Span );
        Dispose();
        return buffer;
    }
    [Pure]
    public Buffer<TValue> EnsureCapacity( int additionalRequestedCapacity )
    {
        uint capacity = (uint)additionalRequestedCapacity;

        return (uint)Length + capacity > (uint)Capacity
                   ? Grow( capacity )
                   : this;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(Buffer<TValue>)} is read only" ); }
    }


    public ReadOnlySpan<TValue>.Enumerator GetEnumerator()
    {
        ReadOnlySpan<TValue> span = Values;
        return span.GetEnumerator();
    }


    /*
    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public ref struct Enumerator( Buffer<TValue> buffer )
    {
        private readonly    Buffer<TValue> _buffer = buffer;
        private             int            _index  = NOT_FOUND;
        public ref readonly TValue         Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer[_index]; }

        public                                                            void Dispose()  { }
        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => (uint)++_index < (uint)_buffer.Length;
    }
    */
}
