// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  12:17

namespace Jakar.Extensions;


public interface IShapeOperators<TSelf> : IMathOperators<TSelf, double>, IMathOperators<TSelf, float>, IMathOperators<TSelf, int>, IMathOperators<TSelf, (double xOffset, double yOffset)>, IMathOperators<TSelf, (float xOffset, float yOffset)>, IMathOperators<TSelf, (int xOffset, int yOffset)>
    where TSelf : IShapeOperators<TSelf>;



public interface IGenericShape<TSelf> : IFormattable, IValidator, IEqualComparable<TSelf>, IShapeOperators<TSelf>
    where TSelf : IGenericShape<TSelf>
{
    public abstract static TSelf Invalid { get; }
    public abstract static TSelf Zero    { get; }
    public                 bool  IsNaN   { get; }
}



public interface IShapeSize<out TNumber>
    where TNumber : INumber<TNumber>
{
    public TNumber Width  { get; }
    public TNumber Height { get; }
}



public interface IShapeLocation<out TNumber>
    where TNumber : INumber<TNumber>
{
    public TNumber X { get; }
    public TNumber Y { get; }
}
