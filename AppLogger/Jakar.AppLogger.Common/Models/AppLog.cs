// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:55 AM

using System.ComponentModel.DataAnnotations;



namespace Jakar.AppLogger.Common;


[Serializable]
public sealed record AppLog : BaseJsonModelRecord, IAppLog, ILogDetails
{
    public                                          DateTimeOffset      AppErrorTime       { get; init; }
    public                                          long                AppID              { get; init; }
    public                                          DateTimeOffset      AppLaunchTimestamp { get; init; }
    public                                          DateTimeOffset      AppStartTime       { get; init; }
    [MaxLength( IAppLog.APP_USER_ID_LENGTH )] public   string?             AppUserID          { get; init; }
    public                                          HashSet<Attachment> Attachments        { get; init; } = new();
    [MaxLength( IAppLog.BUILD_ID_LENGTH )]      public string?             BuildID            { get; init; }
    [MaxLength( IAppLog.CATEGORY_NAME_LENGTH )] public string?             CategoryName       { get; init; }
    public                                          DeviceDescriptor?   Device             { get; init; }
    public                                          long                DeviceID           { get; init; }
    public                                          int                 EventID            { get; init; }
    [MaxLength( IAppLog.EVENT_NAME_LENGTH )] public    string?             EventName          { get; init; }
    public                                          ExceptionDetails?   Exception          { get; init; }
    public                                          Guid                ID                 { get; init; }
    public                                          bool                IsError            => Level > LogLevel.Error;
    public                                          bool                IsFatal            { get; init; }
    public                                          bool                IsValid            => !string.IsNullOrWhiteSpace( Message );
    public                                          LogLevel            Level              { get; init; }
    [MaxLength( IAppLog.MESSAGE_LENGTH )] public       string              Message            { get; init; } = string.Empty;
    public                                          Guid?               ScopeID            { get; init; }
    public                                          Guid                SessionID          { get; init; }
    string? IAppLog.                                                       StackTrace         => GetStackTrace();
    public int                                                          ThreadID           { get; init; } = Environment.CurrentManagedThreadId;
    public DateTimeOffset                                               Timestamp          { get; init; }


    public AppLog() { }
    public AppLog( AppLog log ) : base( log )
    {
        ID                 = log.ID;
        Message            = log.Message;
        Level              = log.Level;
        Timestamp          = log.Timestamp;
        AppLaunchTimestamp = log.AppLaunchTimestamp;
        SessionID          = log.SessionID;
        ScopeID            = log.ScopeID;
        AppUserID          = log.AppUserID;
        AppStartTime       = log.AppStartTime;
        AppErrorTime       = log.AppErrorTime;
        EventID            = log.EventID;
        EventName          = log.EventName;
        BuildID            = log.BuildID;
        IsFatal            = log.IsFatal;
        Exception          = log.Exception;
        Device             = log.Device;
        CategoryName       = log.CategoryName;
        Attachments        = new HashSet<Attachment>( log.Attachments );
    }
    public AppLog( IAppLog log, IEnumerable<Attachment> attachments, DeviceDescriptor device, ExceptionDetails? details )
    {
        ID                 = log.ID;
        Message            = log.Message;
        Level              = log.Level;
        Timestamp          = log.Timestamp;
        AppLaunchTimestamp = log.AppLaunchTimestamp;
        SessionID          = log.SessionID;
        ScopeID            = log.ScopeID;
        AppUserID          = log.AppUserID;
        AppStartTime       = log.AppStartTime;
        AppErrorTime       = log.AppErrorTime;
        EventID            = log.EventID;
        EventName          = log.EventName;
        BuildID            = log.BuildID;
        IsFatal            = log.IsFatal;
        Device             = device;
        CategoryName       = log.CategoryName;
        Exception          = details;
        Attachments        = new HashSet<Attachment>( attachments );
    }
    public AppLog( AppLogger logger, LogLevel level, EventId eventId, string message, IEnumerable<Attachment>? attachments = default, IDictionary<string, JToken?>? eventDetails = default )
    {
        AppLaunchTimestamp = logger.Config.AppLaunchTimestamp;
        AppUserID          = logger.Config.AppUserID;
        SessionID          = logger.Config.SessionID;
        ScopeID            = logger.Config.ScopeID;
        Device             = logger.Config.Device;
        CategoryName       = logger.CategoryName;
        EventID            = eventId.Id;
        EventName          = eventId.Name;
        Level              = level;
        Timestamp          = DateTimeOffset.UtcNow;
        ID                 = Guid.NewGuid();
        Message            = message;
        AdditionalData     = eventDetails;
        if ( attachments is not null ) { Attachments.Add( attachments ); }
    }


    public static AppLog Create( AppLogger logger, LogLevel logLevel, EventId eventId, string? message, Exception? e, IEnumerable<Attachment>? attachments = default, IDictionary<string, JToken?>? eventDetails = default )
    {
        var log = new AppLog( logger, logLevel, eventId, message ?? e?.Message ?? string.Empty, attachments, eventDetails )
                  {
                      Exception = e?.Details()
                  };

        return log;
    }
    public static AppLog Create<TState>( AppLogger                        logger,
                                         LogLevel                         logLevel,
                                         EventId                          eventId,
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
