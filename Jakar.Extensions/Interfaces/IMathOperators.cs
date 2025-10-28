// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:13

namespace Jakar.Extensions;


public interface ISubtractionOperators<TValue, TOffset> : ISubtractionOperators<TValue, TOffset, TValue>
    where TValue : ISubtractionOperators<TValue, TOffset>;



public interface IMultiplyOperators<TValue, TOffset> : IMultiplyOperators<TValue, TOffset, TValue>
    where TValue : IMultiplyOperators<TValue, TOffset>;



public interface IDivisionOperators<TValue, TOffset> : IDivisionOperators<TValue, TOffset, TValue>
    where TValue : IDivisionOperators<TValue, TOffset>;



public interface IAdditionOperators<TValue, TOffset> : IAdditionOperators<TValue, TOffset, TValue>
    where TValue : IAdditionOperators<TValue, TOffset>;



public interface IMathOperators<TValue, TOffset> : IAdditionOperators<TValue, TOffset>, ISubtractionOperators<TValue, TOffset>, IDivisionOperators<TValue, TOffset>, IMultiplyOperators<TValue, TOffset>
    where TValue : IMathOperators<TValue, TOffset>;



public interface ISubtractionOperators<TValue> : ISubtractionOperators<TValue, TValue>
    where TValue : ISubtractionOperators<TValue>;



public interface IMultiplyOperators<TValue> : IMultiplyOperators<TValue, TValue>
    where TValue : IMultiplyOperators<TValue>;



public interface IDivisionOperators<TValue> : IDivisionOperators<TValue, TValue>
    where TValue : IDivisionOperators<TValue>;



public interface IAdditionOperators<TValue> : IAdditionOperators<TValue, TValue>
    where TValue : IAdditionOperators<TValue>;



public interface IMathOperators<TValue> : IMathOperators<TValue, TValue>
    where TValue : IMathOperators<TValue>;
