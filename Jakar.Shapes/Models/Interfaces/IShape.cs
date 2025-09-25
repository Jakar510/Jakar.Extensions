// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  12:17

namespace Jakar.Shapes.Interfaces;


public interface IShape<TSelf> : IFormattable, IValidator, IJsonModel<TSelf>
    where TSelf : IShape<TSelf>
{
    public abstract static ref readonly TSelf Invalid { get; }
    public abstract static ref readonly TSelf One     { get; }
    public abstract static ref readonly TSelf Zero    { get; }
    public                              bool  IsNaN   { get; }


    public abstract static TSelf operator +( TSelf self, int                              other );
    public abstract static TSelf operator +( TSelf self, float                            other );
    public abstract static TSelf operator +( TSelf self, double                           other );
    public abstract static TSelf operator -( TSelf self, int                              other );
    public abstract static TSelf operator -( TSelf self, float                            other );
    public abstract static TSelf operator -( TSelf self, double                           other );
    public abstract static TSelf operator *( TSelf self, int                              other );
    public abstract static TSelf operator *( TSelf self, float                            other );
    public abstract static TSelf operator *( TSelf self, double                           other );
    public abstract static TSelf operator /( TSelf self, int                              other );
    public abstract static TSelf operator /( TSelf self, float                            other );
    public abstract static TSelf operator /( TSelf self, double                           other );
    public abstract static TSelf operator +( TSelf self, (int xOffset, int yOffset)       other );
    public abstract static TSelf operator +( TSelf self, (float xOffset, float yOffset)   other );
    public abstract static TSelf operator +( TSelf self, (double xOffset, double yOffset) other );
    public abstract static TSelf operator -( TSelf self, (int xOffset, int yOffset)       other );
    public abstract static TSelf operator -( TSelf self, (float xOffset, float yOffset)   other );
    public abstract static TSelf operator -( TSelf self, (double xOffset, double yOffset) other );
    public abstract static TSelf operator *( TSelf self, (int xOffset, int yOffset)       other );
    public abstract static TSelf operator *( TSelf self, (float xOffset, float yOffset)   other );
    public abstract static TSelf operator *( TSelf self, (double xOffset, double yOffset) other );
    public abstract static TSelf operator /( TSelf self, (int xOffset, int yOffset)       other );
    public abstract static TSelf operator /( TSelf self, (float xOffset, float yOffset)   other );
    public abstract static TSelf operator /( TSelf self, (double xOffset, double yOffset) other );
}



public interface IShapeSize
{
    public double Height { get; }
    public double Width  { get; }


    public void Deconstruct( out double width, out double height );
}



public interface IShapeLocation
{
    public double X { get; }
    public double Y { get; }


    public void Deconstruct( out double x, out double y );
}
