// Jakar.Extensions :: Jakar.Extensions
// 09/21/2024  10:09

using System.Collections.Generic;



namespace Jakar.Extensions;


public record struct Buffer<TValue> : IEnumerable<TValue>, IDisposable
    where TValue : IEquatable<TValue>
{
    private readonly TValue[] _array;
    private          long     _length;


    public int Capacity { [Pure] get => _array.Length; }
    public int Count
    {
        [Pure] get => (int)Interlocked.Read( ref _length );
        set
        {
            Guard.IsInRange( value, 0, Capacity );
            Interlocked.Exchange( ref _length, value );
        }
    }
    public bool IsEmpty    { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Count == 0; }
    public bool IsNotEmpty { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Count > 0; }
    public bool IsReadOnly { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref TValue this[ int     index ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public ref TValue this[ Index   index ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public Span<TValue> this[ Range range ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Span[range]; }
    public Span<TValue> this[ int   start, int length ] { [Pure] get => Span.Slice( start, length ); }
    public Memory<TValue>       Memory { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_array, 0, Capacity); }
    public Span<TValue>         Next   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_array, Count, Capacity); }
    public Span<TValue>         Span   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_array, 0, Capacity); }
    public ReadOnlySpan<TValue> Values { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(_array, 0, Capacity); }


    public Buffer() : this( DEFAULT_CAPACITY ) { }
    public Buffer( int capacity )
    {
        Guard.IsGreaterThan( capacity, 0 );
        _array = ArrayPool<TValue>.Shared.Rent( Math.Max( capacity, DEFAULT_CAPACITY ) );
    }
    public Buffer( scoped in ReadOnlySpan<TValue> span ) : this( span.Length ) => span.CopyTo( Span );
    public void Dispose() => ArrayPool<TValue>.Shared.Return( _array );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue[] ToArray() => Span.ToArray();


    public void Clear()
    {
        Span.Clear();
        Count = 0;
    }
    public void Fill( TValue value ) => Span.Fill( value );


    public bool TryCopyTo( scoped in Span<TValue> destination, out int length )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            length = Count;
            return true;
        }

        length = 0;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void CopyTo( TValue[] array )                            => CopyTo( array, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void CopyTo( TValue[] array, int destinationStartIndex ) => CopyTo( 0,     array, Count, 0 );
    public void CopyTo( int sourceStartIndex, TValue[] array, int length, int destinationStartIndex )
    {
        Guard.IsGreaterThanOrEqualTo( destinationStartIndex + array.Length, Count );
        Guard.IsGreaterThanOrEqualTo( length,                               0 );
        Guard.IsLessThanOrEqualTo( length, Count );

        Span<TValue> target = new(array, destinationStartIndex, array.Length - destinationStartIndex);
        Span[sourceStartIndex..length].CopyTo( target );
    }


    public void Reverse( int start, int length ) => Span.Slice( start, length ).Reverse();
    public void Reverse() => Span.Reverse();


    public ReadOnlySpan<TValue> AsSpan( TValue? terminate )
    {
        if ( terminate is null ) { return Span; }

        Add( terminate );
        return Span;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<TValue> Slice( int start )             => Span[start..];
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<TValue> Slice( int start, int length ) => Span.Slice( start, length );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( TValue value )            => IndexOf( value, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( TValue value, int start ) => IndexOf( value, start, Count - 1 );
    public int IndexOf( TValue value, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        Span<TValue> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( value.Equals( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( Func<TValue, bool> match )            => FindIndex( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( Func<TValue, bool> match, int start ) => FindIndex( match, start, Count - 1 );
    public int FindIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<TValue> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( TValue value )                   => LastIndexOf( value, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( TValue value, int endInclusive ) => LastIndexOf( value, Count - 1, endInclusive );
    public int LastIndexOf( TValue value, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );
        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( value.Equals( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( Func<TValue, bool> match )                   => FindLastIndex( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( Func<TValue, bool> match, int endInclusive ) => FindLastIndex( match, Count - 1, endInclusive );
    public int FindLastIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? FindLast( Func<TValue, bool> match )                   => FindLast( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? FindLast( Func<TValue, bool> match, int endInclusive ) => FindLast( match, Count - 1, endInclusive );
    public TValue? FindLast( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<TValue> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? Find( Func<TValue, bool> match )            => Find( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? Find( Func<TValue, bool> match, int start ) => Find( match, start, Count - 1 );
    public TValue? Find( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<TValue> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue[] FindAll( Func<TValue, bool> match )            => FindAll( match, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue[] FindAll( Func<TValue, bool> match, int start ) => FindAll( match, start, Count - 1 );
    public TValue[] FindAll( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        using Buffer<TValue> buffer = new(Count);
        ReadOnlySpan<TValue> span   = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { buffer.Add( span[i] ); }
        }

        return buffer.ToArray();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( TValue                         value ) => Span.Contains( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( scoped in ReadOnlySpan<TValue> value ) => Span.Contains( value, EqualityComparer<TValue>.Default );


    public bool RemoveAt( int index )
    {
        if ( index < 0 || index >= Count ) { return false; }

        Array.Copy( _array, index + 1, _array, index, _array.Length - index - 1 );
        Count--;
        return true;
    }
    public bool Remove( TValue value ) => RemoveAt( IndexOf( value, 0 ) );


    public void Replace( int start, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        Guard.IsInRange( start + count, 0, Capacity );

        Span.Slice( start, count ).Fill( value );
    }
    public void Replace( int start, scoped in ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( start,                 0, Count );
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
    public void Insert( int start, scoped in ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( start,                 0, Count );
        Guard.IsInRange( start + values.Length, 0, Capacity );
        if ( values.IsEmpty ) { return; }

        Array.Copy( _array, start, _array, values.Length + start, Capacity - start );
        values.CopyTo( _array.AsSpan( start, values.Length ) );
        Count += values.Length;
    }


    public void Add( TValue                         value )            => Add( value, 1 );
    public void Add( scoped in ReadOnlySpan<TValue> values )     => AddRange( values );
    public void Add( IEnumerable<TValue>            enumerable ) => AddRange( enumerable );
    public void Add( TValue value, int count )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        Guard.IsInRange( Count + count, 0, Capacity );
        Span<TValue> span = Next;

        for ( int i = 0; i < count; i++ ) { span[i] = value; }

        Count += count;
    }
    public void AddRange( scoped in ReadOnlySpan<TValue> values )
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
                Count   += 1;
                return;
            }

            case 2 when span.Length > 2:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                Count   += 2;
                return;
            }

            case 3 when span.Length > 3:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                span[2] =  values[2];
                Count   += 3;
                return;
            }

            case > 0 when span.Length > values.Length:
            {
                values.CopyTo( span );
                Count += values.Length;
                return;
            }

            default:
            {
                Guard.IsInRange( values.Length + Count, 0, Capacity );
                values.CopyTo( Next );
                Count += values.Length;
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

                Guard.IsInRange( array.Length + Count, 0, Capacity );
                new ReadOnlySpan<TValue>( array ).CopyTo( Next );
                Count += count;
                return;
            }

            case List<TValue> list:
            {
                int count = list.Count;
                if ( count <= 0 ) { return; }

                Guard.IsInRange( list.Count + Count, 0, Capacity );
                CollectionsMarshal.AsSpan( list ).CopyTo( Next );
                Count += count;
                return;
            }

            case ICollection<TValue> collection:
            {
                int count = collection.Count;
                if ( count <= 0 ) { return; }

                Guard.IsInRange( collection.Count + Count, 0, Capacity );
                collection.CopyTo( _array, Count );
                Count += count;
                return;
            }

            default:
            {
                foreach ( TValue value in enumerable ) { Add( value ); }

                return;
            }
        }
    }


    public void Trim( scoped in TValue value )
    {
        ReadOnlySpan<TValue> span = Span.Trim( value );
        Count = span.Length;
        span.CopyTo( Span );
    }
    public void Trim( scoped in ReadOnlySpan<TValue> value )
    {
        ReadOnlySpan<TValue> span = Span.Trim( value );
        Count = span.Length;
        span.CopyTo( Span );
    }
    public void TrimStart( scoped in TValue value )
    {
        ReadOnlySpan<TValue> span = Span.TrimStart( value );
        Count = span.Length;
        span.CopyTo( Span );
    }
    public void TrimStart( scoped in ReadOnlySpan<TValue> value )
    {
        ReadOnlySpan<TValue> span = Span.TrimStart( value );
        Count = span.Length;
        span.CopyTo( Span );
    }
    public void TrimEnd( scoped in TValue value )
    {
        ReadOnlySpan<TValue> span = Span.TrimEnd( value );
        Count = span.Length;
        span.CopyTo( Span );
    }
    public void TrimEnd( scoped in ReadOnlySpan<TValue> value )
    {
        ReadOnlySpan<TValue> span = Span.TrimEnd( value );
        Count = span.Length;
        span.CopyTo( Span );
    }


    public void Sort()                                                                   => Sort( Comparer<TValue>.Default );
    public void Sort( IComparer<TValue>  comparer )                                      => Span.Sort( comparer );
    public void Sort( Comparison<TValue> comparer )                                      => Span.Sort( comparer );
    public void Sort( int                start, int length, IComparer<TValue> comparer ) => Span.Slice( start, length ).Sort( comparer );


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalRequestedCapacity"/> to <see cref="Count"/> whichever is greater. </summary>
    /// <param name="additionalRequestedCapacity"> the requested new size of the buffer. </param>
    [Pure]
    public Buffer<TValue> Grow( in uint additionalRequestedCapacity )
    {
        Guard.IsInRange( additionalRequestedCapacity, 1, int.MaxValue );
        return Grow( (uint)Capacity, additionalRequestedCapacity );
    }
    internal Buffer<TValue> Grow( in uint capacity, in uint additionalRequestedCapacity )
    {
        ThrowIfReadOnly();
        int            minimumCount = GetLength( capacity, additionalRequestedCapacity );
        Buffer<TValue> buffer       = new(minimumCount);
        Span.CopyTo( buffer.Span );
        Dispose();
        return buffer;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(Buffer<TValue>)} is read only" ); }
    }


    public IEnumerator<TValue> GetEnumerator() => new Enumerator( this );
    IEnumerator IEnumerable.   GetEnumerator() => GetEnumerator();



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public struct Enumerator( scoped in Buffer<TValue> buffer ) : IEnumerator<TValue>
    {
        private readonly Buffer<TValue> _buffer = buffer;
        private          int            _index  = NOT_FOUND;
        public           TValue         Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer[_index]; }
        object IEnumerator.             Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Current; }

        public                                                            void Dispose()  { }
        [MethodImpl(       MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => (uint)++_index < (uint)_buffer.Count;
    }
}
