// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  5:57 PM

using System.Collections.Immutable;



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
                                             DateTimeOffset                                                       DateCreated,
                                             DateTimeOffset?                                                      LastModified = default
) : LoggerTable<LoggerAttachmentRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<LoggerAttachmentRecord>, ILoggerAttachment, ILogInfo
{
    public static string TableName { get; } = typeof(LoggerAttachmentRecord).GetTableName();


    Guid IStartSession.AppID     => AppID.Value;
    Guid IStartSession.DeviceID  => DeviceID.Value;
    Guid ILogInfo.     LogID     => LogID.Value;
    Guid? ISessionID.  SessionID => SessionID?.Value;


    public LoggerAttachmentRecord( LoggerAttachment attachment, ILogInfo info, RecordID<ScopeRecord>? scope ) : this( attachment.Content,
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
                                                                                                                      DateTimeOffset.UtcNow )
    {
        if ( attachment.Length > LoggerAttachment.MAX_SIZE ) { LoggerAttachment.ThrowTooLong( attachment.Length ); }
    }
    public LoggerAttachmentRecord( LoggerAttachment attachment, AppRecord app, DeviceRecord device, LogRecord log, SessionRecord? session ) : this( attachment.Content,
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
                                                                                                                                                    DateTimeOffset.UtcNow )
    {
        if ( attachment.Length > LoggerAttachment.MAX_SIZE ) { LoggerAttachment.ThrowTooLong( attachment.Length ); }
    }


    public static LoggerAttachmentRecord Create( DbDataReader reader )
    {
        string content      = reader.GetString( nameof(Content) );
        string description  = reader.GetString( nameof(Description) );
        string fileName     = reader.GetString( nameof(FileName) );
        long   length       = reader.GetFieldValue<long>( nameof(LogID) );
        string type         = reader.GetString( nameof(Type) );
        bool   isBinary     = reader.GetFieldValue<bool>( nameof(Type) );
        var    appID        = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(AppID) ) );
        var    deviceID     = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(DeviceID) ) );
        var    logID        = new RecordID<LogRecord>( reader.GetFieldValue<Guid>( nameof(LogID) ) );
        var    sessionID    = new RecordID<SessionRecord>( reader.GetFieldValue<Guid>( nameof(SessionID) ) );
        var    dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var    lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var    id           = new RecordID<LoggerAttachmentRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

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
                                           dateCreated,
                                           lastModified );
    }
    public static async IAsyncEnumerable<LoggerAttachmentRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
    public static IEnumerable<LoggerAttachmentRecord> Create( AppLog log, AppRecord app, DeviceRecord device, LogRecord record, SessionRecord? session )
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( LoggerAttachment attachment in log.Attachments ) { yield return new LoggerAttachmentRecord( attachment, app, device, record, session ); }
    }
    public static ImmutableArray<LoggerAttachmentRecord> CreateArray( AppLog log, AppRecord app, DeviceRecord device, LogRecord record, SessionRecord? session ) => ImmutableArray.CreateRange( Create( log, app, device, record, session ) );


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
                                                                               IsBinary = attachment.IsBinary
                                                                           };


    public override int CompareTo( LoggerAttachmentRecord? other ) => string.CompareOrdinal( Content, other?.Content );
    public override int GetHashCode()                              => HashCode.Combine( Content, base.GetHashCode() );
}



[ Serializable, Table( "LogAttachments" ) ]
public sealed record LoggerAttachmentMappingRecord : Mapping<LoggerAttachmentMappingRecord, LogRecord, LoggerAttachmentRecord>,
                                                     ICreateMapping<LoggerAttachmentMappingRecord, LogRecord, LoggerAttachmentRecord>,
                                                     IDbReaderMapping<LoggerAttachmentMappingRecord>
{
    public static string TableName { get; } = typeof(LoggerAttachmentRecord).GetTableName();


    public LoggerAttachmentMappingRecord( LogRecord key, LoggerAttachmentRecord value ) : base( key, value ) { }
    private LoggerAttachmentMappingRecord( RecordID<LogRecord> key, RecordID<LoggerAttachmentRecord> value, RecordID<LoggerAttachmentMappingRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) :
        base( key, value, id, dateCreated, lastModified ) { }


    public static LoggerAttachmentMappingRecord Create( LogRecord key, LoggerAttachmentRecord value ) => new(key, value);
    public static LoggerAttachmentMappingRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<LogRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<LoggerAttachmentRecord>( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<LoggerAttachmentMappingRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new LoggerAttachmentMappingRecord( key, value, id, dateCreated, lastModified );
    }
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
    public static string TableName { get; } = typeof(LoggerScopeAttachmentMappingRecord).GetTableName();


    public LoggerScopeAttachmentMappingRecord( LoggerAttachmentRecord key, ScopeRecord value ) : base( key, value ) { }
    private LoggerScopeAttachmentMappingRecord( RecordID<LoggerAttachmentRecord> key, RecordID<ScopeRecord> value, RecordID<LoggerScopeAttachmentMappingRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) :
        base( key, value, id, dateCreated, lastModified ) { }


    public static LoggerScopeAttachmentMappingRecord Create( LoggerAttachmentRecord key, ScopeRecord value ) => new(key, value);
    public static LoggerScopeAttachmentMappingRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<LoggerAttachmentRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<LoggerScopeAttachmentMappingRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new LoggerScopeAttachmentMappingRecord( key, value, id, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<LoggerScopeAttachmentMappingRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
