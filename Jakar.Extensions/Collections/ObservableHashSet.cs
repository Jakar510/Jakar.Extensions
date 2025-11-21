// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

namespace Jakar.Extensions;


public class ObservableHashSet<TValue>( HashSet<TValue> values ) : ObservableHashSet<ObservableHashSet<TValue>, TValue>(values), ICollectionAlerts<ObservableHashSet<TValue>, TValue>
{
    public ObservableHashSet() : this(DEFAULT_CAPACITY) { }
    public ObservableHashSet( int                         capacity ) : this(new HashSet<TValue>(capacity)) { }
    public ObservableHashSet( IEnumerable<TValue>         enumerable ) : this(new HashSet<TValue>(enumerable)) { }
    public ObservableHashSet( params ReadOnlySpan<TValue> enumerable ) : this(new HashSet<TValue>(enumerable.Length)) { Add(enumerable); }


    public static implicit operator ObservableHashSet<TValue>( List<TValue>           values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( HashSet<TValue>        values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( ConcurrentBag<TValue>  values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( Collection<TValue>     values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( TValue[]               values ) => new(values.AsSpan());
    public static implicit operator ObservableHashSet<TValue>( ImmutableArray<TValue> values ) => new(values.AsSpan());
    public static implicit operator ObservableHashSet<TValue>( ReadOnlyMemory<TValue> values ) => new(values.Span);
    public static implicit operator ObservableHashSet<TValue>( ReadOnlySpan<TValue>   values ) => new(values);


    public override int  GetHashCode()                                                                    => buffer.GetHashCode();
    public override bool Equals( object?                         other )                                  => ReferenceEquals(this, other) || ( other is ObservableHashSet<TValue> x && Equals(x) );
    public static   bool operator ==( ObservableHashSet<TValue>? left, ObservableHashSet<TValue>? right ) => EqualityComparer<ObservableHashSet<TValue>>.Default.Equals(left, right);
    public static   bool operator !=( ObservableHashSet<TValue>? left, ObservableHashSet<TValue>? right ) => !EqualityComparer<ObservableHashSet<TValue>>.Default.Equals(left, right);
    public static   bool operator >( ObservableHashSet<TValue>   left, ObservableHashSet<TValue>  right ) => Comparer<ObservableHashSet<TValue>>.Default.Compare(left, right) > 0;
    public static   bool operator >=( ObservableHashSet<TValue>  left, ObservableHashSet<TValue>  right ) => Comparer<ObservableHashSet<TValue>>.Default.Compare(left, right) >= 0;
    public static   bool operator <( ObservableHashSet<TValue>   left, ObservableHashSet<TValue>  right ) => Comparer<ObservableHashSet<TValue>>.Default.Compare(left, right) < 0;
    public static   bool operator <=( ObservableHashSet<TValue>  left, ObservableHashSet<TValue>  right ) => Comparer<ObservableHashSet<TValue>>.Default.Compare(left, right) <= 0;
}



public abstract class ObservableHashSet<TSelf, TValue>( HashSet<TValue> values ) : CollectionAlerts<TSelf, TValue>, ISet<TValue>, IReadOnlySet<TValue>
    where TSelf : ObservableHashSet<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>
{
    protected internal readonly HashSet<TValue> buffer = values;
    public sealed override      int             Count      => buffer.Count;
    bool ICollection<TValue>.                   IsReadOnly => ( (ICollection<TValue>)buffer ).IsReadOnly;


    protected ObservableHashSet() : this(DEFAULT_CAPACITY) { }
    protected ObservableHashSet( int                         capacity ) : this(new HashSet<TValue>(capacity)) { }
    protected ObservableHashSet( IEnumerable<TValue>         enumerable ) : this(new HashSet<TValue>(enumerable)) { }
    protected ObservableHashSet( params ReadOnlySpan<TValue> enumerable ) : this(new HashSet<TValue>(enumerable.Length)) { Add(enumerable); }


    public virtual bool IsProperSubsetOf( IEnumerable<TValue>   other ) => buffer.IsProperSubsetOf(other);
    public virtual bool IsProperSupersetOf( IEnumerable<TValue> other ) => buffer.IsProperSupersetOf(other);
    public virtual bool IsSubsetOf( IEnumerable<TValue>         other ) => buffer.IsSubsetOf(other);
    public virtual bool IsSupersetOf( IEnumerable<TValue>       other ) => buffer.IsSupersetOf(other);
    public virtual bool Overlaps( IEnumerable<TValue>           other ) => buffer.Overlaps(other);
    public virtual bool SetEquals( IEnumerable<TValue>          other ) => buffer.SetEquals(other);
    public virtual void ExceptWith( IEnumerable<TValue> other )
    {
        buffer.ExceptWith(other);
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<TValue> other )
    {
        buffer.IntersectWith(other);
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<TValue> other )
    {
        buffer.SymmetricExceptWith(other);
        Reset();
    }
    public virtual void UnionWith( IEnumerable<TValue> other )
    {
        buffer.UnionWith(other);
        Reset();
    }


    public virtual void Clear()
    {
        buffer.Clear();
        Reset();
    }


    void ICollection<TValue>.Add( TValue value ) => Add(value);
    public virtual bool Add( TValue value )
    {
        bool result = buffer.Add(value);
        if ( result ) { Added(in value, -1); }

        return result;
    }
    public virtual void Add( params ReadOnlySpan<TValue> values )
    {
        foreach ( TValue value in values ) { Add(value); }
    }


    public virtual bool Remove( TValue value )
    {
        bool result = buffer.Remove(value);
        if ( result ) { Removed(in value, -1); }

        return result;
    }


    public virtual bool Contains( TValue value )                 => buffer.Contains(value);
    public         void CopyTo( TValue[] array, int arrayIndex ) => buffer.CopyTo(array, arrayIndex);


    [Pure] [MustDisposeResource] protected internal override ArrayBuffer<TValue> FilteredValues()
    {
        int                    count  = buffer.Count;
        ArrayBuffer<TValue>    values = new(count);
        FilterDelegate<TValue> filter = GetFilter();
        int                    index  = 0;


        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( TValue value in buffer )
        {
            if ( filter(index++, in value) ) { values.Add(in value); }
        }

        return values;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
