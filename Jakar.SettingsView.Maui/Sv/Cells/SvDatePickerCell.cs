using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvDatePickerCell : ValueCellBase<DateTime>
{
    public sealed override WidgetType              Type   => WidgetType.Date;
    public override        ErrorOrResult<DateTime> Save() => Value;
}
