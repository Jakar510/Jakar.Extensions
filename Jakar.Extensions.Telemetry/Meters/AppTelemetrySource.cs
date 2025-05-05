// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 05/04/2025  23:59

using System.Diagnostics.Metrics;



namespace Jakar.Extensions.Telemetry.Meters;


public sealed class AppTelemetrySource : IDisposable
{
    public readonly Activity                RootActivity;
    public readonly TelemetryActivitySource Source;
    public readonly AppVersion              Version;
    public readonly string                  Audience;
    public readonly Guid                    AppID;
    public readonly Meter                   Meter;
    public readonly string                  AppName;
    public readonly TelemetrySpan           App;
    public readonly Meters                  Meters;

    public static string? DeviceID { get; set; }


    public AppTelemetrySource( Guid appID, string appName, AppVersion appVersion, string audience )
    {
        ArgumentException.ThrowIfNullOrEmpty( appName );

        Activity.Current = null;
        AppName          = appName;
        Version          = appVersion;
        AppID            = appID;
        Audience         = audience;
        Source           = new TelemetryActivitySource( appName, appVersion.ToString() );
        Meter            = new Meter( appName, appVersion.ToString() );
        RootActivity     = GetActivity( AppName );
        App              = CreateSubSpan( RootActivity, appName );
        Meters           = new Meters( this );
    }
    public void Dispose()
    {
        App.Dispose();
        RootActivity.Dispose();
        Meter.Dispose();
        Source.Dispose();
    }


    public static implicit operator AppVersion( AppTelemetrySource              sources ) => sources.Version;
    public static implicit operator Activity( AppTelemetrySource                sources ) => sources.RootActivity;
    public static implicit operator Meter( AppTelemetrySource                   sources ) => sources.Meter;
    public static implicit operator TelemetryActivitySource( AppTelemetrySource sources ) => sources.Source;

    // public static implicit operator Meters( AppSources sources ) => sources.Meters;

    public static ActivityContext RandomContext() => new(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded);


    public AppTelemetrySource SetActive()
    {
        Activity.Current = RootActivity;
        return this;
    }


    [Pure] public TelemetrySpan CreateSubSpan( string   name )                                                           => App.CreateSubSpan( name );
    [Pure] public TelemetrySpan CreateSubSpan( Activity parent, string name, ActivityKind kind = ActivityKind.Internal ) => new(this, parent, name);


    [Pure]
    public TelemetryActivity GetActivity( string name, TelemetryActivityContext? parentContext = null, ActivityKind kind = ActivityKind.Internal )
    {
        ArgumentException.ThrowIfNullOrEmpty( name );

        Debug.Assert( Source.HasListeners() );
        TelemetryActivity activity = Source.CreateActivity( name, kind ) ?? throw new InvalidOperationException( $"{nameof(Source)}.{nameof(Source.CreateActivity)}" );

        activity.SetStatus( StatusCode.Ok );
        activity.Tags.AddLast( new Pair( nameof(AppName),  AppName ) );
        activity.Tags.AddLast( new Pair( nameof(AppID),    AppID.ToString() ) );
        activity.Tags.AddLast( new Pair( nameof(Version),  Version.ToString() ) );
        activity.Tags.AddLast( new Pair( nameof(DeviceID), DeviceID ) );

        if ( parentContext.HasValue )
        {
            TelemetryActivityContext context = parentContext.Value;
            activity.SetParentID( context.TraceID, context.SpanID, context.TraceFlags );
        }

        return activity.Start();
    }


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
}



public interface IActivityTracer
{
    public TelemetrySpan Trace { get; }
}



[NotSerializable]
public sealed class TelemetrySpan : BaseClass, IDisposable, IActivityTracer
{
    public const     string             ELAPSED_TIME  = nameof(ELAPSED_TIME);
    public const     string             START_STOP_ID = nameof(START_STOP_ID);
    private readonly TelemetryActivity  _parent;
    public readonly  TelemetryActivity  Activity;
    private readonly AppTelemetrySource _source;
    private readonly string             _endTag;
    private readonly TelemetryStopWatch _sw;
    TelemetrySpan IActivityTracer.      Trace => this;


    public TelemetrySpan( AppTelemetrySource source, TelemetryActivity parent, string name )
    {
        Activity = Activity.Current = source.GetActivity( $"{parent.DisplayName}.{name}", parent.Context );
        _endTag  = $"{Activity.DisplayName}.End";
        _source  = source;
        _parent  = parent;
        TelemetryStopWatch     sw         = _sw = TelemetryStopWatch.Start( name );
        ActivityTagsCollection collection = new() { [START_STOP_ID] = sw.ID };
        Activity.AddEvent( $"{Activity.DisplayName}.Start".GetEvent( collection ) );
    }
    public void Dispose()
    {
        ActivityTagsCollection collection = new()
                                            {
                                                [START_STOP_ID] = _sw.ID,
                                                [ELAPSED_TIME]  = _sw.Elapsed
                                            };

        Activity.AddEvent( _endTag.GetEvent( collection ) );
        Activity.Stop();
        Activity.Current = _parent;
    }

    public static implicit operator TelemetryActivity( TelemetrySpan activity ) => activity.Activity;


    public void SetInactive() => TelemetryActivity.Current = _parent;
    public void SetCurrent( [CallerMemberName] string caller = EMPTY )
    {
        Activity.Current = Activity;
        Activity.TrackEvent( caller );
    }


    [Pure] public        TelemetrySpan CreateSubSpan( [CallerMemberName] string caller                                                         = EMPTY ) => new(_source, Activity, caller);
    [Pure] public static TelemetrySpan Create( AppTelemetrySource               source, Activity      parent, [CallerMemberName] string caller = EMPTY ) => new(source, parent, caller);
    [Pure] public static TelemetrySpan Create( AppTelemetrySource               source, TelemetrySpan parent, [CallerMemberName] string caller = EMPTY ) => new(source, parent.Activity, caller);


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity AddTag( string     key, string? value ) => Activity.AddTag( key, (object?)value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity AddTag( string     key, object? value ) => Activity.AddTag( key, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity SetTag( string     key, object? value ) => Activity.SetTag( key, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity AddBaggage( string key, string? value ) => Activity.AddBaggage( key, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity SetBaggage( string key, string? value ) => Activity.SetBaggage( key, value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity AddEvent( ActivityEvent e )    => Activity.AddEvent( e );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity AddLink( ActivityLink   link ) => Activity.AddLink( link );

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public Activity AddException( Exception exception, in TagList tags = default, DateTimeOffset? timestamp = null ) => Activity.AddException( exception, in tags, timestamp ?? DateTimeOffset.UtcNow );
}
