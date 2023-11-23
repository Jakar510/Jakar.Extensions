// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged { }



public abstract class CollectionAlerts<T> : ObservableClass, ICollectionAlerts, IEnumerable<T>
{
    public abstract int Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected CollectionAlerts() { }


    protected void Added( List<T> items )                               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  items ) );
    protected void Added( in T    item )                                => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  item ) );
    protected void Added( in T    item, in int index )                  => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  item, index ) );
    protected void Moved( in T    item, in int index, in int oldIndex ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move, item, index, oldIndex ) );


    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke( this, e );
        OnCountChanged();
    }
    protected      void OnCountChanged()                    => OnPropertyChanged( nameof(Count) );
    protected      void Removed( in T  item )               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item ) );
    protected      void Removed( in T  item, in int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item, index ) );
    protected      void Removed( int   index )                        => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove,  index ) );
    protected      void Replaced( in T old, in T @new )               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new, old ) );
    protected      void Replaced( in T old, in T @new, in int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new, old, index ) );
    protected      void Reset()   => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
    public virtual void Refresh() => Reset();


    protected virtual bool           Filter( T? value ) => true;
    public abstract   IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.         GetEnumerator() => GetEnumerator();
}
