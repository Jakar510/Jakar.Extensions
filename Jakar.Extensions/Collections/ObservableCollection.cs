// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  15:41

using System.Collections.Generic;



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
public class ObservableCollection<TValue>( IComparer<TValue> comparer, int capacity = DEFAULT_CAPACITY ) : CollectionAlerts<TValue>, IIObservableCollection<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList
    where TValue : IEquatable<TValue>
{
    protected internal readonly IComparer<TValue> comparer = comparer;
    protected internal readonly List<TValue>      buffer   = new(capacity);


    public          int  Capacity       { [Pure, MethodImpl(      MethodImplOptions.AggressiveInlining )] get => buffer.Capacity; }
    public override int  Count          { [Pure, MethodImpl(      MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    public          bool IsEmpty        { [Pure, MethodImpl(      MethodImplOptions.AggressiveInlining )] get => Count == 0; }
    bool IList.          IsFixedSize    { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get => ((IList)buffer).IsFixedSize; }
    public bool          IsNotEmpty     { [Pure, MethodImpl(      MethodImplOptions.AggressiveInlining )] get => Count > 0; }
    public bool          IsReadOnly     { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get; init; }
    bool ICollection.    IsSynchronized { [MethodImpl(            MethodImplOptions.AggressiveInlining )] get => false; }
    object? IList.this[ int                index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Get( index ); [MethodImpl( MethodImplOptions.AggressiveInlining )] set => Set( index, (TValue)value! ); }
    public TValue this[ int                index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Get( index ); [MethodImpl( MethodImplOptions.AggressiveInlining )] set => Set( index, value ); }
    TValue IReadOnlyList<TValue>.this[ int index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Get( index ); }
    object ICollection.SyncRoot { [MethodImpl(                    MethodImplOptions.AggressiveInlining )] get => buffer; }


    public ObservableCollection() : this( Comparer<TValue>.Default ) { }
    public ObservableCollection( int                              capacity ) : this( Comparer<TValue>.Default, capacity ) { }
    public ObservableCollection( scoped in ImmutableArray<TValue> values ) : this( values.Length ) => InternalAdd( values.AsSpan() );
    public ObservableCollection( scoped in ImmutableArray<TValue> values, IComparer<TValue> comparer ) : this( comparer, values.Length ) => InternalAdd( values.AsSpan() );
    public ObservableCollection( scoped in ReadOnlyMemory<TValue> values ) : this( values.Length ) => InternalAdd( values.Span );
    public ObservableCollection( scoped in ReadOnlyMemory<TValue> values, IComparer<TValue> comparer ) : this( comparer, values.Length ) => InternalAdd( values.Span );
    public ObservableCollection( scoped in ReadOnlySpan<TValue>   values ) : this( values.Length ) => InternalAdd( values );
    public ObservableCollection( scoped in ReadOnlySpan<TValue>   values, IComparer<TValue> comparer ) : this( comparer, values.Length ) => InternalAdd( values );
    public ObservableCollection( TValue[]                         values ) : this( values.Length ) => InternalAdd( new ReadOnlySpan<TValue>( values ) );
    public ObservableCollection( TValue[]                         values, IComparer<TValue> comparer ) : this( new ReadOnlySpan<TValue>( values ), comparer ) { }
    public ObservableCollection( IEnumerable<TValue>              values ) : this( values, Comparer<TValue>.Default ) { }
    public ObservableCollection( IEnumerable<TValue>              values, IComparer<TValue> comparer ) : this( comparer ) => InternalAdd( values );
    public virtual void Dispose()
    {
        buffer.Clear();
        GC.SuppressFinalize( this );
    }


    public static implicit operator ObservableCollection<TValue>( Buffer<TValue>                                              values ) => new(values.Span);
    public static implicit operator ObservableCollection<TValue>( List<TValue>                                                values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( HashSet<TValue>                                             values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ConcurrentBag<TValue>                                       values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( Collection<TValue>                                          values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( System.Collections.ObjectModel.ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( TValue[]                                                    values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ImmutableArray<TValue>                                      values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ReadOnlyMemory<TValue>                                      values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ReadOnlySpan<TValue>                                        values ) => new(values);


    public TValue[] ToArray() => buffer.ToArray();


    protected internal void InternalInsert( int i, in TValue value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( 1 );
        buffer.Insert( i, value );
        Added( value, i );
    }
    protected internal void InternalInsert( int index, IEnumerable<TValue> collection )
    {
        ThrowIfReadOnly();
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }
    protected internal void InternalInsert( int index, scoped in ReadOnlySpan<TValue> collection )
    {
        ThrowIfReadOnly();
        EnsureCapacity( collection.Length );
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsert( i, value ); }
    }
    private void InternalInsert( int index, TValue collection, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );
        for ( int i = 0; i < count; i++ ) { InternalInsert( index + i, collection ); }
    }


    private void InternalReplace( int index, in TValue value, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );
        for ( int i = 0; i < count; i++ ) { buffer[i + index] = value; }

        Reset();
    }
    protected internal void InternalReplace( int index, scoped in ReadOnlySpan<TValue> value )
    {
        ThrowIfReadOnly();
        EnsureCapacity( value.Length );
        for ( int i = 0; i < value.Length; i++ ) { buffer[i + index] = value[i]; }

        Reset();
    }


    protected internal void InternalRemove( int start, int count )
    {
        ThrowIfReadOnly();
        Guard.IsInRangeFor( start,         buffer, nameof(start) );
        Guard.IsInRangeFor( start + count, buffer, nameof(count) );

        for ( int x = start; x < start + count; x++ )
        {
            buffer.RemoveAt( x );
            Removed( x );
        }
    }


    protected internal bool InternalRemove( in TValue value )
    {
        ThrowIfReadOnly();
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }
    protected internal int InternalRemove( Func<TValue, bool> match )
    {
        ThrowIfReadOnly();
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
        ThrowIfReadOnly();
        buffer.Clear();
        Reset();
    }


    protected internal bool InternalRemoveAt( int index, [NotNullWhen( true )] out TValue? value )
    {
        ThrowIfReadOnly();

        if ( index < 0 || index >= Count )
        {
            value = default;
            return false;
        }

        value = buffer[index];
        buffer.RemoveAt( index );
        Removed( value, index );
        return true;
    }
    protected internal bool InternalTryAdd( in TValue value )
    {
        ThrowIfReadOnly();
        if ( buffer.Contains( value ) ) { return false; }

        InternalAdd( value );
        return true;
    }


    protected internal void InternalAdd( in TValue value )
    {
        ThrowIfReadOnly();
        buffer.Add( value );
        Added( value, Count - 1 );
    }
    protected internal void InternalAdd( in TValue value, int count )
    {
        ThrowIfReadOnly();
        EnsureCapacity( count );
        for ( int i = 0; i < count; i++ ) { InternalAdd( value ); }
    }
    protected internal void InternalAdd( IEnumerable<TValue> values )
    {
        ThrowIfReadOnly();
        buffer.AddRange( values );
        Reset();
    }
    protected internal void InternalAdd( scoped in ReadOnlySpan<TValue> values )
    {
        ThrowIfReadOnly();
        EnsureCapacity( values.Length );
        buffer.AddRange( values );
        Reset();
    }


    protected internal void InternalAddOrUpdate( in TValue value )
    {
        ThrowIfReadOnly();
        int index = buffer.IndexOf( value );

        if ( index >= 0 ) { InternalSet( index, value ); }
        else { InternalAdd( value ); }
    }


    protected internal void InternalSort( IComparer<TValue> compare )
    {
        ThrowIfReadOnly();
        CollectionsMarshal.AsSpan( buffer ).Sort( compare );
        Reset();
    }
    protected internal void InternalSort( Comparison<TValue> compare )
    {
        ThrowIfReadOnly();
        CollectionsMarshal.AsSpan( buffer ).Sort( compare );
        Reset();
    }
    protected internal void InternalSort( int start, int length, IComparer<TValue> compare )
    {
        ThrowIfReadOnly();
        CollectionsMarshal.AsSpan( buffer ).Slice( start, length ).Sort( compare );
        Reset();
    }


    protected internal int InternalRemove( IEnumerable<TValue> values )
    {
        ThrowIfReadOnly();
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
        ThrowIfReadOnly();
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
        ThrowIfReadOnly();
        buffer.Reverse();
        Reset();
    }
    protected internal void InternalReverse( int start, int length )
    {
        ThrowIfReadOnly();
        Guard.IsInRangeFor( start,          buffer, nameof(start) );
        Guard.IsInRangeFor( start + length, buffer, nameof(length) );
        buffer.Reverse( start, length );
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected void ThrowIfReadOnly()
    {
        if ( IsReadOnly ) { throw new NotSupportedException( "Collection is read-only." ); }
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
        ThrowIfReadOnly();

        TValue? old = index < buffer.Count && index >= 0
                          ? buffer[index]
                          : default;

        Guard.IsInRangeFor( index, buffer, nameof(index) );
        buffer[index] = value;
        Replaced( old, value, index );
    }


    public virtual TValue Get( int index )               => InternalGet( index );
    public virtual void   Set( int index, TValue value ) => InternalSet( index, value );


    public virtual bool Exists( Func<TValue, bool> match ) => FindIndex( match ) >= 0;


    public virtual int FindIndex( Func<TValue, bool> match, int start = 0 )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return FindIndex( match, start, Count - 1 );
    }
    public virtual int FindIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );

        for ( int i = start; i < endInclusive; i++ )
        {
            if ( match( buffer[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    public virtual int FindLastIndex( Func<TValue, bool> match, int start = 0 )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return FindLastIndex( match, Count - 1, start );
    }
    public virtual int FindLastIndex( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );

        for ( int i = start; i < endInclusive; i-- )
        {
            if ( match( buffer[i] ) ) { return i; }
        }

        return NOT_FOUND;
    }


    public virtual int IndexOf( TValue value ) => buffer.IndexOf( value );
    public virtual int IndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( value, start );
    }
    public virtual int IndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start,         buffer, nameof(start) );
        Guard.IsInRangeFor( start + count, buffer, nameof(count) );
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
        Guard.IsInRangeFor( start,         buffer, nameof(start) );
        Guard.IsInRangeFor( start + count, buffer, nameof(count) );
        return buffer.LastIndexOf( value, start, count );
    }


    public virtual int     FindCount( Func<TValue, bool> match )            => buffer.Count( match );
    public virtual TValue? Find( Func<TValue, bool>      match )            => Find( match, 0 );
    public virtual TValue? Find( Func<TValue, bool>      match, int start ) => Find( match, start, Count - 1 );
    public virtual TValue? Find( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsLessThanOrEqualTo( start, endInclusive );
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );
        return AsSpan( start, endInclusive - start ).FirstOrDefault( match );
    }
    public virtual TValue? FindLast( Func<TValue, bool> match )            => FindLast( match, 0 );
    public virtual TValue? FindLast( Func<TValue, bool> match, int start ) => FindLast( match, start, Count - 1 );
    public virtual TValue? FindLast( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsLessThanOrEqualTo( start, endInclusive );
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );

        return AsSpan( start, endInclusive - start ).LastOrDefault( match );
    }
    public virtual TValue[] FindAll( Func<TValue, bool> match )            => FindAll( match, 0 );
    public virtual TValue[] FindAll( Func<TValue, bool> match, int start ) => FindAll( match, start, Count - 1 );
    public virtual TValue[] FindAll( Func<TValue, bool> match, int start, int endInclusive )
    {
        Guard.IsLessThanOrEqualTo( start, endInclusive );
        Guard.IsInRangeFor( start,        buffer, nameof(start) );
        Guard.IsInRangeFor( endInclusive, buffer, nameof(endInclusive) );
        List<TValue>         list = new(Count);
        ReadOnlySpan<TValue> span = AsSpan( start, endInclusive - start );

        foreach ( TValue value in span )
        {
            if ( match( value ) ) { list.Add( value ); }
        }

        return list.ToArray();
    }


    public virtual bool TryAdd( TValue           value )            => InternalTryAdd( value );
    public virtual void Add( TValue              value )            => InternalAdd( value );
    public virtual void Add( TValue              value, int count ) => InternalAdd( value, count );
    public virtual void Add( params TValue[]     values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public virtual void Add( IEnumerable<TValue> values ) => InternalAdd( values );
    public virtual void Add( scoped in SpanEnumerable<TValue, EnumerableProducer<TValue>> values )
    {
        if ( values.KnownLength ) { EnsureCapacity( values.Length ); }

        foreach ( TValue value in values ) { InternalAdd( value ); }
    }
    public virtual void Add( scoped in ReadOnlySpan<TValue>   values ) => InternalAdd( values );
    public         void Add( scoped in ReadOnlyMemory<TValue> values ) => InternalAdd( values.Span );
    public         void Add( scoped in ImmutableArray<TValue> values ) => InternalAdd( values.AsSpan() );


    public virtual void AddOrUpdate( TValue value ) { InternalAddOrUpdate( value ); }
    public virtual void AddOrUpdate( IEnumerable<TValue> values )
    {
        foreach ( TValue value in values ) { InternalAddOrUpdate( value ); }
    }
    public void AddOrUpdate( scoped in ReadOnlyMemory<TValue> values ) => AddOrUpdate( values.Span );
    public void AddOrUpdate( scoped in ImmutableArray<TValue> values ) => AddOrUpdate( values.AsSpan() );
    public virtual void AddOrUpdate( scoped in ReadOnlySpan<TValue> values )
    {
        Sort();
        foreach ( TValue value in values ) { InternalAddOrUpdate( value ); }
    }
    public virtual void AddRange( TValue                      value, int count ) => InternalAdd( value, count );
    public virtual void AddRange( scoped ReadOnlySpan<TValue> values )     => InternalAdd( values );
    public virtual void AddRange( IEnumerable<TValue>         enumerable ) => InternalAdd( enumerable );


    public virtual void CopyTo( TValue[] array )                                                                  => buffer.CopyTo( array );
    public virtual void CopyTo( TValue[] array, int destinationStartIndex )                                       => buffer.CopyTo( array,            destinationStartIndex );
    public virtual void CopyTo( TValue[] array, int destinationStartIndex, int length, int sourceStartIndex = 0 ) => buffer.CopyTo( sourceStartIndex, array, length, destinationStartIndex );


    public virtual void Insert( int index, TValue                           value )            => InternalInsert( index, value );
    public virtual void Insert( int index, TValue                           value, int count ) => InternalInsert( index, value, count );
    public virtual void Insert( int index, IEnumerable<TValue>              collection ) => InternalInsert( index, collection );
    public virtual void Insert( int index, scoped in ReadOnlySpan<TValue>   values )     => InternalInsert( index, values );
    public virtual void Insert( int index, scoped in ReadOnlyMemory<TValue> collection ) => Insert( index, collection.Span );
    public virtual void Insert( int index, scoped in ImmutableArray<TValue> collection ) => Insert( index, collection.AsSpan() );


    public         void Replace( int     index, TValue                         value, int count = 1 ) => InternalReplace( index, value, count );
    public         void Replace( int     index, scoped in ReadOnlySpan<TValue> values ) => InternalReplace( index, values );
    public virtual void RemoveRange( int start, int                            count )  => InternalRemove( start, count );


    public virtual int  Remove( Func<TValue, bool>               match )                                          => InternalRemove( match );
    public virtual bool Remove( TValue                           value )                                          => InternalRemove( value );
    public virtual int  Remove( IEnumerable<TValue>              values )                                         => InternalRemove( values );
    public virtual int  Remove( scoped in ReadOnlySpan<TValue>   values )                                         => InternalRemove( values );
    public         int  Remove( scoped in ReadOnlyMemory<TValue> values )                                         => Remove( values.Span );
    public         int  Remove( scoped in ImmutableArray<TValue> values )                                         => Remove( values.AsSpan() );
    void IList<TValue>. RemoveAt( int                            index )                                          => RemoveAt( index );
    void IList.         RemoveAt( int                            index )                                          => RemoveAt( index );
    public virtual bool RemoveAt( int                            index )                                          => InternalRemoveAt( index, out _ );
    public virtual bool RemoveAt( int                            index, [NotNullWhen( true )] out TValue? value ) => InternalRemoveAt( index, out value );


    public virtual void Reverse()                       => InternalReverse();
    public virtual void Reverse( int start, int count ) => InternalReverse( start, count );


    public virtual void Sort()                                                                 => InternalSort( comparer );
    public virtual void Sort( IComparer<TValue>  compare )                                     => InternalSort( compare );
    public virtual void Sort( Comparison<TValue> compare )                                     => InternalSort( compare );
    public virtual void Sort( int                start, int count )                            => InternalSort( start, count, comparer );
    public virtual void Sort( int                start, int count, IComparer<TValue> compare ) => InternalSort( start, count, compare );


    void ICollection.CopyTo( Array array, int start )
    {
        if ( array is TValue[] values ) { CopyTo( values, start ); }
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
    int IList.IndexOf( object? value ) => value is TValue x
                                              ? buffer.IndexOf( x )
                                              : NOT_FOUND;
    void IList.Insert( int index, object? value )
    {
        if ( value is TValue x ) { buffer[index] = x; }
    }


    public virtual bool Contains( TValue                         value ) => buffer.Contains( value );
    public         bool Contains( scoped in ReadOnlySpan<TValue> value ) => AsSpan().Contains( value );
    public virtual void Clear()                                          => InternalClear();


    protected internal override FilterBuffer<TValue> FilteredValues()
    {
        ReadOnlySpan<TValue> span   = AsSpan();
        FilterBuffer<TValue> values = new(span.Length);

        foreach ( TValue value in span )
        {
            if ( Filter( value ) ) { values.Add( value ); }
        }

        return values;
    }


    /// <summary> Use With Caution -- Do not modify the <see cref="buffer"/> while the span is being used. </summary>
    public virtual ReadOnlySpan<TValue> AsSpan() => CollectionsMarshal.AsSpan( buffer );

    /// <summary> Use With Caution -- Do not modify the <see cref="buffer"/> while the span is being used. </summary>
    public virtual ReadOnlySpan<TValue> AsSpan( int start, int length ) => AsSpan().Slice( start, length );


    public virtual void EnsureCapacity( int capacity ) => buffer.EnsureCapacity( buffer.Count + capacity );
    public virtual void TrimExcess()                   => buffer.TrimExcess();
}
