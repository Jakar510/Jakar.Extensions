// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/13/2024  20:06

namespace Jakar.Extensions.Blazor;


public interface IWidget<TLoginState, TErrorState> : IModelState<TErrorState>, ILoginState<TLoginState>
    where TLoginState : ILoginUserState
    where TErrorState : IModelErrorState;



public interface IWidget : IWidget<LoginUserState, ModelErrorState>, IModelState, ILoginState;



public abstract class Widget : ComponentBase, IWidget
{
    [CascadingParameter( Name = ModelErrorState.KEY )] public required ModelErrorState State { get; set; }
    [CascadingParameter( Name = LoginUserState.KEY )]  public required LoginUserState  User  { get; set; }

    // public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}
