namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public struct MutableRectangle( double x, double y, double width, double height ) : IMutableRectangle<MutableRectangle>
{
    public static readonly MutableRectangle Invalid = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly MutableRectangle Zero    = new(0, 0, 0, 0);
    public static readonly MutableRectangle One     = 1;


    public static                EqualComparer<MutableRectangle>           Sorter  => EqualComparer<MutableRectangle>.Default;
    static ref readonly          MutableRectangle IShape<MutableRectangle>.Zero    => ref Zero;
    static ref readonly          MutableRectangle IShape<MutableRectangle>.Invalid => ref Invalid;
    static ref readonly          MutableRectangle IShape<MutableRectangle>.One     => ref One;
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


    public static implicit operator Rectangle( MutableRectangle          self )  => new((int)self.X, (int)self.Y, (int)self.Width, (int)self.Height);
    public static implicit operator RectangleF( MutableRectangle         self )  => new(self.X.AsFloat(), self.Y.AsFloat(), self.Width.AsFloat(), self.Height.AsFloat());
    public static implicit operator ReadOnlyRectangleF( MutableRectangle self )  => new(self.X.AsFloat(), self.Y.AsFloat(), self.Width.AsFloat(), self.Height.AsFloat());
    public static implicit operator ReadOnlyRectangle( MutableRectangle  rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( Rectangle          rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( RectangleF         rect )  => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlySize       rect )  => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlySizeF      rect )  => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( MutableSize        rect )  => new(0, 0, rect.Width, rect.Height);
    public static implicit operator MutableRectangle( ReadOnlyPoint      rect )  => new(rect.X, rect.Y, 0, 0);
    public static implicit operator MutableRectangle( ReadOnlyPointF     rect )  => new(rect.X, rect.Y, 0, 0);
    public static implicit operator MutableRectangle( int                value ) => new(value, value, value, value);
    public static implicit operator MutableRectangle( long               value ) => new(value, value, value, value);
    public static implicit operator MutableRectangle( float              value ) => new(value, value, value, value);
    public static implicit operator MutableRectangle( double             value ) => new(value, value, value, value);


    [Pure]
    public static MutableRectangle Create( params ReadOnlySpan<ReadOnlyPoint> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableRectangle self = Create(points[0].X, points[0].Y, points[0].X, points[0].Y);

        foreach ( ReadOnlyPoint point in points )
        {
            if ( point.X      < self.Left ) { self.X      = point.X; }
            else if ( point.X > self.Right ) { self.Width = point.X - self.Left; }

            if ( point.Y      < self.Top ) { self.Y         = point.Y; }
            else if ( point.Y > self.Bottom ) { self.Height = point.Y - self.Top; }
        }

        return self;
    }
    [Pure]
    public static MutableRectangle Create( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableRectangle self = Create(points[0].X, points[0].Y, points[0].X, points[0].Y);

        foreach ( ReadOnlyPointF point in points )
        {
            if ( point.X      < self.Left ) { self.X      = point.X; }
            else if ( point.X > self.Right ) { self.Width = point.X - self.Left; }

            if ( point.Y      < self.Top ) { self.Y         = point.Y; }
            else if ( point.Y > self.Bottom ) { self.Height = point.Y - self.Top; }
        }

        return self;
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


    public static        bool operator ==( MutableRectangle            left, MutableRectangle                 right )  => Sorter.Equals(left, right);
    public static        bool operator !=( MutableRectangle            left, MutableRectangle                 right )  => Sorter.DoesNotEqual(left, right);
    public static        bool operator >( MutableRectangle             left, MutableRectangle                 right )  => Sorter.GreaterThan(left, right);
    public static        bool operator >=( MutableRectangle            left, MutableRectangle                 right )  => Sorter.GreaterThanOrEqualTo(left, right);
    public static        bool operator <( MutableRectangle             left, MutableRectangle                 right )  => Sorter.LessThan(left, right);
    public static        bool operator <=( MutableRectangle            left, MutableRectangle                 right )  => Sorter.LessThanOrEqualTo(left, right);
    public static        MutableRectangle operator +( MutableRectangle self, MutableRectangle                 other )  => new(self.X + other.X, self.Y + other.Y, self.Width + other.Width, self.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle self, MutableRectangle                 other )  => new(self.X - other.X, self.Y - other.Y, self.Width - other.Width, self.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle self, MutableRectangle                 other )  => new(self.X * other.X, self.Y * other.Y, self.Width * other.Width, self.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle self, MutableRectangle                 other )  => new(self.X / other.X, self.Y / other.Y, self.Width / other.Width, self.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle self, ReadOnlyRectangle                other )  => new(self.X + other.X, self.Y + other.Y, self.Width + other.Width, self.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle self, ReadOnlyRectangle                other )  => new(self.X - other.X, self.Y - other.Y, self.Width - other.Width, self.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle self, ReadOnlyRectangle                other )  => new(self.X * other.X, self.Y * other.Y, self.Width * other.Width, self.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle self, ReadOnlyRectangle                other )  => new(self.X / other.X, self.Y / other.Y, self.Width / other.Width, self.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle self, ReadOnlyRectangleF               other )  => new(self.X + other.X, self.Y + other.Y, self.Width + other.Width, self.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle self, ReadOnlyRectangleF               other )  => new(self.X - other.X, self.Y - other.Y, self.Width - other.Width, self.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle self, ReadOnlyRectangleF               other )  => new(self.X * other.X, self.Y * other.Y, self.Width * other.Width, self.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle self, ReadOnlyRectangleF               other )  => new(self.X / other.X, self.Y / other.Y, self.Width / other.Width, self.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle self, ReadOnlySize                     other )  => new(self.X, self.Y, self.Width + other.Width, self.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle self, ReadOnlySize                     other )  => new(self.X, self.Y, self.Width - other.Width, self.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle self, ReadOnlySize                     other )  => new(self.X, self.Y, self.Width * other.Width, self.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle self, ReadOnlySize                     other )  => new(self.X, self.Y, self.Width / other.Width, self.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle self, ReadOnlySizeF                    other )  => new(self.X, self.Y, self.Width + other.Width, self.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle self, ReadOnlySizeF                    other )  => new(self.X, self.Y, self.Width - other.Width, self.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle self, ReadOnlySizeF                    other )  => new(self.X, self.Y, self.Width * other.Width, self.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle self, ReadOnlySizeF                    other )  => new(self.X, self.Y, self.Width / other.Width, self.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle self, MutableSize                      other )  => new(self.X, self.Y, self.Width + other.Width, self.Height + other.Height);
    public static        MutableRectangle operator -( MutableRectangle self, MutableSize                      other )  => new(self.X, self.Y, self.Width - other.Width, self.Height - other.Height);
    public static        MutableRectangle operator *( MutableRectangle self, MutableSize                      other )  => new(self.X, self.Y, self.Width * other.Width, self.Height * other.Height);
    public static        MutableRectangle operator /( MutableRectangle self, MutableSize                      other )  => new(self.X, self.Y, self.Width / other.Width, self.Height / other.Height);
    public static        MutableRectangle operator +( MutableRectangle self, ReadOnlyPoint                    other )  => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, ReadOnlyPoint                    other )  => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, ReadOnlyPoint                    other )  => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, ReadOnlyPoint                    other )  => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static        MutableRectangle operator +( MutableRectangle self, ReadOnlyPointF                   other )  => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, ReadOnlyPointF                   other )  => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, ReadOnlyPointF                   other )  => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, ReadOnlyPointF                   other )  => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static        MutableRectangle operator +( MutableRectangle self, PointF                           other )  => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, PointF                           other )  => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, PointF                           other )  => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, PointF                           other )  => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static        MutableRectangle operator +( MutableRectangle self, Point                            other )  => new(self.X + other.X, self.Y + other.Y, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, Point                            other )  => new(self.X - other.X, self.Y - other.Y, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, Point                            other )  => new(self.X * other.X, self.Y * other.Y, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, Point                            other )  => new(self.X / other.X, self.Y / other.Y, self.Width, self.Height);
    public static        MutableRectangle operator &( MutableRectangle self, ReadOnlyPointF                   other )  => new(other.X, other.Y, self.Width, self.Height);
    public static        MutableRectangle operator &( MutableRectangle self, ReadOnlyPoint                    other )  => new(other.X, other.Y, self.Width, self.Height);
    public static        MutableRectangle operator &( MutableRectangle self, PointF                           other )  => new(other.X, other.Y, self.Width, self.Height);
    public static        MutableRectangle operator &( MutableRectangle self, Point                            other )  => new(other.X, other.Y, self.Width, self.Height);
    public static        MutableRectangle operator &( MutableRectangle self, ReadOnlySize                     other )  => new(self.X, self.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle self, ReadOnlySizeF                    other )  => new(self.X, self.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle self, Size                             other )  => new(self.X, self.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle self, SizeF                            other )  => new(self.X, self.Y, other.Width, other.Height);
    public static        MutableRectangle operator &( MutableRectangle self, MutableSize                      other )  => new(self.X, self.Y, other.Width, other.Height);
    [Pure] public static MutableRectangle operator +( MutableRectangle self, ReadOnlyThickness                margin ) => new(self.X                     - margin.Left, self.Y - margin.Top, self.Width + margin.Right, self.Height + margin.Bottom);
    [Pure] public static MutableRectangle operator -( MutableRectangle self, ReadOnlyThickness                margin ) => new(self.X                     + margin.Left, self.Y + margin.Top, self.Width - margin.Right, self.Height - margin.Bottom);
    public static        MutableRectangle operator +( MutableRectangle self, int                              value )  => new(self.X, self.Y, self.Width + value, self.Height  + value);
    public static        MutableRectangle operator +( MutableRectangle self, float                            value )  => new(self.X, self.Y, self.Width + value, self.Height  + value);
    public static        MutableRectangle operator +( MutableRectangle self, double                           value )  => new(self.X, self.Y, self.Width + value, self.Height  + value);
    public static        MutableRectangle operator -( MutableRectangle self, int                              value )  => new(self.X, self.Y, self.Width - value, self.Height  - value);
    public static        MutableRectangle operator -( MutableRectangle self, float                            value )  => new(self.X, self.Y, self.Width - value, self.Height  - value);
    public static        MutableRectangle operator -( MutableRectangle self, double                           value )  => new(self.X, self.Y, self.Width - value, self.Height  - value);
    public static        MutableRectangle operator *( MutableRectangle self, int                              value )  => new(self.X, self.Y, self.Width * value, self.Height * value);
    public static        MutableRectangle operator *( MutableRectangle self, float                            value )  => new(self.X, self.Y, self.Width * value, self.Height * value);
    public static        MutableRectangle operator *( MutableRectangle self, double                           value )  => new(self.X, self.Y, self.Width * value, self.Height * value);
    public static        MutableRectangle operator /( MutableRectangle self, int                              value )  => new(self.X, self.Y, self.Width / value, self.Height / value);
    public static        MutableRectangle operator /( MutableRectangle self, float                            value )  => new(self.X, self.Y, self.Width / value, self.Height / value);
    public static        MutableRectangle operator /( MutableRectangle self, double                           value )  => new(self.X, self.Y, self.Width / value, self.Height / value);
    public static        MutableRectangle operator +( MutableRectangle self, (int xOffset, int yOffset)       value )  => new(self.X + value.xOffset, self.Y + value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator +( MutableRectangle self, (float xOffset, float yOffset)   value )  => new(self.X + value.xOffset, self.Y + value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator +( MutableRectangle self, (double xOffset, double yOffset) value )  => new(self.X + value.xOffset, self.Y + value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, (int xOffset, int yOffset)       value )  => new(self.X - value.xOffset, self.Y - value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, (float xOffset, float yOffset)   value )  => new(self.X - value.xOffset, self.Y - value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator -( MutableRectangle self, (double xOffset, double yOffset) value )  => new(self.X - value.xOffset, self.Y - value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, (int xOffset, int yOffset)       value )  => new(self.X * value.xOffset, self.Y * value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, (float xOffset, float yOffset)   value )  => new(self.X * value.xOffset, self.Y * value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator *( MutableRectangle self, (double xOffset, double yOffset) value )  => new(self.X * value.xOffset, self.Y * value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, (int xOffset, int yOffset)       value )  => new(self.X / value.xOffset, self.Y / value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, (float xOffset, float yOffset)   value )  => new(self.X / value.xOffset, self.Y / value.yOffset, self.Width, self.Height);
    public static        MutableRectangle operator /( MutableRectangle self, (double xOffset, double yOffset) value )  => new(self.X / value.xOffset, self.Y / value.yOffset, self.Width, self.Height);
}
