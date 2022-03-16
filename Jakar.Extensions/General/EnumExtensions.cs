namespace Jakar.Extensions.General;


public static class EnumExtensions
{
    public static TValue GetEnumValue<TValue, TEnum>( this TEnum value ) where TEnum : Enum { return GetEnumNamedValues<TEnum, TValue>().First(pair => pair.Key == value.ToString()).Value; }


    /// <summary>
    /// Inspired by <seealso href="https://stackoverflow.com/a/8086788/9530917"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Dictionary<string, TValue> GetEnumNamedValues<TEnum, TValue>() where TEnum : Enum
    {
        IEnumerable<TValue> values = Enum.GetValues(typeof(TEnum)).Cast<TValue>();

        static string KeySelector( TValue     item ) => Enum.GetName(typeof(TEnum), item ?? throw new ArgumentNullException(nameof(item))) ?? throw new NullReferenceException(nameof(item));
        static TValue ElementSelector( TValue item ) => item;

        return values.ToDictionary(KeySelector, ElementSelector);
    }


    public static bool ToEnum<TResult>( this string input, out TResult result, bool ignoreCase = true ) where TResult : struct => Enum.TryParse(input, ignoreCase, out result);
}
