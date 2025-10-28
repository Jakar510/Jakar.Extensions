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


    public static       JsonSerializerContext     JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<Triangle>    JsonTypeInfo  => JakarShapesContext.Default.Triangle;
    public static       JsonTypeInfo<Triangle[]>  JsonArrayInfo => JakarShapesContext.Default.TriangleArray;
    static ref readonly Triangle IShape<Triangle>.Zero          => ref Zero;
    static ref readonly Triangle IShape<Triangle>.One           => ref One;
    static ref readonly Triangle IShape<Triangle>.Invalid       => ref Invalid;
    ReadOnlyPoint IShapeLocation.                 Location      => this.Centroid();
    bool IValidator.                              IsValid       => this.IsValid();
    public bool                                   IsNaN         => A.IsNaN() || B.IsNaN() || C.IsNaN();
    public bool                                   IsValid       => !IsNaN;
    ReadOnlyPoint ITriangle<Triangle>.            A             => A;
    ReadOnlyPoint ITriangle<Triangle>.            B             => B;
    ReadOnlyPoint ITriangle<Triangle>.            C             => C;

    double IShapeLocation.X => this.Centroid()
                                   .X;

    double IShapeLocation.Y => this.Centroid()
                                   .Y;


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
    public static Triangle FromJson( string      json )                                => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    public static Triangle Create( ReadOnlyPoint a, ReadOnlyPoint b, ReadOnlyPoint c ) => new(a, b, c);


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
    public          string ToString( string? format, IFormatProvider? formatProvider ) => this.ToString(format);


    public static bool operator ==( Triangle?   left, Triangle?                        right ) => Nullable.Equals(left, right);
    public static bool operator !=( Triangle?   left, Triangle?                        right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Triangle    left, Triangle                         right ) => EqualityComparer<Triangle>.Default.Equals(left, right);
    public static bool operator !=( Triangle    left, Triangle                         right ) => !EqualityComparer<Triangle>.Default.Equals(left, right);
    public static bool operator >( Triangle     left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) > 0;
    public static bool operator >=( Triangle    left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) >= 0;
    public static bool operator <( Triangle     left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) < 0;
    public static bool operator <=( Triangle    left, Triangle                         right ) => Comparer<Triangle>.Default.Compare(left, right) <= 0;
    public static Triangle operator +( Triangle self, Triangle                         value ) => self.Add(value);
    public static Triangle operator -( Triangle self, Triangle                         value ) => self.Subtract(value);
    public static Triangle operator /( Triangle self, Triangle                         value ) => self.Divide(value);
    public static Triangle operator *( Triangle self, Triangle                         value ) => self.Multiply(value);
    public static Triangle operator +( Triangle self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static Triangle operator -( Triangle self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static Triangle operator /( Triangle self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static Triangle operator *( Triangle self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static Triangle operator +( Triangle self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static Triangle operator -( Triangle self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static Triangle operator *( Triangle self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static Triangle operator /( Triangle self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static Triangle operator +( Triangle self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static Triangle operator -( Triangle self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static Triangle operator *( Triangle self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static Triangle operator /( Triangle self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static Triangle operator +( Triangle self, double                           value ) => self.Add(value);
    public static Triangle operator -( Triangle self, double                           value ) => self.Subtract(value);
    public static Triangle operator /( Triangle self, double                           value ) => self.Divide(value);
    public static Triangle operator *( Triangle self, double                           value ) => self.Multiply(value);
    public static Triangle operator +( Triangle self, float                            value ) => self.Add(value);
    public static Triangle operator -( Triangle self, float                            value ) => self.Subtract(value);
    public static Triangle operator /( Triangle self, float                            value ) => self.Divide(value);
    public static Triangle operator *( Triangle self, float                            value ) => self.Multiply(value);
    public static Triangle operator +( Triangle self, int                              value ) => self.Add(value);
    public static Triangle operator -( Triangle self, int                              value ) => self.Subtract(value);
    public static Triangle operator *( Triangle self, int                              value ) => self.Multiply(value);
    public static Triangle operator /( Triangle self, int                              value ) => self.Divide(value);
}
