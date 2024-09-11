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
/// <typeparam name="TValue"> </typeparam>
[Serializable]
public class ObservableCollection<TValue>( IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer, int capacity = DEFAULT_CAPACITY ) : CollectionAlerts<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList, IDisposable
{
    protected internal readonly IComparer<TValue>         comparer         = comparer;
    protected internal readonly IEqualityComparer<TValue> equalityComparer = equalityComparer;
    protected internal readonly List<TValue>              buffer           = new(capacity);


    public          int      Capacity       { [Pure, MethodImpl(  MethodImplOptions.AggressiveInlining )] get => buffer.Capacity; }
    public override int      Count          { [Pure, MethodImpl(  MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    public          bool     IsEmpty        { [Pure, MethodImpl(  MethodImplOptions.AggressiveInlining )] get => Count == 0; }
    bool IList.              IsFixedSize    { [MethodImpl(        MethodImplOptions.AggressiveInlining )] get => ((IList)buffer).IsReadOnly; }
    public bool              IsNotEmpty     { [Pure, MethodImpl(  MethodImplOptions.AggressiveInlining )] get => Count > 0; }
    bool IList.              IsReadOnly     { [MethodImpl(        MethodImplOptions.AggressiveInlining )] get => ((IList)buffer).IsReadOnly; }
    bool ICollection<TValue>.IsReadOnly     { [MethodImpl(        MethodImplOptions.AggressiveInlining )] get => ((ICollection<TValue>)buffer).IsReadOnly; }
    bool ICollection.        IsSynchronized { [MethodImpl(        MethodImplOptions.AggressiveInlining )] get => false; }
    object? IList.this[ int                index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Get( index ); [MethodImpl( MethodImplOptions.AggressiveInlining )] set => Set( index, (TValue)value! ); }
    public TValue this[ int                index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Get( index ); [MethodImpl( MethodImplOptions.AggressiveInlining )] set => Set( index, value ); }
    TValue IReadOnlyList<TValue>.this[ int index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Get( index ); }
    object ICollection.SyncRoot { [MethodImpl(                    MethodImplOptions.AggressiveInlining )] get => buffer; }


    public ObservableCollection() : this( Comparer<TValue>.Default, EqualityComparer<TValue>.Default ) { }
    public ObservableCollection( int                            capacity ) : this( Comparer<TValue>.Default, EqualityComparer<TValue>.Default, capacity ) { }
    public ObservableCollection( scoped in ReadOnlySpan<TValue> values ) : this( values.Length ) => InternalAdd( values );
    public ObservableCollection( scoped in ReadOnlySpan<TValue> values, IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer ) : this( comparer, equalityComparer, values.Length ) => InternalAdd( values );
    public ObservableCollection( IEnumerable<TValue>            values ) : this( values, Comparer<TValue>.Default, EqualityComparer<TValue>.Default ) { }
    public ObservableCollection( IEnumerable<TValue>            values, IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer ) : this( comparer, equalityComparer ) => InternalAdd( values );
    public ObservableCollection( TValue[]                       values ) : this( values.Length ) => InternalAdd( values.AsSpan() );
    public ObservableCollection( TValue[]                       values, IComparer<TValue> comparer, IEqualityComparer<TValue> equalityComparer ) : this( values.AsSpan(), comparer, equalityComparer ) { }


    public virtual void Dispose() => GC.SuppressFinalize( this );


    // public static implicit operator ObservableCollection<T>( MemoryBuffer<T>                                        values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( List<TValue>                                                values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( HashSet<TValue>                                             values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ConcurrentBag<TValue>                                       values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( Collection<TValue>                                          values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( System.Collections.ObjectModel.ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( TValue[]                                                    values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ReadOnlyMemory<TValue>                                      values ) => new(values.Span);
    public static implicit operator ObservableCollection<TValue>( ReadOnlySpan<TValue>                                        values ) => new(values);


    public TValue[] ToArray() => [.. buffer];


    protected internal void InternalInsert( int i, in TValue value )
    {
        buffer.Insert( i, value );
        Added( value, i );
    }
    protected internal void InternalInsert( int index, IEnumerable<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }
    protected internal void InternalInsert( int index, scoped in ReadOnlySpan<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
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


    protected internal bool InternalRemove( in TValue value )
    {
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }
    protected internal int InternalRemove( in Func<TValue, bool> match )
    {
        int count = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in buffer.Where( match ) )
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


    protected internal bool InternalRemoveAt( int index, [NotNullWhen( true )] out TValue? value )
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
    protected internal bool InternalTryAdd( in TValue value )
    {
        if ( buffer.Contains( value ) ) { return false; }

        InternalAdd( value );
        return true;
    }


    protected internal void InternalAdd( in TValue value )
    {
        buffer.Add( value );
        Added( value, buffer.Count - 1 );
    }
    protected internal void InternalAdd( IEnumerable<TValue> value )
    {
        buffer.AddRange( value );
        Reset();
    }
    protected internal void InternalAdd( scoped in ReadOnlySpan<TValue> values )
    {
        buffer.AddRange( values );
        Reset();
    }


    protected internal void InternalAddOrUpdate( in TValue value )
    {
        int index = buffer.IndexOf( value );

        if ( index >= 0 ) { InternalSet( index, value ); }
        else { InternalAdd( value ); }
    }


    protected internal void InternalSort() => InternalSort( comparer );
    protected internal void InternalSort( IComparer<TValue> compare )
    {
        buffer.Sort( compare );
        Reset();
    }
    protected internal void InternalSort( Comparison<TValue> compare )
    {
        buffer.Sort( compare );
        Reset();
    }
    protected internal void InternalSort( int start, int length ) => InternalSort( start, length, comparer );
    protected internal void InternalSort( int start, int length, IComparer<TValue> compare )
    {
        Guard.IsInRangeFor( start,  buffer, nameof(start) );
        Guard.IsInRangeFor( length, buffer, nameof(length) );
        buffer.Sort( start, length, compare );
        Reset();
    }


    protected internal int InternalRemove( IEnumerable<TValue> values )
    {
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( InternalRemove( value ) is false ) { continue; }

            results++;
        }

        return results;
    }
    protected internal int InternalRemove( scoped in ReadOnlySpan<TValue> values )
    {
        int results = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
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
    protected internal TValue InternalGet( int index )
    {
        Guard.IsInRangeFor( index, buffer, nameof(index) );
        TValue result = buffer[index];
        return result;
    }
    protected internal void InternalSet( int index, TValue value )
    {
        TValue? old = index < buffer.Count
                          ? buffer[index]
                          : default;

        Guard.IsInRangeFor( index, buffer, nameof(index) );
        buffer[index] = value;
        Replaced( old, value, index );
    }


    public virtual TValue Get( int index )               => InternalGet( index );
    public virtual void   Set( int index, TValue value ) => InternalSet( index, value );


    public virtual bool Exists( Predicate<TValue>     match ) => buffer.FindIndex( match ) >= 0;
    public virtual int  FindCount( Func<TValue, bool> match ) => buffer.Count( match );


    public virtual int FindIndex( Predicate<TValue> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );
        return buffer.FindIndex( start, endInclusive, match );
    }
    public virtual int FindIndex( Predicate<TValue> match, int start = 0 )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.FindIndex( start, match );
    }


    public virtual int FindLastIndex( Predicate<TValue> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );
        return buffer.FindLastIndex( start, endInclusive, match );
    }
    public virtual int FindLastIndex( Predicate<TValue> match, int start = 0 )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.FindLastIndex( start, match );
    }


    public int IndexOf( TValue value ) => IndexOf( value, 0 );
    public virtual int IndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( value, start );
    }
    public virtual int IndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( value, start, count );
    }


    public virtual int LastIndexOf( TValue value ) => buffer.LastIndexOf( value );
    public virtual int LastIndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.LastIndexOf( value, start );
    }
    public virtual int LastIndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        return buffer.LastIndexOf( value, start, count );
    }


    public virtual TValue[] FindAll( Predicate<TValue>  match ) => [.. buffer.FindAll( match )];
    public virtual TValue?  Find( Predicate<TValue>     match ) => buffer.Find( match );
    public virtual TValue?  FindLast( Predicate<TValue> match ) => buffer.FindLast( match );


    public virtual bool TryAdd( TValue           value )  => InternalTryAdd( value );
    public virtual void Add( params TValue[]     values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public virtual void Add( IEnumerable<TValue> values ) => InternalAdd( values );
    public virtual void Add( SpanEnumerable<TValue, EnumerableProducer<TValue>> values )
    {
        foreach ( TValue value in values ) { InternalAdd( value ); }
    }
    public virtual void Add( scoped in ReadOnlySpan<TValue>   values ) => InternalAdd( values );
    public         void Add( scoped in ReadOnlyMemory<TValue> values ) => InternalAdd( values.Span );
    public         void Add( scoped in ImmutableArray<TValue> values ) => InternalAdd( values.AsSpan() );


    public virtual void AddOrUpdate( TValue value )
    {
        int index = buffer.IndexOf( value );

        if ( index >= 0 ) { buffer[index] = value; }
        else { buffer.Add( value ); }
    }
    public virtual void AddOrUpdate( IEnumerable<TValue> values )
    {
        foreach ( TValue value in values )
        {
            int index = buffer.IndexOf( value );

            if ( index >= 0 ) { buffer[index] = value; }
            else { buffer.Add( value ); }
        }
    }
    public virtual void AddOrUpdate( scoped in ReadOnlySpan<TValue> values )
    {
        foreach ( TValue value in values )
        {
            int index = buffer.IndexOf( value );

            if ( index >= 0 ) { buffer[index] = value; }
            else { buffer.Add( value ); }
        }
    }


    public virtual void CopyTo( TValue[] array )                                                                  => buffer.CopyTo( array );
    public virtual void CopyTo( TValue[] array, int destinationStartIndex )                                       => buffer.CopyTo( array,            destinationStartIndex );
    public virtual void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 ) => buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex );


    public virtual void InsertRange( int index, IEnumerable<TValue>              collection ) => InternalInsert( index, collection );
    public virtual void InsertRange( int index, scoped in ReadOnlySpan<TValue>   collection ) => InternalInsert( index, collection );
    public         void InsertRange( int index, scoped in ReadOnlyMemory<TValue> collection ) => InsertRange( index, collection.Span );
    public         void InsertRange( int index, scoped in ImmutableArray<TValue> collection ) => InsertRange( index, collection.AsSpan() );


    public virtual void RemoveRange( int start, int count ) => InternalRemoveRange( start, count );


    public virtual int  Remove( Func<TValue, bool>               match )  => InternalRemove( match );
    public virtual bool Remove( TValue                           value )  => InternalRemove( value );
    public virtual int  Remove( IEnumerable<TValue>              values ) => InternalRemove( values );
    public virtual int  Remove( scoped in ReadOnlySpan<TValue>   values ) => InternalRemove( values );
    public         int  Remove( scoped in ReadOnlyMemory<TValue> values ) => Remove( values.Span );
    public         int  Remove( scoped in ImmutableArray<TValue> values ) => Remove( values.AsSpan() );


    public virtual void RemoveAt( int index )                                          => InternalRemoveAt( index, out _ );
    public virtual bool RemoveAt( int index, [NotNullWhen( true )] out TValue? value ) => InternalRemoveAt( index, out value );


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


    public         void Sort()                                                                 => InternalSort();
    public virtual void Sort( IComparer<TValue>  compare )                                     => InternalSort( compare );
    public virtual void Sort( Comparison<TValue> compare )                                     => InternalSort( compare );
    public virtual void Sort( int                start, int count )                            => InternalSort( start, count, comparer );
    public virtual void Sort( int                start, int count, IComparer<TValue> compare ) => InternalSort( start, count, compare );


    void ICollection.CopyTo( Array array, int start )
    {
        if ( array is TValue[] values ) { buffer.CopyTo( values, start ); }
    }
    void IList.Remove( object? value )
    {
        if ( value is TValue x ) { buffer.Remove( x ); }
    }
    int IList.Add( object? value )
    {
        if ( value is not TValue x ) { return NOT_FOUND; }

        buffer.Add( x );
        return buffer.Count;
    }
    bool IList.Contains( object? value ) => value is TValue x && buffer.Contains( x );
    int IList.IndexOf( object? value ) =>
        value is TValue x
            ? buffer.IndexOf( x )
            : NOT_FOUND;
    void IList.Insert( int index, object? value )
    {
        if ( value is TValue x ) { buffer[index] = x; }
    }


    public virtual bool Contains( TValue value )               => buffer.Contains( value );
    public virtual void Add( TValue      value )               => InternalAdd( value );
    public virtual void Insert( int      index, TValue value ) => InternalInsert( index, value );
    public virtual void Clear() => InternalClear();


    protected internal override FilterBuffer<TValue> FilteredValues()
    {
        int                  count  = buffer.Count;
        FilterBuffer<TValue> values = new(count);

        for ( int i = 0; i < count; i++ )
        {
            TValue t = buffer[i];
            if ( Filter( t ) ) { values.Add( t ); }
        }

        return values;
    }


    public ReadOnlySpan<TValue> AsSpan()                          => CollectionsMarshal.AsSpan( buffer );
    public void                 EnsureCapacity( in int capacity ) => buffer.EnsureCapacity( capacity );
}
