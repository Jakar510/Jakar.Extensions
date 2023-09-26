// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  5:57 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Attachments" ) ]
public sealed record LoggerAttachmentRecord( [ property: MaxLength( LoggerAttachment.MAX_SIZE ) ]         string  Content,
                                             [ property: MaxLength( LoggerAttachment.DESCRIPTION_SIZE ) ] string? Description,
                                             [ property: MaxLength( LoggerAttachment.FILE_NAME_SIZE ) ]   string? FileName,
                                             [ property: Range( 0, LoggerAttachment.MAX_SIZE ) ]          long    Length,
                                             [ property: MaxLength( LoggerAttachment.TYPE_SIZE ) ]        string? Type,
                                             bool                                                                 IsBinary,
                                             RecordID<AppRecord>                                                  AppID,
                                             RecordID<DeviceRecord>                                               DeviceID,
                                             RecordID<LogRecord>                                                  LogID,
                                             RecordID<SessionRecord>?                                             SessionID,
                                             RecordID<LoggerAttachmentRecord>                                     ID,
                                             RecordID<UserRecord>?                                                CreatedBy,
                                             Guid?                                                                OwnerUserID,
                                             DateTimeOffset                                                       DateCreated,
                                             DateTimeOffset?                                                      LastModified = default
) : LoggerTable<LoggerAttachmentRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<LoggerAttachmentRecord>, ILoggerAttachment, ILogInfo
{
    Guid IStartSession.AppID     => AppID.Value;
    Guid IStartSession.DeviceID  => DeviceID.Value;
    Guid? ISessionID.  SessionID => SessionID?.Value;
    Guid ILogInfo.     LogID     => LogID.Value;


    public LoggerAttachmentRecord( LoggerAttachment attachment, ILogInfo info, RecordID<ScopeRecord>? scope, UserRecord? caller = default ) : this( attachment.Content,
                                                                                                                                                    attachment.Description,
                                                                                                                                                    attachment.FileName,
                                                                                                                                                    attachment.Length,
                                                                                                                                                    attachment.Type,
                                                                                                                                                    attachment.IsBinary,
                                                                                                                                                    RecordID<AppRecord>.New( info.AppID ),
                                                                                                                                                    RecordID<DeviceRecord>.New( info.DeviceID ),
                                                                                                                                                    RecordID<LogRecord>.New( info.LogID ),
                                                                                                                                                    info.SessionID.HasValue
                                                                                                                                                        ? RecordID<SessionRecord>.New( info.SessionID.Value )
                                                                                                                                                        : default,
                                                                                                                                                    RecordID<LoggerAttachmentRecord>.New(),
                                                                                                                                                    caller?.ID,
                                                                                                                                                    caller?.UserID,
                                                                                                                                                    DateTimeOffset.UtcNow )
    {
        if ( attachment.Length > LoggerAttachment.MAX_SIZE ) { LoggerAttachment.ThrowTooLong( attachment.Length ); }
    }
    public LoggerAttachmentRecord( LoggerAttachment attachment, AppRecord app, DeviceRecord device, LogRecord log, SessionRecord? session, UserRecord? caller = default ) : this( attachment.Content,
                                                                                                                                                                                  attachment.Description,
                                                                                                                                                                                  attachment.FileName,
                                                                                                                                                                                  attachment.Length,
                                                                                                                                                                                  attachment.Type,
                                                                                                                                                                                  attachment.IsBinary,
                                                                                                                                                                                  app.ID,
                                                                                                                                                                                  device.ID,
                                                                                                                                                                                  log.ID,
                                                                                                                                                                                  session?.ID,
                                                                                                                                                                                  RecordID<LoggerAttachmentRecord>.New(),
                                                                                                                                                                                  caller?.ID,
                                                                                                                                                                                  caller?.UserID,
                                                                                                                                                                                  DateTimeOffset.UtcNow )
    {
        if ( attachment.Length > LoggerAttachment.MAX_SIZE ) { LoggerAttachment.ThrowTooLong( attachment.Length ); }
    }


    public static LoggerAttachmentRecord Create( DbDataReader reader )
    {
        var content      = reader.GetString( nameof(Content) );
        var description  = reader.GetString( nameof(Description) );
        var fileName     = reader.GetString( nameof(FileName) );
        var length       = reader.GetFieldValue<long>( nameof(LogID) );
        var type         = reader.GetString( nameof(Type) );
        var isBinary     = reader.GetFieldValue<bool>( nameof(Type) );
        var appID        = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(AppID) ) );
        var deviceID     = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(DeviceID) ) );
        var logID        = new RecordID<LogRecord>( reader.GetFieldValue<Guid>( nameof(LogID) ) );
        var sessionID    = new RecordID<SessionRecord>( reader.GetFieldValue<Guid>( nameof(SessionID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id           = new RecordID<LoggerAttachmentRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

        return new LoggerAttachmentRecord( content,
                                           description,
                                           fileName,
                                           length,
                                           type,
                                           isBinary,
                                           appID,
                                           deviceID,
                                           logID,
                                           sessionID,
                                           id,
                                           createdBy,
                                           ownerUserID,
                                           dateCreated,
                                           lastModified );
    }
    public static async IAsyncEnumerable<LoggerAttachmentRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    public static IEnumerable<LoggerAttachmentRecord> Create( AppLog log, AppRecord app, DeviceRecord device, LogRecord record, SessionRecord? session, UserRecord caller )
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( LoggerAttachment attachment in log.Attachments ) { yield return new LoggerAttachmentRecord( attachment, app, device, record, session, caller ); }
    }


    public static DynamicParameters GetDynamicParameters( LoggerAttachment attachment )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Content), attachment.Content );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( ILogInfo info )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(LogID),     info.LogID );
        parameters.Add( nameof(AppID),     info.AppID );
        parameters.Add( nameof(SessionID), info.SessionID );
        return parameters;
    }


    public byte[]? GetData() => IsBinary
                                    ? Convert.FromBase64String( Content )
                                    : default;

    public LoggerAttachment ToLoggerAttachment() => new(this);
    public LoggerAttachmentRecord Update( LoggerAttachment attachment ) => this with
                                                                           {
                                                                               Length = attachment.Length,
                                                                               Content = attachment.Content,
                                                                               Description = attachment.Description,
                                                                               Type = attachment.Type,
                                                                               FileName = attachment.FileName,
                                                                               IsBinary = attachment.IsBinary,
                                                                           };


    public override int CompareTo( LoggerAttachmentRecord? other ) => string.CompareOrdinal( Content, other?.Content );
    public override int GetHashCode()                              => HashCode.Combine( Content, base.GetHashCode() );
}



[ Serializable, Table( "LogAttachments" ) ]
public sealed record LoggerAttachmentMappingRecord : Mapping<LoggerAttachmentMappingRecord, LogRecord, LoggerAttachmentRecord>,
                                                     ICreateMapping<LoggerAttachmentMappingRecord, LogRecord, LoggerAttachmentRecord>,
                                                     IDbReaderMapping<LoggerAttachmentMappingRecord>
{
    public LoggerAttachmentMappingRecord( LogRecord               key, LoggerAttachmentRecord value, UserRecord? caller = default ) : base( key, value, caller ) { }
    public static LoggerAttachmentMappingRecord Create( LogRecord key, LoggerAttachmentRecord value, UserRecord? caller = default ) => new(key, value, caller);


    public static LoggerAttachmentMappingRecord Create( DbDataReader reader ) => null;
    public static async IAsyncEnumerable<LoggerAttachmentMappingRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}



[ Serializable, Table( "LogScopes" ) ]
public sealed record LoggerScopeAttachmentMappingRecord : Mapping<LoggerScopeAttachmentMappingRecord, LoggerAttachmentRecord, ScopeRecord>,
                                                          ICreateMapping<LoggerScopeAttachmentMappingRecord, LoggerAttachmentRecord, ScopeRecord>,
                                                          IDbReaderMapping<LoggerScopeAttachmentMappingRecord>
{
    public LoggerScopeAttachmentMappingRecord( LoggerAttachmentRecord key, ScopeRecord value, UserRecord? caller = default ) : base( key, value, caller ) { }
    private LoggerScopeAttachmentMappingRecord( RecordID<LoggerAttachmentRecord>             key,
                                                RecordID<ScopeRecord>                        value,
                                                RecordID<LoggerScopeAttachmentMappingRecord> id,
                                                RecordID<UserRecord>                         createdBy,
                                                Guid                                         ownerUserID,
                                                DateTimeOffset                               dateCreated,
                                                DateTimeOffset?                              lastModified
    ) : base( key, value, id, createdBy, ownerUserID, dateCreated, lastModified ) { }


    public static LoggerScopeAttachmentMappingRecord Create( LoggerAttachmentRecord key, ScopeRecord value, UserRecord? caller = default ) => new(key, value, caller);
    public static LoggerScopeAttachmentMappingRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<LoggerAttachmentRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var value        = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id           = new RecordID<LoggerScopeAttachmentMappingRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new LoggerScopeAttachmentMappingRecord( key, value, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<LoggerScopeAttachmentMappingRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
