// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  08:54

namespace Jakar.Shapes;


public static class Circles
{
    public const double TOLERANCE = 1e-8;


    public static string ToString<TCircle>( this TCircle self, string? format )
        where TCircle : struct, ICircle<TCircle>
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson(TCircle.JsonTypeInfo);

            case ",":
                return $"{self.X},{self.Y}";

            case "-":
                return $"{self.X}-{self.Y}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TCircle).Name}<{nameof(self.Center)}: {self.Center}, {nameof(self.Radius)}: {self.Radius}>";
        }
    }


    [Pure] public static TCircle Reverse<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => TCircle.Create(self.Y, self.X);
    [Pure] public static TCircle Round<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => TCircle.Create(self.X.Round(), self.Y.Round());
    [Pure] public static TCircle Floor<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => TCircle.Create(self.X.Floor(), self.Y.Floor());


    [Pure] public static ReadOnlyLine RadiusLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle> =>
        new(self.Center, new ReadOnlyPoint(self.Center.X + self.Radius * Math.Cos(radians.Value), self.Center.Y + self.Radius * Math.Sin(radians.Value)));


    [Pure] public static CalculatedLine RadiusCalculatedLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle>
    {
        ReadOnlyPoint center = self.Center;
        ReadOnlyPoint end    = new(self.Center.X + self.Radius * Math.Cos(radians.Value), self.Center.Y + self.Radius * Math.Sin(radians.Value));

        double dx = end.X - center.X;
        double dy = end.Y - center.Y;

        if ( Math.Abs(dx) < double.Epsilon )
        {
            double value = self.Center.X;
            return CalculatedLine.Create(x => value);
        }

        double m = dy / dx;
        double b = center.Y - m * center.X;

        return CalculatedLine.Create(x => m * x + b);
    }


    [Pure] public static ReadOnlyLine DiameterLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle>
    {
        ReadOnlyPoint center = self.Center;
        ReadOnlyPoint start  = new(center.X - self.Radius * Math.Cos(radians.Value), center.Y - self.Radius * Math.Sin(radians.Value));
        ReadOnlyPoint end    = new(center.X + self.Radius * Math.Cos(radians.Value), center.Y + self.Radius * Math.Sin(radians.Value));
        return new ReadOnlyLine(start, end);
    }


    [Pure] public static CalculatedLine DiameterCalculatedLine<TCircle>( this TCircle self, in Radians radians )
        where TCircle : struct, ICircle<TCircle>
    {
        ReadOnlyPoint center = self.Center;
        ReadOnlyPoint start  = new(center.X - self.Radius * Math.Cos(radians.Value), center.Y - self.Radius * Math.Sin(radians.Value));
        ReadOnlyPoint end    = new(center.X + self.Radius * Math.Cos(radians.Value), center.Y + self.Radius * Math.Sin(radians.Value));

        double dx = end.X - start.X;
        double dy = end.Y - start.Y;

        if ( Math.Abs(dx) < double.Epsilon ) { return CalculatedLine.Create(x => start.X); }

        double m = dy / dx;
        double b = start.Y - m * start.X;

        return CalculatedLine.Create(x => m * x + b);
    }


    public static void Deconstruct<TCircle>( this TCircle self, out float x, out float y, out float radius )
        where TCircle : struct, ICircle<TCircle>
    {
        x      = (float)self.X;
        y      = (float)self.Y;
        radius = (float)self.Radius;
    }
    public static void Deconstruct<TCircle>( this TCircle self, out double x, out double y, out double radius )
        where TCircle : struct, ICircle<TCircle>
    {
        x      = self.X;
        y      = self.Y;
        radius = self.Radius;
    }
    public static void Deconstruct<TCircle>( this TCircle self, out ReadOnlyPoint point, out double radius )
        where TCircle : struct, ICircle<TCircle>
    {
        radius = self.Radius;
        point  = self.Location;
    }
    public static void Deconstruct<TCircle>( this TCircle self, out ReadOnlyPointF point, out double radius )
        where TCircle : struct, ICircle<TCircle>
    {
        radius = self.Radius;
        point  = self.Location;
    }


    public static ReadOnlyPoint Center<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => self.Location;
    public static TCircle Abs<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => TCircle.Create(self.Location.Abs(), double.Abs(self.Radius));
    public static bool IsFinite<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => double.IsFinite(self.X) && double.IsFinite(self.Y) && double.IsFinite(self.Radius);
    public static bool IsInfinity<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => double.IsInfinity(self.X) || double.IsInfinity(self.Y) || double.IsInfinity(self.Radius);
    public static bool IsInteger<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => double.IsInteger(self.X) && double.IsInteger(self.Y) && double.IsInteger(self.Radius);
    public static bool IsNaN<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => double.IsNaN(self.X) || double.IsNaN(self.Y) || double.IsNaN(self.Radius);
    public static bool IsNegative<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => self.Radius < 0 || self.Location.IsNegative();
    public static bool IsValid<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => !self.IsNaN() && self.IsFinite() && self.Radius > 0 && self.Location.IsValid();
    public static bool IsPositive<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => self.Radius > 0 || self.Location.IsPositive();
    public static bool IsZero<TCircle>( this TCircle self )
        where TCircle : struct, ICircle<TCircle> => self.Radius == 0 || self.Location.IsZero();


    public static double DistanceTo<TCircle>( this TCircle self, TCircle other )
        where TCircle : struct, ICircle<TCircle> =>
        self.DistanceTo(other.Center);
    public static double DistanceTo<TCircle, TPoint>( this TCircle self, TPoint other )
        where TCircle : struct, ICircle<TCircle>
        where TPoint : struct, IPoint<TPoint> =>
        self.Center.DistanceTo(other);


    public static bool IsTangent<TCircle>( this TCircle self, ReadOnlyLine line )
        where TCircle : struct, ICircle<TCircle> => self.GetLineRelation(line) is CircleLineRelation.Tangent;
    public static bool IsSecant<TCircle>( this TCircle self, ReadOnlyLine line )
        where TCircle : struct, ICircle<TCircle> => self.GetLineRelation(line) is CircleLineRelation.Secant;
    public static bool IsDisjoint<TCircle>( this TCircle self, ReadOnlyLine line )
        where TCircle : struct, ICircle<TCircle> => self.GetLineRelation(line) is CircleLineRelation.Disjoint;


    public static bool IsTangent<TCircle>( this TCircle self, CalculatedLine line, in double xMin, in double xMax, in int samples = 1000, in double tolerance = TOLERANCE )
        where TCircle : struct, ICircle<TCircle> => self.GetLineRelation(line, xMin, xMax, samples, tolerance) is CircleLineRelation.Tangent;
    public static bool IsSecant<TCircle>( this TCircle self, CalculatedLine line, double xMin, double xMax, int samples = 1000, double tolerance = TOLERANCE )
        where TCircle : struct, ICircle<TCircle> => self.GetLineRelation(line, xMin, xMax, samples, tolerance) is CircleLineRelation.Secant;
    public static bool IsDisjoint<TCircle>( this TCircle self, CalculatedLine line, double xMin, double xMax, int samples = 1000, double tolerance = TOLERANCE )
        where TCircle : struct, ICircle<TCircle> => self.GetLineRelation(line, xMin, xMax, samples, tolerance) is CircleLineRelation.Disjoint;


    public static CircleLineRelation GetLineRelation<TCircle, TLine>( this TCircle self, TLine line )
        where TCircle : struct, ICircle<TCircle>
        where TLine : struct, ILine<TLine>
    {
        double dx           = line.End.X   - line.Start.X;
        double dy           = line.End.Y   - line.Start.Y;
        double fx           = line.Start.X - self.Center.X;
        double fy           = line.Start.Y - self.Center.Y;
        double a            = dx * dx      + dy * dy;
        double b            = 2 * ( fx * dx + fy * dy );
        double c            = fx * fx + fy * fy - self.Radius * self.Radius;
        double discriminant = b                               * b - 4 * a * c;
        if ( discriminant < 0 ) { return CircleLineRelation.Disjoint; }

        if ( !line.IsFinite )
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
    public static CircleLineRelation GetLineRelation<TCircle>( this TCircle self, CalculatedLine curve, double xMin, double xMax, int samples = 1000, double tolerance = TOLERANCE )
        where TCircle : struct, ICircle<TCircle>
    {
        double r2          = self.Radius     * self.Radius;
        double range       = ( xMax - xMin ) / samples;
        bool   foundOn     = false;
        bool   foundInside = false;

        for ( int i = 0; i <= samples; i++ )
        {
            double x = xMin + range * i;
            double y = curve[x];

            if ( double.IsNaN(y) || double.IsInfinity(y) ) { continue; }

            double dx    = x       - self.Center.X;
            double dy    = y       - self.Center.Y;
            double dist2 = dx * dx + dy * dy;
            double diff  = dist2   - r2;

            if ( Math.Abs(diff) < tolerance ) { foundOn = true; }
            else if ( diff      < 0 ) { foundInside     = true; }

            if ( foundInside && foundOn ) { return CircleLineRelation.Secant; }
        }

        if ( foundOn ) { return CircleLineRelation.Tangent; }

        return CircleLineRelation.Disjoint;
    }


    [Pure] [MustDisposeResource] public static ArrayBuffer<ReadOnlyPoint> Intersections<TCircle, TLine>( this TCircle self, CalculatedLine curve, double xMin, double xMax, int samples = 1000, double tolerance = TOLERANCE )
        where TCircle : struct, ICircle<TCircle>
        where TLine : struct, ILine<TLine>
    {
        ReadOnlyPoint              center        = self.Center;
        double                     r2            = self.Radius * self.Radius;
        ArrayBuffer<ReadOnlyPoint> intersections = new(samples);
        double                     step          = ( xMax - xMin ) / samples;

        double prevX = xMin;
        double prevD = helper(prevX, curve, center, r2);

        for ( int i = 1; i <= samples; i++ )
        {
            double currX = xMin + i * step;
            double currD = helper(currX, curve, center, r2);

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
                double        root  = bisection(prevX, currX, tolerance, 50, curve, center, r2);
                ReadOnlyPoint point = new(root, curve[root]);
                intersections.Add(in point);
            }

            prevX = currX;
            prevD = currD;
        }

        return intersections;

        static double helper( double x, CalculatedLine curve, ReadOnlyPoint center, double r2 )
        {
            double y = curve[x];
            if ( double.IsNaN(y) || double.IsInfinity(y) ) { return double.NaN; }

            double dx = x - center.X;
            double dy = y - center.Y;
            return dx * dx + dy * dy - r2;
        }

        static double bisection( double a, double b, double tolerance, int maxIter, CalculatedLine curve, ReadOnlyPoint center, double r2 )
        {
            double fa  = helper(a, curve, center, r2);
            double fb  = helper(b, curve, center, r2);
            double mid = 0.5 * ( a + b );

            for ( int i = 0; i < maxIter; i++ )
            {
                double fm = helper(mid, curve, center, r2);

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
}
