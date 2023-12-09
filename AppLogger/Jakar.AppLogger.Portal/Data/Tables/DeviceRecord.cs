// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  10:06 AM

namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Devices" ) ]
public sealed record DeviceRecord(
    int?                                                          AppBuild,
    [ property: MaxLength( 4096 ) ] string?                       AppNamespace,
    AppVersion                                                    AppVersion,
    [ property: MaxLength( 4096 ) ]                       string  DeviceID,
    [ property: MaxLength( BaseRecord.MAX_STRING_SIZE ) ] HwInfo? HwInfo,
    [ property: MaxLength( 256 ) ]                        string  Locale,
    [ property: MaxLength( 4096 ) ]                       string? Model,
    int?                                                          OsApiLevel,
    Architecture                                                  ProcessArchitecture,
    [ property: MaxLength( 256 ) ] string                         OsName,
    [ property: MaxLength( 256 ) ] AppVersion                     OsVersion,
    PlatformID                                                    Platform,
    [ property: MaxLength( 256 ) ] string                         SdkName,
    [ property: MaxLength( 256 ) ] string                         SdkVersion,
    TimeSpan                                                      TimeZoneOffset,
    RecordID<DeviceRecord>                                        ID,
    DateTimeOffset                                                DateCreated,
    DateTimeOffset?                                               LastModified = default
) : LoggerTable<DeviceRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<DeviceRecord>, IDevice
{
    public static string TableName { get; } = typeof(DeviceRecord).GetTableName();


    AppVersion IDevice.OsVersion => OsVersion;

    public DeviceRecord( IDevice device ) : this( device.AppBuild,
                                                  device.AppNamespace,
                                                  device.AppVersion,
                                                  device.DeviceID,
                                                  device.HwInfo,
                                                  device.Locale,
                                                  device.Model,
                                                  device.OsApiLevel,
                                                  device.ProcessArchitecture,
                                                  device.OsName,
                                                  device.OsVersion,
                                                  device.Platform,
                                                  device.SdkName,
                                                  device.SdkVersion,
                                                  device.TimeZoneOffset,
                                                  RecordID<DeviceRecord>.New(),
                                                  DateTimeOffset.UtcNow ) { }


    [ Pure ]
    public static DeviceRecord Create( DbDataReader reader )
    {
        int        appBuild     = reader.GetFieldValue<int>( nameof(AppBuild) );
        string     appNamespace = reader.GetString( nameof(AppNamespace) );
        AppVersion appVersion   = AppVersion.Parse( reader.GetString( nameof(AppVersion) ) );
        string     deviceID     = reader.GetString( nameof(DeviceID) );

        var hardwareInfo = reader.GetFieldValue<string?>( nameof(HwInfo) )?.FromJson<HwInfo>();

        string locale              = reader.GetString( nameof(Locale) );
        string model               = reader.GetString( nameof(Model) );
        int?   osApiLevel          = reader.GetFieldValue<int?>( nameof(OsApiLevel) );
        var    processArchitecture = (Architecture)reader.GetFieldValue<long>( nameof(ProcessArchitecture) );
        string osName              = reader.GetString( nameof(OsName) );
        string osVersion           = reader.GetString( nameof(OsVersion) );
        var    platform            = (PlatformID)reader.GetFieldValue<long>( nameof(Platform) );
        string sdkName             = reader.GetString( nameof(SdkName) );
        string sdkVersion          = reader.GetString( nameof(SdkVersion) );
        var    timeZoneOffset      = reader.GetFieldValue<TimeSpan>( nameof(TimeZoneOffset) );
        var    dateCreated         = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var    lastModified        = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var    id                  = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

        return new DeviceRecord( appBuild,
                                 appNamespace,
                                 appVersion,
                                 deviceID,
                                 hardwareInfo,
                                 locale,
                                 model,
                                 osApiLevel,
                                 processArchitecture,
                                 osName,
                                 osVersion,
                                 platform,
                                 sdkName,
                                 sdkVersion,
                                 timeZoneOffset,
                                 id,
                                 dateCreated,
                                 lastModified );
    }
    [ Pure ]
    public static async IAsyncEnumerable<DeviceRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    [ Pure ]
    public DeviceRecord Update( IDevice device ) =>
        this with
        {
            DeviceID = device.DeviceID,
            SdkName = device.SdkName,
            SdkVersion = device.SdkVersion,
            Model = device.Model,
            OsName = device.OsName,
            OsVersion = device.OsVersion.ToString(),
            OsApiLevel = device.OsApiLevel,
            Locale = device.Locale,
            TimeZoneOffset = device.TimeZoneOffset,
            AppVersion = device.AppVersion,
            AppBuild = device.AppBuild,
            AppNamespace = device.AppNamespace,
            ProcessArchitecture = device.ProcessArchitecture,
            HwInfo = device.HwInfo
        };


    [ Pure ]
    public static DynamicParameters GetDynamicParameters( DeviceDescriptor device )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(DeviceID), device.DeviceID );
        return parameters;
    }


    [ Pure ] public DeviceDescriptor ToDeviceDescriptor() => new(this);


    public override int CompareTo( DeviceRecord? other ) => string.CompareOrdinal( DeviceID, other?.DeviceID );
    public override int GetHashCode()                    => HashCode.Combine( DeviceID, base.GetHashCode() );
}



[ Serializable, Table( "Devices" ) ]
public sealed record AppDeviceRecord : Mapping<AppDeviceRecord, AppRecord, DeviceRecord>, ICreateMapping<AppDeviceRecord, AppRecord, DeviceRecord>, IDbReaderMapping<AppDeviceRecord>
{
    public static string TableName { get; } = typeof(AppDeviceRecord).GetTableName();


    public AppDeviceRecord( AppRecord            key, DeviceRecord           value ) : base( key, value ) { }
    private AppDeviceRecord( RecordID<AppRecord> key, RecordID<DeviceRecord> value, RecordID<AppDeviceRecord> id, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) : base( key, value, id, dateCreated, lastModified ) { }


    [ Pure ] public static AppDeviceRecord Create( AppRecord key, DeviceRecord value ) => new(key, value);
    [ Pure ]
    public static AppDeviceRecord Create( DbDataReader reader )
    {
        var key          = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var value        = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var id           = new RecordID<AppDeviceRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new AppDeviceRecord( key, value, id, dateCreated, lastModified );
    }
    [ Pure ]
    public static async IAsyncEnumerable<AppDeviceRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}



[ JsonSerializable( typeof(AppDeviceRecord) ) ] public partial class AppDeviceRecordContext : JsonSerializerContext;
