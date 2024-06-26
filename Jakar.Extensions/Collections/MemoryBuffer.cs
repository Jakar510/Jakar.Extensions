﻿// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  21:3

using System;



namespace Jakar.Extensions;


public sealed class MemoryBuffer<T>( IEqualityComparer<T> comparer, int initialCapacity = DEFAULT_CAPACITY ) : ICollection<T>, IDisposable
{
    private readonly IEqualityComparer<T> _comparer = comparer;
    private          long                 _length;
    private          T[]                  _arrayToReturnToPool = ArrayPool<T>.Shared.Rent( initialCapacity );


    public int         Capacity { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => _arrayToReturnToPool.Length; }
    int ICollection<T>.Count    => Count;
    public int Count
    {
        get => (int)Interlocked.Read( ref _length );
        set
        {
            Guard.IsInRange( value, 0, Capacity );
            Interlocked.Exchange( ref _length, value );
        }
    }
    public bool IsEmpty    { [Pure, MethodImpl(                         MethodImplOptions.AggressiveInlining )] get => Count == 0; }
    public bool IsNotEmpty { [Pure, MethodImpl(                         MethodImplOptions.AggressiveInlining )] get => Count > 0; }
    public bool IsReadOnly { [Pure, MethodImpl(                         MethodImplOptions.AggressiveInlining )] get; init; } = false;
    public ref T this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public ref T this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public Span<T> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get => Span[range]; }
    public Span<T> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Span.Slice( start, length ); }
    internal Memory<T> Memory { [Pure] get => new(_arrayToReturnToPool, 0, Count); }
    public   Span<T>   Next   { [Pure] get => new(_arrayToReturnToPool, Count, Capacity - Count); }
    public   Span<T>   Span   { [Pure] get => new(_arrayToReturnToPool, 0, Count); }
    public ReadOnlySpan<T>.Enumerator Values
    {
        get
        {
            ReadOnlySpan<T> span = Span;
            return span.GetEnumerator();
        }
    }


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
    public override string ToString() => $"MemoryBuffer<{typeof(T).Name}>( {nameof(Capacity)}: {Capacity}, {nameof(Count)}: {Count}, {nameof(IsReadOnly)}: {IsReadOnly} )";


    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    public Enumerator             GetEnumerator() => new(this);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new InvalidOperationException( $"{nameof(MemoryBuffer<T>)} is read only" ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void EnsureCapacity( in int additionalCapacityBeyondPos ) => EnsureCapacity( (uint)additionalCapacityBeyondPos );
    private void EnsureCapacity( in uint additionalCapacityBeyondPos )
    {
        Guard.IsInRange( additionalCapacityBeyondPos, 1, int.MaxValue );
        uint capacity = (uint)Capacity;

        if ( Count + additionalCapacityBeyondPos > capacity ) { Grow( capacity, (uint)_length + additionalCapacityBeyondPos ); }
    }


    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="requestedCapacity"/> to <see cref="Count"/> whichever is greater. </summary>
    /// <param name="requestedCapacity"> the requested new size of the buffer. </param>
    /// <param name="capacity"> the current size of the buffer. </param>
    /// <param name="array"> </param>
    private void Grow( in uint capacity, in uint requestedCapacity )
    {
        ThrowIfReadOnly();
        int minimumCount = GetLength( capacity, requestedCapacity );
        T[] array        = ArrayPool<T>.Shared.Rent( minimumCount );
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
        Array.Clear( _arrayToReturnToPool, 0, _arrayToReturnToPool.Length );
        Count = 0;
    }
    public void Fill( T value ) { Array.Fill( _arrayToReturnToPool, value, 0, Count ); }


    public bool TryCopyTo( scoped in Span<T> destination, out int length )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            length = Count;
            return true;
        }

        length = 0;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void CopyTo( T[] array )                            => CopyTo( array, 0 );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void CopyTo( T[] array, int destinationStartIndex ) => CopyTo( 0,     array, Count, 0 );
    public void CopyTo( int sourceStartIndex, T[] array, int length, int destinationStartIndex )
    {
        Guard.IsGreaterThanOrEqualTo( destinationStartIndex + array.Length, Count );
        Guard.IsGreaterThanOrEqualTo( length,                               0 );
        Guard.IsLessThanOrEqualTo( length, Count );

        Span<T> target = new(array, destinationStartIndex, array.Length - destinationStartIndex);
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( T   value, int start = 0 ) => IndexOf( start, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int IndexOf( int start, T   value )     => IndexOf( value, start, Count - 1 );
    public int IndexOf( T value, in int start, in int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        Span<T> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( _comparer.Equals( span[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( Predicate<T> match, int          start = 0 ) => FindIndex( start, match );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindIndex( int          start, Predicate<T> match )     => FindIndex( start, Count - 1, match );
    public int FindIndex( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<T> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( T   value,        int endInclusive = 0 ) => LastIndexOf( endInclusive, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int LastIndexOf( int endInclusive, T   value )            => LastIndexOf( value,        Count - 1, endInclusive );
    public int LastIndexOf( T value, in int start, in int endInclusive )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );
        Span<T> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( _comparer.Equals( span[i], value ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( Predicate<T> match,        int          endInclusive = 0 ) => FindLastIndex( endInclusive, match );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public int FindLastIndex( int          endInclusive, Predicate<T> match )            => FindLastIndex( Count - 1,    endInclusive, match );
    public int FindLastIndex( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<T> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? FindLast( Predicate<T> match,        int          endInclusive = 0 ) => FindLast( endInclusive, match );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? FindLast( int          endInclusive, Predicate<T> match )            => FindLast( Count - 1,    endInclusive, match );
    public T? FindLast( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( start, endInclusive );

        Span<T> span = Span;

        for ( int i = start; i >= endInclusive; i-- )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? Find( Predicate<T> match, int          start = 0 ) => Find( start, match );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T? Find( int          start, Predicate<T> match )     => Find( start, Count - 1, match );
    public T? Find( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );

        Span<T> span = Span;

        for ( int i = start; i <= endInclusive; i++ )
        {
            if ( match( span[i] ) ) { return span[i]; }
        }

        return default;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T[] FindAll( Predicate<T> match, int          start = 0 ) => FindAll( start, match );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T[] FindAll( int          start, Predicate<T> match )     => FindAll( start, Count - 1, match );
    public T[] FindAll( in int start, in int endInclusive, Predicate<T> match )
    {
        Guard.IsInRange( start,        0, Count );
        Guard.IsInRange( endInclusive, 0, Count );
        Guard.IsGreaterThanOrEqualTo( endInclusive, start );
        using Buffer<T> buffer = new(Count);
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
        if ( index < 0 || index >= Count ) { return false; }

        Array.Copy( _arrayToReturnToPool, index + 1, _arrayToReturnToPool, index, _arrayToReturnToPool.Length - index - 1 );
        Count--;
        return true;
    }
    public bool Remove( T item ) => RemoveAt( IndexOf( item ) );


    public void Replace( int index, T value, int count = 1 )
    {
        ThrowIfReadOnly();
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        Guard.IsInRange( index + count, 0, Count );

        EnsureCapacity( count );
        T[]     array = _arrayToReturnToPool;
        Span<T> span  = new(array, 0, Count);
        span.Slice( index, count ).Fill( value );
    }
    public void Replace( int index, scoped in ReadOnlySpan<T> values )
    {
        ThrowIfReadOnly();
        Guard.IsInRange( index,                 0, Count );
        Guard.IsInRange( index + values.Length, 0, Count );

        EnsureCapacity( values.Length );
        T[]     array = _arrayToReturnToPool;
        Span<T> span  = new(array, 0, Count);
        values.CopyTo( span.Slice( index, values.Length ) );
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

        EnsureCapacity( values.Length );
        T[] array = _arrayToReturnToPool;
        if ( index < Capacity ) { Array.Copy( array, index, array, index + values.Length, Capacity - index ); }

        values.CopyTo( array.AsSpan( index, array.Length - index ) );
        Count += values.Length;
    }


    public void Add( T value ) => Add( value, 1 );
    public void Add( T value, in int count )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );
        ThrowIfReadOnly();
        EnsureCapacity( count );
        T[]     array = _arrayToReturnToPool;
        Span<T> span  = new(array, Count, Capacity - Count);

        for ( int i = 0; i < count; i++ ) { span[i] = value; }

        Count += count;
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
                Count   += 1;
                return;
            }

            case 2 when span.Length >= 2:
            {
                span[0] =  values[0];
                span[1] =  values[1];
                Count   += 2;
                return;
            }

            case 3 when span.Length >= 3:
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
                EnsureCapacity( values.Length );
                values.CopyTo( Next );
                Count += values.Length;
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
            T[] array = _arrayToReturnToPool;
            collection.CopyTo( array, Count );
            Count += count;
        }
        else
        {
            foreach ( T obj in enumerable ) { Add( obj ); }
        }
    }



    public struct Enumerator( scoped in MemoryBuffer<T> buffer ) : IEnumerator<T>, IEnumerable<T>
    {
        private readonly MemoryBuffer<T> _buffer = buffer;
        private          int             _index  = 0;


        public readonly ref readonly T                   Current { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _buffer.Span[_index]; }
        readonly                     T IEnumerator<T>.   Current => Current;
        readonly                     object? IEnumerator.Current => Current;


        public void Reset() => _index = 0;
        public bool MoveNext()
        {
            MemoryBuffer<T> buffer = _buffer;
            if ( (uint)_index >= (uint)buffer.Count ) { return false; }

            ++_index;
            return true;
        }


        public readonly void                          Dispose()       { }
        public readonly Enumerator                    GetEnumerator() => this;
        readonly        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        readonly        IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    }
}
