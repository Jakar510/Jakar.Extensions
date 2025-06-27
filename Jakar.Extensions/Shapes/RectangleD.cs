namespace Jakar.Extensions;


[DefaultValue( nameof(Zero) )]
public struct RectangleD( double x, double y, double width, double height ) : IRectangle<RectangleD, ReadOnlySize, ReadOnlyPoint, ReadOnlyThickness, double>, IMathOperators<RectangleD>, IMathOperators<RectangleD, ReadOnlyPointF>, IMathOperators<RectangleD, ReadOnlySize>, IMathOperators<RectangleD, ReadOnlySizeF>, IMathOperators<RectangleD, ReadOnlyRectangleF>, IMathOperators<RectangleD, ReadOnlyRectangle>
{
    public static readonly RectangleD Invalid = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly RectangleD Zero    = new(0, 0, 0, 0);


    public static       Sorter<RectangleD>                   Sorter  => Sorter<RectangleD>.Default;
    static              RectangleD IGenericShape<RectangleD>.Zero    => Zero;
    static              RectangleD IGenericShape<RectangleD>.Invalid => Invalid;
    public              double                               X       { get; set; } = x;
    public              double                               Y       { get; set; } = y;
    public              double                               Width   { get; set; } = width;
    public              double                               Height  { get; set; } = height;
    public              bool                                 IsNaN   => double.IsNaN( X ) || double.IsNaN( Y ) || double.IsNaN( Width ) || double.IsNaN( Height );
    public              bool                                 IsValid => IsNaN is false && X >= 0 && Y >= 0 && Width >= 0 && Height >= 0;
    [JsonIgnore] public double                               Bottom  => Y + Height;
    [JsonIgnore] public ReadOnlyPoint                        Center  => new((X + Width) / 2, (Y + Height) / 2);
    [JsonIgnore] public bool                                 IsEmpty => CheckIfEmpty( in this );
    [JsonIgnore] public double                               Left    => X;
    public ReadOnlyPoint Location
    {
        readonly get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
    [JsonIgnore] public double Right => X + Width;
    public ReadOnlySize Size
    {
        readonly get => new(Width, Height);
        set
        {
            Width  = value.Width;
            Height = value.Height;
        }
    }
    [JsonIgnore] public double Top => Y;


    public static bool CheckIfEmpty( scoped in RectangleD rectangle ) => double.IsNaN( rectangle.X ) || double.IsNaN( rectangle.Y ) || double.IsNaN( rectangle.Width ) || rectangle.Width <= 0 || double.IsNaN( rectangle.Height ) || rectangle.Height <= 0;


    [Pure]
    public static RectangleD Create( params ReadOnlySpan<ReadOnlyPoint> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        RectangleD rectangle = Create( points[0].X, points[0].Y, points[0].X, points[0].Y );

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
    public static RectangleD Create( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        RectangleD rectangle = Create( points[0].X, points[0].Y, points[0].X, points[0].Y );

        foreach ( ReadOnlyPointF point in points )
        {
            if ( point.X      < rectangle.Left ) { rectangle.X      = point.X; }
            else if ( point.X > rectangle.Right ) { rectangle.Width = point.X - rectangle.Left; }

            if ( point.Y      < rectangle.Top ) { rectangle.Y         = point.Y; }
            else if ( point.Y > rectangle.Bottom ) { rectangle.Height = point.Y - rectangle.Top; }
        }

        return rectangle;
    }
    public static        RectangleD Create( double            x,         double               y,   in ReadOnlySize size )                 => default;
    [Pure] public static RectangleD Create( double            left,      double               top, double          right, double bottom ) => new(left, top, right - left, bottom - top);
    [Pure] public static RectangleD Create( in ReadOnlyPoint  point,     in ReadOnlySize      size )        => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static RectangleD Create( in ReadOnlyPointF point,     in ReadOnlySizeF     size )        => new(point.X, point.Y, size.Width, size.Height);
    public static        RectangleD Create( in ReadOnlyPoint  topLeft,   in ReadOnlyPoint     bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X        - topLeft.X, bottomRight.Y                      - topLeft.Y);
    public static        RectangleD Create( in ReadOnlyPointF topLeft,   in ReadOnlyPointF    bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X        - topLeft.X, bottomRight.Y                      - topLeft.Y);
    public static        RectangleD Create( in RectangleD     rectangle, in ReadOnlyThickness padding )     => new(padding.Left, padding.Top, rectangle.Width - padding.HorizontalThickness, rectangle.Height - padding.VerticalThickness);


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is RectangleD x
                   ? CompareTo( x )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(RectangleD) );
    }
    public int CompareTo( RectangleD other )
    {
        int xComparison = X.CompareTo( other.X );
        if ( xComparison != 0 ) { return xComparison; }

        int yComparison = Y.CompareTo( other.Y );
        if ( yComparison != 0 ) { return yComparison; }

        int widthComparison = Width.CompareTo( other.Width );
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo( other.Height );
    }
    public          bool   Equals( RectangleD other )                                  => X.Equals( other.X )   && Y.Equals( other.Y ) && Width.Equals( other.Width ) && Height.Equals( other.Height );
    public override bool   Equals( object?    other )                                  => other is RectangleD x && Equals( x );
    public override int    GetHashCode()                                               => HashCode.Combine( X, Y, Width, Height );
    public override string ToString()                                                  => ToString( null, null );
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<RectangleD, ReadOnlySize, ReadOnlyPoint, ReadOnlyThickness, double>.ToString( this, format );


    public static implicit operator Rectangle( RectangleD  rectangle ) => new((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
    public static implicit operator RectangleF( RectangleD rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator RectangleD( Rectangle  rect )      => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator RectangleD( RectangleF rect )      => new(rect.X, rect.Y, rect.Width, rect.Height);


    [Pure] public bool       IsAtLeast( in      ReadOnlySize   other ) => other.Width <= Width      && other.Height <= Height;
    [Pure] public bool       IsAtLeast( in      ReadOnlySizeF  other ) => other.Width <= Width      && other.Height <= Height;
    [Pure] public bool       Contains( in       ReadOnlyPoint  other ) => other.X     >= X          && other.X      < Right        && other.Y >= Y         && other.Y < Bottom;
    [Pure] public bool       Contains( in       ReadOnlyPointF other ) => other.X     >= X          && other.X      < Right        && other.Y >= Y         && other.Y < Bottom;
    [Pure] public bool       Contains( in       RectangleD     other ) => Left        <= other.Left && Right        >= other.Right && Top     <= other.Top && Bottom  >= other.Bottom;
    [Pure] public bool       IntersectsWith( in RectangleD     other ) => (Left >= other.Right || Right <= other.Left || Top >= other.Bottom || Bottom <= other.Top) is false;
    [Pure] public RectangleD Union( in          RectangleD     other ) => new(Math.Min( X, other.X ), Math.Min( Y, other.Y ), Math.Max( Right, other.Right ), Math.Max( Bottom, other.Bottom ));
    [Pure] public RectangleD Round()                                   => new(X.Round(), Y.Round(), Width.Round(), Height.Round());


    [Pure]
    public RectangleD Intersection( in RectangleD other )
    {
        double x      = Math.Max( X, other.X );
        double y      = Math.Max( Y, other.Y );
        double width  = Math.Min( Right,  other.Right )  - x;
        double height = Math.Min( Bottom, other.Bottom ) - y;

        return width < 0 || height < 0
                   ? Zero
                   : new RectangleD( x, y, width, height );
    }

    [Pure]
    public bool Contains( params ReadOnlySpan<ReadOnlyPoint> points )
    {
        foreach ( ReadOnlyPoint point in points )
        {
            if ( Contains( in point ) ) { return true; }
        }

        return false;
    }

    [Pure]
    public bool Contains( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        foreach ( ReadOnlyPointF point in points )
        {
            if ( Contains( in point ) ) { return true; }
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

                if ( pX < 0 ) { t0 = Math.Max( t0, rX ); }
                else { t1          = Math.Min( t1, rX ); }

                if ( pY < 0 ) { t0 = Math.Max( t0, rY ); }
                else { t1          = Math.Min( t1, rY ); }

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

                if ( pX < 0 ) { t0 = Math.Max( t0, rX ); }
                else { t1          = Math.Min( t1, rX ); }

                if ( pY < 0 ) { t0 = Math.Max( t0, rY ); }
                else { t1          = Math.Min( t1, rY ); }

                if ( t0 > t1 ) { return false; }
            }
        }

        return true;
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


    public static        bool operator ==( RectangleD      left,      RectangleD                       right )  => Sorter.Equals( left, right );
    public static        bool operator !=( RectangleD      left,      RectangleD                       right )  => Sorter.DoesNotEqual( left, right );
    public static        bool operator >( RectangleD       left,      RectangleD                       right )  => Sorter.GreaterThan( left, right );
    public static        bool operator >=( RectangleD      left,      RectangleD                       right )  => Sorter.GreaterThanOrEqualTo( left, right );
    public static        bool operator <( RectangleD       left,      RectangleD                       right )  => Sorter.LessThan( left, right );
    public static        bool operator <=( RectangleD      left,      RectangleD                       right )  => Sorter.LessThanOrEqualTo( left, right );
    [Pure] public static RectangleD operator +( RectangleD rectangle, ReadOnlyThickness                margin ) => new(rectangle.X - margin.Left, rectangle.Y - margin.Top, rectangle.Width + margin.Right, rectangle.Height + margin.Bottom);
    [Pure] public static RectangleD operator -( RectangleD rectangle, ReadOnlyThickness                margin ) => new(rectangle.X + margin.Left, rectangle.Y + margin.Top, rectangle.Width - margin.Right, rectangle.Height - margin.Bottom);
    public static        RectangleD operator +( RectangleD rectangle, RectangleD                       other )  => new(rectangle.X + other.X, rectangle.Y     + other.Y, rectangle.Width    + other.Width, rectangle.Height  + other.Height);
    public static        RectangleD operator -( RectangleD rectangle, RectangleD                       other )  => new(rectangle.X - other.X, rectangle.Y     - other.Y, rectangle.Width    - other.Width, rectangle.Height  - other.Height);
    public static        RectangleD operator *( RectangleD rectangle, RectangleD                       other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        RectangleD operator /( RectangleD rectangle, RectangleD                       other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        RectangleD operator +( RectangleD rectangle, ReadOnlyRectangle                other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        RectangleD operator -( RectangleD rectangle, ReadOnlyRectangle                other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        RectangleD operator *( RectangleD rectangle, ReadOnlyRectangle                other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        RectangleD operator /( RectangleD rectangle, ReadOnlyRectangle                other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        RectangleD operator +( RectangleD rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        RectangleD operator -( RectangleD rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        RectangleD operator *( RectangleD rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        RectangleD operator /( RectangleD rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        RectangleD operator +( RectangleD rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        RectangleD operator -( RectangleD rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        RectangleD operator *( RectangleD rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        RectangleD operator /( RectangleD rectangle, ReadOnlySize                     other )  => new(rectangle.X, rectangle.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        RectangleD operator +( RectangleD rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        RectangleD operator -( RectangleD rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        RectangleD operator *( RectangleD rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        RectangleD operator /( RectangleD rectangle, ReadOnlySizeF                    other )  => new(rectangle.X, rectangle.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        RectangleD operator &( RectangleD rectangle, ReadOnlyPointF                   other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator +( RectangleD rectangle, ReadOnlyPointF                   other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator -( RectangleD rectangle, ReadOnlyPointF                   other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator *( RectangleD rectangle, ReadOnlyPointF                   other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator /( RectangleD rectangle, ReadOnlyPointF                   other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator &( RectangleD rectangle, ReadOnlyPoint                    other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator +( RectangleD rectangle, ReadOnlyPoint                    other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator -( RectangleD rectangle, ReadOnlyPoint                    other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator *( RectangleD rectangle, ReadOnlyPoint                    other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator /( RectangleD rectangle, ReadOnlyPoint                    other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width, rectangle.Height);
    public static        RectangleD operator +( RectangleD rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        RectangleD operator +( RectangleD rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        RectangleD operator +( RectangleD rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        RectangleD operator -( RectangleD rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        RectangleD operator -( RectangleD rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        RectangleD operator -( RectangleD rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        RectangleD operator *( RectangleD rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        RectangleD operator *( RectangleD rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        RectangleD operator *( RectangleD rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        RectangleD operator /( RectangleD rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        RectangleD operator /( RectangleD rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        RectangleD operator /( RectangleD rectangle, double                           value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        RectangleD operator +( RectangleD rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator +( RectangleD rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator +( RectangleD rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator -( RectangleD rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator -( RectangleD rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator -( RectangleD rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator *( RectangleD rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator *( RectangleD rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator *( RectangleD rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator /( RectangleD rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator /( RectangleD rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        RectangleD operator /( RectangleD rectangle, (double xOffset, double yOffset) value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
}
