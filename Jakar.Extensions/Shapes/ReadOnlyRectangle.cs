// TrueLogic :: iTrueLogic.Shared
// 09/21/2023  5:37 PM

using ZLinq;



namespace Jakar.Extensions;


[StructLayout(LayoutKind.Sequential), DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyRectangle( double x, double y, double width, double height ) : IRectangle<ReadOnlyRectangle>, IMathOperators<ReadOnlyRectangle>
{
    public static readonly ReadOnlyRectangle Invalid = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly ReadOnlyRectangle Zero    = new(0, 0, 0, 0);
    public readonly        double            X       = x;
    public readonly        double            Y       = y;
    public readonly        double            Width   = width;
    public readonly        double            Height  = height;


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


    [Pure]
    public static ReadOnlyRectangle Create<T>( ref readonly T rect )
        where T : IRectangle<T>
    {
        return new ReadOnlyRectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
    [Pure] public static ReadOnlyRectangle Create( params ReadOnlySpan<ReadOnlyPoint> points )                                                                  => RectangleD.Create(points);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPoint               point,     in ReadOnlySize      size )                                    => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangle Create( in     ReadOnlyPoint               topLeft,   in ReadOnlyPoint     bottomRight )                             => new(topLeft.X, topLeft.Y, bottomRight.X        - topLeft.X, bottomRight.Y                      - topLeft.Y);
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


    [Pure] public bool              IsAtLeast( in       ReadOnlySize                other )  => other.Width <= Width && other.Height <= Height;
    [Pure] public bool              IsAtLeast( in       ReadOnlySizeF               other )  => other.Width <= Width && other.Height <= Height;
    [Pure] public bool              Contains( in        ReadOnlyPoint               other )  => other.X     >= X     && other.X      < Right && other.Y >= Y && other.Y < Bottom;
    [Pure] public bool              Contains( in        ReadOnlyPointF              other )  => other.X     >= X     && other.X      < Right && other.Y >= Y && other.Y < Bottom;
    public        bool              ContainsAny( params ReadOnlySpan<ReadOnlyPoint> others ) => RectangleD.ContainsAny(in this, others);
    public        bool              ContainsAll( params ReadOnlySpan<ReadOnlyPoint> others ) => RectangleD.ContainsAll(in this, others);
    [Pure] public bool              Contains( in        ReadOnlyRectangle           other )  => Left <= other.Left && Right >= other.Right && Top <= other.Top && Bottom >= other.Bottom;
    [Pure] public bool              IntersectsWith( in  ReadOnlyRectangle           other )  => ( Left >= other.Right || Right <= other.Left || Top >= other.Bottom || Bottom <= other.Top ) is false;
    [Pure] public ReadOnlyRectangle Union( in           ReadOnlyRectangle           other )  => new(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Max(Right, other.Right), Math.Max(Bottom, other.Bottom));
    [Pure] public ReadOnlyRectangle Round()                                                  => new(X.Round(), Y.Round(), Width.Round(), Height.Round());


    [Pure]
    public ReadOnlyRectangle Intersection( in ReadOnlyRectangle other )
    {
        double x      = Math.Max(X, other.X);
        double y      = Math.Max(Y, other.Y);
        double width  = Math.Min(Right,  other.Right)  - x;
        double height = Math.Min(Bottom, other.Bottom) - y;

        return width < 0 || height < 0
                   ? Zero
                   : new ReadOnlyRectangle(x, y, width, height);
    }

    [Pure]
    public bool Contains( params ReadOnlySpan<ReadOnlyPoint> points )
    {
        foreach ( ReadOnlyPoint point in points )
        {
            if ( Contains(in point) ) { return true; }
        }

        return false;
    }

    [Pure]
    public bool Contains( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        foreach ( ReadOnlyPointF point in points )
        {
            if ( Contains(in point) ) { return true; }
        }

        return false;
    }

    [Pure]
    public bool DoesLineIntersect( in ReadOnlyPoint source, in ReadOnlyPoint target )
    {
        double               t0          = 0.0;
        double               t1          = 1.0;
        double               dx          = target.X - source.X;
        double               dy          = target.Y - source.Y;
        ReadOnlySpan<double> boundariesX = [X, X + Width];
        ReadOnlySpan<double> boundariesY = [Y, Y + Height];

        for ( int i = 0; i < 2; i++ )
        {
            double pX = i == 0
                            ? -dx
                            : dx;

            double pY = i == 0
                            ? -dy
                            : dy;

            for ( int j = 0; j < 2; j++ )
            {
                double qX = j == 0
                                ? source.X       - boundariesX[i]
                                : boundariesX[i] - source.X;

                double qY = j == 0
                                ? source.Y       - boundariesY[i]
                                : boundariesY[i] - source.Y;

                if ( pX == 0 && qX < 0 ) { return false; } // Line is parallel to the rectangle's horizontal edge and outside of it

                if ( pY == 0 && qY < 0 ) { return false; } // Line is parallel to the rectangle's vertical edge and outside of it

                double rX = pX != 0
                                ? qX / pX
                                : double.MaxValue;

                double rY = pY != 0
                                ? qY / pY
                                : double.MaxValue;

                if ( pX < 0 ) { t0 = Math.Max(t0, rX); }
                else { t1          = Math.Min(t1, rX); }

                if ( pY < 0 ) { t0 = Math.Max(t0, rY); }
                else { t1          = Math.Min(t1, rY); }

                if ( t0 > t1 ) { return false; }
            }
        }

        return true;
    }


    [Pure]
    public bool DoesLineIntersect( in ReadOnlyPointF source, in ReadOnlyPointF target )
    {
        double               t0          = 0.0;
        double               t1          = 1.0;
        double               dx          = target.X - source.X;
        double               dy          = target.Y - source.Y;
        ReadOnlySpan<double> boundariesX = [X, X + Width];
        ReadOnlySpan<double> boundariesY = [Y, Y + Height];

        for ( int i = 0; i < 2; i++ )
        {
            double pX = i == 0
                            ? -dx
                            : dx;

            double pY = i == 0
                            ? -dy
                            : dy;

            for ( int j = 0; j < 2; j++ )
            {
                double qX = j == 0
                                ? source.X       - boundariesX[i]
                                : boundariesX[i] - source.X;

                double qY = j == 0
                                ? source.Y       - boundariesY[i]
                                : boundariesY[i] - source.Y;

                if ( pX == 0 && qX < 0 ) { return false; } // Line is parallel to the rectangle's horizontal edge and outside of it

                if ( pY == 0 && qY < 0 ) { return false; } // Line is parallel to the rectangle's vertical edge and outside of it

                double rX = pX != 0
                                ? qX / pX
                                : double.MaxValue;

                double rY = pY != 0
                                ? qY / pY
                                : double.MaxValue;

                if ( pX < 0 ) { t0 = Math.Max(t0, rX); }
                else { t1          = Math.Min(t1, rX); }

                if ( pY < 0 ) { t0 = Math.Max(t0, rY); }
                else { t1          = Math.Min(t1, rY); }

                if ( t0 > t1 ) { return false; }
            }
        }

        return true;
    }


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


    public static implicit operator Rectangle( ReadOnlyRectangle  rectangle ) => new((int)rectangle.X.Round(), (int)rectangle.Y.Round(), (int)rectangle.Width.Round(), (int)rectangle.Height.Round());
    public static implicit operator RectangleF( ReadOnlyRectangle rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator RectangleD( ReadOnlyRectangle rectangle ) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);


    public static        bool operator ==( ReadOnlyRectangle             left,      ReadOnlyRectangle                right )  => Sorter.Equals(left, right);
    public static        bool operator !=( ReadOnlyRectangle             left,      ReadOnlyRectangle                right )  => Sorter.DoesNotEqual(left, right);
    public static        bool operator >( ReadOnlyRectangle              left,      ReadOnlyRectangle                right )  => Sorter.GreaterThan(left, right);
    public static        bool operator >=( ReadOnlyRectangle             left,      ReadOnlyRectangle                right )  => Sorter.GreaterThanOrEqualTo(left, right);
    public static        bool operator <( ReadOnlyRectangle              left,      ReadOnlyRectangle                right )  => Sorter.LessThan(left, right);
    public static        bool operator <=( ReadOnlyRectangle             left,      ReadOnlyRectangle                right )  => Sorter.LessThanOrEqualTo(left, right);
    [Pure] public static ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, ReadOnlyThickness                margin ) => new(rectangle.X - margin.Left, rectangle.Y - margin.Top, rectangle.Width + margin.Right, rectangle.Height + margin.Bottom);
    [Pure] public static ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, ReadOnlyThickness                margin ) => new(rectangle.X + margin.Left, rectangle.Y + margin.Top, rectangle.Width - margin.Right, rectangle.Height - margin.Bottom);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X + other.X, rectangle.Y     + other.Y, rectangle.Width    + other.Width, rectangle.Height  + other.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X - other.X, rectangle.Y     - other.Y, rectangle.Width    - other.Width, rectangle.Height  - other.Height);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        ReadOnlyRectangle operator &( ReadOnlyRectangle rectangle, ReadOnlyPointF                   other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, ReadOnlyPointF                   other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, ReadOnlyPointF                   other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator &( ReadOnlyRectangle rectangle, ReadOnlyPoint                    other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, ReadOnlyPoint                    other )  => new(rectangle.X                               + other.X, rectangle.Y    + other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, ReadOnlyPoint                    other )  => new(rectangle.X                               - other.X, rectangle.Y    - other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator +( ReadOnlyRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator -( ReadOnlyRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator *( ReadOnlyRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangle operator /( ReadOnlyRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
}
