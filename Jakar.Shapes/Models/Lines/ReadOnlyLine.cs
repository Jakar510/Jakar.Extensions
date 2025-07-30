// Jakar.Extensions :: Jakar.Shapes
// 07/12/2025  19:15


namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyLine( ReadOnlyPoint start, ReadOnlyPoint end, bool isFinite = true ) : ILine<ReadOnlyLine>, IMathOperators<ReadOnlyLine>
{
    public static readonly ReadOnlyLine  Invalid  = new(ReadOnlyPoint.Invalid, ReadOnlyPoint.Invalid);
    public static readonly ReadOnlyLine  Zero     = 0;
    public static readonly ReadOnlyLine  One      = 1;
    public readonly        ReadOnlyPoint Start    = start;
    public readonly        ReadOnlyPoint End      = end;
    public readonly        bool          IsFinite = isFinite;


    public static       EqualComparer<ReadOnlyLine>              Sorter   => EqualComparer<ReadOnlyLine>.Default;
    static ref readonly ReadOnlyLine IShape<ReadOnlyLine>.Zero     => ref Zero;
    static ref readonly ReadOnlyLine IShape<ReadOnlyLine>.One      => ref One;
    static ref readonly ReadOnlyLine IShape<ReadOnlyLine>.Invalid  => ref Invalid;
    ReadOnlyPoint ILine<ReadOnlyLine>.                    Start    => Start;
    ReadOnlyPoint ILine<ReadOnlyLine>.                    End      => End;
    bool ILine<ReadOnlyLine>.                             IsFinite => IsFinite;
    public bool                                           IsEmpty  => Start.IsEmpty || End.IsEmpty;
    public bool                                           IsNaN    => Start.IsNaN   || End.IsNaN;
    public bool                                           IsValid  => IsNaN is false && Start != End;
    public double                                         Length   => Start.DistanceTo(in End);


    public static implicit operator ReadOnlyLine( int    other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator ReadOnlyLine( long   other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator ReadOnlyLine( float  other ) => new(ReadOnlyPoint.Zero, other);
    public static implicit operator ReadOnlyLine( double other ) => new(ReadOnlyPoint.Zero, other);


    [Pure] public static ReadOnlyLine Create( in ReadOnlyPoint start, in ReadOnlyPoint end, bool isFinite = true ) => new(start, end, isFinite);
    [Pure] public        ReadOnlyLine Round() => new(Start.Round(), End.Round(), IsFinite);
    [Pure] public        ReadOnlyLine Floor() => new(Start.Floor(), End.Floor(), IsFinite);


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
    public          string ToString( string? format, IFormatProvider? formatProvider ) => ILine<ReadOnlyLine>.ToString(in this, format);


    public static bool operator ==( ReadOnlyLine        left, ReadOnlyLine                     right ) => Sorter.Equals(left, right);
    public static bool operator !=( ReadOnlyLine        left, ReadOnlyLine                     right ) => Sorter.DoesNotEqual(left, right);
    public static bool operator >( ReadOnlyLine         left, ReadOnlyLine                     right ) => Sorter.GreaterThan(left, right);
    public static bool operator >=( ReadOnlyLine        left, ReadOnlyLine                     right ) => Sorter.GreaterThanOrEqualTo(left, right);
    public static bool operator <( ReadOnlyLine         left, ReadOnlyLine                     right ) => Sorter.LessThan(left, right);
    public static bool operator <=( ReadOnlyLine        left, ReadOnlyLine                     right ) => Sorter.LessThanOrEqualTo(left, right);
    public static ReadOnlyLine operator *( ReadOnlyLine self, ReadOnlyLine                     other ) => new(self.Start * other.Start, self.End * other.End);
    public static ReadOnlyLine operator +( ReadOnlyLine self, ReadOnlyLine                     other ) => new(self.Start + other.Start, self.End + other.End);
    public static ReadOnlyLine operator -( ReadOnlyLine self, ReadOnlyLine                     other ) => new(self.Start - other.Start, self.End - other.End);
    public static ReadOnlyLine operator /( ReadOnlyLine self, ReadOnlyLine                     other ) => new(self.Start / other.Start, self.End / other.End);
    public static ReadOnlyLine operator -( ReadOnlyLine self, ReadOnlyPoint                    other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, ReadOnlyPointF                   other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, ReadOnlyPoint                    other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, ReadOnlyPointF                   other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator &( ReadOnlyLine self, ReadOnlyPoint                    other ) => new(other, self.End);
    public static ReadOnlyLine operator &( ReadOnlyLine self, ReadOnlyPointF                   other ) => new(other, self.End);
    public static ReadOnlyLine operator ^( ReadOnlyLine self, ReadOnlyPoint                    other ) => new(self.Start, other);
    public static ReadOnlyLine operator ^( ReadOnlyLine self, ReadOnlyPointF                   other ) => new(self.Start, other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, (int xOffset, int yOffset)       other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, (float xOffset, float yOffset)   other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, (double xOffset, double yOffset) other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, (int xOffset, int yOffset)       other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, (float xOffset, float yOffset)   other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, (double xOffset, double yOffset) other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator *( ReadOnlyLine self, (int xOffset, int yOffset)       other ) => new(self.Start * other, self.End * other);
    public static ReadOnlyLine operator *( ReadOnlyLine self, (float xOffset, float yOffset)   other ) => new(self.Start * other, self.End * other);
    public static ReadOnlyLine operator *( ReadOnlyLine self, (double xOffset, double yOffset) other ) => new(self.Start * other, self.End * other);
    public static ReadOnlyLine operator /( ReadOnlyLine self, (int xOffset, int yOffset)       other ) => new(self.Start / other, self.End / other);
    public static ReadOnlyLine operator /( ReadOnlyLine self, (float xOffset, float yOffset)   other ) => new(self.Start / other, self.End / other);
    public static ReadOnlyLine operator /( ReadOnlyLine self, (double xOffset, double yOffset) other ) => new(self.Start / other, self.End / other);
    public static ReadOnlyLine operator *( ReadOnlyLine self, int                              other ) => new(self.Start * other, self.End * other);
    public static ReadOnlyLine operator *( ReadOnlyLine self, float                            other ) => new(self.Start * other, self.End * other);
    public static ReadOnlyLine operator *( ReadOnlyLine self, double                           other ) => new(self.Start * other, self.End * other);
    public static ReadOnlyLine operator /( ReadOnlyLine self, int                              other ) => new(self.Start / other, self.End / other);
    public static ReadOnlyLine operator /( ReadOnlyLine self, float                            other ) => new(self.Start / other, self.End / other);
    public static ReadOnlyLine operator /( ReadOnlyLine self, double                           other ) => new(self.Start / other, self.End / other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, int                              other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, float                            other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator +( ReadOnlyLine self, double                           other ) => new(self.Start + other, self.End + other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, int                              other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, float                            other ) => new(self.Start - other, self.End - other);
    public static ReadOnlyLine operator -( ReadOnlyLine self, double                           other ) => new(self.Start - other, self.End - other);
}
