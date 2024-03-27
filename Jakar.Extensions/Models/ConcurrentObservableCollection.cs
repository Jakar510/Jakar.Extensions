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
public class ConcurrentObservableCollection<TValue> : ObservableCollection<TValue>, IList, ILockedCollection<TValue, ConcurrentObservableCollection<TValue>.AsyncLockerEnumerator, ConcurrentObservableCollection<TValue>.LockerEnumerator>
{
    protected internal readonly Locker locker = Locker.Default;


    public AsyncLockerEnumerator AsyncValues { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }
    public Locker           Lock     { [MethodImpl(         MethodImplOptions.AggressiveInlining )] get => locker; init => locker = value; }
    object ICollection.     SyncRoot { [MethodImpl(         MethodImplOptions.AggressiveInlining )] get => locker; }
    public LockerEnumerator Values   { [MethodImpl(         MethodImplOptions.AggressiveInlining )] get => new(this); }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection() : base() { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( IComparer<TValue>              comparer ) : base( comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( int                            capacity ) : base( capacity ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( int                            capacity, IComparer<TValue> comparer ) : base( capacity, comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( scoped in ReadOnlySpan<TValue> values ) : base( values ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( scoped in ReadOnlySpan<TValue> values, IComparer<TValue> comparer ) : base( values, comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( IEnumerable<TValue>            values ) : base( values ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( IEnumerable<TValue>            values, IComparer<TValue> comparer ) : base( values, comparer ) { }
    protected ConcurrentObservableCollection( MemoryBuffer<TValue>                                                             values, IComparer<TValue> comparer ) : base( values, comparer ) { }


    public override void Dispose()
    {
        locker.Dispose();
        GC.SuppressFinalize( this );
    }

    public static implicit operator ConcurrentObservableCollection<TValue>( List<TValue>                                                values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( HashSet<TValue>                                             values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( ConcurrentBag<TValue>                                       values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( System.Collections.ObjectModel.ObservableCollection<TValue> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( Collection<TValue>                                          values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( TValue[]                                                    values ) => new(new ReadOnlySpan<TValue>( values ));
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlyMemory<TValue>                                      values ) => new(values.Span);
    public static implicit operator ConcurrentObservableCollection<TValue>( ReadOnlySpan<TValue>                                        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<TValue>( MemoryBuffer<TValue>                                        values ) => new(values, Comparer<TValue>.Default);


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<TValue> AsSpan( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return buffer.Span; }
    }
#endif

    public override void Set( int index, TValue value )
    {
        using ( AcquireLock() )
        {
            TValue old = buffer[index];
            buffer[index] = value;
            Replaced( old, value, index );
        }
    }
    public override ref TValue Get( int index )
    {
        using ( AcquireLock() ) { return ref base.Get( index ); }
    }

    public override bool Exists( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.Exists( match ); }
    }
    public async ValueTask<bool> ExistsAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( match ) >= 0; }
    }


    public override int FindIndex( int start, int count, Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( start, count, match ); }
    }
    public override int FindIndex( int start, Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( start, match ); }
    }
    public override int FindIndex( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return base.FindIndex( start, count, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return base.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindIndex( match ); }
    }


    public override int FindLastIndex( int start, int count, Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( start, count, match ); }
    }
    public override int FindLastIndex( int start, Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( start, match ); }
    }
    public override int FindLastIndex( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( int start, int count, Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindLastIndex( start, count, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( int start, Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindLastIndex( match ); }
    }


    public override int IndexOf( TValue value )
    {
        using ( AcquireLock() ) { return base.IndexOf( value ); }
    }
    public override int IndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return base.IndexOf( value, start ); }
    }
    public override int IndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return base.IndexOf( value, start, count ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( value, start, count ); }
    }


    public override int LastIndexOf( TValue value )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value ); }
    }
    public override int LastIndexOf( TValue value, int start )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value, start ); }
    }
    public override int LastIndexOf( TValue value, int start, int count )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value, start, count ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return buffer.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return buffer.LastIndexOf( value, start, count ); }
    }


    public override List<TValue> FindAll( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindAll( match ); }
    }
    public async ValueTask<List<TValue>> FindAllAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.FindAll( match ); }
    }
    public override TValue? Find( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.Find( match ); }
    }
    public async ValueTask<TValue?> FindAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.Find( match ); }
    }
    public override TValue? FindLast( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindLast( match ); }
    }
    public async ValueTask<TValue?> FindLastAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.FindLast( match ); }
    }


    public override bool TryAdd( TValue value )
    {
        using ( AcquireLock() ) { return base.TryAdd( value ); }
    }
    public override void Add( params TValue[] values ) => Add( new ReadOnlySpan<TValue>( values ) );
    public override void Add( IEnumerable<TValue> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { base.Add( value ); }
        }
    }
    public override void Add( SpanEnumerable<TValue, EnumerableProducer<TValue>> values )
    {
        using ( AcquireLock() )
        {
            foreach ( TValue value in values ) { base.Add( value ); }
        }
    }
    public override void Add( scoped in ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { base.Add( values ); }
    }


    public virtual async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            if ( buffer.Contains( value ) ) { return false; }

            buffer.Add( value );
            Added( value );
            return true;
        }
    }
    public virtual async ValueTask AddAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Add( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Add( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            foreach ( TValue value in values )
            {
                buffer.Add( value );
                Added( value );
            }
        }
    }


    public override void CopyTo( TValue[] array )
    {
        using ( AcquireLock() ) { base.CopyTo( array ); }
    }
    public override void CopyTo( TValue[] array, int arrayIndex )
    {
        using ( AcquireLock() ) { base.CopyTo( array, arrayIndex ); }
    }
    public override void CopyTo( TValue[] array, int arrayIndex, int count )
    {
        using ( AcquireLock() ) { base.CopyTo( array, arrayIndex, count ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { buffer.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int arrayIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { buffer.CopyTo( array, arrayIndex ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int arrayIndex, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { buffer.CopyTo( array, arrayIndex, count ); }
    }


    public override void InsertRange( int index, IEnumerable<TValue> collection )
    {
        using ( AcquireLock() ) { base.InsertRange( index, collection ); }
    }
    public override void InsertRange( int index, scoped in ReadOnlySpan<TValue> collection )
    {
        using ( AcquireLock() ) { base.InsertRange( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.InsertRange( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.InsertRange( index, collection.AsSpan() ); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<TValue> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.InsertRange( index, collection.Span ); }
    }


    public override void RemoveRange( int start, int count )
    {
        using ( AcquireLock() ) { base.RemoveRange( start, count ); }
    }
    public async ValueTask RemoveRangeAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.RemoveRange( start, count ); }
    }


    public override int Remove( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.Remove( match ); }
    }
    public override int Remove( IEnumerable<TValue> values )
    {
        using ( AcquireLock() ) { return base.Remove( values ); }
    }
    public override bool Remove( TValue value )
    {
        using ( AcquireLock() ) { return base.Remove( value ); }
    }


    public virtual async ValueTask<bool> RemoveAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( value ); }
    }
    public virtual ValueTask<int> RemoveAsync( Func<TValue, bool> match, CancellationToken token = default ) => RemoveAsync( buffer.Where( match ), token );
    public virtual async ValueTask<int> RemoveAsync( IEnumerable<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( values ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ReadOnlyMemory<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( values.Span ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ImmutableArray<TValue> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( values.AsSpan() ); }
    }


    public override void RemoveAt( int index )
    {
        using ( AcquireLock() ) { base.RemoveAt( index, out _ ); }
    }
    public override bool RemoveAt( int index, [NotNullWhen( true )] out TValue? value )
    {
        using ( AcquireLock() ) { return base.RemoveAt( index, out value ); }
    }
    public async ValueTask<TValue?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            return base.RemoveAt( index, out TValue? value )
                       ? value
                       : default;
        }
    }


    public override void Reverse()
    {
        using ( AcquireLock() ) { base.Reverse(); }
    }
    public override void Reverse( int start, int count )
    {
        using ( AcquireLock() ) { base.Reverse( start, count ); }
    }
    public async ValueTask ReverseAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Reverse(); }
    }
    public async ValueTask ReverseAsync( int start, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Reverse( start, count ); }
    }


    public override void Sort( IComparer<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( Comparison<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( int start, int count ) => Sort( start, count, comparer );
    public override void Sort( int start, int count, IComparer<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( start, count, compare ); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync( comparer, token );
    public async ValueTask SortAsync( IComparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( compare ); }
    }
    public virtual async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( compare ); }
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( start, count, compare ); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() )
        {
            if ( array is TValue[] x ) { buffer.CopyTo( x, start ); }
        }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is TValue x ) { buffer.Remove( x ); }
        }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is not TValue x ) { return -1; }

            buffer.Add( x );
            return buffer.Length;
        }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return value is TValue x && buffer.Contains( x ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() )
        {
            return value is TValue x
                       ? buffer.IndexOf( x )
                       : -1;
        }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is TValue x ) { buffer.Insert( index, x ); }
        }
    }


    public override bool Contains( TValue value )
    {
        using ( AcquireLock() ) { return buffer.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.Contains( value ); }
    }


    public override void Add( TValue value )
    {
        using ( AcquireLock() ) { base.Add( value ); }
    }
    public virtual async ValueTask AddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Add( value ); }
    }


    public override void Clear()
    {
        using ( AcquireLock() ) { base.Clear(); }
    }
    public virtual async ValueTask ClearAsync( CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Clear(); }
    }


    public override void Insert( int index, TValue value )
    {
        using ( AcquireLock() ) { base.Insert( index, value ); }
    }
    public async ValueTask InsertAsync( int index, TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Insert( index, value ); }
    }


    public          IAsyncEnumerator<TValue> GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator( token );
    public override IEnumerator<TValue>      GetEnumerator()                               => Values;
    IEnumerator IEnumerable.                 GetEnumerator()                               => GetEnumerator();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock()                                                                                     => locker.Enter();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( scoped in CancellationToken token )                                                  => locker.Enter( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( ref       bool              lockTaken, scoped in CancellationToken token = default ) => locker.Enter( ref lockTaken, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask<Closer> AcquireLockAsync( CancellationToken      token ) => locker.EnterAsync( token );


    protected internal ReadOnlyMemory<TValue> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    ReadOnlyMemory<TValue> ILockedCollection<TValue>.                              Copy()                               => Copy();
    ConfiguredValueTaskAwaitable<ReadOnlyMemory<TValue>> ILockedCollection<TValue>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );
    protected async ValueTask<ReadOnlyMemory<TValue>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ) ) { return FilteredValues(); }
    }



    public sealed class AsyncLockerEnumerator( ConcurrentObservableCollection<TValue> collection ) : AsyncLockerEnumerator<TValue>( collection );



    public sealed class LockerEnumerator( ConcurrentObservableCollection<TValue> collection ) : LockerEnumerator<TValue>( collection );
}
