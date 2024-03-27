// Jakar.Extensions :: Jakar.Extensions
// 3/25/2024  15:41

using System;



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
    protected internal readonly IComparer<TValue>    comparer;
    protected internal readonly MemoryBuffer<TValue> buffer;


    public sealed override int Count          { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Length; }
    bool IList.                IsFixedSize    { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => buffer.IsReadOnly; }
    bool IList.                IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => buffer.IsReadOnly; }
    bool ICollection<TValue>.  IsReadOnly     { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => buffer.IsReadOnly; }
    bool ICollection.          IsSynchronized { [MethodImpl(       MethodImplOptions.AggressiveInlining )] get => false; }
    object? IList.this[ int index ] { [MethodImpl(                 MethodImplOptions.AggressiveInlining )] get => buffer[index]; set => buffer[index] = (TValue)value!; }
    public TValue this[ int index ] { [MethodImpl(                 MethodImplOptions.AggressiveInlining )] get => Get( index ); set => Set( index, value ); }
    object ICollection.SyncRoot { [MethodImpl(                     MethodImplOptions.AggressiveInlining )] get => buffer; }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection() : this( Comparer<TValue>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( scoped in ReadOnlySpan<TValue> values ) : this( values, Comparer<TValue>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( scoped in ReadOnlySpan<TValue> values, IComparer<TValue> comparer ) : this( comparer, values.Length ) => InternalAdd( values );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( IEnumerable<TValue>            values ) : this( values, Comparer<TValue>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( IEnumerable<TValue>            values, IComparer<TValue> comparer ) : this( comparer ) => Add( values );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( TValue[]                       values ) : this( values, Comparer<TValue>.Default ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( TValue[]                       values, IComparer<TValue> comparer ) : this( values.AsSpan(), comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( int                            capacity ) : this( Comparer<TValue>.Default, capacity ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ObservableCollection( IComparer<TValue>              comparer, int capacity = DEFAULT_CAPACITY ) : this( new MemoryBuffer<TValue>( capacity ), comparer ) { }
    protected internal ObservableCollection( MemoryBuffer<TValue> values, IComparer<TValue> comparer )
    {
        this.comparer = comparer;
        buffer        = values;
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

    public TValue[] ToArray() => buffer.ToArray();

#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<TValue> AsSpan() => buffer.Span;
#endif


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsert( int index, in TValue value )
    {
        buffer.Insert( index, value );
        Added( value );
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
    protected internal bool InternalRemove( in TValue value )
    {
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalClear()
    {
        buffer.Clear();
        Reset();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalRemoveAt( int index, [NotNullWhen( true )] out TValue? value )
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalInsertRange( int i, in TValue value )
    {
        buffer.Insert( i, value );
        Added( value, i );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal bool InternalTryAdd( in TValue value )
    {
        if ( buffer.Contains( value ) ) { return false; }

        InternalAdd( value );
        return true;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAdd( in TValue value )
    {
        buffer.Add( value );
        Added( value );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected internal void InternalAdd( scoped in ReadOnlySpan<TValue> values )
    {
        foreach ( TValue value in values ) { InternalAdd( value ); }
    }


    public virtual ref TValue Get( int index ) => ref buffer[index];
    public virtual void Set( int index, TValue value )
    {
        TValue old = buffer[index];
        buffer[index] = value;
        Replaced( old, value, index );
    }


    public virtual bool Exists( Predicate<TValue> match ) => buffer.IndexOf( match ) >= 0;
    public virtual int FindCount( Predicate<TValue> match )
    {
        int length = 0;
        foreach ( TValue _ in buffer.Span.Where( match ) ) { length++; }

        return length;
    }


    public virtual int FindIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        return buffer.IndexOf( match, start, count );
    }
    public virtual int FindIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.IndexOf( match, start );
    }
    public virtual int FindIndex( Predicate<TValue> match ) => buffer.IndexOf( match );


    public virtual int FindLastIndex( int start, int count, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        return buffer.LastIndexOf( match, start, count );
    }
    public virtual int FindLastIndex( int start, Predicate<TValue> match )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        return buffer.LastIndexOf( match, start );
    }
    public virtual int FindLastIndex( Predicate<TValue> match ) => buffer.LastIndexOf( match );


    public virtual int IndexOf( TValue value ) => buffer.IndexOf( value );
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


    public virtual List<TValue> FindAll( Predicate<TValue>  match ) => buffer.FindAll( match );
    public virtual TValue?      Find( Predicate<TValue>     match ) => buffer.Find( match );
    public virtual TValue?      FindLast( Predicate<TValue> match ) => buffer.FindLast( match );


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


    public virtual void CopyTo( TValue[] array )                            => buffer.CopyTo( array );
    public virtual void CopyTo( TValue[] array, int arrayIndex )            => buffer.CopyTo( array, arrayIndex );
    public virtual void CopyTo( TValue[] array, int arrayIndex, int count ) => buffer.CopyTo( array, arrayIndex, count );


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

        foreach ( TValue value in buffer.Span.Where( match ) )
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
        buffer.Reverse();
        Reset();
    }
    public virtual void Reverse( int start, int count )
    {
        buffer.Reverse( start, count );
        Reset();
    }


    public void Sort() => Sort( comparer );
    public virtual void Sort( IComparer<TValue> compare )
    {
    #if NET6_0_OR_GREATER
        if ( buffer.Length is 0 ) { return; }

        buffer.Span.Sort( compare );
        Reset();
    #else
        Sort( compare.Compare );
    #endif
    }
    public virtual void Sort( Comparison<TValue> compare )
    {
        if ( buffer.Length is 0 ) { return; }

        buffer.Span.Sort( compare );
        Reset();
    }
    public virtual void Sort( int start, int count ) => Sort( start, count, comparer );
    public virtual void Sort( int start, int count, IComparer<TValue> compare )
    {
        if ( buffer.Length is 0 ) { return; }

    #if NET6_0_OR_GREATER
        buffer.Span.Slice( start, count ).Sort( compare );
    #else
        buffer.Span.Slice( start, count ).Sort( compare.Compare );
    #endif

        Reset();
    }


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
        if ( value is not TValue x ) { return -1; }

        buffer.Add( x );
        return buffer.Length;
    }
    bool IList.Contains( object? value ) => value is TValue x && buffer.Contains( x );
    int IList.IndexOf( object? value ) =>
        value is TValue x
            ? buffer.IndexOf( x )
            : -1;
    void IList.Insert( int index, object? value )
    {
        if ( value is TValue x ) { buffer[index] = x; }
    }


    public virtual bool Contains( TValue value )               => buffer.Contains( value );
    public virtual void Add( TValue      value )               => InternalAdd( value );
    public virtual void Insert( int      index, TValue value ) => InternalInsert( index, value );
    public virtual void Clear() => InternalClear();


    protected internal override ReadOnlyMemory<TValue> FilteredValues()
    {
        using Buffer<TValue> buffer = new(Count);
        foreach ( TValue value in this.buffer.Span.Where( Filter ) ) { buffer.Add( value ); }

        return buffer.ToArray();
    }
}
