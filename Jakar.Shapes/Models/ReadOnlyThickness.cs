// TrueLogic :: TrueLogic.Common
// 10/22/2024  13:10

namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly partial struct ReadOnlyThickness( double left, double top, double right, double bottom ) : IThickness<ReadOnlyThickness>, IMathOperators<ReadOnlyThickness>
{
    public static readonly ReadOnlyThickness Invalid             = new(double.NaN, double.NaN, double.NaN, double.NaN);
    public static readonly ReadOnlyThickness Zero                = new(0);
    public static readonly ReadOnlyThickness One                 = 1;
    public readonly        double            Bottom              = bottom;
    public readonly        double            Left                = left;
    public readonly        double            Right               = right;
    public readonly        double            Top                 = top;
    public readonly        double            HorizontalThickness = left + right;
    public readonly        double            VerticalThickness   = top  + bottom;


    static ref readonly ReadOnlyThickness IShape<ReadOnlyThickness>.Zero                => ref Zero;
    static ref readonly ReadOnlyThickness IShape<ReadOnlyThickness>.Invalid             => ref Invalid;
    static ref readonly ReadOnlyThickness IShape<ReadOnlyThickness>.One                 => ref One;
    public              bool                                        IsEmpty             => Left == 0 && Top == 0 && Right == 0 && Bottom == 0;
    public              bool                                        IsNaN               => double.IsNaN(Left) || double.IsNaN(Top) || double.IsNaN(Right) || double.IsNaN(Bottom);
    public              bool                                        IsValid             => !IsNaN;
    double IThickness<ReadOnlyThickness>.                           Bottom              => Bottom;
    double IThickness<ReadOnlyThickness>.                           Left                => Left;
    double IThickness<ReadOnlyThickness>.                           Right               => Right;
    double IThickness<ReadOnlyThickness>.                           Top                 => Top;
    double IThickness<ReadOnlyThickness>.                           HorizontalThickness => HorizontalThickness;
    double IThickness<ReadOnlyThickness>.                           VerticalThickness   => VerticalThickness;


    public ReadOnlyThickness( double uniformSize ) : this(uniformSize, uniformSize, uniformSize, uniformSize) { }
    public ReadOnlyThickness( double horizontalSize, double verticalSize ) : this(horizontalSize / 2, verticalSize / 2, horizontalSize / 2, verticalSize / 2) { }


    public void Deconstruct( out float  left,             out float  top, out float right, out float bottom ) => ( left, top, right, bottom ) = ( Left.AsFloat(), Top.AsFloat(), Right.AsFloat(), Bottom.AsFloat() );
    public void Deconstruct( out float  horizontalMargin, out float  verticalMargin )                           => ( horizontalMargin, verticalMargin ) = ( HorizontalThickness.AsFloat(), VerticalThickness.AsFloat() );
    public void Deconstruct( out double left,             out double top, out double right, out double bottom ) => ( left, top, right, bottom ) = ( Left, Top, Right, Bottom );
    public void Deconstruct( out double horizontalMargin, out double verticalMargin ) => ( horizontalMargin, verticalMargin ) = ( HorizontalThickness, VerticalThickness );


    public static implicit operator ReadOnlySize( ReadOnlyThickness size )        => new(size.HorizontalThickness, size.VerticalThickness);
    public static implicit operator ReadOnlyThickness( ReadOnlySize size )        => new(size.Width, size.Height);
    public static implicit operator ReadOnlyThickness( double       uniformSize ) => new(uniformSize);


    public int CompareTo( ReadOnlyThickness other )
    {
        int bottomComparison = Bottom.CompareTo(other.Bottom);
        if ( bottomComparison != 0 ) { return bottomComparison; }

        int leftComparison = Left.CompareTo(other.Left);
        if ( leftComparison != 0 ) { return leftComparison; }

        int rightComparison = Right.CompareTo(other.Right);
        if ( rightComparison != 0 ) { return rightComparison; }

        return Top.CompareTo(other.Top);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyThickness other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlyThickness)}");
    }
    public          bool   Equals( ReadOnlyThickness other )                           => Bottom.Equals(other.Bottom)    && Left.Equals(other.Left) && Right.Equals(other.Right) && Top.Equals(other.Top);
    public override bool   Equals( object?           obj )                             => obj is ReadOnlyThickness other && Equals(other);
    public override int    GetHashCode()                                               => HashCode.Combine(Bottom, Left, Right, Top);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IThickness<ReadOnlyThickness>.ToString(in this, format);


    public static bool operator ==( ReadOnlyThickness?            left, ReadOnlyThickness?               right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyThickness?            left, ReadOnlyThickness?               right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyThickness             left, ReadOnlyThickness                right ) => EqualityComparer<ReadOnlyThickness>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyThickness             left, ReadOnlyThickness                right ) => !EqualityComparer<ReadOnlyThickness>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyThickness              left, ReadOnlyThickness                right ) => Comparer<ReadOnlyThickness>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyThickness             left, ReadOnlyThickness                right ) => Comparer<ReadOnlyThickness>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyThickness              left, ReadOnlyThickness                right ) => Comparer<ReadOnlyThickness>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyThickness             left, ReadOnlyThickness                right ) => Comparer<ReadOnlyThickness>.Default.Compare(left, right) <= 0;
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, ReadOnlyThickness                value ) => new(self.Left + value.Left, self.Top + value.Top, self.Right + value.Right, self.Bottom + value.Bottom);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, ReadOnlyThickness                value ) => new(self.Left - value.Left, self.Top - value.Top, self.Right - value.Right, self.Bottom - value.Bottom);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, ReadOnlyThickness                value ) => new(self.Left * value.Left, self.Top * value.Top, self.Right * -value.Right, self.Bottom * value.Bottom);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, ReadOnlyThickness                value ) => new(self.Left / value.Left, self.Top / value.Top, self.Right / value.Right, self.Bottom  / value.Bottom);
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, double                           value ) => new(self.Left + value, self.Top + value, self.Right + value, self.Bottom + value);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, double                           value ) => new(self.Left - value, self.Top - value, self.Right - value, self.Bottom - value);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, double                           value ) => new(self.Left * value, self.Top * value, self.Right * value, self.Bottom * value);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, double                           value ) => new(self.Left / value, self.Top / value, self.Right / value, self.Bottom / value);
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, float                            value ) => new(self.Left + value, self.Top + value, self.Right + value, self.Bottom + value);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, float                            value ) => new(self.Left - value, self.Top - value, self.Right - value, self.Bottom - value);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, float                            value ) => new(self.Left * value, self.Top * value, self.Right * value, self.Bottom * value);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, float                            value ) => new(self.Left / value, self.Top / value, self.Right / value, self.Bottom / value);
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, int                              value ) => new(self.Left + value, self.Top + value, self.Right + value, self.Bottom + value);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, int                              value ) => new(self.Left - value, self.Top - value, self.Right - value, self.Bottom - value);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, int                              value ) => new(self.Left * value, self.Top * value, self.Right * value, self.Bottom * value);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, int                              value ) => new(self.Left / value, self.Top / value, self.Right / value, self.Bottom / value);
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, (double xOffset, double yOffset) value ) => new(self.Left + value.xOffset, self.Top + value.yOffset, self.Right + value.xOffset, self.Bottom + value.yOffset);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, (double xOffset, double yOffset) value ) => new(self.Left - value.xOffset, self.Top - value.yOffset, self.Right - value.xOffset, self.Bottom - value.yOffset);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, (double xOffset, double yOffset) value ) => new(self.Left * value.xOffset, self.Top * value.yOffset, self.Right * value.xOffset, self.Bottom * value.yOffset);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, (double xOffset, double yOffset) value ) => new(self.Left / value.xOffset, self.Top / value.yOffset, self.Right / value.xOffset, self.Bottom / value.yOffset);
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, (float xOffset, float yOffset)   value ) => new(self.Left + value.xOffset, self.Top + value.yOffset, self.Right + value.xOffset, self.Bottom + value.yOffset);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, (float xOffset, float yOffset)   value ) => new(self.Left - value.xOffset, self.Top - value.yOffset, self.Right - value.xOffset, self.Bottom - value.yOffset);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, (float xOffset, float yOffset)   value ) => new(self.Left / value.xOffset, self.Top / value.yOffset, self.Right / value.xOffset, self.Bottom / value.yOffset);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, (float xOffset, float yOffset)   value ) => new(self.Left * value.xOffset, self.Top * value.yOffset, self.Right * value.xOffset, self.Bottom * value.yOffset);
    public static ReadOnlyThickness operator +( ReadOnlyThickness self, (int xOffset, int yOffset)       value ) => new(self.Left + value.xOffset, self.Top + value.yOffset, self.Right + value.xOffset, self.Bottom + value.yOffset);
    public static ReadOnlyThickness operator -( ReadOnlyThickness self, (int xOffset, int yOffset)       value ) => new(self.Left - value.xOffset, self.Top - value.yOffset, self.Right - value.xOffset, self.Bottom - value.yOffset);
    public static ReadOnlyThickness operator /( ReadOnlyThickness self, (int xOffset, int yOffset)       value ) => new(self.Left / value.xOffset, self.Top / value.yOffset, self.Right / value.xOffset, self.Bottom / value.yOffset);
    public static ReadOnlyThickness operator *( ReadOnlyThickness self, (int xOffset, int yOffset)       value ) => new(self.Left * value.xOffset, self.Top * value.yOffset, self.Right * value.xOffset, self.Bottom * value.yOffset);



    [JsonSourceGenerationOptions(WriteIndented = true), JsonSerializable(typeof(ReadOnlyThickness))] public partial class Context : JsonSerializerContext;
}
