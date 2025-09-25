// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  11:12

namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly record struct Radians( double Value ) : IFormattable, IMathOperators<Radians, double>
{
    public const           double    INCREMENT = Math.PI / 180;
    public const           double    MAX_VALUE = 2       * Math.PI;
    public static readonly Radians   Zero      = new(0.0);
    public static readonly Radians   One       = new(INCREMENT);
    public static readonly Radians[] Angles    = CreateAngles();


    public readonly double Value = Value;


    public static implicit operator Radians( Degrees degrees ) => new(degrees.Value * INCREMENT);
    public static implicit operator Radians( double  radians ) => Normalize(radians);
    public static implicit operator double( Radians  degrees ) => degrees.Value;


    private static Radians[] CreateAngles()
    {
        Radians[] array = GC.AllocateUninitializedArray<Radians>(360);

        for ( int i = 0; i < array.Length; i++ ) array[i] = new Radians(i * INCREMENT);
        return array;
    }
    public static Radians Normalize( double value )
    {
        value %= MAX_VALUE;
        if ( value < 0 ) { value += MAX_VALUE; }

        return new Radians(value);
    }


    public override string ToString()                                                         => $"{Value:0.####}°";
    public          string ToString( string?     format, IFormatProvider? formatProvider )    => $"{Value.ToString(format, formatProvider)}°";
    public          bool   NearlyEquals( Radians other,  double           tolerance = 1e-10 ) => Math.Abs(Value - other.Value) <= tolerance;


    public static Radians operator +( Radians a, double scalar ) => Normalize(a.Value + scalar);
    public static Radians operator -( Radians a, double scalar ) => Normalize(a.Value - scalar);
    public static Radians operator *( Radians a, double scalar ) => Normalize(a.Value * scalar);
    public static Radians operator /( Radians a, double scalar ) => Normalize(a.Value / scalar);
}
