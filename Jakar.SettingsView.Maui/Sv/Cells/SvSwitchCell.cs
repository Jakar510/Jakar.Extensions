using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvSwitchCell : ValueCellBase<bool>
{
    public sealed override WidgetType          Type      => WidgetType.Switch;
    public override        ErrorOrResult<bool> Save()    => Value;
}
