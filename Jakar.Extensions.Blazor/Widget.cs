// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/13/2024  20:06

namespace Jakar.Extensions.Blazor;


public abstract class Widget : ComponentBase, IModelState, ILoginState
{
    [CascadingParameter( Name = ModelErrorState.KEY )] public required ModelErrorState State { get; set; }
    [CascadingParameter( Name = LoginUserState.KEY )]  public required LoginUserState  User  { get; set; }


    // public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}
