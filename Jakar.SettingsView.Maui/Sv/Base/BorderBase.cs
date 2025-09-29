// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  09:08

using Microsoft.Maui.Controls.Internals;



namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class BorderBase : ContentView, ISectionBorder<Color>, IFontElement, IDecorableTextElement
{
    public static readonly BindableProperty CharacterSpacingProperty        = BindableProperty.Create( nameof(CharacterSpacing),        typeof(double),          typeof(BorderBase), 0.0d, propertyChanged: OnCharacterSpacingChanged );
    public static readonly BindableProperty FontFamilyProperty              = BindableProperty.Create( nameof(FontFamily),              typeof(string),          typeof(BorderBase), propertyChanged: OnFontFamilyChanged );
    public static readonly BindableProperty FontSizeProperty                = BindableProperty.Create( nameof(FontSize),                typeof(double),          typeof(BorderBase), 0d,                  propertyChanged: OnFontSizeChanged, defaultValueCreator: FontSizePropertyDefaultValueCreator );
    public static readonly BindableProperty FontAttributesProperty          = BindableProperty.Create( nameof(FontAttributes),          typeof(FontAttributes),  typeof(BorderBase), FontAttributes.None, propertyChanged: OnFontAttributesChanged );
    public static readonly BindableProperty FontAutoScalingEnabledProperty  = BindableProperty.Create( nameof(FontAutoScalingEnabled),  typeof(bool),            typeof(BorderBase), true,                propertyChanged: OnFontAutoScalingEnabledChanged );
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create( nameof(HorizontalTextAlignment), typeof(TextAlignment),   typeof(BorderBase), TextAlignment.Start, propertyChanged: OnHorizontalTextAlignmentChanged );
    public static readonly BindableProperty VerticalTextAlignmentProperty   = BindableProperty.Create( nameof(VerticalTextAlignment),   typeof(TextAlignment),   typeof(BorderBase), TextAlignment.Center );
    public static readonly BindableProperty TextColorProperty               = BindableProperty.Create( nameof(TextColor),               typeof(Color),           typeof(BorderBase), propertyChanged: OnTextColorChanged );
    public static readonly BindableProperty TitleProperty                   = BindableProperty.Create( nameof(Title),                   typeof(string),          typeof(BorderBase), propertyChanged: OnTitleChanged );
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


    private static   object FontSizePropertyDefaultValueCreator( BindableObject bindable )                                     => ((BorderBase)bindable).FontSizeDefaultValueCreator();
    public virtual   double FontSizeDefaultValueCreator()                                                                      => 0;
    private static   void   OnCharacterSpacingChanged( BindableObject        bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnCharacterSpacingChanged( (double)oldValue, (double)value );
    public abstract  void   OnCharacterSpacingChanged( double                oldValue, double         value );
    private static   void   OnFontFamilyChanged( BindableObject              bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnFontFamilyChanged( (string)oldValue, (string)value );
    public abstract  void   OnFontFamilyChanged( string                      oldValue, string         value );
    private static   void   OnFontSizeChanged( BindableObject                bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnFontSizeChanged( (double)oldValue, (double)value );
    public abstract  void   OnFontSizeChanged( double                        oldValue, double         value );
    private static   void   OnFontAutoScalingEnabledChanged( BindableObject  bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnFontAutoScalingEnabledChanged( (bool)oldValue, (bool)value );
    public abstract  void   OnFontAutoScalingEnabledChanged( bool            oldValue, bool           value );
    private static   void   OnFontAttributesChanged( BindableObject          bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnFontAttributesChanged( (FontAttributes)oldValue, (FontAttributes)value );
    public abstract  void   OnFontAttributesChanged( FontAttributes          oldValue, FontAttributes value );
    protected static void   OnTitleChanged( BindableObject                   bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnTitleChanged( (string)oldValue, (string)value );
    public abstract  void   OnTitleChanged( string                           oldValue, string         value );
    protected static void   OnHorizontalTextAlignmentChanged( BindableObject bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnHorizontalTextAlignmentChanged( (TextAlignment)oldValue, (TextAlignment)value );
    public abstract  void   OnHorizontalTextAlignmentChanged( TextAlignment  oldValue, TextAlignment  value );
    protected static void   OnVerticalTextAlignmentChanged( BindableObject   bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnVerticalTextAlignmentChanged( (TextAlignment)oldValue, (TextAlignment)value );
    public abstract  void   OnVerticalTextAlignmentChanged( TextAlignment    oldValue, TextAlignment  value );
    protected static void   OnTextColorChanged( BindableObject               bindable, object         oldValue, object value ) => ((BorderBase)bindable).OnTextColorChanged( (Color?)oldValue, (Color?)value );
    public abstract  void   OnTextColorChanged( Color?                       oldValue, Color?         value );
}
