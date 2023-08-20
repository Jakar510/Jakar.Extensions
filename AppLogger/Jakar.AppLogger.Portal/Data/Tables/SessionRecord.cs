// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:52 PM


namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Sessions" )]
public sealed record SessionRecord : LoggerTable<SessionRecord>, IStartSession
{
    public Guid      AppID     { get; init; }
    public Guid      DeviceID  { get; init; }
    public Guid      SessionID { get; init; }
    Guid? ISessionID.SessionID => SessionID;


    public SessionRecord() : base() { }
    public SessionRecord( AppRecord app, DeviceRecord device, UserRecord? caller = default ) : base( Guid.NewGuid(), caller )
    {
        AppID    = app.ID;
        DeviceID = device.ID;
    }
    public StartSessionReply ToStartSessionReply() => new(SessionID, AppID, DeviceID);


    public static DynamicParameters GetDynamicParameters( in Guid sessionID )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(SessionID), sessionID );
        return parameters;
    }


    public override int CompareTo( SessionRecord? other ) => Nullable.Compare( AppID, other?.AppID );
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, base.GetHashCode() );
    public override bool Equals( SessionRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return AppID == other.AppID && DeviceID == other.DeviceID;
    }
}
