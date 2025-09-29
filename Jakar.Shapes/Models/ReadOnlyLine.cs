// Jakar.Extensions :: Jakar.Shapes
// 07/12/2025  19:15


using System.Text.Json;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
[method: JsonConstructor]
public readonly struct ReadOnlyLine( ReadOnlyPoint start, ReadOnlyPoint end, bool isFinite = true ) : ILine<ReadOnlyLine>, IMathOperators<ReadOnlyLine>
{
    public static readonly ReadOnlyLine  Invalid  = new(ReadOnlyPoint.Invalid, ReadOnlyPoint.Invalid);
    public static readonly ReadOnlyLine  Zero     = 0;
    public static readonly ReadOnlyLine  One      = 1;
    public readonly        ReadOnlyPoint Start    = start;
    public readonly        ReadOnlyPoint End      = end;
    public readonly        bool          IsFinite = isFinite;


    public static       JsonSerializerContext             JsonContext   => JakarShapesContext.Default;
    public static       JsonTypeInfo<ReadOnlyLine>        JsonTypeInfo  => JakarShapesContext.Default.ReadOnlyLine;
    public static       JsonTypeInfo<ReadOnlyLine[]>      JsonArrayInfo => JakarShapesContext.Default.ReadOnlyLineArray;
    static ref readonly ReadOnlyLine IShape<ReadOnlyLine>.Zero          => ref Zero;
    static ref readonly ReadOnlyLine IShape<ReadOnlyLine>.One           => ref One;
    static ref readonly ReadOnlyLine IShape<ReadOnlyLine>.Invalid       => ref Invalid;
    bool IValidator.                                      IsValid       => this.IsValid();
    ReadOnlyPoint ILine<ReadOnlyLine, ReadOnlyPoint>.     Start         => Start;
    ReadOnlyPoint ILine<ReadOnlyLine, ReadOnlyPoint>.     End           => End;
    bool ILine<ReadOnlyLine, ReadOnlyPoint>.              IsFinite      => IsFinite;
    public double                                         Length        => Start.DistanceTo(End);


    public static implicit operator ReadOnlyLine( int    other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator ReadOnlyLine( long   other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator ReadOnlyLine( float  other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator ReadOnlyLine( double other ) => new(ReadOnlyPoint.Zero, other);


    [Pure] public static ReadOnlyLine Create( in ReadOnlyPoint start, in ReadOnlyPoint end, bool isFinite = true ) => new(start, end, isFinite);
    public static bool TryFromJson( string? json, out ReadOnlyLine result )
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
    public static ReadOnlyLine FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( ReadOnlyLine other )
    {
        int xOffsetComparison = Start.CompareTo(other.Start);

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : End.CompareTo(other.End);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyLine other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlyLine)}");
    }
    public          bool   Equals( ReadOnlyLine other )                                => Start.Equals(other.Start) && End.Equals(other.End) && IsFinite == other.IsFinite;
    public override bool   Equals( object?      other )                                => other is ReadOnlyLine x   && Equals(x);
    public override int    GetHashCode()                                               => HashCode.Combine(Start, End, IsFinite);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => this.ToString(format);


    public static bool operator ==( ReadOnlyLine?       left, ReadOnlyLine?                    right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyLine?       left, ReadOnlyLine?                    right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyLine        left, ReadOnlyLine                     right ) => EqualityComparer<ReadOnlyLine>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyLine        left, ReadOnlyLine                     right ) => !EqualityComparer<ReadOnlyLine>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyLine         left, ReadOnlyLine                     right ) => Comparer<ReadOnlyLine>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyLine        left, ReadOnlyLine                     right ) => Comparer<ReadOnlyLine>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyLine         left, ReadOnlyLine                     right ) => Comparer<ReadOnlyLine>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyLine        left, ReadOnlyLine                     right ) => Comparer<ReadOnlyLine>.Default.Compare(left, right) <= 0;
    public static ReadOnlyLine operator +( ReadOnlyLine self, ReadOnlyLine                     value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, ReadOnlyLine                     value ) => self.Subtract(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, ReadOnlyLine                     value ) => self.Divide(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, ReadOnlyLine                     value ) => self.Multiply(value);
    public static ReadOnlyLine operator +( ReadOnlyLine self, (int xOffset, int yOffset)       value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, (int xOffset, int yOffset)       value ) => self.Subtract(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, (int xOffset, int yOffset)       value ) => self.Divide(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, (int xOffset, int yOffset)       value ) => self.Multiply(value);
    public static ReadOnlyLine operator +( ReadOnlyLine self, (float xOffset, float yOffset)   value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, (float xOffset, float yOffset)   value ) => self.Subtract(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, (float xOffset, float yOffset)   value ) => self.Multiply(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, (float xOffset, float yOffset)   value ) => self.Divide(value);
    public static ReadOnlyLine operator +( ReadOnlyLine self, (double xOffset, double yOffset) value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, (double xOffset, double yOffset) value ) => self.Subtract(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, (double xOffset, double yOffset) value ) => self.Multiply(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, (double xOffset, double yOffset) value ) => self.Divide(value);
    public static ReadOnlyLine operator +( ReadOnlyLine self, double                           value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, double                           value ) => self.Subtract(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, double                           value ) => self.Divide(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, double                           value ) => self.Multiply(value);
    public static ReadOnlyLine operator +( ReadOnlyLine self, float                            value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, float                            value ) => self.Subtract(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, float                            value ) => self.Divide(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, float                            value ) => self.Multiply(value);
    public static ReadOnlyLine operator +( ReadOnlyLine self, int                              value ) => self.Add(value);
    public static ReadOnlyLine operator -( ReadOnlyLine self, int                              value ) => self.Subtract(value);
    public static ReadOnlyLine operator *( ReadOnlyLine self, int                              value ) => self.Multiply(value);
    public static ReadOnlyLine operator /( ReadOnlyLine self, int                              value ) => self.Divide(value);
    public static ReadOnlyLine operator &( ReadOnlyLine self, ReadOnlyPoint                    other ) => new(self.Start, other);
    public static ReadOnlyLine operator &( ReadOnlyLine self, ReadOnlyPointF                   other ) => new(self.Start, other);
    public static ReadOnlyLine operator ^( ReadOnlyLine self, ReadOnlyPoint                    other ) => new(self.Start, other);
    public static ReadOnlyLine operator ^( ReadOnlyLine self, ReadOnlyPointF                   other ) => new(self.Start, other);
}
