using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Telemetry;


public interface ITelemetryActivityContext
{
    string OperationName { get; }
    string SpanID        { get; }
    string TraceID       { get; }
}



public readonly struct TelemetryActivityContext : ITelemetryActivityContext
{
    public readonly TelemetryActivitySpanID  spanID;
    public readonly TelemetryActivityTraceID traceID;


    public required string                   OperationName { get;           init; }
    public required TelemetryActivitySpanID  SpanID        { get => spanID; init => spanID = value; }
    string ITelemetryActivityContext.        SpanID        => spanID.ToString();
    string ITelemetryActivityContext.        TraceID       => traceID.ToString();
    public required TelemetryActivityTraceID TraceID       { get => traceID; init => traceID = value; }


    [SetsRequiredMembers, MethodImpl( MethodImplOptions.AggressiveInlining )]
    public TelemetryActivityContext( string operationName, TelemetryActivityTraceID traceID, TelemetryActivitySpanID spanID ) : this()
    {
        SpanID        = spanID;
        TraceID       = traceID;
        OperationName = operationName;
    }
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public        TelemetryActivityContext CreateChild( string operationName )                                                                   => new(operationName, traceID, TelemetryActivitySpanID.CreateRandom());
    [MethodImpl(       MethodImplOptions.AggressiveInlining )] public static TelemetryActivityContext Create( string      operationName )                                                                   => Create( operationName, TelemetryActivityTraceID.CreateRandom(), TelemetryActivitySpanID.CreateRandom() );
    [MethodImpl(       MethodImplOptions.AggressiveInlining )] public static TelemetryActivityContext Create( string      operationName, TelemetryActivityTraceID traceID, TelemetryActivitySpanID spanID ) => new(operationName, traceID, spanID);
}
