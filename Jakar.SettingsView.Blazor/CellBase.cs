namespace Jakar.SettingsView.Blazor;


public abstract class CellBase : ComponentBase
{
    protected bool? _isVisible;
    protected bool? _isDisabled;

    [Parameter] public                                  string                      Class              { get; set; } = string.Empty;
    [Parameter] public                                  EventCallback<string>       ClassChanged       { get; set; }
    public                                              ElementReference            Control            { get; private set; }
    [Parameter] public                                  string?                     Description        { get; set; }
    [Parameter] public                                  bool                        IsDisabled         { get; set; }
    [Parameter] public                                  EventCallback<bool>         IsDisabledChanged  { get; set; }
    public                                              RenderFragment              Fragment           => Render;
    [Parameter( CaptureUnmatchedValues = true )] public Dictionary<string, object>? HtmlAttributes     { get; set; }
    [Parameter]                                  public bool                        IsVisible          { get; set; } = true;
    [Parameter]                                  public EventCallback<bool>         IsVisibleChanged   { get; set; }
    [CascadingParameter]                         public Section                     Parent             { get; set; } = null!;
    [Parameter]                                  public string?                     Title              { get; set; }
    [Parameter]                                  public EventCallback<string?>      TitleChanged       { get; set; }
    [Parameter]                                  public EventCallback<string?>      DescriptionChanged { get; set; }


    protected internal void SetElementReference( ElementReference reference ) => Control = reference;
    protected override async Task OnParametersSetAsync()
    {
        await SetIsVisible( IsVisible );
        await SetTitle( Title );
        await SetDescription( Description );
        await SetIsDisabled( IsDisabled );
    }
    protected virtual void Render( RenderTreeBuilder builder )
    {
        builder.OpenComponent( 0, GetType() );
        builder.AddMultipleAttributes( 1, HtmlAttributes );
        if ( IsDisabled ) { builder.AddAttribute( 2, SV_DISABLED ); }

        builder.AddElementReferenceCapture( 3, SetElementReference );
        BuildRenderTree( builder );
        builder.CloseComponent();
    }


    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }
    public         Task Disable()        => SetIsDisabled( true );
    public         Task Enable()         => SetIsDisabled( false );
    public async Task SetIsDisabled( bool value )
    {
        if ( _isDisabled == value ) { return; }

        _isDisabled = value;
        await IsDisabledChanged.InvokeAsync( value );

        await SetClass( value
                            ? Class + $" {SV_DISABLED}"
                            : Class.Replace( $" {SV_DISABLED}", string.Empty, StringComparison.Ordinal ) );
    }
    public async ValueTask SetIsVisible( bool value )
    {
        if ( _isVisible == value ) { return; }

        if ( value ) { OnAppearing(); }
        else { OnDisappearing(); }

        _isVisible = value;
        await IsVisibleChanged.InvokeAsync( value );
    }
    public async ValueTask SetTitle( string? value )
    {
        if ( string.Equals( Title, value, StringComparison.Ordinal ) ) { return; }

        Title = value;
        await TitleChanged.InvokeAsync( value );
    }
    public async ValueTask SetDescription( string? value )
    {
        if ( string.Equals( Description, value, StringComparison.Ordinal ) ) { return; }

        Description = value;
        await DescriptionChanged.InvokeAsync( value );
    }
    public async ValueTask SetClass( string value )
    {
        if ( string.Equals( Class, value, StringComparison.Ordinal ) ) { return; }

        Class = value;
        await ClassChanged.InvokeAsync( value );
    }
}



public abstract class CellBase<T> : CellBase
{
    [Parameter] public string?                Hint         { get; set; }
    [Parameter] public EventCallback<string?> HintChanged  { get; set; }
    [Parameter] public T?                     Value        { get; set; }
    [Parameter] public EventCallback<T>       ValueChanged { get; set; }


    public async ValueTask SetHint( string? value )
    {
        if ( string.Equals( Hint, value, StringComparison.Ordinal ) ) { return; }

        Hint = value;
        await HintChanged.InvokeAsync( value );
    }
    public async ValueTask SetValue( T? value )
    {
        if ( EqualityComparer<T>.Default.Equals( Value, value ) ) { return; }

        Value = value;
        await ValueChanged.InvokeAsync( value );
    }
}



public abstract class ContentCellBase : CellBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string?         Content      { get; set; }


    /*
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        base.BuildRenderTree( builder );

        builder.OpenElement( 1, _ElementName );

        builder.AddMultipleAttributes( 2, HtmlAttributes );
        builder.AddAttribute( 3, CLASS,    Class );
        builder.AddAttribute( 4, DISABLED, Disabled );
        builder.AddElementReferenceCapture( 5, SetElementReference );

        if ( string.IsNullOrEmpty( Content ) ) { builder.AddContent( 7, ChildContent ); }
        else { builder.AddContent( 8,                                   Content ); }

        if ( string.IsNullOrEmpty( IconCss ) is false )
        {
            builder.OpenElement( 9, SPAN );
            builder.AddAttribute( 10, CLASS, IconCss );
            builder.CloseElement();
        }

        builder.CloseElement();
    }
    */
}



public abstract class ContentCellBase<T> : CellBase<T>
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string?         Content      { get; set; }


    /*
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        base.BuildRenderTree( builder );

        builder.OpenElement( 1, _ElementName );

        builder.AddMultipleAttributes( 2, HtmlAttributes );
        builder.AddAttribute( 3, CLASS,    Class );
        builder.AddAttribute( 4, DISABLED, Disabled );
        builder.AddElementReferenceCapture( 5, SetElementReference );

        if ( string.IsNullOrEmpty( Content ) ) { builder.AddContent( 7, ChildContent ); }
        else { builder.AddContent( 8,                                   Content ); }

        if ( string.IsNullOrEmpty( IconCss ) is false )
        {
            builder.OpenElement( 9, SPAN );
            builder.AddAttribute( 10, CLASS, IconCss );
            builder.CloseElement();
        }

        builder.CloseElement();
    }
    */
}



public abstract class SelectableCell : ContentCellBase
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


    /*
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        base.BuildRenderTree( builder );

        builder.OpenElement( 1, _ElementName );

        builder.AddMultipleAttributes( 2, HtmlAttributes );
        builder.AddAttribute( 3, CLASS,    Class );
        builder.AddAttribute( 4, DISABLED, Disabled );
        builder.AddElementReferenceCapture( 5, SetElementReference );

        builder.AddAttribute( 6, ON_CLICK, EventCallback.Factory.Create<MouseEventArgs>( this, OnClickHandler ) );

        if ( string.IsNullOrEmpty( Content ) ) { builder.AddContent( 7, ChildContent ); }
        else { builder.AddContent( 8,                                   Content ); }

        if ( string.IsNullOrEmpty( IconCss ) is false )
        {
            builder.OpenElement( 9, SPAN );
            builder.AddAttribute( 10, CLASS, IconCss );
            builder.CloseElement();
        }

        builder.CloseElement();
    }
    */
}



public abstract class SelectableCell<T> : ContentCellBase<T>
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


    /*
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        base.BuildRenderTree( builder );

        builder.OpenElement( 1, _ElementName );

        builder.AddMultipleAttributes( 2, HtmlAttributes );
        builder.AddAttribute( 3, CLASS,    Class );
        builder.AddAttribute( 4, DISABLED, Disabled );
        builder.AddElementReferenceCapture( 5, SetElementReference );

        builder.AddAttribute( 6, ON_CLICK, EventCallback.Factory.Create<MouseEventArgs>( this, OnClickHandler ) );

        if ( string.IsNullOrEmpty( Content ) ) { builder.AddContent( 7, ChildContent ); }
        else { builder.AddContent( 8,                                   Content ); }

        if ( string.IsNullOrEmpty( IconCss ) is false )
        {
            builder.OpenElement( 9, SPAN );
            builder.AddAttribute( 10, CLASS, IconCss );
            builder.CloseElement();
        }

        builder.CloseElement();
    }
    */
}
