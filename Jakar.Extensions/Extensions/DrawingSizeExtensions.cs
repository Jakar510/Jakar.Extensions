#nullable enable
using Newtonsoft.Json.Linq;



namespace Jakar.Extensions;


public static class DrawingSizeExtensions
{
    public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this int  baseSize, params TEnum[] scales ) where TEnum : struct, Enum => new Size( baseSize, baseSize ).GetScaledSizes( scales );
    public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this Size baseSize, params TEnum[] scales ) where TEnum : struct, Enum => scales.Select( scale => (scale, baseSize.Scaled( scale.AsInt() )) );
    public static Size Scaled( this                                      Size baseSize, in     int     value ) => new(baseSize.Width * value, baseSize.Height * value);
}
