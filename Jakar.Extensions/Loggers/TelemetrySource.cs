// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  15:01

using System.Diagnostics.Metrics;



namespace Jakar.Extensions;


public interface ITelemetrySource
{
    public ref readonly AppInformation Info   { get; }
    public ref readonly Meter          Meter  { get; }
    public ref readonly ActivitySource Source { get; }


    public Activity? StartActivity( string name, Activity?          parent = null, ActivityTagsCollection? tags = null, ActivityLink[]? links = null, ActivityKind kind = ActivityKind.Internal, ActivityIdFormat idFormat = ActivityIdFormat.Hierarchical, ActivityTraceFlags traceFlags = ActivityTraceFlags.Recorded );
    public Activity? StartActivity( string name, in ActivityContext parentContext, ActivityTagsCollection? tags = null, ActivityLink[]? links = null, ActivityKind kind = ActivityKind.Internal, ActivityIdFormat idFormat = ActivityIdFormat.Hierarchical, ActivityTraceFlags traceFlags = ActivityTraceFlags.Recorded );


    public DeviceInformation? GetDeviceInformation();
}



public class TelemetrySource : ITelemetrySource, IDisposable, IFuzzyEquals<TelemetrySource>
{
    /*
    public void AddServices( IServiceCollection services )
    {
        services.AddOpenTelemetryTracing( b => b.AddSource( Source.Name )
                                                .SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( AppName ) )
                                                .AddAspNetCoreInstrumentation() // if using any server bits
                                                .AddHttpClientInstrumentation()
                                                .AddConsoleExporter() );

        services.AddOpenTelemetryMetrics( b => b.AddMeter( Meter.Name ).SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( AppName ) ) );
    }
    */

    public static readonly ActivityContext                 EmptyActivityContext = default;
    public readonly        ActivitySource                  Source;
    public readonly        AppInformation                  Info;
    public readonly        Meter                           Meter;
    public static          TelemetrySource?                Current        { get; set; }
    public static          FuzzyEqualizer<TelemetrySource> FuzzyEqualizer { get => FuzzyEqualizer<TelemetrySource>.Default; }


    ref readonly AppInformation ITelemetrySource.Info   => ref Info;
    ref readonly Meter ITelemetrySource.         Meter  => ref Meter;
    ref readonly ActivitySource ITelemetrySource.Source => ref Source;


    static TelemetrySource() => Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;
    public TelemetrySource( in AppInformation info )
    {
        ArgumentException.ThrowIfNullOrEmpty(info.AppName);
        Activity.Current = null;
        Info             = info;
        Source           = new ActivitySource(info.AppName, info.Version.ToString());
        Meter            = new Meter(info.AppName, info.Version.ToString());
    }
    public virtual void Dispose()
    {
        Meter.Dispose();
        Source.Dispose();
        GC.SuppressFinalize(this);
    }


    public static implicit operator AppVersion( TelemetrySource     sources ) => sources.Info.Version;
    public static implicit operator Guid( TelemetrySource           sources ) => sources.Info.AppID;
    public static implicit operator Meter( TelemetrySource          sources ) => sources.Meter;
    public static implicit operator ActivitySource( TelemetrySource sources ) => sources.Source;

    /*
    public void AddServices( IServiceCollection services )
    {
        services.AddOpenTelemetryTracing( b => b.AddSource( Source.Name )
                                                .SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( AppName ) )
                                                .AddAspNetCoreInstrumentation() // if using any server bits
                                                .AddHttpClientInstrumentation()
                                                .AddConsoleExporter() );

        services.AddOpenTelemetryMetrics( b => b.AddMeter( Meter.Name ).SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( AppName ) ) );
    }
    */


    public Activity? StartActivity( string name, Activity? parent = null, ActivityTagsCollection? tags = null, ActivityLink[]? links = null, ActivityKind kind = ActivityKind.Internal, ActivityIdFormat idFormat = ActivityIdFormat.Hierarchical, ActivityTraceFlags traceFlags = ActivityTraceFlags.Recorded )
    {
        ActivityContext parentContext = ( parent ?? Activity.Current )?.Context ?? EmptyActivityContext;
        return StartActivity(name, in parentContext, tags, links, kind, idFormat, traceFlags);
    }
    public Activity? StartActivity( string name, in ActivityContext parentContext, ActivityTagsCollection? tags = null, ActivityLink[]? links = null, ActivityKind kind = ActivityKind.Internal, ActivityIdFormat idFormat = ActivityIdFormat.Hierarchical, ActivityTraceFlags traceFlags = ActivityTraceFlags.Recorded )
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        if ( !Source.HasListeners() ) { return null; }

        Activity activity = Source.CreateActivity(name, kind, parentContext, tags, links, idFormat) ?? throw new InvalidOperationException($"{nameof(Source)}.{nameof(Source.CreateActivity)}");
        activity.ActivityTraceFlags = traceFlags;
        activity.IsAllDataRequested = true;
        activity.TraceStateString   = null;

        activity.SetStatus(ActivityStatusCode.Ok);
        return activity.Start();
    }


    public virtual DeviceInformation? GetDeviceInformation() => null;


    public int CompareTo( object? other ) => other is null
                                                 ? 1
                                                 : ReferenceEquals(this, other)
                                                     ? 0
                                                     : other is TelemetrySource source
                                                         ? CompareTo(source)
                                                         : throw new ExpectedValueTypeException(nameof(other), other, typeof(TelemetrySource));
    public int CompareTo( TelemetrySource? other ) => ReferenceEquals(this, other)
                                                          ? 0
                                                          : other is null
                                                              ? 1
                                                              : Info.CompareTo(other.Info);
    public          bool Equals( TelemetrySource? other )     => other is not null && ( ReferenceEquals(this, other) || Info.Equals(other.Info) );
    public override bool Equals( object?          obj )       => ReferenceEquals(this, obj) || ( obj is TelemetrySource other && Equals(other) );
    public override int  GetHashCode()                        => Info.GetHashCode();
    public          bool FuzzyEquals( TelemetrySource other ) => Info.Version.FuzzyEquals(other.Info.Version);
    public          bool Equals( AppVersion?          other ) => Info.Version.Equals(other);
    public          bool FuzzyEquals( AppVersion      other ) => Info.Version.FuzzyEquals(other);


    public static bool operator ==( TelemetrySource? left, TelemetrySource? right ) => EqualityComparer<TelemetrySource>.Default.Equals(left, right);
    public static bool operator !=( TelemetrySource? left, TelemetrySource? right ) => !EqualityComparer<TelemetrySource>.Default.Equals(left, right);
    public static bool operator >( TelemetrySource   left, TelemetrySource  right ) => Comparer<TelemetrySource>.Default.Compare(left, right) > 0;
    public static bool operator >=( TelemetrySource  left, TelemetrySource  right ) => Comparer<TelemetrySource>.Default.Compare(left, right) >= 0;
    public static bool operator <( TelemetrySource   left, TelemetrySource  right ) => Comparer<TelemetrySource>.Default.Compare(left, right) < 0;
    public static bool operator <=( TelemetrySource  left, TelemetrySource  right ) => Comparer<TelemetrySource>.Default.Compare(left, right) <= 0;
}
