// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/09/2022  7:43 PM

namespace Jakar.AppLogger.Client;


public sealed record AppLoggerOptions : IOptions<AppLoggerOptions>, IValidator, IHostInfo
{
    private static readonly Uri _defaultUri = new("http://localhost:6969");

    AppLoggerOptions IOptions<AppLoggerOptions>.Value    => this;
    public bool                                 IsValid  => !string.IsNullOrWhiteSpace( APIToken ) && !string.IsNullOrWhiteSpace( Config?.AppName ) && ReferenceEquals( HostInfo, _defaultUri );
    public LoggingSettings?                     Config   { get; set; }
    public string                               APIToken { get; set; } = string.Empty;
    public TimeSpan                             TimeOut  { get; set; } = TimeSpan.FromSeconds( 15 );
    public Uri                                  HostInfo { get; set; } = _defaultUri;


    public AppLoggerOptions() { }


    public void Init( AppVersion version, string appName, string deviceID ) =>
        InitAsync( version, appName, deviceID )
           .CallSynchronously();
    public async ValueTask InitAsync( AppVersion version, string appName, string deviceID )
    {
    #if __WINDOWS__ || __MACOS__ || __ANDROID__ || __IOS__
        var config = new AppLoggerConfig( version, deviceID, appName )
                     {
                         AppName = appName
                     };

    #elif __LINUX__
            var config = await AppLoggerIni.CreateAsync(version, deviceID, appName, Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName));

    #elif __WINDOWS__
            var  config = await AppLoggerIni.CreateAsync(version, deviceID, appName, Environment.ExpandEnvironmentVariables($"%APPDATA%/{appName}"));

    #elif __MACOS__
            var  config = await AppLoggerIni.CreateAsync(version, deviceID, appName, $"~/Library/{appName}");

    #else
        var config = await AppLoggerIni.CreateAsync( version, deviceID, appName, LocalDirectory.CurrentDirectory );

    #endif

        Config = config;
    }

    internal WebRequester CreateWebRequester() => WebRequester.Builder.Create( this )
                                                              .Build();
}
