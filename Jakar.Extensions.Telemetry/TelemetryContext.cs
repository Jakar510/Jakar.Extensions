



namespace Jakar.Extensions.Telemetry;


public readonly record struct TelemetryContext( string Name, ActivityID TraceID, ActivityID SpanID )
{
    public static TelemetryContext Create( string name ) => new(name, ActivityID.Create(), ActivityID.Create());
}
