namespace Jakar.Extensions;


public sealed class NullableIntConverter() : JsonConverter<int?>()
{
    public override int? ReadJson( JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        switch ( reader.TokenType )
        {
            // Handle null token
            case JsonToken.Null:
                return null;

            // Handle number token
            case JsonToken.Float:
            case JsonToken.Integer:
                return Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);

            // Handle string token
            case JsonToken.String:
            {
                string s = ( (string?)reader.Value )?.Trim() ?? EMPTY;

                if ( s.Length == 0 ) { return null; }

                if ( int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int result) ) { return result; }

                throw new JsonSerializationException($"Cannot convert string '{s}' to int.");
            }

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing nullable int.");
        }
    }


    public override void WriteJson( JsonWriter writer, int? value, JsonSerializer serializer )
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteValue(value.Value.ToString(CultureInfo.InvariantCulture)); }
    }
}
