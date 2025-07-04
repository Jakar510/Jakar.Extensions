// TrueLogic :: iTrueLogic.Shared
// 09/21/2023  5:37 PM

namespace Jakar.Extensions;


[StructLayout(LayoutKind.Sequential), DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyRectangle( double x, double y, double width, double height ) : IRectangle<ReadOnlyRectangle>
{
    public static readonly ReadOnlyRectangle Invalid       = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly ReadOnlyRectangle Zero          = new(0, 0, 0, 0);
    public static readonly ReadOnlyRectangle One           = 1;
    public static readonly ReadOnlyRectangle Two           = 2;
    public static readonly ReadOnlyRectangle Three         = 3;
    public static readonly ReadOnlyRectangle Four          = 4;
    public static readonly ReadOnlyRectangle Five          = 5;
    public static readonly ReadOnlyRectangle Six           = 6;
    public static readonly ReadOnlyRectangle Seven         = 7;
    public static readonly ReadOnlyRectangle Eight         = 8;
    public static readonly ReadOnlyRectangle Nine          = 9;
    public static readonly ReadOnlyRectangle Ten           = 10;
    public static readonly ReadOnlyRectangle NegativeOne   = -1;
    public static readonly ReadOnlyRectangle NegativeTwo   = -2;
    public static readonly ReadOnlyRectangle NegativeThree = -3;
    public static readonly ReadOnlyRectangle NegativeFour  = -4;
    public static readonly ReadOnlyRectangle NegativeFive  = -5;
    public static readonly ReadOnlyRectangle NegativeSix   = -6;
    public static readonly ReadOnlyRectangle NegativeSeven = -7;
    public static readonly ReadOnlyRectangle NegativeEight = -8;
    public static readonly ReadOnlyRectangle NegativeNine  = -9;
    public static readonly ReadOnlyRectangle NegativeTen   = -10;
    public readonly        double            X             = x;
    public readonly        double            Y             = y;
    public readonly        double            Width         = width;
    public readonly        double            Height        = height;


    public static       Sorter<ReadOnlyRectangle>                   Sorter   => Sorter<ReadOnlyRectangle>.Default;
    static ref readonly ReadOnlyRectangle IShape<ReadOnlyRectangle>.Zero     => ref Zero;
    static ref readonly ReadOnlyRectangle IShape<ReadOnlyRectangle>.Invalid  => ref Invalid;
    public              bool                                        IsEmpty  => IRectangle<ReadOnlyRectangle>.CheckIfEmpty(in this);
    double IShapeLocation.                                          X        => X;
    double IShapeLocation.                                          Y        => Y;
    double IShapeSize.                                              Width    => Width;
    double IShapeSize.                                              Height   => Height;
    public bool                                                     IsNaN    => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Width) || double.IsNaN(Height);
    public bool                                                     IsValid  => IsNaN is false && X >= 0 && Y >= 0 && Width >= 0 && Height >= 0;
    public ReadOnlyPoint                                            Center   => new(Right / 2, Bottom / 2);
    public ReadOnlyPoint                                            Location => new(X, Y);
    public ReadOnlySize                                             Size     => new(Width, Height);
    public double                                                   Bottom   => Y + Height;
    public double                                                   Left     => X;
    public double                                                   Right    => X + Width;
    public double                                                   Top      => Y;


    public static implicit operator Rectangle( ReadOnlyRectangle          rectangle ) => new((int)rectangle.X.Round(), (int)rectangle.Y.Round(), (int)rectangle.Width.Round(), (int)rectangle.Height.Round());
    public static implicit operator RectangleF( ReadOnlyRectangle         rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( ReadOnlyRectangle rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlyRectangle( int                value )     => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangle( long               value )     => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangle( float              value )     => new(value, value, value, value);
    public static implicit operator ReadOnlyRectangle( double             value )     => new(value, value, value, value);


    [Pure]
    public static ReadOnlyRectangle Create<T>( ref readonly T rect )
        where T : IRectangle<T>
    {
        return new ReadOnlyRectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
    [Pure] public static ReadOnlyRectangle Create( params ReadOnlySpan<ReadOnlyPoint> points )                                                                  => MutableRectangle.Create(points);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPoint               point,     in ReadOnlySize      size )                                    => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPointF              point,     in ReadOnlySizeF     size )                                    => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPoint               topLeft,   in ReadOnlyPoint     bottomRight )                             => new(topLeft.X, topLeft.Y, bottomRight.X        - topLeft.X, bottomRight.Y                      - topLeft.Y);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPoint               topLeft,   in ReadOnlyPointF    bottomRight )                             => new(topLeft.X, topLeft.Y, bottomRight.X        - topLeft.X, bottomRight.Y                      - topLeft.Y);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPointF              topLeft,   in ReadOnlyPointF    bottomRight )                             => new(topLeft.X, topLeft.Y, bottomRight.X        - topLeft.X, bottomRight.Y                      - topLeft.Y);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyRectangle           rectangle, in ReadOnlyThickness padding )                                 => new(padding.Left, padding.Top, rectangle.Width - padding.HorizontalThickness, rectangle.Height - padding.VerticalThickness);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyRectangleF          rectangle, in ReadOnlyThickness padding )                                 => new(padding.Left, padding.Top, rectangle.Width - padding.HorizontalThickness, rectangle.Height - padding.VerticalThickness);
    [Pure] public static ReadOnlyRectangle Create( float                              x,         float                y, in ReadOnlySize size )                 => new(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create( double                             x,         double               y, in ReadOnlySize size )                 => new(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create( float                              x,         float                y, float           width, float  height ) => new(x, y, width, height);
    [Pure] public static ReadOnlyRectangle Create( double                             x,         double               y, double          width, double height ) => new(x, y, width, height);


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
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<ReadOnlyRectangle>.ToString(this, format);


    public void Deconstruct( out float x, out float y, out float width, out float height )
    {
        x      = X.AsFloat();
        y      = Y.AsFloat();
        width  = Width.AsFloat();
        height = Height.AsFloat();
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


    public static bool operator ==( ReadOnlyRectangle left, ReadOnlyRectangle right ) => Sorter.Equals(left, right);
    public static bool operator !=( ReadOnlyRectangle left, ReadOnlyRectangle right ) => Sorter.DoesNotEqual(left, right);
    public static bool operator >( ReadOnlyRectangle  left, ReadOnlyRectangle right ) => Sorter.GreaterThan(left, right);
    public static bool operator >=( ReadOnlyRectangle left, ReadOnlyRectangle right ) => Sorter.GreaterThanOrEqualTo(left, right);
    public static bool operator <( ReadOnlyRectangle  left, ReadOnlyRectangle right ) => Sorter.LessThan(left, right);
    public static bool operator <=( ReadOnlyRectangle left, ReadOnlyRectangle right ) => Sorter.LessThanOrEqualTo(left, right);
}
