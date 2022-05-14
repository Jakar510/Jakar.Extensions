// Jakar.Extensions :: Jakar.Extensions
// 04/12/2022  1:54 PM

namespace Jakar.Extensions.Models.Collections;


public interface ICollectionAlerts : INotifyCollectionChanged, INotifyPropertyChanging, INotifyPropertyChanged
{
   protected internal void SendOnChanged( NotifyCollectionChangedEventArgs e );
}



public static class Alerts
{
    public static void OnChanged( this       ICollectionAlerts alerts, in NotifyCollectionChangedEventArgs e ) => alerts.SendOnChanged(e);
    public static void SendAdded<T>( this    ICollectionAlerts alerts, in List<T> items ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
    public static void SendAdded<T>( this    ICollectionAlerts alerts, in T item ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
    public static void SendAdded<T>( this    ICollectionAlerts alerts, in T item, in int index ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    public static void SendRemoved<T>( this  ICollectionAlerts alerts, in T item ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
    public static void SendRemoved<T>( this  ICollectionAlerts alerts, in T item, in int index ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
    public static void SendRemoved( this     ICollectionAlerts alerts, in int index ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, index));
    public static void SendReplaced<T>( this ICollectionAlerts alerts, in T old, in T @new ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, @new, old));
    public static void SendReplaced<T>( this ICollectionAlerts alerts, in T old, in T @new, in int index ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, @new, old, index));
    public static void SendMoved<T>( this     ICollectionAlerts alerts, in T item, in int index, in int oldIndex ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, index, oldIndex));
    public static void SendReset( this       ICollectionAlerts alerts ) => alerts.OnChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
}
