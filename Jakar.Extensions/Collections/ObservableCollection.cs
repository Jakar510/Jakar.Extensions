// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  15:41

namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://stackoverflow.com/a/54733415/9530917"> This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread </see>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/14602121/9530917"> How do I update an ObservableCollection via a worker thread? </see>
///     </para>
/// </summary>
/// <typeparam name="T"> </typeparam>
[Serializable]
public class ObservableCollection<T>( IComparer<T> comparer, IEqualityComparer<T> equalityComparer, int capacity = DEFAULT_CAPACITY ) : CollectionAlerts<T>, IList<T>, IReadOnlyList<T>, IList, IDisposable
{
    protected internal readonly IComparer<T>         comparer         = comparer;
    protected internal readonly IEqualityComparer<T> equalityComparer = equalityComparer;
    protected internal readonly List<T>              buffer           = new(capacity);


    public          int  Capacity       { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Capacity; }
    public override int  Count          { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    public          bool IsEmpty        { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Count == 0; }
    bool IList.          IsFixedSize    { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => ((IList)buffer).IsReadOnly; }
    public bool          IsNotEmpty     { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => Count > 0; }
    bool IList.          IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => ((IList)buffer).IsReadOnly; }
    bool ICollection<T>. IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => ((ICollection<T>)buffer).IsReadOnly; }
    bool ICollection.    IsSynchronized { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => false; }
    object? IList.this[ int      index ] { [MethodImpl(      MethodImplOptions.AggressiveInlining )] get => Get( index ); [MethodImpl( MethodImplOptions.AggressiveInlining )] set => Set( index, (T)value! ); }
    public T this[ int           index ] { [MethodImpl(      MethodImplOptions.AggressiveInlining )] get => Get( index ); [MethodImpl( MethodImplOptions.AggressiveInlining )] set => Set( index, value ); }
    T IReadOnlyList<T>.this[ int index ] { [MethodImpl(      MethodImplOptions.AggressiveInlining )] get => Get( index ); }
    object ICollection.SyncRoot { [MethodImpl(               MethodImplOptions.AggressiveInlining )] get => buffer; }


    public ObservableCollection() : this( Comparer<T>.Default, EqualityComparer<T>.Default ) { }
    public ObservableCollection( int                       capacity ) : this( Comparer<T>.Default, EqualityComparer<T>.Default, capacity ) { }
    public ObservableCollection( scoped in ReadOnlySpan<T> values ) : this( values.Length ) => InternalAdd( values );
    public ObservableCollection( scoped in ReadOnlySpan<T> values, IComparer<T> comparer, IEqualityComparer<T> equalityComparer ) : this( comparer, equalityComparer, values.Length ) => InternalAdd( values );
    public ObservableCollection( IEnumerable<T>            values ) : this( values, Comparer<T>.Default, EqualityComparer<T>.Default ) { }
    public ObservableCollection( IEnumerable<T>            values, IComparer<T> comparer, IEqualityComparer<T> equalityComparer ) : this( comparer, equalityComparer ) => InternalAdd( values );
    public ObservableCollection( T[]                       values ) : this( values.Length ) => InternalAdd( values.AsSpan() );
    public ObservableCollection( T[]                       values, IComparer<T> comparer, IEqualityComparer<T> equalityComparer ) : this( values.AsSpan(), comparer, equalityComparer ) { }


    public virtual void Dispose() => GC.SuppressFinalize( this );


    // public static implicit operator ObservableCollection<T>( MemoryBuffer<T>                                        values ) => new(values);
    public static implicit operator ObservableCollection<T>( List<T>                                                values ) => new(values);
    public static implicit operator ObservableCollection<T>( HashSet<T>                                             values ) => new(values);
    public static implicit operator ObservableCollection<T>( ConcurrentBag<T>                                       values ) => new(values);
    public static implicit operator ObservableCollection<T>( Collection<T>                                          values ) => new(values);
    public static implicit operator ObservableCollection<T>( System.Collections.ObjectModel.ObservableCollection<T> values ) => new(values);
    public static implicit operator ObservableCollection<T>( T[]                                                    values ) => new(values);
    public static implicit operator ObservableCollection<T>( ReadOnlyMemory<T>                                      values ) => new(values.Span);
    public static implicit operator ObservableCollection<T>( ReadOnlySpan<T>                                        values ) => new(values);


    public T[] ToArray() => [.. buffer];


    protected internal void InternalInsert( int i, in T value )
    {
        buffer.Insert( i, value );
        Added( value, i );
    }
    protected internal void InternalInsert( int index, IEnumerable<T> collection )
    {
        foreach ( (int i, T? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }
    protected internal void InternalInsert( int index, scoped in ReadOnlySpan<T> collection )
    {
        foreach ( (int i, T? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }


    protected internal void InternalRemoveRange( int start, int count )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );

        for ( int x = start; x < start + count; x++ )
        {
            buffer.RemoveAt( x );
            Removed( x );
        }
    }


    protected internal bool InternalRemove( in T value )
    {
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }
    protected internal int InternalRemove( in Func<T, bool> match )
    {
        int count = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( T value in buffer.Where( match ) )
        {
            if ( Remove( value ) ) { count++; }
        }

        return count;
    }


    protected internal void InternalClear()
    {
        buffer.Clear();
        Reset();
    }


    protected internal bool InternalRemoveAt( int index, [NotNullWhen( true )] out T? value )
    {
        if ( index < 0 || index >= buffer.Count )
        {
            value = default;
            return false;
        }

        value = buffer[index];
        buffer.RemoveAt( index );
        Removed( value, index );
        return value is not null;
    }
    protected internal bool InternalTryAdd( in T value )
    {
        if ( buffer.Contains( value ) ) { return false; }

        InternalAdd( value );
        return true;
    }


    protected internal void InternalAdd( in T value )
    {
        buffer.Add( value );
        Added( value, buffer.Count - 1 );
    }
    protected internal void InternalAdd( IEnumerable<T> value )
    {
        buffer.AddRange( value );
        Reset();
    }
    protected internal void InternalAdd( scoped in ReadOnlySpan<T> values )
    {
    #if NET8_0_OR_GREATER
        buffer.AddRange( values );
    #else
        buffer.Add( values );
    #endif
        Reset();
    }


    protected internal void InternalAddOrUpdate( in T value )
    {
        int index = buffer.IndexOf( value );

        if ( index >= 0 ) { InternalSet( index, value ); }
        else { InternalAdd( value ); }
    }


    protected internal void InternalSort() => InternalSort( comparer );
    protected internal void InternalSort( IComparer<T> compare )
    {
        buffer.Sort( compare );
        Reset();
    }
    protected internal void InternalSort( Comparison<T> compare )
    {
        buffer.Sort( compare );
        Reset();
    }
    protected internal void InternalSort( int start, int length ) => InternalSort( start, length, comparer );
    protected internal void InternalSort( int start, int length, IComparer<T> compare )
    {
        Guard.IsInRangeFor( start,  buffer, nameof(start) );
        Guard.IsInRangeFor( length, buffer, nameof(length) );
        buffer.Sort( start, length, compare );
        Reset();
    }


    protected internal int InternalRemove( IEnumerable<T> values )
    {
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( T value in values )
        {
            if ( InternalRemove( value ) is false ) { continue; }

            results++;
        }

        return results;
    }
    protected internal int InternalRemove( scoped in ReadOnlySpan<T> values )
    {
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( T value in values )
        {
            if ( InternalRemove( value ) is false ) { continue; }

            results++;
        }

        return results;
    }


    protected internal void InternalReverse()
    {
        buffer.Reverse();
        Reset();
    }
    protected internal void InternalReverse( int start, int length )
    {
        Guard.IsInRangeFor( start,  buffer, nameof(start) );
        Guard.IsInRangeFor( length, buffer, nameof(length) );
        buffer.Reverse( start, length );
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal T InternalGet( int index )
    {
        Guard.IsInRangeFor( index, buffer, nameof(index) );
        T result = buffer[index];
        return result;
    }
    protected internal void InternalSet( int index, T value )
    {
        T? old = index < buffer.Count
                     ? buffer[index]
                     : default;

        Guard.IsInRangeFor( index, buffer, nameof(index) );
        buffer[index] = value;
        Replaced( old, value, index );
    }


    public virtual T    Get( int index )          => InternalGet( index );
    public virtual void Set( int index, T value ) => InternalSet( index, value );


    public virtual bool Exists( Predicate<T>     match ) => buffer.FindIndex( match ) >= 0;
    public virtual int  FindCount( Func<T, bool> match ) => buffer.Count( match );


    public virtual int FindIndex( Predicate<T> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );
        return buffer.FindIndex( start, endInclusive, match );
    }
    public virtual int FindIndex( Predicate<T> match, int start = 0 )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.FindIndex( start, match );
    }


    public virtual int FindLastIndex( Predicate<T> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );
        return buffer.FindLastIndex( start, endInclusive, match );
    }
    public virtual int FindLastIndex( Predicate<T> match, int start = 0 )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.FindLastIndex( start, match );
    }


    public int IndexOf( T value ) => IndexOf( value, 0 );
    public virtual int IndexOf( T value, int start )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( value, start );
    }
    public virtual int IndexOf( T value, int start, int count )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( value, start, count );
    }


    public virtual int LastIndexOf( T value ) => buffer.LastIndexOf( value );
    public virtual int LastIndexOf( T value, int start )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.LastIndexOf( value, start );
    }
    public virtual int LastIndexOf( T value, int start, int count )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        return buffer.LastIndexOf( value, start, count );
    }


    public virtual T[] FindAll( Predicate<T>  match ) => [.. buffer.FindAll( match )];
    public virtual T?  Find( Predicate<T>     match ) => buffer.Find( match );
    public virtual T?  FindLast( Predicate<T> match ) => buffer.FindLast( match );


    public virtual bool TryAdd( T           value )  => InternalTryAdd( value );
    public virtual void Add( params T[]     values ) => Add( new ReadOnlySpan<T>( values ) );
    public virtual void Add( IEnumerable<T> values ) => InternalAdd( values );
    public virtual void Add( SpanEnumerable<T, EnumerableProducer<T>> values )
    {
        foreach ( T value in values ) { InternalAdd( value ); }
    }
    public virtual void Add( scoped in ReadOnlySpan<T>   values ) => InternalAdd( values );
    public         void Add( scoped in ReadOnlyMemory<T> values ) => InternalAdd( values.Span );
    public         void Add( scoped in ImmutableArray<T> values ) => InternalAdd( values.AsSpan() );


    public virtual void AddOrUpdate( T value )
    {
        int index = buffer.IndexOf( value );

        if ( index >= 0 ) { buffer[index] = value; }
        else { buffer.Add( value ); }
    }
    public virtual void AddOrUpdate( IEnumerable<T> values )
    {
        foreach ( T value in values )
        {
            int index = buffer.IndexOf( value );

            if ( index >= 0 ) { buffer[index] = value; }
            else { buffer.Add( value ); }
        }
    }
    public virtual void AddOrUpdate( scoped in ReadOnlySpan<T> values )
    {
        foreach ( T value in values )
        {
            int index = buffer.IndexOf( value );

            if ( index >= 0 ) { buffer[index] = value; }
            else { buffer.Add( value ); }
        }
    }


    public virtual void CopyTo( T[] array )                                                                  => buffer.CopyTo( array );
    public virtual void CopyTo( T[] array, int destinationStartIndex )                                       => buffer.CopyTo( array,            destinationStartIndex );
    public virtual void CopyTo( T[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 ) => buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex );


    public virtual void InsertRange( int index, IEnumerable<T>              collection ) => InternalInsert( index, collection );
    public virtual void InsertRange( int index, scoped in ReadOnlySpan<T>   collection ) => InternalInsert( index, collection );
    public         void InsertRange( int index, scoped in ReadOnlyMemory<T> collection ) => InsertRange( index, collection.Span );
    public         void InsertRange( int index, scoped in ImmutableArray<T> collection ) => InsertRange( index, collection.AsSpan() );


    public virtual void RemoveRange( int start, int count ) => InternalRemoveRange( start, count );


    public virtual int  Remove( Func<T, bool>               match )  => InternalRemove( match );
    public virtual bool Remove( T                           value )  => InternalRemove( value );
    public virtual int  Remove( IEnumerable<T>              values ) => InternalRemove( values );
    public virtual int  Remove( scoped in ReadOnlySpan<T>   values ) => InternalRemove( values );
    public         int  Remove( scoped in ReadOnlyMemory<T> values ) => Remove( values.Span );
    public         int  Remove( scoped in ImmutableArray<T> values ) => Remove( values.AsSpan() );


    public virtual void RemoveAt( int index )                                     => InternalRemoveAt( index, out _ );
    public virtual bool RemoveAt( int index, [NotNullWhen( true )] out T? value ) => InternalRemoveAt( index, out value );


    public virtual void Reverse()
    {
        buffer.Reverse();
        Reset();
    }
    public virtual void Reverse( int start, int count )
    {
        buffer.Reverse( start, count );
        Reset();
    }


    public         void Sort()                                                       => InternalSort();
    public virtual void Sort( IComparer<T>  compare )                                => InternalSort( compare );
    public virtual void Sort( Comparison<T> compare )                                => InternalSort( compare );
    public virtual void Sort( int           start, int count )                       => InternalSort( start, count, comparer );
    public virtual void Sort( int           start, int count, IComparer<T> compare ) => InternalSort( start, count, compare );


    void ICollection.CopyTo( Array array, int start )
    {
        if ( array is T[] values ) { buffer.CopyTo( values, start ); }
    }
    void IList.Remove( object? value )
    {
        if ( value is T x ) { buffer.Remove( x ); }
    }
    int IList.Add( object? value )
    {
        if ( value is not T x ) { return NOT_FOUND; }

        buffer.Add( x );
        return buffer.Count;
    }
    bool IList.Contains( object? value ) => value is T x && buffer.Contains( x );
    int IList.IndexOf( object? value ) =>
        value is T x
            ? buffer.IndexOf( x )
            : NOT_FOUND;
    void IList.Insert( int index, object? value )
    {
        if ( value is T x ) { buffer[index] = x; }
    }


    public virtual bool Contains( T value )          => buffer.Contains( value );
    public virtual void Add( T      value )          => InternalAdd( value );
    public virtual void Insert( int index, T value ) => InternalInsert( index, value );
    public virtual void Clear() => InternalClear();


    protected internal override ReadOnlyMemory<T> FilteredValues()
    {
        int             count  = buffer.Count;
        using Buffer<T> values = new(count);

        for ( int i = 0; i < count; i++ )
        {
            T t = buffer[i];
            if ( Filter( t ) ) { values.Add( t ); }
        }

        return values.ToArray();
    }


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<T> AsSpan()                          => CollectionsMarshal.AsSpan( buffer );
    public             void            EnsureCapacity( in int capacity ) => buffer.EnsureCapacity( capacity );
#endif
}
