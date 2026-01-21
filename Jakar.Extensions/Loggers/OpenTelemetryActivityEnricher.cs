// Jakar.Extensions :: Jakar.Extensions
// 08/13/2025  10:16

using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions;


/// <summary> A log event enricher which adds span information from the current <see cref="Activity"/>. </summary>
public sealed class OpenTelemetryActivityEnricher( IOpenTelemetryActivityEnricher options, TelemetrySource source ) : ILogEventEnricher
{
    public const    string           PARENT_ID     = "ParentId";
    public const    string           PARENT_ID_KEY = "Serilog.ParentId";
    public const    string           SPAN_ID       = "SpanId";
    public const    string           SPAN_ID_KEY   = "Serilog.SpanId";
    public const    string           TRACE_ID      = "TraceId";
    public const    string           TRACE_ID_KEY  = "Serilog.TraceId";
    public readonly LogEventProperty _appInfo      = source.Info.GetProperty();


    /// <summary> Enrich the log event. </summary>
    /// <param name="log"> The log event to enrich. </param>
    /// <param name="factory"> Factory for creating new properties to add to the event. </param>
    public void Enrich( LogEvent log, ILogEventPropertyFactory factory ) => Enrich(log, factory, options.Enrichers);
    public void Enrich( LogEvent log, ILogEventPropertyFactory factory, params ReadOnlySpan<ILogEventEnricher> enrichers )
    {
        ArgumentNullException.ThrowIfNull(log);

        // _options.ScreenShot?.Enrich( log, factory ); // size limit of 262144
        log.AddPropertyIfAbsent(_appInfo);
        options.DeviceInfo.Enrich(log, factory);

        if ( log.Level >= LogEventLevel.Warning )
        {
            log.AddOrUpdateProperty(ThreadInformation.Create()
                                                     .GetProperty());

            log.AddOrUpdateProperty(GcInfo.Create()
                                          .GetProperty());
        }

        foreach ( ref readonly ILogEventEnricher enricher in enrichers ) { enricher.Enrich(log, factory); }

        TryAddDeviceInformation(in log, source);

        Activity? activity = Activity.Current;
        if ( activity is null ) { return; }

        AddSpanId(in log, in activity);
        AddTraceId(in log, in activity);
        AddParentId(in log, in activity);
        AddActivityKind(in log, in activity);
        AddOperationName(in log, in activity);
        AddTraceFlags(in log, in activity);
        AddBaggage(in log, in activity);
        AddTags(in log, in activity);
        AddEvents(in log, in activity);
    }


    public static void TryAddDeviceInformation( ref readonly LogEvent log, TelemetrySource source )
    {
        DeviceInformation? device = source.TryGetDeviceInformation();
        if ( device is not null ) { log.AddOrUpdateProperty(device.ToProperty()); }
    }
    public static void AddSpanId( ref readonly LogEvent log, ref readonly Activity activity )
    {
        object? property = activity.GetCustomProperty(SPAN_ID_KEY);

        if ( property is not LogEventProperty logProperty )
        {
            logProperty = Enricher.GetProperty(activity.GetTraceID(), SPAN_ID);
            activity.SetCustomProperty(SPAN_ID_KEY, logProperty);
        }

        log.AddPropertyIfAbsent(logProperty);
    }
    public static void AddTraceId( ref readonly LogEvent log, ref readonly Activity activity )
    {
        object? property = activity.GetCustomProperty(TRACE_ID_KEY);

        if ( property is not LogEventProperty logProperty )
        {
            logProperty = Enricher.GetProperty(activity.GetTraceID(), PARENT_ID);
            activity.SetCustomProperty(TRACE_ID_KEY, logProperty);
        }

        log.AddPropertyIfAbsent(logProperty);
    }
    public static void AddParentId( ref readonly LogEvent log, ref readonly Activity activity )
    {
        object? property = activity.GetCustomProperty(PARENT_ID_KEY);

        if ( property is not LogEventProperty logProperty )
        {
            logProperty = Enricher.GetProperty(activity.GetParentID(), PARENT_ID);
            activity.SetCustomProperty(PARENT_ID_KEY, logProperty);
        }

        log.AddOrUpdateProperty(logProperty);
    }
    public static void AddEvents( ref readonly        LogEvent log, ref readonly Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.Events,             nameof(Activity.Events)));
    public static void AddOperationName( ref readonly LogEvent log, ref readonly Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.OperationName,      nameof(Activity.OperationName)));
    public static void AddTags( ref readonly          LogEvent log, ref readonly Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.Tags,               nameof(Activity.Tags)));
    public static void AddBaggage( ref readonly       LogEvent log, ref readonly Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.Baggage,            nameof(Activity.Baggage)));
    public static void AddTraceFlags( ref readonly    LogEvent log, ref readonly Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.ActivityTraceFlags, nameof(Activity.ActivityTraceFlags)));
    public static void AddActivityKind( ref readonly  LogEvent log, ref readonly Activity activity ) => log.AddPropertyIfAbsent(Enricher.GetProperty(activity.StatusDescription,  activity.Status));
}
