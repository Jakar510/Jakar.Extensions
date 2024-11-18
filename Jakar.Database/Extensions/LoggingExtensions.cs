// Jakar.Extensions :: Jakar.Database
// 05/22/2023  11:24 AM

namespace Jakar.Database;


public static class LoggingExtensions
{
    public const string CONNECTION_STRINGS = "ConnectionStrings";
    public const string DEFAULT            = "Default";


    public static string ConnectionString( this IConfiguration configuration, string name = DEFAULT ) =>
        configuration.GetSection( CONNECTION_STRINGS ).GetValue<string>( name ) ?? throw new InvalidOperationException( $"{CONNECTION_STRINGS}::{DEFAULT} is not found" );
    public static string ConnectionString( this IServiceProvider provider,      string name = DEFAULT ) => provider.GetRequiredService<IConfiguration>().ConnectionString( name );
    public static string ConnectionString( this WebApplication   configuration, string name = DEFAULT ) => configuration.Services.ConnectionString( name );


    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, string[]                               args )                                             => builder.Configuration.AddCommandLine( args );
    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, string[]                               args, IDictionary<string, string> switchMappings ) => builder.Configuration.AddCommandLine( args, switchMappings );
    public static IConfigurationBuilder AddCommandLine( this          WebApplicationBuilder builder, Action<CommandLineConfigurationSource> configureSource ) => builder.Configuration.AddCommandLine( configureSource );
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder )                => builder.Configuration.AddEnvironmentVariables();
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder, string prefix ) => builder.Configuration.AddEnvironmentVariables( prefix );
    public static IConfigurationBuilder AddEnvironmentVariables( this WebApplicationBuilder builder, params ReadOnlySpan<string> prefix )
    {
        foreach ( string s in prefix ) { builder.Configuration.AddEnvironmentVariables( s ); }

        return builder.Configuration;
    }


    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string                         path )                                                          => builder.Configuration.AddIniFile( path );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string                         path,     bool   optional )                                     => builder.Configuration.AddIniFile( path,     optional );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, string                         path,     bool   optional, bool reloadOnChange )                => builder.Configuration.AddIniFile( path,     optional, reloadOnChange );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, IFileProvider                  provider, string path,     bool optional, bool reloadOnChange ) => builder.Configuration.AddIniFile( provider, path,     optional, reloadOnChange );
    public static IConfigurationBuilder AddIniFile( this   WebApplicationBuilder builder, Action<IniConfigurationSource> configureSource ) => builder.Configuration.AddIniFile( configureSource );
    public static IConfigurationBuilder AddIniStream( this WebApplicationBuilder builder, Stream                         stream )          => builder.Configuration.AddIniStream( stream );


    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string                          path )                                                          => builder.Configuration.AddJsonFile( path );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string                          path,     bool   optional )                                     => builder.Configuration.AddJsonFile( path,     optional );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, string                          path,     bool   optional, bool reloadOnChange )                => builder.Configuration.AddJsonFile( path,     optional, reloadOnChange );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, IFileProvider                   provider, string path,     bool optional, bool reloadOnChange ) => builder.Configuration.AddJsonFile( provider, path,     optional, reloadOnChange );
    public static IConfigurationBuilder AddJsonFile( this   WebApplicationBuilder builder, Action<JsonConfigurationSource> configureSource ) => builder.Configuration.AddJsonFile( configureSource );
    public static IConfigurationBuilder AddJsonStream( this WebApplicationBuilder builder, Stream                          stream )          => builder.Configuration.AddJsonStream( stream );


    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder )
        where T : class => builder.AddDefaultLogging<T>( builder.Environment.EnvironmentName == Environments.Development );
    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder, bool isDevEnvironment )
        where T : class => builder.AddDefaultLogging<T>( isDevEnvironment
                                                             ? LogLevel.Trace
                                                             : LogLevel.Information );
    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder, in LogLevel minimumLevel )
        where T : class => builder.AddDefaultLogging( minimumLevel, typeof(T).Name );


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
