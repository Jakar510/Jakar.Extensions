using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvDateLabelCell : ValueCellBase<DateTime>
{
    public                 string?                 Format { get; set; }
    public sealed override WidgetType              Type   => WidgetType.DateLabel;
    public override        ErrorOrResult<DateTime> Save() => Value;
}
