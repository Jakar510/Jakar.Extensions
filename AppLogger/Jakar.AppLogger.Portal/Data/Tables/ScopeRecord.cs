// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:59 PM


namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Scopes" ) ]
public sealed record ScopeRecord( RecordID<AppRecord>     AppID,
                                  RecordID<DeviceRecord>  DeviceID,
                                  RecordID<SessionRecord> SessionID,
                                  RecordID<ScopeRecord>   ID,
                                  RecordID<UserRecord>?   CreatedBy,
                                  Guid?                   OwnerUserID,
                                  DateTimeOffset          DateCreated,
                                  DateTimeOffset?         LastModified = default
) : LoggerTable<ScopeRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<ScopeRecord>
{
    private ScopeRecord( Guid scopeID, AppRecord app, DeviceRecord device, SessionRecord session, UserRecord? caller = default ) : this( app.ID,
                                                                                                                                         device.ID,
                                                                                                                                         session.ID,
                                                                                                                                         new RecordID<ScopeRecord>( scopeID ),
                                                                                                                                         caller?.ID,
                                                                                                                                         caller?.UserID,
                                                                                                                                         DateTimeOffset.UtcNow ) { }


    public static ScopeRecord Create( DbDataReader reader )
    {
        var                   appID        = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var                   deviceID     = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var                   sessionID    = new RecordID<SessionRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        DateTimeOffset        dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?       lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        Guid                  ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>  createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<ScopeRecord> id           = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new ScopeRecord( appID, deviceID, sessionID, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<ScopeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static IEnumerable<ScopeRecord> Create( AppLog log, AppRecord app, DeviceRecord device, SessionRecord session, UserRecord? caller = default )
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( Guid scopeID in log.ScopeIDs ) { yield return new ScopeRecord( scopeID, app, device, session, caller ); }
    }


    public override int CompareTo( ScopeRecord? other ) => Nullable.Compare( AppID, other?.AppID );
}



[ Serializable, Table( "LogScopes" ) ]
public sealed record LogScopeRecord : Mapping<LogScopeRecord, LogRecord, ScopeRecord>, ICreateMapping<LogScopeRecord, LogRecord, ScopeRecord>, IDbReaderMapping<LogScopeRecord>
{
    public LogScopeRecord( LogRecord key, ScopeRecord value, UserRecord? caller = default ) : base( key, value, caller ) { }
    private LogScopeRecord( RecordID<LogRecord> key, RecordID<ScopeRecord> value, RecordID<LogScopeRecord> id, RecordID<UserRecord> createdBy, Guid ownerUserID, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) :
        base( key, value, id, createdBy, ownerUserID, dateCreated, lastModified ) { }


    public static LogScopeRecord Create( LogRecord key, ScopeRecord value, UserRecord? caller = default ) => new(key, value, caller);
    public static LogScopeRecord Create( DbDataReader reader )
    {
        var                      key          = new RecordID<LogRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var                      value        = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?          lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        Guid                     ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>     createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<LogScopeRecord> id           = new RecordID<LogScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new LogScopeRecord( key, value, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<LogScopeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
