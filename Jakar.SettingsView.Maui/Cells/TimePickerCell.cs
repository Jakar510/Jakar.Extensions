namespace Jakar.SettingsView.Maui.Cells;


public class TimePickerCell : CellBase<TimeSpan>
{
    public sealed override WidgetType              Type   => WidgetType.Time;
    public override ErrorOrResult<TimeSpan> Save() => Value;
}
