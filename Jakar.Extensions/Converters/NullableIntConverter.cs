namespace Jakar.Extensions;


public sealed class NullableIntConverter() : JsonConverter<int?>()
{ 
    public override int? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
               {
                   JsonTokenType.Null => null,

                   JsonTokenType.Number when reader.TryGetInt16(out short s)  => s,
                   JsonTokenType.Number when reader.TryGetInt32(out int i)    => i, 

                   JsonTokenType.String => int.TryParse(reader.GetString(), out int n)
                                               ? n
                                               : null,

                   _ => throw new JsonException($"Unexpected token parsing float?: {reader.TokenType}")
               };
    }

    public override void Write( Utf8JsonWriter writer, int? value, JsonSerializerOptions options )
    {
        if ( value.HasValue ) { writer.WriteNumberValue(value.Value); }
        else { writer.WriteNullValue(); }
    }
}
