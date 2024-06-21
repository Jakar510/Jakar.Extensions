// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:09 AM

namespace Jakar.Extensions.Blazor;


public abstract class Page<TLoginState, TErrorState> : Widget<TLoginState, TErrorState>
    where TLoginState : class, ILoginUserState
    where TErrorState : class, IModelErrorState
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
    [Inject]    public required BlazorServices Services { get; set; }
    [Parameter] public          string?        Title    { get; set; }
}



public abstract class Page : Page<LoginUserState, ModelErrorState>;
