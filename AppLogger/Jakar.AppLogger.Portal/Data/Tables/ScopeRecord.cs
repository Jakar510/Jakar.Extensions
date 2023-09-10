// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:59 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Scopes" ) ]
public sealed record ScopeRecord : LoggerTable<ScopeRecord>
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
    public static IEnumerable<ScopeRecord> Create( AppLog log, AppRecord app, DeviceRecord device, SessionRecord session, UserRecord? caller = default )
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( Guid scopeID in log.ScopeIDs ) { yield return new ScopeRecord( scopeID, app, device, session, caller ); }
    }


    public override int CompareTo( ScopeRecord? other ) => Nullable.Compare( AppID, other?.AppID );
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, SessionID, base.GetHashCode() );
}



[ Serializable, Table( "LogScopes" ) ]
public sealed record LogScopeRecord : Mapping<LogScopeRecord, LogRecord, ScopeRecord>, ICreateMapping<LogScopeRecord, LogRecord, ScopeRecord>
{
    public LogScopeRecord() : base() { }
    public LogScopeRecord( LogRecord key, ScopeRecord value ) : base( key, value ) { }


    public static LogScopeRecord Create( LogRecord key, ScopeRecord value ) => new(key, value);
}
