// Jakar.AppLogger :: Jakar.AppLogger.Client
// 09/09/2022  11:33 AM

namespace Jakar.AppLogger.Client;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static class AppLoggerExtensions
{
    public static IServiceCollection UseAppLogger( this IServiceCollection collection, Action<AppLoggerOptions> configure )
    {
        collection.AddOptions<AppLoggerOptions>()
                  .Configure( configure );

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
