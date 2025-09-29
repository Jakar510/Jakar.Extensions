// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  08:52

namespace Jakar.Shapes;


public static class Rectangles
{
    public static bool IsAtLeast<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther> => other.Width <= self.Width && other.Height <= self.Height;


    public static bool Contains<TRectangle, TPoint>( this TRectangle self, in TPoint other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TPoint : struct, IPoint<TPoint> => other.X >= self.X && other.X < self.MaxWidth() && other.Y >= self.Y && other.Y < self.MaxHeight();
    public static bool Contains<TRectangle, TPoint>( this TRectangle self, params ReadOnlySpan<TPoint> points )
        where TRectangle : struct, IRectangle<TRectangle>
        where TPoint : struct, IPoint<TPoint>
    {
        foreach ( ref readonly TPoint point in points )
        {
            if ( self.Contains(in point) ) { return true; }
        }

        return false;
    }
    public static bool ContainsAny<TRectangle, TPoint>( this TRectangle self, params ReadOnlySpan<TPoint> others )
        where TRectangle : struct, IRectangle<TRectangle>
        where TPoint : struct, IPoint<TPoint>
    {
        foreach ( ref readonly TPoint point in others )
        {
            if ( self.Contains(in point) ) { return true; }
        }

        return false;
    }
    public static bool ContainsAll<TRectangle, TPoint>( this TRectangle self, params ReadOnlySpan<TPoint> others )
        where TRectangle : struct, IRectangle<TRectangle>
        where TPoint : struct, IPoint<TPoint>
    {
        foreach ( ref readonly TPoint point in others )
        {
            if ( !self.Contains(in point) ) { return false; }
        }

        return true;
    }


    [Pure] public static ReadOnlyPoint TopLeft<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.X, self.Y);
    [Pure] public static ReadOnlyPoint TopRight<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.MaxWidth(), self.Y);
    [Pure] public static ReadOnlyPoint BottomLeft<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.X, self.MaxHeight());
    [Pure] public static ReadOnlyPoint BottomRight<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.MaxWidth(), self.MaxHeight());
    [Pure] public static double MaxHeight<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => self.Y + self.Height;
    [Pure] public static double MaxWidth<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => self.X + self.Width;
    [Pure] public static ReadOnlyLine BottomSide<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.BottomLeft(), self.BottomRight());
    [Pure] public static ReadOnlyLine LeftSide<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.TopLeft(), self.BottomLeft());
    [Pure] public static ReadOnlyLine RightSide<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.TopRight(), self.BottomRight());
    [Pure] public static ReadOnlyLine TopSide<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.TopLeft(), self.TopRight());


    [Pure] public static TRectangle Reverse<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location.Reverse(), self.Size.Reverse());
    [Pure] public static TRectangle Round<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X.Round(), self.Y.Round(), self.Width.Round(), self.Height.Round());
    [Pure] public static TRectangle Floor<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X.Floor(), self.Y.Floor(), self.Width.Floor(), self.Height.Floor());


    [Pure] public static bool Contains<TRectangle>( this TRectangle self, in MutableRectangle other )
        where TRectangle : struct, IRectangle<TRectangle> => self.X <= other.MaxWidth() && other.MaxWidth() >= self.X && self.Y <= other.MaxHeight() && self.MaxHeight() >= other.Y;


    [Pure] public static TRectangle Union<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther>
    {
        double x      = Math.Max(self.X, other.X);
        double y      = Math.Max(self.Y, other.Y);
        double width  = Math.Min(self.MaxWidth(),  other.MaxWidth())  - x;
        double height = Math.Min(self.MaxHeight(), other.MaxHeight()) - y;

        return width < 0 || height < 0
                   ? TRectangle.Zero
                   : TRectangle.Create(x, y, width, height);
    }
    [Pure] public static TRectangle SharedArea<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther>
    {
        double x      = Math.Max(self.X, other.X);
        double y      = Math.Max(self.Y, other.Y);
        double width  = Math.Min(self.MaxWidth(),  other.MaxWidth())  - x;
        double height = Math.Min(self.MaxHeight(), other.MaxHeight()) - y;

        return width < 0 || height < 0
                   ? TRectangle.Zero
                   : TRectangle.Create(x, y, width, height);
    }


    public static bool IntersectsWith<TRectangle, TOther>( this TRectangle self, in TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther> => !( ( self.X <= other.MaxWidth() && other.MaxWidth() >= self.X ) || ( self.Y <= other.MaxHeight() && self.MaxHeight() >= other.Y ) );
    public static bool DoesLineIntersect<TRectangle, TPoint>( this TRectangle self, in TPoint source, in TPoint target )
        where TRectangle : struct, IRectangle<TRectangle>
        where TPoint : struct, IPoint<TPoint>
    {
        double               t0          = 0.0;
        double               t1          = 1.0;
        double               dx          = target.X - source.X;
        double               dy          = target.Y - source.Y;
        ReadOnlySpan<double> boundariesX = [self.X, self.MaxWidth()];
        ReadOnlySpan<double> boundariesY = [self.Y, self.MaxHeight()];

        for ( int i = 0; i < 2; i++ )
        {
            double pX = i == 0
                            ? -dx
                            : dx;

            double pY = i == 0
                            ? -dy
                            : dy;

            for ( int j = 0; j < 2; j++ )
            {
                double qX = j == 0
                                ? source.X       - boundariesX[i]
                                : boundariesX[i] - source.X;

                double qY = j == 0
                                ? source.Y       - boundariesY[i]
                                : boundariesY[i] - source.Y;

                if ( pX == 0 && qX < 0 ) { return false; } // Line is parallel to the self's horizontal edge and outside of it

                if ( pY == 0 && qY < 0 ) { return false; } // Line is parallel to the self's vertical edge and outside of it

                double rX = pX != 0
                                ? qX / pX
                                : double.MaxValue;

                double rY = pY != 0
                                ? qY / pY
                                : double.MaxValue;

                if ( pX < 0 ) { t0 = Math.Max(t0, rX); }
                else { t1          = Math.Min(t1, rX); }

                if ( pY < 0 ) { t0 = Math.Max(t0, rY); }
                else { t1          = Math.Min(t1, rY); }

                if ( t0 > t1 ) { return false; }
            }
        }

        return true;
    }


    public static void Deconstruct<TRectangle>( this TRectangle self, out float x, out float y, out float width, out float height )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        x      = (float)self.X;
        y      = (float)self.Y;
        width  = (float)self.Width;
        height = (float)self.Height;
    }
    public static void Deconstruct<TRectangle>( this TRectangle self, out double x, out double y, out double width, out double height )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        x      = self.X;
        y      = self.Y;
        width  = self.Width;
        height = self.Height;
    }
    public static void Deconstruct<TRectangle>( this TRectangle self, out ReadOnlyPoint point, out ReadOnlySize size )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        size  = self.Size;
        point = self.Location;
    }
    public static void Deconstruct<TRectangle>( this TRectangle self, out ReadOnlyPointF point, out ReadOnlySizeF size )
        where TRectangle : struct, IRectangle<TRectangle>
    {
        size  = self.Size;
        point = self.Location;
    }


    public static ReadOnlyPoint Center<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => new(self.MaxWidth() / 2, self.MaxHeight() / 2);
    public static TRectangle Abs<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(double.Abs(self.X), double.Abs(self.Y), double.Abs(self.Width), double.Abs(self.Height));
    public static bool IsFinite<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => double.IsFinite(self.X) && double.IsFinite(self.Y) && double.IsFinite(self.Width) && double.IsFinite(self.Height);
    public static bool IsInfinity<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => double.IsInfinity(self.X) || double.IsInfinity(self.Y) || double.IsInfinity(self.Width) || double.IsInfinity(self.Height);
    public static bool IsInteger<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => double.IsInteger(self.X) && double.IsInteger(self.Y) && double.IsInteger(self.Width) && double.IsInteger(self.Height);
    public static bool IsNaN<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => double.IsNaN(self.X) || double.IsNaN(self.Y) || double.IsNaN(self.Width) || double.IsNaN(self.Height);
    public static bool IsNegative<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => self.Width < 0 || self.Height < 0;
    public static bool IsValid<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => self.IsNaN() || ( self.IsFinite() && ( self.Width <= 0 || self.Height <= 0 ) );
    public static bool IsPositive<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => self.Width > 0 || self.Height > 0;
    public static bool IsZero<TRectangle>( this TRectangle self )
        where TRectangle : struct, IRectangle<TRectangle> => self.Width == 0 || self.Height == 0;


    public static TRectangle Add<TRectangle, TOther>( this TRectangle self, TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther> => TRectangle.Create(self.X + other.X, self.Y + other.Y, self.Width + other.Width, self.Height + other.Height);
    public static TRectangle Subtract<TRectangle, TOther>( this TRectangle self, TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther> => TRectangle.Create(self.X - other.X, self.Y - other.Y, self.Width - other.Width, self.Height - other.Height);
    public static TRectangle Multiply<TRectangle, TOther>( this TRectangle self, TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther> => TRectangle.Create(self.X * other.X, self.Y * other.Y, self.Width * other.Width, self.Height * other.Height);
    public static TRectangle Divide<TRectangle, TOther>( this TRectangle self, TOther other )
        where TRectangle : struct, IRectangle<TRectangle>
        where TOther : struct, IRectangle<TOther> => TRectangle.Create(self.X / other.X, self.Y / other.Y, self.Width / other.Width, self.Height / other.Height);


    public static TRectangle Add<TRectangle>( this TRectangle self, (int xOffset, int yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size + other);
    public static TRectangle Subtract<TRectangle>( this TRectangle self, (int xOffset, int yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size - other);
    public static TRectangle Divide<TRectangle>( this TRectangle self, (int xOffset, int yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size / other);
    public static TRectangle Multiply<TRectangle>( this TRectangle self, (int xOffset, int yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size * other);


    public static TRectangle Add<TRectangle>( this TRectangle self, (float xOffset, float yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size + other);
    public static TRectangle Subtract<TRectangle>( this TRectangle self, (float xOffset, float yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size - other);
    public static TRectangle Divide<TRectangle>( this TRectangle self, (float xOffset, float yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size / other);
    public static TRectangle Multiply<TRectangle>( this TRectangle self, (float xOffset, float yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size * other);


    public static TRectangle Add<TRectangle>( this TRectangle self, (double xOffset, double yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size + other);
    public static TRectangle Subtract<TRectangle>( this TRectangle self, (double xOffset, double yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size - other);
    public static TRectangle Divide<TRectangle>( this TRectangle self, (double xOffset, double yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size / other);
    public static TRectangle Multiply<TRectangle>( this TRectangle self, (double xOffset, double yOffset) other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.Location, self.Size * other);


    public static TRectangle Add<TRectangle>( this TRectangle self, double other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width + other, self.Height + other);
    public static TRectangle Subtract<TRectangle>( this TRectangle self, double other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width - other, self.Height - other);
    public static TRectangle Multiply<TRectangle>( this TRectangle self, double other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width * other, self.Height * other);
    public static TRectangle Divide<TRectangle>( this TRectangle self, double other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width / other, self.Height / other);


    public static TRectangle Add<TRectangle>( this TRectangle self, float other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width + other, self.Height + other);
    public static TRectangle Subtract<TRectangle>( this TRectangle self, float other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width - other, self.Height - other);
    public static TRectangle Divide<TRectangle>( this TRectangle self, float other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width / other, self.Height / other);
    public static TRectangle Multiply<TRectangle>( this TRectangle self, float other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width * other, self.Height * other);


    public static TRectangle Add<TRectangle>( this TRectangle self, int other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width + other, self.Height + other);
    public static TRectangle Subtract<TRectangle>( this TRectangle self, int other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width - other, self.Height - other);
    public static TRectangle Divide<TRectangle>( this TRectangle self, int other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width / other, self.Height / other);
    public static TRectangle Multiply<TRectangle>( this TRectangle self, int other )
        where TRectangle : struct, IRectangle<TRectangle> => TRectangle.Create(self.X, self.Y, self.Width * other, self.Height * other);
}
