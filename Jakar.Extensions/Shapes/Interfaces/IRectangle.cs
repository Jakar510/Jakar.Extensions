// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:36


namespace Jakar.Extensions;


public interface IRectangle<TSelf, TSize, TPoint, TThickness, TNumber> : IGenericShape<TSelf>, IShapeSize<TNumber>, IShapeLocation<TNumber>
    where TSelf : IRectangle<TSelf, TSize, TPoint, TThickness, TNumber>
    where TSize : ISize<TSize, TNumber>
    where TPoint : IPoint<TPoint, TNumber>
    where TThickness : IThickness<TThickness, TNumber>
    where TNumber : INumber<TNumber>
{
    public TNumber Bottom   { get; }
    public TNumber Left     { get; }
    public TNumber Right    { get; }
    public TNumber Top      { get; }
    public TPoint  Center   { get; }
    public TPoint  Location { get; }
    public TSize   Size     { get; }


    public static bool CheckIfEmpty( in TSelf rectangle ) => TNumber.IsNaN( rectangle.X ) || TNumber.IsNaN( rectangle.Y ) || TNumber.IsNegative( rectangle.Width ) || TNumber.IsNaN( rectangle.Height ) || TNumber.IsNaN( rectangle.Width ) || TNumber.IsNegative( rectangle.Height );


    [Pure] public abstract static TSelf Create( params ReadOnlySpan<TPoint> points );
    [Pure] public abstract static TSelf Create( in     TPoint               point,     in TSize      size );
    [Pure] public abstract static TSelf Create( in     TPoint               topLeft,   in TPoint     bottomRight );
    [Pure] public abstract static TSelf Create( in     TSelf                rectangle, in TThickness padding );
    [Pure] public abstract static TSelf Create( TNumber                     x,         TNumber       y, in TSize size );
    [Pure] public abstract static TSelf Create( TNumber                     x,         TNumber       y, TNumber  width, TNumber height );


    [Pure] public bool  IsAtLeast( in      TSize  other );
    [Pure] public bool  Contains( in       TPoint other );
    [Pure] public bool  Contains( in       TSelf  other );
    [Pure] public bool  IntersectsWith( in TSelf  other );
    [Pure] public TSelf Union( in          TSelf  other );
    [Pure] public TSelf Round();


    [Pure] public TSelf Intersection( in TSelf other );


    [Pure] public bool Contains( params ReadOnlySpan<TPoint> points );

    [Pure] public bool DoesLineIntersect( in TPoint source, in TPoint target );


    public void Deconstruct( out TNumber x,     out TNumber y, out TNumber width, out TNumber height );
    public void Deconstruct( out TPoint  point, out TSize   size );


    public static string ToString( TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",": return $"{self.X},{self.Y},{self.Width},{self.Height}";
            case "-": return $"{self.X}-{self.Y}-{self.Width}-{self.Height}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(self.X)}: {self.X}, {nameof(self.Y)}: {self.Y}, {nameof(self.Width)}: {self.Width}, {nameof(self.Height)}: {self.Height}>";
        }
    }
}
