namespace Jakar.SettingsView.Maui.Cells;


public class NumberEntryCell : ValueCellBase<string>
{
    public sealed override WidgetType            Type   => WidgetType.NumberEntry;
    public override        ErrorOrResult<string> Save() => Value ?? string.Empty;
}
