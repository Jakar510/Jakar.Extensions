// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  18:06

namespace Jakar.Extensions.Telemetry;


public sealed record TelemetryEvent( string EventID, DateTimeOffset Timestamp, params TelemetryTag[]? Tags )
{
    public TelemetryEvent( string eventID) : this( eventID, DateTimeOffset.UtcNow, null ) { }
    public TelemetryEvent( string eventID, params TelemetryTag[]? tags ) : this( eventID, DateTimeOffset.UtcNow, tags ) { }



    public sealed class Collection() : LinkedList<TelemetryEvent>()
    {
        public static Collection Create() => new();
        public Collection( IEnumerable<TelemetryEvent> values ) : this()
        {
            foreach ( TelemetryEvent tag in values ) { AddLast( tag ); }
        }
    }
}
