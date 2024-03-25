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
public class ConcurrentObservableCollection<TValue> : ObservableCollection<TValue>, ILockedCollection<TValue>, IAsyncEnumerable<TValue>, IList<TValue>, IReadOnlyList<TValue>, IList, IDisposable
{
    protected internal readonly Locker locker = Locker.Default;


    public AsyncLockerEnumerator AsyncValues { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }

    bool IList.IsFixedSize
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)list).IsFixedSize; }
        }
    }

    bool IList.IsReadOnly
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)list).IsReadOnly; }
        }
    }

    bool ICollection<TValue>.IsReadOnly
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)list).IsReadOnly; }
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)list).IsSynchronized; }
        }
    }

    object? IList.this[ int index ]
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)list)[index]; }
        }
        set
        {
            using ( AcquireLock() ) { ((IList)list)[index] = value; }
        }
    }

    public override TValue this[ int index ]
    {
        get
        {
            using ( AcquireLock() ) { return list[index]; }
        }
        set
        {
            using ( AcquireLock() )
            {
                TValue old = list[index];
                list[index] = value;
                Replaced( old, value, index );
            }
        }
    }

    public Locker Lock { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => locker; init => locker = value; }

    object ICollection.SyncRoot
    {
        get
        {
            using ( AcquireLock() ) { return ((IList)list).SyncRoot; }
        }
    }

    public LockerEnumerator<TValue> Values { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(this); }


    public ConcurrentObservableCollection() : this( Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IComparer<TValue>    comparer ) : this( 64, comparer ) { }
    public ConcurrentObservableCollection( int                  capacity ) : this( capacity, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( int                  capacity, IComparer<TValue> comparer ) : this( new List<TValue>( capacity ), comparer ) { }
    public ConcurrentObservableCollection( ReadOnlySpan<TValue> values ) : this( values, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( ReadOnlySpan<TValue> values, IComparer<TValue> comparer ) : this( values.Length, comparer ) => base.Add( values );
    public ConcurrentObservableCollection( IEnumerable<TValue>  values ) : this( values, Comparer<TValue>.Default ) { }
    public ConcurrentObservableCollection( IEnumerable<TValue>  values, IComparer<TValue> comparer ) : this( new List<TValue>( values ), comparer ) { }
    private ConcurrentObservableCollection( List<TValue>        values ) : this( values, Comparer<TValue>.Default ) { }
    protected ConcurrentObservableCollection( List<TValue>      values, IComparer<TValue> comparer ) : base( values, comparer ) { }


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


#if NET6_0_OR_GREATER
    protected internal ReadOnlySpan<TValue> AsSpan( ref bool lockTaken )
    {
        using ( AcquireLock( ref lockTaken ) ) { return CollectionsMarshal.AsSpan( list ); }
    }
#endif


    public override bool Exists( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.Exists( match ); }
    }
    public async ValueTask<bool> ExistsAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return list.Exists( match ); }
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
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)list, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return list.FindIndex( start, count, match ); }
    }
    public async ValueTask<int> FindIndexAsync( int start, Predicate<TValue> match, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return list.FindIndex( start, match ); }
    }
    public async ValueTask<int> FindIndexAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return list.FindIndex( match ); }
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
        using ( await AcquireLockAsync( token ) ) { return list.IndexOf( value ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return list.IndexOf( value, start ); }
    }
    public async ValueTask<int> IndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return list.IndexOf( value, start, count ); }
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
        using ( await AcquireLockAsync( token ) ) { return list.LastIndexOf( value ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        using ( await AcquireLockAsync( token ) ) { return list.LastIndexOf( value, start ); }
    }
    public async ValueTask<int> LastIndexOfAsync( TValue value, int start, int count, CancellationToken token = default )
    {
        Guard.IsInRangeFor( start, (ICollection<TValue>)list, nameof(start) );
        Guard.IsInRangeFor( count, (ICollection<TValue>)list, nameof(count) );
        using ( await AcquireLockAsync( token ) ) { return list.LastIndexOf( value, start, count ); }
    }


    public override List<TValue> FindAll( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindAll( match ); }
    }
    public async ValueTask<List<TValue>> FindAllAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return list.FindAll( match ); }
    }
    public override TValue? Find( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.Find( match ); }
    }
    public async ValueTask<TValue?> FindAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return list.Find( match ); }
    }
    public override TValue? FindLast( Predicate<TValue> match )
    {
        using ( AcquireLock() ) { return base.FindLast( match ); }
    }
    public async ValueTask<TValue?> FindLastAsync( Predicate<TValue> match, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return list.FindLast( match ); }
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
    public override void Add( scoped in ReadOnlySpan<TValue> values )
    {
        using ( AcquireLock() ) { base.Add( values ); }
    }
    public override void Add( scoped in ReadOnlyMemory<TValue> values )
    {
        using ( AcquireLock() ) { base.Add( values.Span ); }
    }
    public override void Add( scoped in ImmutableArray<TValue> values )
    {
        using ( AcquireLock() ) { base.Add( values.AsSpan() ); }
    }

    public virtual async ValueTask<bool> TryAddAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) )
        {
            if ( list.Contains( value ) ) { return false; }

            list.Add( value );
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
                this.list.Add( value );
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
    public override void CopyTo( int index, TValue[] array, int arrayIndex, int count )
    {
        using ( AcquireLock() ) { base.CopyTo( index, array, arrayIndex, count ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { list.CopyTo( array ); }
    }
    public async ValueTask CopyToAsync( TValue[] array, int arrayIndex, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { list.CopyTo( array, arrayIndex ); }
    }
    public async ValueTask CopyToAsync( int index, TValue[] array, int arrayIndex, int count, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { list.CopyTo( index, array, arrayIndex, count ); }
    }


    public override void InsertRange( int index, IEnumerable<TValue> collection )
    {
        using ( AcquireLock() ) { base.InsertRange( index, collection ); }
    }
    public override void InsertRange( int index, scoped in ReadOnlySpan<TValue> collection )
    {
        using ( AcquireLock() ) { base.InsertRange( index, collection ); }
    }
    public override void InsertRange( int index, scoped in ReadOnlyMemory<TValue> collection ) => InsertRange( index, collection.Span );
    public override void InsertRange( int index, scoped in ImmutableArray<TValue> collection ) => InsertRange( index, collection.AsSpan() );
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


    public override int Remove( Func<TValue, bool> match ) => Remove( list.Where( match ) );
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
    public virtual ValueTask<int> RemoveAsync( Func<TValue, bool> match, CancellationToken token = default ) => RemoveAsync( list.Where( match ), token );
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


    public override void Sort()                             => Sort( _comparer );
    public override void Sort( IComparer<TValue> comparer ) => Sort( comparer.Compare );
    public override void Sort( Comparison<TValue> compare )
    {
        using ( AcquireLock() ) { base.Sort( compare ); }
    }
    public override void Sort( int start, int count ) => Sort( start, count, _comparer );
    public override void Sort( int start, int count, IComparer<TValue> comparer )
    {
        using ( AcquireLock() ) { base.Sort( start, count, comparer ); }
    }
    public virtual ValueTask SortAsync( CancellationToken token                             = default ) => SortAsync( _comparer,        token );
    public virtual ValueTask SortAsync( IComparer<TValue> comparer, CancellationToken token = default ) => SortAsync( comparer.Compare, token );
    public virtual async ValueTask SortAsync( Comparison<TValue> compare, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { Reset(); }
    }
    public virtual ValueTask SortAsync( int start, int count, CancellationToken token = default ) => SortAsync( start, count, _comparer, token );
    public virtual async ValueTask SortAsync( int start, int count, IComparer<TValue> comparer, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { base.Sort( start, count, comparer ); }
    }


    void ICollection.CopyTo( Array array, int start )
    {
        using ( AcquireLock() ) { ((IList)list).CopyTo( array, start ); }
    }
    void IList.Remove( object? value )
    {
        using ( AcquireLock() ) { ((IList)list).Remove( value ); }
    }
    int IList.Add( object? value )
    {
        using ( AcquireLock() ) { return ((IList)list).Add( value ); }
    }
    bool IList.Contains( object? value )
    {
        using ( AcquireLock() ) { return ((IList)list).Contains( value ); }
    }
    int IList.IndexOf( object? value )
    {
        using ( AcquireLock() ) { return ((IList)list).IndexOf( value ); }
    }
    void IList.Insert( int index, object? value )
    {
        using ( AcquireLock() ) { ((IList)list).Insert( index, value ); }
    }


    public override bool Contains( TValue value )
    {
        using ( AcquireLock() ) { return list.Contains( value ); }
    }
    public virtual async ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        using ( await AcquireLockAsync( token ) ) { return list.Contains( value ); }
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public IDisposable            AcquireLock()                                                                                     => locker.Enter();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public IDisposable            AcquireLock( scoped in CancellationToken token )                                                  => locker.Enter( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public IDisposable            AcquireLock( ref       bool              lockTaken, scoped in CancellationToken token = default ) => locker.Enter( ref lockTaken, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public ValueTask<IDisposable> AcquireLockAsync( CancellationToken      token ) => locker.EnterAsync( token );


    ReadOnlyMemory<TValue> ILockedCollection<TValue>.Copy() => FilteredValues();
    protected ReadOnlyMemory<TValue> Copy()
    {
        using ( AcquireLock() ) { return FilteredValues(); }
    }
    ConfiguredValueTaskAwaitable<ReadOnlyMemory<TValue>> ILockedCollection<TValue>.CopyAsync( CancellationToken token ) => CopyAsync( token ).ConfigureAwait( false );
    protected async ValueTask<ReadOnlyMemory<TValue>> CopyAsync( CancellationToken token )
    {
        using ( await AcquireLockAsync( token ) ) { return FilteredValues(); }
    }



    public sealed class AsyncLockerEnumerator( ConcurrentObservableCollection<TValue> collection ) : AsyncLockerEnumerator<TValue>( collection );
}
