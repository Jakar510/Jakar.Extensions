// Jakar.Extensions :: Jakar.Extensions
// 05/27/2025  11:34

namespace Jakar.Extensions;


public interface IEqualityOperators<TValue> : IEqualityOperators<TValue, TValue, bool>, IEquatable<TValue>
    where TValue : IEqualityOperators<TValue>;



public interface IComparisonOperators<TValue> : IComparisonOperators<TValue, TValue, bool>, IComparable<TValue>, IComparable
    where TValue : IComparisonOperators<TValue>;



public interface IEqualComparable<TValue> : IEqualityOperators<TValue>, IComparisonOperators<TValue>
    where TValue : class, IEqualComparable<TValue>
{
    public abstract static Equalizer<TValue> Equalizer { get; }
    public abstract static Sorter<TValue>    Sorter    { get; }
}



public interface IValueEqualComparable<TValue> : IEqualityOperators<TValue>, IComparisonOperators<TValue>, IComparable<TValue?>, IEquatable<TValue?>
    where TValue : struct, IValueEqualComparable<TValue>
{
    public abstract static ValueEqualizer<TValue> Equalizer { get; }
    public abstract static ValueSorter<TValue>    Sorter    { get; }
}
