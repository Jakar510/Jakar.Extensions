// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  19:08

namespace Jakar.SettingsView.Maui.Cells;


public abstract class RadioCell<T> : CellBase<T>
    where T : IEquatable<T>, IComparable<T>
{
    public sealed override WidgetType Type => WidgetType.Radio;
}



public class RadioCell : RadioCell<string>
{
    public override ErrorOrResult<string> Save() => Value ?? string.Empty;
}
