// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2024  10:06

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;



namespace Jakar.Extensions.Telemetry;


public sealed class ActivityCollection : ConcurrentDictionary<string, Activity>, IDisposable
{
    public required AppVersion AppVersion { get; init; }
    public required string     AppName    { get; init; }


    public ActivityCollection() : base( Environment.ProcessorCount, 64, StringComparer.Ordinal ) { }
    public ActivityCollection( IEnumerable<KeyValuePair<string, Activity>> values ) : base( values, StringComparer.Ordinal ) { }
    public ActivityCollection( IDictionary<string, Activity>               values ) : base( values, StringComparer.Ordinal ) { }


    public Activity CreateActivity( string operationName, ActivityKind kind = ActivityKind.Internal ) => this[operationName] = Activity.Create( operationName, kind );
    public Activity StartActivity( string  operationName, ActivityKind kind = ActivityKind.Internal ) => CreateActivity( operationName, kind ).Start();
    public static ActivityCollection Create<TApp>()
        where TApp : IAppName => Create( TApp.AppName, TApp.AppVersion );
    public static ActivityCollection Create( string name, AppVersion appVersion ) => new()
                                                                                     {
                                                                                         AppName    = name,
                                                                                         AppVersion = appVersion
                                                                                     };
    public void Dispose()
    {
        foreach ( Activity activity in Values ) { activity.Dispose(); }

        Clear();
    }
}
