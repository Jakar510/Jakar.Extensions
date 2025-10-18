// Jakar.Extensions :: Jakar.Shapes
// 10/18/2025  00:34

using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ZLinq;
using ZLinq.Linq;



namespace Jakar.Shapes;


[DefaultValue(nameof(Invalid))]
[method: JsonConstructor]
public readonly struct Polygon( params ReadOnlyPoint[]? points ) : ISpline<Polygon>
{
    public static readonly  Polygon         Invalid = new(null);
    public static readonly  Polygon         Zero    = new(ReadOnlyPoint.Zero);
    public static readonly  Polygon         One     = new(ReadOnlyPoint.One);
    private static readonly ReadOnlyPoint[] __empty = [];
    public readonly         ReadOnlyPoint[] Points  = points ?? __empty;


    public static JsonSerializerContext   JsonContext   => JakarShapesContext.Default;
    public static JsonTypeInfo<Polygon>   JsonTypeInfo  => JakarShapesContext.Default.Polygon;
    public static JsonTypeInfo<Polygon[]> JsonArrayInfo => JakarShapesContext.Default.PolygonArray;
    public ref readonly ReadOnlyPoint this[ int   index ] => ref Points[index];
    public ref readonly ReadOnlyPoint this[ Index index ] => ref Points[index];
    public Spline this[ Range                     index ] { [Pure] get => new(Points[index]); }
    static ref readonly Polygon IShape<Polygon>.                Zero    => ref Zero;
    static ref readonly Polygon IShape<Polygon>.                One     => ref One;
    static ref readonly Polygon IShape<Polygon>.                Invalid => ref Invalid;
    [JsonIgnore] public ReadOnlySpan<ReadOnlyPoint>             Span    => Points;
    public              int                                     Length  => Points.Length;
    ReadOnlySpan<ReadOnlyPoint> ISpline<Polygon, ReadOnlyPoint>.Points  => Points;
    public bool                                                 IsEmpty => Points.Length is 0 or 1;
    public bool IsNaN
    {
        get
        {
            ReadOnlySpan<ReadOnlyPoint> span = Span;
            return span.Any(static ( ref readonly ReadOnlyPoint x ) => x.IsNaN());
        }
    }
    public bool IsValid => !IsEmpty && !IsNaN;


    public static implicit operator Polygon( ReadOnlyPoint[]?               points ) => Create(points);
    [Pure] public static            Polygon Create( params ReadOnlyPoint[]? points ) => new(points);
    [Pure] public Polygon Round() => new(AsValueEnumerable()
                                        .Select(static x => x.Round())
                                        .ToArray());
    [Pure] public Polygon Floor() => new(AsValueEnumerable()
                                        .Select(static x => x.Floor())
                                        .ToArray());


    [Pure] public ValueEnumerable<FromArray<ReadOnlyPoint>, ReadOnlyPoint> AsValueEnumerable() => new(new FromArray<ReadOnlyPoint>(Points));


    public static bool TryFromJson( string? json, out Polygon result )
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
    public static Polygon FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( object? other, IComparer comparer ) => other is Polygon spline
                                                                     ? CompareTo(spline)
                                                                     : throw new ExpectedValueTypeException(other, typeof(Polygon));
    public int CompareTo( Polygon other, IComparer<Polygon> comparer ) => comparer.Compare(this, other);
    public int CompareTo( Polygon other )
    {
        int lengthComparison = Length.CompareTo(other.Length);
        if ( lengthComparison != 0 ) { return lengthComparison; }

        for ( int i = 0; i < Length; i++ )
        {
            int pointComparison = Points[i]
               .CompareTo(other.Points[i]);

            if ( pointComparison != 0 ) { return pointComparison; }
        }

        return 0;
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is Polygon spline
                   ? CompareTo(spline)
                   : throw new ExpectedValueTypeException(other, typeof(Polygon));
    }
    public bool Equals( Polygon other )
    {
        if ( !Length.Equals(other.Length) ) { return false; }

        for ( int i = 0; i < Length; i++ )
        {
            if ( !Points[i]
                    .Equals(other.Points[i]) ) { return false; }
        }

        return true;
    }
    public override bool   Equals( object? other ) => other is Polygon x && Equals(x);
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
                return $"{nameof(Polygon)}<{nameof(Length)}: {Length}>";
        }
    }


    public static bool operator ==( Polygon?  left, Polygon?                         right ) => Nullable.Equals(left, right);
    public static bool operator !=( Polygon?  left, Polygon?                         right ) => !Nullable.Equals(left, right);
    public static bool operator ==( Polygon   left, Polygon                          right ) => EqualityComparer<Polygon>.Default.Equals(left, right);
    public static bool operator !=( Polygon   left, Polygon                          right ) => !EqualityComparer<Polygon>.Default.Equals(left, right);
    public static bool operator >( Polygon    left, Polygon                          right ) => Comparer<Polygon>.Default.Compare(left, right) > 0;
    public static bool operator >=( Polygon   left, Polygon                          right ) => Comparer<Polygon>.Default.Compare(left, right) >= 0;
    public static bool operator <( Polygon    left, Polygon                          right ) => Comparer<Polygon>.Default.Compare(left, right) < 0;
    public static bool operator <=( Polygon   left, Polygon                          right ) => Comparer<Polygon>.Default.Compare(left, right) <= 0;
    public static Polygon operator +( Polygon self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Polygon operator +( Polygon self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Polygon operator +( Polygon self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Polygon operator -( Polygon self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Polygon operator -( Polygon self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Polygon operator -( Polygon self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Polygon operator *( Polygon self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Polygon operator *( Polygon self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Polygon operator *( Polygon self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Polygon operator /( Polygon self, int                              other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Polygon operator /( Polygon self, float                            other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Polygon operator /( Polygon self, double                           other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Polygon operator +( Polygon self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Polygon operator +( Polygon self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Polygon operator +( Polygon self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x + other);
    public static Polygon operator -( Polygon self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Polygon operator -( Polygon self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Polygon operator -( Polygon self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x - other);
    public static Polygon operator *( Polygon self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Polygon operator *( Polygon self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Polygon operator *( Polygon self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x * other);
    public static Polygon operator /( Polygon self, (int xOffset, int yOffset)       other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Polygon operator /( Polygon self, (float xOffset, float yOffset)   other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
    public static Polygon operator /( Polygon self, (double xOffset, double yOffset) other ) => self.Points.Create<ReadOnlyPoint>(( ref readonly ReadOnlyPoint x ) => x / other);
}
