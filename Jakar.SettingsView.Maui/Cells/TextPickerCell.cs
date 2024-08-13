namespace Jakar.SettingsView.Maui.Cells;


public class TextPickerCell : PickerCell<string>
{
    public sealed override WidgetType            Type   => WidgetType.TextPicker;
    public override        ErrorOrResult<string> Save() => Value ?? string.Empty;
}
