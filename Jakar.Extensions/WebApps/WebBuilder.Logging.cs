namespace Jakar.Extensions.WebApps;


#nullable enable
#if NET6_0
public static partial class WebBuilder
{
    public static WebApplicationBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder ) where T : IAppName => builder.AddDefaultLogging<T>(builder.Environment.EnvironmentName == Environments.Development);
    public static WebApplicationBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder, bool isDevEnvironment ) where T : IAppName => builder.AddDefaultLogging(isDevEnvironment, typeof(T).Name);
    public static WebApplicationBuilder AddDefaultLogging( this WebApplicationBuilder builder, bool isDevEnvironment, string name )
    {
        builder.Logging.ClearProviders();

    #if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
    #else
        builder.Logging.SetMinimumLevel(isDevEnvironment
                                            ? LogLevel.Debug
                                            : LogLevel.Information);
    #endif

        builder.Logging.AddProvider(new DebugLoggerProvider());

        builder.Logging.AddSimpleConsole(options =>
                                         {
                                             options.ColorBehavior = LoggerColorBehavior.Enabled;
                                             options.SingleLine    = false;
                                             options.IncludeScopes = true;
                                         });


        if ( OperatingSystem.IsWindows() )
        {
            builder.Logging.AddProvider(new EventLogLoggerProvider(new EventLogSettings
                                                                   {
                                                                       SourceName  = name,
                                                                       LogName     = name,
                                                                       MachineName = GetMachineName(),
                                                                       Filter      = ( category, level ) => level > LogLevel.Information
                                                                   }));
        }
        else { builder.Logging.AddSystemdConsole(options => options.UseUtcTimestamp = true); }

        return builder;
    }

    public static string GetMachineName()
    {
        try { return Environment.MachineName; }
        catch ( InvalidOperationException ) { return Dns.GetHostName(); }
    }
}
#endif
