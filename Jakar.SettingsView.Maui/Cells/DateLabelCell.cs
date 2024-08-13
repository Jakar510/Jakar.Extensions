namespace Jakar.SettingsView.Maui.Cells;


public class DateLabelCell : CellBase<DateTime>
{
    public sealed override WidgetType              Type   => WidgetType.DateLabel;
    public                 string?                 Format { get; set; }
    public override        ErrorOrResult<DateTime> Save() => Value;
}
