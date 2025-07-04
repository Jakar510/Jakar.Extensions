// Jakar.Extensions :: Jakar.Extensions
// 09/10/2024  21:09

namespace Jakar.Extensions;


public interface IObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
{
    /*
    public bool SetProperty<TValue>( ref TValue backingStore, TValue value, TComparer equalityComparer , [CallerMemberName] string propertyName = EMPTY );


    public bool SetProperty<TValue>( ref TValue backingStore, TValue value, TComparer equalityComparer );
    */


    [NotifyPropertyChangedInvocator] void OnPropertyChanged( [CallerMemberName]  string property = EMPTY );
    void                                  OnPropertyChanging( [CallerMemberName] string property = EMPTY );
    [NotifyPropertyChangedInvocator] void OnPropertyChanged( PropertyChangedEventArgs   e );
    void                                  OnPropertyChanging( PropertyChangingEventArgs e );
}



public static class ObservableObjects
{
    public static readonly ConcurrentDictionary<string, PropertyChangedEventArgs>  PropertyChangedEventArgsCache  = new(StringComparer.Ordinal);
    public static readonly ConcurrentDictionary<string, PropertyChangingEventArgs> PropertyChangingEventArgsCache = new(StringComparer.Ordinal);


    public static PropertyChangedEventArgs GetPropertyChangedEventArgs( this string property )
    {
        if ( PropertyChangedEventArgsCache.TryGetValue(property, out PropertyChangedEventArgs? args) is false ) { PropertyChangedEventArgsCache[property] = args = new PropertyChangedEventArgs(property); }

        return args;
    }
    public static PropertyChangingEventArgs GetPropertyChangingEventArgs( this string property )
    {
        if ( PropertyChangingEventArgsCache.TryGetValue(property, out PropertyChangingEventArgs? args) is false ) { PropertyChangingEventArgsCache[property] = args = new PropertyChangingEventArgs(property); }

        return args;
    }
}
