// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:59 PM

using Jakar.Database;



namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Scopes" )]
public sealed record ScopeRecord : LoggerTable<ScopeRecord>
{
    public RecordID<AppRecord>     AppID     { get; init; }
    public RecordID<DeviceRecord>  DeviceID  { get; init; }
    public RecordID<SessionRecord> SessionID { get; init; }


    public ScopeRecord() : base() { }
    public ScopeRecord( AppLog log, AppRecord app, DeviceRecord device, SessionRecord session, UserRecord? caller = default ) : base( new RecordID<ScopeRecord>( log.ScopeID ?? throw new ArgumentNullException( nameof(log.ScopeID) ) ), caller )
    {
        AppID     = app.ID;
        DeviceID  = device.ID;
        SessionID = session.ID;
    }


    public override int CompareTo( ScopeRecord? other ) => Nullable.Compare( AppID, other?.AppID );
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, SessionID, base.GetHashCode() );
    public override bool Equals( ScopeRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return AppID == other.AppID && DeviceID == other.DeviceID;
    }
}
