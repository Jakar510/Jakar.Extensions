// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:52 PM


using Jakar.Database;



namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable,Table( "Sessions" )]
public sealed record SessionRecord : LoggerTable<SessionRecord>, IStartSession
{
    public DateTimeOffset         AppStartTime { get; init; }
    public RecordID<AppRecord>    AppID        { get; init; }
    public RecordID<DeviceRecord> DeviceID     { get; init; }
    Guid IStartSession.           AppID        => AppID.Value;
    Guid IStartSession.           DeviceID     => DeviceID.Value;
    Guid? ISessionID.             SessionID    => ID.Value;


    public SessionRecord( StartSession start, AppRecord app, DeviceRecord device, UserRecord? caller = default ) : base( caller )
    {
        AppStartTime = start.AppStartTime;
        AppID        = app.ID;
        DeviceID     = device.ID;
    }
    public StartSessionReply ToStartSessionReply() => new(ID.Value, AppID.Value, DeviceID.Value);


    public static DynamicParameters GetDynamicParameters( in Guid sessionID )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ID), sessionID );
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
