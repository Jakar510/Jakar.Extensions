using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public class SvEntryCell : ValueCellBase<string>
{
    public sealed override WidgetType            Type   => WidgetType.TextEntry;
    public override        ErrorOrResult<string> Save() => Value ?? EMPTY;
}
