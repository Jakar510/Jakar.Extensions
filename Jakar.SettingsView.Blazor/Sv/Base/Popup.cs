// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/06/2024  21:08

using Microsoft.JSInterop;
using Radzen;



namespace Jakar.SettingsView.Blazor.Sv;


public class Popup<TValue> : RadzenComponent
{
    private   bool                       __open;
    protected ElementReference           _target;
    protected TValue?                    _value;
    protected IList<TValue>?             _values;
    protected EventCallback<TValue?>     _valueChanged;
    protected Expression<Func<TValue?>>? _valueExpression;


    public                                      TValue?               Value          => _value;
    [Parameter]                 public          RenderFragment        ChildContent   { get; set; }
    [Parameter]                 public          EventCallback<TValue> Close          { get; set; }
    [Parameter]                 public          bool                  Lazy           { get; set; }
    [Parameter]                 public          EventCallback<TValue> Open           { get; set; }
    [Parameter]                 public          bool                  PreventDefault { get; set; }
    [Parameter, EditorRequired] public required string                ID             { get; set; }


    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder.OpenElement( 0, "div" );
        builder.AddEventPreventDefaultAttribute( 1, "onmousedown", PreventDefault );
        builder.AddMultipleAttributes( 2, Attributes );
        builder.AddAttribute( 3, "style", Style );
        builder.AddAttribute( 4, "id",    GetId() );
        builder.AddElementReferenceCapture( 5, value => Element = value );
        builder.AddMarkupContent( 6, "\r\n" );
        if ( __open || !Lazy ) { builder.AddContent( 7, ChildContent ); }

        builder.CloseElement();
    }


    public async Task ToggleAsync( ElementReference target )
    {
        if ( __open ) { await CloseAsync( target ); }
        else { await OpenAsync( target ); }
    }
    public async Task OpenAsync( ElementReference target, IList<TValue> values, EventCallback<TValue?> changed, Expression<Func<TValue?>>? expression )
    {
        _values          = values;
        _value           = default;
        _valueChanged    = changed;
        _valueExpression = expression;
        await InvokeAsync( StateHasChanged );
        await OpenAsync( target );
    }
    public Task OpenAsync( ElementReference target )
    {
        _target = target;
        return OpenAsync();
    }
    public async Task OpenAsync()
    {
        __open = true;
        await Open.InvokeAsync( _value );

        await JSRuntime.InvokeVoidAsync( "Radzen.openPopup",
                                         _target,
                                         GetId(),
                                         false,
                                         null,
                                         null,
                                         null,
                                         Reference,
                                         nameof(OnClose),
                                         true,
                                         true );
    }
    public async Task CloseAsync( ElementReference target )
    {
        _target = target;
        await CloseAsync();
    }
    public async Task CloseAsync()
    {
        __open = false;
        await Close.InvokeAsync( _value );
        await JSRuntime.InvokeVoidAsync( "Radzen.closePopup", GetId(), Reference, nameof(OnClose) );
    }
    protected async Task CloseAsync( TValue value )
    {
        _value = value;
        await _valueChanged.InvokeAsync( value );
        await CloseAsync();
    }


    [JSInvokable]
    public async Task OnClose()
    {
        __open = false;
        await Close.InvokeAsync( default );
    }
}
