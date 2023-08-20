// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:55 AM

using System.ComponentModel.DataAnnotations;



namespace Jakar.AppLogger.Common;


[Serializable]
public sealed record AppLog( [property: MaxLength( IAppLog.MESSAGE_LENGTH )] string Message, LogLevel Level, EventID EventID, DateTimeOffset TimeStamp, StartSessionReply Session, Guid? ScopeID ) : BaseJsonModelRecord, IAppLog, ILogDetails
{
    [MaxLength( IAppLog.APP_USER_ID_LENGTH )] public   string?             AppUserID    { get; init; }
    public                                             HashSet<Attachment> Attachments  { get; init; } = new();
    [MaxLength( IAppLog.BUILD_ID_LENGTH )]      public string?             BuildID      { get; init; }
    [MaxLength( IAppLog.CATEGORY_NAME_LENGTH )] public string?             CategoryName { get; init; }
    public                                             DeviceDescriptor?   Device       { get; init; }
    public                                             long                DeviceID     { get; init; }
    public                                             ExceptionDetails?   Exception    { get; init; }
    public                                             Guid                ID           { get; init; }
    [JsonIgnore] public                                bool                IsError      => Level > LogLevel.Error;
    public                                             bool                IsFatal      { get; init; }
    public                                             bool                IsValid      => !string.IsNullOrWhiteSpace( Message );
    string? IAppLog.                                                       StackTrace   => GetStackTrace();
    public int                                                             ThreadID     { get; init; } = Environment.CurrentManagedThreadId;


    public AppLog( AppLog log ) : base( log )
    {
        ID           = log.ID;
        Message      = log.Message;
        Level        = log.Level;
        TimeStamp    = log.TimeStamp;
        Session      = log.Session;
        ScopeID      = log.ScopeID;
        AppUserID    = log.AppUserID;
        EventID      = log.EventID;
        BuildID      = log.BuildID;
        IsFatal      = log.IsFatal;
        Exception    = log.Exception;
        Device       = log.Device;
        CategoryName = log.CategoryName;
        Attachments  = new HashSet<Attachment>( log.Attachments );
    }
    public AppLog( IAppLog log, IEnumerable<Attachment> attachments, DeviceDescriptor device, ExceptionDetails? details ) : this( log.Message, log.Level, log.EventID, log.TimeStamp, log.Session, log.ScopeID )
    {
        ID           = log.ID;
        Message      = log.Message;
        Level        = log.Level;
        TimeStamp    = log.TimeStamp;
        Session      = log.Session;
        ScopeID      = log.ScopeID;
        AppUserID    = log.AppUserID;
        EventID      = log.EventID;
        BuildID      = log.BuildID;
        IsFatal      = log.IsFatal;
        Device       = device;
        CategoryName = log.CategoryName;
        Exception    = details;
        Attachments  = new HashSet<Attachment>( attachments );
    }
    public AppLog( AppLogger logger, LogLevel level, EventID eventID, string message, IEnumerable<Attachment>? attachments = default, IDictionary<string, JToken?>? eventDetails = default ) : this( message,
                                                                                                                                                                                                     level,
                                                                                                                                                                                                     eventID,
                                                                                                                                                                                                     DateTimeOffset.UtcNow,
                                                                                                                                                                                                     logger.Config.Session,
                                                                                                                                                                                                     logger.Config.ScopeID )
    {
        AppUserID      = logger.Config.AppUserID;
        Session        = logger.Config.Session;
        ScopeID        = logger.Config.ScopeID;
        Device         = logger.Config.Device;
        CategoryName   = logger.CategoryName;
        EventID        = eventID;
        Level          = level;
        ID             = Guid.NewGuid();
        Message        = message;
        AdditionalData = eventDetails;
        if ( attachments is not null ) { Attachments.Add( attachments ); }
    }


    public static AppLog Create( AppLogger logger, LogLevel logLevel, EventID eventID, string? message, Exception? e, IEnumerable<Attachment>? attachments = default, IDictionary<string, JToken?>? eventDetails = default )
    {
        var log = new AppLog( logger, logLevel, eventID, message ?? e?.Message ?? string.Empty, attachments, eventDetails )
                  {
                      Exception = e?.Details()
                  };

        return log;
    }
    public static AppLog Create<TState>( AppLogger                        logger,
                                         LogLevel                         logLevel,
                                         EventID                          eventId,
                                         TState                           state,
                                         Exception?                       e,
                                         Func<TState, Exception?, string> formatter,
                                         IEnumerable<Attachment>?         attachments  = default,
                                         IDictionary<string, JToken?>?    eventDetails = default
    )
    {
        var log = new AppLog( logger, logLevel, eventId, formatter( state, e ), attachments, eventDetails )
                  {
                      Exception = e?.Details()
                  };

        return log;
    }


    public ExceptionDetails? GetExceptionDetails() => Exception;
    public string? GetStackTrace() => Exception?.StackTrace is not null
                                          ? string.Join( '\n', Exception.StackTrace )
                                          : default;


    public AppLog Update( LogLevel level ) => this with
                                              {
                                                  Level = level,
                                              };
    public AppLog Update( IScopeID scope ) => this with
                                              {
                                                  ScopeID = scope.ScopeID,
                                              };
}
