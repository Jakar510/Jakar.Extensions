#nullable enable
namespace Jakar.Extensions.Converters;


public class VersionConverter : JsonConverter<Version>
{
    public override void WriteJson( JsonWriter writer, Version? value, JsonSerializer serializer ) => writer.WriteValue(value?.ToString());

    public override Version? ReadJson( JsonReader reader, Type objectType, Version? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        var s = reader.Value as string;
        if ( string.IsNullOrWhiteSpace(s) ) { return existingValue; }

        return Version.Parse(s);
    }
}
