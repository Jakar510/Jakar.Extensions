namespace Jakar.Extensions;


public sealed class NullableShortConverter : JsonConverter<short?>
{
    public override void WriteJson( JsonWriter writer, short? value, JsonSerializer serializer ) { writer.WriteValue( value ); }
    public override short? ReadJson( JsonReader reader, Type objectType, short? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        return reader.Value switch
               {
                   null    => existingValue,
                   short n => n,
                   string s => short.TryParse( s, out short n )
                                   ? n
                                   : null,
                   _ => throw new OutOfRangeException( nameof(reader.Value), reader.Value )
               };
    }
}