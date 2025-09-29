// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:14

namespace Jakar.Extensions;


public interface IUnaryNegationOperators<TValue> : IUnaryNegationOperators<TValue, TValue>
    where TValue : IUnaryNegationOperators<TValue>;



public interface IUnaryPlusOperators<TValue> : IUnaryPlusOperators<TValue, TValue>
    where TValue : IUnaryPlusOperators<TValue>;



public interface IBitwiseOperators<TValue> : IBitwiseOperators<TValue, TValue, TValue>
    where TValue : IBitwiseOperators<TValue>;



public interface IShiftOperators<TValue> : IShiftOperators<TValue, TValue, TValue>
    where TValue : IShiftOperators<TValue>;



public interface IUnaryOperators<TValue> : IUnaryNegationOperators<TValue>, IUnaryPlusOperators<TValue>
    where TValue : IUnaryOperators<TValue>;



public interface IBinaryOperators<TValue> : IShiftOperators<TValue>, IBitwiseOperators<TValue>, IUnaryOperators<TValue>
    where TValue : IBinaryOperators<TValue>;
