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
public class ObservableCollection<TValue> : CollectionAlerts<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList, IDisposable
{
    protected internal readonly MemoryBuffer<TValue> list;
    protected internal readonly IComparer<TValue>    _comparer;


    public sealed override int Count          { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => list.Length; }
    bool IList.                IsFixedSize    => list.IsReadOnly;
    bool IList.                IsReadOnly     => list.IsReadOnly;
    bool ICollection<TValue>.  IsReadOnly     => list.IsReadOnly;
    bool ICollection.          IsSynchronized => false;
    object? IList.this[ int index ] { get => list[index]; set => list[index] = (TValue)value!; }
    object ICollection.SyncRoot => list;

    public virtual TValue this[ int index ]
    {
        get => list[index];
        set
        {
            TValue old = list[index];
            list[index] = value;
            Replaced( old, value, index );
        }
    }


    public ObservableCollection() : this( Comparer<TValue>.Default ) { }
    public ObservableCollection( IComparer<TValue>              comparer ) : this( 16, comparer ) { }
    public ObservableCollection( int                            capacity ) : this( capacity, Comparer<TValue>.Default ) { }
    public ObservableCollection( int                            capacity, IComparer<TValue> comparer ) : this( new MemoryBuffer<TValue>( capacity ), comparer ) { }
    public ObservableCollection( scoped in ReadOnlySpan<TValue> values ) : this( values, Comparer<TValue>.Default ) { }
    public ObservableCollection( scoped in ReadOnlySpan<TValue> values, IComparer<TValue> comparer ) : this( new MemoryBuffer<TValue>( values ), comparer ) { }
    public ObservableCollection( IEnumerable<TValue>            values ) : this( values, Comparer<TValue>.Default ) { }
    public ObservableCollection( IEnumerable<TValue>            values, IComparer<TValue> comparer ) : this( new MemoryBuffer<TValue>( values ), comparer ) { }
    protected internal ObservableCollection( MemoryBuffer<TValue> values, IComparer<TValue> comparer )
    {
        _comparer = comparer;
        list      = values;
    }


    public virtual void Dispose() => GC.SuppressFinalize( this );


    public static implicit operator ObservableCollection<TValue>( List<TValue>                                                values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( HashSet<TValue>                                             values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( ConcurrentBag<TValue>                                       values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( Collection<TValue>                                          values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( System.Collections.ObjectModel.ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( TValue[]                                                    values ) => new(new ReadOnlySpan<TValue>( values ));
    public static implicit operator ObservableCollection<TValue>( ReadOnlyMemory<TValue>                                      values ) => new(values.Span);
    public static implicit operator ObservableCollection<TValue>( ReadOnlySpan<TValue>                                        values ) => new(values);
    public static implicit operator ObservableCollection<TValue>( MemoryBuffer<TValue>                                        values ) => new(values, Comparer<TValue>.Default);


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<TValue> AsSpan() => list.Span;
#endif


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalRemoveRange( int start, int count )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        Guard.IsInRangeFor( count, list, nameof(count) );

        for ( int x = start; x < start + count; x++ )
        {
            list.RemoveAt( x );
            Removed( x );
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalRemove( in TValue value )
    {
        bool result = list.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalClear()
    {
        list.Clear();
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalRemoveAt( int index, [NotNullWhen( true )] out TValue? value )
    {
        if ( index < 0 || index >= list.Length )
        {
            value = default;
            return false;
        }

        value = list[index];
        if ( list.RemoveAt( index ) ) { Removed( value, index ); }

        return value is not null;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsertRange( int i, in TValue value )
    {
        list.Insert( i, value );
        Added( value, i );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalTryAdd( in TValue value )
    {
        if ( list.Contains( value ) ) { return false; }

        InternalAdd( value );
        return true;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAdd( in TValue value )
    {
        list.Add( value );
        Added( value );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAdd( scoped in ReadOnlySpan<TValue> values )
    {
        foreach ( TValue value in values ) { InternalAdd( value ); }
    }


    public virtual bool Exists( Predicate<TValue> match ) => list.IndexOf( match ) >= 0;
    public virtual int FindCount( Predicate<TValue> match )
    {
        int length = 0;
        foreach ( TValue _ in WhereExtensions.Where( list.Span, match ) ) { length++; }

        return length;
    }


    public virtual int FindIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        Guard.IsInRangeFor( count, list, nameof(count) );
        return list.IndexOf( match, start, count );
    }
    public virtual int FindIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        return list.IndexOf( match, start );
    }
    public virtual int FindIndex( Predicate<TValue> match ) => list.IndexOf( match );


    public virtual int FindLastIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        Guard.IsInRangeFor( count, list, nameof(count) );
        return list.LastIndexOf( match, start, count );
    }
    public virtual int FindLastIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        return list.LastIndexOf( match, start );
    }
    public virtual int FindLastIndex( Predicate<TValue> match ) => list.LastIndexOf( match );


    public virtual int IndexOf( TValue value ) => list.IndexOf( value );
    public virtual int IndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        return list.IndexOf( value, start );
    }
    public virtual int IndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        return list.IndexOf( value, start, count );
    }


    public virtual int LastIndexOf( TValue value ) => list.LastIndexOf( value );
    public virtual int LastIndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        return list.LastIndexOf( value, start );
    }
    public virtual int LastIndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, list, nameof(start) );
        Guard.IsInRangeFor( count, list, nameof(count) );
        return list.LastIndexOf( value, start, count );
    }


    public virtual List<TValue> FindAll( Predicate<TValue>  match ) => list.FindAll( match );
    public virtual TValue?      Find( Predicate<TValue>     match ) => list.Find( match );
    public virtual TValue?      FindLast( Predicate<TValue> match ) => list.FindLast( match );


    public virtual bool TryAdd( TValue       value )  => InternalTryAdd( value );
    public virtual void Add( params TValue[] values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public virtual void Add( IEnumerable<TValue> values )
    {
        foreach ( TValue value in values ) { InternalAdd( value ); }
    }
    public virtual void Add( SpanEnumerable<TValue, EnumerableProducer<TValue>> values )
    {
        foreach ( TValue value in values ) { InternalAdd( value ); }
    }
    public virtual void Add( scoped in ReadOnlySpan<TValue>   values ) => InternalAdd( values );
    public         void Add( scoped in ReadOnlyMemory<TValue> values ) => InternalAdd( values.Span );
    public         void Add( scoped in ImmutableArray<TValue> values ) => InternalAdd( values.AsSpan() );


    public virtual void CopyTo( TValue[] array )                            => list.CopyTo( array );
    public virtual void CopyTo( TValue[] array, int arrayIndex )            => list.CopyTo( array, arrayIndex );
    public virtual void CopyTo( TValue[] array, int arrayIndex, int count ) => list.CopyTo( array, arrayIndex, count );


    protected internal void InternalInsertRange( int index, IEnumerable<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsertRange( i, value ); }
    }
    protected internal void InternalInsertRange( int index, scoped in ReadOnlySpan<TValue> collection )
    {
        foreach ( (int i, TValue? value) in collection.Enumerate( index ) ) { InternalInsertRange( i, value ); }
    }

    public virtual void InsertRange( int index, IEnumerable<TValue>              collection ) => InternalInsertRange( index, collection );
    public virtual void InsertRange( int index, scoped in ReadOnlySpan<TValue>   collection ) => InternalInsertRange( index, collection );
    public         void InsertRange( int index, scoped in ReadOnlyMemory<TValue> collection ) => InsertRange( index, collection.Span );
    public         void InsertRange( int index, scoped in ImmutableArray<TValue> collection ) => InsertRange( index, collection.AsSpan() );


    public virtual void RemoveRange( int start, int count ) => InternalRemoveRange( start, count );


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


    public virtual int Remove( Predicate<TValue> match )
    {
        int count = 0;

        foreach ( TValue value in WhereExtensions.Where( list.Span, match ) )
        {
            if ( Remove( value ) ) { count++; }
        }

        return count;
    }
    public virtual bool Remove( TValue                           value )  => InternalRemove( value );
    public virtual int  Remove( IEnumerable<TValue>              values ) => InternalRemove( values );
    public virtual int  Remove( scoped in ReadOnlySpan<TValue>   values ) => InternalRemove( values );
    public         int  Remove( scoped in ReadOnlyMemory<TValue> values ) => Remove( values.Span );
    public         int  Remove( scoped in ImmutableArray<TValue> values ) => Remove( values.AsSpan() );


    public virtual void RemoveAt( int index )                                          => InternalRemoveAt( index, out _ );
    public virtual bool RemoveAt( int index, [NotNullWhen( true )] out TValue? value ) => InternalRemoveAt( index, out value );


    public virtual void Reverse()
    {
        list.Reverse();
        Reset();
    }
    public virtual void Reverse( int start, int count )
    {
        list.Reverse( start, count );
        Reset();
    }


    public virtual void Sort()                             => Sort( _comparer );
    public virtual void Sort( IComparer<TValue> comparer ) => Sort( comparer.Compare );
    public virtual void Sort( Comparison<TValue> compare )
    {
        if ( list.Length is 0 ) { return; }

        list.Span.Sort( compare );
        Reset();
    }
    public virtual void Sort( int start, int count ) => Sort( start, count, _comparer );
    public virtual void Sort( int start, int count, IComparer<TValue> comparer )
    {
        if ( list.Length is 0 ) { return; }

        list.Span.Slice( start, count ).Sort( comparer.Compare );
        Reset();
    }


    void ICollection.CopyTo( Array array, int start )
    {
        if ( array is TValue[] values ) { list.CopyTo( values, start ); }
    }
    void IList.Remove( object? value )
    {
        if ( value is TValue x ) { list.Remove( x ); }
    }
    int IList.Add( object? value )
    {
        if ( value is not TValue x ) { return -1; }

        list.Add( x );
        return list.Length;
    }
    bool IList.Contains( object? value ) => value is TValue x && list.Contains( x );
    int IList.IndexOf( object? value ) => value is TValue x
                                              ? list.IndexOf( x )
                                              : -1;
    void IList.Insert( int index, object? value )
    {
        if ( value is TValue x ) { list[index] = x; }
    }


    public virtual bool Contains( TValue value ) => list.Contains( value );


    public virtual void Add( TValue value ) => InternalAdd( value );


    public virtual void Clear() => InternalClear();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsert( int index, TValue value )
    {
        list.Insert( index, value );
        Added( value );
    }


    public virtual void Insert( int index, TValue value ) => InternalInsert( index, value );


    protected internal override ReadOnlyMemory<TValue> FilteredValues() => list.Where( Filter ).ToArray();
}
