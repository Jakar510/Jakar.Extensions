// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 02/14/2025  14:02

namespace Jakar.Extensions.Telemetry;


public class InstrumentJsonConverter : JsonConverter<IMeterInstrument>
{
    public static readonly Dictionary<string, Type> InstrumentTypes = new()
                                                                      {
                                                                          [nameof(CounterLong)]   = typeof(CounterLong),
                                                                          [nameof(CounterULong)]  = typeof(CounterULong),
                                                                          [nameof(CounterDouble)] = typeof(CounterDouble),
                                                                      };
    public static void Register<TInstrument>()
        where TInstrument : IMeterInstrument => Register( typeof(TInstrument) );
    public static void Register( Type type ) => InstrumentTypes.GetOrAdd<string, Type>( type.Name, type );


    public override void WriteJson( JsonWriter writer, IMeterInstrument? value, JsonSerializer serializer ) => serializer.Serialize( writer, value );
    public override IMeterInstrument? ReadJson( JsonReader reader, Type objectType, IMeterInstrument? existingValue, bool hasExistingValue, JsonSerializer serializer )
    {
        JToken  token = JToken.Load( reader );
        string? type  = token[nameof(IMeterInstrument.Type)]?.ToObject<string>();
        if ( string.IsNullOrWhiteSpace( type ) ) { return null; }

        if ( InstrumentTypes.TryGetValue( type, out Type? instrumentType ) is false ) { return null; }

        return (IMeterInstrument?)serializer.Deserialize( reader, instrumentType );
    }
}
