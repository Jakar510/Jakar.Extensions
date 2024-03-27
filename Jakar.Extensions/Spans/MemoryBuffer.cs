// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  21:3

using System;



namespace Jakar.Extensions;


public class MemoryBuffer<TValue>( int initialCapacity, IEqualityComparer<TValue> comparer ) : ICollection<TValue>
{
    public const     int                       DEFAULT_CAPACITY     = 64;
    private readonly IEqualityComparer<TValue> _comparer            = comparer;
    protected        TValue[]                  _arrayToReturnToPool = ArrayPool<TValue>.Shared.Rent( initialCapacity );
    private          int                       _length;


    public int              Capacity   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool.Length; }
    int ICollection<TValue>.Count      => Length;
    public bool             IsEmpty    { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public bool             IsNotEmpty { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public bool             IsReadOnly { [Pure, MethodImpl(                  MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref TValue this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Raw[index]; }
    public ref TValue this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Raw[index]; }
    public Span<TValue> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => Raw[range]; }
    public Span<TValue> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Raw.Slice( start, length ); }
    public             int            Length { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _length; set => _length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    protected internal Memory<TValue> Memory { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool; }
    public             Span<TValue>   Next   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Raw[Length..]; }
    protected internal Span<TValue>   Raw    { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool; }
    public             Span<TValue>   Span   { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Raw[..Length]; }


    public MemoryBuffer() : this( DEFAULT_CAPACITY ) { }
    public MemoryBuffer( int                            initialCapacity ) : this( initialCapacity, EqualityComparer<TValue>.Default ) { }
    public MemoryBuffer( IEnumerable<TValue>            span ) : this( DEFAULT_CAPACITY, EqualityComparer<TValue>.Default ) => Add( span );
    public MemoryBuffer( scoped in Buffer<TValue>       span ) : this( span, EqualityComparer<TValue>.Default ) { }
    public MemoryBuffer( scoped in Buffer<TValue>       span, IEqualityComparer<TValue> comparer ) : this( span.Span, comparer ) { }
    public MemoryBuffer( scoped in ReadOnlySpan<TValue> span ) : this( span, EqualityComparer<TValue>.Default ) { }
    public MemoryBuffer( scoped in ReadOnlySpan<TValue> span, IEqualityComparer<TValue> comparer ) : this( span.Length, comparer ) => Add( span );
    public void Dispose() => ArrayPool<TValue>.Shared.Return( _arrayToReturnToPool );


    public static implicit operator Span<TValue>( MemoryBuffer<TValue> values ) => values.Span;


    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();
    public override string                  ToString()      => $"{nameof(MemoryBuffer<TValue>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    IEnumerator IEnumerable.                GetEnumerator() => GetEnumerator();
    public Enumerator                       GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(MemoryBuffer<TValue>)} is read only" ); }
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ref TValue GetPinnableReference() => ref Raw.GetPinnableReference();
    public ref TValue GetPinnableReference( TValue terminate )
    {
        Add( terminate );
        return ref GetPinnableReference();
    }


    public TValue[] ToArray() => Span.ToArray();


    public void Clear()
    {
        Length = 0;
        Raw.Clear();
    }
    public void Reset( TValue value )
    {
        Length = 0;
        Raw.Fill( value );
    }


    public bool TryCopyTo( scoped in Span<TValue> destination, out int length )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            length = Length;
            return true;
        }

        length = 0;
        return false;
    }
    public void CopyTo( TValue[] array )                            => CopyTo( array, 0 );
    public void CopyTo( TValue[] array, int arrayIndex )            => CopyTo( array, 0, Length );
    public void CopyTo( TValue[] array, int arrayIndex, int count ) => Span.CopyTo( new Span<TValue>( array, arrayIndex, count ) );


    public void Reverse( int start, int count ) => Raw.Slice( start, count ).Reverse();
    public void Reverse() => Raw.Reverse();


    public ReadOnlySpan<TValue> AsSpan( TValue? terminate )
    {
        if ( terminate is null ) { return Span; }

        Add( terminate );
        return Span;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<TValue> Slice( int start )             => Span[start..];
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<TValue> Slice( int start, int length ) => Span.Slice( start, length );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( TValue value, in int start = 0 ) => IndexOf( value, start, Length );
    public int IndexOf( TValue value, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( end >= start );

        for ( int i = start; i < end; i++ )
        {
            if ( _comparer.Equals( Raw[i], value ) ) { return i; }
        }

        return -1;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( Predicate<TValue> match, in int start = 0 ) => IndexOf( match, start, Length );
    public int IndexOf( Predicate<TValue> match, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( end >= start );

        for ( int i = start; i < end; i++ )
        {
            if ( match( Raw[i] ) ) { return i; }
        }

        return -1;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( TValue value, in int end = 0 ) => LastIndexOf( value, Length, end );
    public int LastIndexOf( TValue value, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );

        for ( int i = start; i < end; i-- )
        {
            if ( _comparer.Equals( Raw[i], value ) ) { return i; }
        }

        return -1;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( Predicate<TValue> match, in int end = 0 ) => LastIndexOf( match, Length, end );
    public int LastIndexOf( Predicate<TValue> match, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );

        for ( int i = start; i < end; i-- )
        {
            if ( match( Raw[i] ) ) { return i; }
        }

        return -1;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? FindLast( Predicate<TValue> match, in int end = 0 ) => FindLast( match, Length, end );
    public TValue? FindLast( Predicate<TValue> match, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );

        for ( int i = start; i < end; i-- )
        {
            if ( match( Raw[i] ) ) { return Raw[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public TValue? Find( Predicate<TValue> match, in int start = 0 ) => Find( match, start, Length );
    public TValue? Find( Predicate<TValue> match, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );

        for ( int i = start; i < end; i++ )
        {
            if ( match( Raw[i] ) ) { return Raw[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public List<TValue> FindAll( Predicate<TValue> match, in int start = 0 ) => FindAll( match, start, Length );
    public List<TValue> FindAll( Predicate<TValue> match, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );
        List<TValue> list = new(Length);

        for ( int i = start; i < end; i++ )
        {
            if ( match( Raw[i] ) ) { list.Add( Raw[i] ); }
        }

        return list;
    }


    public bool Contains( TValue value )
    {
        Debug.Assert( _comparer is not null );

        foreach ( TValue x in Raw )
        {
            if ( _comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }
    public bool Contains( scoped in ReadOnlySpan<TValue> value )
    {
        Debug.Assert( _comparer is not null );
        if ( value.Length > Raw.Length ) { return false; }
    #if NET6_0_OR_GREATER
        if ( value.Length == Raw.Length ) { return Raw.SequenceEqual( value, _comparer ); }
    #endif

        for ( int i = 0; i < Raw.Length || i + value.Length < Raw.Length; i++ )
        {
            ReadOnlySpan<TValue> span = Raw.Slice( i, value.Length );

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


    public bool RemoveAt( int index )
    {
        if ( index < 0 || index >= Length ) { return false; }

        Array.Copy( _arrayToReturnToPool, index + 1, _arrayToReturnToPool, index, _arrayToReturnToPool.Length - index - 1 );
        return true;
    }
    public bool Remove( TValue item ) => RemoveAt( IndexOf( item ) );


    public void Replace( int index, TValue value, int count = 1 )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        Raw.Slice( index, count ).Fill( value );
    }
    public void Replace( int index, scoped in ReadOnlySpan<TValue> span )
    {
        ThrowIfReadOnly();
        EnsureCapacity( span.Length );
        span.CopyTo( Raw.Slice( index, span.Length ) );
    }


    public void Insert( int index, TValue value, int count = 1 )
    {
        TValue       x    = value;
        Span<TValue> span = MemoryMarshal.CreateSpan( ref x, count );
        span.Fill( value );
        Insert( index, span );
    }
    public void Insert( int index, scoped in ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        EnsureCapacity( values.Length );

        if ( values.IsEmpty ) { return; }

        TValue[] array = _arrayToReturnToPool;
        if ( index < Capacity ) { Array.Copy( array, index, array, index + values.Length, Capacity - index ); }

        values.CopyTo( array.AsSpan()[index..] );
        Length += values.Length;
    }


    public void Add( TValue value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        Raw[Length++] = value;
    }
    public void Add( TValue c, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );

        for ( int i = Length; i < Length + count; i++ ) { Raw[i] = c; }

        Length += count;
    }
    public void Add( scoped in ReadOnlySpan<TValue> span )
    {
        ThrowIfReadOnly();

        switch ( span.Length )
        {
            // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            case 1 when Length + 1 < Capacity:
            {
                Raw[Length++] = span[0];
                return;
            }

            case 2 when Length + 2 < Capacity:
            {
                Raw[Length++] = span[0];
                Raw[Length++] = span[1];
                return;
            }

            default:
            {
                EnsureCapacity( span.Length );

                span.CopyTo( Next );
                Length += span.Length;
                return;
            }
        }
    }
    public void Add( IEnumerable<TValue> span )
    {
        ThrowIfReadOnly();
        foreach ( TValue c in span ) { Add( c ); }
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public struct Enumerator( scoped in MemoryBuffer<TValue> buffer ) : IEnumerator<TValue>
    {
        private readonly MemoryBuffer<TValue> _buffer = buffer;
        private          int                  _index  = 0;

        public readonly TValue Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer[_index]; }

        readonly object? IEnumerator.Current => Current;

        [MethodImpl( MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
        public                                                      void Dispose()  { }
    }
}
