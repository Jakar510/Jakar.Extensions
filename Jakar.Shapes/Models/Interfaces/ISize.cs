// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  14:02

namespace Jakar.Shapes.Interfaces;


public interface ISize<TSelf> : IShape<TSelf>, IShapeSize
    where TSelf : ISize<TSelf>
{
    public bool IsLandscape { get; }
    public bool IsPortrait  { get; }


    public static bool CheckIfEmpty( ref readonly     TSelf self ) => self.IsNaN               || self.Width <= 0 || self.Height <= 0;
    public static bool CheckIfInvalid( ref readonly   TSelf self ) => double.IsNaN(self.Width) || double.IsNaN(self.Height);
    public static bool CheckIfPortrait( ref readonly  TSelf self ) => self.Width > self.Height;
    public static bool CheckIfLandscape( ref readonly TSelf self ) => self.Width > self.Height;


    [Pure] public abstract static TSelf Create( double width, double height );

    [Pure] public TSelf Reverse();
    [Pure] public TSelf Round();
    [Pure] public TSelf Floor();


    public static string ToString( ref readonly TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",":
                return $"{self.Width},{self.Height}";

            case "-":
                return $"{self.Width}-{self.Height}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(self.Width)}: {self.Width}, {nameof(self.Height)}: {self.Height}>";
        }
    }
}
