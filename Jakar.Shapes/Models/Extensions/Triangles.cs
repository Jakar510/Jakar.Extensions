// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  09:01

namespace Jakar.Shapes;


public static class Triangles
{
    public static string ToString<TTriangle>( this  TTriangle self, string? format )
        where TTriangle : struct, ITriangle<TTriangle>
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",":
                return $"{self.X},{self.Y}";

            case "-":
                return $"{self.X}-{self.Y}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TTriangle).Name}<{nameof(self.A)}: {self.A}, {nameof(self.B)}: {self.B}, {nameof(self.C)}: {self.C}>";
        }
    }


    public static void Deconstruct<TTriangle>( this  TTriangle self, out ReadOnlyPoint a, out ReadOnlyPoint b, out ReadOnlyPoint c )
        where TTriangle : struct, ITriangle<TTriangle>
    {
        a = self.A;
        b = self.B;
        c = self.C;
    }
    public static void Deconstruct<TTriangle>( this  TTriangle self, out ReadOnlyPointF a, out ReadOnlyPointF b, out ReadOnlyPointF c )
        where TTriangle : struct, ITriangle<TTriangle>
    {
        a = self.A;
        b = self.B;
        c = self.C;
    }


    public static ReadOnlyLine Ab<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(self.A, self.B);
    public static ReadOnlyLine Bc<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(self.B, self.C);
    public static ReadOnlyLine Ca<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(self.C, self.A);
    public static double Area<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => Math.Abs(0.5 * ( self.B.X - self.A.X ) * ( self.C.Y - self.A.Y ) - ( self.C.X - self.A.X ) * ( self.B.Y - self.A.Y ));
    public static ReadOnlyPoint Centroid<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(( self.A.X + self.B.X + self.C.X ) / 3, ( self.A.Y + self.B.Y + self.C.Y ) / 3);
    public static Degrees Abc<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(self.A.AngleBetween(self.B, self.C));
    public static Degrees Bac<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(self.B.AngleBetween(self.A, self.C));
    public static Degrees Cab<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => new(self.C.AngleBetween(self.A, self.B));


    public static TTriangle Abs<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A.Abs(), self.B.Abs(), self.C.Abs());
    public static bool IsFinite<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsFinite() && self.B.IsFinite() && self.C.IsFinite();
    public static bool IsInfinity<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsInfinity() || self.B.IsInfinity() || self.C.IsInfinity();
    public static bool IsInteger<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsInteger() && self.B.IsInteger() && self.C.IsInteger();
    public static bool IsNaN<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsNaN() || self.B.IsNaN() || self.C.IsNaN();
    public static bool IsNegative<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsNegative() || self.B.IsNegative() || self.C.IsNegative();
    public static bool IsValid<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => !self.IsNaN() && self.IsFinite() && !( self.A.IsOneOf(self.B, self.C) || self.B.IsOneOf(self.A, self.C) || self.C.IsOneOf(self.A, self.B) );
    public static bool IsPositive<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsPositive() || self.B.IsPositive() || self.C.IsPositive();
    public static bool IsZero<TTriangle>( this  TTriangle self )
        where TTriangle : struct, ITriangle<TTriangle> => self.A.IsZero() || self.B.IsZero() || self.C.IsZero();


    public static TTriangle Add<TTriangle, TOther>( this  TTriangle self, TOther value )
        where TTriangle : struct, ITriangle<TTriangle>
        where TOther : struct, ITriangle<TOther> => TTriangle.Create(self.A + value.A, self.B + value.B, self.C + value.C);
    public static TTriangle Subtract<TTriangle, TOther>( this  TTriangle self, TOther value )
        where TTriangle : struct, ITriangle<TTriangle>
        where TOther : struct, ITriangle<TOther> => TTriangle.Create(self.A - value.A, self.B - value.B, self.C - value.C);
    public static TTriangle Multiply<TTriangle, TOther>( this  TTriangle self, TOther value )
        where TTriangle : struct, ITriangle<TTriangle>
        where TOther : struct, ITriangle<TOther> => TTriangle.Create(self.A * value.A, self.B * value.B, self.C * value.C);
    public static TTriangle Divide<TTriangle, TOther>( this  TTriangle self, TOther value )
        where TTriangle : struct, ITriangle<TTriangle>
        where TOther : struct, ITriangle<TOther> => TTriangle.Create(self.A / value.A, self.B / value.B, self.C / value.C);
    public static TTriangle Add<TTriangle>( this  TTriangle self, (int xOffset, int yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A + value, self.B + value, self.C + value);
    public static TTriangle Subtract<TTriangle>( this  TTriangle self, (int xOffset, int yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A - value, self.B - value, self.C - value);
    public static TTriangle Divide<TTriangle>( this  TTriangle self, (int xOffset, int yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Multiply<TTriangle>( this  TTriangle self, (int xOffset, int yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A * value, self.B * value, self.C * value);
    public static TTriangle Add<TTriangle>( this  TTriangle self, (float xOffset, float yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A + value, self.B + value, self.C + value);
    public static TTriangle Multiply<TTriangle>( this  TTriangle self, (float xOffset, float yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A * value, self.B * value, self.C * value);
    public static TTriangle Divide<TTriangle>( this  TTriangle self, (float xOffset, float yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Subtract<TTriangle>( this  TTriangle self, (float xOffset, float yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A - value, self.B - value, self.C - value);
    public static TTriangle Add<TTriangle>( this  TTriangle self, (double xOffset, double yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A + value, self.B + value, self.C + value);
    public static TTriangle Subtract<TTriangle>( this  TTriangle self, (double xOffset, double yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A - value, self.B - value, self.C - value);
    public static TTriangle Divide<TTriangle>( this  TTriangle self, (double xOffset, double yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Multiply<TTriangle>( this  TTriangle self, (double xOffset, double yOffset) value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A * value, self.B * value, self.C * value);
    public static TTriangle Add<TTriangle>( this  TTriangle self, double value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A + value, self.B + value, self.C + value);
    public static TTriangle Subtract<TTriangle>( this  TTriangle self, double value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A - value, self.B - value, self.C - value);
    public static TTriangle Multiply<TTriangle>( this  TTriangle self, double value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Divide<TTriangle>( this  TTriangle self, double value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Add<TTriangle>( this  TTriangle self, float value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A + value, self.B + value, self.C + value);
    public static TTriangle Subtract<TTriangle>( this  TTriangle self, float value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A - value, self.B - value, self.C - value);
    public static TTriangle Divide<TTriangle>( this  TTriangle self, float value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Multiply<TTriangle>( this  TTriangle self, float value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A * value, self.B * value, self.C * value);
    public static TTriangle Add<TTriangle>( this  TTriangle self, int value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A + value, self.B + value, self.C + value);
    public static TTriangle Subtract<TTriangle>( this  TTriangle self, int value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A - value, self.B - value, self.C - value);
    public static TTriangle Divide<TTriangle>( this  TTriangle self, int value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A / value, self.B / value, self.C / value);
    public static TTriangle Multiply<TTriangle>( this  TTriangle self, int value )
        where TTriangle : struct, ITriangle<TTriangle> => TTriangle.Create(self.A * value, self.B * value, self.C * value);
}
