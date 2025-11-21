namespace Jakar.Extensions;


public sealed class NullableLongConverter() : JsonConverter<long?>()
{
    public override long? ReadJson( JsonReader reader, Type objectType, long? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        switch ( reader.TokenType )
        {
            // Handle null token
            case JsonToken.Null:
                return null;

            // Handle number token
            case JsonToken.Float:
            case JsonToken.Integer:
                return Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture);

            // Handle string token
            case JsonToken.String:
            {
                string s = ( (string?)reader.Value )?.Trim() ?? EMPTY;

                if ( s.Length == 0 ) { return null; }

                if ( long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out long result) ) { return result; }

                throw new JsonSerializationException($"Cannot convert string '{s}' to long.");
            }

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing nullable long.");
        }
    }


    public override void WriteJson( JsonWriter writer, long? value, JsonSerializer serializer )
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteValue(value.Value.ToString(CultureInfo.InvariantCulture)); }
    }
}
