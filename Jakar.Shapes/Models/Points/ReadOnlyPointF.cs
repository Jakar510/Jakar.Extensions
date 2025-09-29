// TrueLogic :: TrueLogic.Common.Maui
// 01/19/2025  21:01


namespace Jakar.Shapes;


[DefaultValue(nameof(Zero))]
public readonly struct ReadOnlyPointF( float x, float y ) : IPoint<ReadOnlyPointF>, IMathOperators<ReadOnlyPointF>
{
    public static readonly ReadOnlyPointF Invalid = new(float.NaN, float.NaN);
    public static readonly ReadOnlyPointF Zero    = 0;
    public static readonly ReadOnlyPointF One     = 1;
    public readonly        float          X       = x;
    public readonly        float          Y       = y;

     
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.Zero    => ref Zero;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.Invalid => ref Invalid;
    static ref readonly ReadOnlyPointF IShape<ReadOnlyPointF>.One     => ref One;
    public              bool                                  IsEmpty => X == 0 && Y == 0;
    public              bool                                  IsNaN   => float.IsNaN(X) || float.IsNaN(Y);
    public              bool                                  IsValid => !IsNaN;
    double IShapeLocation.                                    X       => X;
    double IShapeLocation.                                    Y       => Y;


    public static implicit operator ReadOnlyPointF( Point  value ) => new(value.X, value.Y);
    public static implicit operator ReadOnlyPointF( PointF value ) => new(value.X, value.Y);
    public static implicit operator Point( ReadOnlyPointF  value ) => new((int)value.X.Round(), (int)value.Y.Round());
    public static implicit operator PointF( ReadOnlyPointF value ) => new(value.X, value.Y);
    public static implicit operator ReadOnlyPointF( int    value ) => new(value, value);
    public static implicit operator ReadOnlyPointF( long   value ) => new(value, value);
    public static implicit operator ReadOnlyPointF( float  value ) => new(value, value);
    public static implicit operator ReadOnlyPointF( double value ) => new(value.AsFloat(), value.AsFloat());


    [Pure] public static ReadOnlyPointF Create( float  x, float  y ) => new(x, y);
    [Pure] public static ReadOnlyPointF Create( double x, double y ) => new(x.AsFloat(), y.AsFloat());


    [Pure] public ReadOnlyPointF Reverse() => new(Y, X);
    [Pure] public ReadOnlyPointF Round()   => new(X.Round(), Y.Round());
    [Pure] public ReadOnlyPointF Floor()   => new(X.Floor(), Y.Floor());


    public double DistanceTo( in ReadOnlyPointF other )
    {
        double x      = X - other.X;
        double y      = Y - other.Y;
        double x2     = x * x;
        double y2     = y * y;
        double result = Math.Sqrt(x2 + y2);
        return result;
    }
    public double Dot( in ReadOnlyPoint other ) => X * other.X + Y * other.Y;
    public double Magnitude()                   => Math.Sqrt(X * X + Y * Y);
    public double AngleBetween( ref readonly ReadOnlyPoint p1, ref readonly ReadOnlyPoint p2 )
    {
        ReadOnlyPoint v1 = this - p1;
        ReadOnlyPoint v2 = this - p2;

        double dot  = v1.Dot(in v2);
        double mag1 = v1.Magnitude();
        double mag2 = v2.Magnitude();
        if ( mag1 == 0 || mag2 == 0 ) { return 0; }

        double cosTheta = dot / ( mag1 * mag2 );
        cosTheta = Math.Clamp(cosTheta, -1.0, 1.0); // Avoid NaN due to precision

        return Math.Acos(cosTheta); // In radians
    }


    public void Deconstruct( out double x, out double y )
    {
        x = X;
        y = Y;
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

    
    public static bool operator ==( ReadOnlyPointF? left, ReadOnlyPointF? right ) => Nullable.Equals(left, right);
    public static bool operator !=( ReadOnlyPointF? left, ReadOnlyPointF? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ReadOnlyPointF  left, ReadOnlyPointF  right ) => EqualityComparer<ReadOnlyPointF>.Default.Equals(left, right);
    public static bool operator !=( ReadOnlyPointF  left, ReadOnlyPointF  right ) => !EqualityComparer<ReadOnlyPointF>.Default.Equals(left, right);
    public static bool operator >( ReadOnlyPointF   left, ReadOnlyPointF  right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) > 0;
    public static bool operator >=( ReadOnlyPointF  left, ReadOnlyPointF  right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) >= 0;
    public static bool operator <( ReadOnlyPointF   left, ReadOnlyPointF  right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) < 0;
    public static bool operator <=( ReadOnlyPointF  left, ReadOnlyPointF  right ) => Comparer<ReadOnlyPointF>.Default.Compare(left, right) <= 0;
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
