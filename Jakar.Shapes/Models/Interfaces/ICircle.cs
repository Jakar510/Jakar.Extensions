// Jakar.Extensions :: Jakar.Extensions
// 07/12/2025  14:39

namespace Jakar.Shapes.Interfaces;


public interface ICircle<TSelf> : IShape<TSelf>, IShapeLocation
    where TSelf : struct, ICircle<TSelf>, IJsonModel<TSelf>
{
    ReadOnlyPoint Center { get; }
    double        Radius { get; }


    [Pure] public abstract static TSelf Create( in ReadOnlyPoint center, double radius );
    [Pure] public                 TSelf Round();
    [Pure] public                 TSelf Floor();


    public void Deconstruct( out double        x,     out double y, out double radius );
    public void Deconstruct( out ReadOnlyPoint start, out double radius );


    public static string ToString( ref readonly TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson(TSelf.JsonTypeInfo);

            case ",":
                return $"{self.X},{self.Y}";

            case "-":
                return $"{self.X}-{self.Y}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(Center)}: {self.Center}, {nameof(Radius)}: {self.Radius}>";
        }
    }
}
