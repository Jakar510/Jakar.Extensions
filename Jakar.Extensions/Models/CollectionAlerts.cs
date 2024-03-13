// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

namespace Jakar.Extensions;


public interface ICollectionAlerts : INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged { }



public abstract class CollectionAlerts<T> : ObservableClass, ICollectionAlerts, IReadOnlyCollection<T>
{
    public abstract int Count { get; }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected CollectionAlerts() { }


    public virtual void Refresh()                                             => Reset();
    protected      void OnCountChanged()                                      => OnPropertyChanged( nameof(Count) );
    protected      void Reset()                                               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
    protected      void Added( List<T> items )                                => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  items ) );
    protected      void Added( in   T  value )                                => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  value ) );
    protected      void Added( in   T  value, in int index )                  => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add,  value, index ) );
    protected      void Moved( in   T  value, in int index, in int oldIndex ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move, value, index, oldIndex ) );
    protected      void Removed( in T  value )               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, value ) );
    protected      void Removed( in T  value, in int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, value, index ) );
    protected      void Removed( int   index )                        => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove,  index ) );
    protected      void Replaced( in T old, in T @new )               => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new, old ) );
    protected      void Replaced( in T old, in T @new, in int index ) => OnChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, @new, old, index ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected virtual void OnChanged( NotifyCollectionChangedEventArgs e )
    {
        CollectionChanged?.Invoke( this, e );
        OnCountChanged();
    }


    protected virtual bool           Filter( T? value ) => true;
    public abstract   IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.         GetEnumerator() => GetEnumerator();
}
