// TrueLogic :: TrueLogic.Common.Maui
// 01/19/2025  21:01


namespace Jakar.Extensions;


[DefaultValue( nameof(Zero) )]
public readonly struct ReadOnlyPointF( float x, float y ) : IPoint<ReadOnlyPointF, float>
{
    public static readonly ReadOnlyPointF Invalid = new(float.NaN, float.NaN);
    public static readonly ReadOnlyPointF Zero    = new(0, 0);
    public readonly        float          X       = x;
    public readonly        float          Y       = y;


    public static Sorter<ReadOnlyPointF>                       Sorter  => Sorter<ReadOnlyPointF>.Default;
    static        ReadOnlyPointF IGenericShape<ReadOnlyPointF>.Zero    => Zero;
    static        ReadOnlyPointF IGenericShape<ReadOnlyPointF>.Invalid => Invalid;
    public        bool                                         IsEmpty => X == 0 && Y == 0;
    public        bool                                         IsNaN   => float.IsNaN( X ) || float.IsNaN( Y );
    public        bool                                         IsValid => IsNaN is false;
    float IShapeLocation<float>.                               X       => X;
    float IShapeLocation<float>.                               Y       => Y;


    public static implicit operator ReadOnlyPointF( Point         point )     => new(point.X, point.Y);
    public static implicit operator ReadOnlyPointF( PointF        point )     => new(point.X, point.Y);
    public static implicit operator ReadOnlyPointF( ReadOnlyPoint point )     => new((float)point.X, (float)point.Y);
    public static implicit operator ReadOnlyPoint( ReadOnlyPointF rectangle ) => new(rectangle.X, rectangle.Y);
    public static implicit operator Point( ReadOnlyPointF         rectangle ) => new((int)rectangle.X.Round(), (int)rectangle.Y.Round());
    public static implicit operator PointF( ReadOnlyPointF        rectangle ) => new(rectangle.X, rectangle.Y);


    [Pure] public static ReadOnlyPointF Create( float x, float y ) => new(x, y);
    [Pure] public        ReadOnlyPointF Reverse() => new(Y, X);
    [Pure] public        ReadOnlyPointF Round()   => new(X.Round(), Y.Round());
    [Pure] public        ReadOnlyPointF Floor()   => new(X.Floor(), Y.Floor());


    [Pure, MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public float DistanceTo( in ReadOnlyPointF other )
    {
        float x      = X - other.X;
        float y      = Y - other.Y;
        float x2     = x * x;
        float y2     = y * y;
        float result = (float)Math.Sqrt( x2 + y2 );
        return result;
    }


    public int CompareTo( ReadOnlyPointF other )
    {
        int xOffsetComparison = X.CompareTo( other.X );

        return xOffsetComparison != 0
                   ? xOffsetComparison
                   : Y.CompareTo( other.Y );
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is ReadOnlyPointF other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(ReadOnlyPointF)}" );
    }

    public          bool   Equals( ReadOnlyPointF other )                              => X.Equals( other.X )         && Y.Equals( other.Y );
    public override bool   Equals( object?        obj )                                => obj is ReadOnlyPointF other && Equals( other );
    public override int    GetHashCode()                                               => HashCode.Combine( X, Y );
    public override string ToString()                                                  => ToString( null, null );
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IPoint<ReadOnlyPointF, float>.ToString( this, format );


    public static bool operator ==( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.Equals( left, value );
    public static bool operator !=( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.DoesNotEqual( left, value );
    public static bool operator >( ReadOnlyPointF           left, ReadOnlyPointF                   value ) => Sorter.GreaterThan( left, value );
    public static bool operator >=( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.GreaterThanOrEqualTo( left, value );
    public static bool operator <( ReadOnlyPointF           left, ReadOnlyPointF                   value ) => Sorter.LessThan( left, value );
    public static bool operator <=( ReadOnlyPointF          left, ReadOnlyPointF                   value ) => Sorter.LessThanOrEqualTo( left, value );
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, Point                            value ) => new(size.X + value.X, size.Y                 + value.Y);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, PointF                           value ) => new(size.X + value.X, size.Y                 + value.Y);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X + value.X, size.Y                 + value.Y);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X + value.xOffset, size.Y           + value.yOffset);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X + value.xOffset, size.Y           + value.yOffset);
    public static ReadOnlyPointF operator +( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new(size.X + value.xOffset.AsFloat(), size.Y + value.yOffset.AsFloat());
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, Point                            value ) => new(size.X - value.X, size.Y                 - value.Y);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, PointF                           value ) => new(size.X - value.X, size.Y                 - value.Y);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X - value.X, size.Y                 - value.Y);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X - value.xOffset, size.Y           - value.yOffset);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X - value.xOffset, size.Y           - value.yOffset);
    public static ReadOnlyPointF operator -( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new(size.X - value.xOffset.AsFloat(), size.Y - value.yOffset.AsFloat());
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X * value.X, size.Y                 * value.Y);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, int                              value ) => new(size.X * value, size.Y                   * value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, float                            value ) => new(size.X * value, size.Y                   * value);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, double                           value ) => new(size.X * value.AsFloat(), size.Y         * value.AsFloat());
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X * value.xOffset, size.Y           * value.yOffset);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X * value.xOffset, size.Y           * value.yOffset);
    public static ReadOnlyPointF operator *( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new(size.X * value.xOffset.AsFloat(), size.Y * value.yOffset.AsFloat());
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, ReadOnlyPointF                   value ) => new(size.X / value.X, size.Y                 / value.Y);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, int                              value ) => new(size.X / value, size.Y                   / value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, float                            value ) => new(size.X / value, size.Y                   / value);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, double                           value ) => new(size.X / value.AsFloat(), size.Y         / value.AsFloat());
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, (int xOffset, int yOffset)       value ) => new(size.X / value.xOffset, size.Y           / value.yOffset);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, (float xOffset, float yOffset)   value ) => new(size.X / value.xOffset, size.Y           / value.yOffset);
    public static ReadOnlyPointF operator /( ReadOnlyPointF size, (double xOffset, double yOffset) value ) => new(size.X / value.xOffset.AsFloat(), size.Y / value.yOffset.AsFloat());
    public static ReadOnlyPointF operator +( ReadOnlyPointF left, double                           value ) => new(left.X + value.AsFloat(), left.Y + value.AsFloat());
    public static ReadOnlyPointF operator -( ReadOnlyPointF left, double                           value ) => new(left.X - value.AsFloat(), left.Y - value.AsFloat());
    public static ReadOnlyPointF operator +( ReadOnlyPointF left, float                            value ) => new(left.X + value, left.Y           + value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF left, float                            value ) => new(left.X - value, left.Y           - value);
    public static ReadOnlyPointF operator +( ReadOnlyPointF left, int                              value ) => new(left.X + value, left.Y           + value);
    public static ReadOnlyPointF operator -( ReadOnlyPointF left, int                              value ) => new(left.X - value, left.Y           - value);
}
