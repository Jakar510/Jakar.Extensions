using Jakar.Extensions.Enumerations;



namespace Jakar.Extensions.General;


public static class DrawingSizeExtensions
{
    public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this int baseSize, params TEnum[] scales ) where TEnum : Enum => new Size(baseSize, baseSize).GetScaledSizes(scales);

    public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this Size baseSize, params TEnum[] scales ) where TEnum : Enum
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TEnum scale in scales )
        {
            int value = scale.GetEnumValue<int, TEnum>();
            yield return ( scale, baseSize.Scaled(value) );
        }
    }


    public static Size Scaled( this Size baseSize, in int value ) => new(baseSize.Width * value, baseSize.Height * value);
}
