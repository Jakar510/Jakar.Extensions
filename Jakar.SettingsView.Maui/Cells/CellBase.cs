// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  16:08

using PropertyChangingEventHandler = System.ComponentModel.PropertyChangingEventHandler;



namespace Jakar.SettingsView.Maui.Cells;


public abstract class CellBase : ContentView, ICellData
{
    // public event PropertyChangingEventHandler? PropertyChanging;
    public virtual  ICommand?  OnTappedCommand { get; set; }
    public abstract WidgetType Type            { get; }
    public virtual  string?    ValueText       { get; set; }
    public virtual  string?    Title           { get; set; }
    public virtual  string?    Description     { get; set; }


    public         void Hide()           => IsVisible = false;
    public         void Show()           => IsVisible = true;
    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }
}



public abstract class CellBase<T> : CellBase, IValidateCell<T>
    where T : IEquatable<T>, IComparable<T>
{
    public          T?               Max   { get; init; }
    public          T?               Min   { get; init; }
    public          T?               Value { get; set; }
    public abstract ErrorOrResult<T> Save();
}
