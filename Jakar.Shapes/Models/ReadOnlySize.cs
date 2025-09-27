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
    public              bool                              IsValid       => !IsNaN && !IsEmpty;
    [JsonIgnore] public bool                              IsEmpty       => IsNaN || Width < 0 || Height < 0;
    public              bool                              IsLandscape   => Width < Height;
    public              bool                              IsNaN         => double.IsNaN(Width) || double.IsNaN(Height);
    public              bool                              IsPortrait    => Width > Height;
    double IShapeSize.                                    Width         => Width;
    double IShapeSize.                                    Height        => Height;


    [Pure] public static ReadOnlySize Create( float  width, float  height ) => new(width, height);
    [Pure] public static ReadOnlySize Create( double width, double height ) => new(width, height);
    [Pure] public        ReadOnlySize Reverse() => new(Height, Width);
    [Pure] public        ReadOnlySize Round()   => new(Width.Round(), Height.Round());
    [Pure] public        ReadOnlySize Floor()   => new(Width.Floor(), Height.Floor());


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


    public void Deconstruct( out double width, out double height )
    {
        width  = Width;
        height = Height;
    }


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
    public          string ToString( string? format, IFormatProvider? formatProvider ) => ISize<ReadOnlySize>.ToString(in this, format);


    public static implicit operator Size( ReadOnlySize   rectangle ) => new((int)rectangle.Width, (int)rectangle.Height);
    public static implicit operator SizeF( ReadOnlySize  rectangle ) => new((float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlySize( Size   size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySize( SizeF  size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySize( int    value )     => new(value, value);
    public static implicit operator ReadOnlySize( long   value )     => new(value, value);
    public static implicit operator ReadOnlySize( float  value )     => new(value, value);
    public static implicit operator ReadOnlySize( double value )     => new(value, value);


    public static bool operator ==( ReadOnlySize?       left, ReadOnlySize?                    right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlySize?       left, ReadOnlySize?                    right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlySize        left, ReadOnlySize                     right ) => EqualityComparer<ReadOnlySize>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlySize        left, ReadOnlySize                     right ) => !EqualityComparer<ReadOnlySize>.Default.Equals(left, right);
    public static bool operator >( ReadOnlySize         left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlySize        left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlySize         left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlySize        left, ReadOnlySize                     right ) => Comparer<ReadOnlySize>.Default.Compare(left, right) <= 0;
    public static ReadOnlySize operator +( ReadOnlySize size, Size                             value ) => new(size.Width + value.Width, size.Height   + value.Height);
    public static ReadOnlySize operator +( ReadOnlySize size, SizeF                            value ) => new(size.Width + value.Width, size.Height   + value.Height);
    public static ReadOnlySize operator +( ReadOnlySize size, ReadOnlySize                     value ) => new(size.Width + value.Width, size.Height   + value.Height);
    public static ReadOnlySize operator +( ReadOnlySize size, (int xOffset, int yOffset)       value ) => new(size.Width + value.xOffset, size.Height + value.yOffset);
    public static ReadOnlySize operator +( ReadOnlySize size, (float xOffset, float yOffset)   value ) => new(size.Width + value.xOffset, size.Height + value.yOffset);
    public static ReadOnlySize operator +( ReadOnlySize size, (double xOffset, double yOffset) value ) => new(size.Width + value.xOffset, size.Height + value.yOffset);
    public static ReadOnlySize operator -( ReadOnlySize size, Size                             value ) => new(size.Width - value.Width, size.Height   - value.Height);
    public static ReadOnlySize operator -( ReadOnlySize size, SizeF                            value ) => new(size.Width - value.Width, size.Height   - value.Height);
    public static ReadOnlySize operator -( ReadOnlySize size, ReadOnlySize                     value ) => new(size.Width - value.Width, size.Height   - value.Height);
    public static ReadOnlySize operator -( ReadOnlySize size, (int xOffset, int yOffset)       value ) => new(size.Width - value.xOffset, size.Height - value.yOffset);
    public static ReadOnlySize operator -( ReadOnlySize size, (float xOffset, float yOffset)   value ) => new(size.Width - value.xOffset, size.Height - value.yOffset);
    public static ReadOnlySize operator -( ReadOnlySize size, (double xOffset, double yOffset) value ) => new(size.Width - value.xOffset, size.Height - value.yOffset);
    public static ReadOnlySize operator *( ReadOnlySize size, ReadOnlySize                     value ) => new(size.Width * value.Width, size.Height   * value.Height);
    public static ReadOnlySize operator *( ReadOnlySize size, int                              value ) => new(size.Width * value, size.Height         * value);
    public static ReadOnlySize operator *( ReadOnlySize size, float                            value ) => new(size.Width * value, size.Height         * value);
    public static ReadOnlySize operator *( ReadOnlySize size, double                           value ) => new(size.Width * value, size.Height         * value);
    public static ReadOnlySize operator *( ReadOnlySize size, (int xOffset, int yOffset)       value ) => new(size.Width * value.xOffset, size.Height * value.yOffset);
    public static ReadOnlySize operator *( ReadOnlySize size, (float xOffset, float yOffset)   value ) => new(size.Width * value.xOffset, size.Height * value.yOffset);
    public static ReadOnlySize operator *( ReadOnlySize size, (double xOffset, double yOffset) value ) => new(size.Width * value.xOffset, size.Height * value.yOffset);
    public static ReadOnlySize operator /( ReadOnlySize size, ReadOnlySize                     value ) => new(size.Width / value.Width, size.Height   / value.Height);
    public static ReadOnlySize operator /( ReadOnlySize size, int                              value ) => new(size.Width / value, size.Height         / value);
    public static ReadOnlySize operator /( ReadOnlySize size, float                            value ) => new(size.Width / value, size.Height         / value);
    public static ReadOnlySize operator /( ReadOnlySize size, double                           value ) => new(size.Width / value, size.Height         / value);
    public static ReadOnlySize operator /( ReadOnlySize size, (int xOffset, int yOffset)       value ) => new(size.Width / value.xOffset, size.Height / value.yOffset);
    public static ReadOnlySize operator /( ReadOnlySize size, (float xOffset, float yOffset)   value ) => new(size.Width / value.xOffset, size.Height / value.yOffset);
    public static ReadOnlySize operator /( ReadOnlySize size, (double xOffset, double yOffset) value ) => new(size.Width / value.xOffset, size.Height / value.yOffset);
    public static ReadOnlySize operator +( ReadOnlySize left, double                           value ) => new(left.Width + value, left.Height + value);
    public static ReadOnlySize operator -( ReadOnlySize left, double                           value ) => new(left.Width - value, left.Height - value);
    public static ReadOnlySize operator +( ReadOnlySize left, float                            value ) => new(left.Width + value, left.Height + value);
    public static ReadOnlySize operator -( ReadOnlySize left, float                            value ) => new(left.Width - value, left.Height - value);
    public static ReadOnlySize operator +( ReadOnlySize left, int                              value ) => new(left.Width + value, left.Height + value);
    public static ReadOnlySize operator -( ReadOnlySize left, int                              value ) => new(left.Width - value, left.Height - value);
}
