namespace Jakar.Extensions;


public sealed class VersionConverter() : JsonConverter<Version>()
{
    public static readonly VersionConverter Instance = new();
    public override Version? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        switch ( reader.TokenType )
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
            {
                string? s = reader.GetString();
                if ( string.IsNullOrWhiteSpace(s) ) { return null; }

                if ( Version.TryParse(s, out Version? v) ) { return v; }

                throw new JsonException($"Invalid version string: '{s}'");
            }

            default:
                throw new JsonException($"Unexpected token parsing Version. TokenType={reader.TokenType}");
        }
    }

    public override void Write( Utf8JsonWriter writer, Version value, JsonSerializerOptions options ) { writer.WriteStringValue(value.ToString()); }
}