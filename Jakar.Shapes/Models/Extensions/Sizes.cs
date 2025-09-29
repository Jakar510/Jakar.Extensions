// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  09:01

namespace Jakar.Shapes;


public static class Sizes
{
    public static string ToString<TSize>( this TSize self, string? format )
        where TSize : struct, ISize<TSize>
    {
        switch ( format )
        {
            case "json":
            case "JSON":
            case "Json":
                return self.ToJson();

            case ",":
                return $"{self.Width},{self.Height}";

            case "-":
                return $"{self.Width}-{self.Height}";

            case EMPTY:
            case null:
            default:
                return $"{typeof(TSize).Name}<{nameof(self.Width)}: {self.Width}, {nameof(self.Height)}: {self.Height}>";
        }
    }


    [Pure] public static TSize Reverse<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Height, self.Width);
    [Pure] public static TSize Round<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width.Round(), self.Height.Round());
    [Pure] public static TSize Floor<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width.Floor(), self.Height.Floor());


    public static void Deconstruct<TSize>( this TSize self, out float width, out float height )
        where TSize : struct, ISize<TSize>
    {
        width  = (float)self.Width;
        height = (float)self.Height;
    }
    public static void Deconstruct<TSize>( this TSize self, out double width, out double height )
        where TSize : struct, ISize<TSize>
    {
        width  = self.Width;
        height = self.Height;
    }


    public static bool IsPortrait<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => self.Width > self.Height;
    public static bool IsLandscape<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => self.Width > self.Height;


    public static TSize Abs<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => TSize.Create(double.Abs(self.Width), double.Abs(self.Height));
    public static bool IsFinite<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => double.IsFinite(self.Width) && double.IsFinite(self.Height);
    public static bool IsInfinity<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => double.IsInfinity(self.Width) || double.IsInfinity(self.Height);
    public static bool IsInteger<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => double.IsInteger(self.Width) && double.IsInteger(self.Height);
    public static bool IsNaN<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => double.IsNaN(self.Width) || double.IsNaN(self.Height);
    public static bool IsNegative<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => self is { Width: < 0, Height: < 0 };
    public static bool IsValid<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => !self.IsNaN() && self.IsFinite() && self.IsPositive();
    public static bool IsPositive<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => self is { Width: > 0, Height: > 0 };
    public static bool IsZero<TSize>( this TSize self )
        where TSize : struct, ISize<TSize> => self is { Width: 0, Height: 0 };


    public static TSize Add<TSize>( this TSize self, Size value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value.Width, self.Height + value.Height);
    public static TSize Subtract<TSize>( this TSize self, Size value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value.Width, self.Height - value.Height);
    public static TSize Multiply<TSize>( this TSize self, Size value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value.Width, self.Height * value.Height);
    public static TSize Divide<TSize>( this TSize self, Size value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value.Width, self.Height / value.Height);
    public static TSize Add<TSize>( this TSize self, SizeF value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value.Width, self.Height + value.Height);
    public static TSize Subtract<TSize>( this TSize self, SizeF value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value.Width, self.Height - value.Height);
    public static TSize Multiply<TSize>( this TSize self, SizeF value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value.Width, self.Height * value.Height);
    public static TSize Divide<TSize>( this TSize self, SizeF value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value.Width, self.Height / value.Height);
    public static TSize Add<TSize, TOther>( this TSize self, TOther value )
        where TSize : struct, ISize<TSize>
        where TOther : struct, ISize<TOther> => TSize.Create(self.Width + value.Width, self.Height + value.Height);
    public static TSize Subtract<TSize, TOther>( this TSize self, TOther value )
        where TSize : struct, ISize<TSize>
        where TOther : struct, ISize<TOther> => TSize.Create(self.Width - value.Width, self.Height - value.Height);
    public static TSize Multiply<TSize, TOther>( this TSize self, TOther value )
        where TSize : struct, ISize<TSize>
        where TOther : struct, ISize<TOther> => TSize.Create(self.Width * value.Width, self.Height * value.Height);
    public static TSize Divide<TSize, TOther>( this TSize self, TOther value )
        where TSize : struct, ISize<TSize>
        where TOther : struct, ISize<TOther> => TSize.Create(self.Width / value.Width, self.Height / value.Height);
    public static TSize Add<TSize>( this TSize self, (int xOffset, int yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value.xOffset, self.Height + value.yOffset);
    public static TSize Subtract<TSize>( this TSize self, (int xOffset, int yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value.xOffset, self.Height - value.yOffset);
    public static TSize Divide<TSize>( this TSize self, (int xOffset, int yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value.xOffset, self.Height / value.yOffset);
    public static TSize Multiply<TSize>( this TSize self, (int xOffset, int yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value.xOffset, self.Height * value.yOffset);
    public static TSize Add<TSize>( this TSize self, (float xOffset, float yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value.xOffset, self.Height + value.yOffset);
    public static TSize Multiply<TSize>( this TSize self, (float xOffset, float yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value.xOffset, self.Height * value.yOffset);
    public static TSize Divide<TSize>( this TSize self, (float xOffset, float yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value.xOffset, self.Height / value.yOffset);
    public static TSize Subtract<TSize>( this TSize self, (float xOffset, float yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value.xOffset, self.Height - value.yOffset);
    public static TSize Add<TSize>( this TSize self, (double xOffset, double yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value.xOffset, self.Height + value.yOffset);
    public static TSize Subtract<TSize>( this TSize self, (double xOffset, double yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value.xOffset, self.Height - value.yOffset);
    public static TSize Divide<TSize>( this TSize self, (double xOffset, double yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value.xOffset, self.Height / value.yOffset);
    public static TSize Multiply<TSize>( this TSize self, (double xOffset, double yOffset) value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value.xOffset, self.Height * value.yOffset);
    public static TSize Add<TSize>( this TSize self, double value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value, self.Height + value);
    public static TSize Subtract<TSize>( this TSize self, double value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value, self.Height - value);
    public static TSize Multiply<TSize>( this TSize self, double value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value, self.Height * value);
    public static TSize Divide<TSize>( this TSize self, double value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value, self.Height / value);
    public static TSize Add<TSize>( this TSize self, float value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value, self.Height + value);
    public static TSize Subtract<TSize>( this TSize self, float value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value, self.Height - value);
    public static TSize Divide<TSize>( this TSize self, float value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value, self.Height / value);
    public static TSize Multiply<TSize>( this TSize self, float value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value, self.Height * value);
    public static TSize Add<TSize>( this TSize self, int value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width + value, self.Height + value);
    public static TSize Subtract<TSize>( this TSize self, int value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width - value, self.Height - value);
    public static TSize Divide<TSize>( this TSize self, int value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width / value, self.Height / value);
    public static TSize Multiply<TSize>( this TSize self, int value )
        where TSize : struct, ISize<TSize> => TSize.Create(self.Width * value, self.Height * value);
}
