using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlyPoint( double x, double y ) : IPoint<ReadOnlyPoint>, IMathOperators<ReadOnlyPoint>, IEqualComparable<ReadOnlyPoint>
{
    public static readonly ReadOnlyPoint Invalid       = new(double.NaN, double.NaN);
    public static readonly ReadOnlyPoint Zero          = 0;
    public static readonly ReadOnlyPoint One           = 1;
    public static readonly ReadOnlyPoint Two           = 2;
    public static readonly ReadOnlyPoint Three         = 3;
    public static readonly ReadOnlyPoint Four          = 4;
    public static readonly ReadOnlyPoint Five          = 5;
    public static readonly ReadOnlyPoint Six           = 6;
    public static readonly ReadOnlyPoint Seven         = 7;
    public static readonly ReadOnlyPoint Eight         = 8;
    public static readonly ReadOnlyPoint Nine          = 9;
    public static readonly ReadOnlyPoint Ten           = 10;
    public static readonly ReadOnlyPoint NegativeOne   = -1;
    public static readonly ReadOnlyPoint NegativeTwo   = -2;
    public static readonly ReadOnlyPoint NegativeThree = -3;
    public static readonly ReadOnlyPoint NegativeFour  = -4;
    public static readonly ReadOnlyPoint NegativeFive  = -5;
    public static readonly ReadOnlyPoint NegativeSix   = -6;
    public static readonly ReadOnlyPoint NegativeSeven = -7;
    public static readonly ReadOnlyPoint NegativeEight = -8;
    public static readonly ReadOnlyPoint NegativeNine  = -9;
    public static readonly ReadOnlyPoint NegativeTen   = -10;
    public readonly        double        X             = x;
    public readonly        double        Y             = y;


    public static       JsonSerializerContext               JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlyPoint>         JsonTypeInfo  => JakarShapesContext.Default.ReadOnlyPoint;
    public static       JsonTypeInfo<ReadOnlyPoint[]>       JsonArrayInfo => JakarShapesContext.Default.ReadOnlyPointArray;
    static ref readonly ReadOnlyPoint IShape<ReadOnlyPoint>.Zero          => ref Zero;
    static ref readonly ReadOnlyPoint IShape<ReadOnlyPoint>.Invalid       => ref Invalid;
    static ref readonly ReadOnlyPoint IShape<ReadOnlyPoint>.One           => ref One;
    ReadOnlyPoint IShapeLocation.                           Location      => this;
    bool IValidator.                                        IsValid       => this.IsValid();
    double IShapeLocation.                                  X             => X;
    double IShapeLocation.                                  Y             => Y;


    public static implicit operator Point( ReadOnlyPoint          point ) => new((int)point.X.Round(), (int)point.Y.Round());
    public static implicit operator PointF( ReadOnlyPoint         point ) => new((float)point.X, (float)point.Y);
    public static implicit operator ReadOnlyPointF( ReadOnlyPoint point ) => new((float)point.X, (float)point.Y);
    public static implicit operator ReadOnlyPoint( ReadOnlyPointF point ) => new(point.X, point.Y);
    public static implicit operator ReadOnlyPoint( Point          point ) => new(point.X, point.Y);
    public static implicit operator ReadOnlyPoint( PointF         point ) => new(point.X, point.Y);
    public static implicit operator ReadOnlyPoint( int            value ) => new(value, value);
    public static implicit operator ReadOnlyPoint( long           value ) => new(value, value);
    public static implicit operator ReadOnlyPoint( float          value ) => new(value, value);
    public static implicit operator ReadOnlyPoint( double         value ) => new(value, value);


    [Pure] public static ReadOnlyPoint Create( float  x, float  y ) => new(x, y);
    [Pure] public static ReadOnlyPoint Create( double x, double y ) => new(x, y);
    public static bool TryFromJson( string? json, out ReadOnlyPoint result )
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
    public static ReadOnlyPoint FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( ReadOnlyPoint other )
    {
        int xOffsetComparison = X.CompareTo(other.X);

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : Y.CompareTo(other.Y);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyPoint other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlyPoint)}");
    }
    public          bool   Equals( ReadOnlyPoint other )                               => X.Equals(other.X)          && Y.Equals(other.Y);
    public override bool   Equals( object?       obj )                                 => obj is ReadOnlyPoint other && Equals(other);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IPoint<ReadOnlyPoint>.ToString(in this, format);


    public static bool operator ==( ReadOnlyPoint?        self, ReadOnlyPoint?                   right ) => Nullable.Equals(self, right);
    public static bool operator !=( ReadOnlyPoint?        self, ReadOnlyPoint?                   right ) => !Nullable.Equals(self, right);
    public static bool operator ==( ReadOnlyPoint         self, ReadOnlyPoint                    right ) => EqualityComparer<ReadOnlyPoint>.Default.Equals(self, right);
    public static bool operator !=( ReadOnlyPoint         self, ReadOnlyPoint                    right ) => !EqualityComparer<ReadOnlyPoint>.Default.Equals(self, right);
    public static bool operator >( ReadOnlyPoint          self, ReadOnlyPoint                    right ) => Comparer<ReadOnlyPoint>.Default.Compare(self, right) > 0;
    public static bool operator >=( ReadOnlyPoint         self, ReadOnlyPoint                    right ) => Comparer<ReadOnlyPoint>.Default.Compare(self, right) >= 0;
    public static bool operator <( ReadOnlyPoint          self, ReadOnlyPoint                    right ) => Comparer<ReadOnlyPoint>.Default.Compare(self, right) < 0;
    public static bool operator <=( ReadOnlyPoint         self, ReadOnlyPoint                    right ) => Comparer<ReadOnlyPoint>.Default.Compare(self, right) <= 0;
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, ReadOnlyPoint                    value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, ReadOnlyPoint                    value ) => self.Subtract(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, ReadOnlyPoint                    value ) => self.Divide(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, ReadOnlyPoint                    value ) => self.Multiply(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, ReadOnlyPointF                   value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, ReadOnlyPointF                   value ) => self.Subtract(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, ReadOnlyPointF                   value ) => self.Divide(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, ReadOnlyPointF                   value ) => self.Multiply(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, double                           value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, double                           value ) => self.Subtract(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, double                           value ) => self.Divide(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, double                           value ) => self.Multiply(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, float                            value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, float                            value ) => self.Subtract(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, float                            value ) => self.Divide(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, float                            value ) => self.Multiply(value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint self, int                              value ) => self.Add(value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint self, int                              value ) => self.Subtract(value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint self, int                              value ) => self.Multiply(value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint self, int                              value ) => self.Divide(value);
}
