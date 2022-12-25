// Jakar.Extensions :: Jakar.AppLogger
// 07/06/2022  1:34 PM

namespace Jakar.AppLogger.Common;


public sealed class AppLoggerIni : LoggingSettings
{
    public const     string    CONFIG_FILE_NAME = "AppCenter.ini";
    private readonly IniConfig _config          = new();


    private LocalFile? _file;


    internal LocalFile Path
    {
        get => _file ??= LocalDirectory.CurrentDirectory.Join( CONFIG_FILE_NAME );
        set => _file = value;
    }
    internal IniConfig.Section Section => _config[nameof(AppLogger)];


    public AppLoggerIni( AppVersion version, DeviceDescriptor device, LocalDirectory directory ) : this( version, device, directory.Join( CONFIG_FILE_NAME ) ) { }
    public AppLoggerIni( AppVersion version, DeviceDescriptor device, LocalFile      file ) : this( version, device ) => Path = file;
    public AppLoggerIni( AppVersion version, DeviceDescriptor device ) : base( version, device ) { }


    public static AppLoggerIni Create( AppVersion version, string appName, bool             includeHwInfo ) => Create( version,                           appName, DeviceDescriptor.Create( includeHwInfo, version ) );
    public static AppLoggerIni Create( AppVersion version, string appName, DeviceDescriptor device ) => Create( version,                                  appName, device, LocalDirectory.CurrentDirectory );
    public static AppLoggerIni Create( AppVersion version, string appName, DeviceDescriptor device,        LocalDirectory directory ) => Create( version, appName, device, directory.Join( CONFIG_FILE_NAME ) );
    public static AppLoggerIni Create( AppVersion version, string appName, bool             includeHwInfo, LocalDirectory directory ) => Create( version, appName, includeHwInfo, directory.Join( CONFIG_FILE_NAME ) );
    public static AppLoggerIni Create( AppVersion version, string appName, bool             includeHwInfo, LocalFile      file ) => Create( version,      appName, DeviceDescriptor.Create( includeHwInfo, version ), file );
    public static AppLoggerIni Create( AppVersion version, string appName, DeviceDescriptor device, LocalFile file ) => new(version, device, file)
                                                                                                                        {
                                                                                                                            AppName = appName
                                                                                                                        };


    public static ValueTask<AppLoggerIni> CreateAsync( AppVersion version, DeviceDescriptor device, string appName ) => CreateAsync( version,                           device, appName, LocalDirectory.CurrentDirectory );
    public static ValueTask<AppLoggerIni> CreateAsync( AppVersion version, DeviceDescriptor device, string appName, LocalDirectory directory ) => CreateAsync( version, device, appName, directory.Join( CONFIG_FILE_NAME ) );
    public static async ValueTask<AppLoggerIni> CreateAsync( AppVersion version, DeviceDescriptor device, string appName, LocalFile file )
    {
        AppLoggerIni ini = await new AppLoggerIni( version, device, file ).RefreshAsync();
        ini.AppName = appName;
        return ini;
    }


    protected override void HandleValue<T>( T value, string propertyName ) =>
        Section[propertyName] = value switch
                                {
                                    null     => string.Empty,
                                    string s => s,
                                    Enum b   => b.ToString(),
                                    Guid b   => b.ToString(),
                                    bool b   => b.ToString(),
                                    JToken b => b.ToString(),
                                    short b  => b.ToString(),
                                    ushort b => b.ToString(),
                                    int b    => b.ToString(),
                                    uint b   => b.ToString(),
                                    long b   => b.ToString(),
                                    ulong b  => b.ToString(),
                                    float b  => b.ToString( CultureInfo.InvariantCulture ),
                                    double b => b.ToString( CultureInfo.InvariantCulture ),
                                    _        => value.ToJson()
                                };

    public override async ValueTask InitAsync()
    {
        var device = await DeviceDescriptor.CreateAsync( IncludeHwInfo, Version );
        SetDevice( device );
        await RefreshAsync();
    }


    public async ValueTask<AppLoggerIni> RefreshAsync()
    {
        IniConfig? ini = await IniConfig.ReadFromFileAsync( Path );
        return Refresh( ini );
    }
    public AppLoggerIni Refresh()
    {
        IniConfig? ini = IniConfig.ReadFromFile( Path );
        return Refresh( ini );
    }


    private AppLoggerIni Refresh( IniConfig? config )
    {
        if ( config is null ) { return this; }

        IniConfig.Section section = config[nameof(AppLogger)];

        foreach ( (string key, string? value) in section )
        {
            switch ( key )
            {
                case nameof(AppName):
                {
                    AppName = value ?? string.Empty;
                    break;
                }

                case nameof(InstallID):
                {
                    InstallID = Guid.TryParse( value, out Guid id )
                                    ? id
                                    : Guid.NewGuid();

                    break;
                }

                case nameof(SessionID):
                {
                    SessionID = Guid.TryParse( value, out Guid id )
                                    ? id
                                    : default;

                    break;
                }

                case nameof(AppUserID):
                {
                    AppUserID = value;
                    break;
                }

                case nameof(LogLevel):
                {
                    LogLevel = Enum.TryParse( value, out LogLevel level )
                                   ? level
                                   : LogLevel.Trace;

                    break;
                }

                case nameof(TakeScreenshotOnError):
                {
                    TakeScreenshotOnError = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(IncludeAppStateOnError):
                {
                    IncludeAppStateOnError = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(IncludeUserIDOnError):
                {
                    IncludeUserIDOnError = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(IncludeDeviceInfoOnError):
                {
                    IncludeDeviceInfoOnError = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(IncludeEventDetailsOnError):
                {
                    IncludeEventDetailsOnError = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(IncludeRequestsOnError):
                {
                    IncludeRequestsOnError = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(IncludeHwInfo):
                {
                    IncludeHwInfo = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(EnableAnalytics):
                {
                    EnableAnalytics = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(EnableCrashes):
                {
                    EnableCrashes = bool.TryParse( value, out bool result ) && result;
                    break;
                }

                case nameof(EnableApi):
                {
                    EnableApi = bool.TryParse( value, out bool result ) && result;
                    break;
                }
            }
        }

        return this;
    }


    public async Task SaveAsync() => await _config.WriteToFile( Path );
}
