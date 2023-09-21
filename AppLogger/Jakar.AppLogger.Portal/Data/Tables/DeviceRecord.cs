﻿// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  10:06 AM

namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Devices" ) ]
public sealed record DeviceRecord( int?                                                          AppBuild,
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
                                   RecordID<UserRecord>?                                         CreatedBy,
                                   Guid?                                                         OwnerUserID,
                                   DateTimeOffset                                                DateCreated,
                                   DateTimeOffset?                                               LastModified = default
) : LoggerTable<DeviceRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<DeviceRecord>, IDevice
{
    AppVersion IDevice.OsVersion => OsVersion;

    public DeviceRecord( IDevice device, UserRecord? caller = default ) : this( device.AppBuild,
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
                                                                                caller?.ID,
                                                                                caller?.UserID,
                                                                                DateTimeOffset.UtcNow ) { }


    public static DeviceRecord Create( DbDataReader reader )
    {
        var appBuild     = reader.GetFieldValue<int>( nameof(AppBuild) );
        var appNamespace = reader.GetString( nameof(AppNamespace) );
        var appVersion   = AppVersion.Parse( reader.GetString( nameof(AppVersion) ) );
        var deviceID     = reader.GetString( nameof(DeviceID) );

        var hardwareInfo = reader.GetFieldValue<string?>( nameof(HwInfo) )
                                ?.FromJson<HwInfo>();

        var locale              = reader.GetString( nameof(Locale) );
        var model               = reader.GetString( nameof(Model) );
        var osApiLevel          = reader.GetFieldValue<int?>( nameof(OsApiLevel) );
        var processArchitecture = (Architecture)reader.GetFieldValue<long>( nameof(ProcessArchitecture) );
        var osName              = reader.GetString( nameof(OsName) );
        var osVersion           = reader.GetString( nameof(OsVersion) );
        var platform            = (PlatformID)reader.GetFieldValue<long>( nameof(Platform) );
        var sdkName             = reader.GetString( nameof(SdkName) );
        var sdkVersion          = reader.GetString( nameof(SdkVersion) );
        var timeZoneOffset      = reader.GetFieldValue<TimeSpan>( nameof(TimeZoneOffset) );
        var dateCreated         = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified        = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var ownerUserID         = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy           = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id                  = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

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
                                 createdBy,
                                 ownerUserID,
                                 dateCreated,
                                 lastModified );
    }
    public static async IAsyncEnumerable<DeviceRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


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
                   HwInfo = device.HwInfo
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
    public override int GetHashCode()                    => HashCode.Combine( DeviceID, base.GetHashCode() );
}



[ Serializable, Table( "Devices" ) ]
public sealed record AppDeviceRecord : Mapping<AppDeviceRecord, AppRecord, DeviceRecord>, ICreateMapping<AppDeviceRecord, AppRecord, DeviceRecord>, IDbReaderMapping<AppDeviceRecord>
{
    public AppDeviceRecord( AppRecord key, DeviceRecord value, UserRecord? caller = default ) : base( key, value, caller ) { }
    private AppDeviceRecord( RecordID<AppRecord> key, RecordID<DeviceRecord> value, RecordID<AppDeviceRecord> id, RecordID<UserRecord> createdBy, Guid ownerUserID, DateTimeOffset dateCreated, DateTimeOffset? lastModified ) :
        base( key, value, id, createdBy, ownerUserID, dateCreated, lastModified ) { }
    public static AppDeviceRecord Create( AppRecord key, DeviceRecord value, UserRecord? caller = default ) => new(key, value, caller);
    public static AppDeviceRecord Create( DbDataReader reader )
    {
        var             key          = new RecordID<AppRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        var             value        = new RecordID<DeviceRecord>( reader.GetFieldValue<Guid>( nameof(KeyID) ) );
        DateTimeOffset  dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset? lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        Guid            ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var             createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var             id           = new RecordID<AppDeviceRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new AppDeviceRecord( key, value, id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<AppDeviceRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }
}
