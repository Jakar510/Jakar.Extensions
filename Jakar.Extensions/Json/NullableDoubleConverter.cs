namespace Jakar.Extensions;


public sealed class NullableDoubleConverter() : JsonConverter<double?>()
{
    public override double? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
               {
                   JsonTokenType.Null => null,

                   JsonTokenType.Number when reader.TryGetInt16(out short s)   => s,
                   JsonTokenType.Number when reader.TryGetInt32(out int i)     => i,
                   JsonTokenType.Number when reader.TryGetInt64(out long l)    => l,
                   JsonTokenType.Number when reader.TryGetSingle(out float f)  => f,
                   JsonTokenType.Number when reader.TryGetDouble(out double d) => d,

                   JsonTokenType.String => double.TryParse(reader.GetString(), out double n)
                                               ? n
                                               : null,

                   _ => throw new JsonException($"Unexpected token parsing double?: {reader.TokenType}")
               };
    }

    public override void Write( Utf8JsonWriter writer, double? value, JsonSerializerOptions options )
    {
        if ( value.HasValue ) { writer.WriteNumberValue(value.Value); }
        else { writer.WriteNullValue(); }
    }
}
