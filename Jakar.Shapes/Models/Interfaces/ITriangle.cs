// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  10:32

namespace Jakar.Shapes.Interfaces;


public interface ITriangle<TSelf> : IShape<TSelf>, IShapeLocation
    where TSelf : struct, ITriangle<TSelf>
{
    ReadOnlyPoint        A        { get; }
    public ReadOnlyLine  Ab       { get; }
    public Degrees       Abc      { get; }
    public double        Area     { get; }
    ReadOnlyPoint        B        { get; }
    public Degrees       Bac      { get; }
    public ReadOnlyLine  Bc       { get; }
    ReadOnlyPoint        C        { get; }
    public ReadOnlyLine  Ca       { get; }
    public Degrees       Cab      { get; }
    public ReadOnlyPoint Centroid { get; }


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
                return $"{typeof(TSelf).Name}<{nameof(A)}: {self.A}, {nameof(B)}: {self.B}, {nameof(C)}: {self.C}>";
        }
    }
}
