// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/10/2025  11:01

namespace Jakar.Extensions.Telemetry;


public readonly record struct TelemetryTag( string Key, string? Value )
{
    public static implicit operator KeyValuePair<string, string?>( TelemetryTag tag ) => new(tag.Key, tag.Value);
    public static implicit operator (string Key, string? Value)( TelemetryTag   tag ) => new(tag.Key, tag.Value);
    public static implicit operator TelemetryTag( KeyValuePair<string, string?> tag ) => new(tag.Key, tag.Value);
    public static implicit operator TelemetryTag( (string Key, string? Value)   tag ) => new(tag.Key, tag.Value);



    public sealed class Collection() : LinkedList<TelemetryTag>()
    {
        public static Collection Create() => new();
        public Collection( IEnumerable<TelemetryTag> values ) : this()
        {
            foreach ( var tag in values ) { AddLast( tag ); }
        }
    }
}
