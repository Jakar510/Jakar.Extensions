// Jakar.Extensions :: Jakar.Extensions
// 09/10/2024  21:09

namespace Jakar.Extensions;


public interface IObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
{
    [NotifyPropertyChangedInvocator] public void OnPropertyChanged( [CallerMemberName] string property = EMPTY );
    [NotifyPropertyChangedInvocator] public void OnPropertyChanged( PropertyChangedEventArgs  e );


    public void OnPropertyChanging( [CallerMemberName] string property = EMPTY );
    public void OnPropertyChanging( PropertyChangingEventArgs e );


    bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value );
    bool SetPropertyWithoutNotify<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer )
        where TComparer : IEqualityComparer<TValue>;
    bool SetProperty<TValue>( ref TValue backingStore, TValue value, [CallerMemberName] string propertyName = EMPTY );
    bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TComparer : IEqualityComparer<TValue>;
    bool SetProperty<TValue, TComparer>( ref TValue backingStore, TValue value, in TValue minValue, TComparer comparer, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
        where TComparer : IEqualityComparer<TValue>;
}



public static class ObservableObjects
{
    public static readonly ConcurrentDictionary<string, PropertyChangedEventArgs>  PropertyChangedEventArgsCache  = new(StringComparer.Ordinal);
    public static readonly ConcurrentDictionary<string, PropertyChangingEventArgs> PropertyChangingEventArgsCache = new(StringComparer.Ordinal);


    public static PropertyChangedEventArgs GetPropertyChangedEventArgs( this string property )
    {
        if ( !PropertyChangedEventArgsCache.TryGetValue(property, out PropertyChangedEventArgs? args) ) { PropertyChangedEventArgsCache[property] = args = new PropertyChangedEventArgs(property); }

        return args;
    }
    public static PropertyChangingEventArgs GetPropertyChangingEventArgs( this string property )
    {
        if ( !PropertyChangingEventArgsCache.TryGetValue(property, out PropertyChangingEventArgs? args) ) { PropertyChangingEventArgsCache[property] = args = new PropertyChangingEventArgs(property); }

        return args;
    }
}
