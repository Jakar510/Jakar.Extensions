// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  11:12

namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly struct Radians( double value ) : IFormattable, IEquatable<Radians>
{
    public static readonly          Radians Zero  = new(0.0);
    public readonly                 double  Value = value;
    public static implicit operator Radians( Degrees degrees ) => new(degrees.Value * ( Math.PI / 180.0 ));

    public          bool   Equals( Radians other )                                     => Value.Equals(other.Value);
    public override bool   Equals( object? obj )                                       => obj is Radians other && Equals(other);
    public override int    GetHashCode()                                               => Value.GetHashCode();
    public override string ToString()                                                  => $"{Value} rad";
    public          string ToString( string? format, IFormatProvider? formatProvider ) => ToString();
}
