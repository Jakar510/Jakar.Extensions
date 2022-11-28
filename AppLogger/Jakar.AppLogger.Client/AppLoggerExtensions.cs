// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/09/2022  11:33 AM

namespace Jakar.AppLogger.Client;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static class AppLoggerExtensions
{
    public static IServiceCollection UseAppLogger( this IServiceCollection collection, string apiToken, string appName, AppVersion version, Uri baseHost )
    {
    #if __WINDOWS__ || __MACOS__ || __ANDROID__ || __IOS__
        var config = new AppLoggerConfig( version )
                     {
                         AppName = appName
                     };

    #elif __LINUX__
            var config = await AppLoggerIni.CreateAsync(appName, version, Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName));

    #elif __WINDOWS__
            var  config = await AppLoggerIni.CreateAsync(appName, version, Environment.ExpandEnvironmentVariables($"%APPDATA%/{appName}"));

    #elif __MACOS__
            var  config = await AppLoggerIni.CreateAsync(appName, version, $"~/Library/{appName}");

    #else
            var config = await AppLoggerIni.CreateAsync(appName, version, LocalDirectory.CurrentDirectory);

    #endif


        return collection.UseAppLogger( apiToken, baseHost, config );
    }
    public static IServiceCollection UseAppLogger( this IServiceCollection collection, string apiToken, Uri baseHost, LoggingSettings settings )
    {
        collection.AddOptions<AppLoggerOptions>()
                  .Configure( configure =>
                              {
                                  configure.APIToken = apiToken;
                                  configure.HostInfo = baseHost;
                                  configure.Config   = settings;
                              } );

        collection.AddSingleton<IAppLogger, AppLogger>();
        collection.AddHostedService( provider => provider.GetRequiredService<IAppLogger>() );
        return collection;
    }
    public static IServiceCollection UseAppLogger( this IServiceCollection collection, AppLoggerOptions options )
    {
        collection.AddSingleton( options );
        collection.AddSingleton<IAppLogger, AppLogger>();
        collection.AddHostedService( provider => provider.GetRequiredService<IAppLogger>() );
        return collection;
    }
}
