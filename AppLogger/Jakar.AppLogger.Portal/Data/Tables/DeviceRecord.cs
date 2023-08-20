// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  10:06 AM

namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Devices" )]
public sealed record DeviceRecord : LoggerTable<DeviceRecord>, IDevice
{
    public                                           int?       AppBuild            { get; init; }
    [MaxLength( 4096 )] public                       string?    AppNamespace        { get; init; }
    public                                           AppVersion AppVersion          { get; init; } = new();
    [MaxLength( 4096 )]                       public string     DeviceID            { get; init; } = string.Empty;
    [MaxLength( BaseRecord.MAX_STRING_SIZE )] public string?    HardwareInfo        { get; init; }
    HwInfo? IDevice.                                            HwInfo              => HardwareInfo?.FromJson<HwInfo>();
    AppVersion IDevice.                                         OsVersion           => OsVersion;
    [MaxLength( 256 )]  public string                           Locale              { get; init; } = string.Empty;
    [MaxLength( 4096 )] public string?                          Model               { get; init; }
    public                     int?                             OsApiLevel          { get; init; }
    [MaxLength( 256 )] public  string?                          OsBuild             { get; init; }
    public                     Architecture                     ProcessArchitecture { get; init; }
    [MaxLength( 256 )] public  string                           OsName              { get; init; } = string.Empty;
    [MaxLength( 256 )] public  string                           OsVersion           { get; init; } = string.Empty;
    public                     PlatformID                       Platform            { get; init; }
    [MaxLength( 256 )] public  string                           SdkName             { get; init; } = string.Empty;
    [MaxLength( 256 )] public  string                           SdkVersion          { get; init; } = string.Empty;
    public                     TimeSpan                         TimeZoneOffset      { get; init; }


    public DeviceRecord() : base() { }
    public DeviceRecord( IDevice device, UserRecord? caller = default ) : base( Guid.NewGuid(), caller )
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


    public static DynamicParameters GetDynamicParameters( DeviceDescriptor device, UserRecord caller )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(DeviceID),  device.DeviceID );
        parameters.Add( nameof(CreatedBy), caller.ID );
        return parameters;
    }


    public DeviceDescriptor ToDeviceDescriptor() => new(this);


    public override int CompareTo( DeviceRecord? other ) => string.CompareOrdinal( DeviceID, other?.DeviceID );
    public override int GetHashCode() => HashCode.Combine( DeviceID, base.GetHashCode() );
    public override bool Equals( DeviceRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( DeviceID, other.DeviceID, StringComparison.Ordinal );
    }
}
