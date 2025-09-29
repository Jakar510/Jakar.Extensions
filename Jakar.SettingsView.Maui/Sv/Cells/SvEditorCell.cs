// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  19:08

using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvEditorCell : ValueCellBase<string>
{
    public sealed override WidgetType            Type   => WidgetType.Editor;
    public override        ErrorOrResult<string> Save() => Value ?? string.Empty;
}
