namespace Jakar.Extensions;


[DefaultValue(nameof(Zero))]
public struct MutableRectangle( double x, double y, double width, double height ) : IMutableRectangle<MutableRectangle>
{
    public static readonly MutableRectangle Invalid       = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly MutableRectangle Zero          = new(0, 0, 0, 0);
    public static readonly MutableRectangle One           = 1;
    public static readonly MutableRectangle Two           = 2;
    public static readonly MutableRectangle Three         = 3;
    public static readonly MutableRectangle Four          = 4;
    public static readonly MutableRectangle Five          = 5;
    public static readonly MutableRectangle Six           = 6;
    public static readonly MutableRectangle Seven         = 7;
    public static readonly MutableRectangle Eight         = 8;
    public static readonly MutableRectangle Nine          = 9;
    public static readonly MutableRectangle Ten           = 10;
    public static readonly MutableRectangle NegativeOne   = -1;
    public static readonly MutableRectangle NegativeTwo   = -2;
    public static readonly MutableRectangle NegativeThree = -3;
    public static readonly MutableRectangle NegativeFour  = -4;
    public static readonly MutableRectangle NegativeFive  = -5;
    public static readonly MutableRectangle NegativeSix   = -6;
    public static readonly MutableRectangle NegativeSeven = -7;
    public static readonly MutableRectangle NegativeEight = -8;
    public static readonly MutableRectangle NegativeNine  = -9;
    public static readonly MutableRectangle NegativeTen   = -10;


    public static                Sorter<MutableRectangle>                  Sorter  => Sorter<MutableRectangle>.Default;
    static ref readonly          MutableRectangle IShape<MutableRectangle>.Zero    => ref Zero;
    static ref readonly          MutableRectangle IShape<MutableRectangle>.Invalid => ref Invalid;
    public                       double                                    X       { get; set; } = x;
    public                       double                                    Y       { get; set; } = y;
    public                       double                                    Width   { get; set; } = width;
    public                       double                                    Height  { get; set; } = height;
    public readonly              bool                                      IsNaN   => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Width) || double.IsNaN(Height);
    public readonly              bool                                      IsValid => IsNaN is false && X >= 0 && Y >= 0 && Width >= 0 && Height >= 0;
    [JsonIgnore] public readonly double                                    Bottom  => Y + Height;
    [JsonIgnore] public readonly ReadOnlyPoint                             Center  => new(( X + Width ) / 2, ( Y + Height ) / 2);
    [JsonIgnore] public readonly bool                                      IsEmpty => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Width) || Width <= 0 || double.IsNaN(Height) || Height <= 0;
    [JsonIgnore] public readonly double                                    Left    => X;
    public ReadOnlyPoint Location
    {
        readonly get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
    [JsonIgnore] public readonly double Right => X + Width;
    public ReadOnlySize Size
    {
        readonly get => new(Width, Height);
        set
        {
            Width  = value.Width;
            Height = value.Height;
        }
    }
    [JsonIgnore] public readonly double Top => Y;


    public static implicit operator Rectangle( MutableRectangle          rectangle ) => new((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
    public static implicit operator RectangleF( MutableRectangle         rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( MutableRectangle rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlyRectangle( MutableRectangle  rect )      => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( Rectangle          rect )      => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( RectangleF         rect )      => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlySize       rect )      => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlySizeF      rect )      => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlyPoint      rect )      => new(rect.X, rect.Y, 0, 0);
    public static implicit operator MutableRectangle( ReadOnlyPointF     rect )      => new(rect.X, rect.Y, 0, 0);
    public static implicit operator MutableRectangle( int                value )     => new(value, value, value, value);
    public static implicit operator MutableRectangle( long               value )     => new(value, value, value, value);
    public static implicit operator MutableRectangle( float              value )     => new(value, value, value, value);
    public static implicit operator MutableRectangle( double             value )     => new(value, value, value, value);


    [Pure]
    public static MutableRectangle Create( params ReadOnlySpan<ReadOnlyPoint> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableRectangle rectangle = Create(points[0].X, points[0].Y, points[0].X, points[0].Y);

        foreach ( ReadOnlyPoint point in points )
        {
            if ( point.X      < rectangle.Left ) { rectangle.X      = point.X; }
            else if ( point.X > rectangle.Right ) { rectangle.Width = point.X - rectangle.Left; }

            if ( point.Y      < rectangle.Top ) { rectangle.Y         = point.Y; }
            else if ( point.Y > rectangle.Bottom ) { rectangle.Height = point.Y - rectangle.Top; }
        }

        return rectangle;
    }
    [Pure]
    public static MutableRectangle Create( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableRectangle rectangle = Create(points[0].X, points[0].Y, points[0].X, points[0].Y);

        foreach ( ReadOnlyPointF point in points )
        {
            if ( point.X      < rectangle.Left ) { rectangle.X      = point.X; }
            else if ( point.X > rectangle.Right ) { rectangle.Width = point.X - rectangle.Left; }

            if ( point.Y      < rectangle.Top ) { rectangle.Y         = point.Y; }
            else if ( point.Y > rectangle.Bottom ) { rectangle.Height = point.Y - rectangle.Top; }
        }

        return rectangle;
    }

    [Pure]
    public static MutableRectangle Create<T>( ref readonly T rect )
        where T : IRectangle<T>
    {
        return new MutableRectangle(rect.X, rect.Y, rect.Width, rect.Height);
    }
    [Pure] public static MutableRectangle Create( double              x,         double               y,   in ReadOnlySize size )                 => new(x, y, size.Width, size.Height);
    [Pure] public static MutableRectangle Create( double              left,      double               top, double          right, double bottom ) => new(left, top, right - left, bottom - top);
    [Pure] public static MutableRectangle Create( in ReadOnlyPoint    point,     in ReadOnlySize      size )        => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static MutableRectangle Create( in ReadOnlyPointF   point,     in ReadOnlySizeF     size )        => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static MutableRectangle Create( in ReadOnlyPoint    topLeft,   in ReadOnlyPoint     bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    [Pure] public static MutableRectangle Create( in ReadOnlyPointF   topLeft,   in ReadOnlyPointF    bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    [Pure] public static MutableRectangle Create( in MutableRectangle rectangle, in ReadOnlyThickness padding )     => rectangle + padding;


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
    public readonly string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<MutableRectangle>.ToString(this, format);


    [Pure] public readonly bool IsAtLeast( in       ReadOnlySize                other )  => other.Width <= Width && other.Height <= Height;
    [Pure] public readonly bool IsAtLeast( in       ReadOnlySizeF               other )  => other.Width <= Width && other.Height <= Height;
    [Pure] public readonly bool Contains( in        ReadOnlyPoint               other )  => other.X     >= X     && other.X      < Right && other.Y >= Y && other.Y < Bottom;
    [Pure] public readonly bool Contains( in        ReadOnlyPointF              other )  => other.X     >= X     && other.X      < Right && other.Y >= Y && other.Y < Bottom;
    public readonly        bool ContainsAny( params ReadOnlySpan<ReadOnlyPoint> others ) => ContainsAny(in this, others);
    public static bool ContainsAny<T>( ref readonly T rect, params ReadOnlySpan<ReadOnlyPoint> others )
        where T : IRectangle<T>
    {
        MutableRectangle rectangle = Create(in rect);

        foreach ( ref readonly ReadOnlyPoint point in others )
        {
            if ( rectangle.Contains(in point) ) { return true; }
        }

        return false;
    }
    public readonly bool ContainsAll( params ReadOnlySpan<ReadOnlyPoint> others ) => ContainsAll(in this, others);
    public static bool ContainsAll<T>( ref readonly T rect, params ReadOnlySpan<ReadOnlyPoint> others )
        where T : IRectangle<T>
    {
        MutableRectangle rectangle = Create(in rect);

        foreach ( ref readonly ReadOnlyPoint point in others )
        {
            if ( rectangle.Contains(in point) is false ) { return false; }
        }

        return true;
    }
    [Pure] public readonly bool Contains( in       MutableRectangle other ) => Left <= other.Left && Right >= other.Right && Top <= other.Top && Bottom >= other.Bottom;
    [Pure] public readonly bool IntersectsWith( in MutableRectangle other ) => ( Left >= other.Right || Right <= other.Left || Top >= other.Bottom || Bottom <= other.Top ) is false;

    public MutableRectangle Round()
    {
        X      = X.Round();
        Y      = Y.Round();
        Width  = Width.Round();
        Height = Height.Round();
        return this;
    }

    [Pure]
    public MutableRectangle Union( in MutableRectangle other )
    {
        double x      = Math.Min(X, other.X);
        double y      = Math.Min(Y, other.Y);
        double width  = Math.Max(Right,  other.Right)  - X;
        double height = Math.Max(Bottom, other.Bottom) - Y;

        return width < 0 || height < 0
                   ? Zero
                   : new MutableRectangle(x, y, width, height);
    }
    [Pure]
    public MutableRectangle Intersection( in MutableRectangle other )
    {
        double x      = Math.Max(X, other.X);
        double y      = Math.Max(Y, other.Y);
        double width  = Math.Min(Right,  other.Right)  - x;
        double height = Math.Min(Bottom, other.Bottom) - y;

        return width < 0 || height < 0
                   ? Zero
                   : new MutableRectangle(x, y, width, height);
    }

    [Pure]
    public readonly bool Contains( params ReadOnlySpan<ReadOnlyPoint> points )
    {
        foreach ( ref readonly ReadOnlyPoint point in points )
        {
            if ( Contains(in point) ) { return true; }
        }

        return false;
    }

    [Pure]
    public readonly bool Contains( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        foreach ( ref readonly ReadOnlyPointF point in points )
        {
            if ( Contains(in point) ) { return true; }
        }

        return false;
    }

    [Pure]
    public readonly bool DoesLineIntersect( in ReadOnlyPoint source, in ReadOnlyPoint target )
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
    public readonly bool DoesLineIntersect( in ReadOnlyPointF source, in ReadOnlyPointF target )
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


    public static        bool operator ==( MutableRectangle            left,      MutableRectangle                 right )  => Sorter.Equals(left, right);
    public static        bool operator !=( MutableRectangle            left,      MutableRectangle                 right )  => Sorter.DoesNotEqual(left, right);
    public static        bool operator >( MutableRectangle             left,      MutableRectangle                 right )  => Sorter.GreaterThan(left, right);
    public static        bool operator >=( MutableRectangle            left,      MutableRectangle                 right )  => Sorter.GreaterThanOrEqualTo(left, right);
    public static        bool operator <( MutableRectangle             left,      MutableRectangle                 right )  => Sorter.LessThan(left, right);
    public static        bool operator <=( MutableRectangle            left,      MutableRectangle                 right )  => Sorter.LessThanOrEqualTo(left, right);
    public static        MutableRectangle operator +( MutableRectangle rectangle, MutableRectangle                 other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, MutableRectangle                 other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, MutableRectangle                 other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, MutableRectangle                 other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, ReadOnlyRectangle                other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, ReadOnlyPoint                    other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, ReadOnlyPoint                    other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, ReadOnlyPoint                    other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, ReadOnlyPoint                    other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, ReadOnlyPointF                   other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, ReadOnlyPointF                   other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, ReadOnlyPointF                   other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, ReadOnlyPointF                   other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, PointF                           other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, PointF                           other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, PointF                           other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, PointF                           other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, Point                            other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, Point                            other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, Point                            other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, Point                            other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, ReadOnlyPointF                   other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, ReadOnlyPoint                    other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, PointF                           other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, Point                            other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, Size                             other )  => new(rectangle.X, rectangle.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle rectangle, SizeF                            other )  => new(rectangle.X, rectangle.Y, other.Width, other.Height);
    [Pure] public static MutableRectangle operator +( MutableRectangle rectangle, ReadOnlyThickness                margin ) => new(rectangle.X                               - margin.Left, rectangle.Y - margin.Top, rectangle.Width + margin.Right, rectangle.Height + margin.Bottom);
    [Pure] public static MutableRectangle operator -( MutableRectangle rectangle, ReadOnlyThickness                margin ) => new(rectangle.X                               + margin.Left, rectangle.Y + margin.Top, rectangle.Width - margin.Right, rectangle.Height - margin.Bottom);
    public static        MutableRectangle operator +( MutableRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height  + value);
    public static        MutableRectangle operator +( MutableRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height  + value);
    public static        MutableRectangle operator +( MutableRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height  + value);
    public static        MutableRectangle operator -( MutableRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height  - value);
    public static        MutableRectangle operator -( MutableRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height  - value);
    public static        MutableRectangle operator -( MutableRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height  - value);
    public static        MutableRectangle operator *( MutableRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        MutableRectangle operator *( MutableRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        MutableRectangle operator *( MutableRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        MutableRectangle operator /( MutableRectangle rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        MutableRectangle operator /( MutableRectangle rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        MutableRectangle operator /( MutableRectangle rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        MutableRectangle operator +( MutableRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator +( MutableRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator -( MutableRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator *( MutableRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        MutableRectangle operator /( MutableRectangle rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
}
