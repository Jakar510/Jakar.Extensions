namespace Jakar.SettingsView.Maui.Cells;


public abstract class PickerCell<T> : ValueCellBase<T>
    where T : IEquatable<T>, IComparable<T>
{
    public override WidgetType Type   => WidgetType.Picker;
    public          IList<T>?  Values { get; set; }


    public override ErrorOrResult<T> Save()
    {
        return Value is not null
                   ? Value
                   : Error.Create( Status.NoContent, default );
    }
}
