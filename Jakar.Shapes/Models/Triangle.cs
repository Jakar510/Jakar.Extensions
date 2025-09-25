// Jakar.Extensions :: Jakar.Shapes
// 07/14/2025  10:08

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct Triangle( ReadOnlyPoint a, ReadOnlyPoint b, ReadOnlyPoint c ) : ITriangle<Triangle>
{
    public static readonly Triangle      Invalid = new(ReadOnlyPoint.Invalid, ReadOnlyPoint.Invalid, ReadOnlyPoint.Invalid);
    public static readonly Triangle      Zero    = new(ReadOnlyPoint.Zero, ReadOnlyPoint.Zero, ReadOnlyPoint.Zero);
    public static readonly Triangle      One     = new(ReadOnlyPoint.One, ReadOnlyPoint.One, ReadOnlyPoint.One);
    public readonly        ReadOnlyPoint A       = a;
    public readonly        ReadOnlyPoint B       = b;
    public readonly        ReadOnlyPoint C       = c;


    static ref readonly Triangle IShape<Triangle>.Zero     => ref Zero;
    static ref readonly Triangle IShape<Triangle>.One      => ref One;
    static ref readonly Triangle IShape<Triangle>.Invalid  => ref Invalid;
    public              bool                      IsEmpty  => A.IsOneOf(B, C) || B.IsOneOf(A, C) || C.IsOneOf(A, B);
    public              bool                      IsNaN    => A.IsNaN         || B.IsNaN         || C.IsNaN;
    public              bool                      IsValid  => !IsNaN;
    ReadOnlyPoint ITriangle<Triangle>.            A        => A;
    ReadOnlyPoint ITriangle<Triangle>.            B        => B;
    ReadOnlyPoint ITriangle<Triangle>.            C        => C;
    public ReadOnlyLine                           Ab       => new(A, B);
    public ReadOnlyLine                           Bc       => new(B, C);
    public ReadOnlyLine                           Ca       => new(C, A);
    public double                                 Area     => Math.Abs(0.5 * ( B.X - A.X ) * ( C.Y - A.Y ) - ( C.X - A.X ) * ( B.Y - A.Y ));
    public ReadOnlyPoint                          Centroid => new(( A.X + B.X + C.X ) / 3, ( A.Y + B.Y + C.Y ) / 3);
    public Degrees                                Abc      => new(A.AngleBetween(in B, in C));
    public Degrees                                Bac      => new(B.AngleBetween(in A, in C));
    public Degrees                                Cab      => new(C.AngleBetween(in A, in B));
    double IShapeLocation.                        X        => Centroid.X;
    double IShapeLocation.                        Y        => Centroid.Y;


    public void Deconstruct( out double x, out double y ) => Centroid.Deconstruct(out x, out y);


    public int CompareTo( Triangle other )
    {
        int aComparison = A.CompareTo(other.A);
        if ( aComparison != 0 ) { return aComparison; }

        int bComparison = B.CompareTo(other.B);
        if ( bComparison != 0 ) { return bComparison; }

        return C.CompareTo(other.C);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is Circle other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(Circle)}");
    }
    public          bool   Equals( Triangle other )                                    => A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C);
    public override bool   Equals( object?  other )                                    => other is Circle x && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(A, B, C);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => ITriangle<Triangle>.ToString(in this, format);


    public static bool operator ==( Triangle?   left, Triangle?                        right ) => Nullable.Equals(left, right);
    public static bool operator !=( Triangle?   left, Triangle?                        right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Triangle    left, Triangle                         right ) => EqualityComparer<Triangle>.Default.Equals(left, right);
    public static bool operator !=( Triangle    left, Triangle                         right ) => !EqualityComparer<Triangle>.Default.Equals(left, right);
    public static bool operator >( Triangle     left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) > 0;
    public static bool operator >=( Triangle    left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) >= 0;
    public static bool operator <( Triangle     left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) < 0;
    public static bool operator <=( Triangle    left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) <= 0;
    public static Triangle operator *( Triangle self, Triangle                         other ) => new(self.A * other.A, self.B * other.B, self.C * other.B);
    public static Triangle operator +( Triangle self, Triangle                         other ) => new(self.A + other.A, self.B + other.B, self.C + other.B);
    public static Triangle operator -( Triangle self, Triangle                         other ) => new(self.A - other.A, self.B - other.B, self.C - other.B);
    public static Triangle operator /( Triangle self, Triangle                         other ) => new(self.A / other.A, self.B / other.B, self.C / other.B);
    public static Triangle operator *( Triangle self, int                              other ) => new(self.A * other, self.B   * other, self.C   * other);
    public static Triangle operator *( Triangle self, float                            other ) => new(self.A * other, self.B   * other, self.C   * other);
    public static Triangle operator *( Triangle self, double                           other ) => new(self.A * other, self.B   * other, self.C   * other);
    public static Triangle operator /( Triangle self, int                              other ) => new(self.A / other, self.B   / other, self.C   / other);
    public static Triangle operator /( Triangle self, float                            other ) => new(self.A / other, self.B   / other, self.C   / other);
    public static Triangle operator /( Triangle self, double                           other ) => new(self.A / other, self.B   / other, self.C   / other);
    public static Triangle operator +( Triangle self, int                              other ) => new(self.A + other, self.B + other, self.C + other);
    public static Triangle operator +( Triangle self, float                            other ) => new(self.A + other, self.B + other, self.C + other);
    public static Triangle operator +( Triangle self, double                           other ) => new(self.A + other, self.B + other, self.C + other);
    public static Triangle operator -( Triangle self, int                              other ) => new(self.A - other, self.B - other, self.C - other);
    public static Triangle operator -( Triangle self, float                            other ) => new(self.A - other, self.B - other, self.C - other);
    public static Triangle operator -( Triangle self, double                           other ) => new(self.A - other, self.B - other, self.C - other);
    public static Triangle operator +( Triangle self, (int xOffset, int yOffset)       other ) => new(self.A + other, self.B + other, self.C + other);
    public static Triangle operator +( Triangle self, (float xOffset, float yOffset)   other ) => new(self.A + other, self.B + other, self.C + other);
    public static Triangle operator +( Triangle self, (double xOffset, double yOffset) other ) => new(self.A + other, self.B + other, self.C + other);
    public static Triangle operator -( Triangle self, (int xOffset, int yOffset)       other ) => new(self.A - other, self.B - other, self.C - other);
    public static Triangle operator -( Triangle self, (float xOffset, float yOffset)   other ) => new(self.A - other, self.B - other, self.C - other);
    public static Triangle operator -( Triangle self, (double xOffset, double yOffset) other ) => new(self.A - other, self.B - other, self.C - other);
    public static Triangle operator *( Triangle self, (int xOffset, int yOffset)       other ) => new(self.A * other, self.B * other, self.C * other);
    public static Triangle operator *( Triangle self, (float xOffset, float yOffset)   other ) => new(self.A * other, self.B * other, self.C * other);
    public static Triangle operator *( Triangle self, (double xOffset, double yOffset) other ) => new(self.A * other, self.B * other, self.C * other);
    public static Triangle operator /( Triangle self, (int xOffset, int yOffset)       other ) => new(self.A / other, self.B / other, self.C / other);
    public static Triangle operator /( Triangle self, (float xOffset, float yOffset)   other ) => new(self.A / other, self.B / other, self.C / other);
    public static Triangle operator /( Triangle self, (double xOffset, double yOffset) other ) => new(self.A / other, self.B / other, self.C / other);


    public static JsonSerializerContext    JsonContext   => JakarShapesContext.Default;
    public static JsonTypeInfo<Triangle>   JsonTypeInfo  => JakarShapesContext.Default.Triangle;
    public static JsonTypeInfo<Triangle[]> JsonArrayInfo => JakarShapesContext.Default.TriangleArray;
    public static bool TryFromJson( string? json, out Triangle result )
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
    public static Triangle FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
}
