namespace Jakar.SettingsView.Maui.Cells;


public class DateLabelCell : ValueCellBase<DateTime>
{
    public sealed override WidgetType              Type   => WidgetType.DateLabel;
    public                 string?                 Format { get; set; }
    public override        ErrorOrResult<DateTime> Save() => Value;
}
