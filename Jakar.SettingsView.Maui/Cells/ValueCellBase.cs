// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  20:08

namespace Jakar.SettingsView.Maui.Cells;


public abstract class ValueCellBase<T> : DescriptionCellBase, ISvCellValue<T>
    where T : IEquatable<T>, IComparable<T>
{
    public static   IEqualityComparer<T> Equalizer { get; set; } = EqualityComparer<T>.Default;
    public static   IComparer<T>         Sorter    { get; set; } = Comparer<T>.Default;
    public          string?              Hint      { get; set; }
    public virtual  bool                 IsValid   => Value is not null && Equalizer.Equals( Value, default ) is false;
    public          T?                   Max       { get; set; }
    public          T?                   Min       { get; set; }
    public          T?                   Value     { get; set; }
    public abstract ErrorOrResult<T>     Save();
}
