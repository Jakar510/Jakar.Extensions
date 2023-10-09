// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:59 PM


using System.Collections.Immutable;



namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Scopes" ) ]
public sealed record ScopeRecord( RecordID<AppRecord>     AppID,
                                  RecordID<DeviceRecord>  DeviceID,
                                  RecordID<SessionRecord> SessionID,
                                  RecordID<ScopeRecord>   ID,
                                  DateTimeOffset          DateCreated,
                                  DateTimeOffset?         LastModified = default
) : LoggerTable<ScopeRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<ScopeRecord>
{
    public static string TableName { get; } = typeof(ScopeRecord).GetTableName();


    private ScopeRecord( Guid scopeID, AppRecord app, DeviceRecord device, SessionRecord session ) : this( app.ID, device.ID, session.ID, new RecordID<ScopeRecord>( scopeID ), DateTimeOffset.UtcNow ) { }


    public static ScopeRecord Create( DbDataReader reader )
    {
        var appID        = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(AppID) ) );
        var deviceID     = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(DeviceID) ) );
        var sessionID    = new RecordID<SessionRecord>( reader.GetFieldValue<Guid>( nameof(SessionID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new ScopeRecord( appID, deviceID, sessionID, id, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<ScopeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
    public static IEnumerable<ScopeRecord> Create( AppLog log, AppRecord app, DeviceRecord device, SessionRecord session )
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( Guid scopeID in log.ScopeIDs ) { yield return new ScopeRecord( scopeID, app, device, session ); }
    }
    public static ImmutableArray<ScopeRecord> CreateArray( AppLog log, AppRecord app, DeviceRecord device, SessionRecord session ) => ImmutableArray.CreateRange( Create( log, app, device, session ) );


    public override int CompareTo( ScopeRecord? other ) => Nullable.Compare( AppID, other?.AppID );
}



[ Serializable, Table( "LogScopes" ) ]
public sealed record LogScopeRecord : Mapping<LogScopeRecord, LogRecord, ScopeRecord>, ICreateMapping<LogScopeRecord, LogRecord, ScopeRecord>, IDbReaderMapping<LogScopeRecord>
{
    public static string TableName { get; } = typeof(LogScopeRecord).GetTableName();


    public LogScopeRecord( LogRecord            key, ScopeRecord           value ) : base( key, value ) { }
    private LogScopeRecord( RecordID<LogRecord> key, RecordID<ScopeRecord> value, RecordID<LogScopeRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    public static LogScopeRecord Create( LogRecord key, ScopeRecord value ) => new(key, value);
    public static LogScopeRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<LogRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<ScopeRecord>( reader.GetFieldValue<Guid>( nameof(ValueID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<LogScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new LogScopeRecord( key, value, id, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<LogScopeRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
