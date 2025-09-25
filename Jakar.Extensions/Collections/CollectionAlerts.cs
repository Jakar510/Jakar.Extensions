// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

using ZLinq;



namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, IObservableObject;



public interface ICollectionAlerts<TValue> : IReadOnlyCollection<TValue>, IValueEnumerable<FilterBuffer<TValue>, TValue>, ICollectionAlerts;



public interface ICollectionAlerts<TClass, TValue> : ICollectionAlerts<TValue>, IJsonModel<TClass>
    where TClass : ICollectionAlerts<TClass, TValue>
{
    public abstract static implicit operator TClass( List<TValue>           values );
    public abstract static implicit operator TClass( HashSet<TValue>        values );
    public abstract static implicit operator TClass( ConcurrentBag<TValue>  values );
    public abstract static implicit operator TClass( Collection<TValue>     values );
    public abstract static implicit operator TClass( TValue[]               values );
    public abstract static implicit operator TClass( ImmutableArray<TValue> values );
    public abstract static implicit operator TClass( ReadOnlyMemory<TValue> values );
    public abstract static implicit operator TClass( ReadOnlySpan<TValue>   values );
}



public abstract class CollectionAlerts<TClass, TValue> : BaseClass<TClass>, ICollectionAlerts<TValue>
    where TClass : CollectionAlerts<TClass, TValue>, ICollectionAlerts<TClass, TValue>
{
// ReSharper disable once StaticMemberInGenericType
    protected static readonly NotifyCollectionChangedEventArgs _resetArgs = new(NotifyCollectionChangedAction.Reset);
    public abstract           int                              Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public override bool Equals( TClass?    other ) => ReferenceEquals(this, other);
    public override int  CompareTo( TClass? other ) => Nullable.Compare(Count, other?.Count);


    public virtual                                               void Refresh()                                         => Reset();
    protected                                                    void Reset()                                           => OnChanged(_resetArgs);
    protected                                                    void Added( TValue[]                value, int index ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
    protected                                                    void Added( ref readonly TValue     value, int index ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
    protected                                                    void Removed( TValue[]              value, int index ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
    protected                                                    void Removed( ref readonly TValue   value, int index ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
    protected                                                    void Removed( int                   index )                                                       => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,  index));
    protected                                                    void Moved( TValue[]                value, ref readonly int    index, ref readonly int oldIndex ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,    value, index, oldIndex));
    protected                                                    void Moved( ref readonly    TValue  value, ref readonly int    index, ref readonly int oldIndex ) => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,    value, index, oldIndex));
    protected                                                    void Replaced( ref readonly TValue? old,   ref readonly TValue value, int              index )    => OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old,   index));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected void OnCountChanged() => OnPropertyChanged(nameof(Count));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke(this, e);
        if ( e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset ) { OnCountChanged(); }
    }
    protected virtual                                       bool                                          Filter( int index, ref readonly TValue? value ) => true;
    [Pure, MustDisposeResource] protected internal abstract FilterBuffer<TValue>                          FilteredValues();
    [Pure, MustDisposeResource] public                      ValueEnumerable<FilterBuffer<TValue>, TValue> AsValueEnumerable() => new(FilteredValues());
    public virtual IEnumerator<TValue> GetEnumerator()
    {
        using FilterBuffer<TValue> owner = FilteredValues();
        for ( int i = 0; i < owner.Length; i++ ) { yield return owner.Values[i]; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
