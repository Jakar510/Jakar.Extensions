namespace Jakar.SettingsView.Blazor;


public abstract class CellBase : ComponentBase
{
    private bool _isVisible;

    [Parameter] public                                  string                      Class            { get; set; } = string.Empty;
    [Parameter] public                                  EventCallback<string>       ClassChanged     { get; set; }
    public                                              ElementReference            Control          { get; private set; }
    [Parameter] public                                  string?                     Description      { get; set; }
    [Parameter] public                                  bool                        Disabled         { get; set; }
    public                                              RenderFragment              Fragment         => Render;
    [Parameter( CaptureUnmatchedValues = true )] public Dictionary<string, object>? HtmlAttributes   { get; set; }
    [Parameter]                                  public bool                        IsVisible        { get; set; } = true;
    [Parameter]                                  public EventCallback<bool>         IsVisibleChanged { get; set; }
    [CascadingParameter]                         public Section                     Parent           { get; set; } = null!;
    [Parameter]                                  public string?                     Title            { get; set; }


    public async Task SetIsVisible( bool value )
    {
        if ( _isVisible == value ) { return; }

        if ( value ) { OnAppearing(); }
        else { OnDisappearing(); }

        _isVisible = value;
        await IsVisibleChanged.InvokeAsync( value );
    }
    protected override async Task OnParametersSetAsync() { await SetIsVisible( IsVisible ); }
    public virtual           void OnAppearing()          { }
    public virtual           void OnDisappearing()       { }


    protected virtual void Render( RenderTreeBuilder builder )
    {
        builder.OpenComponent( 0, GetType() );
        builder.CloseComponent();
    }


    public void Disable()
    {
        Disabled =  true;
        Class    += DISABLED;
    }
    public void Enable()
    {
        Disabled = false;
        Class    = Class.Replace( DISABLED, string.Empty, StringComparison.Ordinal );
    }

    public async Task SetClass( string value )
    {
        if ( Class == value ) { return; }

        Class = value;
        await ClassChanged.InvokeAsync( value );
    }
    protected internal void SetElementReference( ElementReference reference ) => Control = reference;
}



public abstract class ContentCellBase : CellBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string?         Content      { get; set; }
    [Parameter] public string?         IconCss      { get; set; }


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
    [Parameter] public bool                          IsToggle { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick  { get; set; }


    protected async Task OnClickHandler( MouseEventArgs? args = null )
    {
        if ( IsToggle )
        {
            Class = Class.Contains( "sv-active", StringComparison.Ordinal ) is false
                        ? Class + " sv-active"
                        : Class.Replace( " sv-active", string.Empty, StringComparison.Ordinal );
        }

        if ( args is null ) { return; }

        EventCallback<MouseEventArgs> eventCallback = OnClick;
        if ( eventCallback.HasDelegate ) { await eventCallback.InvokeAsync( args ); }
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



public abstract class CellBase<T> : CellBase
{
    [Parameter] public string? Hint  { get; set; }
    [Parameter] public T?      Value { get; set; }


    public event EventHandler<T?>? ValueChanged;


    public void SetValue( T? value )
    {
        if ( EqualityComparer<T>.Default.Equals( Value, value ) ) { return; }

        Value = value;
        ValueChanged?.Invoke( this, value );
    }
}



public abstract class ContentCellBase<T> : CellBase<T>
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string?         Content      { get; set; }
    [Parameter] public string?         IconCss      { get; set; }


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



public abstract class SelectableCell<T> : ContentCellBase<T>
{
    [Parameter] public bool                          IsToggle { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick  { get; set; }


    protected async Task OnClickHandler( MouseEventArgs? args = null )
    {
        if ( IsToggle )
        {
            Class = Class.Contains( "sv-active", StringComparison.Ordinal ) is false
                        ? Class + " sv-active"
                        : Class.Replace( " sv-active", string.Empty, StringComparison.Ordinal );
        }

        if ( args is null ) { return; }

        EventCallback<MouseEventArgs> eventCallback = OnClick;
        if ( eventCallback.HasDelegate ) { await eventCallback.InvokeAsync( args ); }
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
