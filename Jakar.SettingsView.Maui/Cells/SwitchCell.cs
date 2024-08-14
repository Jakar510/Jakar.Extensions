namespace Jakar.SettingsView.Maui.Cells;


public class SwitchCell : ValueCellBase<bool>
{
    public sealed override WidgetType          Type      => WidgetType.Switch;
    public override        ErrorOrResult<bool> Save()    => Value;
}
