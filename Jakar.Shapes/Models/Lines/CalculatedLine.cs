// Jakar.Extensions :: Jakar.Shapes
// 08/07/2025  11:20

namespace Jakar.Shapes;


[DefaultValue(nameof(Invalid))]
public readonly struct CalculatedLine( Func<double, double> func ) : IEqualityOperators<CalculatedLine>
{
    private readonly       Func<double, double> _func   = func;
    public static readonly CalculatedLine       Invalid = new();


    public double this[ double x ] => _func(x);


    public CalculatedLine() : this(static _ => double.NaN) { }
    [Pure] public static CalculatedLine Create( Func<double, double> func ) => new(func);

    [Pure]
    public static CalculatedLine Create( ReadOnlyLine line )
    {
        double m = line.Slope;
        double b = line.Start.Y - m * line.Start.X;
        return new CalculatedLine(Func);
        double Func( double y ) => m * y + b;
    }


    public static implicit operator CalculatedLine( ReadOnlyLine line ) => Create(line);


    public          bool Equals( CalculatedLine other ) => Equals(_func, other._func);
    public override bool Equals( object?        other ) => other is CalculatedLine x && Equals(x);
    public override int  GetHashCode()                  => HashCode.Combine(_func);


    public static bool operator ==( CalculatedLine left, CalculatedLine right ) => left.Equals(right);
    public static bool operator !=( CalculatedLine left, CalculatedLine right ) => left.Equals(right) is false;
}
