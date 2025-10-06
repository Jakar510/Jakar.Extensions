// TrueLogic :: TrueLogic.Common
// 02/27/2025  11:02


using Serilog.Core;
using Serilog.Events;
using ZLinq;



namespace Jakar.Extensions;


public static class Enricher
{
    private static readonly ConcurrentDictionary<string, LogEventProperty> __sourceContexts = new();


    /// <summary>
    /// Gets the span unique identifier regardless of the activity identifier format.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The span unique identifier.</returns>
    public static string GetSpanID( this Activity activity )
    {
        ArgumentNullException.ThrowIfNull(activity);

        string? spanId = activity.IdFormat switch
                         {
                             ActivityIdFormat.Hierarchical => activity.Id,
                             ActivityIdFormat.W3C          => activity.SpanId.ToHexString(),
                             ActivityIdFormat.Unknown      => null,
                             _                             => null,
                         };

        return spanId ?? string.Empty;
    }

    /// <summary>
    /// Gets the span trace unique identifier regardless of the activity identifier format.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The span trace unique identifier.</returns>
    public static string GetTraceID( this Activity activity )
    {
        ArgumentNullException.ThrowIfNull(activity);

        string? traceId = activity.IdFormat switch
                          {
                              ActivityIdFormat.Hierarchical => activity.RootId,
                              ActivityIdFormat.W3C          => activity.TraceId.ToHexString(),
                              ActivityIdFormat.Unknown      => null,
                              _                             => null,
                          };

        return traceId ?? string.Empty;
    }

    /// <summary>
    /// Gets the span parent unique identifier regardless of the activity identifier format.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The span parent unique identifier.</returns>
    public static string GetParentID( this Activity activity )
    {
        ArgumentNullException.ThrowIfNull(activity);

        string? parentId = activity.IdFormat switch
                           {
                               ActivityIdFormat.Hierarchical => activity.ParentId,
                               ActivityIdFormat.W3C          => activity.ParentSpanId.ToHexString(),
                               ActivityIdFormat.Unknown      => null,
                               _                             => null,
                           };

        return parentId ?? string.Empty;
    }


    public static void             TryEnrich( this         LogEvent log, string sourceContext ) => log.AddPropertyIfAbsent(__sourceContexts.GetOrAdd(sourceContext, GetSourceProperty));
    public static LogEventProperty GetSourceProperty( this string   sourceContext ) => new(Constants.SourceContextPropertyName, new ScalarValue(sourceContext));


    public static LogEventProperty GetProperty( string?           value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( bool              value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( short             value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( int               value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( long              value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( float             value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( double            value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( in DateTimeOffset value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( in DateTime       value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( in TimeSpan       value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( in DateOnly       value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( in TimeOnly       value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( in Guid           value, string name ) => new(name, new ScalarValue(value));
    public static LogEventProperty GetProperty( AppVersion        value, string name ) => new(name, new ScalarValue(value.ToString()));
    public static LogEventProperty GetProperty<TEnum>( TEnum value, string name )
        where TEnum : struct, Enum => new(name, new ScalarValue(value.ToString()));


    public static LogEventProperty GetProperty( in ReadOnlySpan<GcGenerationInformation> value, string name ) => new(name,
                                                                                                                     new SequenceValue([
                                                                                                                                           ..value.AsValueEnumerable()
                                                                                                                                                  .Select(static v => v.GetProperty())
                                                                                                                                       ]));
    public static LogEventProperty GetProperty( in ReadOnlySpan<TimeSpan> value, string name ) => new(name,
                                                                                                      new SequenceValue([
                                                                                                                            ..value.AsValueEnumerable()
                                                                                                                                   .Select(v => new ScalarValue(v))
                                                                                                                        ]));


    public static LogEventProperty GetProperty( string?                                    description, ActivityStatusCode code ) => new(nameof(Activity.Status), new StructureValue([GetProperty(description, "Description"), GetProperty(code, "Code")]));
    public static LogEventProperty GetProperty( IEnumerable<ActivityEvent>                 events,      string             name ) => new(name, new StructureValue(events.Select(GetProperty)));
    public static LogEventProperty GetProperty( ActivityEvent                              value )              => new(value.Name, new StructureValue([GetProperty(value.Timestamp, nameof(ActivityEvent.Timestamp)), GetProperty(value.Tags, nameof(ActivityEvent.Tags))]));
    public static LogEventProperty GetProperty( IEnumerable<KeyValuePair<string, string?>> value, string name ) => new(name, new StructureValue(value.Select(GetProperty)));
    public static LogEventProperty GetProperty( KeyValuePair<string, string?>              value )              => new(value.Key, new ScalarValue(value.Value));
    public static LogEventProperty GetProperty( IEnumerable<KeyValuePair<string, object?>> value, string name ) => new(name, new StructureValue(value.Select(GetPropertyValue)));
    public static LogEventProperty GetPropertyValue( KeyValuePair<string, object?> tag ) =>
        tag.Value switch
        {
            string n         => new LogEventProperty(tag.Key, new ScalarValue(n)),
            short n          => new LogEventProperty(tag.Key, new ScalarValue(n)),
            int n            => new LogEventProperty(tag.Key, new ScalarValue(n)),
            long n           => new LogEventProperty(tag.Key, new ScalarValue(n)),
            float n          => new LogEventProperty(tag.Key, new ScalarValue(n)),
            double n         => new LogEventProperty(tag.Key, new ScalarValue(n)),
            DateTimeOffset n => new LogEventProperty(tag.Key, new ScalarValue(n)),
            DateTime n       => new LogEventProperty(tag.Key, new ScalarValue(n)),
            TimeSpan n       => new LogEventProperty(tag.Key, new ScalarValue(n)),
            DateOnly n       => new LogEventProperty(tag.Key, new ScalarValue(n)),
            TimeOnly n       => new LogEventProperty(tag.Key, new ScalarValue(n)),
            Guid n           => new LogEventProperty(tag.Key, new ScalarValue(n)),
            AppVersion n     => new LogEventProperty(tag.Key, new ScalarValue(n)),
            _ => throw new ExpectedValueTypeException(tag.Value,
                                                      typeof(string),
                                                      typeof(short),
                                                      typeof(int),
                                                      typeof(long),
                                                      typeof(float),
                                                      typeof(double),
                                                      typeof(DateTimeOffset),
                                                      typeof(DateTime),
                                                      typeof(TimeSpan),
                                                      typeof(DateOnly),
                                                      typeof(TimeOnly),
                                                      typeof(Guid),
                                                      typeof(AppVersion))
        };


    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( string?           value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( short             value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( int               value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( long              value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( float             value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( double            value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( in DateTimeOffset value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( in DateTime       value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( in TimeSpan       value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( in DateOnly       value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( in TimeOnly       value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( in Guid           value, string name ) => new(new ScalarValue(name), new ScalarValue(value));
    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( AppVersion        value, string name ) => new(new ScalarValue(name), new ScalarValue(value.ToString()));


    public static KeyValuePair<ScalarValue, LogEventPropertyValue> GetPropertyValue( IEnumerable<KeyValuePair<string, object?>> value, string name ) => new(new ScalarValue(name), new StructureValue(value.Select(GetPropertyValue)));
}
