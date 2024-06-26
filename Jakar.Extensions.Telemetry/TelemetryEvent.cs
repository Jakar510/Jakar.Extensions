// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  18:06

namespace Jakar.Extensions.Telemetry;


public readonly record struct TelemetryEvent( string ID, DateTimeOffset Timestamp, params TelemetryTag[]? Tags )
{
    public TelemetryEvent( string id ) : this( id, DateTimeOffset.UtcNow, null ) { }
    public TelemetryEvent( string id, params TelemetryTag[] tags ) : this( id, DateTimeOffset.UtcNow, tags ) { }
}



public readonly record struct TelemetryTag( string Key, string? Value )
{
    public static implicit operator KeyValuePair<string, string?>( TelemetryTag tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, string? Value)( TelemetryTag   tag ) => new(tag.Key, tag.Value);
    public static implicit operator TelemetryTag( KeyValuePair<string, string?> tag ) => new(tag.Key, tag.Value);
    public static implicit operator TelemetryTag( (string Key, string? Value)   tag ) => new(tag.Key, tag.Value);
}



public readonly record struct TelemetryBaggage( string Key, JToken? Value )
{
    public static implicit operator KeyValuePair<string, JToken?>( TelemetryBaggage tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, JToken? Value)( TelemetryBaggage   tag ) => new(tag.Key, tag.Value);
    public static implicit operator TelemetryBaggage( KeyValuePair<string, JToken?> tag ) => new(tag.Key, tag.Value);
    public static implicit operator TelemetryBaggage( (string Key, JToken? Value)   tag ) => new(tag.Key, tag.Value);
}
