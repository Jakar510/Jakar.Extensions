namespace Jakar.Extensions;


public sealed class NullableDoubleConverter() : JsonConverter<double?>()
{
    public override double? ReadJson( JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        switch ( reader.TokenType )
        {
            // Handle null token
            case JsonToken.Null:
                return null;

            // Handle number token
            case JsonToken.Float:
            case JsonToken.Integer:
                return Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);

            // Handle string token
            case JsonToken.String:
            {
                string s = ( (string?)reader.Value )?.Trim() ?? EMPTY;

                if ( s.Length == 0 ) { return null; }

                if ( double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ) { return result; }

                throw new JsonSerializationException($"Cannot convert string '{s}' to double.");
            }

            default:
                throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing nullable double.");
        }
    }


    public override void WriteJson( JsonWriter writer, double? value, JsonSerializer serializer )
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteValue(value.Value.ToString(CultureInfo.InvariantCulture)); }
    }
}
