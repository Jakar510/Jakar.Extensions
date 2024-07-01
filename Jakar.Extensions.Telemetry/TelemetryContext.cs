namespace Jakar.Extensions.Telemetry;




public readonly record struct AppContext( string AppName, Guid AppID, AppVersion AppVersion )
{
    public static AppContext Create<TApp>()
        where TApp : IAppID => Create( TApp.AppName, TApp.AppID, TApp.AppVersion );
    public static AppContext Create( string appName, Guid appID, AppVersion appVersion ) => new(appName, appID, appVersion);
}



public readonly record struct TelemetryContext( AppContext AppContext, ActivityTraceId? TraceID = null, ActivitySpanId? SpanID = null )
{
    public static TelemetryContext Create<TApp>( ActivityTraceId? traceID = null, ActivitySpanId? spanID = null )
        where TApp : IAppID => Create( AppContext.Create<TApp>(), traceID, spanID );
    public static TelemetryContext Create( AppContext context, ActivityTraceId? traceID = null, ActivitySpanId? spanID = null ) => new(context, traceID, spanID);
}
