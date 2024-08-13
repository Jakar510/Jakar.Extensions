namespace Jakar.SettingsView.Maui.Cells;


public class SwitchCell : CellBase<bool>
{
    public sealed override WidgetType          Type      => WidgetType.Switch;
    public override        ErrorOrResult<bool> Save()    => Value;
}
