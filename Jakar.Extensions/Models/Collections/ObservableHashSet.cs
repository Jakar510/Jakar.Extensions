// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class ObservableHashSet<TValue> : CollectionAlerts<TValue>, ISet<TValue>
{
    private readonly       HashSet<TValue> _values;
    public sealed override int             Count      => _values.Count;
    public virtual         bool            IsReadOnly => ( (ICollection<TValue>)_values ).IsReadOnly;


    public ObservableHashSet() : this(new HashSet<TValue>()) { }
    public ObservableHashSet( HashSet<TValue> values ) => _values = values;


    public virtual bool IsProperSubsetOf( IEnumerable<TValue>   other ) => _values.IsProperSubsetOf(other);
    public virtual bool IsProperSupersetOf( IEnumerable<TValue> other ) => _values.IsProperSupersetOf(other);
    public virtual bool IsSubsetOf( IEnumerable<TValue>         other ) => _values.IsSubsetOf(other);
    public virtual bool IsSupersetOf( IEnumerable<TValue>       other ) => _values.IsSupersetOf(other);
    public virtual bool Overlaps( IEnumerable<TValue>           other ) => _values.Overlaps(other);
    public virtual bool SetEquals( IEnumerable<TValue>          other ) => _values.SetEquals(other);
    public virtual void ExceptWith( IEnumerable<TValue> other )
    {
        _values.ExceptWith(other);
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<TValue> other )
    {
        _values.IntersectWith(other);
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<TValue> other )
    {
        _values.SymmetricExceptWith(other);
        Reset();
    }
    public virtual void UnionWith( IEnumerable<TValue> other )
    {
        _values.UnionWith(other);
        Reset();
    }


    void ICollection<TValue>.Add( TValue item ) => Add(item);
    public virtual void Add( params TValue[] items )
    {
        foreach ( TValue value in items ) { Add(value); }
    }
    public virtual void Add( IEnumerable<TValue> items )
    {
        foreach ( TValue value in items ) { Add(value); }
    }
    public virtual bool Add( TValue item )
    {
        bool result = _values.Add(item);
        if ( result ) { Added(item); }

        return result;
    }
    public virtual bool Remove( TValue item )
    {
        bool result = _values.Remove(item);
        if ( result ) { Added(item); }

        return result;
    }
    public virtual bool Contains( TValue item ) => _values.Contains(item);
    public virtual void Clear()
    {
        _values.Clear();
        Reset();
    }

    public void CopyTo( TValue[] array, int arrayIndex ) => _values.CopyTo(array, arrayIndex);


    public IEnumerator<TValue> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
