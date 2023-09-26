// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:52 PM


namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Sessions" ) ]
public sealed partial record SessionRecord( DateTimeOffset          AppStartTime,
                                            RecordID<AppRecord>     AppID,
                                            RecordID<DeviceRecord>  DeviceID,
                                            RecordID<SessionRecord> ID,
                                            RecordID<UserRecord>?   CreatedBy,
                                            Guid?                   OwnerUserID,
                                            DateTimeOffset          DateCreated,
                                            DateTimeOffset?         LastModified = default
) : LoggerTable<SessionRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<SessionRecord>, IStartSession
{
    Guid IStartSession.AppID     => AppID.Value;
    Guid IStartSession.DeviceID  => DeviceID.Value;
    Guid? ISessionID.  SessionID => ID.Value;


    public SessionRecord( StartSession start, AppRecord app, DeviceRecord device, UserRecord? caller = default ) : this( start.AppStartTime, app.ID, device.ID, RecordID<SessionRecord>.New(), caller?.ID, caller?.UserID, DateTimeOffset.UtcNow )
    {
        AppStartTime = start.AppStartTime;
        AppID        = app.ID;
        DeviceID     = device.ID;
    }
    public static SessionRecord Create( DbDataReader reader )
    {
        var appStartTime = reader.GetFieldValue<DateTimeOffset>( nameof(AppStartTime) );
        var appID        = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(AppID) ) );
        var deviceID     = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(DeviceID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id           = new RecordID<SessionRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new SessionRecord( appStartTime, appID, deviceID, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<SessionRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    public StartSessionReply ToStartSessionReply() => new(ID.Value, AppID.Value, DeviceID.Value);


    public static DynamicParameters GetDynamicParameters( in Guid sessionID )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(ID), sessionID );
        return parameters;
    }


    public override int CompareTo( SessionRecord? other )
    {
        if ( other == null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        var appCompare = AppID.CompareTo( other.AppID );
        if ( appCompare != 0 ) { return appCompare; }

        return DeviceID.CompareTo( other.DeviceID );
    }
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, base.GetHashCode() );
}
