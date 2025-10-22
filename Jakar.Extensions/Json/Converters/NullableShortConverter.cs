namespace Jakar.Extensions;


public sealed class NullableShortConverter() : JsonConverter<short?>()
{
    public override short? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
               {
                   JsonTokenType.Null => null,

                   JsonTokenType.Number when reader.TryGetInt16(out short s) => s,

                   JsonTokenType.String => short.TryParse(reader.GetString(), out short n)
                                               ? n
                                               : null,

                   _ => throw new JsonException($"Unexpected token parsing float?: {reader.TokenType}")
               };
    }

    public override void Write( Utf8JsonWriter writer, short? value, JsonSerializerOptions options )
    {
        if ( value.HasValue ) { writer.WriteNumberValue(value.Value); }
        else { writer.WriteNullValue(); }
    }
}
