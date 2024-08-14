// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  20:08

namespace Jakar.SettingsView.Maui.Cells;


public abstract class DescriptionCellBase : CellBase, ISvCellDescription
{
    public virtual string? Description { get; set; }
}
