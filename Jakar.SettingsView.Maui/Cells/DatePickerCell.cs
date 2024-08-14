namespace Jakar.SettingsView.Maui.Cells;


public class DatePickerCell : ValueCellBase<DateTime>
{
    public sealed override WidgetType              Type   => WidgetType.Date;
    public override        ErrorOrResult<DateTime> Save() => Value;
}
