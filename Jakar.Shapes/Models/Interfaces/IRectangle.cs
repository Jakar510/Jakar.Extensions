// Jakar.Extensions :: Jakar.Extensions
// 06/26/2025  11:36


using Jakar.Extensions;



namespace Jakar.Shapes.Interfaces;


public interface IRectangle<TSelf> : IShape<TSelf>, IShapeSize, IShapeLocation
    where TSelf : IRectangle<TSelf>, IJsonModel<TSelf>
{
    public double        Bottom   { get; }
    public double        Left     { get; }
    public double        Right    { get; }
    public double        Top      { get; }
    public ReadOnlyPoint Center   { get; }
    public ReadOnlyPoint Location { get; }
    public ReadOnlySize  Size     { get; }


    public abstract static implicit operator Rectangle( TSelf  self );
    public abstract static implicit operator RectangleF( TSelf self );
    public abstract static implicit operator TSelf( int        value );
    public abstract static implicit operator TSelf( long       value );
    public abstract static implicit operator TSelf( float      value );
    public abstract static implicit operator TSelf( double     value );


    public static bool CheckIfEmpty( in TSelf self ) => double.IsNaN(self.Bottom) || double.IsNaN(self.Top) || double.IsNegative(self.Left) || double.IsNaN(self.Left) || double.IsNaN(self.Right) || double.IsNegative(self.Right);


    [Pure] public abstract static TSelf Create( params ReadOnlySpan<ReadOnlyPoint> points );
    [Pure] public abstract static TSelf Create( in     ReadOnlyPoint               point,     in ReadOnlySize      size );
    [Pure] public abstract static TSelf Create( in     ReadOnlyPoint               topLeft,   in ReadOnlyPoint     bottomRight );
    [Pure] public abstract static TSelf Create( in     TSelf                       rectangle, in ReadOnlyThickness padding );
    [Pure] public abstract static TSelf Create( double                             x,         double               y, in ReadOnlySize size );
    [Pure] public abstract static TSelf Create( double                             x,         double               y, double          width, double height );
    [Pure]
    public abstract static TSelf Create<T>( ref readonly T rect )
        where T : IRectangle<T>;


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
