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
    protected internal readonly List<TValue>      list;
    protected internal          IComparer<TValue> _comparer;


    public sealed override int Count          => list.Count;
    bool IList.                IsFixedSize    => ((IList)list).IsFixedSize;
    bool IList.                IsReadOnly     => ((IList)list).IsReadOnly;
    bool ICollection<TValue>.  IsReadOnly     => ((IList)list).IsReadOnly;
    bool ICollection.          IsSynchronized => ((IList)list).IsSynchronized;
    object? IList.this[ int index ] { get => ((IList)list)[index]; set => ((IList)list)[index] = value; }

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
    object ICollection.SyncRoot => ((IList)list).SyncRoot;


    public ObservableCollection() : this( Comparer<TValue>.Default ) { }
    public ObservableCollection( IComparer<TValue>    comparer ) : this( 16, comparer ) { }
    public ObservableCollection( int                  capacity ) : this( capacity, Comparer<TValue>.Default ) { }
    public ObservableCollection( int                  capacity, IComparer<TValue> comparer ) : this( new List<TValue>( capacity ), comparer ) { }
    public ObservableCollection( ReadOnlySpan<TValue> values ) : this( values, Comparer<TValue>.Default ) { }
    public ObservableCollection( ReadOnlySpan<TValue> values, IComparer<TValue> comparer ) : this( values.Length, comparer ) => InternalAdd( values );
    public ObservableCollection( IEnumerable<TValue>  values ) : this( values, Comparer<TValue>.Default ) { }
    public ObservableCollection( IEnumerable<TValue>  values, IComparer<TValue> comparer ) : this( new List<TValue>( values ), comparer ) { }
    private ObservableCollection( List<TValue>        values ) : this( values, Comparer<TValue>.Default ) { }
    protected internal ObservableCollection( List<TValue> values, IComparer<TValue> comparer )
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


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<TValue> AsSpan() => CollectionsMarshal.AsSpan( list );
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
    protected internal bool InternalRemove( TValue value )
    {
        bool result = list.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAdd( TValue value )
    {
        list.Add( value );
        Added( value );
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
        if ( index < 0 || index >= list.Count )
        {
            value = default;
            return false;
        }

        value = list[index];
        list.RemoveAt( index );
        Removed( value, index );
        return value is not null;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsertRange( int i, TValue value )
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


    public virtual bool Exists( Predicate<TValue> match ) => list.Exists( match );


    public virtual int FindIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)list, nameof(count) );
        return list.FindIndex( start, count, match );
    }
    public virtual int FindIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        return list.FindIndex( start, match );
    }
    public virtual int FindIndex( Predicate<TValue> match ) => list.FindIndex( match );


    public virtual int FindLastIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)list, nameof(count) );
        return list.FindLastIndex( start, count, match );
    }
    public virtual int FindLastIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        return list.FindLastIndex( start, match );
    }
    public virtual int FindLastIndex( Predicate<TValue> match ) => list.FindLastIndex( match );


    public virtual int IndexOf( TValue value ) => list.IndexOf( value );
    public virtual int IndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        return list.IndexOf( value, start );
    }
    public virtual int IndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        return list.IndexOf( value, start, count );
    }


    public virtual int LastIndexOf( TValue value ) => list.LastIndexOf( value );
    public virtual int LastIndexOf( TValue value, int start )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        return list.LastIndexOf( value, start );
    }
    public virtual int LastIndexOf( TValue value, int start, int count )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)list, nameof(count) );
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
    public virtual void Add( scoped in ReadOnlySpan<TValue>   values ) => InternalAdd( values );
    public virtual void Add( scoped in ReadOnlyMemory<TValue> values ) => InternalAdd( values.Span );
    public virtual void Add( scoped in ImmutableArray<TValue> values ) => InternalAdd( values.AsSpan() );


    public virtual void CopyTo( TValue[] array )                                            => list.CopyTo( array );
    public virtual void CopyTo( TValue[] array, int      arrayIndex )                       => list.CopyTo( array, arrayIndex );
    public virtual void CopyTo( int      index, TValue[] array, int arrayIndex, int count ) => list.CopyTo( index, array, arrayIndex, count );


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
    public virtual void InsertRange( int index, scoped in ReadOnlyMemory<TValue> collection ) => InsertRange( index, collection.Span );
    public virtual void InsertRange( int index, scoped in ImmutableArray<TValue> collection ) => InsertRange( index, collection.AsSpan() );


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

    public virtual int  Remove( Func<TValue, bool>             match )  => Remove( list.Where( match ) );
    public virtual int  Remove( IEnumerable<TValue>            values ) => InternalRemove( values );
    public virtual int  Remove( scoped in ReadOnlySpan<TValue> values ) => InternalRemove( values );
    public virtual bool Remove( TValue                         value )  => InternalRemove( value );


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
        if ( list.Count == 0 ) { return; }

        list.Sort( compare );
        Reset();
    }
    public virtual void Sort( int start, int count ) => Sort( start, count, _comparer );
    public virtual void Sort( int start, int count, IComparer<TValue> comparer )
    {
        if ( list.Count == 0 ) { return; }

        list.Sort( start, count, comparer );
        Reset();
    }


    void ICollection.CopyTo( Array     array, int start ) => ((IList)list).CopyTo( array, start );
    void IList.      Remove( object?   value )                => ((IList)list).Remove( value );
    int IList.       Add( object?      value )                => ((IList)list).Add( value );
    bool IList.      Contains( object? value )                => ((IList)list).Contains( value );
    int IList.       IndexOf( object?  value )                => ((IList)list).IndexOf( value );
    void IList.      Insert( int       index, object? value ) => ((IList)list).Insert( index, value );


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
