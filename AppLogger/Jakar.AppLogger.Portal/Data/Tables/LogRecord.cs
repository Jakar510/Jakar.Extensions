// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  10:06 AM

namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Logs" )]
public sealed record LogRecord : LoggerTable<LogRecord>, IAppLog, ILogInfo
{
    public                                    DateTimeOffset AppErrorTime       { get; init; }
    public                                    Guid           AppID              { get; init; }
    public                                    DateTimeOffset AppLaunchTimestamp { get; init; }
    public                                    DateTimeOffset AppStartTime       { get; init; }
    [MaxLength( 1024 )] public                string?        AppUserID          { get; init; }
    [MaxLength( 1024 )] public                string?        BuildID            { get; init; }
    public                                    string?        CategoryName       { get; init; }
    public                                    Guid           DeviceID           { get; init; }
    public                                    int            EventID            { get; init; }
    [MaxLength( 1024 )]                public string?        EventName          { get; init; }
    [MaxLength( Attachment.MAX_SIZE )] public string?        ExceptionDetails   { get; init; }
    public                                    bool           IsError            { get; init; }
    public                                    bool           IsFatal            { get; init; }
    public                                    bool           IsValid            { get; init; }
    public                                    LogLevel       Level              { get; init; }
    Guid ILogInfo.                                           LogID              => ID;
    [MaxLength( Attachment.MAX_SIZE )] public string         Message            { get; init; } = string.Empty;
    public                                    Guid?          ScopeID            { get; init; }
    public                                    Guid           SessionID          { get; init; }
    [MaxLength( Attachment.MAX_SIZE )] public string?        StackTrace         { get; init; }
    public                                    DateTimeOffset Timestamp          { get; init; }


    public LogRecord() : base() { }
    public LogRecord( AppLog log, SessionRecord session, UserRecord? caller = default ) : base( log.ID, caller )
    {
        IsValid            = log.IsValid;
        IsError            = log.IsError;
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
        StackTrace         = log.GetStackTrace();
        EventID            = log.EventID;
        EventName          = log.EventName;
        BuildID            = log.BuildID;
        AdditionalData     = log.AdditionalData?.ToPrettyJson();
        ExceptionDetails   = log.Exception?.ToPrettyJson();
        AppID              = session.AppID;
        DeviceID           = session.DeviceID;
    }


    public static DynamicParameters GetDynamicParameters( AppLog log )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Message),            log.Message );
        parameters.Add( nameof(Timestamp),          log.Timestamp );
        parameters.Add( nameof(AppLaunchTimestamp), log.AppLaunchTimestamp );
        parameters.Add( nameof(AppErrorTime),       log.AppErrorTime );
        parameters.Add( nameof(Level),              log.Level );
        parameters.Add( nameof(EventID),            log.EventID );
        parameters.Add( nameof(EventName),          log.EventName );
        parameters.Add( nameof(SessionID),          log.SessionID );
        parameters.Add( nameof(ScopeID),            log.ScopeID );
        return parameters;
    }

    public async ValueTask<AppLog> ToLog( DbConnection connection, DbTransaction transaction, LoggerDB db, CancellationToken token = default )
    {
        AttachmentRecord[] records = await db.Attachments.Where( connection, transaction, true, AttachmentRecord.GetDynamicParameters( this ), token );
        DeviceRecord       device  = await db.Devices.Get( connection, transaction, DeviceID, token ) ?? throw new RecordNotFoundException( DeviceID.ToString() );
        ExceptionDetails?  details = GetExceptionDetails();

        return new AppLog( this, records.Select( x => x.ToAttachment() ), device.ToDeviceDescriptor(), details );
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
