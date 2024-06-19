// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/18/2024  22:06

using System.ComponentModel;



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
    public virtual bool HasError => !string.IsNullOrEmpty( ErrorText ) || State.ErrorCount > 0;
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
        bool result = State.TryAddModelException( key, exception );
        OnPropertyChanged( nameof(State) );
        return result;
    }
    public virtual void AddModelError( string key, Exception exception, ModelMetadata metadata )
    {
        State.AddModelError( key, exception, metadata );
        OnPropertyChanged( nameof(State) );
    }
    public virtual bool TryAddModelError( string key, Exception exception, ModelMetadata metadata )
    {
        bool result = State.TryAddModelError( key, exception, metadata );
        OnPropertyChanged( nameof(State) );
        return result;
    }
    public virtual void AddModelError( string key, string errorMessage )
    {
        State.AddModelError( key, errorMessage );
        OnPropertyChanged( nameof(State) );
    }
    public virtual bool TryAddModelError( string key, string errorMessage )
    {
        bool result = State.TryAddModelError( key, errorMessage );
        OnPropertyChanged( nameof(State) );
        return result;
    }
    public virtual void MarkFieldValid( string key )
    {
        State.MarkFieldValid( key );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void MarkFieldSkipped( string key )
    {
        State.MarkFieldSkipped( key );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void Merge( ModelStateDictionary dictionary )
    {
        State.Merge( dictionary );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void SetModelValue( string key, object? rawValue, string? attemptedValue )
    {
        State.SetModelValue( key, rawValue, attemptedValue );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void SetModelValue( string key, ValueProviderResult valueProviderResult )
    {
        State.SetModelValue( key, valueProviderResult );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void ClearValidationState( string key )
    {
        State.ClearValidationState( key );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void Remove( string key )
    {
        State.Remove( key );
        OnPropertyChanged( nameof(State) );
    }
    public virtual void Clear()
    {
        ErrorText = null;
        State.Clear();
        OnPropertyChanged( nameof(State) );
    }
}



public interface IModelState
{
    [CascadingParameter( Name = ModelErrorState.KEY )] public ModelErrorState State { get; set; }
}
