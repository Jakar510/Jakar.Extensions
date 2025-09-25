// Jakar.Extensions :: TestMauiApp
// 06/28/2024  16:06

namespace Jakar.Extensions.Serilog;


public sealed record TelemetryLogEvent( DateTimeOffset TimeStamp, LogEventLevel Level, ExceptionDetails? Exception, MessageTemplate MessageTemplate, ReadOnlyDictionary<string, JsonNode?> Properties, ActivityTraceId? TraceID, ActivitySpanId? SpanID )
{
    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )] public static implicit operator TelemetryLogEvent( LogEvent                                  logEvent ) => Create( logEvent );
    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )] public static                   IEnumerable<TelemetryLogEvent> Create( IEnumerable<LogEvent> logEvent ) => logEvent.Select( Create );


    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
    public static TelemetryLogEvent Create( LogEvent logEvent )
    {
        Dictionary<string, JsonNode?> dictionary = new(logEvent.Properties.Count);
        foreach ( KeyValuePair<string, LogEventPropertyValue> pair in logEvent.Properties ) { dictionary[pair.Key] = Convert( pair.Value ); }

        return new TelemetryLogEvent( logEvent.Timestamp.DateTime,
                                      logEvent.Level,
                                      logEvent.Exception is null
                                          ? null
                                          : new ExceptionDetails( logEvent.Exception ),
                                      logEvent.MessageTemplate,
                                      new ReadOnlyDictionary<string, JsonNode?>( dictionary ),
                                      logEvent.TraceId,
                                      logEvent.SpanId );
    }
    private static JsonNode? Convert( LogEventPropertyValue value )
    {
        JsonNode? token = value switch
                        {
                            DictionaryValue x => Convert( x ),
                            ScalarValue x     => Convert( x ),
                            SequenceValue x   => Convert( x ),
                            StructureValue x  => Convert( x ),
                            _                 => JsonNode.FromObject( value, JsonNet.Serializer )
                        };

        return token;
    }
    private static KeyValuePair<string, JsonNode?> Convert( KeyValuePair<ScalarValue, LogEventPropertyValue> pair )
    {
        string? key = pair.Key.Value?.ToString();
        ArgumentNullException.ThrowIfNull( key );

        JsonNode? token = pair.Value switch
                        {
                            DictionaryValue x => Convert( x ),
                            ScalarValue x     => Convert( x ),
                            SequenceValue x   => Convert( x ),
                            StructureValue x  => Convert( x ),
                            _                 => JsonNode.FromObject( pair.Value, JsonNet.Serializer )
                        };

        return new KeyValuePair<string, JsonNode?>( key, token );
    }
    private static JsonNode? Convert( [NotNullIfNotNull( nameof(value) )] StructureValue? value )
    {
        if ( value is null ) { return null; }

        JsonNode?[] array = GC.AllocateUninitializedArray<JsonNode?>( value.Properties.Count );

        for ( int i = 0; i < value.Properties.Count; i++ )
        {
            LogEventProperty property = value.Properties[i];
            array[i] = Convert( property.Value );
        }

        return JsonNode.FromObject( array, JsonNet.Serializer );
    }
    private static JsonNode? Convert( [NotNullIfNotNull( nameof(value) )] SequenceValue? value )
    {
        if ( value is null ) { return null; }

        JsonNode?[] array = value.Elements.Select( Convert ).ToArray( value.Elements.Count );
        return JsonNode.FromObject( array, JsonNet.Serializer );
    }
    private static JsonNode? Convert( [NotNullIfNotNull( nameof(value) )] ScalarValue? value ) =>
        value?.Value is null
            ? null
            : JsonNode.FromObject( value.Value );
    private static JsonNode? Convert( [NotNullIfNotNull( nameof(value) )] DictionaryValue? value )
    {
        if ( value is null ) { return null; }

        Dictionary<string, JsonNode?> array = value.Elements.Select( Convert ).ToDictionary( StringComparer.Ordinal );
        return JsonNode.FromObject( array, JsonNet.Serializer );
    }
}
