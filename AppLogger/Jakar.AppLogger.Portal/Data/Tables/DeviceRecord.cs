// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  10:06 AM

using Jakar.Database.Generators;



namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Devices" ) ]
public sealed partial record DeviceRecord : LoggerTable<DeviceRecord>, IDbReaderMapping<DeviceRecord>, IDevice
{
    public                                  int?       AppBuild            { get; init; }
    [ MaxLength( 4096 ) ] public            string?    AppNamespace        { get; init; }
    public                                  AppVersion AppVersion          { get; init; } = new();
    [ MaxLength( 4096 ) ]            public string     DeviceID            { get; init; } = string.Empty;
    [ MaxLength( MAX_STRING_SIZE ) ] public string?    HardwareInfo        { get; init; }
    HwInfo? IDevice.                                   HwInfo              => HardwareInfo?.FromJson<HwInfo>();
    AppVersion IDevice.                                OsVersion           => OsVersion;
    [ MaxLength( 256 ) ]  public string                Locale              { get; init; } = string.Empty;
    [ MaxLength( 4096 ) ] public string?               Model               { get; init; }
    public                       int?                  OsApiLevel          { get; init; }
    [ MaxLength( 256 ) ] public  string?               OsBuild             { get; init; }
    public                       Architecture          ProcessArchitecture { get; init; }
    [ MaxLength( 256 ) ] public  string                OsName              { get; init; } = string.Empty;
    [ MaxLength( 256 ) ] public  string                OsVersion           { get; init; } = string.Empty;
    public                       PlatformID            Platform            { get; init; }
    [ MaxLength( 256 ) ] public  string                SdkName             { get; init; } = string.Empty;
    [ MaxLength( 256 ) ] public  string                SdkVersion          { get; init; } = string.Empty;
    public                       TimeSpan              TimeZoneOffset      { get; init; }


    public DeviceRecord( IDevice device, UserRecord? caller = default ) : base( caller )
    {
        DeviceID            = device.DeviceID;
        SdkName             = device.SdkName;
        SdkVersion          = device.SdkVersion;
        Model               = device.Model;
        OsName              = device.OsName;
        OsVersion           = device.OsVersion.ToString();
        OsApiLevel          = device.OsApiLevel;
        Locale              = device.Locale;
        TimeZoneOffset      = device.TimeZoneOffset;
        AppVersion          = device.AppVersion;
        AppBuild            = device.AppBuild;
        AppNamespace        = device.AppNamespace;
        ProcessArchitecture = device.ProcessArchitecture;
        HardwareInfo        = device.HwInfo?.ToJson();
    }


    [ DbReaderMapping ] public static partial DeviceRecord Create( DbDataReader reader );
    /*
    public static DeviceRecord Create( DbDataReader reader )
    {
        DateTimeOffset       dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset       lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                 ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord> createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<RoleRecord> id           = new RecordID<RoleRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new DeviceRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<DeviceRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }*/

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    public DeviceRecord Update( IDevice device, UserRecord caller )
    {
        if ( CreatedBy != caller.ID ) { throw new ArgumentException( $"{nameof(caller)} doesn't own this {nameof(device)}", nameof(caller) ); }

        return this with
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
                   HardwareInfo = device.HwInfo?.ToJson(),
               };
    }


    public static DynamicParameters GetDynamicParameters( DeviceDescriptor device, UserRecord caller ) => GetDynamicParameters( device, caller.ID );
    public static DynamicParameters GetDynamicParameters( DeviceDescriptor device, RecordID<UserRecord> caller )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(DeviceID),  device.DeviceID );
        parameters.Add( nameof(CreatedBy), caller.Value );
        return parameters;
    }


    public DeviceDescriptor ToDeviceDescriptor() => new(this);


    public override int CompareTo( DeviceRecord? other ) => string.CompareOrdinal( DeviceID, other?.DeviceID );
    public override int GetHashCode() => HashCode.Combine( DeviceID, base.GetHashCode() );
}



[ Serializable, Table( "Devices" ) ]
public sealed record AppDeviceRecord : Mapping<AppDeviceRecord, AppRecord, DeviceRecord>, ICreateMapping<AppDeviceRecord, AppRecord, DeviceRecord>, IDbReaderMapping<AppDeviceRecord>
{
    public AppDeviceRecord( AppRecord               key, DeviceRecord value ) : base( key, value ) { }
    public static AppDeviceRecord Create( AppRecord key, DeviceRecord value ) => new(key, value);
    public static AppDeviceRecord Create( DbDataReader reader )
    {
        DateTimeOffset       dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset       lastModified = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        Guid                 ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        RecordID<UserRecord> createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        RecordID<RoleRecord> id           = new RecordID<RoleRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new AppDeviceRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<AppDeviceRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
