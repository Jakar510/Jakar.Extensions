namespace Jakar.Extensions;


public sealed class NullableShortConverter() : JsonConverter<short?>()
{
    public override short? ReadJson( JsonReader reader, Type objectType, short? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        switch ( reader.TokenType )
        {
            // Handle null token
            case JsonToken.Null:
                return null;

            // Handle number token
            case JsonToken.Float:
            case JsonToken.Integer:
                return Convert.ToInt16(reader.Value, CultureInfo.InvariantCulture);

            // Handle string token
            case JsonToken.String:
            {
                string s = ( (string?)reader.Value )?.Trim() ?? EMPTY;

                if ( s.Length == 0 ) { return null; }

                if ( short.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out short result) ) { return result; }

                throw new JsonSerializationException($"Cannot convert string '{s}' to short.");
            }

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing nullable short.");
        }
    }


    public override void WriteJson( JsonWriter writer, short? value, JsonSerializer serializer )
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteValue(value.Value.ToString(CultureInfo.InvariantCulture)); }
    }
}
