// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:55 AM

namespace Jakar.AppLogger.Common;


[ Serializable ]
public sealed record AppLog
    ( [ property: MaxLength( IAppLog.MESSAGE_LENGTH ) ] string Message, LogLevel Level, EventID EventID, DateTimeOffset TimeStamp, StartSessionReply Session, DeviceDescriptor Device ) : BaseJsonModelRecord, IAppLog, ILogDetails, ILogScopes
{
    Guid IStartSession.                                                            AppID        => Session.AppID;
    [ MaxLength( IAppLog.APP_USER_ID_LENGTH ) ] public   string?                   AppUserID    { get; init; }
    public                                               HashSet<LoggerAttachment> Attachments  { get; init; } = new();
    [ MaxLength( IAppLog.BUILD_ID_LENGTH ) ]      public string?                   BuildID      { get; init; }
    [ MaxLength( IAppLog.CATEGORY_NAME_LENGTH ) ] public string?                   CategoryName { get; init; }
    Guid IStartSession.                                                            DeviceID     => Session.DeviceID;
    public                ExceptionDetails?                                        Exception    { get; init; }
    public                Guid                                                     ID           { get; init; }
    [ JsonIgnore ] public bool                                                     IsError      => Level > LogLevel.Error;
    public                bool                                                     IsFatal      { get; init; }
    public                bool                                                     IsValid      => !string.IsNullOrWhiteSpace( Message );
    Guid ILogInfo.                                                                 LogID        => ID;
    public HashSet<Guid>                                                           ScopeIDs     { get; init; } = new();
    Guid? ISessionID.                                                              SessionID    => Session.SessionID;
    string? IAppLog.                                                               StackTrace   => GetStackTrace();
    public int                                                                     ThreadID     { get; init; } = Environment.CurrentManagedThreadId;


    public AppLog( IAppLog log, IEnumerable<LoggerAttachment> attachments, IEnumerable<Guid> scopeIDs, DeviceDescriptor device, ExceptionDetails? details ) : this( log.Message, log.Level, log.EventID, log.TimeStamp, log.Session, device )
    {
        ID           = log.ID;
        Message      = log.Message;
        Level        = log.Level;
        TimeStamp    = log.TimeStamp;
        Session      = log.Session;
        AppUserID    = log.AppUserID;
        EventID      = log.EventID;
        BuildID      = log.BuildID;
        IsFatal      = log.IsFatal;
        CategoryName = log.CategoryName;
        Exception    = details;
        ScopeIDs     = new HashSet<Guid>( scopeIDs );
        Attachments  = new HashSet<LoggerAttachment>( attachments );
    }
    public static AppLog Create( AppLogger logger, LogLevel level, EventID eventID, string message, IEnumerable<LoggerAttachment>? attachments = default, IDictionary<string, JToken?>? eventDetails = default, ExceptionDetails? exception = default ) =>
        new(message, level, eventID, DateTimeOffset.UtcNow, logger.Config.Session ?? throw new ApiDisabledException( nameof(AppLoggerOptions.Config) ), logger.Config.Device)
        {
            AppUserID      = logger.Config.AppUserID,
            Device         = logger.Config.Device,
            CategoryName   = logger.CategoryName,
            EventID        = eventID,
            Level          = level,
            ID             = Guid.NewGuid(),
            Message        = message,
            AdditionalData = eventDetails,
            Exception      = exception,
            ScopeIDs       = new HashSet<Guid>( logger.Config.ScopeIDs ),
            Attachments    = new HashSet<LoggerAttachment>( attachments ?? Array.Empty<LoggerAttachment>() )
        };


    public static AppLog Create( AppLogger logger, LogLevel logLevel, EventID eventID, string? message, Exception? e, IEnumerable<LoggerAttachment>? attachments = default, IDictionary<string, JToken?>? eventDetails = default ) =>
        Create( logger, logLevel, eventID, message ?? e?.Message ?? string.Empty, attachments, eventDetails, e?.Details() );

    public static AppLog Create<TState>( AppLogger                        logger,
                                         LogLevel                         logLevel,
                                         EventID                          eventId,
                                         TState                           state,
                                         Exception?                       e,
                                         Func<TState, Exception?, string> formatter,
                                         IEnumerable<LoggerAttachment>?   attachments  = default,
                                         IDictionary<string, JToken?>?    eventDetails = default
    ) => Create( logger, logLevel, eventId, formatter( state, e ), attachments, eventDetails, e?.Details() );


    public ExceptionDetails? GetExceptionDetails() => Exception;

    public string? GetStackTrace() => Exception?.StackTrace is not null
                                          ? string.Join( '\n', Exception.StackTrace )
                                          : default;


    public AppLog Update( LogLevel level ) => this with
                                              {
                                                  Level = level
                                              };

    public AppLog Update( LoggingSettings settings )
    {
        ScopeIDs.Add( settings.ScopeIDs );
        return this;
    }
}
