// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  16:26

namespace Jakar.Extensions;


public interface IPoint<TSelf> : INegativeShapes<TSelf>, IShapeLocation
    where TSelf : IPoint<TSelf>
{
    [Pure] public abstract static TSelf Create( double x, double y );

    [Pure] public TSelf  Reverse();
    [Pure] public TSelf  Round();
    [Pure] public TSelf  Floor();
    [Pure] public double DistanceTo( in TSelf other );




    public static string ToString( ref readonly TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",":
                return $"{self.X},{self.Y}";

            case "-":
                return $"{self.X}-{self.Y}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(self.X)}: {self.X}, {nameof(self.Y)}: {self.Y}>";
        }
    }
}
