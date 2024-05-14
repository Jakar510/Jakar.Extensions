// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:09 AM

namespace Jakar.Extensions.Blazor;


public abstract class Widget<TServices> : ComponentBase, IModelState
    where TServices : AppServices
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
    public string? ErrorText { get; set; }
    public bool    HasError  => !string.IsNullOrEmpty( ErrorText ) || ModelState.ErrorCount > 0;


    [CascadingParameter( Name = ModelStateDictionaryCascadingValueSource.KEY )] public ModelStateDictionary ModelState { get; set; } = new();
    [Inject]                                                                    public TServices            Services   { get; set; } = default!;
    [Parameter]                                                                 public string?              Title      { get; set; }


    public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}



public abstract class Widget : Widget<AppServices>;
