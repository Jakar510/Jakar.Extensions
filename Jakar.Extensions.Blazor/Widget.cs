// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/13/2024  20:06

namespace Jakar.Extensions.Blazor;


public abstract class Widget : ComponentBase, IModelState, ILoginState
{
    [Inject, CascadingParameter( Name = ErrorState.KEY )] public ErrorState Errors { get; set; } = default!;
    [Inject, CascadingParameter( Name = LoginState.KEY )] public LoginState User   { get; set; } = default!;


    public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}
