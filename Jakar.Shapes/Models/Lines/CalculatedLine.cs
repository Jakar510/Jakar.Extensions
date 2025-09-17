// Jakar.Extensions :: Jakar.Shapes
// 08/07/2025  11:20

namespace Jakar.Shapes;


[DefaultValue(nameof(Invalid))]
public readonly struct CalculatedLine( Func<double, double> func ) : IEqualityOperators<CalculatedLine>
{
    private readonly       Func<double, double> __func   = func;
    public static readonly CalculatedLine       Invalid = new();


    public double this[ double x ] => __func(x);


    public CalculatedLine() : this(static _ => double.NaN) { }
    [Pure] public static CalculatedLine Create( Func<double, double> func ) => new(func);

    [Pure]
    public static CalculatedLine Create( ReadOnlyLine line )
    {
        double m = line.Slope;
        double b = line.Start.Y - m * line.Start.X;
        return new CalculatedLine(func);
        double func( double y ) => m * y + b;
    }


    public static implicit operator CalculatedLine( ReadOnlyLine line ) => Create(line);


    public          bool Equals( CalculatedLine other ) => Equals(__func, other.__func);
    public override bool Equals( object?        other ) => other is CalculatedLine x && Equals(x);
    public override int  GetHashCode()                  => HashCode.Combine(__func);


    public static bool operator ==( CalculatedLine left, CalculatedLine right ) => left.Equals(right);
    public static bool operator !=( CalculatedLine left, CalculatedLine right ) => !left.Equals(right);
}
