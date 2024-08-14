namespace Jakar.SettingsView.Maui.Cells;


public class EntryCell : ValueCellBase<string>
{
    public sealed override WidgetType            Type   => WidgetType.TextEntry;
    public override        ErrorOrResult<string> Save() => Value ?? string.Empty;
}