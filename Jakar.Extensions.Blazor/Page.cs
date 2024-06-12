// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:09 AM

namespace Jakar.Extensions.Blazor;


public abstract class Widget : ComponentBase, IModelState, ILoginState
{
    [Inject, CascadingParameter( Name = ErrorState.KEY )] public ErrorState Errors { get; set; } = default!;
    [Inject, CascadingParameter( Name = LoginState.KEY )] public LoginState User   { get; set; } = default!;


    public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}



public abstract class Page : Widget
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
    [Inject]    public BlazorServices Services { get; set; } = default!;
    [Parameter] public string?        Title    { get; set; }
}
