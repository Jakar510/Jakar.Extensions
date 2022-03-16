using System.Windows.Input;


namespace Jakar.Extensions.Models.Base.Records;


public record BaseNotifyPropertyModelRecord : BaseRecord, INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangedEventHandler?  PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;


    /// <summary>
    /// Calls <see cref="OnPropertyChanging"/>, sets the value, then calls <see cref="OnPropertyChanged"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns><see langword="true"/> if the <paramref name="backingStore"/> field was changed, otherwise <see langword="false"/></returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool SetProperty<T>( ref T backingStore, T value, [CallerMemberName] string? propertyName = default )
    {
        if ( EqualityComparer<T>.Default.Equals(backingStore, value) ) { return false; }

        OnPropertyChanging(propertyName);
        backingStore = value;
        OnPropertyChanged(propertyName);

        return true;
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed, and onChanged.CanExecute(value) is true. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="onChanged"></param>
    /// <param name="propertyName"></param>
    /// <returns><see langword="true"/> if the <paramref name="backingStore"/> field was changed, otherwise <see langword="false"/></returns>
    [NotifyPropertyChangedInvocator]
    protected void SetProperty<T>( ref T backingStore, T value, ICommand onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

        if ( onChanged.CanExecute(value) ) { onChanged.Execute(value); }
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="onChanged"></param>
    /// <param name="propertyName"></param>
    /// <returns><see langword="true"/> if the <paramref name="backingStore"/> field was changed, otherwise <see langword="false"/></returns>
    [NotifyPropertyChangedInvocator]
    protected void SetProperty<T>( ref T backingStore, T value, Action onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

        onChanged();
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="onChanged"></param>
    /// <param name="propertyName"></param>
    /// <returns><see langword="true"/> if the <paramref name="backingStore"/> field was changed, otherwise <see langword="false"/></returns>
    [NotifyPropertyChangedInvocator]
    protected void SetProperty<T>( ref T backingStore, T value, Action<T> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return; }

        onChanged(value);
    }


    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="onChanged"></param>
    /// <param name="propertyName"></param>
    /// <returns><see langword="true"/> if the <paramref name="backingStore"/> field was changed, otherwise <see langword="false"/></returns>
    [NotifyPropertyChangedInvocator]
    protected Task SetPropertyAsync<T>( ref T backingStore, T value, Func<Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return Task.CompletedTask; }

        return onChanged();
    }

    /// <summary>
    /// "onChanged" only called if the backingStore value has changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingStore"></param>
    /// <param name="value"></param>
    /// <param name="onChanged"></param>
    /// <param name="propertyName"></param>
    /// <returns><see langword="true"/> if the <paramref name="backingStore"/> field was changed, otherwise <see langword="false"/></returns>
    [NotifyPropertyChangedInvocator]
    protected Task SetPropertyAsync<T>( ref T backingStore, T value, Func<T, Task> onChanged, [CallerMemberName] string? propertyName = default )
    {
        if ( !SetProperty(ref backingStore, value, propertyName) ) { return Task.CompletedTask; }

        return onChanged(value);
    }

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged( [CallerMemberName] string? propertyName = default ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));

    protected void OnPropertyChanging( [CallerMemberName] string? propertyName = default ) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName ?? string.Empty));
}
