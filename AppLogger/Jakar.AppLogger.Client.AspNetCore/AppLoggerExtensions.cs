using Jakar.AppLogger.Common;
using Jakar.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;



namespace Jakar.AppLogger.Client.AspNetCore
{
    public static class AppLoggerExtensions
    {
        public static IServiceCollection UseAppLogger( this IServiceCollection collection, string apiToken, string appName, AppVersion version, Uri baseHost )
        {
        #if __WINDOWS__ || __MACOS__ || __ANDROID__ || __IOS__
        var config = new AppLoggerConfig
                     {
                         AppName = appName,
                         Version = version
                     };

        #elif __LINUX__
            var config = await AppLoggerIni.CreateAsync(appName, version, Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName));

        #elif __WINDOWS__
            var  config = await AppLoggerIni.CreateAsync(appName, version, Environment.ExpandEnvironmentVariables($"%APPDATA%/{appName}"));

        #elif __MACOS__
            var  config = await AppLoggerIni.CreateAsync(appName, version, $"~/Library/{appName}");

        #else
            var config = AppLoggerIni.Create(appName, version, LocalDirectory.CurrentDirectory);

        #endif


            return collection.UseAppLogger(new AppLoggerOptions(apiToken, baseHost, config));
        }
        public static IServiceCollection UseAppLogger( this IServiceCollection collection, AppLoggerOptions options )
        {
            collection.AddSingleton(options);
            collection.AddSingleton<IAppLogger, AppLogger>();
            collection.AddHostedService(provider => provider.GetRequiredService<IAppLogger>());
            return collection;
        }
        public static WebApplicationBuilder UseAppLogger( this WebApplicationBuilder builder, string apiToken, string appName, AppVersion version, Uri baseHost )
        {
            builder.Services.UseAppLogger(apiToken, appName, version, baseHost);
            return builder;
        }
        public static WebApplicationBuilder UseAppLogger( this WebApplicationBuilder builder, AppLoggerOptions options )
        {
            builder.Services.UseAppLogger(options);
            return builder;
        }
    }
}
