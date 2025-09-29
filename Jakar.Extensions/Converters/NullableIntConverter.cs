namespace Jakar.Extensions;


public sealed class NullableIntConverter() : JsonConverter<int?>()
{
    public override void WriteJson( JsonWriter writer, int? value, JsonSerializer serializer ) => writer.WriteValue( value );
    public override int? ReadJson( JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonSerializer serializer ) =>
        reader.Value switch
        {
            null    => existingValue,
            short n => n,
            int n   => n,
            long n  => (int)n,
            string s => int.TryParse( s, out int n )
                            ? n
                            : null,
            _ => throw new OutOfRangeException( reader.Value )
        };
}
