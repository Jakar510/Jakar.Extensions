// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

using ZLinq;



namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, IObservableObject;



public abstract class CollectionAlerts<TValue> : ObservableClass, IReadOnlyCollection<TValue>, IValueEnumerable<FilterBuffer<TValue>, TValue>, ICollectionAlerts
{
    // ReSharper disable once StaticMemberInGenericType
    protected static readonly NotifyCollectionChangedEventArgs _resetArgs = new(NotifyCollectionChangedAction.Reset);


    public abstract int Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    public virtual void Refresh()                                         => Reset();
    protected      void Reset()                                           => OnChanged( _resetArgs );
    protected      void Added( TValue[]                value, int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Added( ref readonly TValue     value, int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( TValue[]              value, int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( ref readonly TValue   value, int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, value, index ) );
    protected      void Removed( int                   index )                                                       => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove,  index ) );
    protected      void Moved( TValue[]                value, ref readonly int    index, ref readonly int oldIndex ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move,    value, index, oldIndex ) );
    protected      void Moved( ref readonly    TValue  value, ref readonly int    index, ref readonly int oldIndex ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move,    value, index, oldIndex ) );
    protected      void Replaced( ref readonly TValue? old,   ref readonly TValue value, int              index )    => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, value, old,   index ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected void OnCountChanged() => OnPropertyChanged( nameof(Count) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke( this, e );
        if ( e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset ) { OnCountChanged(); }
    }


    protected virtual bool Filter( int index, ref readonly TValue? value ) => true;


    [Pure, MustDisposeResource] protected internal abstract FilterBuffer<TValue>                          FilteredValues();
    [Pure, MustDisposeResource] public                      ValueEnumerable<FilterBuffer<TValue>, TValue> AsValueEnumerable() => new(FilteredValues());


    public virtual IEnumerator<TValue> GetEnumerator()
    {
        using FilterBuffer<TValue> owner = FilteredValues();
        for ( int i = 0; i < owner.Length; i++ ) { yield return owner.Values[i]; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
