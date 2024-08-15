using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public sealed class SvCheckBoxCell : ValueCellBase<bool>
{
    public sealed override WidgetType          Type   => WidgetType.CheckBox;
    public override        ErrorOrResult<bool> Save() => Value;
}