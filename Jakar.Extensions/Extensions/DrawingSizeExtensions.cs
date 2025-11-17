namespace Jakar.Extensions;


public static class DrawingSizeExtensions
{
    [RequiresDynamicCode(nameof(GetScaledSizes))] public static IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( this int baseSize, params TEnum[] scales )
        where TEnum : unmanaged, Enum => new Size(baseSize, baseSize).GetScaledSizes(scales);


    extension( Size baseSize )
    {
        [RequiresDynamicCode(nameof(GetScaledSizes))] public IEnumerable<(TEnum, Size)> GetScaledSizes<TEnum>( params TEnum[] scales )
            where TEnum : unmanaged, Enum
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( TEnum scale in scales )
            {
                int value = scale.GetEnumValue<int, TEnum>();
                yield return ( scale, baseSize.Scaled(value) );
            }
        }
        public Size Scaled( int value ) => new(baseSize.Width * value, baseSize.Height * value);
    }
}
