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
public class ObservableCollection<T> : CollectionAlerts<T>, IList<T>, IReadOnlyList<T>, IList, IDisposable
{
    protected internal readonly IComparer<T>    comparer;
    protected internal readonly MemoryBuffer<T> buffer;


    public sealed override int Count          { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Length; }
    bool IList.                IsFixedSize    { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => buffer.IsReadOnly; }
    bool IList.                IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => buffer.IsReadOnly; }
    bool ICollection<T>.       IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => buffer.IsReadOnly; }
    bool ICollection.          IsSynchronized { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => false; }
    object? IList.this[ int index ] { [MethodImpl(                 MethodImplOptions.AggressiveInlining )] get => buffer[index]; set => buffer[index] = (T)value!; }
    public T this[ int      index ] { [MethodImpl(                 MethodImplOptions.AggressiveInlining )] get => Get( index ); set => Set( index, value ); }
    object ICollection.SyncRoot { [MethodImpl(                     MethodImplOptions.AggressiveInlining )] get => buffer; }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection() : this( Comparer<T>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( scoped in ReadOnlySpan<T> values ) : this( values, Comparer<T>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( scoped in ReadOnlySpan<T> values, IComparer<T> comparer ) : this( comparer, values.Length ) => InternalAdd( values );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( IEnumerable<T>            values ) : this( values, Comparer<T>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( IEnumerable<T>            values, IComparer<T> comparer ) : this( comparer ) => Add( values );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( T[]                       values ) : this( values, Comparer<T>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( T[]                       values, IComparer<T> comparer ) : this( values.AsSpan(), comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( int                       capacity ) : this( Comparer<T>.Default, capacity ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( IComparer<T>              comparer, int capacity = DEFAULT_CAPACITY ) : this( new MemoryBuffer<T>( capacity ), comparer ) { }
    protected internal ObservableCollection( MemoryBuffer<T> values, IComparer<T> comparer )
    {
        this.comparer = comparer;
        buffer        = values;
    }


    public virtual void Dispose() => GC.SuppressFinalize( this );


    public static implicit operator ObservableCollection<T>( List<T>                                                values ) => new(values);
    public static implicit operator ObservableCollection<T>( HashSet<T>                                             values ) => new(values);
    public static implicit operator ObservableCollection<T>( ConcurrentBag<T>                                       values ) => new(values);
    public static implicit operator ObservableCollection<T>( Collection<T>                                          values ) => new(values);
    public static implicit operator ObservableCollection<T>( System.Collections.ObjectModel.ObservableCollection<T> values ) => new(values);
    public static implicit operator ObservableCollection<T>( T[]                                                    values ) => new(new ReadOnlySpan<T>( values ));
    public static implicit operator ObservableCollection<T>( ReadOnlyMemory<T>                                      values ) => new(values.Span);
    public static implicit operator ObservableCollection<T>( ReadOnlySpan<T>                                        values ) => new(values);
    public static implicit operator ObservableCollection<T>( MemoryBuffer<T>                                        values ) => new(values, Comparer<T>.Default);


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void EnsureCapacity( in int  capacity ) => buffer.EnsureCapacity( capacity );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void EnsureCapacity( in uint capacity ) => buffer.EnsureCapacity( capacity );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public T[]  ToArray()                          => buffer.Span.ToArray();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsert( int i, in T value )
    {
        buffer.Insert( i, value );
        Added( value, i );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsert( int index, IEnumerable<T> collection )
    {
        foreach ( (int i, T? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsert( int index, scoped in ReadOnlySpan<T> collection )
    {
        foreach ( (int i, T? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalRemove( in T value )
    {
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal int InternalRemove( in Predicate<T> match )
    {
        int i = buffer.IndexOf( match );
        if ( buffer.RemoveAt( i ) is false ) { return NOT_FOUND; }

        Removed( i );
        return i;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalClear()
    {
        buffer.Clear();
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalRemoveAt( int index, [NotNullWhen( true )] out T? value )
    {
        if ( index < 0 || index >= buffer.Length )
        {
            value = default;
            return false;
        }

        value = buffer[index];
        if ( buffer.RemoveAt( index ) ) { Removed( value, index ); }

        return value is not null;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal bool InternalTryAdd( in T value ) => buffer.Contains( value ) is false && InternalAdd( value );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalAdd( in T value )
    {
        buffer.Add( value );
        Added( value );
        return true;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalAdd( IEnumerable<T> value )
    {
        buffer.Add( value );
        Reset();
        return true;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAddOrUpdate( in T value )
    {
        int index = buffer.IndexOf( value );

        if ( index >= 0 ) { InternalSet( index, value ); }
        else { InternalAdd( value ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAdd( scoped in ReadOnlySpan<T> values )
    {
        foreach ( T value in values ) { InternalAdd( value ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal void InternalSort() => InternalSort( comparer );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalSort( IComparer<T> compare )
    {
    #if NET6_0_OR_GREATER
        buffer.Span.Sort( compare );
    #else
        buffer.Span.Sort( compare.Compare );
    #endif
        Reset();
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalSort( Comparison<T> compare )
    {
        buffer.Span.Sort( compare );
        Reset();
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal void InternalSort( int start, int count ) => InternalSort( start, count, comparer );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalSort( int start, int count, IComparer<T> compare )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );

    #if NET6_0_OR_GREATER
        buffer.Span.Slice( start, count ).Sort( compare );
    #else
        buffer.Span.Slice( start, count ).Sort( compare.Compare );
    #endif

        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalReverse()
    {
        buffer.Span.Reverse();
        Reset();
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalReverse( int start, int count )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );

        buffer.Span.Slice( start, count ).Reverse();
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal ref T InternalGet( int index ) => ref buffer[index];

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalSet( int index, T value )
    {
        T old = buffer[index];
        buffer[index] = value;
        Replaced( old, value, index );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal ReadOnlySpan<T>   AsSpan()   => buffer.Span;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal ReadOnlyMemory<T> AsMemory() => buffer.Memory;


    public virtual ref T    Get( int index )          => ref InternalGet( index );
    public virtual     void Set( int index, T value ) => InternalSet( index, value );


    public virtual bool Exists( Predicate<T> match ) => buffer.IndexOf( match ) >= 0;
    public virtual int FindCount( Predicate<T> match )
    {
        int length = 0;
        foreach ( T _ in buffer.Span.Where( match ) ) { length++; }

        return length;
    }


    public virtual int FindIndex( int start, int count, Predicate<T> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        return buffer.IndexOf( start, count, match );
    }
    public virtual int FindIndex( int start, Predicate<T> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( match, start );
    }
    public virtual int FindIndex( Predicate<T> match ) => buffer.IndexOf( match );


    public virtual int FindLastIndex( int start, int count, Predicate<T> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        return buffer.LastIndexOf( start, count, match );
    }
    public virtual int FindLastIndex( int start, Predicate<T> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.LastIndexOf( match, start );
    }
    public virtual int FindLastIndex( Predicate<T> match ) => buffer.LastIndexOf( match );


    public virtual int IndexOf( T value ) => buffer.IndexOf( value );
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


    public virtual T[] FindAll( Predicate<T>  match ) => buffer.FindAll( match );
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


    public virtual void CopyTo( T[] array )                            => buffer.CopyTo( array );
    public virtual void CopyTo( T[] array, int arrayIndex )            => buffer.CopyTo( array, arrayIndex );
    public virtual void CopyTo( T[] array, int arrayIndex, int count ) => buffer.CopyTo( array, arrayIndex, count );


    public virtual void InsertRange( int index, IEnumerable<T>              collection ) => InternalInsert( index, collection );
    public virtual void InsertRange( int index, scoped in ReadOnlySpan<T>   collection ) => InternalInsert( index, collection );
    public         void InsertRange( int index, scoped in ReadOnlyMemory<T> collection ) => InsertRange( index, collection.Span );
    public         void InsertRange( int index, scoped in ImmutableArray<T> collection ) => InsertRange( index, collection.AsSpan() );


    public virtual void RemoveRange( int start, int count ) => InternalRemoveRange( start, count );


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


    public virtual int Remove( Predicate<T> match )
    {
        int count = 0;

        foreach ( T value in buffer.Span.Where( match ) )
        {
            if ( Remove( value ) ) { count++; }
        }

        return count;
    }
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
        return buffer.Length;
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected internal override ReadOnlyMemory<T> FilteredValues() => FilteredValues( buffer.Span );
}
