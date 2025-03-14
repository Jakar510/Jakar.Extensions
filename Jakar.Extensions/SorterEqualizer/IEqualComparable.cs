// Jakar.Extensions :: Jakar.Extensions
// 03/14/2025  11:03

namespace Jakar.Extensions;


public interface IEqualComparable<T> : IEquatable<T>, IComparable<T>
    where T : class, IEqualComparable<T>
{
    public abstract static Equalizer<T> Equalizer { get; }
    public abstract static Sorter<T>    Sorter    { get; }
}



public interface IValueEqualComparable<T> : IEquatable<T>, IComparable<T>
    where T : struct, IValueEqualComparable<T>
{
    public abstract static ValueEqualizer<T> Equalizer { get; }
    public abstract static ValueSorter<T>    Sorter    { get; }
}
