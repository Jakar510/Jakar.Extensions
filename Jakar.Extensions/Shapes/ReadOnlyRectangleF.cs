// TrueLogic :: TrueLogic.Common.Maui
// 01/20/2025  08:01

namespace Jakar.Extensions;


[StructLayout( LayoutKind.Sequential ), DefaultValue( nameof(Zero) )]
public readonly struct ReadOnlyRectangleF( float x, float y, float width, float height ) : IRectangle<ReadOnlyRectangleF, ReadOnlySizeF, ReadOnlyPointF, ReadOnlyThickness, float>, IMathOperators<ReadOnlyRectangleF>
{
    public static readonly ReadOnlyRectangleF Invalid = new(float.NaN, float.NaN, float.NaN, float.NaN);
    public static readonly ReadOnlyRectangleF Zero    = new(0, 0, 0, 0);
    public readonly        float              X       = x;
    public readonly        float              Y       = y;
    public readonly        float              Width   = width;
    public readonly        float              Height  = height;


    public static Sorter<ReadOnlyRectangleF>                           Sorter  => Sorter<ReadOnlyRectangleF>.Default;
    static        ReadOnlyRectangleF IGenericShape<ReadOnlyRectangleF>.Zero    => Zero;
    static        ReadOnlyRectangleF IGenericShape<ReadOnlyRectangleF>.Invalid => Invalid;


    public bool                 IsEmpty  => IRectangle<ReadOnlyRectangleF, ReadOnlySizeF, ReadOnlyPointF, ReadOnlyThickness, float>.CheckIfEmpty( in this );
    float IShapeLocation<float>.X        => X;
    float IShapeLocation<float>.Y        => Y;
    float IShapeSize<float>.    Width    => Width;
    float IShapeSize<float>.    Height   => Height;
    public bool                 IsNaN    => double.IsNaN( X ) || double.IsNaN( Y ) || double.IsNaN( Width ) || double.IsNaN( Height );
    public bool                 IsValid  => IsNaN is false && X >= 0 && Y >= 0 && Width >= 0 && Height >= 0;
    public ReadOnlyPointF       Center   => new(Right / 2, Bottom / 2);
    public ReadOnlyPointF       Location => new(X, Y);
    public ReadOnlySizeF        Size     => new(Width, Height);
    public float                Bottom   => Y + Height;
    public float                Left     => X;
    public float                Right    => X + Width;
    public float                Top      => Y;


    [Pure]
    public static ReadOnlySize AddMargin( in ReadOnlySize value, in ReadOnlyThickness margin )
    {
        ReadOnlySize result = new(value.Width + margin.HorizontalThickness, value.Height + margin.VerticalThickness);
        Debug.Assert( result >= value );
        return result;
    }


    [Pure] public static ReadOnlyRectangleF Create( params ReadOnlySpan<ReadOnlyPointF> points )                                 => RectangleD.Create( points );
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPointF               point,   in ReadOnlySizeF  size )        => new(point.X, point.Y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create( in     ReadOnlyPointF               topLeft, in ReadOnlyPointF bottomRight ) => new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

    [Pure]
    public static ReadOnlyRectangleF Create( in ReadOnlyRectangle rectangle, in ReadOnlyThickness padding )
    {
        float x      = (float)(rectangle.Left   + padding.Left);
        float y      = (float)(rectangle.Top    + padding.Top);
        float width  = (float)(rectangle.Width  - padding.HorizontalThickness);
        float height = (float)(rectangle.Height - padding.VerticalThickness);
        return new ReadOnlyRectangleF( x, y, width, height );
    }

    [Pure]
    public static ReadOnlyRectangleF Create( in ReadOnlyRectangleF rectangle, in ReadOnlyThickness padding )
    {
        float x      = (float)(rectangle.Left   + padding.HorizontalThickness);
        float y      = (float)(rectangle.Top    + padding.VerticalThickness);
        float width  = (float)(rectangle.Width  - padding.HorizontalThickness);
        float height = (float)(rectangle.Height - padding.VerticalThickness);
        return new ReadOnlyRectangleF( x, y, width, height );
    }


    [Pure] public static ReadOnlyRectangleF Create( float x, float y, in ReadOnlySizeF size )                => new(x, y, size.Width, size.Height);
    [Pure] public static ReadOnlyRectangleF Create( float x, float y, float            width, float height ) => new(x, y, width, height);


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        return other is ReadOnlyRectangleF x
                   ? CompareTo( x )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(ReadOnlyRectangleF) );
    }
    public int CompareTo( ReadOnlyRectangleF other )
    {
        int xComparison = X.CompareTo( other.X );
        if ( xComparison != 0 ) { return xComparison; }

        int yComparison = Y.CompareTo( other.Y );
        if ( yComparison != 0 ) { return yComparison; }

        int widthComparison = Width.CompareTo( other.Width );
        if ( widthComparison != 0 ) { return widthComparison; }

        return Height.CompareTo( other.Height );
    }
    public          bool   Equals( ReadOnlyRectangleF other )                          => X.Equals( other.X )           && Y.Equals( other.Y ) && Width.Equals( other.Width ) && Height.Equals( other.Height );
    public override bool   Equals( object?            other )                          => other is ReadOnlyRectangleF x && Equals( x );
    public override int    GetHashCode()                                               => HashCode.Combine( X, Y, Width, Height );
    public override string ToString()                                                  => ToString( null, null );
    public          string ToString( string? format, IFormatProvider? formatProvider ) => IRectangle<ReadOnlyRectangleF, ReadOnlySizeF, ReadOnlyPointF, float>.ToString( this, format );


    [Pure] public bool               IsAtLeast( in      ReadOnlySizeF      other ) => other.Width <= Width      && other.Height <= Height;
    [Pure] public bool               Contains( in       ReadOnlyPointF     other ) => other.X     >= X          && other.X      < Right        && other.Y >= Y         && other.Y < Bottom;
    [Pure] public bool               Contains( in       ReadOnlyRectangleF other ) => Left        <= other.Left && Right        >= other.Right && Top     <= other.Top && Bottom  >= other.Bottom;
    [Pure] public bool               IntersectsWith( in ReadOnlyRectangleF other ) => (Left >= other.Right || Right <= other.Left || Top >= other.Bottom || Bottom <= other.Top) is false;
    [Pure] public ReadOnlyRectangleF Union( in          ReadOnlyRectangleF other ) => new(Math.Min( X, other.X ), Math.Min( Y, other.Y ), Math.Max( Right, other.Right ), Math.Max( Bottom, other.Bottom ));
    [Pure] public ReadOnlyRectangleF Round()                                       => new((float)X.Round(), (float)Y.Round(), (float)Width.Round(), (float)Height.Round());


    [Pure]
    public ReadOnlyRectangleF Intersection( in ReadOnlyRectangleF other )
    {
        float x      = Math.Max( X, other.X );
        float y      = Math.Max( Y, other.Y );
        float width  = Math.Min( Right,  other.Right )  - x;
        float height = Math.Min( Bottom, other.Bottom ) - y;

        return width < 0 || height < 0
                   ? Zero
                   : new ReadOnlyRectangleF( x, y, width, height );
    }


    [Pure]
    public bool Contains( params ReadOnlySpan<ReadOnlyPointF> points )
    {
        foreach ( ReadOnlyPointF point in points )
        {
            if ( Contains( in point ) ) { return true; }
        }

        return false;
    }


    [Pure]
    public bool DoesLineIntersect( in ReadOnlyPointF source, in ReadOnlyPointF target )
    {
        float               t0          = 0.0f;
        float               t1          = 1.0f;
        float               dx          = target.X - source.X;
        float               dy          = target.Y - source.Y;
        ReadOnlySpan<float> boundariesX = [X, X + Width];
        ReadOnlySpan<float> boundariesY = [Y, Y + Height];

        for ( int i = 0; i < 2; i++ )
        {
            float pX = i == 0
                           ? -dx
                           : dx;

            float pY = i == 0
                           ? -dy
                           : dy;

            for ( int j = 0; j < 2; j++ )
            {
                float qX = j == 0
                               ? source.X       - boundariesX[i]
                               : boundariesX[i] - source.X;

                float qY = j == 0
                               ? source.Y       - boundariesY[i]
                               : boundariesY[i] - source.Y;

                if ( pX == 0 && qX < 0 ) { return false; } // Line is parallel to the rectangle's horizontal edge and outside of it

                if ( pY == 0 && qY < 0 ) { return false; } // Line is parallel to the rectangle's vertical edge and outside of it

                float rX = pX != 0
                               ? qX / pX
                               : float.MaxValue;

                float rY = pY != 0
                               ? qY / pY
                               : float.MaxValue;

                if ( pX < 0 ) { t0 = Math.Max( t0, rX ); }
                else { t1          = Math.Min( t1, rX ); }

                if ( pY < 0 ) { t0 = Math.Max( t0, rY ); }
                else { t1          = Math.Min( t1, rY ); }

                if ( t0 > t1 ) { return false; }
            }
        }

        return true;
    }


    public void Deconstruct( out float x, out float y, out float width, out float height )
    {
        x      = X;
        y      = Y;
        width  = Width;
        height = Height;
    }
    public void Deconstruct( out ReadOnlyPointF point, out ReadOnlySizeF size )
    {
        point = Location;
        size  = Size;
    }


    public RectangleD ToRectD()        => new(X, Y, Width, Height);
    public Rectangle  ToDrawingRect()  => new((int)X.Round(), (int)Y.Round(), (int)Width.Round(), (int)Height.Round());
    public RectangleF ToDrawingRectF() => new(X, Y, Width, Height);


    public static implicit operator Rectangle( ReadOnlyRectangleF         rectangle ) => rectangle.ToDrawingRect();
    public static implicit operator RectangleF( ReadOnlyRectangleF        rectangle ) => rectangle.ToDrawingRectF();
    public static implicit operator RectangleD( ReadOnlyRectangleF        rectangle ) => rectangle.ToRectD();
    public static implicit operator ReadOnlyRectangle( ReadOnlyRectangleF rectangle ) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( ReadOnlyRectangle rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( Rectangle         rectangle ) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( RectangleF        rectangle ) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    public static implicit operator ReadOnlyRectangleF( RectangleD        rectangle ) => new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);


    public static        bool operator ==( ReadOnlyRectangleF              left,      ReadOnlyRectangleF               right )  => Sorter.Equals( left, right );
    public static        bool operator !=( ReadOnlyRectangleF              left,      ReadOnlyRectangleF               right )  => Sorter.DoesNotEqual( left, right );
    public static        bool operator >( ReadOnlyRectangleF               left,      ReadOnlyRectangleF               right )  => Sorter.GreaterThan( left, right );
    public static        bool operator >=( ReadOnlyRectangleF              left,      ReadOnlyRectangleF               right )  => Sorter.GreaterThanOrEqualTo( left, right );
    public static        bool operator <( ReadOnlyRectangleF               left,      ReadOnlyRectangleF               right )  => Sorter.LessThan( left, right );
    public static        bool operator <=( ReadOnlyRectangleF              left,      ReadOnlyRectangleF               right )  => Sorter.LessThanOrEqualTo( left, right );
    [Pure] public static ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, ReadOnlyThickness                margin ) => new((float)(rectangle.X - margin.Left), (float)(rectangle.Y - margin.Top), (float)(rectangle.Width + margin.Right), (float)(rectangle.Height + margin.Bottom));
    [Pure] public static ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, ReadOnlyThickness                margin ) => new((float)(rectangle.X + margin.Left), (float)(rectangle.Y + margin.Top), (float)(rectangle.Width - margin.Right), (float)(rectangle.Height - margin.Bottom));
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width + other.Width, rectangle.Height + other.Height);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width - other.Width, rectangle.Height - other.Height);
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X * other.X, rectangle.Y * other.Y, rectangle.Width * other.Width, rectangle.Height * other.Height);
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, ReadOnlyRectangleF               other )  => new(rectangle.X / other.X, rectangle.Y / other.Y, rectangle.Width / other.Width, rectangle.Height / other.Height);
    public static        ReadOnlyRectangleF operator &( ReadOnlyRectangleF rectangle, ReadOnlyPointF                   other )  => new(other.X, other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, ReadOnlyPointF                   other )  => new(rectangle.X + other.X, rectangle.Y + other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, ReadOnlyPointF                   other )  => new(rectangle.X - other.X, rectangle.Y - other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator &( ReadOnlyRectangleF rectangle, ReadOnlyPoint                    other )  => new((float)other.X, (float)other.Y, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, ReadOnlyPoint                    other )  => new((float)(rectangle.X + other.X), (float)(rectangle.Y + other.Y), rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, ReadOnlyPoint                    other )  => new((float)(rectangle.X - other.X), (float)(rectangle.Y - other.Y), rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width + value, rectangle.Height + value);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, double                           value )  => new(rectangle.X, rectangle.Y, (float)(rectangle.Width + value), (float)(rectangle.Height + value));
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width - value, rectangle.Height - value);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, double                           value )  => new(rectangle.X, rectangle.Y, (float)(rectangle.Width - value), (float)(rectangle.Height - value));
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width * value, rectangle.Height * value);
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, double                           value )  => new(rectangle.X, rectangle.Y, (float)(rectangle.Width * value), (float)(rectangle.Height * value));
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, int                              value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, float                            value )  => new(rectangle.X, rectangle.Y, rectangle.Width / value, rectangle.Height / value);
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, double                           value )  => new(rectangle.X, rectangle.Y, (float)(rectangle.Width / value), (float)(rectangle.Height / value));
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X + value.xOffset, rectangle.Y + value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator +( ReadOnlyRectangleF rectangle, (double xOffset, double yOffset) value )  => new((float)(rectangle.X + value.xOffset), (float)(rectangle.Y + value.yOffset), rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X - value.xOffset, rectangle.Y - value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator -( ReadOnlyRectangleF rectangle, (double xOffset, double yOffset) value )  => new((float)(rectangle.X - value.xOffset), (float)(rectangle.Y - value.yOffset), rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X * value.xOffset, rectangle.Y * value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator *( ReadOnlyRectangleF rectangle, (double xOffset, double yOffset) value )  => new((float)(rectangle.X * value.xOffset), (float)(rectangle.Y * value.yOffset), rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, (int xOffset, int yOffset)       value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, (float xOffset, float yOffset)   value )  => new(rectangle.X / value.xOffset, rectangle.Y / value.yOffset, rectangle.Width, rectangle.Height);
    public static        ReadOnlyRectangleF operator /( ReadOnlyRectangleF rectangle, (double xOffset, double yOffset) value )  => new((float)(rectangle.X / value.xOffset), (float)(rectangle.Y / value.yOffset), rectangle.Width, rectangle.Height);
}
