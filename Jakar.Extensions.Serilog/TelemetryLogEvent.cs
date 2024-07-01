// Jakar.Extensions :: TestMauiApp
// 06/28/2024  16:06

namespace Jakar.Extensions.Serilog;


public sealed record TelemetryLogEvent( DateTimeOffset TimeStamp, LogEventLevel Level, ExceptionDetails? Exception, MessageTemplate MessageTemplate, ReadOnlyDictionary<string, JToken?> Properties, [property: CLSCompliant( false )] ActivityTraceId? TraceID, [property: CLSCompliant( false )] ActivitySpanId? SpanID )
{
    public static implicit operator TelemetryLogEvent( LogEvent logEvent ) => Create( logEvent );


    public static IEnumerable<TelemetryLogEvent> Create( IEnumerable<LogEvent> logEvent ) => logEvent.Select( Create );
    public static TelemetryLogEvent Create( LogEvent logEvent )
    {
        Dictionary<string, JToken?> dictionary = new(logEvent.Properties.Count);
        foreach ( KeyValuePair<string, LogEventPropertyValue> pair in logEvent.Properties ) { dictionary[pair.Key] = Convert( pair.Value ); }

        return new TelemetryLogEvent( logEvent.Timestamp.DateTime,
                                      logEvent.Level,
                                      logEvent.Exception is null
                                          ? null
                                          : new ExceptionDetails( logEvent.Exception ),
                                      logEvent.MessageTemplate,
                                      new ReadOnlyDictionary<string, JToken?>( dictionary ),
                                      logEvent.TraceId,
                                      logEvent.SpanId );
    }
    private static JToken? Convert( LogEventPropertyValue value )
    {
        JToken? token = value switch
                        {
                            DictionaryValue x => Convert( x ),
                            ScalarValue x     => Convert( x ),
                            SequenceValue x   => Convert( x ),
                            StructureValue x  => Convert( x ),
                            _                 => JToken.FromObject( value, JsonNet.Serializer )
                        };

        return token;
    }
    private static KeyValuePair<string, JToken?> Convert( KeyValuePair<ScalarValue, LogEventPropertyValue> pair )
    {
        string? key = pair.Key.Value?.ToString();
        ArgumentNullException.ThrowIfNull( key );

        JToken? token = pair.Value switch
                        {
                            DictionaryValue x => Convert( x ),
                            ScalarValue x     => Convert( x ),
                            SequenceValue x   => Convert( x ),
                            StructureValue x  => Convert( x ),
                            _                 => JToken.FromObject( pair.Value, JsonNet.Serializer )
                        };

        return new KeyValuePair<string, JToken?>( key, token );
    }
    private static JToken? Convert( [NotNullIfNotNull( nameof(value) )] StructureValue? value )
    {
        if ( value is null ) { return null; }

        JToken?[] array = GC.AllocateUninitializedArray<JToken?>( value.Properties.Count );

        for ( int i = 0; i < value.Properties.Count; i++ )
        {
            LogEventProperty property = value.Properties[i];
            array[i] = Convert( property.Value );
        }

        return JToken.FromObject( array, JsonNet.Serializer );
    }
    private static JToken? Convert( [NotNullIfNotNull( nameof(value) )] SequenceValue? value )
    {
        if ( value is null ) { return null; }

        JToken?[] array = value.Elements.Select( Convert ).ToArray( value.Elements.Count );
        return JToken.FromObject( array, JsonNet.Serializer );
    }
    private static JToken? Convert( [NotNullIfNotNull( nameof(value) )] ScalarValue? value ) =>
        value?.Value is null
            ? null
            : JToken.FromObject( value.Value );
    private static JToken? Convert( [NotNullIfNotNull( nameof(value) )] DictionaryValue? value )
    {
        if ( value is null ) { return null; }

        Dictionary<string, JToken?> array = value.Elements.Select( Convert ).ToDictionary( StringComparer.Ordinal );
        return JToken.FromObject( array, JsonNet.Serializer );
    }
}
