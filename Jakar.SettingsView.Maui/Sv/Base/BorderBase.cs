// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  09:08

using Microsoft.Maui.Controls.Internals;



namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class BorderBase : ContentView, ISectionBorder<Color>, IFontElement, IDecorableTextElement
{
    public static readonly BindableProperty CharacterSpacingProperty        = BindableProperty.Create( nameof(CharacterSpacing),        typeof(double),          typeof(BorderBase), 0.0d, propertyChanged: OnCharacterSpacingPropertyChanged );
    public static readonly BindableProperty FontFamilyProperty              = BindableProperty.Create( nameof(FontFamily),              typeof(string),          typeof(BorderBase), propertyChanged: OnFontFamilyPropertyChanged );
    public static readonly BindableProperty FontSizeProperty                = BindableProperty.Create( nameof(FontSize),                typeof(double),          typeof(BorderBase), 0d,                  propertyChanged: OnFontSizePropertyChanged, defaultValueCreator: FontSizePropertyDefaultValueCreator );
    public static readonly BindableProperty FontAttributesProperty          = BindableProperty.Create( nameof(FontAttributes),          typeof(FontAttributes),  typeof(BorderBase), FontAttributes.None, propertyChanged: OnFontAttributesPropertyChanged );
    public static readonly BindableProperty FontAutoScalingEnabledProperty  = BindableProperty.Create( nameof(FontAutoScalingEnabled),  typeof(bool),            typeof(BorderBase), true,                propertyChanged: OnFontAutoScalingEnabledPropertyChanged );
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create( nameof(HorizontalTextAlignment), typeof(TextAlignment),   typeof(BorderBase), TextAlignment.Start, propertyChanged: OnHorizontalTextAlignmentPropertyChanged );
    public static readonly BindableProperty VerticalTextAlignmentProperty   = BindableProperty.Create( nameof(VerticalTextAlignment),   typeof(TextAlignment),   typeof(BorderBase), TextAlignment.Center );
    public static readonly BindableProperty TextColorProperty               = BindableProperty.Create( nameof(TextColor),               typeof(Color),           typeof(BorderBase), propertyChanged: OnTextColorPropertyChanged );
    public static readonly BindableProperty TitleProperty                   = BindableProperty.Create( nameof(Title),                   typeof(string),          typeof(BorderBase), propertyChanged: OnTitlePropertyChanged );
    public static readonly BindableProperty TextDecorationsProperty         = BindableProperty.Create( nameof(TextDecorations),         typeof(TextDecorations), typeof(BorderBase), TextDecorations.None );


    public                                              double          CharacterSpacing        { get => (double)GetValue( CharacterSpacingProperty );               set => SetValue( CharacterSpacingProperty,        value ); }
    public                                              FontAttributes  FontAttributes          { get => (FontAttributes)GetValue( FontAttributesProperty );         set => SetValue( FontAttributesProperty,          value ); }
    public                                              bool            FontAutoScalingEnabled  { get => (bool)GetValue( FontAutoScalingEnabledProperty );           set => SetValue( FontAutoScalingEnabledProperty,  value ); }
    public                                              string?         FontFamily              { get => (string?)GetValue( FontFamilyProperty );                    set => SetValue( FontFamilyProperty,              value ); }
    [TypeConverter( typeof(FontSizeConverter) )] public double          FontSize                { get => (double)GetValue( FontSizeProperty );                       set => SetValue( FontSizeProperty,                value ); }
    public                                              TextAlignment   HorizontalTextAlignment { get => (TextAlignment)GetValue( HorizontalTextAlignmentProperty ); set => SetValue( HorizontalTextAlignmentProperty, value ); }
    public                                              Color           TextColor               { get => (Color)GetValue( TextColorProperty );                       set => SetValue( TextColorProperty,               value ); }
    public                                              TextDecorations TextDecorations         { get => (TextDecorations)GetValue( TextDecorationsProperty );       set => SetValue( TextDecorationsProperty,         value ); }
    public virtual                                      string          Title                   { get => (string)GetValue( TitleProperty );                          set => SetValue( TitleProperty,                   value ); }
    public                                              TextAlignment   VerticalTextAlignment   { get => (TextAlignment)GetValue( VerticalTextAlignmentProperty );   set => SetValue( VerticalTextAlignmentProperty,   value ); }


    public virtual void Dispose()
    {
        var label = this.FindByName<Label>( nameof(Title) );

        GestureRecognizers.Clear();
        GC.SuppressFinalize( this );
    }


    private static   object FontSizePropertyDefaultValueCreator( BindableObject      bindable )                                => ((BorderBase)bindable).FontSizeDefaultValueCreator();
    private static   void   OnCharacterSpacingPropertyChanged( BindableObject        bindable, object oldValue, object value ) => ((BorderBase)bindable).OnCharacterSpacingChanged( (double)oldValue, (double)value );
    private static   void   OnFontFamilyPropertyChanged( BindableObject              bindable, object oldValue, object value ) => ((BorderBase)bindable).OnFontFamilyChanged( (string)oldValue, (string)value );
    private static   void   OnFontSizePropertyChanged( BindableObject                bindable, object oldValue, object value ) => ((BorderBase)bindable).OnFontSizeChanged( (double)oldValue, (double)value );
    private static   void   OnFontAttributesPropertyChanged( BindableObject          bindable, object oldValue, object value ) => ((BorderBase)bindable).OnFontAttributesChanged( (FontAttributes)oldValue, (FontAttributes)value );
    private static   void   OnFontAutoScalingEnabledPropertyChanged( BindableObject  bindable, object oldValue, object value ) => ((BorderBase)bindable).OnFontAutoScalingEnabledChanged( (bool)oldValue, (bool)value );
    protected static void   OnVerticalTextAlignmentPropertyChanged( BindableObject   bindable, object oldValue, object value ) => ((BorderBase)bindable).OnVerticalTextAlignmentChanged( (TextAlignment)oldValue, (TextAlignment)value );
    protected static void   OnHorizontalTextAlignmentPropertyChanged( BindableObject bindable, object oldValue, object value ) => ((BorderBase)bindable).OnHorizontalTextAlignmentChanged( (TextAlignment)oldValue, (TextAlignment)value );
    protected static void   OnTextColorPropertyChanged( BindableObject               bindable, object oldValue, object value ) => ((BorderBase)bindable).OnTextColorChanged( (Color?)oldValue, (Color?)value );
    protected static void   OnTitlePropertyChanged( BindableObject                   bindable, object oldValue, object value ) => ((BorderBase)bindable).OnTitleChanged( (string)oldValue, (string)value );


    public virtual  double FontSizeDefaultValueCreator() => 0;
    public abstract void   OnCharacterSpacingChanged( double               oldValue, double         value );
    public abstract void   OnFontFamilyChanged( string                     oldValue, string         value );
    public abstract void   OnFontSizeChanged( double                       oldValue, double         value );
    public abstract void   OnFontAutoScalingEnabledChanged( bool           oldValue, bool           value );
    public abstract void   OnFontAttributesChanged( FontAttributes         oldValue, FontAttributes value );
    public abstract void   OnTitleChanged( string                          oldValue, string         value );
    public abstract void   OnHorizontalTextAlignmentChanged( TextAlignment oldValue, TextAlignment  value );
    public abstract void   OnVerticalTextAlignmentChanged( TextAlignment   oldValue, TextAlignment  value );
    public abstract void   OnTextColorChanged( Color?                      oldValue, Color?         value );
}
