// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:59 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Scopes" ) ]
public sealed record ScopeRecord : LoggerTable<ScopeRecord>, IDbReaderMapping<ScopeRecord>
{
    public RecordID<AppRecord>     AppID     { get; init; }
    public RecordID<DeviceRecord>  DeviceID  { get; init; }
    public RecordID<SessionRecord> SessionID { get; init; }


    private ScopeRecord( Guid scopeID, AppRecord app, DeviceRecord device, SessionRecord session, UserRecord? caller = default ) : base( new RecordID<ScopeRecord>( scopeID ), caller )
    {
        AppID     = app.ID;
        DeviceID  = device.ID;
        SessionID = session.ID;
    }
    public static ScopeRecord Create( DbDataReader reader )
    {
        DateTimeOffset        dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset        lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                  ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>  createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<ScopeRecord> id           = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new ScopeRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
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
    public LogScopeRecord( LogRecord key, ScopeRecord value ) : base( key, value ) { }


    public static LogScopeRecord Create( LogRecord key, ScopeRecord value ) => new(key, value);
    public static LogScopeRecord Create( DbDataReader reader )
    {
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset           lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                     ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>     createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<LogScopeRecord> id           = new RecordID<LogScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new LogScopeRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<LogScopeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
