// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  20:08

namespace Jakar.SettingsView.Maui.Sv.Base;


public abstract class DescriptionCellBase : CellBase, ISvCellDescription
{
    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(DescriptionCellBase));
    public virtual         string?          Description { get => (string?)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }
}
