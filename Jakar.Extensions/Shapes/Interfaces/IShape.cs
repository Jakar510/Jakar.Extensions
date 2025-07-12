// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  12:17

namespace Jakar.Extensions;


public interface IShapeOperators<TSelf> : IMathOperators<TSelf, double>, IMathOperators<TSelf, float>, IMathOperators<TSelf, int>, IMathOperators<TSelf, (double xOffset, double yOffset)>, IMathOperators<TSelf, (float xOffset, float yOffset)>, IMathOperators<TSelf, (int xOffset, int yOffset)>
    where TSelf : IShapeOperators<TSelf>;



public interface IShape<TSelf> : IFormattable, IValidator, IEqualComparable<TSelf>
    where TSelf : IShape<TSelf>
{
    public abstract static ref readonly TSelf Invalid { get; }
    public abstract static ref readonly TSelf Zero    { get; }
    public abstract static ref readonly TSelf One     { get; }
    public abstract static ref readonly TSelf Two     { get; }
    public abstract static ref readonly TSelf Three   { get; }
    public abstract static ref readonly TSelf Four    { get; }
    public abstract static ref readonly TSelf Five    { get; }
    public abstract static ref readonly TSelf Six     { get; }
    public abstract static ref readonly TSelf Seven   { get; }
    public abstract static ref readonly TSelf Eight   { get; }
    public abstract static ref readonly TSelf Nine    { get; }
    public abstract static ref readonly TSelf Ten     { get; }
    public                              bool  IsNaN   { get; }
}



public interface INegativeShapes<TSelf> : IShape<TSelf>
    where TSelf : INegativeShapes<TSelf>
{
    public abstract static ref readonly TSelf NegativeOne   { get; }
    public abstract static ref readonly TSelf NegativeTwo   { get; }
    public abstract static ref readonly TSelf NegativeThree { get; }
    public abstract static ref readonly TSelf NegativeFour  { get; }
    public abstract static ref readonly TSelf NegativeFive  { get; }
    public abstract static ref readonly TSelf NegativeSix   { get; }
    public abstract static ref readonly TSelf NegativeSeven { get; }
    public abstract static ref readonly TSelf NegativeEight { get; }
    public abstract static ref readonly TSelf NegativeNine  { get; }
    public abstract static ref readonly TSelf NegativeTen   { get; }
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
