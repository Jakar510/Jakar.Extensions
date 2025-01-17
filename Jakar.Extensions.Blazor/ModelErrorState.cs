// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/18/2024  22:06

namespace Jakar.Extensions.Blazor;


public interface IModelErrorState : ICascadingValueName, INotifyPropertyChanged
{
    string?              ErrorText { get; set; }
    bool                 HasError  { get; }
    ModelStateDictionary State     { get; set; }
    bool                 TryAddModelException( string key, Exception exception );
    void                 AddModelError( string        key, Exception exception, ModelMetadata metadata );
    void                 AddModelError( string        key, string    errorMessage );
    bool                 TryAddModelError( string     key, Exception exception, ModelMetadata metadata );
    bool                 TryAddModelError( string     key, string    errorMessage );
    void                 MarkFieldValid( string       key );
    void                 MarkFieldSkipped( string     key );
    void                 Merge( ModelStateDictionary  dictionary );
    void                 SetModelValue( string        key, object?             rawValue, string? attemptedValue );
    void                 SetModelValue( string        key, ValueProviderResult valueProviderResult );
    void                 ClearValidationState( string key );
    void                 Remove( string               key );
    void                 Clear();
}



public class ModelErrorState : ObservableClass, IModelErrorState
{
    public const string               KEY    = nameof(ModelErrorState);
    private      ModelStateDictionary _state = new();
    private      string?              _errorText;


    public static string CascadingName => KEY;
    public virtual string? ErrorText
    {
        get => _errorText;
        set
        {
            if ( SetProperty( ref _errorText, value ) ) { OnPropertyChanged( nameof(HasError) ); }
        }
    }
    public virtual bool HasError => string.IsNullOrEmpty( ErrorText ) is false || _state.ErrorCount > 0;
    public virtual ModelStateDictionary State
    {
        get => _state;
        set
        {
            if ( SetProperty( ref _state, value ) ) { OnPropertyChanged( nameof(HasError) ); }
        }
    }


    public static ModelErrorState Get( IServiceProvider provider ) => provider.GetRequiredService<ModelErrorState>();


    public virtual bool TryAddModelException( string key, Exception exception )
    {
        bool result = _state.TryAddModelException( key, exception );
        OnPropertyChanged( nameof(_state) );
        return result;
    }
    public virtual void AddModelError( string key, Exception exception, ModelMetadata metadata )
    {
        _state.AddModelError( key, exception, metadata );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual bool TryAddModelError( string key, Exception exception, ModelMetadata metadata )
    {
        bool result = _state.TryAddModelError( key, exception, metadata );
        OnPropertyChanged( nameof(_state) );
        return result;
    }
    public virtual void AddModelError( string key, string errorMessage )
    {
        _state.AddModelError( key, errorMessage );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual bool TryAddModelError( string key, string errorMessage )
    {
        bool result = _state.TryAddModelError( key, errorMessage );
        OnPropertyChanged( nameof(_state) );
        return result;
    }
    public virtual void MarkFieldValid( string key )
    {
        _state.MarkFieldValid( key );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void MarkFieldSkipped( string key )
    {
        _state.MarkFieldSkipped( key );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void Merge( ModelStateDictionary dictionary )
    {
        _state.Merge( dictionary );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void SetModelValue( string key, object? rawValue, string? attemptedValue )
    {
        _state.SetModelValue( key, rawValue, attemptedValue );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void SetModelValue( string key, ValueProviderResult valueProviderResult )
    {
        _state.SetModelValue( key, valueProviderResult );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void ClearValidationState( string key )
    {
        _state.ClearValidationState( key );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void Remove( string key )
    {
        _state.Remove( key );
        OnPropertyChanged( nameof(_state) );
    }
    public virtual void Clear()
    {
        ErrorText = null;
        _state.Clear();
        OnPropertyChanged( nameof(_state) );
    }
}



public interface IModelState<T>
    where T : IModelErrorState
{
    [CascadingParameter( Name = ModelErrorState.KEY )] public T State { get; set; }
}



public interface IModelState : IModelState<ModelErrorState>;
