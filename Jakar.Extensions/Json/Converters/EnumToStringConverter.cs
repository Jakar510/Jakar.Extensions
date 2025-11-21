// Jakar.Extensions :: Jakar.Extensions
// 05/01/2023  3:58 PM

namespace Jakar.Extensions;


public abstract class EnumToStringConverter<TSelf, TEnum>() : JsonConverter<TEnum>()
    where TEnum : unmanaged, Enum
    where TSelf : EnumToStringConverter<TSelf, TEnum>, new()
{
    public static readonly TSelf Instance = new();
    protected static readonly FrozenDictionary<TEnum, string> _enumToString = Enum.GetValues<TEnum>()
                                                                                  .ToFrozenDictionary(Self, ToString);
    protected static readonly FrozenDictionary<string, TEnum> _stringToEnum = Enum.GetValues<TEnum>()
                                                                                  .ToFrozenDictionary(ToString, Self);
    protected static readonly FrozenDictionary<long, TEnum> _intToEnum = Enum.GetValues<TEnum>()
                                                                             .ToFrozenDictionary(ToNumber, Self);


    private static TEnum  Self( TEnum     x ) => x;
    private static string ToString( TEnum x ) => x.ToString();
    private static long   ToNumber( TEnum e ) => e.AsLong();


    public override TEnum ReadJson( JsonReader reader, Type objectType, TEnum existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        if ( reader.TokenType is JsonToken.Null )
        {
            if ( !objectType.IsGenericType || objectType.GetGenericTypeDefinition() != typeof(Nullable<>) ) throw new JsonSerializationException($"Cannot convert null to {typeof(TEnum).FullName}");

            return default!;
        }

        switch ( reader.TokenType )
        {
            case JsonToken.String:
            {
                string s = (string)reader.Value!;
                if ( _stringToEnum.TryGetValue(s, out TEnum parsed) ) return parsed;

                throw new JsonSerializationException($"Unknown {typeof(TEnum).Name} value '{s}'.");
            }

            case JsonToken.Integer:
            {
                long n = Convert.ToInt64(reader.Value);
                if ( _intToEnum.TryGetValue(n, out TEnum parsed) ) return parsed;

                throw new JsonSerializationException($"Unknown numeric value {n} for enum {typeof(TEnum).Name}.");
            }
        }

        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing {typeof(TEnum).Name}.");
    }


    public override void WriteJson( JsonWriter writer, TEnum value, JsonSerializer serializer )
    {
        // Always emit the string name
        if ( _enumToString.TryGetValue(value, out string? s) )
        {
            writer.WriteValue(s);
            return;
        }

        // Should never happen unless enum is corrupted
        writer.WriteValue(value.ToString());
    }
}
