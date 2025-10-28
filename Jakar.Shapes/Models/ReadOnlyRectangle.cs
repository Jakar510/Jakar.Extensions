// TrueLogic :: iTrueLogic.Shared
// 09/21/2023  5:37 PM

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes;


[StructLayout(LayoutKind.Sequential)]
[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlyRectangle( double x, double y, double width, double height ) : IRectangle<ReadOnlyRectangle>
{
    public static readonly ReadOnlyRectangle Invalid = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly ReadOnlyRectangle Zero    = new(0, 0, 0, 0);
    public static readonly ReadOnlyRectangle One     = 1;
    public readonly        double            X       = x;
    public readonly        double            Y       = y;
    public readonly        double            Width   = width;
    public readonly        double            Height  = height;


    public static       JsonSerializerContext                       JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlyRectangle>             JsonTypeInfo  => JakarShapesContext.Default.ReadOnlyRectangle;
    public static       JsonTypeInfo<ReadOnlyRectangle[]>           JsonArrayInfo => JakarShapesContext.Default.ReadOnlyRectangleArray;
    static ref readonly ReadOnlyRectangle IShape<ReadOnlyRectangle>.Zero          => ref Zero;
    static ref readonly ReadOnlyRectangle IShape<ReadOnlyRectangle>.Invalid       => ref Invalid;
    static ref readonly ReadOnlyRectangle IShape<ReadOnlyRectangle>.One           => ref One;
    bool IValidator.                                                IsValid       => this.IsValid();
    double IShapeLocation.                                          X             => X;
    double IShapeLocation.                                          Y             => Y;
    double IShapeSize.                                              Width         => Width;
    double IShapeSize.                                              Height        => Height;
    public ReadOnlyPoint                                            Location      => new(X, Y);
    public ReadOnlySize                                             Size          => new(Width, Height);


    public static implicit operator Rectangle( ReadOnlyRectangle          self )  => new((int)self.X.Round(), (int)self.Y.Round(), (int)self.Width.Round(), (int)self.Height.Round());
    public static implicit operator RectangleF( ReadOnlyRectangle         self )  => new(self.X.AsFloat(), self.Y.AsFloat(), self.Width.AsFloat(), self.Height.AsFloat());
    public static implicit operator ReadOnlyRectangleF( ReadOnlyRectangle self )  => new(self.X.AsFloat(), self.Y.AsFloat(), self.Width.AsFloat(), self.Height.AsFloat());
    public static implicit operator ReadOnlyRectangle( int                value ) => Create<ReadOnlySize>(0, 0, value);
    public static implicit operator ReadOnlyRectangle( long               value ) => Create<ReadOnlySize>(0, 0, value);
    public static implicit operator ReadOnlyRectangle( float              value ) => Create<ReadOnlySize>(0, 0, value);
    public static implicit operator ReadOnlyRectangle( double             value ) => Create<ReadOnlySize>(0, 0, value.AsFloat());


    public static bool TryFromJson( string? json, out ReadOnlyRectangle result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = Invalid;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = Invalid;
        return false;
    }
    public static ReadOnlyRectangle FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    [Pure] public static ReadOnlyRectangle Create<TPoint>( params ReadOnlySpan<TPoint> points )
        where TPoint : struct, IPoint<TPoint>
    {
        if ( points.Length <= 0 ) { return Zero; }

        ValueEnumerable<FromSpan<TPoint>, TPoint> enumerable = points.AsValueEnumerable();

        double x      = enumerable.Min(static p => p.X);
        double y      = enumerable.Min(static p => p.Y);
        double width  = enumerable.Max(static p => p.X) - x;
        double height = enumerable.Max(static p => p.Y) - y;

        return Create(x, y, width, height);
    }
    [Pure] public static ReadOnlyRectangle Create<TRectangle>( in TRectangle rect )
        where TRectangle : struct, IRectangle<TRectangle> => Create(rect.X, rect.Y, rect.Width, rect.Height);
    [Pure] public static ReadOnlyRectangle Create( float  x, float  y, float  width, float  height ) => new(x, y, width, height);
    [Pure] public static ReadOnlyRectangle Create( double x, double y, double width, double height ) => new(x, y, width, height);
    [Pure] public static ReadOnlyRectangle Create<TPoint>( in TPoint topLeft, in TPoint bottomRight )
        where TPoint : struct, IPoint<TPoint> => Create(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    [Pure] public static ReadOnlyRectangle Create<TPoint, TSize>( in TPoint point, in TSize size )
        where TPoint : struct, IPoint<TPoint>
        where TSize : struct, ISize<TSize> => Create(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create<TSize>( double x, double y, in TSize size )
        where TSize : struct, ISize<TSize> => Create(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create<TRectangle>( in TRectangle rectangle, in ReadOnlyThickness padding )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        double x      = rectangle.X      + padding.Left;
        double y      = rectangle.Y      + padding.Top;
        double width  = rectangle.Width  - padding.HorizontalThickness;
        double height = rectangle.Height - padding.VerticalThickness;
        return Create(x, y, width, height);
    }


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is ReadOnlyRectangle x
                   ? CompareTo(x)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(ReadOnlyRectangle));
    }
    public int CompareTo( ReadOnlyRectangle other )
    {
        int xComparison = X.CompareTo(other.X);
        if ( xComparison != 0 ) { return xComparison; }

        int yComparison = Y.CompareTo(other.Y);
        if ( yComparison != 0 ) { return yComparison; }

        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public          bool   Equals( ReadOnlyRectangle other )                           => X.Equals(other.X)            && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool   Equals( object?           other )                           => other is ReadOnlyRectangle x && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y, Width, Height);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<ReadOnlyRectangle>.ToString(in this, format);


    public static bool operator ==( ReadOnlyRectangle?            left, ReadOnlyRectangle?               right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangle?            left, ReadOnlyRectangle?               right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyRectangle             left, ReadOnlyRectangle                right ) => EqualityComparer<ReadOnlyRectangle>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangle             left, ReadOnlyRectangle                right ) => !EqualityComparer<ReadOnlyRectangle>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyRectangle              left, ReadOnlyRectangle                right ) => Comparer<ReadOnlyRectangle>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyRectangle             left, ReadOnlyRectangle                right ) => Comparer<ReadOnlyRectangle>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyRectangle              left, ReadOnlyRectangle                right ) => Comparer<ReadOnlyRectangle>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyRectangle             left, ReadOnlyRectangle                right ) => Comparer<ReadOnlyRectangle>.Default.Compare(left, right) <= 0;
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, ReadOnlyRectangle                value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, ReadOnlyRectangle                value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, ReadOnlyRectangle                value ) => self.Divide(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, ReadOnlyRectangle                value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, ReadOnlyRectangleF               value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, ReadOnlyRectangleF               value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, ReadOnlyRectangleF               value ) => self.Divide(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, ReadOnlyRectangleF               value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, double                           value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, double                           value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, double                           value ) => self.Divide(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, double                           value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, float                            value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, float                            value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, float                            value ) => self.Divide(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, float                            value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator +( ReadOnlyRectangle self, int                              value ) => self.Add(value);
    public static ReadOnlyRectangle operator -( ReadOnlyRectangle self, int                              value ) => self.Subtract(value);
    public static ReadOnlyRectangle operator *( ReadOnlyRectangle self, int                              value ) => self.Multiply(value);
    public static ReadOnlyRectangle operator /( ReadOnlyRectangle self, int                              value ) => self.Divide(value);
}
