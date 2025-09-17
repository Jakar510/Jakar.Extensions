﻿// Jakar.Extensions :: Jakar.Extensions.Blazor
// 06/13/2024  20:06

using System.Linq.Expressions;



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



public class BlazorSetting<T>( T value ) : ObservableClass
{
    private T __value = value;
    public T Value
    {
        get => __value;
        set
        {
            SetProperty(ref __value, value);
            _ = ValueChanged.InvokeAsync(value);
        }
    }
    public EventCallback<T>     ValueChanged    { get; set; }
    public Expression<Func<T>>? ValueExpression { get; set; }
}
