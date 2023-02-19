// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:52 PM

using Jakar.AppLogger.Portal.Data.Interfaces;



namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Sessions" )]
public sealed record SessionRecord : LoggerTable<SessionRecord>, ISession
{
    public Guid   SessionID { get; init; }
    public string AppID     { get; init; } = string.Empty;
    public string DeviceID  { get; init; } = string.Empty;


    public SessionRecord() : base() { }
    public SessionRecord( AppRecord app, DeviceRecord device ) : base( Guid.NewGuid() )
    {
        AppID    = app.ID;
        DeviceID = device.ID;
    }
    public SessionRecord( AppRecord app, DeviceRecord device, UserRecord caller ) : base( caller )
    {
        AppID    = app.ID;
        DeviceID = device.ID;
    }


    public static DynamicParameters GetDynamicParameters( in Guid sessionID )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(SessionID), sessionID );
        return parameters;
    }


    public override int CompareTo( SessionRecord? other ) => string.Compare( AppID, other?.AppID, StringComparison.Ordinal );
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, base.GetHashCode() );
    public override bool Equals( SessionRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return AppID == other.AppID && DeviceID == other.DeviceID;
    }
}
