using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlySize( double width, double height ) : ISize<ReadOnlySize>, IMathOperators<ReadOnlySize>
{
    public static readonly ReadOnlySize Invalid = new(double.NaN, double.NaN);
    public static readonly ReadOnlySize Zero    = 0;
    public static readonly ReadOnlySize One     = 1;
    public readonly        double       Height  = height;
    public readonly        double       Width   = width;


    public static       JsonSerializerContext             JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlySize>        JsonTypeInfo  => JakarShapesContext.Default.ReadOnlySize;
    public static       JsonTypeInfo<ReadOnlySize[]>      JsonArrayInfo => JakarShapesContext.Default.ReadOnlySizeArray;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Zero          => ref Zero;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Invalid       => ref Invalid;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.One           => ref One;
    ReadOnlySize IShapeSize.                              Size          => this;
    bool IValidator.                                      IsValid       => this.IsValid();
    double IShapeSize.                                    Width         => Width;
    double IShapeSize.                                    Height        => Height;


    [Pure] public static ReadOnlySize Create( float  width, float  height ) => new(width, height);
    [Pure] public static ReadOnlySize Create( double width, double height ) => new(width, height);


    public static bool TryFromJson( string? json, out ReadOnlySize result )
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
    public static ReadOnlySize FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( ReadOnlySize other )
    {
        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlySize other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlySize)}");
    }
    public          bool   Equals( ReadOnlySize other )                                => Height.Equals(other.Height);
    public override bool   Equals( object?      obj )                                  => obj is ReadOnlySize other && Equals(other);
    public override int    GetHashCode()                                               => Height.GetHashCode();
    public override string ToString()                                                  => ToString(null, null);
    public readonly string ToString( string? format, IFormatProvider? formatProvider ) => this.ToString(format);


    public static implicit operator Size( ReadOnlySize          rectangle ) => new((int)rectangle.Width, (int)rectangle.Height);
    public static implicit operator SizeF( ReadOnlySize         rectangle ) => new((float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlySizeF( ReadOnlySize rectangle ) => new((float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlySize( Size          size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySize( SizeF         size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySize( int           value )     => new(value, value);
    public static implicit operator ReadOnlySize( long          value )     => new(value, value);
    public static implicit operator ReadOnlySize( float         value )     => new(value, value);
    public static implicit operator ReadOnlySize( double        value )     => new(value, value);


    public static bool operator ==( ReadOnlySize?       left, ReadOnlySize?                    right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlySize?       left, ReadOnlySize?                    right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlySize        left, ReadOnlySize                     right ) => EqualityComparer<ReadOnlySize>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlySize        left, ReadOnlySize                     right ) => !EqualityComparer<ReadOnlySize>.Default.Equals(left, right);
    public static bool operator >( ReadOnlySize         left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlySize        left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlySize         left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlySize        left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) <= 0;
    public static ReadOnlySize operator +( ReadOnlySize self, ReadOnlySize                     value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, ReadOnlySize                     value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, ReadOnlySize                     value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, ReadOnlySize                     value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, ReadOnlySizeF                    value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, ReadOnlySizeF                    value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, ReadOnlySizeF                    value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, ReadOnlySizeF                    value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, Size                             value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, Size                             value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, Size                             value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, Size                             value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, SizeF                            value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, SizeF                            value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, SizeF                            value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, SizeF                            value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlySize operator *( ReadOnlySize self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlySize operator /( ReadOnlySize self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlySize operator +( ReadOnlySize self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlySize operator *( ReadOnlySize self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlySize operator /( ReadOnlySize self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlySize operator +( ReadOnlySize self, double                           value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, double                           value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, double                           value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, double                           value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, float                            value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, float                            value ) => self.Subtract(value);
    public static ReadOnlySize operator /( ReadOnlySize self, float                            value ) => self.Divide(value);
    public static ReadOnlySize operator *( ReadOnlySize self, float                            value ) => self.Multiply(value);
    public static ReadOnlySize operator +( ReadOnlySize self, int                              value ) => self.Add(value);
    public static ReadOnlySize operator -( ReadOnlySize self, int                              value ) => self.Subtract(value);
    public static ReadOnlySize operator *( ReadOnlySize self, int                              value ) => self.Multiply(value);
    public static ReadOnlySize operator /( ReadOnlySize self, int                              value ) => self.Divide(value);
}
