// Jakar.Extensions :: Jakar.Extensions
// 09/10/2024  21:09

namespace Jakar.Extensions;


public interface IObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
{
    /*
    public bool SetProperty<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY );


    public bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null );
    */


    [NotifyPropertyChangedInvocator] void OnPropertyChanged( [CallerMemberName]  string property = EMPTY );
    void                                  OnPropertyChanging( [CallerMemberName] string property = EMPTY );
    [NotifyPropertyChangedInvocator] void OnPropertyChanged( PropertyChangedEventArgs   e );
    void                                  OnPropertyChanging( PropertyChangingEventArgs e );


    /// <summary>
    ///     <para> Checks the values with <paramref name="equalityComparer"/> . </para>
    ///     Then calls <see cref="OnPropertyChanging(string)"/> , sets the value, then calls <see cref="OnPropertyChanged(string)"/>
    /// </summary>
    /// <typeparam name="TValue"> </typeparam>
    /// <param name="backingStore"> </param>
    /// <param name="value"> </param>
    /// <param name="equalityComparer"> </param>
    /// <param name="propertyName"> </param>
    /// <returns>
    ///     <para> Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/> </para>
    /// </returns>
    public bool SetProperty<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY );
    public bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null );
}



public static class ObservableObjects
{
    public static readonly ConcurrentDictionary<string, PropertyChangedEventArgs>  PropertyChangedEventArgsCache  = new(StringComparer.Ordinal);
    public static readonly ConcurrentDictionary<string, PropertyChangingEventArgs> PropertyChangingEventArgsCache = new(StringComparer.Ordinal);


    public static PropertyChangedEventArgs GetPropertyChangedEventArgs( this string property )
    {
        if ( PropertyChangedEventArgsCache.TryGetValue( property, out PropertyChangedEventArgs? args ) is false ) { PropertyChangedEventArgsCache[property] = args = new PropertyChangedEventArgs( property ); }

        return args;
    }
    public static PropertyChangingEventArgs GetPropertyChangingEventArgs( this string property )
    {
        if ( PropertyChangingEventArgsCache.TryGetValue( property, out PropertyChangingEventArgs? args ) is false ) { PropertyChangingEventArgsCache[property] = args = new PropertyChangingEventArgs( property ); }

        return args;
    }


    public static bool SetPropertyWithoutNotify<TValue>( ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null )
    {
        equalityComparer ??= EqualityComparer<TValue>.Default;
        if ( equalityComparer.Equals( backingStore, value ) ) { return false; }

        backingStore = value;
        return true;
    }
    public static bool SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        equalityComparer ??= EqualityComparer<TValue>.Default;
        if ( equalityComparer.Equals( backingStore, value ) ) { return false; }

        observable.OnPropertyChanging( propertyName );
        backingStore = value;
        observable.OnPropertyChanged( propertyName );

        return true;
    }
    public static bool SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, in TValue minValue, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        value = value < minValue
                    ? minValue
                    : value;

        return observable.SetPropertyNotify( ref backingStore, value, equalityComparer ?? EqualityComparer<TValue>.Default, propertyName );
    }
    public static bool SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, in TValue minValue, IComparer<TValue> comparer, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        value = comparer.Compare( value, minValue ) < 0
                    ? minValue
                    : value;

        return observable.SetPropertyNotify( ref backingStore, value, equalityComparer ?? EqualityComparer<TValue>.Default, propertyName );
    }
    public static bool SetPropertyNotify( this IObservableObject observable, ref DateTime backingStore, DateTime value, in DateTime minDate, IEqualityComparer<DateTime>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? Sorter<DateTime>.Default, propertyName );
    }
    public static bool SetPropertyNotify( this IObservableObject observable, ref DateTime? backingStore, DateTime? value, in DateTime minDate, IEqualityComparer<DateTime?>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minDate
                    ? null
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? ValueSorter<DateTime>.Default, propertyName );
    }


    public static bool SetPropertyNotify( this IObservableObject observable, ref DateTimeOffset backingStore, DateTimeOffset value, in DateTimeOffset minDate, IEqualityComparer<DateTimeOffset>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minDate
                    ? minDate
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? Sorter<DateTimeOffset>.Default, propertyName );
    }
    public static bool SetPropertyNotify( this IObservableObject observable, ref DateTimeOffset? backingStore, DateTimeOffset? value, in DateTimeOffset minDate, IEqualityComparer<DateTimeOffset?>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minDate
                    ? null
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? ValueSorter<DateTimeOffset>.Default, propertyName );
    }


    public static bool SetPropertyNotify( this IObservableObject observable, ref DateOnly backingStore, DateOnly value, in DateOnly minValue, IEqualityComparer<DateOnly>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minValue
                    ? minValue
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? Sorter<DateOnly>.Default, propertyName );
    }
    public static bool SetPropertyNotify( this IObservableObject observable, ref DateOnly? backingStore, DateOnly? value, in DateOnly? minValue, IEqualityComparer<DateOnly?>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minValue
                    ? null
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? ValueSorter<DateOnly>.Default, propertyName );
    }


    public static bool SetPropertyNotify( this IObservableObject observable, ref TimeOnly backingStore, TimeOnly value, in TimeOnly minValue, IEqualityComparer<TimeOnly>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minValue
                    ? minValue
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? Sorter<TimeOnly>.Default, propertyName );
    }
    public static bool SetPropertyNotify( this IObservableObject observable, ref TimeOnly? backingStore, TimeOnly? value, in TimeOnly? minValue, IEqualityComparer<TimeOnly?>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minValue
                    ? null
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? ValueSorter<TimeOnly>.Default, propertyName );
    }


    public static bool SetPropertyNotify( this IObservableObject observable, ref TimeSpan backingStore, TimeSpan value, in TimeSpan minValue, IEqualityComparer<TimeSpan>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minValue
                    ? minValue
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? Sorter<TimeSpan>.Default, propertyName );
    }
    public static bool SetPropertyNotify( this IObservableObject observable, ref TimeSpan? backingStore, TimeSpan? value, in TimeSpan? minValue, IEqualityComparer<TimeSpan?>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        value = value < minValue
                    ? null
                    : value;

        return observable.SetProperty( ref backingStore, value, equalityComparer ?? ValueSorter<TimeSpan>.Default, propertyName );
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static Task SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, Func<Task> onChanged, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static Task SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, IEqualityComparer<TValue> equalityComparer, Func<Task> onChanged, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static Task SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, Func<TValue, Task> onChanged, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return Task.CompletedTask; }

        return onChanged( value );
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static void SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, ICommand onChanged, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return; }

        if ( onChanged.CanExecute( value ) ) { onChanged.Execute( value ); }
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static void SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, Action onChanged, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return; }

        onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static void SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, IEqualityComparer<TValue> equalityComparer, Action onChanged, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return; }

        onChanged();
    }


    /// <summary> "onChanged" only called if the backingStore value has changed. </summary>
    public static void SetPropertyNotify<TValue>( this IObservableObject observable, ref TValue backingStore, TValue value, Action<TValue> onChanged, IEqualityComparer<TValue>? equalityComparer = null, [CallerMemberName] string propertyName = EMPTY )
    {
        if ( observable.SetProperty( ref backingStore, value, equalityComparer, propertyName ) is false ) { return; }

        onChanged( value );
    }
}
