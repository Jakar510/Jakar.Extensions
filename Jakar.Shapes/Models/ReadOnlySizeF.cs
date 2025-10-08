// TrueLogic :: TrueLogic.Common.Maui
// 01/20/2025  08:01

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlySizeF( float width, float height ) : ISize<ReadOnlySizeF>, IMathOperators<ReadOnlySizeF>
{
    public static readonly ReadOnlySizeF Invalid = new(float.NaN, float.NaN);
    public static readonly ReadOnlySizeF Zero    = 0;
    public static readonly ReadOnlySizeF One     = 1;
    public readonly        float         Height  = height;
    public readonly        float         Width   = width;


    public static       JsonSerializerContext               JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlySizeF>         JsonTypeInfo  => JakarShapesContext.Default.ReadOnlySizeF;
    public static       JsonTypeInfo<ReadOnlySizeF[]>       JsonArrayInfo => JakarShapesContext.Default.ReadOnlySizeFArray;
    static ref readonly ReadOnlySizeF IShape<ReadOnlySizeF>.Zero          => ref Zero;
    static ref readonly ReadOnlySizeF IShape<ReadOnlySizeF>.Invalid       => ref Invalid;
    static ref readonly ReadOnlySizeF IShape<ReadOnlySizeF>.One           => ref One;
    ReadOnlySize IShapeSize.                                Size          => this;
    bool IValidator.                                        IsValid       => this.IsValid();
    double IShapeSize.                                      Width         => Width;
    double IShapeSize.                                      Height        => Height;


    public static implicit operator Size( ReadOnlySizeF         rectangle ) => new((int)rectangle.Width, (int)rectangle.Height);
    public static implicit operator SizeF( ReadOnlySizeF        rectangle ) => new(rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlySize( ReadOnlySizeF rectangle ) => new(rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlySizeF( Size         size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySizeF( SizeF        size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySizeF( int          value )     => new(value, value);
    public static implicit operator ReadOnlySizeF( long         value )     => new(value, value);
    public static implicit operator ReadOnlySizeF( float        value )     => new(value, value);
    public static implicit operator ReadOnlySizeF( double       value )     => new(value.AsFloat(), value.AsFloat());


    public static bool TryFromJson( string? json, out ReadOnlySizeF result )
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
    public static        ReadOnlySizeF FromJson( string json )        => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));
    [Pure] public static ReadOnlySizeF Create( float    x, float  y ) => new(x, y);
    [Pure] public static ReadOnlySizeF Create( double   x, double y ) => new((float)x, (float)y);


    public int CompareTo( ReadOnlySizeF other )
    {
        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlySizeF other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlySizeF)}");
    }
    public          bool   Equals( ReadOnlySizeF other )                               => Height.Equals(other.Height) && Width.Equals(other.Width);
    public override bool   Equals( object?       obj )                                 => obj is ReadOnlySizeF other  && Equals(other);
    public override int    GetHashCode()                                               => HashCode.Combine(Height, Width);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => this.ToString(format);


    public static bool operator ==( ReadOnlySizeF?        left, ReadOnlySizeF?                   right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlySizeF?        left, ReadOnlySizeF?                   right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlySizeF         left, ReadOnlySizeF                    right ) => EqualityComparer<ReadOnlySizeF>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlySizeF         left, ReadOnlySizeF                    right ) => !EqualityComparer<ReadOnlySizeF>.Default.Equals(left, right);
    public static bool operator >( ReadOnlySizeF          left, ReadOnlySizeF                    right ) => Comparer<ReadOnlySizeF>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlySizeF         left, ReadOnlySizeF                    right ) => Comparer<ReadOnlySizeF>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlySizeF          left, ReadOnlySizeF                    right ) => Comparer<ReadOnlySizeF>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlySizeF         left, ReadOnlySizeF                    right ) => Comparer<ReadOnlySizeF>.Default.Compare(left, right) <= 0;
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, ReadOnlySizeF                    value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, ReadOnlySizeF                    value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, ReadOnlySizeF                    value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, ReadOnlySizeF                    value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, ReadOnlySize                     value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, ReadOnlySize                     value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, ReadOnlySize                     value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, ReadOnlySize                     value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, Size                             value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, Size                             value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, Size                             value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, Size                             value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, SizeF                            value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, SizeF                            value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, SizeF                            value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, SizeF                            value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, double                           value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, double                           value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, double                           value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, double                           value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, float                            value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, float                            value ) => self.Subtract(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, float                            value ) => self.Divide(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, float                            value ) => self.Multiply(value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF self, int                              value ) => self.Add(value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF self, int                              value ) => self.Subtract(value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF self, int                              value ) => self.Multiply(value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF self, int                              value ) => self.Divide(value);
}
