// Jakar.Extensions :: Jakar.Extensions.Blazor
// 04/25/2024  11:04

namespace Jakar.Extensions.Blazor;


public class ErrorState : ObservableClass
{
    public const string  KEY = nameof(ErrorState);
    private      string? _errorText;


    public string? ErrorText
    {
        get => _errorText;
        set
        {
            if ( SetProperty( ref _errorText, value ) ) { OnPropertyChanged( nameof(HasError) ); }
        }
    }
    public bool                 HasError => !string.IsNullOrEmpty( ErrorText ) || State.ErrorCount > 0;
    public ModelStateDictionary State    { get; } = new();


    public static ErrorState Get( IServiceProvider provider ) => provider.GetRequiredService<ErrorState>();
}



public interface IModelState
{
    [CascadingParameter( Name = ErrorState.KEY )] public ErrorState Errors { get; set; }
}
