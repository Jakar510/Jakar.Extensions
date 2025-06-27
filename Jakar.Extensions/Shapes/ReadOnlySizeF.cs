// TrueLogic :: TrueLogic.Common.Maui
// 01/20/2025  08:01

namespace Jakar.Extensions;


[DefaultValue( nameof(Zero) )]
public readonly struct ReadOnlySizeF( float width, float height ) : ISize<ReadOnlySizeF, float>, IMathOperators<ReadOnlySizeF>
{
    public static readonly ReadOnlySizeF Invalid = new(float.NaN, float.NaN);
    public static readonly ReadOnlySizeF Zero    = new(0, 0);
    public readonly        float         Height  = height;
    public readonly        float         Width   = width;


    public static Sorter<ReadOnlySizeF>                      Sorter      => Sorter<ReadOnlySizeF>.Default;
    static        ReadOnlySizeF IGenericShape<ReadOnlySizeF>.Zero        => Zero;
    static        ReadOnlySizeF IGenericShape<ReadOnlySizeF>.Invalid     => Invalid;
    public        bool                                       IsValid     => IsNaN is false;
    public        bool                                       IsEmpty     => Width == 0 && Height == 0;
    public        bool                                       IsLandscape => Width < Height;
    public        bool                                       IsNaN       => float.IsNaN( Width ) || float.IsNaN( Height );
    public        bool                                       IsPortrait  => Width > Height;
    float IShapeSize<float>.                                 Width       => Width;
    float IShapeSize<float>.                                 Height      => Height;


    public static implicit operator Size( ReadOnlySizeF         rectangle ) => new((int)rectangle.Width, (int)rectangle.Height);
    public static implicit operator SizeF( ReadOnlySizeF        rectangle ) => new(rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlySize( ReadOnlySizeF rectangle ) => new(rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlySizeF( ReadOnlySize size )      => new((float)size.Width, (float)size.Height);
    public static implicit operator ReadOnlySizeF( Size         size )      => new(size.Width, size.Height);
    public static implicit operator ReadOnlySizeF( SizeF        size )      => new(size.Width, size.Height);


    [Pure] public static ReadOnlySizeF Create( float x, float y ) => new(x, y);
    [Pure] public        ReadOnlySizeF Reverse() => new(Height, Width);
    [Pure] public        ReadOnlySizeF Round()   => new(Width.Round(), Height.Round());
    [Pure] public        ReadOnlySizeF Floor()   => new(Width.Floor(), Height.Floor());


    public int CompareTo( ReadOnlySizeF other )
    {
        int widthComparison = Width.CompareTo( other.Width );
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo( other.Height );
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlySizeF other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(ReadOnlySizeF)}" );
    }
    public          bool   Equals( ReadOnlySizeF other )                               => Height.Equals( other.Height ) && Width.Equals( other.Width );
    public override bool   Equals( object?       obj )                                 => obj is ReadOnlySizeF other    && Equals( other );
    public override int    GetHashCode()                                               => HashCode.Combine( Height, Width );
    public override string ToString()                                                  => ToString( null, null );
    public          string ToString( string? format, IFormatProvider? formatProvider ) => ISize<ReadOnlySizeF, float>.ToString( this, format );


    public static bool operator ==( ReadOnlySizeF         left, ReadOnlySizeF                    value ) => Sorter.Equals( left, value );
    public static bool operator !=( ReadOnlySizeF         left, ReadOnlySizeF                    value ) => Sorter.DoesNotEqual( left, value );
    public static bool operator >( ReadOnlySizeF          left, ReadOnlySizeF                    value ) => Sorter.GreaterThan( left, value );
    public static bool operator >=( ReadOnlySizeF         left, ReadOnlySizeF                    value ) => Sorter.GreaterThanOrEqualTo( left, value );
    public static bool operator <( ReadOnlySizeF          left, ReadOnlySizeF                    value ) => Sorter.LessThan( left, value );
    public static bool operator <=( ReadOnlySizeF         left, ReadOnlySizeF                    value ) => Sorter.LessThanOrEqualTo( left, value );
    public static ReadOnlySizeF operator +( ReadOnlySizeF size, Size                             value ) => new(size.Width + value.Width, size.Height             + value.Height);
    public static ReadOnlySizeF operator +( ReadOnlySizeF size, SizeF                            value ) => new(size.Width + value.Width, size.Height             + value.Height);
    public static ReadOnlySizeF operator +( ReadOnlySizeF size, ReadOnlySizeF                    value ) => new(size.Width + value.Width, size.Height             + value.Height);
    public static ReadOnlySizeF operator +( ReadOnlySizeF size, (int xOffset, int yOffset)       value ) => new(size.Width + value.xOffset, size.Height           + value.yOffset);
    public static ReadOnlySizeF operator +( ReadOnlySizeF size, (float xOffset, float yOffset)   value ) => new(size.Width + value.xOffset, size.Height           + value.yOffset);
    public static ReadOnlySizeF operator +( ReadOnlySizeF size, (double xOffset, double yOffset) value ) => new(size.Width + value.xOffset.AsFloat(), size.Height + value.yOffset.AsFloat());
    public static ReadOnlySizeF operator -( ReadOnlySizeF size, Size                             value ) => new(size.Width - value.Width, size.Height             - value.Height);
    public static ReadOnlySizeF operator -( ReadOnlySizeF size, SizeF                            value ) => new(size.Width - value.Width, size.Height             - value.Height);
    public static ReadOnlySizeF operator -( ReadOnlySizeF size, ReadOnlySizeF                    value ) => new(size.Width - value.Width, size.Height             - value.Height);
    public static ReadOnlySizeF operator -( ReadOnlySizeF size, (int xOffset, int yOffset)       value ) => new(size.Width - value.xOffset, size.Height           - value.yOffset);
    public static ReadOnlySizeF operator -( ReadOnlySizeF size, (float xOffset, float yOffset)   value ) => new(size.Width - value.xOffset, size.Height           - value.yOffset);
    public static ReadOnlySizeF operator -( ReadOnlySizeF size, (double xOffset, double yOffset) value ) => new(size.Width - value.xOffset.AsFloat(), size.Height - value.yOffset.AsFloat());
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, ReadOnlySizeF                    value ) => new(size.Width * value.Width, size.Height             * value.Height);
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, int                              value ) => new(size.Width * value, size.Height                   * value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, float                            value ) => new(size.Width * value, size.Height                   * value);
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, double                           value ) => new(size.Width * value.AsFloat(), size.Height         * value.AsFloat());
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, (int xOffset, int yOffset)       value ) => new(size.Width * value.xOffset, size.Height           * value.yOffset);
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, (float xOffset, float yOffset)   value ) => new(size.Width * value.xOffset, size.Height           * value.yOffset);
    public static ReadOnlySizeF operator *( ReadOnlySizeF size, (double xOffset, double yOffset) value ) => new(size.Width * value.xOffset.AsFloat(), size.Height * value.yOffset.AsFloat());
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, ReadOnlySizeF                    value ) => new(size.Width / value.Width, size.Height             / value.Height);
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, int                              value ) => new(size.Width / value, size.Height                   / value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, float                            value ) => new(size.Width / value, size.Height                   / value);
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, double                           value ) => new(size.Width / value.AsFloat(), size.Height         / value.AsFloat());
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, (int xOffset, int yOffset)       value ) => new(size.Width / value.xOffset, size.Height           / value.yOffset);
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, (float xOffset, float yOffset)   value ) => new(size.Width / value.xOffset, size.Height           / value.yOffset);
    public static ReadOnlySizeF operator /( ReadOnlySizeF size, (double xOffset, double yOffset) value ) => new(size.Width / value.xOffset.AsFloat(), size.Height / value.yOffset.AsFloat());
    public static ReadOnlySizeF operator +( ReadOnlySizeF left, double                           value ) => new(left.Width + value.AsFloat(), left.Height + value.AsFloat());
    public static ReadOnlySizeF operator -( ReadOnlySizeF left, double                           value ) => new(left.Width - value.AsFloat(), left.Height - value.AsFloat());
    public static ReadOnlySizeF operator +( ReadOnlySizeF left, float                            value ) => new(left.Width + value, left.Height           + value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF left, float                            value ) => new(left.Width - value, left.Height           - value);
    public static ReadOnlySizeF operator +( ReadOnlySizeF left, int                              value ) => new(left.Width + value, left.Height           + value);
    public static ReadOnlySizeF operator -( ReadOnlySizeF left, int                              value ) => new(left.Width - value, left.Height           - value);
}
