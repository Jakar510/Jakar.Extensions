// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 05/04/2025  23:59

namespace Jakar.Extensions.Telemetry.Meters;


public sealed class TelemetrySource : Jakar.Extensions.TelemetrySource, IDisposable
{
    public readonly Meters                  Meters;
    public readonly TelemetryActivitySource Source;
    public readonly TelemetrySpan           App;
    private         TelemetryActivity?      __rootActivity;

    public static string?           DeviceID     { get; set; }
    public        TelemetryActivity RootActivity => __rootActivity ??= GetActivity(Info.AppName);


    public TelemetrySource( in AppInformation info ) : base(in info)
    {
        ArgumentException.ThrowIfNullOrEmpty(info.AppName);

        Activity.Current = null;
        Source           = new TelemetryActivitySource(in info);
        App              = CreateSubSpan(RootActivity, Info.AppName);
        Meters           = new Meters(this);
    }
    public override void Dispose()
    {
        base.Dispose();
        App.Dispose();
        Meters.Dispose();
        Source.Dispose();
    }


    public static implicit operator AppVersion( TelemetrySource              sources ) => sources.Info.Version;
    public static implicit operator Meters( TelemetrySource                  sources ) => sources.Meters;
    public static implicit operator TelemetryActivitySource( TelemetrySource sources ) => sources.Source;

    // public static implicit operator Meters( AppSources sources ) => sources.Meters;

    public static ActivityContext RandomContext() => new(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded);


    public TelemetrySource SetActive()
    {
        TelemetryActivity.Current = RootActivity;
        return this;
    }


    [Pure] public TelemetrySpan CreateSubSpan( string            name )                                                           => App.CreateSubSpan(name);
    [Pure] public TelemetrySpan CreateSubSpan( TelemetryActivity parent, string name, ActivityKind kind = ActivityKind.Internal ) => new(this, parent, name);


    [Pure]
    public TelemetryActivity GetActivity( string operationName, TelemetryActivity? parent = null, ActivityKind kind = ActivityKind.Internal )
    {
        ArgumentException.ThrowIfNullOrEmpty(operationName);
        TelemetryActivity activity = Source.GetOrAddActivity(operationName).SetKind(kind).SetParent(parent);
        activity.SetStatus(StatusCode.Ok);
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



[NotSerializable, StructLayout(LayoutKind.Auto)]
public readonly struct TelemetrySpan : IDisposable, IActivityTracer
{
    public const     string             ELAPSED_TIME  = nameof(ELAPSED_TIME);
    public const     string             START_STOP_ID = nameof(START_STOP_ID);
    private readonly TelemetryActivity  _parent;
    public readonly  TelemetryActivity  Activity;
    private readonly TelemetrySource    _source;
    private readonly string             _endTag;
    private readonly TelemetryStopWatch _sw;
    TelemetrySpan IActivityTracer.      Trace => this;


    public TelemetrySpan( TelemetrySource source, TelemetryActivity parent, string name )
    {
        Activity = TelemetryActivity.Current = source.GetActivity($"{parent.DisplayName}.{name}", parent);
        _endTag  = $"{Activity.DisplayName}.End";
        _source  = source;
        _parent  = parent;
        TelemetryStopWatch sw         = _sw = TelemetryStopWatch.Start(name);
        Pairs              collection = [new Pair(START_STOP_ID, sw.ID)];
        Activity.AddEvent(GetEvent($"{Activity.DisplayName}.Start", collection));
    }
    public void Dispose()
    {
        Pairs collection = [new Pair(START_STOP_ID, _sw.ID), new Pair(ELAPSED_TIME, _sw.Elapsed.ToString())];
        Activity.AddEvent(GetEvent(_endTag, collection));
        Activity.Stop();
        TelemetryActivity.Current = _parent;
    }

    public static implicit operator TelemetryActivity( TelemetrySpan activity ) => activity.Activity;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TelemetryEvent GetEvent( string name, Pairs tags ) => new(name, DateTimeOffset.UtcNow, tags);
    public                                                           void           SetInactive() => TelemetryActivity.Current = _parent;
    public void SetCurrent( [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        TelemetryActivity.Current = Activity;
        Activity.AddEvent(caller);
    }


    [Pure] public        TelemetrySpan CreateSubSpan( [CallerMemberName] string caller                                                             = BaseRecord.EMPTY ) => new(_source, Activity, caller);
    [Pure] public static TelemetrySpan Create( TelemetrySource                  source, TelemetryActivity parent, [CallerMemberName] string caller = BaseRecord.EMPTY ) => new(source, parent, caller);
    [Pure] public static TelemetrySpan Create( TelemetrySource                  source, TelemetrySpan     parent, [CallerMemberName] string caller = BaseRecord.EMPTY ) => new(source, parent.Activity, caller);


    public TelemetrySpan AddTag( string key, string? value )
    {
        Activity.Tags.Add(new Pair(key, value));
        return this;
    }
    public TelemetrySpan AddTag<T>( string key, T? value )
    {
        Activity.Tags.Add(new Pair(key, value?.ToString()));
        return this;
    }
    public TelemetrySpan SetTag( string key, string? value )
    {
        Activity.Tags.SetTag(key, value);
        return this;
    }


    public TelemetrySpan AddBaggage( string key, string? value )
    {
        Activity.AddTag(key, value);
        return this;
    }
    public TelemetrySpan SetBaggage( string key, string? value )
    {
        Activity.AddTag(key, value);
        return this;
    }


    public TelemetrySpan AddEvent( TelemetryEvent e )
    {
        Activity.AddEvent(e);
        return this;
    }

    public TelemetrySpan AddLink( TelemetryActivityLink link )
    {
        Activity.AddLink(link);
        return this;
    }

    public TelemetrySpan AddException( Exception exception, in Pairs tags = default, DateTimeOffset? timestamp = null )
    {
        Activity.AddException(exception, in tags, timestamp ?? DateTimeOffset.UtcNow);
        return this;
    }
}
