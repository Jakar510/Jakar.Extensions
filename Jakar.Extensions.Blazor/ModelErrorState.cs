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
    private      ModelStateDictionary __state = new();
    private      string?              __errorText;


    public static string CascadingName => KEY;
    public virtual string? ErrorText
    {
        get => __errorText;
        set
        {
            if ( SetProperty( ref __errorText, value ) ) { OnPropertyChanged( nameof(HasError) ); }
        }
    }
    public virtual bool HasError => string.IsNullOrEmpty( ErrorText ) is false || __state.ErrorCount > 0;
    public virtual ModelStateDictionary State
    {
        get => __state;
        set
        {
            if ( SetProperty( ref __state, value ) ) { OnPropertyChanged( nameof(HasError) ); }
        }
    }


    public static ModelErrorState Get( IServiceProvider provider ) => provider.GetRequiredService<ModelErrorState>();


    public virtual bool TryAddModelException( string key, Exception exception )
    {
        bool result = __state.TryAddModelException( key, exception );
        OnPropertyChanged( nameof(__state) );
        return result;
    }
    public virtual void AddModelError( string key, Exception exception, ModelMetadata metadata )
    {
        __state.AddModelError( key, exception, metadata );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual bool TryAddModelError( string key, Exception exception, ModelMetadata metadata )
    {
        bool result = __state.TryAddModelError( key, exception, metadata );
        OnPropertyChanged( nameof(__state) );
        return result;
    }
    public virtual void AddModelError( string key, string errorMessage )
    {
        __state.AddModelError( key, errorMessage );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual bool TryAddModelError( string key, string errorMessage )
    {
        bool result = __state.TryAddModelError( key, errorMessage );
        OnPropertyChanged( nameof(__state) );
        return result;
    }
    public virtual void MarkFieldValid( string key )
    {
        __state.MarkFieldValid( key );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void MarkFieldSkipped( string key )
    {
        __state.MarkFieldSkipped( key );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void Merge( ModelStateDictionary dictionary )
    {
        __state.Merge( dictionary );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void SetModelValue( string key, object? rawValue, string? attemptedValue )
    {
        __state.SetModelValue( key, rawValue, attemptedValue );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void SetModelValue( string key, ValueProviderResult valueProviderResult )
    {
        __state.SetModelValue( key, valueProviderResult );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void ClearValidationState( string key )
    {
        __state.ClearValidationState( key );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void Remove( string key )
    {
        __state.Remove( key );
        OnPropertyChanged( nameof(__state) );
    }
    public virtual void Clear()
    {
        ErrorText = null;
        __state.Clear();
        OnPropertyChanged( nameof(__state) );
    }
}



public interface IModelState<TValue>
    where TValue : IModelErrorState
{
    [CascadingParameter( Name = ModelErrorState.KEY )] public TValue State { get; set; }
}



public interface IModelState : IModelState<ModelErrorState>;
