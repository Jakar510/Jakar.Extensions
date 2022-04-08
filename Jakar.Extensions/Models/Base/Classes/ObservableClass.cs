using System.Windows.Input;


namespace Jakar.Extensions.Models.Base.Classes;


public abstract class ObservableClass : BaseClass, INotifyPropertyChanged, INotifyPropertyChanging
{
    public static readonly DateTime sqlMinDate = DateTime.Parse("1/1/1753 12:00:00 AM", CultureInfo.InvariantCulture);


    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged( [CallerMemberName] string? propertyName = default ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
    protected void OnPropertyChanging( [CallerMemberName] string? propertyName = default ) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName ?? string.Empty));


    /// <summary>
    /// <para>Checks the values with <see cref="EqualityComparer{T}.Default"/>.</para>
    /// Then calls <see cref="OnPropertyChanging"/>, sets the value, then calls <see cref="OnPropertyChanged"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <returns>
    /// <para>Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/></para>
    /// </returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool SetPropertyWithoutNotify<T>( ref T backingStore, T value ) => SetPropertyWithoutNotify(ref backingStore, value, EqualityComparer<T>.Default);

    /// <summary>
    /// <para>Checks the values with <paramref name="comparer"/>.</para>
    /// Then calls <see cref="OnPropertyChanging"/>, sets the value, then calls <see cref="OnPropertyChanged"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns>
    /// <para>Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/></para>
    /// </returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool SetPropertyWithoutNotify<T>( ref T backingStore, T value, IEqualityComparer<T> comparer )
    {
        if ( comparer.Equals(backingStore, value) ) { return false; }

        backingStore = value;
        return true;
    }


    /// <summary>
    /// <para>Checks the values with <paramref name="comparer"/>.</para>
    /// Then calls <see cref="OnPropertyChanging"/>, sets the value, then calls <see cref="OnPropertyChanged"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <param name="propertyName"></param>
    /// <returns>
    /// <para>Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/></para>
    /// </returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = default )
    {
        if ( comparer.Equals(backingStore, value) ) { return false; }

        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }


    /// <summary>
    /// <para>Checks the values with <see cref="EqualityComparer{T}.Default"/>.</para>
    /// Then calls <see cref="OnPropertyChanging"/>, sets the value, then calls <see cref="OnPropertyChanged"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns>
    /// <para>Returns <see langword="false"/> if the values are equal, and therefore the <paramref name="backingStore"/> was not changed, otherwise <see langword="true"/></para>
    /// </returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string? propertyName = default ) => SetProperty(ref backingStore, value, EqualityComparer<T>.Default, propertyName);


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual void SetProperty<T>( ref T backingStore, T value, ICommand onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

        if ( onChanged.CanExecute(value) ) { onChanged.Execute(value); }
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, ICommand onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, comparer, propertyName) ) { return; }

        if ( onChanged.CanExecute(value) ) { onChanged.Execute(value); }
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual void SetProperty<T>( ref T backingStore, T value, Action onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

        onChanged();
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Action onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, comparer, propertyName) ) { return; }

        onChanged();
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual void SetProperty<T>( ref T backingStore, T value, Action<T> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

        onChanged(value);
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual void SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Action<T> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, comparer, propertyName) ) { return; }

        onChanged(value);
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual Task SetProperty<T>( ref T backingStore, T value, Func<Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual Task SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Func<Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, comparer, propertyName) ) { return Task.CompletedTask; }

        return onChanged();
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual Task SetProperty<T>( ref T backingStore, T value, Func<T, Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return Task.CompletedTask; }

        return onChanged(value);
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    [NotifyPropertyChangedInvocator]
    protected virtual Task SetProperty<T>( ref T backingStore, T value, IEqualityComparer<T> comparer, Func<T, Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, comparer, propertyName) ) { return Task.CompletedTask; }

        return onChanged(value);
    }


    [NotifyPropertyChangedInvocator]
    protected virtual bool SetProperty( ref DateTime? backingStore, DateTime? value, in DateTime minDate, [CallerMemberName] string? caller = default )
    {
        value = value < minDate
                    ? null
                    : value;

        return SetProperty(ref backingStore, value, caller);
    }


    [NotifyPropertyChangedInvocator]
    protected virtual bool SetProperty( ref DateTimeOffset? backingStore, DateTimeOffset? value, in DateTimeOffset minDate, [CallerMemberName] string? caller = default )
    {
        value = value < minDate
                    ? null
                    : value;

        return SetProperty(ref backingStore, value, caller);
    }


    protected virtual bool SetProperty( ref DateTime?       backingStore, DateTime?       value, [CallerMemberName] string? caller = default ) => SetProperty(ref backingStore, value, sqlMinDate, caller);
    protected virtual bool SetProperty( ref DateTimeOffset? backingStore, DateTimeOffset? value, [CallerMemberName] string? caller = default ) => SetProperty(ref backingStore, value, sqlMinDate, caller);
}
