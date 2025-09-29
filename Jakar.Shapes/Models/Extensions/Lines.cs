// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  09:01

namespace Jakar.Shapes;


public static class Lines
{
    public static string ToString<TLine>( this  TLine self, string? format )
        where TLine : struct, ILine<TLine>
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson(TLine.JsonTypeInfo);

            case ",":
                return $"{self.Start.ToString(format, null)},{self.End.ToString(format, null)}";

            case "-":
                return $"{self.Start.ToString(format, null)}-{self.End.ToString(format, null)}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TLine).Name}<{nameof(self.Start)}: {self.Start}, {nameof(self.End)}: {self.End}, {nameof(IsFinite)}: {self.IsFinite}>";
        }
    }


    public static void Deconstruct<TLine>( this  TLine self, out ReadOnlyPoint start, out ReadOnlyPoint end, out bool isFinite )
        where TLine : struct, ILine<TLine>
    {
        start    = self.Start;
        end      = self.End;
        isFinite = self.IsFinite;
    }
    public static void Deconstruct<TLine>( this  TLine self, out float x1, out float y1, out float x2, out float y2 )
        where TLine : struct, ILine<TLine>
    {
        ReadOnlyPoint start = self.Start;
        ReadOnlyPoint end   = self.End;
        x1 = (float)start.X;
        y1 = (float)start.Y;
        x2 = (float)end.X;
        y2 = (float)end.Y;
    }
    public static void Deconstruct<TLine>( this  TLine self, out double x1, out double y1, out double x2, out double y2 )
        where TLine : struct, ILine<TLine>
    {
        ReadOnlyPoint start = self.Start;
        ReadOnlyPoint end   = self.End;
        x1 = start.X;
        y1 = start.Y;
        x2 = end.X;
        y2 = end.Y;
    }
    public static void Deconstruct<TLine>( this  TLine self, out ReadOnlyPointF start, out ReadOnlyPointF end )
        where TLine : struct, ILine<TLine>
    {
        start = self.Start;
        end   = self.End;
    }
    public static void Deconstruct<TLine>( this  TLine self, out ReadOnlyPoint start, out ReadOnlyPoint end )
        where TLine : struct, ILine<TLine>
    {
        start = self.Start;
        end   = self.End;
    }


    [Pure] public static TLine Round<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start.Round(), self.End.Round(), self.IsFinite);
    [Pure] public static TLine Floor<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start.Floor(), self.End.Floor(), self.IsFinite);


    public static ReadOnlyPoint Center<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => new(( self.Start.Y + self.End.Y ) / 2, ( self.Start.X + self.End.X ) / 2);
    public static double Slope<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => ( self.End.Y - self.Start.Y ) / ( self.End.X - self.Start.X );


    public static TLine Abs<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start.Abs(), self.End.Abs(), self.IsFinite);
    public static bool IsFinite<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsFinite() && self.End.IsFinite();
    public static bool IsInfinity<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsInfinity() || self.End.IsInfinity();
    public static bool IsInteger<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsInteger() || self.End.IsInteger();
    public static bool IsNaN<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsNaN() || self.End.IsNaN();
    public static bool IsNegative<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsNegative() || self.End.IsNegative();
    public static bool IsValid<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsValid() || self.End.IsValid();
    public static bool IsPositive<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsNaN() || self.End.IsNaN();
    public static bool IsZero<TLine>( this  TLine self )
        where TLine : struct, ILine<TLine> => self.Start.IsZero() || self.End.IsZero();


    public static TLine WithStart<TLine, TOther>( this  TLine self, TOther value )
        where TLine : struct, ILine<TLine, TOther>
        where TOther : struct, IPoint<TOther> => TLine.Create(value, self.End);
    public static TLine WithEnd<TLine, TOther>( this  TLine self, TOther value )
        where TLine : struct, ILine<TLine, TOther>
        where TOther : struct, IPoint<TOther> => TLine.Create(self.Start, value);
    public static TLine Add<TLine, TOther>( this  TLine self, TOther value )
        where TLine : struct, ILine<TLine>
        where TOther : struct, ILine<TOther> => TLine.Create(self.Start + value.Start, self.End + value.End);
    public static TLine Subtract<TLine, TOther>( this  TLine self, TOther value )
        where TLine : struct, ILine<TLine>
        where TOther : struct, ILine<TOther> => TLine.Create(self.Start - value.Start, self.End - value.End);
    public static TLine Multiply<TLine, TOther>( this  TLine self, TOther value )
        where TLine : struct, ILine<TLine>
        where TOther : struct, ILine<TOther> => TLine.Create(self.Start * value.Start, self.End * value.End);
    public static TLine Divide<TLine, TOther>( this  TLine self, TOther value )
        where TLine : struct, ILine<TLine>
        where TOther : struct, ILine<TOther> => TLine.Create(self.Start / value.Start, self.End / value.End);
    public static TLine Add<TLine>( this  TLine self, (int xOffset, int yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start + value.xOffset, self.End + value.yOffset);
    public static TLine Subtract<TLine>( this  TLine self, (int xOffset, int yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start - value.xOffset, self.End - value.yOffset);
    public static TLine Divide<TLine>( this  TLine self, (int xOffset, int yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start / value.xOffset, self.End / value.yOffset);
    public static TLine Multiply<TLine>( this  TLine self, (int xOffset, int yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start * value.xOffset, self.End * value.yOffset);
    public static TLine Add<TLine>( this  TLine self, (float xOffset, float yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start + value.xOffset, self.End + value.yOffset);
    public static TLine Multiply<TLine>( this  TLine self, (float xOffset, float yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start * value.xOffset, self.End * value.yOffset);
    public static TLine Divide<TLine>( this  TLine self, (float xOffset, float yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start / value.xOffset, self.End / value.yOffset);
    public static TLine Subtract<TLine>( this  TLine self, (float xOffset, float yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start - value.xOffset, self.End - value.yOffset);
    public static TLine Add<TLine>( this  TLine self, (double xOffset, double yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start + value.xOffset, self.End + value.yOffset);
    public static TLine Subtract<TLine>( this  TLine self, (double xOffset, double yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start - value.xOffset, self.End - value.yOffset);
    public static TLine Divide<TLine>( this  TLine self, (double xOffset, double yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start / value.xOffset, self.End / value.yOffset);
    public static TLine Multiply<TLine>( this  TLine self, (double xOffset, double yOffset) value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start * value.xOffset, self.End * value.yOffset);
    public static TLine Add<TLine>( this  TLine self, double value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start + value, self.End + value);
    public static TLine Subtract<TLine>( this  TLine self, double value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start - value, self.End - value);
    public static TLine Multiply<TLine>( this  TLine self, double value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start * value, self.End * value);
    public static TLine Divide<TLine>( this  TLine self, double value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start / value, self.End / value);
    public static TLine Add<TLine>( this  TLine self, float value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start + value, self.End + value);
    public static TLine Subtract<TLine>( this  TLine self, float value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start - value, self.End - value);
    public static TLine Divide<TLine>( this  TLine self, float value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start / value, self.End / value);
    public static TLine Multiply<TLine>( this  TLine self, float value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start * value, self.End * value);
    public static TLine Add<TLine>( this  TLine self, int value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start + value, self.End + value);
    public static TLine Subtract<TLine>( this  TLine self, int value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start - value, self.End - value);
    public static TLine Divide<TLine>( this  TLine self, int value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start / value, self.End / value);
    public static TLine Multiply<TLine>( this  TLine self, int value )
        where TLine : struct, ILine<TLine> => TLine.Create(self.Start * value, self.End * value);
}
