using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public struct MutableRectangle( double x, double y, double width, double height ) : IMutableRectangle<MutableRectangle>
{
    public static readonly MutableRectangle Invalid = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly MutableRectangle Zero    = new(0, 0, 0, 0);
    public static readonly MutableRectangle One     = 1;


    public static       JsonSerializerContext                     JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<MutableRectangle>            JsonTypeInfo  => JakarShapesContext.Default.MutableRectangle;
    public static       JsonTypeInfo<MutableRectangle[]>          JsonArrayInfo => JakarShapesContext.Default.MutableRectangleArray;
    static ref readonly MutableRectangle IShape<MutableRectangle>.Zero          => ref Zero;
    static ref readonly MutableRectangle IShape<MutableRectangle>.Invalid       => ref Invalid;
    static ref readonly MutableRectangle IShape<MutableRectangle>.One           => ref One;
    bool IValidator.                                              IsValid       => this.IsValid();
    public double                                                 X             { get; set; } = x;
    public double                                                 Y             { get; set; } = y;
    public double                                                 Width         { get; set; } = width;
    public double                                                 Height        { get; set; } = height;
    public ReadOnlyPoint Location
    {
        readonly get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
    public ReadOnlySize Size
    {
        readonly get => new(Width, Height);
        set
        {
            Width  = value.Width;
            Height = value.Height;
        }
    }


    public static implicit operator Rectangle( MutableRectangle          self )  => new((int)self.X, (int)self.Y, (int)self.Width, (int)self.Height);
    public static implicit operator RectangleF( MutableRectangle         self )  => new(self.X.AsFloat(), self.Y.AsFloat(), self.Width.AsFloat(), self.Height.AsFloat());
    public static implicit operator ReadOnlyRectangleF( MutableRectangle self )  => new(self.X.AsFloat(), self.Y.AsFloat(), self.Width.AsFloat(), self.Height.AsFloat());
    public static implicit operator ReadOnlyRectangle( MutableRectangle  rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( Rectangle          rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( RectangleF         rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlyRectangle  rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlyRectangleF rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlySize       rect )  => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlySizeF      rect )  => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( MutableSize        rect )  => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlyPoint      rect )  => new(rect.X, rect.Y, 0, 0);
    public static implicit operator MutableRectangle( ReadOnlyPointF     rect )  => new(rect.X, rect.Y, 0, 0);
    public static implicit operator MutableRectangle( int                other ) => new(other, other, other, other);
    public static implicit operator MutableRectangle( long               other ) => new(other, other, other, other);
    public static implicit operator MutableRectangle( float              other ) => new(other, other, other, other);
    public static implicit operator MutableRectangle( double             other ) => new(other, other, other, other);


    public static bool TryFromJson( string? json, out MutableRectangle result )
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
    public static MutableRectangle FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    [Pure] public static MutableRectangle Create<TPoint>( params ReadOnlySpan<TPoint> points )
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
    [Pure] public static MutableRectangle Create<TRectangle>( in TRectangle rect )
        where TRectangle : struct, IRectangle<TRectangle> => new(rect.X, rect.Y, rect.Width, rect.Height);
    [Pure] public static MutableRectangle Create( float  x, float  y, float  width, float  height ) => new(x, y, width, height);
    [Pure] public static MutableRectangle Create( double x, double y, double width, double height ) => new(x, y, width, height);
    [Pure] public static MutableRectangle Create<TPoint>( in TPoint topLeft, in TPoint bottomRight )
        where TPoint : struct, IPoint<TPoint> => new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    [Pure] public static MutableRectangle Create<TPoint, TSize>( in TPoint point, in TSize size )
        where TPoint : struct, IPoint<TPoint>
        where TSize : struct, ISize<TSize> => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static MutableRectangle Create<TSize>( double x, double y, in TSize size )
        where TSize : struct, ISize<TSize> => new(x, y, size.Width, size.Height);
    [Pure] public static MutableRectangle Create<TRectangle>( in TRectangle rectangle, in ReadOnlyThickness padding )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        double x      = rectangle.X      + padding.Left;
        double y      = rectangle.Y      + padding.Top;
        double width  = rectangle.Width  - padding.HorizontalThickness;
        double height = rectangle.Height - padding.VerticalThickness;
        return new MutableRectangle(x, y, width, height);
    }


    public void AddMargin( in ReadOnlyThickness margin )
    {
        X      -= margin.Left;
        Y      -= margin.Top;
        Width  += margin.Right;
        Height += margin.Bottom;
    }


    public void Deconstruct( out double x, out double y )
    {
        x = X;
        y = Y;
    }

    public readonly int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is MutableRectangle x
                   ? CompareTo(x)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(MutableRectangle));
    }
    public readonly int CompareTo( MutableRectangle other )
    {
        int xComparison = X.CompareTo(other.X);
        if ( xComparison != 0 ) { return xComparison; }

        int yComparison = Y.CompareTo(other.Y);
        if ( yComparison != 0 ) { return yComparison; }

        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public readonly bool   Equals( MutableRectangle other )                            => X.Equals(other.X)           && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool   Equals( object?          other )                            => other is MutableRectangle x && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y, Width, Height);
    public override string ToString()                                                  => ToString(null, null);
    public readonly string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<MutableRectangle>.ToString(in this, format);


    public MutableRectangle Round()
    {
        X      = X.Round();
        Y      = Y.Round();
        Width  = Width.Round();
        Height = Height.Round();
        return this;
    }
    public MutableRectangle Floor()
    {
        X      = X.Floor();
        Y      = Y.Floor();
        Width  = Width.Floor();
        Height = Height.Floor();
        return this;
    }
    public MutableRectangle Reverse()
    {
        double x      = X;
        double y      = Y;
        double width  = Width;
        double height = Height;
        X      = y;
        Y      = x;
        Width  = height;
        Height = width;
        return this;
    }


    public void Deconstruct( out float x, out float y, out float width, out float height )
    {
        x      = X.AsFloat();
        y      = Y.AsFloat();
        width  = Width.AsFloat();
        height = Height.AsFloat();
    }
    public readonly void Deconstruct( out double x, out double y, out double width, out double height )
    {
        x      = X;
        y      = Y;
        width  = Width;
        height = Height;
    }
    public readonly void Deconstruct( out ReadOnlyPoint point, out ReadOnlySize size )
    {
        point = Location;
        size  = Size;
    }
    public readonly void Deconstruct( out ReadOnlyPointF point, out ReadOnlySizeF size )
    {
        point = Location;
        size  = Size;
    }
    public readonly void Deconstruct( out ReadOnlyPoint point, out MutableSize size )
    {
        point = Location;
        size  = Size;
    }
    public readonly void Deconstruct( out ReadOnlyPointF point, out MutableSize size )
    {
        point = Location;
        size  = Size;
    }


    public static bool operator ==( MutableRectangle?           left, MutableRectangle?                right ) => Nullable.Equals(left, right);
    public static bool operator !=( MutableRectangle?           left, MutableRectangle?                right ) => !Nullable.Equals(left, right);
    public static bool operator ==( MutableRectangle            left, MutableRectangle                 right ) => EqualityComparer<MutableRectangle>.Default.Equals(left, right);
    public static bool operator !=( MutableRectangle            left, MutableRectangle                 right ) => !EqualityComparer<MutableRectangle>.Default.Equals(left, right);
    public static bool operator >( MutableRectangle             left, MutableRectangle                 right ) => Comparer<MutableRectangle>.Default.Compare(left, right) > 0;
    public static bool operator >=( MutableRectangle            left, MutableRectangle                 right ) => Comparer<MutableRectangle>.Default.Compare(left, right) >= 0;
    public static bool operator <( MutableRectangle             left, MutableRectangle                 right ) => Comparer<MutableRectangle>.Default.Compare(left, right) < 0;
    public static bool operator <=( MutableRectangle            left, MutableRectangle                 right ) => Comparer<MutableRectangle>.Default.Compare(left, right) <= 0;
    public static MutableRectangle operator +( MutableRectangle self, MutableRectangle                 other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, MutableRectangle                 other ) => self.Subtract(other);
    public static MutableRectangle operator /( MutableRectangle self, MutableRectangle                 other ) => self.Divide(other);
    public static MutableRectangle operator *( MutableRectangle self, MutableRectangle                 other ) => self.Multiply(other);
    public static MutableRectangle operator +( MutableRectangle self, ReadOnlyRectangle                other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, ReadOnlyRectangle                other ) => self.Subtract(other);
    public static MutableRectangle operator /( MutableRectangle self, ReadOnlyRectangle                other ) => self.Divide(other);
    public static MutableRectangle operator *( MutableRectangle self, ReadOnlyRectangle                other ) => self.Multiply(other);
    public static MutableRectangle operator +( MutableRectangle self, ReadOnlyRectangleF               other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, ReadOnlyRectangleF               other ) => self.Subtract(other);
    public static MutableRectangle operator /( MutableRectangle self, ReadOnlyRectangleF               other ) => self.Divide(other);
    public static MutableRectangle operator *( MutableRectangle self, ReadOnlyRectangleF               other ) => self.Multiply(other);
    public static MutableRectangle operator +( MutableRectangle self, (int xOffset, int yOffset)       other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, (int xOffset, int yOffset)       other ) => self.Subtract(other);
    public static MutableRectangle operator /( MutableRectangle self, (int xOffset, int yOffset)       other ) => self.Divide(other);
    public static MutableRectangle operator *( MutableRectangle self, (int xOffset, int yOffset)       other ) => self.Multiply(other);
    public static MutableRectangle operator +( MutableRectangle self, (float xOffset, float yOffset)   other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, (float xOffset, float yOffset)   other ) => self.Subtract(other);
    public static MutableRectangle operator *( MutableRectangle self, (float xOffset, float yOffset)   other ) => self.Multiply(other);
    public static MutableRectangle operator /( MutableRectangle self, (float xOffset, float yOffset)   other ) => self.Divide(other);
    public static MutableRectangle operator +( MutableRectangle self, (double xOffset, double yOffset) other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, (double xOffset, double yOffset) other ) => self.Subtract(other);
    public static MutableRectangle operator *( MutableRectangle self, (double xOffset, double yOffset) other ) => self.Multiply(other);
    public static MutableRectangle operator /( MutableRectangle self, (double xOffset, double yOffset) other ) => self.Divide(other);
    public static MutableRectangle operator +( MutableRectangle self, double                           other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, double                           other ) => self.Subtract(other);
    public static MutableRectangle operator /( MutableRectangle self, double                           other ) => self.Divide(other);
    public static MutableRectangle operator *( MutableRectangle self, double                           other ) => self.Multiply(other);
    public static MutableRectangle operator +( MutableRectangle self, float                            other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, float                            other ) => self.Subtract(other);
    public static MutableRectangle operator /( MutableRectangle self, float                            other ) => self.Divide(other);
    public static MutableRectangle operator *( MutableRectangle self, float                            other ) => self.Multiply(other);
    public static MutableRectangle operator +( MutableRectangle self, int                              other ) => self.Add(other);
    public static MutableRectangle operator -( MutableRectangle self, int                              other ) => self.Subtract(other);
    public static MutableRectangle operator *( MutableRectangle self, int                              other ) => self.Multiply(other);
    public static MutableRectangle operator /( MutableRectangle self, int                              other ) => self.Divide(other);

    [Pure] public static MutableRectangle operator +( MutableRectangle self, ReadOnlyThickness margin ) => new(self.X - margin.Left, self.Y - margin.Top, self.Width + margin.Right, self.Height + margin.Bottom);
    [Pure] public static MutableRectangle operator -( MutableRectangle self, ReadOnlyThickness margin ) => new(self.X + margin.Left, self.Y + margin.Top, self.Width - margin.Right, self.Height - margin.Bottom);

    public static MutableRectangle operator +( MutableRectangle self, ReadOnlySize   other ) => new(self.X, self.Y, self.Width + other.Width, self.Height + other.Height);
    public static MutableRectangle operator -( MutableRectangle self, ReadOnlySize   other ) => new(self.X, self.Y, self.Width - other.Width, self.Height - other.Height);
    public static MutableRectangle operator *( MutableRectangle self, ReadOnlySize   other ) => new(self.X, self.Y, self.Width * other.Width, self.Height * other.Height);
    public static MutableRectangle operator /( MutableRectangle self, ReadOnlySize   other ) => new(self.X, self.Y, self.Width / other.Width, self.Height / other.Height);
    public static MutableRectangle operator +( MutableRectangle self, ReadOnlySizeF  other ) => new(self.X, self.Y, self.Width + other.Width, self.Height + other.Height);
    public static MutableRectangle operator -( MutableRectangle self, ReadOnlySizeF  other ) => new(self.X, self.Y, self.Width - other.Width, self.Height - other.Height);
    public static MutableRectangle operator *( MutableRectangle self, ReadOnlySizeF  other ) => new(self.X, self.Y, self.Width * other.Width, self.Height * other.Height);
    public static MutableRectangle operator /( MutableRectangle self, ReadOnlySizeF  other ) => new(self.X, self.Y, self.Width / other.Width, self.Height / other.Height);
    public static MutableRectangle operator +( MutableRectangle self, MutableSize    other ) => new(self.X, self.Y, self.Width + other.Width, self.Height + other.Height);
    public static MutableRectangle operator -( MutableRectangle self, MutableSize    other ) => new(self.X, self.Y, self.Width - other.Width, self.Height - other.Height);
    public static MutableRectangle operator *( MutableRectangle self, MutableSize    other ) => new(self.X, self.Y, self.Width * other.Width, self.Height * other.Height);
    public static MutableRectangle operator /( MutableRectangle self, MutableSize    other ) => new(self.X, self.Y, self.Width / other.Width, self.Height / other.Height);
    public static MutableRectangle operator +( MutableRectangle self, ReadOnlyPoint  other ) => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static MutableRectangle operator -( MutableRectangle self, ReadOnlyPoint  other ) => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static MutableRectangle operator *( MutableRectangle self, ReadOnlyPoint  other ) => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static MutableRectangle operator /( MutableRectangle self, ReadOnlyPoint  other ) => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static MutableRectangle operator +( MutableRectangle self, ReadOnlyPointF other ) => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static MutableRectangle operator -( MutableRectangle self, ReadOnlyPointF other ) => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static MutableRectangle operator *( MutableRectangle self, ReadOnlyPointF other ) => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static MutableRectangle operator /( MutableRectangle self, ReadOnlyPointF other ) => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static MutableRectangle operator +( MutableRectangle self, PointF         other ) => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static MutableRectangle operator -( MutableRectangle self, PointF         other ) => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static MutableRectangle operator *( MutableRectangle self, PointF         other ) => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static MutableRectangle operator /( MutableRectangle self, PointF         other ) => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static MutableRectangle operator +( MutableRectangle self, Point          other ) => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static MutableRectangle operator -( MutableRectangle self, Point          other ) => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static MutableRectangle operator *( MutableRectangle self, Point          other ) => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static MutableRectangle operator /( MutableRectangle self, Point          other ) => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static MutableRectangle operator &( MutableRectangle self, ReadOnlyPointF other ) => new(other.X, other.Y, self.Width, self.Height);
    public static MutableRectangle operator &( MutableRectangle self, ReadOnlyPoint  other ) => new(other.X, other.Y, self.Width, self.Height);
    public static MutableRectangle operator &( MutableRectangle self, PointF         other ) => new(other.X, other.Y, self.Width, self.Height);
    public static MutableRectangle operator &( MutableRectangle self, Point          other ) => new(other.X, other.Y, self.Width, self.Height);
    public static MutableRectangle operator &( MutableRectangle self, ReadOnlySize   other ) => new(self.X, self.Y, other.Width, other.Height);
    public static MutableRectangle operator &( MutableRectangle self, ReadOnlySizeF  other ) => new(self.X, self.Y, other.Width, other.Height);
    public static MutableRectangle operator &( MutableRectangle self, Size           other ) => new(self.X, self.Y, other.Width, other.Height);
    public static MutableRectangle operator &( MutableRectangle self, SizeF          other ) => new(self.X, self.Y, other.Width, other.Height);
    public static MutableRectangle operator &( MutableRectangle self, MutableSize    other ) => new(self.X, self.Y, other.Width, other.Height);
}
