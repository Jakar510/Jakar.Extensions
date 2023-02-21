// Jakar.Extensions :: Jakar.Extensions.AppCenter
// 07/06/2022  8:55 AM

using System.ComponentModel.DataAnnotations;



namespace Jakar.AppLogger.Common;


[Serializable]
public sealed record Log : BaseJsonModelRecord, ILog, ILogDetails
{
    public                             bool                IsError            => Level > LogLevel.Error;
    public                             bool                IsFatal            { get; init; }
    public                             bool                IsValid            => !string.IsNullOrWhiteSpace( Message );
    public                             DateTimeOffset      AppErrorTime       { get; init; }
    public                             DateTimeOffset      AppLaunchTimestamp { get; init; }
    public                             DateTimeOffset      AppStartTime       { get; init; }
    public                             DateTimeOffset      Timestamp          { get; init; }
    public                             DeviceDescriptor?   Device             { get; init; }
    public                             ExceptionDetails?   Exception          { get; init; }
    public                             Guid                SessionID          { get; init; }
    public                             Guid?               ScopeID            { get; init; }
    public                             HashSet<Attachment> Attachments        { get; init; } = new();
    public                             int                 EventID            { get; init; }
    public                             int                 ThreadID           { get; init; } = Environment.CurrentManagedThreadId;
    public                             LogLevel            Level              { get; init; }
    public                             long                AppID              { get; init; }
    public                             long                DeviceID           { get; init; }
    public                             Guid                ID                 { get; init; }
    [MaxLength( int.MaxValue )] public string              Message            { get; init; } = string.Empty;
    [MaxLength( 1024 )]         public string?             AppUserID          { get; init; }
    [MaxLength( 1024 )]         public string?             BuildID            { get; init; }
    [MaxLength( 1024 )]         public string?             EventName          { get; init; }


    // public ErrorDetails.Collection? Details { get; init; }


    [JsonIgnore]
    public string? StackTrace => Exception?.StackTrace is not null
                                     ? string.Join( '\n', Exception.StackTrace )
                                     : default;


    public Log() { }
    public Log( Log log ) : base( log )
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
        Attachments        = new HashSet<Attachment>( log.Attachments );

        // if ( log.Details is not null ) { Details = new ErrorDetails.Collection(log.Details); }
    }
    public Log( ILog log, IEnumerable<Attachment> attachments, ExceptionDetails? details = default )
    {
        ID                 = log.ID;
        IsFatal            = log.IsFatal;
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
        Exception          = details;
        Attachments        = new HashSet<Attachment>( attachments );
    }


    public Log( LoggingSettings value, LogLevel level )
    {
        AppLaunchTimestamp = value.AppLaunchTimestamp;
        AppUserID          = value.AppUserID;
        SessionID          = value.SessionID;
        ScopeID            = value.ScopeID;
        Level              = level;
        ID                 = Guid.NewGuid();
    }
    public Log( LoggingSettings value, LogLevel level, IEnumerable<Attachment> attachments ) : this( value, level ) => Attachments = new HashSet<Attachment>( attachments );
    public Log( LoggingSettings config, IEnumerable<Attachment> attachments, Exception e, LogLevel level = LogLevel.Error ) : this( config, level, attachments )
    {
        AppLaunchTimestamp = config.AppLaunchTimestamp;
        Device             = config.Device;
        EventID            = e.HResult;
        EventName          = e.Source;
        Message            = e.Message;
        Level              = LogLevel.Error;
        Exception          = e.Details();
    }
    public Log( LoggingSettings config, Exception? e, EventId eventId, string message, LogLevel level ) : this( config, level )
    {
        AppLaunchTimestamp = config.AppLaunchTimestamp;
        Device             = config.Device;
        EventID            = eventId.Id;
        EventName          = eventId.Name;
        Message            = message;
        Level              = level;
        Exception          = e?.Details();
    }
    public Log( LoggingSettings config, IEnumerable<Attachment> attachments, Exception? e, EventId eventId, string message, LogLevel level ) : this( config, level, attachments )
    {
        AppLaunchTimestamp = config.AppLaunchTimestamp;
        Device             = config.Device;
        EventID            = eventId.Id;
        EventName          = eventId.Name;
        Message            = message;
        Level              = level;
        Exception          = e?.Details();
    }


    public Log Update( LoggingSettings settings ) => this with
                                                     {
                                                         AppLaunchTimestamp = settings.AppLaunchTimestamp,
                                                         AppUserID = settings.AppUserID,
                                                         SessionID = settings.SessionID,
                                                         ScopeID = settings.ScopeID
                                                     };
    public Log Update( LoggingSettings settings, LogLevel level ) => this with
                                                                     {
                                                                         AppLaunchTimestamp = settings.AppLaunchTimestamp,
                                                                         AppUserID = settings.AppUserID,
                                                                         SessionID = settings.SessionID,
                                                                         ScopeID = settings.ScopeID,
                                                                         Level = level
                                                                     };
    public Log Update( LogLevel level ) => this with
                                           {
                                               Level = level
                                           };
    public Log Update( IScopeID scope ) => this with
                                           {
                                               ScopeID = scope.ScopeID
                                           };
}
