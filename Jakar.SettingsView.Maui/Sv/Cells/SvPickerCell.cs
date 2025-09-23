using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


public abstract class SvPickerCell<TValue> : ValueCellBase<TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public override WidgetType Type   => WidgetType.Picker;
    public          IList<TValue>?  Values { get; set; }


    public override ErrorOrResult<TValue> Save()
    {
        return Value is not null
                   ? Value
                   : Error.Create( Status.NoContent );
    }
}
