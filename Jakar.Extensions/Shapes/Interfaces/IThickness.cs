// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  16:36

namespace Jakar.Extensions;


public interface IThickness<TSelf, TNumber> : IGenericShape<TSelf>
    where TSelf : IThickness<TSelf, TNumber>
    where TNumber : INumber<TNumber>
{
    public double Bottom              { get; }
    public double Left                { get; }
    public double Right               { get; }
    public double Top                 { get; }
    public double HorizontalThickness { get; }
    public double VerticalThickness   { get; }


    void Deconstruct( out TNumber left,             out TNumber top, out TNumber right, out TNumber bottom );
    void Deconstruct( out TNumber horizontalMargin, out TNumber verticalMargin );


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
