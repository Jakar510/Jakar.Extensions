// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/06/2024  17:08

namespace Jakar.SettingsView.Blazor.Sv;


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



public abstract class SelectableCell<TValue, TArgs> : ValueCellBase<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
    where TArgs : class
{
    protected bool? _isToggle;

    [Parameter] public bool?                IsToggle        { get; set; }
    [Parameter] public EventCallback<bool>  IsToggleChanged { get; set; }
    [Parameter] public EventCallback<TArgs> OnClick         { get; set; }


    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );
        await SetIsToggle( IsToggle );
    }
    public async ValueTask SetIsToggle( bool? value )
    {
        if ( Nullable.Equals( _isToggle, value ) ) { return; }

        _isToggle = value;
        if ( value.HasValue ) { await IsVisibleChanged.InvokeAsync( value.Value ); }
    }
    protected virtual async Task OnClickHandler( TArgs? args = null )
    {
        if ( IsToggle.HasValue ) { Class = Class.ToggleClass( IsToggle.Value ); }

        await OnClick.Execute( args );
    }
}



public abstract class SelectableCell<TValue> : SelectableCell<TValue, MouseEventArgs>
    where TValue : IEquatable<TValue>, IComparable<TValue>;
