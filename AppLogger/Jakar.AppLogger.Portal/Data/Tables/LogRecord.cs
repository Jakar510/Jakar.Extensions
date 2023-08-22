// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  10:06 AM


using Debug = Jakar.AppLogger.Common.Debug;



namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Logs" )]
public sealed record LogRecord : LoggerTable<LogRecord>, IAppLog, ILogInfo, IAppStartTime
{
    public                                           DateTimeOffset          TimeStamp        { get; init; }
    public                                           RecordID<AppRecord>     AppID            { get; init; }
    public                                           DateTimeOffset          AppStartTime     { get; init; }
    [MaxLength( IAppLog.APP_USER_ID_LENGTH )] public string?                 AppUserID        { get; init; }
    [MaxLength( IAppLog.BUILD_ID_LENGTH )]    public string?                 BuildID          { get; init; }
    public                                           string?                 CategoryName     { get; init; }
    public                                           RecordID<DeviceRecord>  DeviceID         { get; init; }
    public                                           int                     EventID          { get; init; }
    [MaxLength( IAppLog.EVENT_NAME_LENGTH )] public  string?                 EventName        { get; init; }
    [MaxLength( MAX_STRING_SIZE )]           public  string?                 ExceptionDetails { get; init; }
    public                                           bool                    IsError          { get; init; }
    public                                           bool                    IsFatal          { get; init; }
    public                                           bool                    IsValid          { get; init; }
    public                                           LogLevel                Level            { get; init; }
    [MaxLength( MAX_STRING_SIZE )] public            string                  Message          { get; init; } = string.Empty;
    public                                           RecordID<SessionRecord> SessionID        { get; init; }
    [MaxLength( MAX_STRING_SIZE )] public            string?                 StackTrace       { get; init; }
    public                                           DateTimeOffset          Timestamp        { get; init; }


    StartSessionReply IAppLog.Session   => new(SessionID.Value, DeviceID.Value, AppID.Value);
    EventID IAppLog.          EventID   => new(EventID, EventName);
    Guid? ISessionID.         SessionID => SessionID.Value;
    Guid ILogInfo.            LogID     => ID.Value;
    Guid IStartSession.       DeviceID  => DeviceID.Value;
    Guid IStartSession.       AppID     => AppID.Value;


    public LogRecord() : base() { }
    public LogRecord( AppLog log, SessionRecord session, UserRecord? caller = default ) : base( caller )
    {
        System.Diagnostics.Debug.Assert( session.ID.Value == log.Session.SessionID );

        IsValid          = log.IsValid;
        IsError          = log.IsError;
        IsFatal          = log.IsFatal;
        Message          = log.Message;
        Level            = log.Level;
        Timestamp        = log.TimeStamp;
        AppStartTime     = session.DateCreated;
        SessionID        = session.ID;
        AppUserID        = log.AppUserID;
        StackTrace       = log.GetStackTrace();
        EventID          = log.EventID.ID;
        EventName        = log.EventID.Name;
        BuildID          = log.BuildID;
        AdditionalData   = log.AdditionalData?.ToPrettyJson();
        ExceptionDetails = log.Exception?.ToPrettyJson();
        AppID            = session.AppID;
        DeviceID         = session.DeviceID;
    }


    public static DynamicParameters GetDynamicParameters( AppLog log )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Message),   log.Message );
        parameters.Add( nameof(Timestamp), log.TimeStamp );
        parameters.Add( nameof(Level),     log.Level );
        parameters.Add( nameof(EventID),   log.EventID );
        parameters.Add( nameof(EventName), log.EventID.Name );
        parameters.Add( nameof(SessionID), log.Session.SessionID );
        parameters.Add( nameof(DeviceID),  log.Session.DeviceID );
        parameters.Add( nameof(AppID),     log.Session.AppID );
        return parameters;
    }


    public async ValueTask<AppLog> ToLog( DbConnection connection, DbTransaction transaction, LoggerDB db, CancellationToken token = default )
    {
        IEnumerable<LoggerAttachmentRecord> records = await db.Attachments.Where( connection, transaction, true, LoggerAttachmentRecord.GetDynamicParameters( this ), token );
        DeviceRecord                        device  = await db.Devices.Get( connection, transaction, DeviceID, token ) ?? throw new RecordNotFoundException( DeviceID.ToString() );
        ExceptionDetails?                   details = GetExceptionDetails();

        IEnumerable<Guid> scopeIDs = Array.Empty<Guid>();

        return new AppLog( this, records.Select( x => x.ToLoggerAttachment() ), scopeIDs, device.ToDeviceDescriptor(), details );
    }


    public ExceptionDetails? GetExceptionDetails() => ExceptionDetails?.FromJson<ExceptionDetails>();
    public string? GetStackTrace() => StackTrace;


    public override int CompareTo( LogRecord? other ) => string.CompareOrdinal( Message, other?.Message );
    public override int GetHashCode() => HashCode.Combine( Message, base.GetHashCode() );
    public override bool Equals( LogRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( Message, other.Message, StringComparison.Ordinal );
    }
}
