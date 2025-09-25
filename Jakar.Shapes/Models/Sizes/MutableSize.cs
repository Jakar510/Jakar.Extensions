// Jakar.Extensions :: Jakar.Extensions
// 07/11/2025  15:17

using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public struct MutableSize( double width, double height ) : ISize<MutableSize>
{
    public static readonly MutableSize Invalid = new(double.NaN, double.NaN);
    public static readonly MutableSize Zero    = 0;
    public static readonly MutableSize One     = 1;


    static ref readonly          MutableSize IShape<MutableSize>.Zero        => ref Zero;
    static ref readonly          MutableSize IShape<MutableSize>.Invalid     => ref Invalid;
    static ref readonly          MutableSize IShape<MutableSize>.One         => ref One;
    public                       bool                            IsLandscape => Width < Height;
    public                       bool                            IsPortrait  => Width > Height;
    public                       double                          Width       { get; set; } = width;
    public                       double                          Height      { get; set; } = height;
    public readonly              bool                            IsNaN       => double.IsNaN(Width) || double.IsNaN(Height);
    public readonly              bool                            IsValid     => !IsNaN && !IsEmpty;
    [JsonIgnore] public readonly bool                            IsEmpty     => IsNaN || Width < 0 || Height < 0;


    public static implicit operator ReadOnlySize( MutableSize  rect )  => new(rect.Width, rect.Height);
    public static implicit operator ReadOnlySizeF( MutableSize rect )  => new((float)rect.Width, (float)rect.Height);
    public static implicit operator Size( MutableSize          rect )  => new((int)rect.Width, (int)rect.Height);
    public static implicit operator SizeF( MutableSize         rect )  => new((float)rect.Width, (float)rect.Height);
    public static implicit operator MutableSize( Size          rect )  => new(rect.Width, rect.Height);
    public static implicit operator MutableSize( SizeF         rect )  => new(rect.Width, rect.Height);
    public static implicit operator MutableSize( ReadOnlySize  rect )  => new(rect.Width, rect.Height);
    public static implicit operator MutableSize( ReadOnlySizeF rect )  => new(rect.Width, rect.Height);
    public static implicit operator MutableSize( int           value ) => new(value, value);
    public static implicit operator MutableSize( long          value ) => new(value, value);
    public static implicit operator MutableSize( float         value ) => new(value, value);
    public static implicit operator MutableSize( double        value ) => new(value, value);


    [Pure]
    public static MutableSize Min( params ReadOnlySpan<ReadOnlySize> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableSize result = Zero;

        foreach ( ref readonly ReadOnlySize size in points )
        {
            result.Width  = Math.Min(result.Width,  size.Width);
            result.Height = Math.Min(result.Height, size.Height);
        }

        return result;
    }
    [Pure]
    public static MutableSize Min( params ReadOnlySpan<ReadOnlySizeF> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableSize result = Zero;

        foreach ( ref readonly ReadOnlySizeF size in points )
        {
            result.Width  = Math.Min(result.Width,  size.Width);
            result.Height = Math.Min(result.Height, size.Height);
        }

        return result;
    }


    [Pure]
    public static MutableSize Max( params ReadOnlySpan<ReadOnlySize> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableSize result = Zero;

        foreach ( ref readonly ReadOnlySize size in points )
        {
            result.Width  = Math.Max(result.Width,  size.Width);
            result.Height = Math.Max(result.Height, size.Height);
        }

        return result;
    }
    [Pure]
    public static MutableSize Max( params ReadOnlySpan<ReadOnlySizeF> points )
    {
        if ( points.Length <= 0 ) { return Zero; }

        MutableSize result = Zero;

        foreach ( ref readonly ReadOnlySizeF size in points )
        {
            result.Width  = Math.Max(result.Width,  size.Width);
            result.Height = Math.Max(result.Height, size.Height);
        }

        return result;
    }


    [Pure] public static MutableSize Create( double               width, double height ) => new(width, height);
    [Pure] public static MutableSize Create( in ReadOnlySize      size )    => new(size.Width, size.Height);
    [Pure] public static MutableSize Create( in ReadOnlySizeF     size )    => new(size.Width, size.Height);
    [Pure] public static MutableSize Create( in ReadOnlyThickness padding ) => new(padding.HorizontalThickness, padding.VerticalThickness);


    public readonly int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is MutableSize x
                   ? CompareTo(x)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(MutableSize));
    }
    public readonly int CompareTo( MutableSize other )
    {
        int widthComparison = Width.CompareTo(other.Width);
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo(other.Height);
    }
    public readonly bool   Equals( MutableSize other )                                 => Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool   Equals( object?     other )                                 => other is MutableSize x    && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(Width, Height);
    public override string ToString()                                                  => ToString(null, null);
    public readonly string ToString( string? format, IFormatProvider? formatProvider ) => ISize<MutableSize>.ToString(in this, format);


    [Pure] public readonly bool IsAtLeast( in ReadOnlySize  other ) => other.Width <= Width && other.Height <= Height;
    [Pure] public readonly bool IsAtLeast( in ReadOnlySizeF other ) => other.Width <= Width && other.Height <= Height;


    public MutableSize Round()
    {
        Width  = Width.Round();
        Height = Height.Round();
        return this;
    }
    public MutableSize Floor()
    {
        Width  = Width.Floor();
        Height = Height.Floor();
        return this;
    }
    public MutableSize Reverse()
    {
        double width  = Width;
        double height = Height;
        Width  = height;
        Height = width;
        return this;
    }


    public readonly void Deconstruct( out double width, out double height )
    {
        width  = Width;
        height = Height;
    }


    public static        bool operator ==( MutableSize?      left, MutableSize?                     right )  => Nullable.Equals(left, right);
    public static        bool operator !=( MutableSize?      left, MutableSize?                     right )  => !Nullable.Equals(left, right);
    public static        bool operator ==( MutableSize       left, MutableSize                      right )  => EqualityComparer<MutableSize>.Default.Equals(left, right);
    public static        bool operator !=( MutableSize       left, MutableSize                      right )  => !EqualityComparer<MutableSize>.Default.Equals(left, right);
    public static        bool operator >( MutableSize        left, MutableSize                      right )  => Comparer<MutableSize>.Default.Compare(left, right) > 0;
    public static        bool operator >=( MutableSize       left, MutableSize                      right )  => Comparer<MutableSize>.Default.Compare(left, right) >= 0;
    public static        bool operator <( MutableSize        left, MutableSize                      right )  => Comparer<MutableSize>.Default.Compare(left, right) < 0;
    public static        bool operator <=( MutableSize       left, MutableSize                      right )  => Comparer<MutableSize>.Default.Compare(left, right) <= 0;
    public static        MutableSize operator +( MutableSize self, MutableSize                      other )  => new(self.Width + other.Width, self.Height + other.Height);
    public static        MutableSize operator -( MutableSize self, MutableSize                      other )  => new(self.Width - other.Width, self.Height - other.Height);
    public static        MutableSize operator *( MutableSize self, MutableSize                      other )  => new(self.Width * other.Width, self.Height * other.Height);
    public static        MutableSize operator /( MutableSize self, MutableSize                      other )  => new(self.Width / other.Width, self.Height / other.Height);
    public static        MutableSize operator +( MutableSize self, ReadOnlySize                     other )  => new(self.Width + other.Width, self.Height + other.Height);
    public static        MutableSize operator -( MutableSize self, ReadOnlySize                     other )  => new(self.Width - other.Width, self.Height - other.Height);
    public static        MutableSize operator *( MutableSize self, ReadOnlySize                     other )  => new(self.Width * other.Width, self.Height * other.Height);
    public static        MutableSize operator /( MutableSize self, ReadOnlySize                     other )  => new(self.Width / other.Width, self.Height / other.Height);
    public static        MutableSize operator +( MutableSize self, ReadOnlySizeF                    other )  => new(self.Width + other.Width, self.Height + other.Height);
    public static        MutableSize operator -( MutableSize self, ReadOnlySizeF                    other )  => new(self.Width - other.Width, self.Height - other.Height);
    public static        MutableSize operator *( MutableSize self, ReadOnlySizeF                    other )  => new(self.Width * other.Width, self.Height * other.Height);
    public static        MutableSize operator /( MutableSize self, ReadOnlySizeF                    other )  => new(self.Width / other.Width, self.Height / other.Height);
    public static        MutableSize operator +( MutableSize self, SizeF                            other )  => new(self.Width + other.Width, self.Height + other.Height);
    public static        MutableSize operator -( MutableSize self, SizeF                            other )  => new(self.Width - other.Width, self.Height - other.Height);
    public static        MutableSize operator *( MutableSize self, SizeF                            other )  => new(self.Width * other.Width, self.Height * other.Height);
    public static        MutableSize operator /( MutableSize self, SizeF                            other )  => new(self.Width / other.Width, self.Height / other.Height);
    public static        MutableSize operator +( MutableSize self, Size                             other )  => new(self.Width + other.Width, self.Height + other.Height);
    public static        MutableSize operator -( MutableSize self, Size                             other )  => new(self.Width - other.Width, self.Height - other.Height);
    public static        MutableSize operator *( MutableSize self, Size                             other )  => new(self.Width * other.Width, self.Height * other.Height);
    public static        MutableSize operator /( MutableSize self, Size                             other )  => new(self.Width / other.Width, self.Height / other.Height);
    [Pure] public static MutableSize operator +( MutableSize self, ReadOnlyThickness                margin ) => new(self.Width + margin.HorizontalThickness, self.Height + margin.VerticalThickness);
    [Pure] public static MutableSize operator -( MutableSize self, ReadOnlyThickness                margin ) => new(self.Width - margin.HorizontalThickness, self.Height - margin.VerticalThickness);
    public static        MutableSize operator +( MutableSize self, int                              value )  => new(self.Width + value, self.Height                      + value);
    public static        MutableSize operator +( MutableSize self, float                            value )  => new(self.Width + value, self.Height                      + value);
    public static        MutableSize operator +( MutableSize self, double                           value )  => new(self.Width + value, self.Height                      + value);
    public static        MutableSize operator -( MutableSize self, int                              value )  => new(self.Width - value, self.Height                      - value);
    public static        MutableSize operator -( MutableSize self, float                            value )  => new(self.Width - value, self.Height                      - value);
    public static        MutableSize operator -( MutableSize self, double                           value )  => new(self.Width - value, self.Height                      - value);
    public static        MutableSize operator *( MutableSize self, int                              value )  => new(self.Width * value, self.Height * value);
    public static        MutableSize operator *( MutableSize self, float                            value )  => new(self.Width * value, self.Height * value);
    public static        MutableSize operator *( MutableSize self, double                           value )  => new(self.Width * value, self.Height * value);
    public static        MutableSize operator /( MutableSize self, int                              value )  => new(self.Width / value, self.Height / value);
    public static        MutableSize operator /( MutableSize self, float                            value )  => new(self.Width / value, self.Height / value);
    public static        MutableSize operator /( MutableSize self, double                           value )  => new(self.Width / value, self.Height / value);
    public static        MutableSize operator +( MutableSize self, (int xOffset, int yOffset)       value )  => new(self.Width + value.xOffset, self.Height + value.yOffset);
    public static        MutableSize operator +( MutableSize self, (float xOffset, float yOffset)   value )  => new(self.Width + value.xOffset, self.Height + value.yOffset);
    public static        MutableSize operator +( MutableSize self, (double xOffset, double yOffset) value )  => new(self.Width + value.xOffset, self.Height + value.yOffset);
    public static        MutableSize operator -( MutableSize self, (int xOffset, int yOffset)       value )  => new(self.Width - value.xOffset, self.Height - value.yOffset);
    public static        MutableSize operator -( MutableSize self, (float xOffset, float yOffset)   value )  => new(self.Width - value.xOffset, self.Height - value.yOffset);
    public static        MutableSize operator -( MutableSize self, (double xOffset, double yOffset) value )  => new(self.Width - value.xOffset, self.Height - value.yOffset);
    public static        MutableSize operator *( MutableSize self, (int xOffset, int yOffset)       value )  => new(self.Width * value.xOffset, self.Height * value.yOffset);
    public static        MutableSize operator *( MutableSize self, (float xOffset, float yOffset)   value )  => new(self.Width * value.xOffset, self.Height * value.yOffset);
    public static        MutableSize operator *( MutableSize self, (double xOffset, double yOffset) value )  => new(self.Width * value.xOffset, self.Height * value.yOffset);
    public static        MutableSize operator /( MutableSize self, (int xOffset, int yOffset)       value )  => new(self.Width / value.xOffset, self.Height / value.yOffset);
    public static        MutableSize operator /( MutableSize self, (float xOffset, float yOffset)   value )  => new(self.Width / value.xOffset, self.Height / value.yOffset);
    public static        MutableSize operator /( MutableSize self, (double xOffset, double yOffset) value )  => new(self.Width / value.xOffset, self.Height / value.yOffset);
    public static        JsonSerializerContext       JsonContext   => JakarShapesContext.Default;
    public static        JsonTypeInfo<MutableSize>   JsonTypeInfo  => JakarShapesContext.Default.MutableSize;
    public static        JsonTypeInfo<MutableSize[]> JsonArrayInfo => JakarShapesContext.Default.MutableSizeArray;
    public static        bool                        TryFromJson( string? json, out MutableSize result )
    {
        result = default;
        return false;
    }
    public static MutableSize FromJson( string json ) => default;
}
