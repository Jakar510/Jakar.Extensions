// Jakar.Extensions :: Jakar.Extensions
// 09/21/2025  16:31

namespace Jakar.Extensions;


public abstract class SerializeAsStringJsonConverter<TClass, T> : JsonConverter<T>
    where T : IParsable<T>
    where TClass : SerializeAsStringJsonConverter<TClass, T>, new()
{
    public static readonly TClass Instance = new();


    public override T? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        string? value = reader.GetString();

        return T.TryParse(value, CultureInfo.InvariantCulture, out T? result)
                   ? result
                   : default;
    }
    public override void Write( Utf8JsonWriter writer, T value, JsonSerializerOptions options ) => writer.WriteStringValue(value.ToString());
}
