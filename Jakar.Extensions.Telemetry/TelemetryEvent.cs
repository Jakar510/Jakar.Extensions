// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  18:06

namespace Jakar.Extensions.Telemetry;


[Serializable, StructLayout(LayoutKind.Auto)]
public readonly record struct TelemetryEvent( string EventID, in DateTimeOffset Timestamp, in Pairs Tags ) : IComparable<TelemetryEvent>, IComparable
{
    public readonly Pairs          Tags      = Tags;
    public readonly string         EventID   = EventID;
    public readonly DateTimeOffset Timestamp = Timestamp;


    public TelemetryEvent( string eventID ) : this(eventID, DateTimeOffset.UtcNow, in Pairs.Empty) { }
    public TelemetryEvent( string eventID, in Pairs tags ) : this(eventID, DateTimeOffset.UtcNow, in tags) { }


    public int CompareTo( TelemetryEvent other )
    {
        int eventIDComparison = string.Compare(EventID, other.EventID, StringComparison.Ordinal);
        if ( eventIDComparison != 0 ) { return eventIDComparison; }

        return Timestamp.CompareTo(other.Timestamp);
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is TelemetryEvent other
                   ? CompareTo(other)
                   : throw new ArgumentException($"Object must be of type {nameof(TelemetryEvent)}");
    }
    public static bool operator <( TelemetryEvent  left, TelemetryEvent right ) => left.CompareTo(right) < 0;
    public static bool operator >( TelemetryEvent  left, TelemetryEvent right ) => left.CompareTo(right) > 0;
    public static bool operator <=( TelemetryEvent left, TelemetryEvent right ) => left.CompareTo(right) <= 0;
    public static bool operator >=( TelemetryEvent left, TelemetryEvent right ) => left.CompareTo(right) >= 0;
}
