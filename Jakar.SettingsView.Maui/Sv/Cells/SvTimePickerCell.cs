using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvTimePickerCell : ValueCellBase<TimeSpan>
{
    public sealed override WidgetType              Type   => WidgetType.Time;
    public override ErrorOrResult<TimeSpan> Save() => Value;
}
