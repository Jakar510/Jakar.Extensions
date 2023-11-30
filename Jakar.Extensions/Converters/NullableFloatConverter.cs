namespace Jakar.Extensions;


public sealed class NullableFloatConverter : Newtonsoft.Json.JsonConverter<float?>
{
    public override void WriteJson( JsonWriter writer, float? value, JsonNetSerializer serializer ) => writer.WriteValue( value );
    public override float? ReadJson( JsonReader reader, Type objectType, float? existingValue, bool hasExistingValue, JsonNetSerializer serializer ) =>
        reader.Value switch
        {
            null    => existingValue,
            short n => n,
            int n   => n,
            long n  => n,
            float n => n,
            string s => float.TryParse( s, out float n )
                            ? n
                            : null,
            _ => throw new OutOfRangeException( nameof(reader.Value), reader.Value )
        };
}
