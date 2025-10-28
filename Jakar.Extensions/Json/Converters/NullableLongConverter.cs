namespace Jakar.Extensions;


public sealed class NullableLongConverter() : JsonConverter<long?>()
{
    public override long? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
               {
                   JsonTokenType.Null => null,

                   JsonTokenType.Number when reader.TryGetInt16(out short s) => s,
                   JsonTokenType.Number when reader.TryGetInt32(out int i)   => i,
                   JsonTokenType.Number when reader.TryGetInt64(out long l)  => l,

                   JsonTokenType.String => long.TryParse(reader.GetString(), out long n)
                                               ? n
                                               : null,

                   _ => throw new JsonException($"Unexpected token parsing float?: {reader.TokenType}")
               };
    }

    public override void Write( Utf8JsonWriter writer, long? value, JsonSerializerOptions options )
    {
        if ( value.HasValue ) { writer.WriteNumberValue(value.Value); }
        else { writer.WriteNullValue(); }
    }
}
