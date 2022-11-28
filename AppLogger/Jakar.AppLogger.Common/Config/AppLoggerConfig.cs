namespace Jakar.AppLogger.Common;


public sealed class AppLoggerConfig : LoggingSettings
{
    public AppLoggerConfig( in AppVersion version ) : base( version ) { }
    public AppLoggerConfig( in AppVersion version, DeviceDescriptor device ) : base( version, device ) { }
    public AppLoggerConfig( in AppVersion version, string           appName ) : base( version ) => AppName = appName;
    public AppLoggerConfig( in AppVersion version, DeviceDescriptor device, string appName ) : base( version, device ) => AppName = appName;


    public override async ValueTask InitAsync()
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

        var device = await DeviceDescriptor.CreateAsync( IncludeHwInfo, Version );
        SetDevice( device );
    }


    protected override void HandleValue<T>( T value, string propertyName )
    {
        switch (value)
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
