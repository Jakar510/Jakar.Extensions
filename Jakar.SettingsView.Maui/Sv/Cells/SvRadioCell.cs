// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/12/2024  19:08

using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public abstract class SvRadioCell<TValue> : ValueCellBase<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public sealed override WidgetType Type => WidgetType.Radio;
}



public class RadioCell : SvRadioCell<string>
{
    public override ErrorOrResult<string> Save() => Value ?? EMPTY;
}
