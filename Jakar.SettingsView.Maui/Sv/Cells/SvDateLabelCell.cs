using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvDateLabelCell : ValueCellBase<DateTime>
{
    public sealed override WidgetType              Type   => WidgetType.DateLabel;
    public                 string?                 Format { get; set; }
    public override        ErrorOrResult<DateTime> Save() => Value;
}
