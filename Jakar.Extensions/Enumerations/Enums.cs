﻿namespace Jakar.Extensions.Enumerations;


[SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
public static partial class Enums
{
    public static TValue GetEnumValue<TValue, TEnum>( this TEnum  value ) where TEnum : Enum => GetEnumNamedValues<TEnum, TValue>().First(pair => pair.Key == value.ToString()).Value;
    public static bool ToEnum<TResult>( this               string input, out TResult result, bool ignoreCase = true ) where TResult : struct => Enum.TryParse(input, ignoreCase, out result);


    /// <summary>
    ///     Inspired by
    ///     <seealso href = "https://stackoverflow.com/a/8086788/9530917" />
    /// </summary>
    /// <typeparam name = "TEnum" > </typeparam>
    /// <typeparam name = "TValue" > </typeparam>
    /// <returns> </returns>
    public static IReadOnlyDictionary<string, TValue> GetEnumNamedValues<TEnum, TValue>() where TEnum : Enum
    {
        IEnumerable<TValue> values = Enum.GetValues(typeof(TEnum)).Cast<TValue>();

        static string KeySelector( TValue item ) { return Enum.GetName(typeof(TEnum), item ?? throw new ArgumentNullException(nameof(item))) ?? throw new NullReferenceException(nameof(item)); }

        static TValue ElementSelector( TValue item ) { return item; }

        return values.ToDictionary(KeySelector, ElementSelector);
    }
}