namespace Jakar.Extensions;


public sealed class VersionConverter() : JsonConverter<Version>()
{
    public static readonly VersionConverter Instance = new();


    public override Version? ReadJson( JsonReader reader, Type objectType, Version? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        if ( reader.TokenType is JsonToken.Null ) { return null; }

        if ( reader.TokenType is not JsonToken.String ) { throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing Version. Expected string."); }

        string? s = ( (string?)reader.Value )?.Trim();

        try
        {
            return string.IsNullOrWhiteSpace(s)
                       ? null
                       : new Version(s);
        }
        catch ( Exception ex ) { throw new JsonSerializationException($"Invalid Version string '{s}'.", ex); }
    }

    public override void WriteJson( JsonWriter writer, Version? value, JsonSerializer serializer )
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(value.ToString());
    }
}
