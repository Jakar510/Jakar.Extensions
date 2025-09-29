// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  12:17

namespace Jakar.Shapes.Interfaces;


public interface IShape<TSelf> : IValidator, IFormattable, IJsonModel<TSelf>
    where TSelf : IShape<TSelf>
{
    public abstract static ref readonly TSelf Invalid { get; }
    public abstract static ref readonly TSelf One     { get; }
    public abstract static ref readonly TSelf Zero    { get; }


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
    public double       Height { get; }
    public double       Width  { get; }
    public ReadOnlySize Size   { get; }
}



public interface IShapeLocation
{
    public double        X        { get; }
    public double        Y        { get; }
    public ReadOnlyPoint Location { get; }
}
