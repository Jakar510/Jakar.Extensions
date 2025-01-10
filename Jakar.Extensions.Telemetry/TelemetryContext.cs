namespace Jakar.Extensions.Telemetry;


public sealed class TelemetryContext()
{
    public required AppContext      AppContext { get; init; }
    public required ActivityTraceID TraceID    { get; init; }
    public required ActivitySpanID  SpanID     { get; init; }

    public static TelemetryContext Create<TApp>( ActivityTraceID? traceID = null, ActivitySpanID? spanID = null )
        where TApp : IAppID => Create( AppContext.Create<TApp>(), traceID, spanID );
    public static TelemetryContext Create( AppContext context, ActivityTraceID? traceID = null, ActivitySpanID? spanID = null ) => new()
                                                                                                                                   {
                                                                                                                                       AppContext = context,
                                                                                                                                       TraceID    = traceID ?? ActivityTraceID.CreateRandom(),
                                                                                                                                       SpanID     = spanID  ?? ActivitySpanID.CreateRandom()
                                                                                                                                   };
}
