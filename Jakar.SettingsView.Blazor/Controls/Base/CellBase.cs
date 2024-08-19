﻿using System.ComponentModel;
using System.Globalization;
using Jakar.SettingsView.Abstractions;



namespace Jakar.SettingsView.Blazor.Controls;


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



public abstract class CellBase : ComponentBase, ISvCellTitle
{
    protected bool? _isVisible;
    protected bool? _isDisabled;


    protected                                                       ElementReference                     _control;
    [Parameter] public                                              string                               ID                  { get; set; } = Guid.NewGuid().ToString();
    [Parameter] public                                              string                               Class               { get; set; } = string.Empty;
    [Parameter] public                                              EventCallback<string>                ClassChanged        { get; set; }
    [Parameter] public                                              bool                                 IsDisabled          { get; set; }
    [Parameter] public                                              EventCallback<bool>                  IsDisabledChanged   { get; set; }
    public                                                          RenderFragment                       Fragment            => Render;
    [Parameter( CaptureUnmatchedValues = true )] public             IReadOnlyDictionary<string, object>? HtmlAttributes      { get; set; }
    [CascadingParameter]                         protected internal EditContext?                         CascadedEditContext { get; set; }
    [Parameter]                                  public             bool                                 IsVisible           { get; set; } = true;
    [Parameter]                                  public             EventCallback<bool>                  IsVisibleChanged    { get; set; }
    [CascadingParameter]                         public             SvSection                            Section             { get; set; } = null!;
    [Parameter]                                  public             string?                              Title               { get; set; }
    [Parameter]                                  public             EventCallback<string?>               TitleChanged        { get; set; }
    [Parameter]                                  public             Expression<Func<string?>>?           TitleExpression     { get; set; }
    [Parameter]                                  public             bool                                 IsEnabled           { get; set; }
    [Parameter]                                  public             CultureInfo?                         Culture             { get; set; }
    [CascadingParameter]                         public             CultureInfo?                         DefaultCulture      { get; set; }
    public                                                          CultureInfo                          CurrentCulture      => Culture ?? DefaultCulture ?? CultureInfo.CurrentUICulture;


    protected internal void SetElementReference( ElementReference reference ) => _control = reference;
    protected override async Task OnParametersSetAsync()
    {
        await SetIsVisible( IsVisible );
        await SetTitle( Title );
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
    public async ValueTask SetClass( string value )
    {
        if ( string.Equals( Class, value, StringComparison.Ordinal ) ) { return; }

        Class = value;
        await ClassChanged.InvokeAsync( value );
    }
}
