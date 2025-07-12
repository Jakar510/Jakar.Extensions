// TrueLogic :: TrueLogic.Common.Maui
// 01/19/2025  21:01


namespace Jakar.Extensions;


[DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyPointF( float x, float y ) : IPoint<ReadOnlyPointF>, IMathOperators<ReadOnlyPointF>, IShapeOperators<ReadOnlyPointF>
{
    public static readonly ReadOnlyPointF Invalid       = new(float.NaN, float.NaN);
    public static readonly ReadOnlyPointF Zero          = 0;
    public static readonly ReadOnlyPointF One           = 1;
    public static readonly ReadOnlyPointF Two           = 2;
    public static readonly ReadOnlyPointF Three         = 3;
    public static readonly ReadOnlyPointF Four          = 4;
    public static readonly ReadOnlyPointF Five          = 5;
    public static readonly ReadOnlyPointF Six           = 6;
    public static readonly ReadOnlyPointF Seven         = 7;
    public static readonly ReadOnlyPointF Eight         = 8;
    public static readonly ReadOnlyPointF Nine          = 9;
    public static readonly ReadOnlyPointF Ten           = 10;
    public static readonly ReadOnlyPointF NegativeOne   = -1;
    public static readonly ReadOnlyPointF NegativeTwo   = -2;
    public static readonly ReadOnlyPointF NegativeThree = -3;
    public static readonly ReadOnlyPointF NegativeFour  = -4;
    public static readonly ReadOnlyPointF NegativeFive  = -5;
    public static readonly ReadOnlyPointF NegativeSix   = -6;
    public static readonly ReadOnlyPointF NegativeSeven = -7;
    public static readonly ReadOnlyPointF NegativeEight = -8;
    public static readonly ReadOnlyPointF NegativeNine  = -9;
    public static readonly ReadOnlyPointF NegativeTen   = -10;
    public readonly        float          X             = x;
    public readonly        float          Y             = y;


    public static       Sorter<ReadOnlyPointF>                Sorter  => Sorter<ReadOnlyPointF>.Default;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.Zero    => ref Zero;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.Invalid => ref Invalid;
    public              bool                                  IsEmpty => X == 0 && Y == 0;
    public              bool                                  IsNaN   => float.IsNaN(X) || float.IsNaN(Y);
    public              bool                                  IsValid => IsNaN is false;
    double IShapeLocation.                                    X       => X;
    double IShapeLocation.                                    Y       => Y;


    public static implicit operator ReadOnlyPointF( Point  point )     => new(point.X, point.Y);
    public static implicit operator ReadOnlyPointF( PointF point )     => new(point.X, point.Y);
    public static implicit operator Point( ReadOnlyPointF  rectangle ) => new((int)rectangle.X.Round(), (int)rectangle.Y.Round());
    public static implicit operator PointF( ReadOnlyPointF rectangle ) => new(rectangle.X, rectangle.Y);
    public static implicit operator ReadOnlyPointF( int    value )     => new(value, value);
    public static implicit operator ReadOnlyPointF( long   value )     => new(value, value);
    public static implicit operator ReadOnlyPointF( float  value )     => new(value, value);
    public static implicit operator ReadOnlyPointF( double value )     => new(value.AsFloat(), value.AsFloat());


    [Pure] public static ReadOnlyPointF Create( float  x, float  y ) => new(x, y);
    [Pure] public static ReadOnlyPointF Create( double x, double y ) => new(x.AsFloat(), y.AsFloat());
    [Pure] public        ReadOnlyPointF Reverse() => new(Y, X);
    [Pure] public        ReadOnlyPointF Round()   => new(X.Round(), Y.Round());
    [Pure] public        ReadOnlyPointF Floor()   => new(X.Floor(), Y.Floor());


    [Pure, MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public double DistanceTo( in ReadOnlyPointF other )
    {
        float x      = X - other.X;
        float y      = Y - other.Y;
        float x2     = x * x;
        float y2     = y * y;
        float result = (float)Math.Sqrt(x2 + y2);
        return result;
    }


    public int CompareTo( ReadOnlyPointF other )
    {
        int xOffsetComparison = X.CompareTo(other.X);

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : Y.CompareTo(other.Y);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyPointF other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(ReadOnlyPointF)}");
    }

    public          bool   Equals( ReadOnlyPointF other )                              => X.Equals(other.X)           && Y.Equals(other.Y);
    public override bool   Equals( object?        obj )                                => obj is ReadOnlyPointF other && Equals(other);
    public override int    GetHashCode()                                               => HashCode.Combine(X, Y);
    public override string ToString()                                                  => ToString(null, null);
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IPoint<ReadOnlyPointF>.ToString(in this, format);


    public static bool operator ==( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.Equals(left, value);
    public static bool operator !=( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.DoesNotEqual(left, value);
    public static bool operator >( ReadOnlyPointF           left, ReadOnlyPointF                   value ) => Sorter.GreaterThan(left, value);
    public static bool operator >=( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.GreaterThanOrEqualTo(left, value);
    public static bool operator <( ReadOnlyPointF           left, ReadOnlyPointF                   value ) => Sorter.LessThan(left, value);
    public static bool operator <=( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.LessThanOrEqualTo(left, value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, ReadOnlyPoint                    value ) => new((float)( size.X + value.X ), (float)( size.Y + value.Y ));
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, ReadOnlyPoint                    value ) => new((float)( size.X - value.X ), (float)( size.Y - value.Y ));
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, ReadOnlyPoint                    value ) => new((float)( size.X * value.X ), (float)( size.Y * value.Y ));
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, ReadOnlyPoint                    value ) => new((float)( size.X / value.X ), (float)( size.Y / value.Y ));
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X + value.X, size.Y + value.Y);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X - value.X, size.Y - value.Y);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X * value.X, size.Y * value.Y);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X / value.X, size.Y / value.Y);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X + value.xOffset, size.Y + value.yOffset);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X - value.xOffset, size.Y - value.yOffset);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X / value.xOffset, size.Y / value.yOffset);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X * value.xOffset, size.Y * value.yOffset);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X + value.xOffset, size.Y + value.yOffset);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X - value.xOffset, size.Y - value.yOffset);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X / value.xOffset, size.Y / value.yOffset);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X * value.xOffset, size.Y * value.yOffset);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new((float)( size.X + value.xOffset ), (float)( size.Y + value.yOffset ));
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new((float)( size.X - value.xOffset ), (float)( size.Y - value.yOffset ));
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new((float)( size.X * value.xOffset ), (float)( size.Y * value.yOffset ));
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new((float)( size.X / value.xOffset ), (float)( size.Y / value.yOffset ));
    public static ReadOnlyPointF operator +( ReadOnlyPointF left, double                           value ) => new((float)( left.X + value ), (float)( left.Y + value ));
    public static ReadOnlyPointF operator -( ReadOnlyPointF left, double                           value ) => new((float)( left.X - value ), (float)( left.Y - value ));
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, double                           value ) => new((float)( size.X * value ), (float)( size.Y * value ));
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, double                           value ) => new((float)( size.X / value ), (float)( size.Y / value ));
    public static ReadOnlyPointF operator +( ReadOnlyPointF left, float                            value ) => new(left.X + value, left.Y + value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF left, float                            value ) => new(left.X - value, left.Y - value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, float                            value ) => new(size.X / value, size.Y / value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, float                            value ) => new(size.X * value, size.Y * value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF left, int                              value ) => new(left.X + value, left.Y + value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF left, int                              value ) => new(left.X - value, left.Y - value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, int                              value ) => new(size.X / value, size.Y / value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, int                              value ) => new(size.X * value, size.Y * value);
}
