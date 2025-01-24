namespace Jakar.Extensions;


public sealed class NullableDoubleConverter() : JsonConverter<double?>()
{
    public override void WriteJson( JsonWriter writer, double? value, JsonSerializer serializer ) => writer.WriteValue( value );
    public override double? ReadJson( JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer ) =>
        reader.Value switch
        {
            null     => existingValue,
            short n  => n,
            int n    => n,
            long n   => n,
            float n  => n,
            double n => n,
            string s => double.TryParse( s, out double n )
                            ? n
                            : null,
            _ => throw new OutOfRangeException( reader.Value )
        };
}
