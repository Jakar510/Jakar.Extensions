// Jakar.Extensions :: Jakar.Extensions
// 05/01/2023  3:58 PM

namespace Jakar.Extensions;


public abstract class EnumToStringConverter<TEnum>() : JsonConverter<TEnum>()
    where TEnum : struct, Enum
{
    protected static readonly FrozenDictionary<TEnum, string> _enumToString = Enum.GetValues<TEnum>().ToFrozenDictionary( Self,     ToString );
    protected static readonly FrozenDictionary<string, TEnum> _stringToEnum = Enum.GetValues<TEnum>().ToFrozenDictionary( ToString, Self );


    public override bool   CanRead             => true;
    public override bool   CanWrite            => true;
    private static  TEnum  Self( TEnum     x ) => x;
    private static  string ToString( TEnum x ) => x.ToString();


    public override void WriteJson( JsonWriter writer, TEnum value, JsonSerializer serializer ) => writer.WriteRaw( _enumToString[value] );
    public override TEnum ReadJson( JsonReader reader, Type objectType, TEnum existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        string? value = reader.ReadAsString();
        if ( string.IsNullOrWhiteSpace( value ) ) { return existingValue; }

        if ( _stringToEnum.TryGetValue( value, out TEnum t ) ) { return t; }

        return Enum.TryParse( value, true, out TEnum result )
                   ? result
                   : existingValue;
    }
}
