// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  16:08

namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class CellBase : ContentView, ISvCellTitle, IEquatable<CellBase>
{
    public static readonly BindableProperty OnTappedCommandProperty = BindableProperty.Create(nameof(OnTappedCommand), typeof(ICommand), typeof(CellBase));
    public static readonly BindableProperty TitleProperty           = BindableProperty.Create(nameof(Title),           typeof(string),   typeof(CellBase));


    public virtual  ICommand?  OnTappedCommand { get => (ICommand?)GetValue(OnTappedCommandProperty); set => SetValue(OnTappedCommandProperty, value); }
    public virtual  string?    Title           { get => (string?)GetValue(TitleProperty);             set => SetValue(TitleProperty,           value); }
    public abstract WidgetType Type            { get; }


    public         void Hide()           => IsVisible = false;
    public         void Show()           => IsVisible = true;
    public virtual void OnAppearing()    { }
    public virtual void OnDisappearing() { }


    public virtual bool Equals( CellBase? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        if ( other.GetType() != GetType() ) { return false; }

        return Type == other.Type;
    }
    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || ( obj is CellBase cell && Equals(cell) );
    public override int  GetHashCode()         => HashCode.Combine(Type);
}
