// TrueLogic :: TrueLogic.Common.Maui
// 01/19/2025  21:01


using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlyPointF( float x, float y ) : IPoint<ReadOnlyPointF>, IMathOperators<ReadOnlyPointF>
{
    public static readonly ReadOnlyPointF Invalid = new(float.NaN, float.NaN);
    public static readonly ReadOnlyPointF Zero    = 0;
    public static readonly ReadOnlyPointF One     = 1;
    public readonly        float          X       = x;
    public readonly        float          Y       = y;


    public static       JsonSerializerContext                 JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlyPointF>          JsonTypeInfo  => JakarShapesContext.Default.ReadOnlyPointF;
    public static       JsonTypeInfo<ReadOnlyPointF[]>        JsonArrayInfo => JakarShapesContext.Default.ReadOnlyPointFArray;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.Zero          => ref Zero;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.Invalid       => ref Invalid;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.One           => ref One;
    ReadOnlyPoint IShapeLocation.                             Location      => this;
    bool IValidator.                                          IsValid       => this.IsValid();
    double IShapeLocation.                                    X             => X;
    double IShapeLocation.                                    Y             => Y;


    public static implicit operator ReadOnlyPointF( Point  value ) => new(value.X, value.Y);
    public static implicit operator ReadOnlyPointF( PointF value ) => new(value.X, value.Y);
    public static implicit operator Point( ReadOnlyPointF  value ) => new((int)value.X.Round(), (int)value.Y.Round());
    public static implicit operator PointF( ReadOnlyPointF value ) => new(value.X, value.Y);
    public static implicit operator ReadOnlyPointF( int    value ) => new(value, value);
    public static implicit operator ReadOnlyPointF( long   value ) => new(value, value);
    public static implicit operator ReadOnlyPointF( float  value ) => new(value, value);
    public static implicit operator ReadOnlyPointF( double value ) => new(value.AsFloat(), value.AsFloat());


    [Pure] public static ReadOnlyPointF Create( float  x, float  y ) => new(x, y);
    [Pure] public static ReadOnlyPointF Create( double x, double y ) => new(x.AsFloat(), y.AsFloat());
    public static bool TryFromJson( string? json, out ReadOnlyPointF result )
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
    public static ReadOnlyPointF FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( ReadOnlyPointF other )
    {
        int xOffsetComparison = X.CompareTo(other.X);

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : Y.CompareTo(other.Y);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyPointF other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlyPointF)}");
    }

    public          bool   Equals( ReadOnlyPointF other )                              => X.Equals(other.X)           && Y.Equals(other.Y);
    public override bool   Equals( object?        obj )                                => obj is ReadOnlyPointF other && Equals(other);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IPoint<ReadOnlyPointF>.ToString(in this, format);


    public static bool operator ==( ReadOnlyPointF?         left, ReadOnlyPointF?                  right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyPointF?         left, ReadOnlyPointF?                  right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyPointF          left, ReadOnlyPointF                   right ) => EqualityComparer<ReadOnlyPointF>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyPointF          left, ReadOnlyPointF                   right ) => !EqualityComparer<ReadOnlyPointF>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyPointF           left, ReadOnlyPointF                   right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyPointF          left, ReadOnlyPointF                   right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyPointF           left, ReadOnlyPointF                   right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyPointF          left, ReadOnlyPointF                   right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) <= 0;
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, ReadOnlyPointF                   value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, ReadOnlyPointF                   value ) => self.Subtract(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, ReadOnlyPointF                   value ) => self.Divide(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, ReadOnlyPointF                   value ) => self.Multiply(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, ReadOnlyPoint                    value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, ReadOnlyPoint                    value ) => self.Subtract(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, ReadOnlyPoint                    value ) => self.Divide(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, ReadOnlyPoint                    value ) => self.Multiply(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, double                           value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, double                           value ) => self.Subtract(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, double                           value ) => self.Divide(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, double                           value ) => self.Multiply(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, float                            value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, float                            value ) => self.Subtract(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, float                            value ) => self.Divide(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, float                            value ) => self.Multiply(value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF self, int                              value ) => self.Add(value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF self, int                              value ) => self.Subtract(value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF self, int                              value ) => self.Multiply(value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF self, int                              value ) => self.Divide(value);
}
