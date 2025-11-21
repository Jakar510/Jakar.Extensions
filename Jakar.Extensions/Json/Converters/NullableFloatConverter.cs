namespace Jakar.Extensions;


public sealed class NullableFloatConverter() : JsonConverter<float?>()
{
    public override float? ReadJson( JsonReader reader, Type objectType, float? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        switch ( reader.TokenType )
        {
            // Handle null token
            case JsonToken.Null:
                return null;

            // Handle number token
            case JsonToken.Float:
            case JsonToken.Integer:
                return Convert.ToSingle(reader.Value, CultureInfo.InvariantCulture);

            // Handle string token
            case JsonToken.String:
            {
                string s = ( (string?)reader.Value )?.Trim() ?? EMPTY;

                if ( s.Length == 0 ) { return null; }

                if ( float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out float result) ) { return result; }

                throw new JsonSerializationException($"Cannot convert string '{s}' to float.");
            }

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing nullable float.");
        }
    }


    public override void WriteJson( JsonWriter writer, float? value, JsonSerializer serializer )
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteValue(value.Value.ToString(CultureInfo.InvariantCulture)); }
    }
}
