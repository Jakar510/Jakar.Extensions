// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableHashSet<TValue>( HashSet<TValue> values ) : CollectionAlerts<TValue>, ISet<TValue>, IReadOnlySet<TValue>
{
    protected internal readonly HashSet<TValue> buffer = values;
    public sealed override      int             Count      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    bool ICollection<TValue>.                   IsReadOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ((ICollection<TValue>)buffer).IsReadOnly; }


    public ObservableHashSet() : this( DEFAULT_CAPACITY ) { }
    public ObservableHashSet( int                 capacity ) : this( new HashSet<TValue>( capacity ) ) { }
    public ObservableHashSet( IEnumerable<TValue> enumerable ) : this( [..enumerable] ) { }


    public static implicit operator ObservableHashSet<TValue>( List<TValue>          values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( HashSet<TValue>       values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( ConcurrentBag<TValue> values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( Collection<TValue>    values ) => new(values);
    public static implicit operator ObservableHashSet<TValue>( TValue[]              values ) => new(values);


    public virtual bool IsProperSubsetOf( IEnumerable<TValue>   other ) => buffer.IsProperSubsetOf( other );
    public virtual bool IsProperSupersetOf( IEnumerable<TValue> other ) => buffer.IsProperSupersetOf( other );
    public virtual bool IsSubsetOf( IEnumerable<TValue>         other ) => buffer.IsSubsetOf( other );
    public virtual bool IsSupersetOf( IEnumerable<TValue>       other ) => buffer.IsSupersetOf( other );
    public virtual bool Overlaps( IEnumerable<TValue>           other ) => buffer.Overlaps( other );
    public virtual bool SetEquals( IEnumerable<TValue>          other ) => buffer.SetEquals( other );
    public virtual void ExceptWith( IEnumerable<TValue> other )
    {
        buffer.ExceptWith( other );
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<TValue> other )
    {
        buffer.IntersectWith( other );
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<TValue> other )
    {
        buffer.SymmetricExceptWith( other );
        Reset();
    }
    public virtual void UnionWith( IEnumerable<TValue> other )
    {
        buffer.UnionWith( other );
        Reset();
    }


    public virtual void Clear()
    {
        buffer.Clear();
        Reset();
    }


    void ICollection<TValue>.Add( TValue value ) => Add( value );
    public virtual bool Add( TValue value )
    {
        bool result = buffer.Add( value );
        if ( result ) { Added( value ); }

        return result;
    }


    public virtual bool Remove( TValue value )
    {
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }


    public virtual bool Contains( TValue value )                 => buffer.Contains( value );
    public         void CopyTo( TValue[] array, int arrayIndex ) => buffer.CopyTo( array, arrayIndex );


    protected internal override FilterBuffer<TValue> FilteredValues()
    {
        int                  count  = buffer.Count;
        FilterBuffer<TValue> values = new(count);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( TValue t in buffer )
        {
            if ( Filter( t ) ) { values.Add( t ); }
        }

        return values;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
