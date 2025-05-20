// TrueLogic :: TrueLogic.Common
// 01/02/2025  12:01

namespace Jakar.Extensions.Loggers;


public sealed class EventProperties : ActivityTagsCollection
{
    public const string APP_BUILD               = nameof(APP_BUILD);
    public const string APP_DEVICE_ID           = nameof(APP_DEVICE_ID);
    public const string APP_GC_ALLOCATED_BYTES  = nameof(APP_GC_ALLOCATED_BYTES);
    public const string APP_GC_TOTAL_PAUSE_TIME = nameof(APP_GC_TOTAL_PAUSE_TIME);
    public const string APP_ID                  = nameof(APP_ID);
    public const string APP_ISO_LANGUAGE        = nameof(APP_ISO_LANGUAGE);
    public const string APP_NAME                = nameof(APP_NAME);
    public const string APP_PACKAGE_NAME        = nameof(APP_PACKAGE_NAME);
    public const string APP_STATE               = nameof(APP_STATE);
    public const string APP_THREAD_ID           = nameof(APP_THREAD_ID);
    public const string APP_THREAD_LANGUAGE     = nameof(APP_THREAD_LANGUAGE);
    public const string APP_THREAD_NAME         = nameof(APP_THREAD_NAME);
    public const string APP_TOTAL_MEMORY        = nameof(APP_TOTAL_MEMORY);
    public const string APP_UI_ISO_LANGUAGE     = nameof(APP_UI_ISO_LANGUAGE);
    public const string APP_UI_LANGUAGE         = nameof(APP_UI_LANGUAGE);
    public const string APP_VERSION             = nameof(APP_VERSION);
    public const string DEVICE_APP_VERSION      = nameof(DEVICE_APP_VERSION);
    public const string DEVICE_MANUFACTURER     = nameof(DEVICE_MANUFACTURER);
    public const string DEVICE_MODEL            = nameof(DEVICE_MODEL);
    public const string DEVICE_PLATFORM         = nameof(DEVICE_PLATFORM);
    public const string DEVICE_VERSION          = nameof(DEVICE_VERSION);
    public const string TIMESTAMP               = nameof(TIMESTAMP);


    public EventProperties() : base() { }
    public EventProperties( IDictionary<string, string?> dictionary ) : base()
    {
        foreach ( (string key, string? value) in dictionary ) { Add( key, value ); }
    }


    public override string ToString() => this.ToJson( Formatting.Indented );


    public static EventProperties Create<TApp>()
        where TApp : IAppID =>
        new()
        {
            [APP_NAME]        = TApp.AppName,
            [APP_ID]          = TApp.AppID.ToString(),
            [APP_VERSION]     = TApp.AppVersion.ToString(),
            [TIMESTAMP]       = DateTimeOffset.UtcNow.ToString( CultureInfo.InvariantCulture ),
            [APP_UI_LANGUAGE] = CultureInfo.CurrentUICulture.DisplayName
        };

    public static JToken CreateToken<TApp>()
        where TApp : IAppID => JToken.FromObject( Create<TApp>() );

    public static EventProperties Create( TelemetrySource sources, DeviceMetaData? deviceInfo = null )
    {
        Thread thread = Thread.CurrentThread;

        EventProperties properties = new()
                                     {
                                         [APP_NAME]                = sources.AppName,
                                         [APP_PACKAGE_NAME]        = sources.PackageName,
                                         [APP_ID]                  = sources.AppID.ToString(),
                                         [APP_VERSION]             = sources.Version.ToString(),
                                         [APP_BUILD]               = sources.Version.Build.ToString(),
                                         [APP_THREAD_ID]           = thread.ManagedThreadId.ToString(),
                                         [APP_THREAD_NAME]         = thread.Name,
                                         [APP_ISO_LANGUAGE]        = thread.CurrentCulture.DisplayName,
                                         [APP_UI_ISO_LANGUAGE]     = thread.CurrentUICulture.DisplayName,
                                         [APP_THREAD_LANGUAGE]     = CultureInfo.CurrentCulture.DisplayName,
                                         [APP_UI_LANGUAGE]         = CultureInfo.CurrentUICulture.DisplayName,
                                         [TIMESTAMP]               = DateTimeOffset.UtcNow.ToString( CultureInfo.InvariantCulture ),
                                         [APP_TOTAL_MEMORY]        = GC.GetTotalMemory( false ).ToString(),
                                         [APP_GC_TOTAL_PAUSE_TIME] = GC.GetTotalPauseDuration().ToString(),
                                         [APP_GC_ALLOCATED_BYTES]  = GC.GetTotalAllocatedBytes().ToString()
                                     };


        if ( deviceInfo is null ) { return properties; }

        properties[APP_DEVICE_ID]       = deviceInfo.DeviceID;
        properties[DEVICE_VERSION]      = deviceInfo.DeviceVersion;
        properties[DEVICE_MODEL]        = deviceInfo.DeviceModel;
        properties[DEVICE_PLATFORM]     = deviceInfo.DevicePlatform;
        properties[DEVICE_MANUFACTURER] = deviceInfo.DeviceManufacturer;

        return properties;
    }
    public static JToken CreateToken( TelemetrySource sources ) => JToken.FromObject( Create( sources ) );
}



public sealed class DeviceMetaData
{
    public string? DeviceAppVersion   { get; set; }
    public string? DeviceID           { get; set; }
    public string? DeviceManufacturer { get; set; }
    public string? DeviceModel        { get; set; }
    public string? DevicePlatform     { get; set; }
    public string? DeviceVersion      { get; set; }
}
