// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableHashSet<T>( HashSet<T> values ) : CollectionAlerts<T>, ISet<T>, IReadOnlySet<T>
{
    private readonly       HashSet<T> _values = values;
    public sealed override int        Count      => _values.Count;
    bool ICollection<T>.              IsReadOnly => ((ICollection<T>)_values).IsReadOnly;


    public ObservableHashSet() : this( [] ) { }
    public ObservableHashSet( int            capacity ) : this( new HashSet<T>( capacity ) ) { }
    public ObservableHashSet( IEnumerable<T> enumerable ) : this( [..enumerable] ) { }


    public static implicit operator ObservableHashSet<T>( List<T>                 items ) => new(items);
    public static implicit operator ObservableHashSet<T>( HashSet<T>              items ) => new(items);
    public static implicit operator ObservableHashSet<T>( ConcurrentBag<T>        items ) => new(items);
    public static implicit operator ObservableHashSet<T>( ObservableCollection<T> items ) => new(items);
    public static implicit operator ObservableHashSet<T>( Collection<T>           items ) => new(items);
    public static implicit operator ObservableHashSet<T>( T[]                     items ) => new(items);


    public virtual bool IsProperSubsetOf( IEnumerable<T>   other ) => _values.IsProperSubsetOf( other );
    public virtual bool IsProperSupersetOf( IEnumerable<T> other ) => _values.IsProperSupersetOf( other );
    public virtual bool IsSubsetOf( IEnumerable<T>         other ) => _values.IsSubsetOf( other );
    public virtual bool IsSupersetOf( IEnumerable<T>       other ) => _values.IsSupersetOf( other );
    public virtual bool Overlaps( IEnumerable<T>           other ) => _values.Overlaps( other );
    public virtual bool SetEquals( IEnumerable<T>          other ) => _values.SetEquals( other );
    public virtual void ExceptWith( IEnumerable<T> other )
    {
        _values.ExceptWith( other );
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<T> other )
    {
        _values.IntersectWith( other );
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<T> other )
    {
        _values.SymmetricExceptWith( other );
        Reset();
    }
    public virtual void UnionWith( IEnumerable<T> other )
    {
        _values.UnionWith( other );
        Reset();
    }


    public virtual void Clear()
    {
        _values.Clear();
        Reset();
    }


    void ICollection<T>.Add( T item ) => Add( item );
    public virtual bool Add( T item )
    {
        bool result = _values.Add( item );
        if ( result ) { Added( item ); }

        return result;
    }


    public virtual bool Remove( T item )
    {
        bool result = _values.Remove( item );
        if ( result ) { Removed( item ); }

        return result;
    }


    public virtual bool Contains( T item )                  => _values.Contains( item );
    public         void CopyTo( T[] array, int arrayIndex ) => _values.CopyTo( array, arrayIndex );


    protected internal override ReadOnlyMemory<T> FilteredValues() => _values.Where( Filter ).ToArray();
    IEnumerator IEnumerable.                      GetEnumerator()  => GetEnumerator();
}
