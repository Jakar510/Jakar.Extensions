// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/13/2024  20:06

namespace Jakar.Extensions.Blazor;


public interface IWidget<TLoginState, TErrorState> : IModelState<TErrorState>, ILoginState<TLoginState>
    where TLoginState :  ILoginUserState
    where TErrorState :  IModelErrorState;






public abstract class Widget<TLoginState, TErrorState> : ComponentBase, IWidget<TLoginState, TErrorState>
    where TLoginState :  ILoginUserState
    where TErrorState :  IModelErrorState
{
    [CascadingParameter( Name = ModelErrorState.KEY )] public required TErrorState State { get; set; }
    [CascadingParameter( Name = LoginUserState.KEY )]  public required TLoginState User  { get; set; }


    // public Task StateHasChangedAsync() => InvokeAsync( StateHasChanged );
}



public abstract class Widget : Widget<LoginUserState, ModelErrorState> { }