// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:13

namespace Jakar.Extensions;


public interface IOperators<TValue> : IBinaryOperators<TValue>, IMathOperators<TValue>
    where TValue : IOperators<TValue>;
