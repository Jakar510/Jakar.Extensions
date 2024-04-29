// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  21:3

using NoAlloq;



namespace Jakar.Extensions;


public sealed class MemoryBuffer<T>( IEqualityComparer<T> comparer, int initialCapacity = DEFAULT_CAPACITY ) : ICollection<T>, IDisposable
{
    private readonly IEqualityComparer<T> _comparer = comparer;
    private          int                  _length;
    private          T[]                  _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );


    public int         Capacity   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool.Length; }
    int ICollection<T>.Count      => Length;
    public bool        IsEmpty    { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public bool        IsNotEmpty { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public bool        IsReadOnly { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref T this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public ref T this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public Span<T> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => Span[range]; }
    public Span<T> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Span.Slice( start, length ); }
    public int Length
    {
        [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _length;
        set
        {
            Guard.IsInRange( value, 0, Capacity );
            _length = value;
        }
    }
    internal Memory<T> Memory { [Pure] get => new(_arrayToReturnToPool, 0, Length); }
    public   Span<T>   Next   { [Pure] get => new(_arrayToReturnToPool, Length, Capacity - Length); }
    public   Span<T>   Span   { [Pure] get => new(_arrayToReturnToPool, 0, Length); }


    public MemoryBuffer( int                       initialCapacity ) : this( EqualityComparer<T>.Default, initialCapacity ) { }
    public MemoryBuffer( IEnumerable<T>            span ) : this( EqualityComparer<T>.Default ) => Add( span );
    public MemoryBuffer( scoped in Buffer<T>       span ) : this( span, EqualityComparer<T>.Default ) { }
    public MemoryBuffer( scoped in Buffer<T>       span, IEqualityComparer<T> comparer ) : this( span.Span, comparer ) { }
    public MemoryBuffer( scoped in ReadOnlySpan<T> span ) : this( span, EqualityComparer<T>.Default ) { }
    public MemoryBuffer( scoped in ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( comparer, span.Length ) => Add( span );
    public void Dispose()
    {
        ArrayPool<T>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = [];
    }
    public override string ToString() => $"MemoryBuffer<{typeof(T).Name}>( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";


    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    public Enumerator             GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(MemoryBuffer<T>)} is read only" ); }
    }


    public  void EnsureCapacity( in int  additionalCapacityBeyondPos )                => EnsureCapacity( (uint)additionalCapacityBeyondPos, out T[] _ );
    public  void EnsureCapacity( in uint additionalCapacityBeyondPos )                => EnsureCapacity( additionalCapacityBeyondPos,       out T[] _ );
    private void EnsureCapacity( in int  additionalCapacityBeyondPos, out T[] array ) => EnsureCapacity( (uint)additionalCapacityBeyondPos, out array );
    private void EnsureCapacity( in uint additionalCapacityBeyondPos, out T[] array )
    {
        Guard.IsInRange( additionalCapacityBeyondPos, 1, int.MaxValue );
        uint capacity = (uint)Capacity;

        if ( Length + additionalCapacityBeyondPos > capacity )
        {
            Grow( capacity, (uint)_length + additionalCapacityBeyondPos, out array );
            return;
        }

        array = _arrayToReturnToPool;
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="requestedCapacity"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="requestedCapacity"> the requested new size of the buffer. </param>
    /// <param name="capacity"> the current size of the buffer. </param>
    /// <param name="array"> </param>
    private void Grow( in uint capacity, in uint requestedCapacity, out T[] array )
    {
        ThrowIfReadOnly();
        int minimumLength = GetLength( capacity, requestedCapacity );
        array = ArrayPool<T>.Shared.Rent( minimumLength );
        new Span<T>( _arrayToReturnToPool ).CopyTo( array );
        ArrayPool<T>.Shared.Return( _arrayToReturnToPool );
        _arrayToReturnToPool = array;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ref T GetPinnableReference() => ref Span.GetPinnableReference();
    public ref T GetPinnableReference( T terminate )
    {
        Add( terminate );
        return ref GetPinnableReference();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T[] ToArray() => Span.ToArray();


    public void Clear()
    {
        Length = 0;
        Span.Clear();
    }
    public void Reset( T value )
    {
        Length = 0;
        Span.Fill( value );
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
    public void CopyTo( T[] array )                            => CopyTo( array, 0 );
    public void CopyTo( T[] array, int destinationStartIndex ) => CopyTo( array, Length - 1, 0 );
    public void CopyTo( T[] array, int length, int destinationStartIndex, int sourceStartIndex = 0 )
    {
        Guard.IsInRange( length, 0, Length );
        Span<T> target = new(array, destinationStartIndex, array.Length);
        Span[sourceStartIndex..length].CopyTo( target );
    }


    public void Reverse( int start, int length ) => Span.Slice( start, length ).Reverse();
    public void Reverse() => Span.Reverse();


    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return Span; }

        Add( terminate );
        return Span;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<T> Slice( int start )             => Span[start..];
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<T> Slice( int start, int length ) => Span.Slice( start, length );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( T value, int start = 0 ) => IndexOf( value, start, Length - 1 );
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( Predicate<T> match, int start = 0 ) => FindIndex( start, Length - 1, match );
    public int FindIndex( in int start, in int endInclusive, Predicate<T> match )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( Predicate<T> match, int endInclusive = 0 ) => FindLastIndex( Length - 1, endInclusive, match );
    public int FindLastIndex( in int start, in int endInclusive, Predicate<T> match )
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

        Array.Copy( _arrayToReturnToPool, index + 1, _arrayToReturnToPool, index, _arrayToReturnToPool.Length - index - 1 );
        Length--;
        return true;
    }
    public bool Remove( T item ) => RemoveAt( IndexOf( item ) );


    public void Replace( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        Guard.IsInRange( index + count, 0, Length );

        EnsureCapacity( count, out T[] array );
        Span<T> span = new(array, 0, Length);
        span.Slice( index, count ).Fill( value );
    }
    public void Replace( int index, scoped in ReadOnlySpan<T> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( index,                 0, Length );
        Guard.IsInRange( index + values.Length, 0, Length );

        EnsureCapacity( values.Length, out T[] array );
        Span<T> span = new(array, 0, Length);
        span.CopyTo( span.Slice( index, values.Length ) );
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
        if ( values.IsEmpty ) { return; }

        EnsureCapacity( values.Length, out T[] array );
        if ( index < Capacity ) { Array.Copy( array, index, array, index + values.Length, Capacity - index ); }

        values.CopyTo( array.AsSpan( index, array.Length - index ) );
        Length += values.Length;
    }


    public void Add( T value ) => Add( value, 1 );
    public void Add( T value, in int count )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        ThrowIfReadOnly();
        EnsureCapacity( count, out T[] array );
        Span<T> span = new(array, Length, Capacity - Length);

        for ( int i = 0; i < count; i++ ) { span[i] = value; }

        Length += count;
    }
    public void Add( scoped in ReadOnlySpan<T> values )
    {
        ThrowIfReadOnly();
        Span<T> span = Next;

        switch ( values.Length )
        {
            case 0: return;

            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when span.Length >= 1:
            {
                span[0] =  values[0];
                Length  += 1;
                return;
            }

            case 2 when span.Length >= 2:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                Length  += 2;
                return;
            }

            case 3 when span.Length >= 3:
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

            EnsureCapacity( count, out T[] array );
            collection.CopyTo( array, Length );
            Length += count;
        }
        else
        {
            foreach ( T obj in enumerable ) { Add( obj ); }
        }
    }



    public sealed class Enumerator( scoped in MemoryBuffer<T> buffer ) : IEnumerator<T>
    {
        private readonly MemoryBuffer<T> _buffer = buffer;
        private          int             _index;

        public T            Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer[_index]; }
        object? IEnumerator.Current => Current;

        [MethodImpl( MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = NOT_FOUND;
        [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
        public                                                      void Dispose()  { }
    }
}
