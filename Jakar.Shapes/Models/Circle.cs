// Jakar.Extensions :: Jakar.Extensions
// 07/11/2025  18:56

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


// [Experimental("Jakar_Shapes_Circle")]
[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct Circle( ReadOnlyPoint center, double radius ) : ICircle<Circle>, IMathOperators<Circle>
{
    private const          double        TOLERANCE = 1e-8;
    public static readonly Circle        Invalid   = new(ReadOnlyPoint.Invalid, double.NaN);
    public static readonly Circle        Zero      = 0;
    public static readonly Circle        One       = 1;
    public readonly        ReadOnlyPoint Center    = center;
    public readonly        double        Radius    = radius;


    public static       JsonSerializerContext  JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<Circle>   JsonTypeInfo  => JakarShapesContext.Default.Circle;
    public static       JsonTypeInfo<Circle[]> JsonArrayInfo => JakarShapesContext.Default.CircleArray;
    static ref readonly Circle IShape<Circle>. Zero          => ref Zero;
    static ref readonly Circle IShape<Circle>. One           => ref One;
    static ref readonly Circle IShape<Circle>. Invalid       => ref Invalid;
    bool IValidator.                           IsValid       => this.IsValid();
    double ICircle<Circle>.                    Radius        => Radius;
    ReadOnlyPoint ICircle<Circle>.             Center        => Center;
    ReadOnlyPoint IShapeLocation.              Location      => Center;
    double IShapeLocation.                     X             => Center.X;
    double IShapeLocation.                     Y             => Center.Y;


    public static implicit operator Circle( ReadOnlyPointF point ) => new(point.X, point.Y);
    public static implicit operator Circle( Point          point ) => new(point.X, point.Y);
    public static implicit operator Circle( PointF         point ) => new(point.X, point.Y);
    public static implicit operator Circle( int            other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Circle( long           other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Circle( float          other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Circle( double         other ) => new(ReadOnlyPoint.Zero, other);


    public static bool TryFromJson( string? json, out Circle result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = Invalid;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = Invalid;
        return false;
    }
    public static                                     Circle FromJson( string         json )                  => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    [System.Diagnostics.Contracts.Pure] public static Circle Create( in ReadOnlyPoint center, double radius ) => new(center, radius);
    [System.Diagnostics.Contracts.Pure] public static Circle Create( float            x,      float  y )      => Create(new ReadOnlyPoint(x, y), 1);
    [System.Diagnostics.Contracts.Pure] public static Circle Create( double           x,      double y )      => Create(new ReadOnlyPoint(x, y), 1);

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
    public          string ToString( string? format, IFormatProvider? formatProvider ) => this.ToString(format);


    public static bool operator ==( Circle? left, Circle?                          right ) => Nullable.Equals(left, right);
    public static bool operator !=( Circle? left, Circle?                          right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Circle  left, Circle                           right ) => EqualityComparer<Circle>.Default.Equals(left, right);
    public static bool operator !=( Circle  left, Circle                           right ) => !EqualityComparer<Circle>.Default.Equals(left, right);
    public static bool operator >( Circle   left, Circle                           right ) => Comparer<Circle>.Default.Compare(left, right) > 0;
    public static bool operator >=( Circle  left, Circle                           right ) => Comparer<Circle>.Default.Compare(left, right) >= 0;
    public static bool operator <( Circle   left, Circle                           right ) => Comparer<Circle>.Default.Compare(left, right) < 0;
    public static bool operator <=( Circle  left, Circle                           right ) => Comparer<Circle>.Default.Compare(left, right) <= 0;
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
