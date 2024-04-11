// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  21:3

namespace Jakar.Extensions;


public sealed class MemoryBuffer<T>( int initialCapacity, IEqualityComparer<T> comparer ) : ICollection<T>
{
    private readonly IEqualityComparer<T> _comparer = comparer;
    private          int                  _length;
    private          T[]                  _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );


    internal Span<T>   Array      { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool; }
    public   int       Capacity   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool.Length; }
    int ICollection<T>.Count      => Length;
    public bool        IsEmpty    { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public bool        IsNotEmpty { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public bool        IsReadOnly { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref T this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public ref T this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public Span<T> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => Span[range]; }
    public Span<T> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Span.Slice( start, length ); }
    public   int       Length { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _length; set => _length = Math.Clamp( value, 0, Capacity ); }
    internal Memory<T> Memory { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool; }
    public   Span<T>   Next   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Array[Length..]; }
    public   Span<T>   Span   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Array[..Length]; }


    public MemoryBuffer() : this( DEFAULT_CAPACITY ) { }
    public MemoryBuffer( int                       initialCapacity ) : this( initialCapacity, EqualityComparer<T>.Default ) { }
    public MemoryBuffer( IEnumerable<T>            span ) : this( DEFAULT_CAPACITY, EqualityComparer<T>.Default ) => Add( span );
    public MemoryBuffer( scoped in Buffer<T>       span ) : this( span, EqualityComparer<T>.Default ) { }
    public MemoryBuffer( scoped in Buffer<T>       span, IEqualityComparer<T> comparer ) : this( span.Span, comparer ) { }
    public MemoryBuffer( scoped in ReadOnlySpan<T> span ) : this( span, EqualityComparer<T>.Default ) { }
    public MemoryBuffer( scoped in ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( span.Length, comparer ) => Add( span );
    public void Dispose() => ArrayPool<T>.Shared.Return( _arrayToReturnToPool );


    public static implicit operator Span<T>( MemoryBuffer<T> values ) => values.Span;

    public override string ToString() => $"{nameof(MemoryBuffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";


    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    public Enumerator             GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(MemoryBuffer<T>)} is read only" ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void EnsureCapacity( in int additionalCapacityBeyondPos ) => EnsureCapacity( (uint)additionalCapacityBeyondPos );
    public void EnsureCapacity( in uint additionalCapacityBeyondPos )
    {
        Guard.IsGreaterThan( additionalCapacityBeyondPos, 0 );
        uint capacity = (uint)Capacity;
        if ( Length + additionalCapacityBeyondPos > capacity ) { Grow( capacity, (uint)_length + additionalCapacityBeyondPos ); }
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="requestedCapacity"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="requestedCapacity"> the requested new size of the buffer. </param>
    /// <param name="capacity"> the current size of the buffer. </param>
    private void Grow( in uint capacity, in uint requestedCapacity )
    {
        ThrowIfReadOnly();
        int minimumLength = GetLength( capacity, requestedCapacity );
        T[] poolArray     = ArrayPool<T>.Shared.Rent( minimumLength );
        new Span<T>( _arrayToReturnToPool ).CopyTo( poolArray );
        ArrayPool<T>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = poolArray;
    }
    private static int GetLength( in ulong capacity, in ulong requestedCapacity )
    {
        Debug.Assert( capacity          <= int.MaxValue );
        Debug.Assert( requestedCapacity <= int.MaxValue );
        Guard.IsGreaterThan( requestedCapacity, capacity );

        ulong result = Math.Max( requestedCapacity, capacity * 2 );
        return (int)Math.Min( result, int.MaxValue );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ref T GetPinnableReference() => ref Array.GetPinnableReference();
    public ref T GetPinnableReference( T terminate )
    {
        Add( terminate );
        return ref GetPinnableReference();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T[] ToArray() => Span.ToArray();


    public void Clear()
    {
        Length = 0;
        Array.Clear();
    }
    public void Reset( T value )
    {
        Length = 0;
        Array.Fill( value );
    }


    public bool TryCopyTo( scoped in Span<T> destination, out int length )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            length = Length;
            return true;
        }

        length = 0;
        return false;
    }
    public void CopyTo( T[] array )                             => CopyTo( array, 0 );
    public void CopyTo( T[] array, int startIndex )             => CopyTo( array, 0, Length );
    public void CopyTo( T[] array, int startIndex, int length ) => Span.CopyTo( new Span<T>( array, startIndex, length ) );


    public void Reverse( int start, int length ) => Array.Slice( start, length ).Reverse();
    public void Reverse() => Array.Reverse();


    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return Span; }

        Add( terminate );
        return Span;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<T> Slice( int start )             => Span[start..];
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<T> Slice( int start, int length ) => Span.Slice( start, length );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( T value, in int start = 0 ) => IndexOf( value, start, Length - 1 );
    public int IndexOf( T value, in int start, in int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        Span<T> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( _comparer.Equals( span[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( Predicate<T> match, in int start = 0 ) => IndexOf( start, Length - 1, match );
    public int IndexOf( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<T> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( T value, in int endInclusive = 0 ) => LastIndexOf( value, Length - 1, endInclusive );
    public int LastIndexOf( T value, in int start, in int endInclusive )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );
        Span<T> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( _comparer.Equals( span[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( Predicate<T> match, in int endInclusive = 0 ) => LastIndexOf( Length - 1, endInclusive, match );
    public int LastIndexOf( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<T> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? FindLast( Predicate<T> match, in int endInclusive = 0 ) => FindLast( Length - 1, endInclusive, match );
    public T? FindLast( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<T> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? Find( Predicate<T> match, in int start = 0 ) => Find( start, Length - 1, match );
    public T? Find( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<T> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T[] FindAll( Predicate<T> match, in int start = 0 ) => FindAll( start, Length - 1, match );
    public T[] FindAll( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Length );
        Guard.IsInRange( endInclusive, 0, Length );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        using Buffer<T> buffer = new(Length);
        ReadOnlySpan<T> span   = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { buffer.Add( span[i] ); }
        }

        return buffer.ToArray();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( T                         value ) => Span.Contains( value, _comparer );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool Contains( scoped in ReadOnlySpan<T> value ) => Span.Contains( value, _comparer );


    public bool RemoveAt( int index )
    {
        if ( index < 0 || index >= Length ) { return false; }

        System.Array.Copy( _arrayToReturnToPool, index + 1, _arrayToReturnToPool, index, _arrayToReturnToPool.Length - index - 1 );
        Length--;
        return true;
    }
    public bool Remove( T item ) => RemoveAt( IndexOf( item ) );


    public void Replace( int index, T value, int count = 1 )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        ThrowIfReadOnly();
        EnsureCapacity( count );

        Span.Slice( index, count ).Fill( value );
    }
    public void Replace( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );
        span.CopyTo( Span.Slice( index, span.Length ) );
    }


    public void Insert( int index, T value, int count = 1 )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        T       x    = value;
        Span<T> span = MemoryMarshal.CreateSpan( ref x, count );
        span.Fill( value );
        Insert( index, span );
    }
    public void Insert( int index, scoped in ReadOnlySpan<T> values )
    {
        ThrowIfReadOnly();
        EnsureCapacity( values.Length );

        if ( values.IsEmpty ) { return; }

        T[] array = _arrayToReturnToPool;
        if ( index < Capacity ) { System.Array.Copy( array, index, array, index + values.Length, Capacity - index ); }

        values.CopyTo( array.AsSpan()[index..] );
        Length += values.Length;
    }


    public void Add( T value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        Array[Length++] = value;
    }
    public void Add( T c, int count )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        ThrowIfReadOnly();
        EnsureCapacity( count );

        Span<T> span = Array;
        for ( int i = Length; i < Length + count; i++ ) { span[i] = c; }

        Length += count;
    }
    public void Add( scoped in ReadOnlySpan<T> values )
    {
        ThrowIfReadOnly();
        Span<T> span = Array;

        switch ( values.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when Length + 1 < Capacity:
            {
                span[Length++] = values[0];
                return;
            }

            case 2 when Length + 2 < Capacity:
            {
                span[Length++] = values[0];
                span[Length++] = values[1];
                return;
            }

            default:
            {
                EnsureCapacity( values.Length );

                values.CopyTo( Next );
                Length += values.Length;
                return;
            }
        }
    }
    public void Add( IEnumerable<T> enumerable )
    {
        ThrowIfReadOnly();

        if ( enumerable is ICollection<T> collection )
        {
            int count = collection.Count;
            if ( count <= 0 ) { return; }

            EnsureCapacity( count );
            collection.CopyTo( _arrayToReturnToPool, Length );
            Length += count;
        }
        else
        {
            foreach ( T obj in enumerable ) { Add( obj ); }
        }
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public sealed class Enumerator( scoped in MemoryBuffer<T> buffer ) : IEnumerator<T>
    {
        private readonly MemoryBuffer<T> _buffer = buffer;
        private          int             _index;

        public T            Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer[_index]; }
        object? IEnumerator.Current => Current;

        [MethodImpl( MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
        public                                                      void Dispose()  { }
    }
}
