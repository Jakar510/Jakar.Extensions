namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyPoint( double x, double y ) : IPoint<ReadOnlyPoint>, IMathOperators<ReadOnlyPoint>, IShapeOperators<ReadOnlyPoint>
{
    public static readonly ReadOnlyPoint Invalid       = new(double.NaN, double.NaN);
    public static readonly ReadOnlyPoint Zero          = 0;
    public static readonly ReadOnlyPoint One           = 1;
    public static readonly ReadOnlyPoint Two           = 2;
    public static readonly ReadOnlyPoint Three         = 3;
    public static readonly ReadOnlyPoint Four          = 4;
    public static readonly ReadOnlyPoint Five          = 5;
    public static readonly ReadOnlyPoint Six           = 6;
    public static readonly ReadOnlyPoint Seven         = 7;
    public static readonly ReadOnlyPoint Eight         = 8;
    public static readonly ReadOnlyPoint Nine          = 9;
    public static readonly ReadOnlyPoint Ten           = 10;
    public static readonly ReadOnlyPoint NegativeOne   = -1;
    public static readonly ReadOnlyPoint NegativeTwo   = -2;
    public static readonly ReadOnlyPoint NegativeThree = -3;
    public static readonly ReadOnlyPoint NegativeFour  = -4;
    public static readonly ReadOnlyPoint NegativeFive  = -5;
    public static readonly ReadOnlyPoint NegativeSix   = -6;
    public static readonly ReadOnlyPoint NegativeSeven = -7;
    public static readonly ReadOnlyPoint NegativeEight = -8;
    public static readonly ReadOnlyPoint NegativeNine  = -9;
    public static readonly ReadOnlyPoint NegativeTen   = -10;
    public readonly        double        X             = x;
    public readonly        double        Y             = y;


    public static       Sorter<ReadOnlyPoint>                        Sorter        => Sorter<ReadOnlyPoint>.Default;
    static ref readonly ReadOnlyPoint IShape<ReadOnlyPoint>.         Zero          => ref Zero;
    static ref readonly ReadOnlyPoint IShape<ReadOnlyPoint>.         Invalid       => ref Invalid;
    static ref readonly ReadOnlyPoint IShape<ReadOnlyPoint>.         One           => ref One; 
    public              bool                                         IsEmpty       => X == 0 && Y == 0;
    public              bool                                         IsNaN         => double.IsNaN(X) || double.IsNaN(Y);
    public              bool                                         IsValid       => IsNaN is false;
    double IShapeLocation.                                           X             => X;
    double IShapeLocation.                                           Y             => Y;


    public static implicit operator Point( ReadOnlyPoint          point ) => new((int)point.X.Round(), (int)point.Y.Round());
    public static implicit operator PointF( ReadOnlyPoint         point ) => new((float)point.X, (float)point.Y);
    public static implicit operator ReadOnlyPointF( ReadOnlyPoint point ) => new((float)point.X, (float)point.Y);
    public static implicit operator ReadOnlyPoint( ReadOnlyPointF point ) => new(point.X, point.Y);
    public static implicit operator ReadOnlyPoint( Point          point ) => new(point.X, point.Y);
    public static implicit operator ReadOnlyPoint( PointF         point ) => new(point.X, point.Y);
    public static implicit operator ReadOnlyPoint( int            value ) => new(value, value);
    public static implicit operator ReadOnlyPoint( long           value ) => new(value, value);
    public static implicit operator ReadOnlyPoint( float          value ) => new(value, value);
    public static implicit operator ReadOnlyPoint( double         value ) => new(value, value);


    [Pure] public static ReadOnlyPoint Create( float  x, float  y ) => new(x, y);
    [Pure] public static ReadOnlyPoint Create( double x, double y ) => new(x, y);
    [Pure] public        ReadOnlyPoint Reverse() => new(Y, X);
    [Pure] public        ReadOnlyPoint Round()   => new(X.Round(), Y.Round());
    [Pure] public        ReadOnlyPoint Floor()   => new(X.Floor(), Y.Floor());


    [Pure, MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public double DistanceTo( in ReadOnlyPoint other )
    {
        double x      = X - other.X;
        double y      = Y - other.Y;
        double x2     = x * x;
        double y2     = y * y;
        double result = Math.Sqrt(x2 + y2);
        return result;
    }


    public int CompareTo( ReadOnlyPoint other )
    {
        int xOffsetComparison = X.CompareTo(other.X);

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : Y.CompareTo(other.Y);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyPoint other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlyPoint)}");
    }
    public          bool   Equals( ReadOnlyPoint other )                               => X.Equals(other.X)          && Y.Equals(other.Y);
    public override bool   Equals( object?       obj )                                 => obj is ReadOnlyPoint other && Equals(other);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IPoint<ReadOnlyPoint>.ToString(in this, format);


    public static bool operator ==( ReadOnlyPoint         left, ReadOnlyPoint                    value ) => Sorter.Equals(left, value);
    public static bool operator !=( ReadOnlyPoint         left, ReadOnlyPoint                    value ) => Sorter.DoesNotEqual(left, value);
    public static bool operator >( ReadOnlyPoint          left, ReadOnlyPoint                    value ) => Sorter.GreaterThan(left, value);
    public static bool operator >=( ReadOnlyPoint         left, ReadOnlyPoint                    value ) => Sorter.GreaterThanOrEqualTo(left, value);
    public static bool operator <( ReadOnlyPoint          left, ReadOnlyPoint                    value ) => Sorter.LessThan(left, value);
    public static bool operator <=( ReadOnlyPoint         left, ReadOnlyPoint                    value ) => Sorter.LessThanOrEqualTo(left, value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint size, ReadOnlyPoint                    value ) => new(size.X + value.X, size.Y + value.Y);
    public static ReadOnlyPoint operator -( ReadOnlyPoint size, ReadOnlyPoint                    value ) => new(size.X - value.X, size.Y - value.Y);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, ReadOnlyPoint                    value ) => new(size.X * value.X, size.Y * value.Y);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, ReadOnlyPoint                    value ) => new(size.X / value.X, size.Y / value.Y);
    public static ReadOnlyPoint operator +( ReadOnlyPoint size, ReadOnlyPointF                   value ) => new(size.X + value.X, size.Y + value.Y);
    public static ReadOnlyPoint operator -( ReadOnlyPoint size, ReadOnlyPointF                   value ) => new(size.X - value.X, size.Y - value.Y);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, ReadOnlyPointF                   value ) => new(size.X * value.X, size.Y * value.Y);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, ReadOnlyPointF                   value ) => new(size.X / value.X, size.Y / value.Y);
    public static ReadOnlyPoint operator +( ReadOnlyPoint size, (int xOffset, int yOffset)       value ) => new(size.X + value.xOffset, size.Y + value.yOffset);
    public static ReadOnlyPoint operator -( ReadOnlyPoint size, (int xOffset, int yOffset)       value ) => new(size.X - value.xOffset, size.Y - value.yOffset);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, (int xOffset, int yOffset)       value ) => new(size.X / value.xOffset, size.Y / value.yOffset);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, (int xOffset, int yOffset)       value ) => new(size.X * value.xOffset, size.Y * value.yOffset);
    public static ReadOnlyPoint operator +( ReadOnlyPoint size, (float xOffset, float yOffset)   value ) => new(size.X + value.xOffset, size.Y + value.yOffset);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, (float xOffset, float yOffset)   value ) => new(size.X * value.xOffset, size.Y * value.yOffset);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, (float xOffset, float yOffset)   value ) => new(size.X / value.xOffset, size.Y / value.yOffset);
    public static ReadOnlyPoint operator -( ReadOnlyPoint size, (float xOffset, float yOffset)   value ) => new(size.X - value.xOffset, size.Y - value.yOffset);
    public static ReadOnlyPoint operator +( ReadOnlyPoint size, (double xOffset, double yOffset) value ) => new(size.X + value.xOffset, size.Y + value.yOffset);
    public static ReadOnlyPoint operator -( ReadOnlyPoint size, (double xOffset, double yOffset) value ) => new(size.X - value.xOffset, size.Y - value.yOffset);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, (double xOffset, double yOffset) value ) => new(size.X / value.xOffset, size.Y / value.yOffset);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, (double xOffset, double yOffset) value ) => new(size.X * value.xOffset, size.Y * value.yOffset);
    public static ReadOnlyPoint operator +( ReadOnlyPoint left, double                           value ) => new(left.X + value, left.Y + value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint left, double                           value ) => new(left.X - value, left.Y - value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, double                           value ) => new(size.X * value, size.Y * value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, double                           value ) => new(size.X / value, size.Y / value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint left, float                            value ) => new(left.X + value, left.Y + value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint left, float                            value ) => new(left.X - value, left.Y - value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, float                            value ) => new(size.X / value, size.Y / value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, float                            value ) => new(size.X * value, size.Y * value);
    public static ReadOnlyPoint operator +( ReadOnlyPoint left, int                              value ) => new(left.X + value, left.Y + value);
    public static ReadOnlyPoint operator -( ReadOnlyPoint left, int                              value ) => new(left.X - value, left.Y - value);
    public static ReadOnlyPoint operator /( ReadOnlyPoint size, int                              value ) => new(size.X / value, size.Y / value);
    public static ReadOnlyPoint operator *( ReadOnlyPoint size, int                              value ) => new(size.X * value, size.Y * value);
}
