namespace Jakar.Extensions;


public static class DrawingSizeExtensions
{
    [RequiresDynamicCode(nameof(GetScaledSizes))] public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this int baseSize, params TEnum[] scales )
        where TEnum : struct, Enum => new Size(baseSize, baseSize).GetScaledSizes(scales);


    [RequiresDynamicCode(nameof(GetScaledSizes))] public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this Size baseSize, params TEnum[] scales )
        where TEnum : struct, Enum
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TEnum scale in scales )
        {
            int value = scale.GetEnumValue<int, TEnum>();
            yield return ( scale, baseSize.Scaled(value) );
        }
    }


    public static Size Scaled( this Size baseSize, int value ) => new(baseSize.Width * value, baseSize.Height * value);
}
