// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2024  10:06

namespace Jakar.Extensions.Telemetry;


public sealed class ActivityCollection : ConcurrentDictionary<string, Activity>, IDisposable
{
    public required TelemetryContext Context { get; init; }


    public ActivityCollection() : base( Environment.ProcessorCount, 64, StringComparer.Ordinal ) { }
    public ActivityCollection( IEnumerable<KeyValuePair<string, Activity>> values ) : base( values, StringComparer.Ordinal ) { }
    public ActivityCollection( IDictionary<string, Activity>               values ) : base( values, StringComparer.Ordinal ) { }


    public Activity CreateActivity( string operationName, ActivityKind kind = ActivityKind.Internal ) => this[operationName] = Activity.Create( operationName, Context, kind );
    public Activity StartActivity( string  operationName, ActivityKind kind = ActivityKind.Internal ) => CreateActivity( operationName, kind ).Start();


    public static ActivityCollection Create<TApp>()
        where TApp : IAppID => Create( TelemetryContext.Create<TApp>() );
    public static ActivityCollection Create( TelemetryContext context ) => new() { Context = context };
    public void Dispose()
    {
        foreach ( Activity activity in Values ) { activity.Dispose(); }

        Clear();
    }
}
