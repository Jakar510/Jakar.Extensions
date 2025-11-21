// Jakar.Extensions :: Jakar.Extensions
// 09/23/2025  18:44

namespace Jakar.Extensions;


public class EncodingConverter : JsonConverter<Encoding>
{
    public static readonly EncodingConverter Instance = new();

    public override Encoding? ReadJson( JsonReader reader, Type objectType, Encoding? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        if ( reader.TokenType is JsonToken.Null ) { return null; }

        if ( reader.TokenType is not JsonToken.String ) { throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing Encoding. Expected a string."); }

        string? name = (string?)reader.Value;
                if ( string.IsNullOrWhiteSpace(name) ) { return null; }

        try { return Encoding.GetEncoding(name); }
        catch ( Exception ex ) { throw new JsonSerializationException($"Unknown encoding '{name}'.", ex); }
    }

    public override void WriteJson( JsonWriter writer, Encoding? value, JsonSerializer serializer )
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        // Use WebName: canonical, stable, lowercase
        writer.WriteValue(value.WebName);
    }
}
