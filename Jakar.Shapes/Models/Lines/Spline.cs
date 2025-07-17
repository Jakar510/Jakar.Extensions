// Jakar.Extensions :: Jakar.Shapes
// 07/12/2025  19:51

using System.Collections;
using System.Text;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes;


[DefaultValue(nameof(Invalid))]
public readonly struct Spline( params ReadOnlyPoint[]? points ) : IShape<Spline>, IStructuralComparable, IValueEnumerable<FromArray<ReadOnlyPoint>, ReadOnlyPoint>
{
    public static readonly  Spline          Invalid = new(null);
    public static readonly  Spline          Zero    = new(ReadOnlyPoint.Zero);
    public static readonly  Spline          One     = new(ReadOnlyPoint.One);
    private static readonly ReadOnlyPoint[] _empty  = [];
    public readonly         ReadOnlyPoint[] Points  = points ?? _empty;


    public ref ReadOnlyPoint this[ int index ] => ref Points[index];


    public static implicit operator Spline( ReadOnlyPoint[]? points ) => new(points);


    public static       Sorter<Spline>              Sorter  => Sorter<Spline>.Default;
    static ref readonly Spline IShape<Spline>.      Zero    => ref Zero;
    static ref readonly Spline IShape<Spline>.      One     => ref One;
    static ref readonly Spline IShape<Spline>.      Invalid => ref Invalid;
    [JsonIgnore] public ReadOnlySpan<ReadOnlyPoint> Span    => Points;
    public              int                         Length  => Points.Length;
    public              bool                        IsEmpty => Points.Length is 0 or 1;
    public bool IsNaN
    {
        get
        {
            ReadOnlySpan<ReadOnlyPoint> span = Span;
            return span.Any(static ( ref readonly ReadOnlyPoint x ) => x.IsNaN);
        }
    }
    public bool IsValid => IsEmpty is false && IsNaN is false;


    public static implicit operator Spline( int    other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Spline( long   other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Spline( float  other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator Spline( double other ) => new(ReadOnlyPoint.Zero, other);


    [Pure] public static Spline                                                   Create( params ReadOnlyPoint[]? points ) => new(points);
    [Pure] public        Spline                                                   Round()                                  => new(Points.AsValueEnumerable().Select(static x => x.Round()).ToArray());
    [Pure] public        Spline                                                   Floor()                                  => new(Points.AsValueEnumerable().Select(static x => x.Floor()).ToArray());
    [Pure] public        ValueEnumerable<FromArray<ReadOnlyPoint>, ReadOnlyPoint> AsValueEnumerable()                      => new(new FromArray<ReadOnlyPoint>(Points));


    public int CompareTo( object? other, IComparer comparer ) => other is Spline spline
                                                                     ? CompareTo(spline)
                                                                     : throw new ExpectedValueTypeException(other, typeof(Spline));
    public int CompareTo( Spline other )
    {
        int lengthComparison = Length.CompareTo(other.Length);
        if ( lengthComparison != 0 ) { return lengthComparison; }

        for ( int i = 0; i < Length; i++ )
        {
            int pointComparison = Points[i].CompareTo(other.Points[i]);
            if ( pointComparison != 0 ) { return pointComparison; }
        }

        return 0;
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Spline spline
                   ? CompareTo(spline)
                   : throw new ExpectedValueTypeException(other, typeof(Spline));
    }
    public bool Equals( Spline other )
    {
        if ( Length.Equals(other.Length) is false ) { return false; }

        for ( int i = 0; i < Length; i++ )
        {
            if ( Points[i].Equals(other.Points[i]) is false ) { return false; }
        }

        return true;
    }
    public override bool   Equals( object? other ) => other is Spline x && Equals(x);
    public override int    GetHashCode()           => Points.GetHashCode();
    public override string ToString()              => ToString(null, null);
    public string ToString( string? format, IFormatProvider? formatProvider )
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return this.ToJson();

            case ",":
            {
                StringBuilder sb     = new();
                int           length = 0;

                foreach ( ref readonly ReadOnlyPoint point in Span )
                {
                    sb.Append(point.ToString(format, formatProvider));
                    if ( length++ < Length ) { sb.Append(','); }
                }

                return sb.ToString();
            }

            case "-":
            {
                StringBuilder sb     = new();
                int           length = 0;

                foreach ( ref readonly ReadOnlyPoint point in Span )
                {
                    sb.Append(point.ToString(format, formatProvider));
                    if ( length++ < Length ) { sb.Append('-'); }
                }

                return sb.ToString();
            }

            case EMPTY:
            case null:
            default:
                return $"{nameof(Spline)}<{nameof(Length)}: {Length}>";
        }
    }


    public static bool operator ==( Spline  left, Spline                           right ) => Sorter.Equals(left, right);
    public static bool operator !=( Spline  left, Spline                           right ) => Sorter.DoesNotEqual(left, right);
    public static bool operator >( Spline   left, Spline                           right ) => Sorter.GreaterThan(left, right);
    public static bool operator >=( Spline  left, Spline                           right ) => Sorter.GreaterThanOrEqualTo(left, right);
    public static bool operator <( Spline   left, Spline                           right ) => Sorter.LessThan(left, right);
    public static bool operator <=( Spline  left, Spline                           right ) => Sorter.LessThanOrEqualTo(left, right);
    public static Spline operator +( Spline self, int                              other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, float                            other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, double                           other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator -( Spline self, int                              other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, float                            other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, double                           other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator *( Spline self, int                              other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, float                            other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, double                           other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator /( Spline self, int                              other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, float                            other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, double                           other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator +( Spline self, (int xOffset, int yOffset)       other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, (float xOffset, float yOffset)   other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, (double xOffset, double yOffset) other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator -( Spline self, (int xOffset, int yOffset)       other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, (float xOffset, float yOffset)   other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, (double xOffset, double yOffset) other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator *( Spline self, (int xOffset, int yOffset)       other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, (float xOffset, float yOffset)   other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, (double xOffset, double yOffset) other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator /( Spline self, (int xOffset, int yOffset)       other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, (float xOffset, float yOffset)   other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, (double xOffset, double yOffset) other ) => Shapes.Create<ReadOnlyPoint>(in self.Points, ( ref readonly ReadOnlyPoint x ) => x / other);
}
