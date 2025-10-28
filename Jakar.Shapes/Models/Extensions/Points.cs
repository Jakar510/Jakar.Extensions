// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  09:00

namespace Jakar.Shapes;


public static class Points
{
    [Pure] public static TPoint Reverse<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.Y, self.X);
    [Pure] public static TPoint Round<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X.Round(), self.Y.Round());
    [Pure] public static TPoint Floor<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X.Floor(), self.Y.Floor());


    public static void Deconstruct<TPoint>( this TPoint self, out float x, out float y )
        where TPoint : struct, IPoint<TPoint>
    {
        x = (float)self.X;
        y = (float)self.Y;
    }
    public static void Deconstruct<TPoint>( this TPoint self, out double x, out double y )
        where TPoint : struct, IPoint<TPoint>
    {
        x = self.X;
        y = self.Y;
    }


    public static TPoint Abs<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(double.Abs(self.X), double.Abs(self.Y));
    public static bool IsFinite<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => double.IsFinite(self.X) && double.IsFinite(self.Y);
    public static bool IsInfinity<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => double.IsInfinity(self.X) || double.IsInfinity(self.Y);
    public static bool IsInteger<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => double.IsInteger(self.X) && double.IsInteger(self.Y);
    public static bool IsNaN<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => double.IsNaN(self.X) || double.IsNaN(self.Y);
    public static bool IsNegative<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => self is { X: < 0, Y: < 0 };
    public static bool IsValid<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => !self.IsNaN() && self.IsFinite();
    public static bool IsPositive<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => self is { X: > 0, Y: > 0 };
    public static bool IsZero<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => self is { X: 0, Y: 0 };


    public static double DistanceTo<TPoint, TOther>( this TPoint self, TOther other )
        where TPoint : struct, IPoint<TPoint>
        where TOther : struct, IPoint<TOther>
    {
        double x      = self.X - other.X;
        double y      = self.Y - other.Y;
        double x2     = x * x;
        double y2     = y * y;
        double result = Math.Sqrt(x2 + y2);
        return result;
    }
    public static double Dot<TPoint>( this TPoint self, TPoint other )
        where TPoint : struct, IPoint<TPoint> => self.X * other.X + self.Y * other.Y;
    public static double Magnitude<TPoint>( this TPoint self )
        where TPoint : struct, IPoint<TPoint> => Math.Sqrt(self.X * self.X + self.Y * self.Y);
    public static double AngleBetween<TPoint>( this TPoint self, TPoint p1, TPoint p2 )
        where TPoint : struct, IPoint<TPoint>
    {
        TPoint v1 = self.Subtract(p1);
        TPoint v2 = self.Subtract(p2);

        double dot  = v1.Dot(v2);
        double mag1 = v1.Magnitude();
        double mag2 = v2.Magnitude();
        if ( mag1 == 0 || mag2 == 0 ) { return 0; }

        double cosTheta = dot / ( mag1 * mag2 );
        cosTheta = Math.Clamp(cosTheta, -1.0, 1.0); // Avoid NaN due to precision

        return Math.Acos(cosTheta); // In radians
    }


    public static TPoint Add<TPoint, TOther>( this TPoint self, TOther value )
        where TPoint : struct, IPoint<TPoint>
        where TOther : struct, IPoint<TOther> => TPoint.Create(self.X + value.X, self.Y + value.Y);
    public static TPoint Subtract<TPoint, TOther>( this TPoint self, TOther value )
        where TPoint : struct, IPoint<TPoint>
        where TOther : struct, IPoint<TOther> => TPoint.Create(self.X - value.X, self.Y - value.Y);
    public static TPoint Multiply<TPoint, TOther>( this TPoint self, TOther value )
        where TPoint : struct, IPoint<TPoint>
        where TOther : struct, IPoint<TOther> => TPoint.Create(self.X * value.X, self.Y * value.Y);
    public static TPoint Divide<TPoint, TOther>( this TPoint self, TOther value )
        where TPoint : struct, IPoint<TPoint>
        where TOther : struct, IPoint<TOther> => TPoint.Create(self.X / value.X, self.Y / value.Y);
    public static TPoint Add<TPoint>( this TPoint self, (int xOffset, int yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X + value.xOffset, self.Y + value.yOffset);
    public static TPoint Subtract<TPoint>( this TPoint self, (int xOffset, int yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X - value.xOffset, self.Y - value.yOffset);
    public static TPoint Divide<TPoint>( this TPoint self, (int xOffset, int yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X / value.xOffset, self.Y / value.yOffset);
    public static TPoint Multiply<TPoint>( this TPoint self, (int xOffset, int yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X * value.xOffset, self.Y * value.yOffset);
    public static TPoint Add<TPoint>( this TPoint self, (float xOffset, float yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X + value.xOffset, self.Y + value.yOffset);
    public static TPoint Multiply<TPoint>( this TPoint self, (float xOffset, float yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X * value.xOffset, self.Y * value.yOffset);
    public static TPoint Divide<TPoint>( this TPoint self, (float xOffset, float yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X / value.xOffset, self.Y / value.yOffset);
    public static TPoint Subtract<TPoint>( this TPoint self, (float xOffset, float yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X - value.xOffset, self.Y - value.yOffset);
    public static TPoint Add<TPoint>( this TPoint self, (double xOffset, double yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X + value.xOffset, self.Y + value.yOffset);
    public static TPoint Subtract<TPoint>( this TPoint self, (double xOffset, double yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X - value.xOffset, self.Y - value.yOffset);
    public static TPoint Divide<TPoint>( this TPoint self, (double xOffset, double yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X / value.xOffset, self.Y / value.yOffset);
    public static TPoint Multiply<TPoint>( this TPoint self, (double xOffset, double yOffset) value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X * value.xOffset, self.Y * value.yOffset);
    public static TPoint Add<TPoint>( this TPoint self, double value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X + value, self.Y + value);
    public static TPoint Subtract<TPoint>( this TPoint self, double value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X - value, self.Y - value);
    public static TPoint Multiply<TPoint>( this TPoint self, double value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X * value, self.Y * value);
    public static TPoint Divide<TPoint>( this TPoint self, double value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X / value, self.Y / value);
    public static TPoint Add<TPoint>( this TPoint self, float value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X + value, self.Y + value);
    public static TPoint Subtract<TPoint>( this TPoint self, float value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X - value, self.Y - value);
    public static TPoint Divide<TPoint>( this TPoint self, float value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X / value, self.Y / value);
    public static TPoint Multiply<TPoint>( this TPoint self, float value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X * value, self.Y * value);
    public static TPoint Add<TPoint>( this TPoint self, int value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X + value, self.Y + value);
    public static TPoint Subtract<TPoint>( this TPoint self, int value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X - value, self.Y - value);
    public static TPoint Divide<TPoint>( this TPoint self, int value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X / value, self.Y / value);
    public static TPoint Multiply<TPoint>( this TPoint self, int value )
        where TPoint : struct, IPoint<TPoint> => TPoint.Create(self.X * value, self.Y * value);
}
