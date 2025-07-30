// Jakar.Extensions :: Jakar.Extensions
// 07/11/2025  18:56

namespace Jakar.Shapes;


// [Experimental("Jakar_Shapes_Circle")]
[DefaultValue(nameof(Zero))]
public readonly struct Circle( ReadOnlyPoint center, double radius ) : ICircle<Circle>, IMathOperators<Circle>
{
    public static readonly Circle        Invalid = new(double.NaN, double.NaN);
    public static readonly Circle        Zero    = 0;
    public static readonly Circle        One     = 1;
    public readonly        ReadOnlyPoint Center  = center;
    public readonly        double        Radius  = radius;


    public static       EqualComparer<Circle>        Sorter  => EqualComparer<Circle>.Default;
    static ref readonly Circle IShape<Circle>.Zero    => ref Zero;
    static ref readonly Circle IShape<Circle>.One     => ref One;
    static ref readonly Circle IShape<Circle>.Invalid => ref Invalid;
    double ICircle<Circle>.                   Radius  => Radius;
    ReadOnlyPoint ICircle<Circle>.            Center  => Center;
    public bool                               IsEmpty => Center.IsEmpty || double.IsNaN(Radius);
    public bool                               IsNaN   => Center.IsNaN   || double.IsNaN(Radius);
    public bool                               IsValid => IsNaN is false && Radius >= 0;
    double IShapeLocation.                    X       => Center.X;
    double IShapeLocation.                    Y       => Center.Y;


    public static implicit operator Circle( ReadOnlyPointF point ) => new(point.X, point.Y);
    public static implicit operator Circle( Point          point ) => new(point.X, point.Y);
    public static implicit operator Circle( PointF         point ) => new(point.X, point.Y);
    public static implicit operator Circle( int            other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Circle( long           other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Circle( float          other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Circle( double         other ) => new(ReadOnlyPoint.Zero, other);


    [Pure] public static Circle Create( float  x, float  y ) => new(x, y);
    [Pure] public static Circle Create( double x, double y ) => new(x, y);
    [Pure] public        Circle Round() => new(Center, Radius.Round());
    [Pure] public        Circle Floor() => new(Center, Radius.Floor());


    [Pure, MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public double DistanceTo( in Circle other )
    {
        double x      = Center.X - other.Center.X;
        double y      = Center.Y - other.Center.Y;
        double x2     = x * x;
        double y2     = y * y;
        double result = Math.Sqrt(x2 + y2);
        return result;
    }


    public int CompareTo( Circle other )
    {
        int xOffsetComparison = Center.CompareTo(other.Center);

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : Radius.CompareTo(other.Radius);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is Circle other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(Circle)}");
    }
    public          bool   Equals( Circle  other )                                     => Center.Equals(other.Center) && Radius.Equals(other.Radius);
    public override bool   Equals( object? other )                                     => other is Circle x           && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(Center, Radius);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => ICircle<Circle>.ToString(in this, format);


    public static bool operator ==( Circle  left, Circle                           right ) => Sorter.Equals(left, right);
    public static bool operator !=( Circle  left, Circle                           right ) => Sorter.DoesNotEqual(left, right);
    public static bool operator >( Circle   left, Circle                           right ) => Sorter.GreaterThan(left, right);
    public static bool operator >=( Circle  left, Circle                           right ) => Sorter.GreaterThanOrEqualTo(left, right);
    public static bool operator <( Circle   left, Circle                           right ) => Sorter.LessThan(left, right);
    public static bool operator <=( Circle  left, Circle                           right ) => Sorter.LessThanOrEqualTo(left, right);
    public static Circle operator *( Circle self, Circle                           other ) => new(self.Center * other.Center, self.Radius * other.Radius);
    public static Circle operator +( Circle self, Circle                           other ) => new(self.Center + other.Center, self.Radius + other.Radius);
    public static Circle operator -( Circle self, Circle                           other ) => new(self.Center - other.Center, self.Radius - other.Radius);
    public static Circle operator /( Circle self, Circle                           other ) => new(self.Center / other.Center, self.Radius / other.Radius);
    public static Circle operator -( Circle self, ReadOnlyPoint                    other ) => new(self.Center - other, self.Radius);
    public static Circle operator -( Circle self, ReadOnlyPointF                   other ) => new(self.Center - other, self.Radius);
    public static Circle operator +( Circle self, ReadOnlyPoint                    other ) => new(self.Center + other, self.Radius);
    public static Circle operator +( Circle self, ReadOnlyPointF                   other ) => new(self.Center + other, self.Radius);
    public static Circle operator &( Circle self, ReadOnlyPoint                    other ) => new(other, self.Radius);
    public static Circle operator &( Circle self, ReadOnlyPointF                   other ) => new(other, self.Radius);
    public static Circle operator +( Circle self, (int xOffset, int yOffset)       other ) => new(self.Center + other, self.Radius);
    public static Circle operator +( Circle self, (float xOffset, float yOffset)   other ) => new(self.Center + other, self.Radius);
    public static Circle operator +( Circle self, (double xOffset, double yOffset) other ) => new(self.Center + other, self.Radius);
    public static Circle operator -( Circle self, (int xOffset, int yOffset)       other ) => new(self.Center - other, self.Radius);
    public static Circle operator -( Circle self, (float xOffset, float yOffset)   other ) => new(self.Center - other, self.Radius);
    public static Circle operator -( Circle self, (double xOffset, double yOffset) other ) => new(self.Center - other, self.Radius);
    public static Circle operator *( Circle self, (int xOffset, int yOffset)       other ) => new(self.Center              * other, self.Radius);
    public static Circle operator *( Circle self, (float xOffset, float yOffset)   other ) => new(self.Center              * other, self.Radius);
    public static Circle operator *( Circle self, (double xOffset, double yOffset) other ) => new(self.Center              * other, self.Radius);
    public static Circle operator /( Circle self, (int xOffset, int yOffset)       other ) => new(self.Center              / other, self.Radius);
    public static Circle operator /( Circle self, (float xOffset, float yOffset)   other ) => new(self.Center              / other, self.Radius);
    public static Circle operator /( Circle self, (double xOffset, double yOffset) other ) => new(self.Center              / other, self.Radius);
    public static Circle operator *( Circle self, int                              other ) => new(self.Center, self.Radius * other);
    public static Circle operator *( Circle self, float                            other ) => new(self.Center, self.Radius * other);
    public static Circle operator *( Circle self, double                           other ) => new(self.Center, self.Radius * other);
    public static Circle operator /( Circle self, int                              other ) => new(self.Center, self.Radius / other);
    public static Circle operator /( Circle self, float                            other ) => new(self.Center, self.Radius / other);
    public static Circle operator /( Circle self, double                           other ) => new(self.Center, self.Radius / other);
    public static Circle operator +( Circle self, int                              other ) => new(self.Center, self.Radius + other);
    public static Circle operator +( Circle self, float                            other ) => new(self.Center, self.Radius + other);
    public static Circle operator +( Circle self, double                           other ) => new(self.Center, self.Radius + other);
    public static Circle operator -( Circle self, int                              other ) => new(self.Center, self.Radius - other);
    public static Circle operator -( Circle self, float                            other ) => new(self.Center, self.Radius - other);
    public static Circle operator -( Circle self, double                           other ) => new(self.Center, self.Radius - other);
}
