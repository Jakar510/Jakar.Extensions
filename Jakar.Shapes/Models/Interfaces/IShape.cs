// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  12:17

namespace Jakar.Shapes.Interfaces;


public interface IShapeOperators<TSelf> : IMathOperators<TSelf, double>, IMathOperators<TSelf, float>, IMathOperators<TSelf, int>, IMathOperators<TSelf, (double xOffset, double yOffset)>, IMathOperators<TSelf, (float xOffset, float yOffset)>, IMathOperators<TSelf, (int xOffset, int yOffset)>
    where TSelf : IShapeOperators<TSelf>;



public interface IShape<TSelf> : IFormattable, IValidator, IEqualComparable<TSelf>
    where TSelf : IShape<TSelf>
{
    public abstract static ref readonly TSelf Invalid { get; }
    public abstract static ref readonly TSelf Zero    { get; }
    public abstract static ref readonly TSelf One     { get; }
    public                              bool  IsNaN   { get; }
}



public interface IShapeSize
{
    public double Width  { get; }
    public double Height { get; }
}



public interface IShapeLocation
{
    public double X { get; }
    public double Y { get; }
}
