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
public class ConcurrentObservableCollection<T> : ObservableCollection<T>, IList, ILockedCollection<T, ConcurrentObservableCollection<T>.AsyncLockerEnumerator, ConcurrentObservableCollection<T>.LockerEnumerator>
{
    protected internal readonly Locker locker = Locker.Default;


    public AsyncLockerEnumerator AsyncValues { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }
    public Locker                Lock        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; init => locker = value; }
    object ICollection.          SyncRoot    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; }
    public LockerEnumerator      Values      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection() : base() { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( int                       capacity ) : base( capacity ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( IComparer<T>              comparer, int capacity = DEFAULT_CAPACITY ) : base( comparer, capacity ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( scoped in ReadOnlySpan<T> values ) : base( values ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( scoped in ReadOnlySpan<T> values, IComparer<T> comparer ) : base( values, comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( IEnumerable<T>            values ) : base( values ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ConcurrentObservableCollection( IEnumerable<T>            values, IComparer<T> comparer ) : base( values, comparer ) { }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected ConcurrentObservableCollection( MemoryBuffer<T>        values, IComparer<T> comparer ) : base( values, comparer ) { }


    public override void Dispose()
    {
        locker.Dispose();
        GC.SuppressFinalize( this );
    }

    public static implicit operator ConcurrentObservableCollection<T>( List<T>                                                values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( HashSet<T>                                             values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( ConcurrentBag<T>                                       values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( System.Collections.ObjectModel.ObservableCollection<T> values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( Collection<T>                                          values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( T[]                                                    values ) => new(new ReadOnlySpan<T>( values ));
    public static implicit operator ConcurrentObservableCollection<T>( ReadOnlyMemory<T>                                      values ) => new(values.Span);
    public static implicit operator ConcurrentObservableCollection<T>( ReadOnlySpan<T>                                        values ) => new(values);
    public static implicit operator ConcurrentObservableCollection<T>( MemoryBuffer<T>                                        values ) => new(values, Comparer<T>.Default);


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<T> AsSpan( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return buffer.Span; }
    }
#endif

    public override void Set( int index, T value )
    {
        using ( AcquireLock() )
        {
            T old = buffer[index];
            buffer[index] = value;
            Replaced( old, value, index );
        }
    }
    public override ref T Get( int index )
    {
        using ( AcquireLock() ) { return ref base.Get( index ); }
    }

    public override bool Exists( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.Exists( match ); }
    }
    public async ValueTask<bool> ExistsAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( match ) >= 0; }
    }


    public override int FindIndex( int start, int count, Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( start, count, match ); }
    }
    public override int FindIndex( int start, Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( start, match ); }
    }
    public override int FindIndex( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindIndex( match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, int count, Predicate<T> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return base.FindIndex( start, count, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Predicate<T> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return base.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindIndex( match ); }
    }


    public override int FindLastIndex( int start, int count, Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( start, count, match ); }
    }
    public override int FindLastIndex( int start, Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( start, match ); }
    }
    public override int FindLastIndex( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindLastIndex( match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( int start, int count, Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindLastIndex( start, count, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( int start, Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindLastIndex( start, match ); }
    }
    public async ValueTask<int> FindLastIndexAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.FindLastIndex( match ); }
    }


    public override int IndexOf( T value )
    {
        using ( AcquireLock() ) { return base.IndexOf( value ); }
    }
    public override int IndexOf( T value, int start )
    {
        using ( AcquireLock() ) { return base.IndexOf( value, start ); }
    }
    public override int IndexOf( T value, int start, int count )
    {
        using ( AcquireLock() ) { return base.IndexOf( value, start, count ); }
    }
    public async ValueTask<int> IndexOfAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( T value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( T value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return buffer.IndexOf( value, start, count ); }
    }


    public override int LastIndexOf( T value )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value ); }
    }
    public override int LastIndexOf( T value, int start )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value, start ); }
    }
    public override int LastIndexOf( T value, int start, int count )
    {
        using ( AcquireLock() ) { return base.LastIndexOf( value, start, count ); }
    }
    public async ValueTask<int> LastIndexOfAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( T value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return buffer.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( T value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, buffer, nameof(start) );
        Guard.IsInRangeFor( count, buffer, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return buffer.LastIndexOf( value, start, count ); }
    }


    public override List<T> FindAll( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindAll( match ); }
    }
    public async ValueTask<List<T>> FindAllAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.FindAll( match ); }
    }
    public override T? Find( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.Find( match ); }
    }
    public async ValueTask<T?> FindAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.Find( match ); }
    }
    public override T? FindLast( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.FindLast( match ); }
    }
    public async ValueTask<T?> FindLastAsync( Predicate<T> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.FindLast( match ); }
    }


    public override bool TryAdd( T value )
    {
        using ( AcquireLock() ) { return base.TryAdd( value ); }
    }
    public override void Add( params T[] values ) => Add( new ReadOnlySpan<T>( values ) );
    public override void Add( IEnumerable<T> values )
    {
        using ( AcquireLock() )
        {
            foreach ( T value in values ) { base.Add( value ); }
        }
    }
    public override void Add( SpanEnumerable<T, EnumerableProducer<T>> values )
    {
        using ( AcquireLock() )
        {
            foreach ( T value in values ) { base.Add( value ); }
        }
    }
    public override void Add( scoped in ReadOnlySpan<T> values )
    {
        using ( AcquireLock() ) { base.Add( values ); }
    }


    public virtual async ValueTask<bool> TryAddAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            if ( buffer.Contains( value ) ) { return false; }

            buffer.Add( value );
            Added( value );
            return true;
        }
    }
    public virtual async ValueTask AddAsync( ReadOnlyMemory<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Add( values.Span ); }
    }
    public virtual async ValueTask AddAsync( ImmutableArray<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Add( values.AsSpan() ); }
    }
    public virtual async ValueTask AddAsync( IEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            foreach ( T value in values )
            {
                buffer.Add( value );
                Added( value );
            }
        }
    }


    public override void CopyTo( T[] array )
    {
        using ( AcquireLock() ) { base.CopyTo( array ); }
    }
    public override void CopyTo( T[] array, int arrayIndex )
    {
        using ( AcquireLock() ) { base.CopyTo( array, arrayIndex ); }
    }
    public override void CopyTo( T[] array, int arrayIndex, int count )
    {
        using ( AcquireLock() ) { base.CopyTo( array, arrayIndex, count ); }
    }
    public async ValueTask CopyToAsync( T[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { buffer.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( T[] array, int arrayIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { buffer.CopyTo( array, arrayIndex ); }
    }
    public async ValueTask CopyToAsync( T[] array, int arrayIndex, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { buffer.CopyTo( array, arrayIndex, count ); }
    }


    public override void InsertRange( int index, IEnumerable<T> collection )
    {
        using ( AcquireLock() ) { base.InsertRange( index, collection ); }
    }
    public override void InsertRange( int index, scoped in ReadOnlySpan<T> collection )
    {
        using ( AcquireLock() ) { base.InsertRange( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, IEnumerable<T> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.InsertRange( index, collection ); }
    }
    public async ValueTask InsertRangeAsync( int index, ImmutableArray<T> collection, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.InsertRange( index, collection.AsSpan() ); }
    }
    public async ValueTask InsertRangeAsync( int index, ReadOnlyMemory<T> collection, CancellationToken token = default )
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


    public override int Remove( Predicate<T> match )
    {
        using ( AcquireLock() ) { return base.Remove( match ); }
    }
    public override int Remove( IEnumerable<T> values )
    {
        using ( AcquireLock() ) { return base.Remove( values ); }
    }
    public override bool Remove( T value )
    {
        using ( AcquireLock() ) { return base.Remove( value ); }
    }


    public virtual async ValueTask<bool> RemoveAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( value ); }
    }
    public virtual ValueTask<int> RemoveAsync( Func<T, bool> match, CancellationToken token = default ) => RemoveAsync( buffer.Where( match ), token );
    public virtual async ValueTask<int> RemoveAsync( IEnumerable<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( values ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ReadOnlyMemory<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( values.Span ); }
    }
    public virtual async ValueTask<int> RemoveAsync( ImmutableArray<T> values, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return base.Remove( values.AsSpan() ); }
    }


    public override void RemoveAt( int index )
    {
        using ( AcquireLock() ) { base.RemoveAt( index, out _ ); }
    }
    public override bool RemoveAt( int index, [NotNullWhen( true )] out T? value )
    {
        using ( AcquireLock() ) { return base.RemoveAt( index, out value ); }
    }
    public async ValueTask<T?> RemoveAtAsync( int index, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            return base.RemoveAt( index, out T? value )
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


    public override void Sort( IComparer<T> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( Comparison<T> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( int start, int count ) => Sort( start, count, comparer );
    public override void Sort( int start, int count, IComparer<T> compare )
    {
        using ( AcquireLock() ) { base.Sort( start, count, compare ); }
    }
    public ValueTask SortAsync( CancellationToken token = default ) => SortAsync( comparer, token );
    public async ValueTask SortAsync( IComparer<T> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( compare ); }
    }
    public virtual async ValueTask SortAsync( Comparison<T> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( compare ); }
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<T> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( start, count, compare ); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() )
        {
            if ( array is T[] x ) { buffer.CopyTo( x, start ); }
        }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is T x ) { buffer.Remove( x ); }
        }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is not T x ) { return -1; }

            buffer.Add( x );
            return buffer.Length;
        }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return value is T x && buffer.Contains( x ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() )
        {
            return value is T x
                       ? buffer.IndexOf( x )
                       : -1;
        }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() )
        {
            if ( value is T x ) { buffer.Insert( index, x ); }
        }
    }


    public override bool Contains( T value )
    {
        using ( AcquireLock() ) { return buffer.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return buffer.Contains( value ); }
    }


    public override void Add( T value )
    {
        using ( AcquireLock() ) { base.Add( value ); }
    }
    public virtual async ValueTask AddAsync( T value, CancellationToken token = default )
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


    public override void Insert( int index, T value )
    {
        using ( AcquireLock() ) { base.Insert( index, value ); }
    }
    public async ValueTask InsertAsync( int index, T value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Insert( index, value ); }
    }


    public          IAsyncEnumerator<T> GetAsyncEnumerator( CancellationToken token ) => AsyncValues.GetAsyncEnumerator( token );
    public override IEnumerator<T>      GetEnumerator()                               => Values;
    IEnumerator IEnumerable.            GetEnumerator()                               => GetEnumerator();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock()                                                                                     => locker.Enter();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( scoped in CancellationToken token )                                                  => locker.Enter( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Closer            AcquireLock( ref       bool              lockTaken, scoped in CancellationToken token = default ) => locker.Enter( ref lockTaken, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask<Closer> AcquireLockAsync( CancellationToken      token ) => locker.EnterAsync( token );


    protected internal ReadOnlyMemory<T> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    ReadOnlyMemory<T> ILockedCollection<T>.                              Copy()                               => Copy();
    ConfiguredValueTaskAwaitable<ReadOnlyMemory<T>> ILockedCollection<T>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );
    protected async ValueTask<ReadOnlyMemory<T>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ) ) { return FilteredValues(); }
    }



    public sealed class AsyncLockerEnumerator( ConcurrentObservableCollection<T> collection ) : AsyncLockerEnumerator<T>( collection );



    public sealed class LockerEnumerator( ConcurrentObservableCollection<T> collection ) : LockerEnumerator<T>( collection );
}
