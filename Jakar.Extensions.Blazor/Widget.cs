// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:09 AM

namespace Jakar.Extensions.Blazor;


public abstract class Widget<TServices> : ComponentBase, IModelState where TServices : AppServices
{
    private IModalReference? _popup;


    protected IModalReference? _Popup
    {
        get => _popup;
        set
        {
            _popup?.Close();
            _popup = value;
        }
    }
    public string  BaseUri   => Services.BaseUri;
    public string? ErrorText { get; set; }
    public bool    HasError  => !string.IsNullOrEmpty( ErrorText ) || ModelState.ErrorCount > 0;


    [CascadingParameter( Name = nameof(ModelStateDictionary) )] public ModelStateDictionary ModelState { get; set; } = new();

    // ReSharper disable once NullableWarningSuppressionIsUsed
    [Inject] public TServices Services { get; set; } = default!;


    [Parameter] public string? Title { get; set; }
    public             string  Uri   => Services.Uri;


    public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}



public abstract class Widget : Widget<AppServices> { }
