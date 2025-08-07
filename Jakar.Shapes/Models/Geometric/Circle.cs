// Jakar.Extensions :: Jakar.Extensions
// 07/11/2025  18:56

using System.Security.Cryptography;
using JetBrains.Annotations;



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


    public static       EqualComparer<Circle> Sorter  => EqualComparer<Circle>.Default;
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


    [System.Diagnostics.Contracts.Pure] public static Circle Create( float  x, float  y ) => new(x, y);
    [System.Diagnostics.Contracts.Pure] public static Circle Create( double x, double y ) => new(x, y);
    [System.Diagnostics.Contracts.Pure] public        Circle Round() => new(Center, Radius.Round());
    [System.Diagnostics.Contracts.Pure] public        Circle Floor() => new(Center, Radius.Floor());


    public bool IsTangent( ref readonly  ReadOnlyLine line ) => GetLineRelation(in line) is CircleLineRelation.Tangent;
    public bool IsSecant( ref readonly   ReadOnlyLine line ) => GetLineRelation(in line) is CircleLineRelation.Secant;
    public bool IsDisjoint( ref readonly ReadOnlyLine line ) => GetLineRelation(in line) is CircleLineRelation.Disjoint;


    public bool IsTangent( ref readonly  CalculatedLine line, in double xMin, in double xMax, in int samples = 1000, in double tolerance = 1e-8 ) => GetLineRelation(in line, in xMin, in xMax, in samples, in tolerance) is CircleLineRelation.Tangent;
    public bool IsSecant( ref readonly   CalculatedLine line, in double xMin, in double xMax, in int samples = 1000, in double tolerance = 1e-8 ) => GetLineRelation(in line, in xMin, in xMax, in samples, in tolerance) is CircleLineRelation.Secant;
    public bool IsDisjoint( ref readonly CalculatedLine line, in double xMin, in double xMax, in int samples = 1000, in double tolerance = 1e-8 ) => GetLineRelation(in line, in xMin, in xMax, in samples, in tolerance) is CircleLineRelation.Disjoint;


    public CircleLineRelation GetLineRelation( ref readonly ReadOnlyLine line )
    {
        double dx           = line.End.X   - line.Start.X;
        double dy           = line.End.Y   - line.Start.Y;
        double fx           = line.Start.X - Center.X;
        double fy           = line.Start.Y - Center.Y;
        double a            = dx * dx      + dy * dy;
        double b            = 2 * ( fx * dx + fy * dy );
        double c            = fx * fx + fy * fy - Radius * Radius;
        double discriminant = b                          * b - 4 * a * c;
        if ( discriminant < 0 ) { return CircleLineRelation.Disjoint; }

        if ( line.IsFinite is false )
        {
            return discriminant == 0
                       ? CircleLineRelation.Tangent
                       : CircleLineRelation.Secant;
        }

        // Finite segment: check if intersection lies on segment
        discriminant = Math.Sqrt(discriminant);
        double t1 = ( -b - discriminant ) / ( 2 * a );
        double t2 = ( -b + discriminant ) / ( 2 * a );

        bool onSegment1 = t1 is >= 0 and <= 1;
        bool onSegment2 = t2 is >= 0 and <= 1;

        if ( onSegment1 && onSegment2 ) { return CircleLineRelation.Secant; }

        if ( onSegment1 || onSegment2 )
        {
            return discriminant == 0
                       ? CircleLineRelation.Tangent
                       : CircleLineRelation.Secant;
        }

        return CircleLineRelation.Disjoint;
    }
    public CircleLineRelation GetLineRelation( ref readonly CalculatedLine curve, in double xMin, in double xMax, in int samples = 1000, in double tolerance = 1e-8 )
    {
        double r2          = Radius          * Radius;
        double range       = ( xMax - xMin ) / samples;
        bool   foundOn     = false;
        bool   foundInside = false;

        for ( int i = 0; i <= samples; i++ )
        {
            double x = xMin + range * i;
            double y = curve[x];

            if ( double.IsNaN(y) || double.IsInfinity(y) ) { continue; }

            double dx    = x       - Center.X;
            double dy    = y       - Center.Y;
            double dist2 = dx * dx + dy * dy;
            double diff  = dist2   - r2;

            if ( Math.Abs(diff) < tolerance ) { foundOn = true; }
            else if ( diff      < 0 ) { foundInside     = true; }

            if ( foundInside && foundOn ) { return CircleLineRelation.Secant; }
        }

        if ( foundOn ) { return CircleLineRelation.Tangent; }

        return CircleLineRelation.Disjoint;
    }


    [MustDisposeResource]
    public FilterBuffer<ReadOnlyPoint> Intersections( ref readonly CalculatedLine curve, in double xMin, in double xMax, in int samples = 1000, in double tolerance = 1e-8 )
    {
        double                      r2            = Radius * Radius;
        FilterBuffer<ReadOnlyPoint> intersections = new(samples);
        double                      step          = ( xMax - xMin ) / samples;

        double prevX = xMin;
        double prevD = Helper(prevX, in curve, in Center, in r2);

        for ( int i = 1; i <= samples; i++ )
        {
            double currX = xMin + i * step;
            double currD = Helper(currX, in curve, in Center, in r2);

            if ( double.IsNaN(prevD) || double.IsNaN(currD) )
            {
                prevX = currX;
                prevD = currD;
                continue;
            }

            // Check for sign change or very close to zero
            if ( Math.Abs(prevD) < tolerance )
            {
                ReadOnlyPoint point = new(prevX, curve[prevX]);
                intersections.Add(in point);
            }
            else if ( prevD * currD < 0 ) // sign change → root in between
            {
                double        root  = Bisection(prevX, currX, in tolerance, 50, in curve, in Center, in r2);
                ReadOnlyPoint point = new(root, curve[root]);
                intersections.Add(in point);
            }

            prevX = currX;
            prevD = currD;
        }

        return intersections;

        static double Helper( double x, ref readonly CalculatedLine curve, ref readonly ReadOnlyPoint center, in double r2 )
        {
            double y = curve[x];
            if ( double.IsNaN(y) || double.IsInfinity(y) ) { return double.NaN; }

            double dx = x - center.X;
            double dy = y - center.Y;
            return dx * dx + dy * dy - r2;
        }

        static double Bisection( double a, double b, in double tolerance, int maxIter, ref readonly CalculatedLine curve, ref readonly ReadOnlyPoint center, in double r2 )
        {
            double fa  = Helper(a, in curve, in center, in r2);
            double fb  = Helper(b, in curve, in center, in r2);
            double mid = 0.5 * ( a + b );

            for ( int i = 0; i < maxIter; i++ )
            {
                double fm = Helper(mid, in curve, in center, in r2);

                if ( Math.Abs(fm) < tolerance ) { return mid; }

                if ( fa * fm < 0 )
                {
                    b  = mid;
                    fb = fm;
                }
                else
                {
                    a  = mid;
                    fa = fm;
                }
            }

            return mid;
        }
    }


    public ReadOnlyLine Bisector( ref readonly Degrees degrees ) => new();
    public ReadOnlyLine Bisector( ref readonly Radians radians ) => new();


    public ReadOnlyLine Diameter( ref readonly Degrees degrees ) => new();
    public ReadOnlyLine Diameter( ref readonly Radians radians ) => new();


    [System.Diagnostics.Contracts.Pure, MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public double DistanceTo( in Circle other )
    {
        double x      = Center.X - other.Center.X;
        double y      = Center.Y - other.Center.Y;
        double x2     = x * x;
        double y2     = y * y;
        double result = Math.Sqrt(x2 + y2);
        return result;
    }

    public void Deconstruct( out ReadOnlyPoint start, out double radius )
    {
        start  = Center;
        radius = Radius;
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
