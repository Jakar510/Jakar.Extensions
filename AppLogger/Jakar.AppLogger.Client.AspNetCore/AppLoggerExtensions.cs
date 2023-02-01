using Jakar.AppLogger.Common;
using Jakar.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;



namespace Jakar.AppLogger.Client.AspNetCore;


public static class AppLoggerExtensions
{
    public static IServiceCollection UseAppLogger( this IServiceCollection collection, string apiToken, AppVersion version, string deviceID, string appName, Uri host )
    {
        return collection.UseAppLogger( options =>
                                        {
                                            options.Config   = AppLoggerIni.Create( version, deviceID, appName );
                                            options.APIToken = apiToken;
                                            options.HostInfo = host;
                                        } );
    }
    public static IServiceCollection UseAppLogger( this IServiceCollection collection, Action<AppLoggerOptions> options )
    {
        collection.AddOptions<AppLoggerOptions>()
                  .Configure( options );

        collection.AddSingleton<IAppLogger, Common.AppLogger>();
        collection.AddHostedService( provider => provider.GetRequiredService<IAppLogger>() );
        collection.AddSingleton<ILoggerProvider>( provider => provider.GetRequiredService<IAppLogger>() );
        return collection;
    }
}
