#nullable enable
namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "PartialTypeWithSinglePart" )]
public static partial class Enums
{
    /// <summary> Inspired by <seealso href="https://stackoverflow.com/a/8086788/9530917"/> </summary>
    /// <typeparam name="TEnum"> </typeparam>
    /// <typeparam name="TValue"> </typeparam>
    /// <returns> </returns>
    public static IReadOnlyDictionary<string, TValue> GetEnumNamedValues<TEnum, TValue>() where TEnum : Enum
    {
        IEnumerable<TValue> values = Enum.GetValues( typeof(TEnum) )
                                         .Cast<TValue>();

        static string KeySelector( TValue item ) => Enum.GetName( typeof(TEnum), item ?? throw new ArgumentNullException( nameof(item) ) ) ?? throw new NullReferenceException( nameof(item) );

        static TValue ElementSelector( TValue item ) => item;

        return values.ToDictionary( KeySelector, ElementSelector );
    }
    public static TValue GetEnumValue<TValue, TEnum>( this TEnum value ) where TEnum : Enum => GetEnumNamedValues<TEnum, TValue>()
                                                                                              .First( pair => pair.Key == value.ToString() )
                                                                                              .Value;
    public static TEnum GetEnumValue<TEnum>( this IReadOnlyDictionary<string, TEnum> dictionary, TEnum value ) where TEnum : Enum => dictionary.First( pair => pair.Key == value.ToString() )
                                                                                                                                               .Value;
    public static bool ToEnum<TResult>( this string input, out TResult result, bool ignoreCase = true ) where TResult : struct => Enum.TryParse( input, ignoreCase, out result );
}
