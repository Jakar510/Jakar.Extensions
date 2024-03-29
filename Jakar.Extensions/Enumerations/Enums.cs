﻿namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "PartialTypeWithSinglePart" )]
public static partial class Enums
{
    public static bool ToEnum<TResult>( this string input, out TResult result, bool ignoreCase = true )
        where TResult : struct => Enum.TryParse( input, ignoreCase, out result );


    /// <summary> Inspired by <seealso href="https://stackoverflow.com/a/8086788/9530917"/> </summary>
    /// <typeparam name="TEnum"> </typeparam>
    /// <typeparam name="TValue"> </typeparam>
    /// <returns> </returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode( nameof(GetEnumNamedValues) )]
#endif
    public static FrozenDictionary<string, TValue> GetEnumNamedValues<TEnum, TValue>()
        where TEnum : struct, Enum
    {
    #if NET6_0_OR_GREATER
        IEnumerable<TValue> values = Enum.GetValues<TEnum>().Cast<TValue>();
    #else
        IEnumerable<TValue> values = Enum.GetValues( typeof(TEnum) ).Cast<TValue>();
    #endif

        return values.ToFrozenDictionary( KeySelector, ElementSelector );

        static string KeySelector( TValue item ) => Enum.GetName( typeof(TEnum), item ?? throw new ArgumentNullException( nameof(item) ) ) ?? throw new NullReferenceException( nameof(item) );

        static TValue ElementSelector( TValue item ) => item;
    }


    public static TEnum GetEnumValue<TEnum>( this IReadOnlyDictionary<string, TEnum> dictionary, TEnum value )
        where TEnum : Enum => dictionary.First( pair => pair.Key == value.ToString() ).Value;


#if NET7_0_OR_GREATER
    [RequiresDynamicCode( nameof(GetEnumValue) )]
#endif
    public static TValue GetEnumValue<TValue, TEnum>( this TEnum value )
        where TEnum : struct, Enum => GetEnumNamedValues<TEnum, TValue>().First( pair => pair.Key == value.ToString() ).Value;
}
