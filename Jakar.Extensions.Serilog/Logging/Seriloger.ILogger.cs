using JetBrains.Annotations;



namespace Jakar.Extensions.Serilog;


public abstract partial class Serilogger<TSerilogger, TSeriloggerSettings>
{
    /// <summary> Create a Logger that enriches log events via the provided enrichers. </summary>
    /// <param name="enricher"> Enricher that applies in the context. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public ILogger ForContext( ILogEventEnricher enricher ) => Logger.ForContext( enricher );


    /// <summary> Create a Logger that enriches log events via the provided enrichers. </summary>
    /// <param name="enrichers"> Enrichers that apply in the context. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public ILogger ForContext( IEnumerable<ILogEventEnricher> enrichers ) => Logger.ForContext( enrichers );


    /// <summary> Create a Logger that enriches log events with the specified property. </summary>
    /// <param name="propertyName"> The name of the property. Must be non-empty. </param>
    /// <param name="value"> The property value. </param>
    /// <param name="destructureObjects"> If <see langword="true"/>, the value will be serialized as a structured object if possible; if <see langword="false"/>, the object will be recorded as a scalar or simple array. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public ILogger ForContext( string propertyName, object? value, bool destructureObjects = false ) => Logger.ForContext( propertyName, value, destructureObjects );


    /// <summary> Create a Logger that marks log events as being from the specified source type. </summary>
    /// <typeparam name="TSource"> Type generating log messages in the context. </typeparam>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public ILogger ForContext<TSource>() => ForContext( typeof(TSource) );


    /// <summary> Create a Logger that marks log events as being from the specified source type. </summary>
    /// <param name="source"> Type generating log messages in the context. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public ILogger ForContext( Type source ) => Logger.ForContext( source );


    /// <summary> Write an event to the log. </summary>
    /// <param name="logEvent"> The event to write. </param>
    public void Write( LogEvent logEvent ) => Logger.Write( logEvent );


    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate ) => Logger.Write( level, messageTemplate );


    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T>( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Logger.Write( level, messageTemplate, propertyValue );


    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1>( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Logger.Write( level, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1, T2>( LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Logger.Write( level, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> </param>
    /// <param name="propertyValues"> </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Logger.Write( level, messageTemplate, propertyValues );


    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Logger.Write( level, exception, messageTemplate );


    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T>( LogEventLevel level, Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Logger.Write( level, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1>( LogEventLevel level, Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Logger.Write( level, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1, T2>( LogEventLevel level, Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Logger.Write( level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Logger.Write( level, exception, messageTemplate, propertyValues );


    /// <summary> Determine if events at the specified level will be passed through to the log sinks. </summary>
    /// <param name="level"> Level to check. </param>
    /// <returns> <see langword="true"/> if the level is enabled; otherwise, <see langword="false"/>. </returns>
    public bool IsEnabled( LogEventLevel level ) => Logger.IsEnabled( level );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Verbose, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Verbose, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Verbose( (Exception?)null, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Verbose, exception, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Debug, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Debug, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Debug( (Exception?)null, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Debug, exception, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Information, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Information, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Information( (Exception?)null, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Information, exception, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Warning, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Warning, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Warning( (Exception?)null, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Warning, exception, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Error, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Error, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Error( (Exception?)null, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Error, exception, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1, T2>( Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Fatal, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Fatal, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( string messageTemplate, params object?[]? propertyValues ) => Fatal( (Exception?)null, messageTemplate, propertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Fatal, exception, messageTemplate, NoPropertyValues );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValue );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1, T2>( Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );


    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValues );
}
