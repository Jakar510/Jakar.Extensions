namespace Jakar.SettingsView.Maui.Sv;


public class SvTextPickerCell : SvPickerCell<string>
{
    public sealed override WidgetType            Type   => WidgetType.TextPicker;
    public override        ErrorOrResult<string> Save() => Value ?? EMPTY;
}
