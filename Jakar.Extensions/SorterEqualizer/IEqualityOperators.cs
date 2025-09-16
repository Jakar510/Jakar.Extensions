// Jakar.Extensions :: Jakar.Extensions
// 05/27/2025  11:34

namespace Jakar.Extensions;


public interface IEqualityOperators<TValue> : IEqualityOperators<TValue, TValue, bool>, IEquatable<TValue>
    where TValue : IEqualityOperators<TValue>;



public interface IComparisonOperators<TValue> : IComparisonOperators<TValue, TValue, bool>, IComparable<TValue>, IComparable
    where TValue : IComparisonOperators<TValue>;



public interface IEqualComparable<TValue> : IEqualityOperators<TValue>, IComparisonOperators<TValue>
    where TValue : IEqualComparable<TValue>
{
    public abstract static EqualComparer<TValue> Sorter { get; }


    public bool Equals( object? other );
    public int  GetHashCode();
}
