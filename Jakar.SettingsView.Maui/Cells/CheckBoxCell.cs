namespace Jakar.SettingsView.Maui.Cells;


public sealed class CheckBoxCell : ValueCellBase<bool>
{
    public sealed override WidgetType          Type   => WidgetType.CheckBox;
    public override        ErrorOrResult<bool> Save() => Value;
}