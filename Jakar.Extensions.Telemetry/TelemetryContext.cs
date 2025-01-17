using System.Text;



namespace Jakar.Extensions.Telemetry;


public interface ITelemetryContext
{
    string     CategoryName { get; }
    AppContext AppContext   { get; }
    string     TraceID      { get; }
    string     SpanID       { get; }
}



public sealed class TelemetryContext( string categoryName, AppContext appContext, ActivityTraceID traceID, ActivitySpanID spanID ) : ITelemetryContext
{
    public string            CategoryName { get; init; } = categoryName;
    public AppContext        AppContext   { get; init; } = appContext;
    public ActivityTraceID   TraceID      { get; init; } = traceID;
    public ActivitySpanID    SpanID       { get; init; } = spanID;
    string ITelemetryContext.TraceID      => TraceID.ToString();
    string ITelemetryContext.SpanID       => SpanID.ToString();
    public TelemetryContext? Parent       { get; init; }


    public TelemetryContext CreateChild( string name ) => new(name, AppContext, TraceID, ActivitySpanID.CreateRandom()) { Parent = this };

    public string GetSpanID()
    {
        StringBuilder sb = new(1024);
        sb.Append( '|' );
        ActivitySpanID? spanID = SpanID;

        while ( spanID.HasValue )
        {
            sb.Append( spanID.Value.ToString() );
            spanID = Parent?.SpanID;
            if ( spanID.HasValue ) { sb.Append( '|' ); }
        }

        return sb.ToString();
    }

    public static TelemetryContext Create<TApp>( ActivityTraceID? traceID = null, ActivitySpanID? spanID = null )
        where TApp : IAppID => Create( TApp.AppName, AppContext.Create<TApp>(), traceID, spanID );
    public static TelemetryContext Create( string name, AppContext context, ActivityTraceID? traceID = null, ActivitySpanID? spanID = null ) => new(name, context, traceID ?? ActivityTraceID.CreateRandom(), spanID ?? ActivitySpanID.CreateRandom());
}
