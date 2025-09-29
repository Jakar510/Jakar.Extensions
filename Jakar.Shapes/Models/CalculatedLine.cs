// Jakar.Extensions :: Jakar.Shapes
// 08/07/2025  11:20

namespace Jakar.Shapes;


[DefaultValue(nameof(Invalid))]
[NotSerializable]
[IsNotJsonSerializable]
public readonly struct CalculatedLine( Func<double, double> func ) : IEqualityOperators<CalculatedLine>
{
    private readonly       Func<double, double> __func  = func;
    public static readonly CalculatedLine       Invalid = new();


    public double this[ double x ] { [Pure] get => __func(x); }

    public CalculatedLine() : this(static _ => double.NaN) { }
    [Pure] public static CalculatedLine Create( Func<double, double> func ) => new(func);


    /*
    public static CalculatedLine CreateNoIntercept( double exponent, params ReadOnlySpan<ReadOnlyPoint> points )
    {
        if ( points.Length < 1 ) { return CalculatedLine.Invalid; }

        double num = 0.0;
        double den = 0.0;

        foreach ( var (x, y) in points )
        {
            double xk = Math.Pow(x, exponent);
            if ( double.IsNaN(xk) || double.IsInfinity(xk) ) { return CalculatedLine.Invalid; }

            num += y  * xk;
            den += xk * xk;
        }

        if ( den == 0.0 ) { return CalculatedLine.Invalid; }

        double A = num / den;

        return CalculatedLine.Create(x =>
                                     {
                                         double px = Math.Pow(x, exponent);

                                         return double.IsNaN(px)
                                                    ? double.NaN
                                                    : A * px;
                                     });
    }
    public static CalculatedLine CreateWithIntercept( double exponent, params ReadOnlySpan<ReadOnlyPoint> points )
    {
        if ( points.Length < 2 ) { return CalculatedLine.Invalid; }

        double S_xk2 = 0.0, S_xk = 0.0, S_xk_y = 0.0, S_y = 0.0;
        int    n     = 0;

        foreach ( var (x, y) in points )
        {
            double xk = Math.Pow(x, exponent);
            if ( double.IsNaN(xk) || double.IsInfinity(xk) ) { return CalculatedLine.Invalid; }

            S_xk2  += xk * xk;
            S_xk   += xk;
            S_xk_y += xk * y;
            S_y    += y;
            n++;
        }

        double det = S_xk2 * n - S_xk * S_xk;
        if ( det == 0.0 ) { return CalculatedLine.Invalid; }

        double A = ( n     * S_xk_y - S_xk * S_y )    / det;
        double b = ( S_xk2 * S_y    - S_xk * S_xk_y ) / det;

        return CalculatedLine.Create(x =>
                                     {
                                         double px = Math.Pow(x, exponent);

                                         return double.IsNaN(px)
                                                    ? double.NaN
                                                    : A * px + b;
                                     });
    }
    public static CalculatedLine CreateWithLog( double exponent, params ReadOnlySpan<ReadOnlyPoint> points )
    {
        if ( points.Length == 0 ) { return CalculatedLine.Invalid; }

        double sum = 0.0;
        int    n   = 0;

        foreach ( var (x, y) in points )
        {
            if ( x <= 0.0 || y <= 0.0 )
            {
                return CalculatedLine.Invalid; // log-method requires positive values
            }

            sum += Math.Log(y) - exponent * Math.Log(x);
            n++;
        }

        if ( n == 0 ) { return CalculatedLine.Invalid; }

        double lnA = sum / n;
        double A   = Math.Exp(lnA);

        return CalculatedLine.Create(x =>
                                     {
                                         if ( x <= 0.0 ) { return double.NaN; }

                                         return A * Math.Pow(x, exponent);
                                     });
    }
    */


    [Pure] public static CalculatedLine Create( ReadOnlyLine line )
    {
        double m = line.Slope;
        double b = line.Start.Y - m * line.Start.X;
        return new CalculatedLine(func);
        double func( double y ) => m * y + b;
    }
    public Spline ToSpline( params ReadOnlySpan<double> xValues )
    {
        ReadOnlyPoint[] points = new ReadOnlyPoint[xValues.Length];
        for ( int i = 0; i < xValues.Length; i++ ) { points[i] = Get(xValues[i]); }

        return new Spline(points);
    }
    public ReadOnlyPoint Get( double x ) => new(x, this[x]);


    public static implicit operator CalculatedLine( ReadOnlyLine line ) => Create(line);


    public          bool Equals( CalculatedLine other ) => Equals(__func, other.__func);
    public override bool Equals( object?        other ) => other is CalculatedLine x && Equals(x);
    public override int  GetHashCode()                  => HashCode.Combine(__func);


    public static bool operator ==( CalculatedLine left, CalculatedLine right ) => left.Equals(right);
    public static bool operator !=( CalculatedLine left, CalculatedLine right ) => !left.Equals(right);
}
