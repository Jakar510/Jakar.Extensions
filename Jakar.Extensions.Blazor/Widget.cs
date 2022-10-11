// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:09 AM

namespace Jakar.Extensions.Blazor;


public abstract class Widget<TServices> : ComponentBase, IModelState where TServices : AppServices
{
    private IModalReference? _popup;
    public  bool             HasError => !string.IsNullOrEmpty( ErrorText ) || ModelState.ErrorCount > 0;


    protected IModalReference? _Popup
    {
        get => _popup;
        set
        {
            _popup?.Close();
            _popup = value;
        }
    }


    [CascadingParameter( Name = nameof(ModelStateDictionary) )] public ModelStateDictionary ModelState { get; set; } = new();
    public                                                             string               BaseUri    => Services.BaseUri;
    public                                                             string               Uri        => Services.Uri;
    public                                                             string?              ErrorText  { get; set; }


    [Parameter] public string? Title { get; set; }


    [Inject] public TServices Services { get; set; } = default!;


    public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}



public abstract class Widget : Widget<AppServices> { }
