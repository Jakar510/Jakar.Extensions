// Jakar.Extensions :: Jakar.Database
// 05/22/2023  11:24 AM

namespace Jakar.Database;


public static class LoggingExtensions
{
    public const string CONNECTION_STRINGS = "ConnectionStrings";
    public const string DEFAULT            = "Default";


    public static string ConnectionString( this IConfiguration   configuration, string name = DEFAULT ) { return configuration.GetSection( CONNECTION_STRINGS ).GetValue<string>( name ) ?? throw new InvalidOperationException( $"{CONNECTION_STRINGS}::{DEFAULT} is not found" ); }
    public static string ConnectionString( this IServiceProvider provider,      string name = DEFAULT ) { return provider.GetRequiredService<IConfiguration>().ConnectionString( name ); }
    public static string ConnectionString( this WebApplication   configuration, string name = DEFAULT ) { return configuration.Services.ConnectionString( name ); }


    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, string[]                               args )                                             { return builder.Configuration.AddCommandLine( args ); }
    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, string[]                               args, IDictionary<string, string> switchMappings ) { return builder.Configuration.AddCommandLine( args, switchMappings ); }
    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, Action<CommandLineConfigurationSource> configureSource ) { return builder.Configuration.AddCommandLine( configureSource ); }
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder )                { return builder.Configuration.AddEnvironmentVariables(); }
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder, string prefix ) { return builder.Configuration.AddEnvironmentVariables( prefix ); }
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder, params ReadOnlySpan<string> prefix )
    {
        foreach ( string s in prefix ) { builder.Configuration.AddEnvironmentVariables( s ); }

        return builder.Configuration;
    }


    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string                         path )                                                          { return builder.Configuration.AddIniFile( path ); }
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string                         path,     bool   optional )                                     { return builder.Configuration.AddIniFile( path,     optional ); }
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string                         path,     bool   optional, bool reloadOnChange )                { return builder.Configuration.AddIniFile( path,     optional, reloadOnChange ); }
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, IFileProvider                  provider, string path,     bool optional, bool reloadOnChange ) { return builder.Configuration.AddIniFile( provider, path,     optional, reloadOnChange ); }
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, Action<IniConfigurationSource> configureSource ) { return builder.Configuration.AddIniFile( configureSource ); }
    public static IConfigurationBuilder AddIniStream( this WebApplicationBuilder builder, Stream                         stream )          { return builder.Configuration.AddIniStream( stream ); }


    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string                          path )                                                          { return builder.Configuration.AddJsonFile( path ); }
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string                          path,     bool   optional )                                     { return builder.Configuration.AddJsonFile( path,     optional ); }
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string                          path,     bool   optional, bool reloadOnChange )                { return builder.Configuration.AddJsonFile( path,     optional, reloadOnChange ); }
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, IFileProvider                   provider, string path,     bool optional, bool reloadOnChange ) { return builder.Configuration.AddJsonFile( provider, path,     optional, reloadOnChange ); }
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, Action<JsonConfigurationSource> configureSource ) { return builder.Configuration.AddJsonFile( configureSource ); }
    public static IConfigurationBuilder AddJsonStream( this WebApplicationBuilder builder, Stream                          stream )          { return builder.Configuration.AddJsonStream( stream ); }


    public static ILoggingBuilder AddDefaultLogging<TValue>( this WebApplicationBuilder builder )
        where TValue : class
    {
        return builder.AddDefaultLogging<TValue>( builder.Environment.EnvironmentName == Environments.Development );
    }
    public static ILoggingBuilder AddDefaultLogging<TValue>( this WebApplicationBuilder builder, bool isDevEnvironment )
        where TValue : class
    {
        return builder.AddDefaultLogging<TValue>( isDevEnvironment
                                                      ? LogLevel.Trace
                                                      : LogLevel.Information );
    }
    public static ILoggingBuilder AddDefaultLogging<TValue>( this WebApplicationBuilder builder, in LogLevel minimumLevel )
        where TValue : class
    {
        return builder.AddDefaultLogging( minimumLevel, typeof(TValue).Name );
    }


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


        if ( OperatingSystem.IsWindows() )
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
    #pragma warning disable RS1035
        try { return Environment.MachineName; }
        catch ( InvalidOperationException ) { return Dns.GetHostName(); }
    #pragma warning restore RS1035
    }
}
