// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  11:12

namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly record struct Degrees( double Value ) : IFormattable, IMathOperators<Degrees, double>
{
    public const           double    INCREMENT = 180;
    public const           double    MAX_VALUE = 360;
    public static readonly Degrees   Zero      = new(0);
    public static readonly Degrees[] Angles    = CreateAngles();


    public readonly                 double Value = Value;
    public static implicit operator Degrees( Radians radians ) => new(radians.Value * ( INCREMENT / Math.PI ));
    public static implicit operator Degrees( double  degrees ) => Normalize(degrees);
    public static implicit operator double( Degrees  degrees ) => degrees.Value;


    private static Degrees[] CreateAngles()
    {
        Degrees[] array = GC.AllocateUninitializedArray<Degrees>(360);

        for ( int i = 0; i < array.Length; i++ ) { array[i] = new Degrees(i); }

        return array;
    }
    public static Degrees Normalize( double value )
    {
        value %= MAX_VALUE;
        if ( value < 0 ) { value += MAX_VALUE; }

        return new Degrees(value);
    }


    public override string ToString()                                                  => $"{Value} rad";
    public          string ToString( string? format, IFormatProvider? formatProvider ) => $"{Value.ToString(format, formatProvider)} rad";


    public static Degrees operator +( Degrees a, double scalar ) => Normalize(a.Value + scalar);
    public static Degrees operator -( Degrees a, double scalar ) => Normalize(a.Value - scalar);
    public static Degrees operator *( Degrees a, double scalar ) => Normalize(a.Value * scalar);
    public static Degrees operator /( Degrees a, double scalar ) => Normalize(a.Value / scalar);
}
