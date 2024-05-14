// Jakar.Extensions :: Jakar.Extensions
// 08/21/2022  9:29 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ObservableHashSet<T>( HashSet<T> values ) : CollectionAlerts<T>, ISet<T>, IReadOnlySet<T>
{
    protected internal readonly HashSet<T> buffer = values;
    public sealed override      int        Count      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => buffer.Count; }
    bool ICollection<T>.                   IsReadOnly { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ((ICollection<T>)buffer).IsReadOnly; }


    public ObservableHashSet() : this( DEFAULT_CAPACITY ) { }
    public ObservableHashSet( int            capacity ) : this( new HashSet<T>( capacity ) ) { }
    public ObservableHashSet( IEnumerable<T> enumerable ) : this( [..enumerable] ) { }


    public static implicit operator ObservableHashSet<T>( List<T>                 values ) => new(values);
    public static implicit operator ObservableHashSet<T>( HashSet<T>              values ) => new(values);
    public static implicit operator ObservableHashSet<T>( ConcurrentBag<T>        values ) => new(values);
    public static implicit operator ObservableHashSet<T>( ObservableCollection<T> values ) => new(values);
    public static implicit operator ObservableHashSet<T>( Collection<T>           values ) => new(values);
    public static implicit operator ObservableHashSet<T>( T[]                     values ) => new(values);


    public virtual bool IsProperSubsetOf( IEnumerable<T>   other ) => buffer.IsProperSubsetOf( other );
    public virtual bool IsProperSupersetOf( IEnumerable<T> other ) => buffer.IsProperSupersetOf( other );
    public virtual bool IsSubsetOf( IEnumerable<T>         other ) => buffer.IsSubsetOf( other );
    public virtual bool IsSupersetOf( IEnumerable<T>       other ) => buffer.IsSupersetOf( other );
    public virtual bool Overlaps( IEnumerable<T>           other ) => buffer.Overlaps( other );
    public virtual bool SetEquals( IEnumerable<T>          other ) => buffer.SetEquals( other );
    public virtual void ExceptWith( IEnumerable<T> other )
    {
        buffer.ExceptWith( other );
        Reset();
    }
    public virtual void IntersectWith( IEnumerable<T> other )
    {
        buffer.IntersectWith( other );
        Reset();
    }
    public virtual void SymmetricExceptWith( IEnumerable<T> other )
    {
        buffer.SymmetricExceptWith( other );
        Reset();
    }
    public virtual void UnionWith( IEnumerable<T> other )
    {
        buffer.UnionWith( other );
        Reset();
    }


    public virtual void Clear()
    {
        buffer.Clear();
        Reset();
    }


    void ICollection<T>.Add( T value ) => Add( value );
    public virtual bool Add( T value )
    {
        bool result = buffer.Add( value );
        if ( result ) { Added( value ); }

        return result;
    }


    public virtual bool Remove( T value )
    {
        bool result = buffer.Remove( value );
        if ( result ) { Removed( value ); }

        return result;
    }


    public virtual bool Contains( T value )                 => buffer.Contains( value );
    public         void CopyTo( T[] array, int arrayIndex ) => buffer.CopyTo( array, arrayIndex );


    protected internal override ReadOnlyMemory<T> FilteredValues()
    {
        int             count  = buffer.Count;
        using Buffer<T> values = new(count);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( T t in buffer )
        {
            if ( Filter( t ) ) { values.Add( t ); }
        }

        return values.ToArray();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
