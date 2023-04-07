// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableHashSet<TElement> : CollectionAlerts<TElement>, ISet<TElement>, IReadOnlySet<TElement>, ISerializable, IDeserializationCallback
{
    private readonly       HashSet<TElement> _values;
    public sealed override int               Count      => _values.Count;
    bool ICollection<TElement>.              IsReadOnly => ((ICollection<TElement>)_values).IsReadOnly;


    public ObservableHashSet() : this( new HashSet<TElement>() ) { }
    public ObservableHashSet( HashSet<TElement>              values ) => _values = values;
    void IDeserializationCallback.OnDeserialization( object? sender ) => _values.OnDeserialization( sender );

    void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context ) => _values.GetObjectData( info, context );


    public virtual bool IsProperSubsetOf( IEnumerable<TElement>   other ) => _values.IsProperSubsetOf( other );
    public virtual bool IsProperSupersetOf( IEnumerable<TElement> other ) => _values.IsProperSupersetOf( other );
    public virtual bool IsSubsetOf( IEnumerable<TElement>         other ) => _values.IsSubsetOf( other );
    public virtual bool IsSupersetOf( IEnumerable<TElement>       other ) => _values.IsSupersetOf( other );
    public virtual bool Overlaps( IEnumerable<TElement>           other ) => _values.Overlaps( other );
    public virtual bool SetEquals( IEnumerable<TElement>          other ) => _values.SetEquals( other );
    public virtual void ExceptWith( IEnumerable<TElement> other )
    {
        _values.ExceptWith( other );
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<TElement> other )
    {
        _values.IntersectWith( other );
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<TElement> other )
    {
        _values.SymmetricExceptWith( other );
        Reset();
    }
    public virtual void UnionWith( IEnumerable<TElement> other )
    {
        _values.UnionWith( other );
        Reset();
    }


    void ICollection<TElement>.Add( TElement item ) => Add( item );
    public virtual bool Add( TElement item )
    {
        bool result = _values.Add( item );
        if ( result ) { Added( item ); }

        return result;
    }


    public virtual bool Remove( TElement item )
    {
        bool result = _values.Remove( item );
        if ( result ) { Removed( item ); }

        return result;
    }


    public virtual bool Contains( TElement item ) => _values.Contains( item );
    public virtual void Clear()
    {
        _values.Clear();
        Reset();
    }

    public void CopyTo( TElement[] array, int arrayIndex ) => _values.CopyTo( array, arrayIndex );


    public IEnumerator<TElement> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
