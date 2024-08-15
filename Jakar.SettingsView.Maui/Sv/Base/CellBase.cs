// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  16:08

namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class CellBase : ContentView, ISvCellTitle
{
    // public event PropertyChangingEventHandler? PropertyChanging;
    public virtual  ICommand?  OnTappedCommand { get; set; }
    public virtual  string?    Title           { get; set; }
    public abstract WidgetType Type            { get; }
    public virtual  string?    ValueText       { get; set; }


    public         void Hide()           => IsVisible = false;
    public         void Show()           => IsVisible = true;
    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }
}