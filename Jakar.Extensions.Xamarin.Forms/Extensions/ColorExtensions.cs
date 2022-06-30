


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public static class ColorExtensions
{
    public static Color ToXfColor( this System.Drawing.Color color ) => new(color.R, color.G, color.B, color.A);

    public static System.Drawing.Color ToDrawingColor( this Color color ) => System.Drawing.Color.FromArgb(color.A.AsInt(), color.R.AsInt(), color.G.AsInt(), color.B.AsInt());
}
