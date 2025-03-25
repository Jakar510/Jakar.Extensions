// Jakar.Extensions :: Jakar.Extensions
// 03/14/2025  11:03

namespace Jakar.Extensions;


public interface IEqualComparable<TValue> : IEquatable<TValue>, IComparable<TValue>
    where TValue : class, IEqualComparable<TValue>
{
    public abstract static Equalizer<TValue> Equalizer { get; }
    public abstract static Sorter<TValue>    Sorter    { get; }
}



public interface IValueEqualComparable<TValue> : IEquatable<TValue>, IComparable<TValue>
    where TValue : struct, IValueEqualComparable<TValue>
{
    public abstract static ValueEqualizer<TValue> Equalizer { get; }
    public abstract static ValueSorter<TValue>    Sorter    { get; }
}
