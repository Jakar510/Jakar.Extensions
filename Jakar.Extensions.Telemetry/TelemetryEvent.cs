// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  18:06

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;



namespace Jakar.Extensions.Telemetry;


public readonly record struct TelemetryEvent( string ID, DateTimeOffset Timestamp, params KeyValuePair<string, object?>[]? Tags )
{
    public TelemetryEvent( string id ) : this( id, DateTimeOffset.UtcNow, null ) { }
    public TelemetryEvent( string id, params KeyValuePair<string, object?>[] tags ) : this( id, DateTimeOffset.UtcNow, tags ) { }
}



public readonly record struct TelemetryTag( string Key, string? Value )
{
    public static implicit operator KeyValuePair<string, string?>( TelemetryTag tag ) => new(tag.Key, tag.Value);
}



public readonly record struct TelemetryBaggage( string Key, JToken? Value )
{
    public static implicit operator KeyValuePair<string, JToken?>( TelemetryBaggage tag ) => new(tag.Key, tag.Value);
}
