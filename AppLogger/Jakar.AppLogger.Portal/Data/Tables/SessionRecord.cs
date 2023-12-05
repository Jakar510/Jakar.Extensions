// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/21/2022  4:52 PM


namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Sessions" ) ]
public sealed record SessionRecord
    ( DateTimeOffset AppStartTime, RecordID<AppRecord> AppID, RecordID<DeviceRecord> DeviceID, RecordID<SessionRecord> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) :
        LoggerTable<SessionRecord>( ID, DateCreated, LastModified ),
        IDbReaderMapping<SessionRecord>,
        IStartSession,
        MsJsonModels.IJsonizer<SessionRecord>
{
    public static string TableName { get; } = typeof(SessionRecord).GetTableName();


    Guid IStartSession.AppID     => AppID.Value;
    Guid IStartSession.DeviceID  => DeviceID.Value;
    Guid? ISessionID.  SessionID => ID.Value;


    public SessionRecord( StartSession start, AppRecord app, DeviceRecord device ) : this( start.AppStartTime, app.ID, device.ID, RecordID<SessionRecord>.New(), DateTimeOffset.UtcNow )
    {
        AppStartTime = start.AppStartTime;
        AppID        = app.ID;
        DeviceID     = device.ID;
    }


    [ Pure ]
    public static SessionRecord Create( DbDataReader reader )
    {
        var appStartTime = reader.GetFieldValue<DateTimeOffset>( nameof(AppStartTime) );
        var appID        = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(AppID) ) );
        var deviceID     = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(DeviceID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<SessionRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new SessionRecord( appStartTime, appID, deviceID, id, dateCreated, lastModified );
    }
    [ Pure ]
    public static async IAsyncEnumerable<SessionRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }

    [ Pure ] public StartSessionReply ToStartSessionReply() => new(ID.Value, AppID.Value, DeviceID.Value);


    [ Pure ]
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

        int appCompare = AppID.CompareTo( other.AppID );
        if ( appCompare != 0 ) { return appCompare; }

        return DeviceID.CompareTo( other.DeviceID );
    }
    public override int GetHashCode() => HashCode.Combine( AppID, DeviceID, base.GetHashCode() );


    [ Pure ] public static SessionRecord FromJson( string json ) => json.FromJson( JsonTypeInfo() );
    [ Pure ]
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = SessionRecordContext.Default
                                                                         };
    [ Pure ] public static JsonTypeInfo<SessionRecord> JsonTypeInfo() => SessionRecordContext.Default.SessionRecord;
}



[ JsonSerializable( typeof(SessionRecord) ) ] public partial class SessionRecordContext : JsonSerializerContext;
