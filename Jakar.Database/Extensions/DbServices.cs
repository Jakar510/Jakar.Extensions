// Jakar.Extensions :: Jakar.Database
// 1/10/2024  14:10


using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static class DbServices
{
    public const string OTEL_EXPORTER_OTLP_ENDPOINT        = nameof(OTEL_EXPORTER_OTLP_ENDPOINT);
    public const string AUTHENTICATION_SCHEME              = JwtBearerDefaults.AuthenticationScheme;
    public const string AUTHENTICATION_SCHEME_DISPLAY_NAME = $"Jwt.{AUTHENTICATION_SCHEME}";


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValid<TClass>( this TClass value )
        where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
    {
        return value.ID.IsValid();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNotValid<TClass>( this TClass value )
        where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
    {
        return value.IsValid() is false;
    }


    public static string GetFullName( this Type type ) { return type.AssemblyQualifiedName ?? type.FullName ?? type.Name; }


    public static IHostApplicationBuilder OpenTelemetry( this IHostApplicationBuilder builder )
    {
        builder.Logging.AddOpenTelemetry( static x =>
                                          {
                                              x.IncludeScopes           = true;
                                              x.IncludeFormattedMessage = true;
                                          } );

        builder.Services.AddOpenTelemetry()
               .WithMetrics( static x => { x.AddRuntimeInstrumentation().AddMeter( "Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http", "WeatherApp.Api" ); } )
               .WithTracing( x =>
                             {
                                 if ( builder.Environment.IsDevelopment() ) { x.SetSampler<AlwaysOnSampler>(); }

                                 x.AddAspNetCoreInstrumentation().AddGrpcClientInstrumentation().AddHttpClientInstrumentation();
                             } );

        return builder.AddOpenTelemetryExporters();
    }
    public static IHostApplicationBuilder AddOpenTelemetryExporters( this IHostApplicationBuilder builder )
    {
        bool useOtlpExporter = string.IsNullOrWhiteSpace( builder.Configuration[OTEL_EXPORTER_OTLP_ENDPOINT] ) is false;

        if ( useOtlpExporter )
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>( static logging => logging.AddOtlpExporter() );
            builder.Services.ConfigureOpenTelemetryMeterProvider( static metrics => metrics.AddOtlpExporter() );
            builder.Services.ConfigureOpenTelemetryTracerProvider( static tracing => tracing.AddOtlpExporter() );
        }

        builder.Services.AddOpenTelemetry().WithMetrics( static x => x.AddPrometheusExporter() );

        return builder;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static LogLevel GetLogLevel( this bool isDevEnvironment )
    {
        return isDevEnvironment
                   ? LogLevel.Trace
                   : LogLevel.Information;
    }
    public static ILoggingBuilder AddDefaultLogging<TApp>( this WebApplicationBuilder builder )
        where TApp : IAppName
    {
        return AddDefaultLogging<TApp>( builder.Logging, builder.Environment.IsDevelopment() );
    }
    public static ILoggingBuilder AddDefaultLogging<TApp>( this ILoggingBuilder builder, bool isDevEnvironment )
        where TApp : IAppName
    {
        return AddDefaultLogging<TApp>( builder, isDevEnvironment.GetLogLevel() );
    }
    public static ILoggingBuilder AddDefaultLogging<TApp>( this ILoggingBuilder builder, in LogLevel minimumLevel )
        where TApp : IAppName
    {
        return AddDefaultLogging( builder, minimumLevel, TApp.AppName );
    }
    public static ILoggingBuilder AddDefaultLogging( this ILoggingBuilder builder, in LogLevel minimumLevel, in string name )
    {
        builder.ClearProviders();
        builder.SetMinimumLevel( minimumLevel );
        builder.AddProvider( new DebugLoggerProvider() );

        builder.AddSimpleConsole( static options =>
                                  {
                                      options.ColorBehavior = LoggerColorBehavior.Enabled;
                                      options.SingleLine    = false;
                                      options.IncludeScopes = true;
                                  } );


        if ( OperatingSystem.IsWindows() ) { builder.AddProvider( name.GetEventLogLoggerProvider() ); }
        else if ( OperatingSystem.IsLinux() ) { builder.AddSystemdConsole( static options => options.UseUtcTimestamp = true ); }

        return builder.AddFluentMigratorLogger();
    }


    [SupportedOSPlatform( "Windows" )]
    public static EventLogLoggerProvider GetEventLogLoggerProvider( this string name )
    {
        return GetEventLogLoggerProvider( name, Filter );
        static bool Filter( string category, LogLevel level ) { return level > LogLevel.Information; }
    }
    [SupportedOSPlatform( "Windows" )]
    public static EventLogLoggerProvider GetEventLogLoggerProvider( this string name, Func<string, LogLevel, bool> filter )
    {
        return new EventLogLoggerProvider(new EventLogSettings
                                          {
                                              SourceName  = name,
                                              LogName     = name,
                                              MachineName = GetMachineName(),
                                              Filter      = filter
                                          });
    }
    public static string GetMachineName()
    {
    #pragma warning disable RS1035
        try { return Environment.MachineName; }
        catch ( InvalidOperationException ) { return Dns.GetHostName(); }
    #pragma warning restore RS1035
    }


    public static IHealthChecksBuilder AddHealthCheck<TValue>( this IServiceCollection services )
        where TValue : IHealthCheck
    {
        return services.AddHealthCheck( HealthChecks.Create<TValue>() );
    }
    public static IHealthChecksBuilder AddHealthCheck( this IServiceCollection services, HealthCheckRegistration registration ) { return services.AddHealthChecks().Add( registration ); }


    public static FluentMigratorConsoleLoggerProvider GetFluentMigratorConsoleLoggerProvider( this FluentMigratorLoggerOptions options )                                       { return new FluentMigratorConsoleLoggerProvider(new OptionsWrapper<FluentMigratorLoggerOptions>( options )); }
    public static ILoggingBuilder                     AddFluentMigratorLogger( this                ILoggingBuilder             services, FluentMigratorLoggerOptions options ) { return services.AddProvider( options.GetFluentMigratorConsoleLoggerProvider() ); }
    public static ILoggingBuilder AddFluentMigratorLogger( this ILoggingBuilder services, bool showSql = true, bool showElapsedTime = true )
    {
        return services.AddFluentMigratorLogger( new FluentMigratorLoggerOptions
                                                 {
                                                     ShowElapsedTime = showElapsedTime,
                                                     ShowSql         = showSql
                                                 } );
    }


    public static void MigrationsMsSql( this IMigrationRunnerBuilder migration )
    {
        migration.AddSqlServer2016();
        DbOptions.GetConnectionString( migration );
        migration.ScanIn( typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void MigrationsMsSql<TValue>( this IMigrationRunnerBuilder migration )
        where TValue : IAppName
    {
        migration.AddSqlServer2016();
        DbOptions.GetConnectionString( migration );
        migration.ScanIn( typeof(TValue).Assembly, typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void MigrationsPostgres( this IMigrationRunnerBuilder migration )
    {
        migration.AddPostgres();
        DbOptions.GetConnectionString( migration );
        migration.ScanIn( typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void MigrationsPostgres<TValue>( this IMigrationRunnerBuilder migration )
        where TValue : IAppName
    {
        migration.AddPostgres();
        DbOptions.GetConnectionString( migration );
        migration.ScanIn( typeof(TValue).Assembly, typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/46775832/9530917"> Using ASP.NET Identity in an ASP.NET Core MVC application without Entity Framework and Migrations </see>
    ///     <para>
    ///         <see cref="AUTHENTICATION_SCHEME"/>
    ///     </para>
    /// </summary>
    /// <returns> </returns>
    public static IdentityBuilder AddIdentityServices( this IServiceCollection services, Action<IdentityOptions>? options = null )
    {
        services.AddOptions<IdentityOptions>().Configure( options ?? IdentityOptions );

        RoleStore.Register( services );
        UserStore.Register( services );

        return services.AddIdentity<UserRecord, RoleRecord>().AddUserStore<UserStore>().AddUserManager<UserRecordManager>().AddRoleStore<RoleStore>().AddRoleManager<RoleManager>().AddSignInManager<SignInManager>().AddUserValidator<UserValidator>().AddRoleValidator<RoleValidator>().AddPasswordValidator<UserPasswordValidator>().AddDefaultTokenProviders().AddTokenProvider<TokenProvider>( nameof(TokenProvider) );

        static void IdentityOptions( IdentityOptions _ ) { }
    }


    public static IServiceCollection AddOptions<TValue>( this IServiceCollection services, Action<TValue> options )
        where TValue : class, IOptions<TValue>
    {
        return services.AddOptions( options, Options.DefaultName );
    }
    public static IServiceCollection AddOptions<TValue>( this IServiceCollection services, Action<TValue> options, string name )
        where TValue : class, IOptions<TValue>
    {
        services.AddSingleton<TValue>();
        services.Configure( name, options );
        services.AddTransient<IOptions<TValue>>( static provider => provider.GetRequiredService<TValue>() );
        return services;
    }


    public static JwtBearerOptions GetJwtBearerOptions( this IServiceProvider provider )
    {
        JwtBearerOptions? bearer = provider.GetService<JwtBearerOptions>();
        if ( bearer is not null ) { return bearer; }

        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        DbOptions      options       = provider.GetRequiredService<DbOptions>();

        JwtBearerOptions jwt = new()
                               {
                                   Audience                  = options.TokenAudience,
                                   ClaimsIssuer              = options.TokenIssuer,
                                   TokenValidationParameters = configuration.GetTokenValidationParameters( options )
                               };

        jwt.TokenHandlers.TryAdd( DbTokenHandler.Instance );
        return jwt;
    }


    public static IServiceCollection AddDataProtection( this IServiceCollection services )
    {
        DataProtectionServiceCollectionExtensions.AddDataProtection( services );
        ProtectedDataProvider.Register( services );
        return services;
    }
    public static IServiceCollection AddEmailer( this IServiceCollection services ) { return services.AddEmailer( static options => { } ); }
    public static IServiceCollection AddEmailer( this IServiceCollection services, Action<Emailer.Options> options )
    {
        services.AddOptions( options );
        services.AddScoped<Emailer>();
        return services;
    }


    public static IServiceCollection AddPasswordValidator( this IServiceCollection services ) { return services.AddPasswordValidator( static requirements => { } ); }
    public static IServiceCollection AddPasswordValidator( this IServiceCollection services, Action<PasswordRequirements> options )
    {
        services.AddOptions( options );
        services.AddScoped<IPasswordValidator<UserRecord>, UserPasswordValidator>();
        return services;
    }


    public static IServiceCollection AddTokenizer( this IServiceCollection services ) { return services.AddTokenizer<Tokenizer>(); }
    public static IServiceCollection AddTokenizer<TTokenizer>( this IServiceCollection services )
        where TTokenizer : class, ITokenService
    {
        services.AddScoped<ITokenService, TTokenizer>();
        return services;
    }


    public static AuthorizationBuilder RequireMultiFactorAuthentication( this AuthorizationBuilder builder ) { return builder.AddPolicy( nameof(RequireMfa), static policy => policy.Requirements.Add( RequireMfa.Instance ) ); }
}
