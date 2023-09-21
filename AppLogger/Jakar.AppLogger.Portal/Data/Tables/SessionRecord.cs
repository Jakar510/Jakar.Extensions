// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:52 PM


namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Sessions" ) ]
public sealed partial record SessionRecord : LoggerTable<SessionRecord>, IDbReaderMapping<SessionRecord>, IStartSession
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
    public static SessionRecord Create( DbDataReader reader )
    {
        DateTimeOffset           dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?          lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        Guid                     ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord>     createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<LogScopeRecord> id           = new RecordID<LogScopeRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new SessionRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
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
