// Jakar.Extensions :: Jakar.Extensions
// 06/27/2025  15:00

namespace Jakar.Extensions;


public interface IIncrementDecrementOperators<TValue> : IDecrementOperators<TValue>, IIncrementOperators<TValue>
    where TValue : IIncrementDecrementOperators<TValue>;
