namespace Jakar.Extensions.Telemetry;


public interface IAppID : IAppName
{
    public static abstract Guid AppID { get; }
}



public sealed record TelemetryContext( string AppName, Guid AppID, AppVersion AppVersion, ActivityID? TraceID = null, ActivityID? SpanID = null )
{
    public ActivityID? TraceID { get; set; } = TraceID;
    public ActivityID? SpanID  { get; set; } = SpanID;


    public static TelemetryContext Create<TApp>()
        where TApp : IAppID => Create( TApp.AppName, TApp.AppID, TApp.AppVersion );
    public static TelemetryContext Create( string appName, Guid appID, AppVersion appVersion ) => new(appName, appID, appVersion);
}
