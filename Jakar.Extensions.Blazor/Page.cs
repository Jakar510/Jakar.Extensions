// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:09 AM

using Blazored.Modal;



namespace Jakar.Extensions.Blazor;


public abstract class Page : Widget
{
    private IModalReference? __popup;


    protected IModalReference? _Popup
    {
        get => __popup;
        set
        {
            __popup?.Close();
            __popup = value;
        }
    }
    [Inject]    public required BlazorServices Services { get; set; }
    [Parameter] public          string?        Title    { get; set; }
}
