// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  19:08

using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public abstract class SvRadioCell<T> : ValueCellBase<T>
    where T : IEquatable<T>, IComparable<T>
{
    public sealed override WidgetType Type => WidgetType.Radio;
}



public class RadioCell : SvRadioCell<string>
{
    public override ErrorOrResult<string> Save() => Value ?? string.Empty;
}
