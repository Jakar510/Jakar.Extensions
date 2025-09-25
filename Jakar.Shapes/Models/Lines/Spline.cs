// Jakar.Extensions :: Jakar.Shapes
// 07/12/2025  19:51

using System.Collections;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes;


[DefaultValue(nameof(Invalid))]
[method: JsonConstructor]
public readonly struct Spline( params ReadOnlyPoint[]? points ) : IShape<Spline>, IStructuralComparable, IValueEnumerable<FromArray<ReadOnlyPoint>, ReadOnlyPoint>
{
    public static readonly  Spline          Invalid = new(null);
    public static readonly  Spline          Zero    = new(ReadOnlyPoint.Zero);
    public static readonly  Spline          One     = new(ReadOnlyPoint.One);
    private static readonly ReadOnlyPoint[] __empty = [];
    public readonly         ReadOnlyPoint[] Points  = points ?? __empty;


    public ref ReadOnlyPoint this[ int index ] => ref Points[index];


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
    public bool IsValid => !IsEmpty && !IsNaN;


    public static implicit operator Spline( ReadOnlyPoint[]? points ) => new(points);


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
        if ( !Length.Equals(other.Length) ) { return false; }

        for ( int i = 0; i < Length; i++ )
        {
            if ( !Points[i].Equals(other.Points[i]) ) { return false; }
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


    public static bool operator ==( Spline? left, Spline?                          right ) => Nullable.Equals(left, right);
    public static bool operator !=( Spline? left, Spline?                          right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Spline  left, Spline                           right ) => EqualityComparer<Spline>.Default.Equals(left, right);
    public static bool operator !=( Spline  left, Spline                           right ) => !EqualityComparer<Spline>.Default.Equals(left, right);
    public static bool operator >( Spline   left, Spline                           right ) => Comparer<Spline>.Default.Compare(left, right) > 0;
    public static bool operator >=( Spline  left, Spline                           right ) => Comparer<Spline>.Default.Compare(left, right) >= 0;
    public static bool operator <( Spline   left, Spline                           right ) => Comparer<Spline>.Default.Compare(left, right) < 0;
    public static bool operator <=( Spline  left, Spline                           right ) => Comparer<Spline>.Default.Compare(left, right) <= 0;
    public static Spline operator +( Spline self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator -( Spline self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator *( Spline self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator /( Spline self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator +( Spline self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator +( Spline self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Spline operator -( Spline self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator -( Spline self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Spline operator *( Spline self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator *( Spline self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Spline operator /( Spline self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Spline operator /( Spline self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static JsonSerializerContext  JsonContext   => JakarShapesContext.Default;
    public static JsonTypeInfo<Spline>   JsonTypeInfo  => JakarShapesContext.Default.Spline;
    public static JsonTypeInfo<Spline[]> JsonArrayInfo => JakarShapesContext.Default.SplineArray;
    public static bool                   TryFromJson( string? json, out Spline result )
    {
        result = default;
        return false;
    }
    public static Spline FromJson( string json ) => default;
}
