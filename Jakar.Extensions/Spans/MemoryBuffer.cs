// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  21:3

namespace Jakar.Extensions;


public sealed class MemoryBuffer<T>( int initialCapacity, IEqualityComparer<T> comparer ) : ICollection<T>
{
    private readonly IEqualityComparer<T> _comparer            = comparer;
    private          IMemoryOwner<T>      _arrayToReturnToPool = MemoryPool<T>.Shared.Rent( initialCapacity );
    private          int                  _length;


    public int         Capacity   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Memory.Length; }
    int ICollection<T>.Count      => Length;
    public bool        IsEmpty    { [MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length == 0; }
    public bool        IsNotEmpty { [MethodImpl(                  MethodImplOptions.AggressiveInlining )] get => Length > 0; }
    public bool        IsReadOnly { [MethodImpl(                  MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref T this[ int     index ] { [MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Raw[index]; }
    public ref T this[ Index   index ] { [MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Raw[index]; }
    public Span<T> this[ Range range ] { [MethodImpl(             MethodImplOptions.AggressiveInlining )] get => Raw[range]; }
    public Span<T> this[ int   start, int length ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Raw.Slice( start, length ); }
    public   int       Length { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _length; set => _length = Math.Max( Math.Min( Capacity, value ), 0 ); }
    public   Memory<T> Memory { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool.Memory; }
    public   Span<T>   Next   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Raw[Length..]; }
    internal Span<T>   Raw    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Memory.Span; }
    public   Span<T>   Span   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Raw[..Length]; }


    public MemoryBuffer() : this( 64 ) { }
    public MemoryBuffer( int                       initialCapacity ) : this( initialCapacity, EqualityComparer<T>.Default ) { }
    public MemoryBuffer( IEnumerable<T>            span ) : this( 64, EqualityComparer<T>.Default ) => Add( span );
    public MemoryBuffer( scoped in ReadOnlySpan<T> span ) : this( span, EqualityComparer<T>.Default ) => Add( span );
    public MemoryBuffer( scoped in ReadOnlySpan<T> span, IEqualityComparer<T> comparer ) : this( span.Length, comparer ) => Add( span );
    public void Dispose() => _arrayToReturnToPool.Dispose();


    public static implicit operator Span<T>( MemoryBuffer<T> values ) => values.Span;


    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    public override string        ToString()      => $"{nameof(MemoryBuffer<T>)} ( {nameof(Capacity)}: {Capacity}, {nameof(Length)}: {Length}, {nameof(IsReadOnly)}: {IsReadOnly} )";
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    public Enumerator             GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(MemoryBuffer<T>)} is read only" ); }
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalCapacityBeyondPos"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalCapacityBeyondPos"> Number of chars requested beyond current position. </param>
    private void Grow( in int additionalCapacityBeyondPos )
    {
        ThrowIfReadOnly();
        Debug.Assert( additionalCapacityBeyondPos          > 0 );
        Debug.Assert( Length + additionalCapacityBeyondPos >= Capacity, "Grow called incorrectly, no resize is needed." );

        IMemoryOwner<T> poolArray = MemoryPool<T>.Shared.Rent( Math.Max( Length + additionalCapacityBeyondPos, Capacity * 2 ) );
        _arrayToReturnToPool.Memory.Span.CopyTo( poolArray.Memory.Span );
        _arrayToReturnToPool.Dispose();
        _arrayToReturnToPool = poolArray;
    }
    private void GrowAndAppend( T c )
    {
        Grow( Length * 2 );
        Add( c );
    }
    public void EnsureCapacity( in int capacity )
    {
        if ( Length + capacity > Capacity ) { Grow( capacity ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ref T GetPinnableReference() => ref Raw.GetPinnableReference();
    public ref T GetPinnableReference( T terminate )
    {
        Add( terminate );
        return ref GetPinnableReference();
    }


    public T[] ToArray()
    {
        T[] array = Span.ToArray();
        Dispose();
        return array;
    }


    public void Clear()
    {
        Length = 0;
        Raw.Clear();
    }
    public void Reset( T value )
    {
        Length = 0;
        Raw.Fill( value );
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
    public void CopyTo( T[] array, int arrayIndex )            => CopyTo( array, 0, Length );
    public void CopyTo( T[] array, int arrayIndex, int count ) => Span.CopyTo( new Span<T>( array, arrayIndex, count ) );


    public void Reverse( int start, int count ) => Raw.Slice( start, count ).Reverse();
    public void Reverse() => Raw.Reverse();


    public ReadOnlySpan<T> AsSpan( T? terminate )
    {
        if ( terminate is null ) { return Span; }

        Add( terminate );
        return Span;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<T> Slice( int start )             => Span[start..];
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Span<T> Slice( int start, int length ) => Span.Slice( start, length );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( T value, in int start = 0 ) => IndexOf( value, start, Length );
    public int IndexOf( T value, in int start, in int end )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( Predicate<T> match, in int start = 0 ) => IndexOf( match, start, Length );
    public int IndexOf( Predicate<T> match, in int start, in int end )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( T value, in int end = 0 ) => LastIndexOf( value, Length, end );
    public int LastIndexOf( T value, in int start, in int end )
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

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( Predicate<T> match, in int end = 0 ) => LastIndexOf( match, Length, end );
    public int LastIndexOf( Predicate<T> match, in int start, in int end )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? FindLast( Predicate<T> match, in int end = 0 ) => FindLast( match, Length, end );
    public T? FindLast( Predicate<T> match, in int start, in int end )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? Find( Predicate<T> match, in int start = 0 ) => Find( match, start, Length );
    public T? Find( Predicate<T> match, in int start, in int end )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public List<T> FindAll( Predicate<T> match, in int start = 0 ) => FindAll( match, start, Length );
    public List<T> FindAll( Predicate<T> match, in int start, in int end )
    {
        Trace.Assert( start >= 0 && start <= Length );
        Trace.Assert( end   >= 0 && end   <= Length );
        Trace.Assert( start >= end );
        List<T> list = new(Length);

        for ( int i = start; i < end; i++ )
        {
            if ( match( Raw[i] ) ) { list.Add( Raw[i] ); }
        }

        return list;
    }


    public bool Contains( T value )
    {
        Debug.Assert( _comparer is not null );

        foreach ( T x in Raw )
        {
            if ( _comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }
    public bool Contains( scoped in ReadOnlySpan<T> value )
    {
        Debug.Assert( _comparer is not null );
        if ( value.Length > Raw.Length ) { return false; }
    #if NET6_0_OR_GREATER
        if ( value.Length == Raw.Length ) { return Raw.SequenceEqual( value, _comparer ); }
    #endif

        for ( int i = 0; i < Raw.Length || i + value.Length < Raw.Length; i++ )
        {
            ReadOnlySpan<T> span = Raw.Slice( i, value.Length );

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

        Span[(index + 1)..].CopyTo( Span[index..] );
        Length--;
        return true;
    }
    public bool Remove( T item )
    {
        int index = IndexOf( item );
        return index >= 0 && RemoveAt( index );
    }


    public void Replace( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        if ( Length + count > Raw.Length ) { Grow( count ); }

        Raw.Slice( index, count ).Fill( value );
    }
    public void Replace( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( Length + span.Length > Raw.Length ) { Grow( span.Length ); }

        span.CopyTo( Raw.Slice( index, span.Length ) );
    }


    public void Insert( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        if ( Length + count > Raw.Length ) { Grow( count ); }

        int remaining = Length - index;

        Raw.Slice( index, remaining ).CopyTo( Raw[(index + count)..] );

        Raw.Slice( index, count ).Fill( value );

        Length += count;
    }
    public void Insert( int index, scoped in ReadOnlySpan<T> span )
    {
        ThrowIfReadOnly();
        if ( Length + span.Length > Raw.Length ) { Grow( span.Length ); }

        int remaining = Length - index;

        Raw.Slice( index, remaining ).CopyTo( Raw[(index + span.Length)..] );

        span.CopyTo( Raw[index..] );

        Length += span.Length;
    }


    public void Add( T value )
    {
        ThrowIfReadOnly();

        if ( (uint)Length < (uint)Raw.Length ) { Raw[Length++] = value; }
        else { GrowAndAppend( value ); }
    }
    public void Add( T c, int count )
    {
        ThrowIfReadOnly();
        if ( Length + count >= Capacity ) { Grow( count ); }

        for ( int i = Length; i < Length + count; i++ ) { Raw[i] = c; }

        Length += count;
    }
    public void Add( scoped in ReadOnlySpan<T> span )
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
                if ( Length + span.Length >= Capacity ) { Grow( span.Length ); }

                span.CopyTo( Next );
                Length += span.Length;
                return;
            }
        }
    }
    public void Add( IEnumerable<T> span )
    {
        ThrowIfReadOnly();
        foreach ( T c in span ) { Add( c ); }
    }



    [method: MethodImpl( MethodImplOptions.AggressiveInlining )]
    public struct Enumerator( scoped in MemoryBuffer<T> buffer ) : IEnumerator<T>
    {
        private readonly MemoryBuffer<T> _buffer = buffer;
        private          int             _index  = 0;

        public readonly T Current { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer[_index]; }

        readonly object? IEnumerator.Current => Current;

        [MethodImpl( MethodImplOptions.AggressiveInlining )] public void Reset()    => _index = 0;
        [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool MoveNext() => ++_index < _buffer.Length;
        public                                                      void Dispose()  { }
    }
}
