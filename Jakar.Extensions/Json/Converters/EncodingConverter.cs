// Jakar.Extensions :: Jakar.Extensions
// 09/23/2025  18:44

namespace Jakar.Extensions;


/// <summary> Writes an <see cref="Encoding"/> as its <see cref="Encoding.WebName"/> (canonical, stable, lowercase) and reads it back with <see cref="Encoding.GetEncoding(string)"/>. </summary>
public sealed class EncodingConverter : JsonConverter<Encoding>
{
    public static readonly EncodingConverter Instance = new();


    public override Encoding? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType is JsonTokenType.Null ) { return null; }

        if ( reader.TokenType is not JsonTokenType.String ) { throw new JsonException($"Unexpected token {reader.TokenType} when parsing Encoding. Expected a string."); }

        string? name = reader.GetString();
        if ( string.IsNullOrWhiteSpace(name) ) { return null; }

        try { return Encoding.GetEncoding(name); }
        catch ( ArgumentException e ) { throw new JsonException($"Unknown encoding '{name}'.", e); }
    }


    public override void Write( Utf8JsonWriter writer, Encoding? value, JsonSerializerOptions options )
    {
        if ( value is null )
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.WebName);
    }
}
