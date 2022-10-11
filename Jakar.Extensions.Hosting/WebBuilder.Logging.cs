namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder ) where T : class => builder.AddDefaultLogging<T>( builder.Environment.EnvironmentName == Environments.Development );
    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder, bool isDevEnvironment ) where T : class => builder.AddDefaultLogging<T>( isDevEnvironment
                                                                                                                                                                         ? LogLevel.Trace
                                                                                                                                                                         : LogLevel.Information );
    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder, in LogLevel minimumLevel ) where T : class => builder.AddDefaultLogging( minimumLevel, typeof(T).Name );


    public static ILoggingBuilder AddDefaultLogging( this WebApplicationBuilder builder, in LogLevel minimumLevel, in string name )
    {
        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel( minimumLevel );
        builder.Logging.AddProvider( new DebugLoggerProvider() );

        builder.Logging.AddSimpleConsole( options =>
                                          {
                                              options.ColorBehavior = LoggerColorBehavior.Enabled;
                                              options.SingleLine    = false;
                                              options.IncludeScopes = true;
                                          } );


        if (OperatingSystem.IsWindows())
        {
            builder.Logging.AddProvider( new EventLogLoggerProvider( new EventLogSettings
                                                                     {
                                                                         SourceName  = name,
                                                                         LogName     = name,
                                                                         MachineName = GetMachineName(),
                                                                         Filter      = ( category, level ) => level > LogLevel.Information
                                                                     } ) );
        }
        else { builder.Logging.AddSystemdConsole( options => options.UseUtcTimestamp = true ); }


        return builder.Logging;
    }


    public static string GetMachineName()
    {
        try { return Environment.MachineName; }
        catch (InvalidOperationException) { return Dns.GetHostName(); }
    }
}
