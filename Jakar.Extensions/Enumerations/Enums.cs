namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
public static partial class Enums
{
    public static bool ToEnum<TResult>( this string input, out TResult result, bool ignoreCase = true )
        where TResult : struct => Enum.TryParse(input, ignoreCase, out result);


    /// <summary> Inspired by <seealso href="https://stackoverflow.com/a/8086788/9530917"/> </summary>
    /// <typeparam name="TEnum"> </typeparam>
    /// <typeparam name="TValue"> </typeparam>
    /// <returns> </returns>
    [RequiresDynamicCode(nameof(GetEnumNamedValues))] public static FrozenDictionary<string, TValue> GetEnumNamedValues<TEnum, TValue>()
        where TEnum : struct, Enum
    {
        IEnumerable<TValue> values = Enum.GetValues<TEnum>()
                                         .Cast<TValue>();

        return values.ToFrozenDictionary(keySelector, elementSelector);

        static string keySelector( TValue item ) => Enum.GetName(typeof(TEnum), item ?? throw new ArgumentNullException(nameof(item))) ?? throw new NullReferenceException(nameof(item));

        static TValue elementSelector( TValue item ) => item;
    }


    public static TEnum GetEnumValue<TEnum>( this IReadOnlyDictionary<string, TEnum> dictionary, TEnum value )
        where TEnum : Enum => dictionary.First(pair => pair.Key == value.ToString())
                                        .Value;


    [RequiresDynamicCode(nameof(GetEnumValue))] public static TValue GetEnumValue<TValue, TEnum>( this TEnum value )
        where TEnum : struct, Enum => GetEnumNamedValues<TEnum, TValue>()
                                     .First(pair => pair.Key == value.ToString())
                                     .Value;
}
