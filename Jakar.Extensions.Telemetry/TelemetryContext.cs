namespace Jakar.Extensions.Telemetry;


public readonly struct TelemetryContext( string name, AppContext appContext, ActivityTraceID traceID, ActivitySpanID spanID )
{
    public string          Name       { get; } = name;
    public AppContext      AppContext { get; } = appContext;
    public ActivityTraceID TraceID    { get; } = traceID;
    public ActivitySpanID  SpanID     { get; } = spanID;


    public TelemetryContext CreateChild( string name ) => new(name, AppContext, TraceID, ActivitySpanID.CreateRandom());


    public static TelemetryContext Create<TApp>( ActivityTraceID? traceID = null, ActivitySpanID? spanID = null )
        where TApp : IAppID => Create( TApp.AppName, AppContext.Create<TApp>(), traceID, spanID );
    public static TelemetryContext Create( string name, AppContext context, ActivityTraceID? traceID = null, ActivitySpanID? spanID = null ) => new(name, context, traceID ?? ActivityTraceID.CreateRandom(), spanID ?? ActivitySpanID.CreateRandom());
}
