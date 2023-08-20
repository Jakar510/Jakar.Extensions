// Jakar.Extensions :: Jakar.AppLogger
// 07/06/2022  1:34 PM

using System.Globalization;
using Jakar.AppLogger.Common;
using Jakar.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;



namespace Jakar.AppLogger.Client.AspNetCore;


public sealed class AppLoggerIni : LoggingSettings
{
    private readonly IniConfig _config = new();


    internal LocalFile         File    { get; }
    internal IniConfig.Section Section => _config[nameof(AppLogger)];


    public AppLoggerIni( AppVersion   version, string           deviceID, string    appName ) : this( version, deviceID, GetFile( appName ) ) => AppName = appName;
    internal AppLoggerIni( AppVersion version, string           deviceID, LocalFile file ) : this( version, DeviceDescriptor.Create( version, deviceID ), file ) { }
    internal AppLoggerIni( AppVersion version, DeviceDescriptor device,   LocalFile file ) : base( version, device ) => File = file;


    public static AppLoggerIni Create( AppVersion version, string deviceID, string appName )
    {
        Task<AppLoggerIni> task = Task.Run( async () => await CreateAsync( version, deviceID, appName ) );
        task.Wait();

        return task.Result;
    }
    public static async ValueTask<AppLoggerIni> CreateAsync( AppVersion version, string deviceID, string appName )
    {
        var ini = new AppLoggerIni( version, deviceID, appName );
        return await ini.RefreshAsync();
    }


    protected override void HandleValue<T>( T value, string propertyName ) => Section[propertyName] = value switch
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
                                                                                                          _        => value.ToJson(),
                                                                                                      };


    public static LocalFile GetFile( string appName ) => GetDirectory( appName )
       .Join( appName );
    public static LocalDirectory GetDirectory( string appName )
    {
        PlatformID platform = Environment.OSVersion.Platform;

        switch ( platform )
        {
            case PlatformID.MacOSX: return LocalDirectory.Create( $"~/Library/{appName}" );


            case PlatformID.Unix: return LocalDirectory.Create( $"/etc/{appName}" );

            case PlatformID.Win32NT:
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.WinCE:
            case PlatformID.Xbox:
                return LocalDirectory.Create( Environment.ExpandEnvironmentVariables( $"%APPDATA%/{appName}" ) );


            case PlatformID.Other:
            default:
                LocalDirectory directory = Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData );
                if ( directory.DoesNotExist ) { directory = LocalDirectory.CurrentDirectory; }

                return directory;
        }
    }


    public override async ValueTask InitAsync()
    {
        await RefreshAsync();

        Device.HwInfo = IncludeHwInfo
                            ? HwInfo.TryCreate()
                            : default;
    }


    public async ValueTask<AppLoggerIni> RefreshAsync()
    {
        IniConfig ini = await IniConfig.ReadFromFileAsync( File );
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

                case nameof(Session): { break; }

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


    public async Task SaveAsync() => await _config.WriteToFile( File );


    public override ValueTask<byte[]?> TakeScreenshot() => default; // TODO: TakeScreenshot
}
