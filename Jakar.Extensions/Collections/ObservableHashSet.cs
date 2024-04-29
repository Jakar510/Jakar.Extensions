// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

using System;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableHashSet<T>( HashSet<T> values ) : CollectionAlerts<T>, ISet<T>, IReadOnlySet<T>
{
    private readonly       HashSet<T> _buffer = values;
    public sealed override int        Count      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer.Count; }
    bool ICollection<T>.              IsReadOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ((ICollection<T>)_buffer).IsReadOnly; }


    public ObservableHashSet() : this( DEFAULT_CAPACITY ) { }
    public ObservableHashSet( int            capacity ) : this( new HashSet<T>( capacity ) ) { }
    public ObservableHashSet( IEnumerable<T> enumerable ) : this( [..enumerable] ) { }


    public static implicit operator ObservableHashSet<T>( List<T>                 items ) => new(items);
    public static implicit operator ObservableHashSet<T>( HashSet<T>              items ) => new(items);
    public static implicit operator ObservableHashSet<T>( ConcurrentBag<T>        items ) => new(items);
    public static implicit operator ObservableHashSet<T>( ObservableCollection<T> items ) => new(items);
    public static implicit operator ObservableHashSet<T>( Collection<T>           items ) => new(items);
    public static implicit operator ObservableHashSet<T>( T[]                     items ) => new(items);


    public virtual bool IsProperSubsetOf( IEnumerable<T>   other ) => _buffer.IsProperSubsetOf( other );
    public virtual bool IsProperSupersetOf( IEnumerable<T> other ) => _buffer.IsProperSupersetOf( other );
    public virtual bool IsSubsetOf( IEnumerable<T>         other ) => _buffer.IsSubsetOf( other );
    public virtual bool IsSupersetOf( IEnumerable<T>       other ) => _buffer.IsSupersetOf( other );
    public virtual bool Overlaps( IEnumerable<T>           other ) => _buffer.Overlaps( other );
    public virtual bool SetEquals( IEnumerable<T>          other ) => _buffer.SetEquals( other );
    public virtual void ExceptWith( IEnumerable<T> other )
    {
        _buffer.ExceptWith( other );
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<T> other )
    {
        _buffer.IntersectWith( other );
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<T> other )
    {
        _buffer.SymmetricExceptWith( other );
        Reset();
    }
    public virtual void UnionWith( IEnumerable<T> other )
    {
        _buffer.UnionWith( other );
        Reset();
    }


    public virtual void Clear()
    {
        _buffer.Clear();
        Reset();
    }


    void ICollection<T>.Add( T item ) => Add( item );
    public virtual bool Add( T item )
    {
        bool result = _buffer.Add( item );
        if ( result ) { Added( item ); }

        return result;
    }


    public virtual bool Remove( T item )
    {
        bool result = _buffer.Remove( item );
        if ( result ) { Removed( item ); }

        return result;
    }


    public virtual bool Contains( T item )                  => _buffer.Contains( item );
    public         void CopyTo( T[] array, int arrayIndex ) => _buffer.CopyTo( array, arrayIndex );


    protected internal override T[] FilteredValues()
    {
        T[] result;
        T[] array = ArrayPool<T>.Shared.Rent( _buffer.Count );
        _buffer.CopyTo( array );

        try { result = FilteredValues( new ReadOnlySpan<T>( array, 0, _buffer.Count ) ); }
        finally { ArrayPool<T>.Shared.Return( array ); }

        return result;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
