// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  16:08

namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class CellBase : ContentView, ISvCellTitle
{
    public static readonly BindableProperty TitleProperty           = BindableProperty.Create( nameof(Title),           typeof(string),   typeof(CellBase) );
    public static readonly BindableProperty OnTappedCommandProperty = BindableProperty.Create( nameof(OnTappedCommand), typeof(ICommand), typeof(CellBase) );


    public virtual  ICommand?  OnTappedCommand { get => (ICommand?)GetValue( OnTappedCommandProperty ); set => SetValue( OnTappedCommandProperty, value ); }
    public virtual  string?    Title           { get => (string?)GetValue( TitleProperty );             set => SetValue( TitleProperty,           value ); }
    public abstract WidgetType Type            { get; }


    public         void Hide()           => IsVisible = false;
    public         void Show()           => IsVisible = true;
    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }
}
