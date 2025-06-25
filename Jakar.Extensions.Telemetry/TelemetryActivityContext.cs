namespace Jakar.Extensions.Telemetry;


public interface ITelemetryActivityContext
{
    string OperationName { get; }
    string SpanID        { get; }
    string TraceID       { get; }
}



[Serializable, StructLayout( LayoutKind.Auto )]
[method: MethodImpl( MethodImplOptions.AggressiveInlining )]
public readonly record struct TelemetryActivityContext( string OperationName, TelemetryActivityTraceID TraceID, TelemetryActivitySpanID SpanID ) : ITelemetryActivityContext
{
    public static readonly TelemetryActivityContext Empty         = new(nameof(Empty), TelemetryActivityTraceID.Empty, TelemetryActivitySpanID.Empty);
    public readonly        string                   OperationName = OperationName;
    public readonly        TelemetryActivitySpanID  SpanID        = SpanID;
    public readonly        TelemetryActivityTraceID TraceID       = TraceID;
    string ITelemetryActivityContext.               SpanID        => SpanID.ToString();
    string ITelemetryActivityContext.               TraceID       => TraceID.ToString();
    string ITelemetryActivityContext.               OperationName => OperationName;


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public        TelemetryActivityContext CreateChild( string operationName ) => new(operationName, TraceID, TelemetryActivitySpanID.CreateRandom());
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryActivityContext Create( string      operationName ) => Create( operationName, TelemetryActivityTraceID.CreateRandom(), TelemetryActivitySpanID.CreateRandom() );

    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public static TelemetryActivityContext Create( string operationName, TelemetryActivityTraceID traceID, TelemetryActivitySpanID spanID ) => new(operationName, traceID, spanID);
}
