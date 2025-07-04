// TrueLogic :: TrueLogic.Common.Maui
// 01/20/2025  08:01

namespace Jakar.Extensions;


[StructLayout(LayoutKind.Sequential), DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyRectangleF( float x, float y, float width, float height ) : IShape<ReadOnlyRectangleF>, IShapeSize, IShapeLocation
{
    public static readonly ReadOnlyRectangleF Invalid       = new(float.NaN, float.NaN, float.NaN, float.NaN);
    public static readonly ReadOnlyRectangleF Zero          = new(0, 0, 0, 0);
    public static readonly ReadOnlyRectangleF  One           = 1;
    public static readonly ReadOnlyRectangleF  Two           = 2;
    public static readonly ReadOnlyRectangleF  Three         = 3;
    public static readonly ReadOnlyRectangleF  Four          = 4;
    public static readonly ReadOnlyRectangleF  Five          = 5;
    public static readonly ReadOnlyRectangleF  Six           = 6;
    public static readonly ReadOnlyRectangleF  Seven         = 7;
    public static readonly ReadOnlyRectangleF  Eight         = 8;
    public static readonly ReadOnlyRectangleF  Nine          = 9;
    public static readonly ReadOnlyRectangleF  Ten           = 10;
    public static readonly ReadOnlyRectangleF  NegativeOne   = -1;
    public static readonly ReadOnlyRectangleF  NegativeTwo   = -2;
    public static readonly ReadOnlyRectangleF  NegativeThree = -3;
    public static readonly ReadOnlyRectangleF  NegativeFour  = -4;
    public static readonly ReadOnlyRectangleF  NegativeFive  = -5;
    public static readonly ReadOnlyRectangleF  NegativeSix   = -6;
    public static readonly ReadOnlyRectangleF  NegativeSeven = -7;
    public static readonly ReadOnlyRectangleF  NegativeEight = -8;
    public static readonly ReadOnlyRectangleF  NegativeNine  = -9;
    public static readonly ReadOnlyRectangleF  NegativeTen   = -10;
    public readonly        float              X             = x;
    public readonly        float              Y             = y;
    public readonly        float              Width         = width;
    public readonly        float              Height        = height;


    public static       Sorter<ReadOnlyRectangleF>                    Sorter  => Sorter<ReadOnlyRectangleF>.Default;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.Zero    => ref Zero;
    static ref readonly ReadOnlyRectangleF IShape<ReadOnlyRectangleF>.Invalid => ref Invalid;


    public bool           IsEmpty  => double.IsNaN(Bottom) || double.IsNaN(Top) || double.IsNegative(Left) || double.IsNaN(Left) || double.IsNaN(Right) || double.IsNegative(Right);
    double IShapeLocation.X        => X;
    double IShapeLocation.Y        => Y;
    double IShapeSize.    Width    => Width;
    double IShapeSize.    Height   => Height;
    public bool           IsNaN    => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Width) || double.IsNaN(Height);
    public bool           IsValid  => IsNaN is false && X >= 0 && Y >= 0 && Width >= 0 && Height >= 0;
    public ReadOnlyPoint  Center   => new(Right / 2, Bottom / 2);
    public ReadOnlyPoint  Location => new(X, Y);
    public ReadOnlySize   Size     => new(Width, Height);
    public float          Bottom   => Y + Height;
    public float          Left     => X;
    public float          Right    => X + Width;
    public float          Top      => Y;


    public static implicit operator Rectangle( ReadOnlyRectangleF  rectangle ) => rectangle.ToDrawingRect();
    public static implicit operator RectangleF( ReadOnlyRectangleF rectangle ) => rectangle.ToDrawingRectF();
    public static implicit operator ReadOnlyRectangleF( Rectangle  rectangle ) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( RectangleF rectangle ) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( int        value )     => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangleF( long       value )     => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangleF( float      value )     => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangleF( double     value )     => value.AsFloat();


    [Pure]
    public static ReadOnlySize AddMargin( in ReadOnlySize value, in ReadOnlyThickness margin )
    {
        ReadOnlySize result = new(value.Width + margin.HorizontalThickness, value.Height + margin.VerticalThickness);
        Debug.Assert(result >= value);
        return result;
    }


    [Pure] public static ReadOnlyRectangleF Create( params ReadOnlySpan<ReadOnlyPoint>  points )                                 => MutableRectangle.Create(points);
    [Pure] public static ReadOnlyRectangleF Create( params ReadOnlySpan<ReadOnlyPointF> points )                                 => MutableRectangle.Create(points);
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPoint                point,   in ReadOnlySize   size )        => new(point.X.AsFloat(), point.Y.AsFloat(), size.Width.AsFloat(), size.Height.AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPointF               point,   in ReadOnlySizeF  size )        => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPoint                topLeft, in ReadOnlyPoint  bottomRight ) => new(topLeft.X.AsFloat(), topLeft.Y.AsFloat(), ( bottomRight.X - topLeft.X ).AsFloat(), ( bottomRight.Y - topLeft.Y ).AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPointF               topLeft, in ReadOnlyPointF bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

    [Pure]
    public static ReadOnlyRectangleF Create( in ReadOnlyRectangle rectangle, in ReadOnlyThickness padding )
    {
        float x      = (float)( rectangle.Left   + padding.Left );
        float y      = (float)( rectangle.Top    + padding.Top );
        float width  = (float)( rectangle.Width  - padding.HorizontalThickness );
        float height = (float)( rectangle.Height - padding.VerticalThickness );
        return new ReadOnlyRectangleF(x, y, width, height);
    }

    [Pure]
    public static ReadOnlyRectangleF Create( in ReadOnlyRectangleF rectangle, in ReadOnlyThickness padding )
    {
        float x      = (float)( rectangle.Left   + padding.HorizontalThickness );
        float y      = (float)( rectangle.Top    + padding.VerticalThickness );
        float width  = (float)( rectangle.Width  - padding.HorizontalThickness );
        float height = (float)( rectangle.Height - padding.VerticalThickness );
        return new ReadOnlyRectangleF(x, y, width, height);
    }


    [Pure] public static ReadOnlyRectangleF Create( double x, double y, in ReadOnlySize  size )                 => new(x.AsFloat(), y.AsFloat(), size.Width.AsFloat(), size.Height.AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( double x, double y, double           width, double height ) => new(x.AsFloat(), y.AsFloat(), width.AsFloat(), height.AsFloat());
    [Pure] public static ReadOnlyRectangleF Create( float  x, float  y, in ReadOnlySizeF size )                => new(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create( float  x, float  y, float            width, float height ) => new(x, y, width, height);


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
    public          bool   Equals( ReadOnlyRectangleF other ) => X.Equals(other.X)             && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool   Equals( object?            other ) => other is ReadOnlyRectangleF x && Equals(x);
    public override int    GetHashCode()                      => HashCode.Combine(X, Y, Width, Height);
    public override string ToString()                         => ToString(null, null);
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return this.ToJson();

            case ",":
                return $"{X},{Y},{Width},{Height}";

            case "-":
                return $"{X}-{Y}-{Width}-{Height}";

            case EMPTY:
            case null:
            default:
                return $"{nameof(ReadOnlyRectangleF)}<{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}>";
        }
    }


    public void Deconstruct( out float x, out float y, out float width, out float height )
    {
        x      = X;
        y      = Y;
        width  = Width;
        height = Height;
    }
    public void Deconstruct( out double x, out double y, out double width, out double height )
    {
        x      = X;
        y      = Y;
        width  = Width;
        height = Height;
    }
    public void Deconstruct( out ReadOnlyPoint point, out ReadOnlySize size )
    {
        point = Location;
        size  = Size;
    }
    public void Deconstruct( out ReadOnlyPointF point, out ReadOnlySizeF size )
    {
        point = Location;
        size  = Size;
    }


    public MutableRectangle ToRectD()        => new(X, Y, Width, Height);
    public Rectangle        ToDrawingRect()  => new((int)X.Round(), (int)Y.Round(), (int)Width.Round(), (int)Height.Round());
    public RectangleF       ToDrawingRectF() => new(X, Y, Width, Height);


    public static bool operator ==( ReadOnlyRectangleF left, ReadOnlyRectangleF right ) => Sorter.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangleF left, ReadOnlyRectangleF right ) => Sorter.DoesNotEqual(left, right);
    public static bool operator >( ReadOnlyRectangleF  left, ReadOnlyRectangleF right ) => Sorter.GreaterThan(left, right);
    public static bool operator >=( ReadOnlyRectangleF left, ReadOnlyRectangleF right ) => Sorter.GreaterThanOrEqualTo(left, right);
    public static bool operator <( ReadOnlyRectangleF  left, ReadOnlyRectangleF right ) => Sorter.LessThan(left, right);
    public static bool operator <=( ReadOnlyRectangleF left, ReadOnlyRectangleF right ) => Sorter.LessThanOrEqualTo(left, right);
}
