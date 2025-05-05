// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 05/04/2025  23:59

using System.Diagnostics.Metrics;



namespace Jakar.Extensions.Telemetry.Meters;


public sealed class AppTelemetrySource : IDisposable
{
    public readonly TelemetryActivity       RootActivity;
    public readonly TelemetryActivitySource Source;
    public readonly AppVersion              Version;
    public readonly string                  Audience;
    public readonly Guid                    AppID;
    public readonly Meter                   Meter;
    public readonly string                  AppName;
    public readonly TelemetrySpan           App;
    public readonly Meters                  Meters;


    public static string? DeviceID { get; set; }


    public AppTelemetrySource( AppContext appContext, string audience )
    {
        ArgumentException.ThrowIfNullOrEmpty( appContext.Name );

        Activity.Current = null;
        AppName          = appContext.Name;
        Version          = appContext.Version;
        AppID            = appContext.ID;
        Audience         = audience;
        Source           = new TelemetryActivitySource( appContext );
        RootActivity     = GetActivity( AppName );
        App              = CreateSubSpan( RootActivity, AppName );
        Meters           = new Meters( this );
    }
    public void Dispose()
    {
        App.Dispose();
        RootActivity.Dispose();
        Meter.Dispose();
        Source.Dispose();
    }


    public static implicit operator AppVersion( AppTelemetrySource              sources ) { return sources.Version; }
    public static implicit operator TelemetryActivity( AppTelemetrySource       sources ) { return sources.RootActivity; }
    public static implicit operator Meter( AppTelemetrySource                   sources ) { return sources.Meter; }
    public static implicit operator TelemetryActivitySource( AppTelemetrySource sources ) { return sources.Source; }

    // public static implicit operator Meters( AppSources sources ) => sources.Meters;

    public static ActivityContext RandomContext() { return new ActivityContext( ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded ); }


    public AppTelemetrySource SetActive()
    {
        Activity.Current = RootActivity;
        return this;
    }


    [Pure] public TelemetrySpan CreateSubSpan( string            name )                                                           { return App.CreateSubSpan( name ); }
    [Pure] public TelemetrySpan CreateSubSpan( TelemetryActivity parent, string name, ActivityKind kind = ActivityKind.Internal ) { return new TelemetrySpan( this, parent, name ); }


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
    TelemetrySpan IActivityTracer.      Trace { get { return this; } }


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

    public static implicit operator TelemetryActivity( TelemetrySpan activity ) { return activity.Activity; }


    public void SetInactive() { TelemetryActivity.Current = _parent; }
    public void SetCurrent( [CallerMemberName] string caller = EMPTY )
    {
        Activity.Current = Activity;
        Activity.TrackEvent( caller );
    }


    [Pure] public        TelemetrySpan CreateSubSpan( [CallerMemberName] string caller                                                             = EMPTY ) { return new TelemetrySpan( _source, Activity,        caller ); }
    [Pure] public static TelemetrySpan Create( AppTelemetrySource               source, TelemetryActivity parent, [CallerMemberName] string caller = EMPTY ) { return new TelemetrySpan( source,  parent,          caller ); }
    [Pure] public static TelemetrySpan Create( AppTelemetrySource               source, TelemetrySpan     parent, [CallerMemberName] string caller = EMPTY ) { return new TelemetrySpan( source,  parent.Activity, caller ); }


    public TelemetrySpan AddTag( string key, string? value )
    {
        Activity.Tags.AddLast( new Pair( key, value ) );
        return this;
    }
    public TelemetrySpan AddTag( string key, object? value )
    {
        Activity.Tags.AddLast( new Pair( key, value?.ToString() ) );
        return this;
    }

    public TelemetrySpan SetTag( string key, object? value )
    {
        Pair pair = Activity.Tags.First( x => string.Equals( x.Key, key, StringComparison.Ordinal ) );
        pair.Value = value?.ToString();
        return this;
    }

    public TelemetrySpan AddBaggage( string key, string? value )
    {
        Activity.AddBaggage( key, value );
        return this;
    }

    public TelemetrySpan SetBaggage( string key, string? value )
    {
        Activity.SetBaggage( key, value );
        return this;
    }


    public TelemetrySpan AddEvent( ActivityEvent e )
    {
        Activity.AddEvent( e );
        return this;
    }

    public TelemetrySpan AddLink( ActivityLink link )
    {
        Activity.AddLink( link );
        return this;
    }

    public TelemetrySpan AddException( Exception exception, in TagList tags = default, DateTimeOffset? timestamp = null )
    {
        Activity.AddException( exception, in tags, timestamp ?? DateTimeOffset.UtcNow );
        return this;
    }
}
