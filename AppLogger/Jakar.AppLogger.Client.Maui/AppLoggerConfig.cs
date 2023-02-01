namespace Jakar.AppLogger.Client.Maui;


public sealed class AppLoggerConfig : LoggingSettings
{
    public AppLoggerConfig( AppVersion version, string deviceID, string appName ) : base( version, DeviceDescriptor.Create( version, deviceID ) ) => AppName = appName;


    public override ValueTask InitAsync()
    {
        InstallID                  = Guid.Parse( Preferences.Get( nameof(InstallID), EmptyGuid ) );
        SessionID                  = Guid.Parse( Preferences.Get( nameof(SessionID), EmptyGuid ) );
        LogLevel                   = (LogLevel)Preferences.Get( nameof(LogLevel), LogLevel.Debug.AsInt() );
        AppName                    = Preferences.Get( nameof(AppName),                    string.Empty );
        AppUserID                  = Preferences.Get( nameof(AppUserID),                  string.Empty );
        EnableApi                  = Preferences.Get( nameof(EnableApi),                  true );
        EnableCrashes              = Preferences.Get( nameof(EnableCrashes),              true );
        EnableAnalytics            = Preferences.Get( nameof(EnableAnalytics),            true );
        TakeScreenshotOnError      = Preferences.Get( nameof(TakeScreenshotOnError),      true );
        IncludeAppStateOnError     = Preferences.Get( nameof(IncludeAppStateOnError),     true );
        IncludeUserIDOnError       = Preferences.Get( nameof(IncludeUserIDOnError),       true );
        IncludeDeviceInfoOnError   = Preferences.Get( nameof(IncludeDeviceInfoOnError),   true );
        IncludeEventDetailsOnError = Preferences.Get( nameof(IncludeEventDetailsOnError), true );
        IncludeRequestsOnError     = Preferences.Get( nameof(IncludeRequestsOnError),     true );
        IncludeHwInfo              = Preferences.Get( nameof(IncludeHwInfo),              true );

        Device.HwInfo = IncludeHwInfo
                            ? HwInfo.TryCreate()
                            : default;

        return default;
    }


    public override async ValueTask<byte[]?> TakeScreenshot()
    {
        if ( !Screenshot.IsCaptureSupported ) { return default; }

        IScreenshotResult result = await Screenshot.CaptureAsync();
        await using var   target = new MemoryStream();
        await result.CopyToAsync( target );
        return target.ToArray();
    }
    protected override void HandleValue<T>( T value, string propertyName )
    {
        switch ( value )
        {
            case null:
                Preferences.Set( propertyName, null );
                break;

            case string s:
                Preferences.Set( propertyName, s );
                break;

            case bool b:
                Preferences.Set( propertyName, b );
                break;

            case Guid b:
                Preferences.Set( propertyName, b.ToString() );
                break;

            case LogLevel b:
                Preferences.Set( propertyName, b.AsInt() );
                break;

            case Enum b:
                Preferences.Set( propertyName, b.ToString() );
                break;

            default:
                Preferences.Set( propertyName, value.ToJson() );
                break;
        }
    }
}
