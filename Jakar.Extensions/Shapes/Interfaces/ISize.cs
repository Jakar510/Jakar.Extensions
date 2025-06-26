// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  14:02

namespace Jakar.Extensions;


public interface ISize<TSelf, TNumber> : IGenericShape<TSelf>, IShapeSize<TNumber>
    where TSelf : ISize<TSelf, TNumber>
    where TNumber : INumber<TNumber>
{
    public bool IsLandscape { get; }
    public bool IsPortrait  { get; }


    [Pure] public abstract static TSelf Create( TNumber width, TNumber height );

    [Pure] public TSelf Reverse();
    [Pure] public TSelf Round();
    [Pure] public TSelf Floor();


    public static string ToString( TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",": return $"{self.Width},{self.Height}";
            case "-": return $"{self.Width}-{self.Height}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(self.Width)}: {self.Width}, {nameof(self.Height)}: {self.Height}>";
        }
    }
}
