// TrueLogic :: TrueLogic.Common.Maui
// 01/20/2025  08:01

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes;


[StructLayout(LayoutKind.Sequential)]
[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlyRectangleF( float x, float y, float width, float height ) : IRectangle<ReadOnlyRectangleF>
{
    public static readonly ReadOnlyRectangleF Invalid = new(float.NaN, float.NaN, float.NaN, float.NaN);
    public static readonly ReadOnlyRectangleF Zero    = new(0, 0, 0, 0);
    public static readonly ReadOnlyRectangleF One     = 1;
    public readonly        float              X       = x;
    public readonly        float              Y       = y;
    public readonly        float              Width   = width;
    public readonly        float              Height  = height;


    public static       JsonSerializerContext                         JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlyRectangleF>              JsonTypeInfo  => JakarShapesContext.Default.ReadOnlyRectangleF;
    public static       JsonTypeInfo<ReadOnlyRectangleF[]>            JsonArrayInfo => JakarShapesContext.Default.ReadOnlyRectangleFArray;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.Zero          => ref Zero;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.Invalid       => ref Invalid;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.One           => ref One;
    bool IValidator.                                                  IsValid       => this.IsValid();
    double IShapeLocation.                                            X             => X;
    double IShapeLocation.                                            Y             => Y;
    double IShapeSize.                                                Width         => Width;
    double IShapeSize.                                                Height        => Height;
    public ReadOnlyPoint                                              Location      => new(X, Y);
    public ReadOnlySize                                               Size          => new(Width, Height);


    public static implicit operator Rectangle( ReadOnlyRectangleF  self )  => new((int)self.X.Round(), (int)self.Y.Round(), (int)self.Width.Round(), (int)self.Height.Round());
    public static implicit operator RectangleF( ReadOnlyRectangleF self )  => new(self.X, self.Y, self.Width, self.Height);
    public static implicit operator ReadOnlyRectangleF( Rectangle  self )  => new(self.X, self.Y, self.Width, self.Height);
    public static implicit operator ReadOnlyRectangleF( RectangleF self )  => new(self.X, self.Y, self.Width, self.Height);
    public static implicit operator ReadOnlyRectangleF( int        value ) => Create<ReadOnlySize>(0, 0, value);
    public static implicit operator ReadOnlyRectangleF( long       value ) => Create<ReadOnlySize>(0, 0, value);
    public static implicit operator ReadOnlyRectangleF( float      value ) => Create<ReadOnlySize>(0, 0, value);
    public static implicit operator ReadOnlyRectangleF( double     value ) => Create<ReadOnlySize>(0, 0, value.AsFloat());


    public static bool TryFromJson( string? json, out ReadOnlyRectangleF result )
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
    public static ReadOnlyRectangleF FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    [Pure] public static ReadOnlyRectangleF Create<TPoint>( params ReadOnlySpan<TPoint> points )
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
    [Pure] public static ReadOnlyRectangleF Create<TRectangle>( in TRectangle rectangle )
        where TRectangle : struct, IRectangle<TRectangle> => Create(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    [Pure] public static ReadOnlyRectangleF Create( float  x, float  y, float  width, float  height ) => new(x, y, width, height);
    [Pure] public static ReadOnlyRectangleF Create( double x, double y, double width, double height ) => new((float)x, (float)y, (float)width, (float)height);
    [Pure] public static ReadOnlyRectangleF Create<TPoint>( in TPoint topLeft, in TPoint bottomRight )
        where TPoint : struct, IPoint<TPoint> => Create(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    [Pure] public static ReadOnlyRectangleF Create<TPoint, TSize>( in TPoint point, in TSize size )
        where TPoint : struct, IPoint<TPoint>
        where TSize : struct, ISize<TSize> => Create(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create<TSize>( double x, double y, in TSize size )
        where TSize : struct, ISize<TSize> => Create(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create<TRectangle>( in TRectangle rectangle, in ReadOnlyThickness padding )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        double x      = ( rectangle.X      + padding.Left );
        double y      = ( rectangle.Y      + padding.Top );
        double width  = ( rectangle.Width  - padding.HorizontalThickness );
        double height = ( rectangle.Height - padding.VerticalThickness );
        return Create(x, y, width, height);
    }


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is ReadOnlyRectangleF x
                   ? CompareTo(x)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(ReadOnlyRectangleF));
    }
    public int CompareTo( ReadOnlyRectangleF other )
    {
        int xComparison = X.CompareTo(other.X);
        if ( xComparison != 0 ) { return xComparison; }

        int yComparison = Y.CompareTo(other.Y);
        if ( yComparison != 0 ) { return yComparison; }

        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public          bool   Equals( ReadOnlyRectangleF other )                          => X.Equals(other.X)             && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool   Equals( object?            other )                          => other is ReadOnlyRectangleF x && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y, Width, Height);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<ReadOnlyRectangleF>.ToString(in this, format);


    public static bool operator ==( ReadOnlyRectangleF?             left, ReadOnlyRectangleF?              right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangleF?             left, ReadOnlyRectangleF?              right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => EqualityComparer<ReadOnlyRectangleF>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => !EqualityComparer<ReadOnlyRectangleF>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyRectangleF               left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyRectangleF               left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyRectangleF              left, ReadOnlyRectangleF               right ) => Comparer<ReadOnlyRectangleF>.Default.Compare(left, right) <= 0;
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, ReadOnlyRectangleF               value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, ReadOnlyRectangleF               value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, ReadOnlyRectangleF               value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, ReadOnlyRectangleF               value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, ReadOnlyRectangle                value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, ReadOnlyRectangle                value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, ReadOnlyRectangle                value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, ReadOnlyRectangle                value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, double                           value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, double                           value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, double                           value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, double                           value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, float                            value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, float                            value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, float                            value ) => self.Divide(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, float                            value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF self, int                              value ) => self.Add(value);
    public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF self, int                              value ) => self.Subtract(value);
    public static ReadOnlyRectangleF operator *( ReadOnlyRectangleF self, int                              value ) => self.Multiply(value);
    public static ReadOnlyRectangleF operator /( ReadOnlyRectangleF self, int                              value ) => self.Divide(value);
}
