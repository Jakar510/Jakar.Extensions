namespace Jakar.Extensions;


public class VersionConverter : Newtonsoft.Json.JsonConverter<Version>
{
    public override Version? ReadJson( JsonReader reader, Type objectType, Version? existingValue, bool hasExistingValue, JsonNetSerializer serializer )
    {
        string? s = reader.Value as string;
        if ( string.IsNullOrWhiteSpace( s ) ) { return existingValue; }

        return Version.Parse( s );
    }
    public override void WriteJson( JsonWriter writer, Version? value, JsonNetSerializer serializer ) => writer.WriteValue( value?.ToString() );
}
