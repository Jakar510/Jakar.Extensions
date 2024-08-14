// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  09:08

using Jakar.SettingsView.Abstractions;



namespace Jakar.SettingsView.Maui.Cells;


public abstract class BorderBase : ContentView, ISectionBorder<Color>
{
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create( nameof(TextColor), typeof(Color),  typeof(BorderBase), propertyChanged: OnTextColorChanged );
    public static readonly BindableProperty TitleProperty     = BindableProperty.Create( nameof(Title),     typeof(string), typeof(BorderBase), propertyChanged: OnTitleChanged );


    public         Color   TextColor  { get => (Color)GetValue( TextColorProperty ); set => SetValue( TextColorProperty, value ); }
    public virtual string  Title      { get => (string)GetValue( TitleProperty );    set => SetValue( TitleProperty,     value ); }
    public         double  FontSize   { get;                                         set; }
    public         string? FontFamily { get;                                         set; }


    public virtual void Dispose()
    {
        GestureRecognizers.Clear();
        GC.SuppressFinalize( this );
    }
    protected static void OnTextColorChanged( BindableObject bindable, object oldValue, object value )
    {
        if ( bindable is not BorderBase header ) { return; }

        header.OnTextColorChanged( (Color?)value );
    }
    protected static void OnTitleChanged( BindableObject bindable, object oldValue, object value )
    {
        if ( bindable is not BorderBase header ) { return; }

        header.OnTitleChanged( (string)value );
    }


    protected abstract void OnTitleChanged( string     value );
    protected abstract void OnTextColorChanged( Color? value );
}
