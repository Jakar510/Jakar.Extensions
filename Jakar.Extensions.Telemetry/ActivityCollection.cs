// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2024  10:06

namespace Jakar.Extensions.Telemetry;


public sealed class ActivityCollection() : ConcurrentDictionary<string, Activity>( Environment.ProcessorCount, Buffers.DEFAULT_CAPACITY, StringComparer.Ordinal ), IDisposable
{
    public required AppContext AppContext { get; init; }


    public ActivityCollection( IEnumerable<KeyValuePair<string, Activity>> values ) : this()
    {
        foreach ( (string? key, Activity? value) in values ) { GetOrAdd( key, value ); }
    }
    public ActivityCollection( IDictionary<string, Activity> values ) : this()
    {
        foreach ( (string? key, Activity? value) in values ) { GetOrAdd( key, value ); }
    }


    public Activity CreateActivity( string operationName, ActivityTraceID? traceID = null, ActivitySpanID? spanID = null, ActivityKind kind = ActivityKind.Internal ) => this[operationName] = Activity.Create( operationName, TelemetryContext.Create( operationName, AppContext, traceID, spanID ), kind );
    public Activity StartActivity( string  operationName, ActivityTraceID? traceID = null, ActivitySpanID? spanID = null, ActivityKind kind = ActivityKind.Internal ) => CreateActivity( operationName, traceID, spanID, kind ).Start();


    public static ActivityCollection Create<TApp>()
        where TApp : IAppID => Create( AppContext.Create<TApp>() );
    public static ActivityCollection Create( AppContext context ) => new() { AppContext = context };
    public void Dispose()
    {
        foreach ( Activity activity in Values ) { activity.Dispose(); }

        Clear();
    }
}