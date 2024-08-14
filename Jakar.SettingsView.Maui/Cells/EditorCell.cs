// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  19:08

namespace Jakar.SettingsView.Maui.Cells;


public class EditorCell : ValueCellBase<string>
{
    public sealed override WidgetType            Type   => WidgetType.Editor;
    public override        ErrorOrResult<string> Save() => Value ?? string.Empty;
}
