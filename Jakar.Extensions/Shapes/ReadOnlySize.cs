namespace Jakar.Extensions;


[DefaultValue(nameof(Zero))]
public readonly struct ReadOnlySize( double width, double height ) : ISize<ReadOnlySize>, IMathOperators<ReadOnlySize>
{
    public static readonly ReadOnlySize Invalid = new(double.NaN, double.NaN);
    public static readonly ReadOnlySize Zero    = 0;
    public static readonly ReadOnlySize One     = 1;
    public static readonly ReadOnlySize Two     = 2;
    public static readonly ReadOnlySize Three   = 3;
    public static readonly ReadOnlySize Four    = 4;
    public static readonly ReadOnlySize Five    = 5;
    public static readonly ReadOnlySize Six     = 6;
    public static readonly ReadOnlySize Seven   = 7;
    public static readonly ReadOnlySize Eight   = 8;
    public static readonly ReadOnlySize Nine    = 9;
    public static readonly ReadOnlySize Ten     = 10;
    public readonly        double       Height  = height;
    public readonly        double       Width   = width;


    public static       Sorter<ReadOnlySize>              Sorter      => Sorter<ReadOnlySize>.Default;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Zero        => ref Zero;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Invalid     => ref Invalid;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.One         => ref One;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Two         => ref Two;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Three       => ref Three;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Four        => ref Four;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Five        => ref Five;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Six         => ref Six;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Seven       => ref Seven;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Eight       => ref Eight;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Nine        => ref Nine;
    static ref readonly ReadOnlySize IShape<ReadOnlySize>.Ten         => ref Ten;
    public              bool                              IsValid     => IsNaN is false && IsEmpty is false;
    [JsonIgnore] public bool                              IsEmpty     => IsNaN || Width < 0 || Height < 0;
    public              bool                              IsLandscape => Width < Height;
    public              bool                              IsNaN       => double.IsNaN(Width) || double.IsNaN(Height);
    public              bool                              IsPortrait  => Width > Height;
    double IShapeSize.                                    Width       => Width;
    double IShapeSize.                                    Height      => Height;


    [Pure] public static ReadOnlySize Create( float  width, float  height ) => new(width, height);
    [Pure] public static ReadOnlySize Create( double width, double height ) => new(width, height);
    [Pure] public        ReadOnlySize Reverse() => new(Height, Width);
    [Pure] public        ReadOnlySize Round()   => new(Width.Round(), Height.Round());
    [Pure] public        ReadOnlySize Floor()   => new(Width.Floor(), Height.Floor());


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


    public static bool operator ==( ReadOnlySize        left, ReadOnlySize                     value ) => Sorter.Equals(left, value);
    public static bool operator !=( ReadOnlySize        left, ReadOnlySize                     value ) => Sorter.DoesNotEqual(left, value);
    public static bool operator >( ReadOnlySize         left, ReadOnlySize                     value ) => Sorter.GreaterThan(left, value);
    public static bool operator >=( ReadOnlySize        left, ReadOnlySize                     value ) => Sorter.GreaterThanOrEqualTo(left, value);
    public static bool operator <( ReadOnlySize         left, ReadOnlySize                     value ) => Sorter.LessThan(left, value);
    public static bool operator <=( ReadOnlySize        left, ReadOnlySize                     value ) => Sorter.LessThanOrEqualTo(left, value);
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
