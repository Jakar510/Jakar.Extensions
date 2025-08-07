// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  11:12

namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly struct Degrees( double value ) : IFormattable, IEquatable<Degrees>
{
    public static readonly          Degrees Zero  = new(0.0);
    public readonly                 double  Value = value;
    public static implicit operator Degrees( Radians radians ) => new(radians.Value * ( 180.0 / Math.PI ));


    public          bool   Equals( Degrees other )                                      => Value.Equals(other.Value);
    public override bool   Equals( object? obj )                                        => obj is Degrees other && Equals(other);
    public override int    GetHashCode()                                                => Value.GetHashCode();
    public override string ToString()                                                   => $"{Value} rad";
    public          string ToString( string?  format, IFormatProvider? formatProvider ) => ToString();
    public static   bool operator ==( Degrees left,   Degrees          right )          => left.Equals(right);
    public static   bool operator !=( Degrees left,   Degrees          right )          => left.Equals(right) is false;
}
