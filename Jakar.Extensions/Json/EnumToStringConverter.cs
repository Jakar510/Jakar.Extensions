// Jakar.Extensions :: Jakar.Extensions
// 05/01/2023  3:58 PM

namespace Jakar.Extensions;


public abstract class EnumToStringConverter<TSelf, TEnum>() : JsonConverter<TEnum>()
    where TEnum : struct, Enum
    where TSelf : EnumToStringConverter<TSelf, TEnum>, new()
{
    public static readonly    TSelf                          Instance      = new();
    protected static readonly FrozenDictionary<TEnum, string> _enumToString = Enum.GetValues<TEnum>().ToFrozenDictionary(Self,     ToString);
    protected static readonly FrozenDictionary<string, TEnum> _stringToEnum = Enum.GetValues<TEnum>().ToFrozenDictionary(ToString, Self);
    protected static readonly FrozenDictionary<long, TEnum>   _intToEnum    = Enum.GetValues<TEnum>().ToFrozenDictionary(ToNumber, Self);


    private static TEnum  Self( TEnum     x ) => x;
    private static string ToString( TEnum x ) => x.ToString();
    private static long   ToNumber( TEnum e ) => e.AsLong();


    public override TEnum Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
               {
                   JsonTokenType.String => ReadFromString(ref reader),
                   JsonTokenType.Number => ReadFromNumber(ref reader),
                   _                    => throw new JsonException($"Unexpected token parsing {typeof(TEnum).Name}: {reader.TokenType}")
               };
    }

    private static TEnum ReadFromString( ref Utf8JsonReader reader )
    {
        string? str = reader.GetString();
        if ( str is null ) { throw new JsonException($"Null string for {typeof(TEnum).Name}"); }

        if ( _stringToEnum.TryGetValue(str, out TEnum enumValue) || ( long.TryParse(str, out long n) && _intToEnum.TryGetValue(n, out enumValue) ) ) { return enumValue; }

        throw new JsonException($"Invalid value '{str}' for {typeof(TEnum).Name}");
    }

    private static TEnum ReadFromNumber( ref Utf8JsonReader reader )
    {
        if ( reader.TryGetInt32(out int intVal) && Enum.IsDefined(typeof(TEnum), intVal) ) { return (TEnum)Enum.ToObject(typeof(TEnum), intVal); }

        throw new JsonException($"Invalid numeric value for {typeof(TEnum).Name}");
    }

    public override void Write( Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options )
    {
        writer.WriteStringValue(_enumToString.TryGetValue(value, out string? str)
                                    ? str
                                    : value.ToString()); // fallback (shouldn’t normally happen)
    }
}
