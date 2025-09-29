// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:36


namespace Jakar.Shapes.Interfaces;


public interface IRectangle<TSelf> : IShape<TSelf>, IShapeSize, IShapeLocation
    where TSelf : struct, IRectangle<TSelf>, IJsonModel<TSelf>
{
    public abstract static implicit operator Rectangle( TSelf  self );
    public abstract static implicit operator RectangleF( TSelf self );
    public abstract static implicit operator TSelf( int        value );
    public abstract static implicit operator TSelf( long       value );
    public abstract static implicit operator TSelf( float      value );
    public abstract static implicit operator TSelf( double     value );


    [Pure] public abstract static TSelf Create( float  x, float  y, float  width, float  height );
    [Pure] public abstract static TSelf Create( double x, double y, double width, double height );
    [Pure] public abstract static TSelf Create<TPoint>( params ReadOnlySpan<TPoint> points )
        where TPoint : struct, IPoint<TPoint>;
    [Pure] public abstract static TSelf Create<TPoint>( in TPoint topLeft, in TPoint bottomRight )
        where TPoint : struct, IPoint<TPoint>;
    [Pure] public abstract static TSelf Create<TPoint, TSize>( in TPoint point, in TSize size )
        where TPoint : struct, IPoint<TPoint>
        where TSize : struct, ISize<TSize>;
    [Pure] public abstract static TSelf Create<TRectangle>( in TRectangle rectangle )
        where TRectangle : struct, IRectangle<TRectangle>;
    [Pure] public abstract static TSelf Create<TRectangle>( in TRectangle rectangle, in ReadOnlyThickness padding )
        where TRectangle : struct, IRectangle<TRectangle>;
    [Pure] public abstract static TSelf Create<TSize>( double x, double y, in TSize size )
        where TSize : struct, ISize<TSize>;


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
