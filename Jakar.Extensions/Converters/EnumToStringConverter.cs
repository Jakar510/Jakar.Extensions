// Jakar.Extensions :: Jakar.Extensions
// 05/01/2023  3:58 PM

namespace Jakar.Extensions;


/*
public sealed class EnumToStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
#if NET6_0_OR_GREATER
    private static readonly Dictionary<TEnum, string> _dictionary = Enum.GetValues<TEnum>()
                                                                        .ToDictionary( x => x, x => x.ToString() );
#else
    private static readonly Dictionary<TEnum, string> _dictionary = Enum.GetValues( typeof(TEnum) )
                                                                        .Cast<TEnum>()
                                                                        .ToDictionary( x => x, x => x.ToString() );
#endif

    public override bool CanRead  => true;
    public override bool CanWrite => true;


    public override void WriteJson( JsonWriter writer, TEnum value, JsonSerializer serializer ) => writer.WriteRaw( _dictionary[value] );


    public override TEnum ReadJson( JsonReader reader, Type objectType, TEnum existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        string? value = reader.ReadAsString();

    #if NET6_0_OR_GREATER
        return Enum.TryParse( value, true, out TEnum result )
                   ? result
                   : existingValue;

    #else
        return Enum.TryParse( typeof(TEnum), value, true, out object result )
                   ? (TEnum)result
                   : existingValue;

    #endif
    }
}
*/
