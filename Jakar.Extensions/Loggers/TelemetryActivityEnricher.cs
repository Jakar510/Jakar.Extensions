// TrueLogic :: TrueLogic.Models
// 07/29/2025  09:23

using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions;


public interface ITelemetryActivityEnricher
{
    DeviceInfo           DeviceInfo { get; }
    ref readonly AppInfo Info       { get; }
}



/// <summary> A log event enricher which adds span information from the current <see cref="Activity"/>. </summary>
public sealed class TelemetryActivityEnricher( ITelemetryActivityEnricher options, TelemetrySource source ) : ILogEventEnricher
{
    public const     string                     PARENT_ID     = "ParentId";
    public const     string                     PARENT_ID_KEY = "Serilog.ParentId";
    public const     string                     SPAN_ID_KEY   = "Serilog.SpanId";
    public const     string                     SPAN_ID       = "SpanId";
    public const     string                     TRACE_ID_KEY  = "Serilog.TraceId";
    public const     string                     TRACE_ID      = "TraceId";
    private readonly LogEventProperty           __appInfo     = Enricher.GetProperty(in source.Info);
    private readonly ITelemetryActivityEnricher __options     = options;


    public static ILogEventEnricher[] Enrichers { get; set; } = [];


    /// <summary> Enrich the log event. </summary>
    /// <param name="log"> The log event to enrich. </param>
    /// <param name="factory"> Factory for creating new properties to add to the event. </param>
    public void Enrich( LogEvent log, ILogEventPropertyFactory factory ) => Enrich(log, factory, Enrichers);
    public void Enrich( LogEvent log, ILogEventPropertyFactory factory, params ReadOnlySpan<ILogEventEnricher> enrichers )
    {
        ArgumentNullException.ThrowIfNull(log);

        // _options.ScreenShot?.Enrich( log, factory ); // size limit of 262144
        log.AddPropertyIfAbsent(__appInfo);
        __options.DeviceInfo.Enrich(log, factory);

        if ( log.Level >= LogEventLevel.Warning )
        {
            log.AddOrUpdateProperty(Enricher.GetProperty(ThreadInfo.Create()));
            log.AddOrUpdateProperty(Enricher.GetProperty(GcInfo.Create()));
        }

        foreach ( ref readonly ILogEventEnricher enricher in enrichers ) { enricher.Enrich(log, factory); }

        Activity? activity = Activity.Current;
        if ( activity is null ) { return; }

        AddSpanId(log, activity);
        AddTraceId(log, activity);
        AddParentId(log, activity);
        AddActivityKind(log, activity);
        AddOperationName(log, activity);
        AddTraceFlags(log, activity);
        AddBaggage(log, activity);
        AddTags(log, activity);
        AddEvents(log, activity);
    }


    private static void AddSpanId( LogEvent log, Activity activity )
    {
        object? property = activity.GetCustomProperty(SPAN_ID_KEY);

        if ( property is not LogEventProperty logProperty )
        {
            logProperty = Enricher.GetProperty(activity.GetTraceId(), SPAN_ID);
            activity.SetCustomProperty(SPAN_ID_KEY, logProperty);
        }

        log.AddPropertyIfAbsent(logProperty);
    }
    private static void AddTraceId( LogEvent log, Activity activity )
    {
        object? property = activity.GetCustomProperty(TRACE_ID_KEY);

        if ( property is not LogEventProperty logProperty )
        {
            logProperty = Enricher.GetProperty(activity.GetTraceId(), PARENT_ID);
            activity.SetCustomProperty(TRACE_ID_KEY, logProperty);
        }

        log.AddPropertyIfAbsent(logProperty);
    }
    private static void AddParentId( LogEvent log, Activity activity )
    {
        object? property = activity.GetCustomProperty(PARENT_ID_KEY);

        if ( property is not LogEventProperty logProperty )
        {
            logProperty = Enricher.GetProperty(activity.GetParentId(), PARENT_ID);
            activity.SetCustomProperty(PARENT_ID_KEY, logProperty);
        }

        log.AddOrUpdateProperty(logProperty);
    }
    private static void AddEvents( LogEvent        log, Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.Events,             nameof(Activity.Events)));
    private static void AddOperationName( LogEvent log, Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.OperationName,      nameof(Activity.OperationName)));
    private static void AddTags( LogEvent          log, Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.Tags,               nameof(Activity.Tags)));
    private static void AddBaggage( LogEvent       log, Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.Baggage,            nameof(Activity.Baggage)));
    private static void AddTraceFlags( LogEvent    log, Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.ActivityTraceFlags, nameof(Activity.ActivityTraceFlags)));
    private static void AddActivityKind( LogEvent  log, Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.StatusDescription,  activity.Status));
}
