// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/06/2024  17:08

namespace Jakar.SettingsView.Blazor.Controls;


public abstract class SelectableCell : CellBase
{
    protected bool? _isToggle;

    [Parameter] public bool                          IsToggle        { get; set; }
    [Parameter] public EventCallback<bool>           IsToggleChanged { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick         { get; set; }


    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );
        await SetIsToggle( IsToggle );
    }
    public async ValueTask SetIsToggle( bool value )
    {
        if ( _isToggle == value ) { return; }

        _isToggle = value;
        await IsVisibleChanged.InvokeAsync( value );
    }
    protected async Task OnClickHandler( MouseEventArgs? args = null )
    {
        Class = Class.ToggleClass( IsToggle );
        await OnClick.Execute( args );
    }
}



public abstract class SelectableCell<T> : ValueCellBase<T>
    where T : IEquatable<T>, IComparable<T>
{
    protected bool? _isToggle;

    [Parameter] public bool                          IsToggle        { get; set; }
    [Parameter] public EventCallback<bool>           IsToggleChanged { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick         { get; set; }


    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );
        await SetIsToggle( IsToggle );
    }
    public async ValueTask SetIsToggle( bool value )
    {
        if ( _isToggle == value ) { return; }

        _isToggle = value;
        await IsVisibleChanged.InvokeAsync( value );
    }
    protected async Task OnClickHandler( MouseEventArgs? args = null )
    {
        Class = Class.ToggleClass( IsToggle );
        await OnClick.Execute( args );
    }
}
