namespace Jakar.SettingsView.Maui.Cells;


public class DatePickerCell : CellBase<DateTime>
{
    public sealed override WidgetType              Type   => WidgetType.Date;
    public override        ErrorOrResult<DateTime> Save() => Value;
}
