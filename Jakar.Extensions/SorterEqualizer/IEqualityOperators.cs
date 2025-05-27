// Jakar.Extensions :: Jakar.Extensions
// 05/27/2025  11:34

namespace Jakar.Extensions;


public interface IEqualityOperators<TValue> : IEqualityOperators<TValue, TValue, bool>, IEquatable<TValue>
    where TValue : IEqualityOperators<TValue>;



public interface IComparisonOperators<TValue> : IEqualityOperators<TValue>, IComparisonOperators<TValue, TValue, bool>, IComparable<TValue>, IComparable
    where TValue : IComparisonOperators<TValue>;
