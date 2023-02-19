// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:59 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Scopes" )]
public sealed record ScopeRecord : LoggerTable<ScopeRecord>
{
    public Guid   ScopeID   { get; init; }
    public Guid   SessionID { get; init; }
    public string AppID     { get; init; } = string.Empty;
    public string DeviceID  { get; init; } = string.Empty;


    public ScopeRecord() : base() { }
    public ScopeRecord( Log log, AppRecord app, DeviceRecord device, SessionRecord session ) : base( Guid.NewGuid() )
    {
        ArgumentNullException.ThrowIfNull( log.ScopeID );

        AppID     = app.ID;
        DeviceID  = device.ID;
        SessionID = session.SessionID;
        ScopeID   = log.ScopeID.Value;
    }
    public ScopeRecord( Log log, AppRecord app, DeviceRecord device, SessionRecord session, UserRecord caller ) : base( caller )
    {
        ArgumentNullException.ThrowIfNull( log.ScopeID );

        AppID     = app.ID;
        DeviceID  = device.ID;
        SessionID = session.SessionID;
        ScopeID   = log.ScopeID.Value;
    }


    public override int CompareTo( ScopeRecord? other ) => string.Compare( AppID, other?.AppID, StringComparison.Ordinal );
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, base.GetHashCode() );
    public override bool Equals( ScopeRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return AppID == other.AppID && DeviceID == other.DeviceID;
    }
}
