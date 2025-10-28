using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvNumberEntryCell : ValueCellBase<string>
{
    public sealed override WidgetType            Type   => WidgetType.NumberEntry;
    public override        ErrorOrResult<string> Save() => Value ?? EMPTY;
}
