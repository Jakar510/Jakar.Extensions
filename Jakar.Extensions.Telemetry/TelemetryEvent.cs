// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/24/2024  18:06

namespace Jakar.Extensions.Telemetry;


[Serializable, StructLayout( LayoutKind.Auto )]
public readonly struct TelemetryEvent( string eventID, in DateTimeOffset timestamp, in TagsList? tags ) : IEquatable<TelemetryEvent>, IComparable<TelemetryEvent>, IComparable
{
    public readonly TagsList?      Tags      = tags;
    public readonly string         EventID   = eventID;
    public readonly DateTimeOffset Timestamp = timestamp;


    public static ValueSorter<TelemetryEvent> Sorter => ValueSorter<TelemetryEvent>.Default;
    public TelemetryEvent( string eventID ) : this( eventID, DateTimeOffset.UtcNow, null ) { }
    public TelemetryEvent( string eventID, TagsList? tags ) : this( eventID, DateTimeOffset.UtcNow, tags ) { }


    public override bool Equals( object?        obj )   => obj is TelemetryEvent other && Equals( other );
    public          bool Equals( TelemetryEvent other ) => EventID == other.EventID    && Timestamp.Equals( other.Timestamp );
    public int CompareTo( TelemetryEvent other )
    {
        int eventIDComparison = string.Compare( EventID, other.EventID, StringComparison.Ordinal );
        if ( eventIDComparison != 0 ) { return eventIDComparison; }

        return Timestamp.CompareTo( other.Timestamp );
    }
    public int CompareTo( object? obj )
    {
        if ( obj is null ) { return 1; }

        return obj is TelemetryEvent other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(TelemetryEvent)}" );
    }
    public override int GetHashCode()                                             => HashCode.Combine( EventID, Timestamp );
    public static   bool operator ==( TelemetryEvent left, TelemetryEvent right ) => Sorter.Equals( left, right );
    public static   bool operator !=( TelemetryEvent left, TelemetryEvent right ) => Sorter.DoesNotEqual( left, right );
    public static   bool operator <( TelemetryEvent  left, TelemetryEvent right ) => Sorter.Compare( left, right ) < 0;
    public static   bool operator >( TelemetryEvent  left, TelemetryEvent right ) => Sorter.Compare( left, right ) > 0;
    public static   bool operator <=( TelemetryEvent left, TelemetryEvent right ) => Sorter.Compare( left, right ) <= 0;
    public static   bool operator >=( TelemetryEvent left, TelemetryEvent right ) => Sorter.Compare( left, right ) >= 0;
}
