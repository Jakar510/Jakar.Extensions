// unset


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public class IntToEnumConverter<TEnum> : JsonConverter where TEnum : Enum
{
    public override bool CanConvert( Type objectType ) =>
        objectType == typeof(byte) || objectType == typeof(sbyte) || objectType == typeof(short) || objectType == typeof(ushort) || objectType == typeof(int) || objectType == typeof(uint) || objectType == typeof(long) ||
        objectType == typeof(ulong) || objectType == typeof(TEnum);

    public override object ReadJson( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer ) =>
        existingValue switch
        {
            TEnum screens => screens,
            int value     => Enum.ToObject( typeof(TEnum), value ),
            _             => throw new JsonReaderException( nameof(existingValue) )
        };
    public override void WriteJson( JsonWriter writer, object? value, JsonSerializer serializer )
    {
        string result = value switch
                        {
                            TEnum n => Enum.ToObject( typeof(TEnum), n )
                                           .ToString(),
                            int number => number.ToString(),
                            _          => throw new JsonReaderException( nameof(value) )
                        };

        writer.WriteValue( result );
    }
}
