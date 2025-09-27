// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:36


namespace Jakar.Shapes.Interfaces;


public interface IRectangle<TSelf> : IShape<TSelf>, IShapeSize, IShapeLocation
    where TSelf : IRectangle<TSelf>, IJsonModel<TSelf>
{
    public ReadOnlyLine  Bottom      { get; }
    public ReadOnlyPoint Center      { get; }
    public ReadOnlyLine  Left        { get; }
    public ReadOnlyLine  Right       { get; }
    public ReadOnlyLine  Top         { get; }
    public ReadOnlyPoint TopLeft     { get; }
    public ReadOnlyPoint TopRight    { get; }
    public ReadOnlyPoint BottomLeft  { get; }
    public ReadOnlyPoint BottomRight { get; }


    public abstract static implicit operator Rectangle( TSelf  self );
    public abstract static implicit operator RectangleF( TSelf self );
    public abstract static implicit operator TSelf( int        value );
    public abstract static implicit operator TSelf( long       value );
    public abstract static implicit operator TSelf( float      value );
    public abstract static implicit operator TSelf( double     value );


    public static bool CheckIfNaN( in   TSelf self ) => double.IsNaN(self.X) || double.IsNaN(self.Y) || double.IsNaN(self.Width) || double.IsNaN(self.Height);
    public static bool CheckIfEmpty( in TSelf self ) => double.IsNaN(self.X) || double.IsNaN(self.Y) || double.IsNaN(self.Width) || self.Width <= 0 || double.IsNaN(self.Height) || self.Height <= 0;


    [Pure] public abstract static TSelf Create( float  x, float  y, float  width, float  height );
    [Pure] public abstract static TSelf Create( double x, double y, double width, double height );
    [Pure] public abstract static TSelf Create<TPoint>( params ReadOnlySpan<TPoint> points )
        where TPoint : IPoint<TPoint>;
    [Pure] public abstract static TSelf Create<TPoint>( in TPoint topLeft, in TPoint bottomRight )
        where TPoint : IPoint<TPoint>;
    [Pure] public abstract static TSelf Create<TPoint, TSize>( in TPoint point, in TSize size )
        where TPoint : IPoint<TPoint>
        where TSize : ISize<TSize>;
    [Pure] public abstract static TSelf Create<TRectangle>( in TRectangle rectangle )
        where TRectangle : IRectangle<TRectangle>;
    [Pure] public abstract static TSelf Create<TRectangle>( in TRectangle rectangle, in ReadOnlyThickness padding )
        where TRectangle : IRectangle<TRectangle>;
    [Pure] public abstract static TSelf Create<TSize>( double x, double y, in TSize size )
        where TSize : ISize<TSize>;


    public void Deconstruct( out float          x,     out float         y, out float  width, out float  height );
    public void Deconstruct( out double         x,     out double        y, out double width, out double height );
    public void Deconstruct( out ReadOnlyPoint  point, out ReadOnlySize  size );
    public void Deconstruct( out ReadOnlyPointF point, out ReadOnlySizeF size );


    public static string ToString( ref readonly TSelf self, string? format )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson(TSelf.JsonTypeInfo);

            case ",":
                return $"{self.X},{self.Y},{self.Width},{self.Height}";

            case "-":
                return $"{self.X}-{self.Y}-{self.Width}-{self.Height}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSelf).Name}<{nameof(self.X)}: {self.X}, {nameof(self.Y)}: {self.Y}, {nameof(self.Width)}: {self.Width}, {nameof(self.Height)}: {self.Height}>";
        }
    }
}
