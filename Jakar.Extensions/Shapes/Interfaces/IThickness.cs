// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  16:36

namespace Jakar.Extensions;


public interface IThickness<TSelf> : IGenericShape<TSelf>
    where TSelf : IThickness<TSelf>
{
    public double Bottom              { get; }
    public double Left                { get; }
    public double Right               { get; }
    public double Top                 { get; }
    public double HorizontalThickness { get; }
    public double VerticalThickness   { get; }


    void Deconstruct( out double left,             out double top, out double right, out double bottom );
    void Deconstruct( out double horizontalMargin, out double verticalMargin );


    public static string ToString( TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",": return $"{self.Left},{self.Top},{self.Right},{self.Bottom}";
            case "-": return $"{self.Left}-{self.Top}-{self.Right}-{self.Bottom}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(self.Left)}: {self.Left}, {nameof(self.Top)}: {self.Top}, {nameof(self.Right)}: {self.Right}, {nameof(self.Bottom)}: {self.Bottom}>";
        }
    }
}
