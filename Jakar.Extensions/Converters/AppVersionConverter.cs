#nullable enable
namespace Jakar.Extensions;


public class AppVersionConverter : JsonConverter<AppVersion>
{
    public override bool CanRead  => true;
    public override bool CanWrite => true;


    public override AppVersion ReadJson( JsonReader reader, Type objectType, AppVersion existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        if (reader.Value is string item)
        {
            return AppVersion.Parse( item );

            // if ( AppVersion.TryParse(item, out AppVersion? version) ) { return version; }
        }

        return existingValue;
    }


    public override void WriteJson( JsonWriter writer, AppVersion value, JsonSerializer serializer )
    {
        JToken item = JToken.FromObject( value.ToString() );
        item.WriteTo( writer );
    }
}



public class AppVersionNullableConverter : JsonConverter<AppVersion?>
{
    public override bool CanRead  => true;
    public override bool CanWrite => true;


    public override AppVersion? ReadJson( JsonReader reader, Type objectType, AppVersion? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        if (reader.Value is string item)
        {
            return AppVersion.Parse( item );

            // if ( AppVersion.TryParse(item, out AppVersion? version) ) { return version; }
        }

        return existingValue;
    }


    public override void WriteJson( JsonWriter writer, AppVersion? version, JsonSerializer serializer )
    {
        if (version is null) { return; }

        JToken item = JToken.FromObject( version.Value.ToString() );
        item.WriteTo( writer );
    }
}
