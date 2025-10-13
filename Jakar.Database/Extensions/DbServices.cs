// Jakar.Extensions :: Jakar.Database
// 1/10/2024  14:10


namespace Jakar.Database;


[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static class DbServices
{
    public const string AUTHENTICATION_SCHEME              = JwtBearerDefaults.AuthenticationScheme;
    public const string AUTHENTICATION_SCHEME_DISPLAY_NAME = $"Jwt.{AUTHENTICATION_SCHEME}";
    public const string OTEL_EXPORTER_OTLP_ENDPOINT        = nameof(OTEL_EXPORTER_OTLP_ENDPOINT);


    public static bool IsValid<TSelf>( this TSelf value )
        where TSelf : class, ITableRecord<TSelf> =>
        value.ID.IsValid();


    public static bool IsNotValid<TSelf>( this TSelf value )
        where TSelf : class, ITableRecord<TSelf> =>
        !value.IsValid();


    public static string GetFullName( this Type type ) => type.AssemblyQualifiedName ?? type.FullName ?? type.Name;


    public static IHostApplicationBuilder OpenTelemetry( this IHostApplicationBuilder builder )
    {
        builder.Logging.AddOpenTelemetry(static x =>
                                         {
                                             x.IncludeScopes           = true;
                                             x.IncludeFormattedMessage = true;
                                         });

        builder.Services.AddOpenTelemetry()
               .WithMetrics(static x =>
                            {
                                x.AddRuntimeInstrumentation()
                                 .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http", "WeatherApp.Api");
                            })
               .WithTracing(x =>
                            {
                                if ( builder.Environment.IsDevelopment() ) { x.SetSampler<AlwaysOnSampler>(); }

                                x.AddAspNetCoreInstrumentation()
                                 .AddGrpcClientInstrumentation()
                                 .AddHttpClientInstrumentation();
                            });

        return builder.AddOpenTelemetryExporters();
    }
    public static IHostApplicationBuilder AddOpenTelemetryExporters( this IHostApplicationBuilder builder )
    {
        bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration[OTEL_EXPORTER_OTLP_ENDPOINT]);

        if ( useOtlpExporter )
        {
            builder.Services.Configure<OpenTelemetryLoggerOptions>(static logging => logging.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryMeterProvider(static metrics => metrics.AddOtlpExporter());
            builder.Services.ConfigureOpenTelemetryTracerProvider(static tracing => tracing.AddOtlpExporter());
        }

        builder.Services.AddOpenTelemetry()
               .WithMetrics(static x => x.AddPrometheusExporter());

        return builder;
    }


    private static LogLevel GetLogLevel( this bool isDevEnvironment ) =>
        isDevEnvironment
            ? LogLevel.Trace
            : LogLevel.Information;
    public static ILoggingBuilder AddDefaultLogging<TApp>( this WebApplicationBuilder builder )
        where TApp : IAppName =>
        AddDefaultLogging<TApp>(builder.Logging, builder.Environment.IsDevelopment());
    public static ILoggingBuilder AddDefaultLogging<TApp>( this ILoggingBuilder builder, bool isDevEnvironment )
        where TApp : IAppName =>
        AddDefaultLogging<TApp>(builder, isDevEnvironment.GetLogLevel());
    public static ILoggingBuilder AddDefaultLogging<TApp>( this ILoggingBuilder builder, in LogLevel minimumLevel )
        where TApp : IAppName =>
        AddDefaultLogging(builder, minimumLevel, TApp.AppName);
    public static ILoggingBuilder AddDefaultLogging( this ILoggingBuilder builder, in LogLevel minimumLevel, in string name )
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(minimumLevel);
        builder.AddProvider(new DebugLoggerProvider());

        builder.AddSimpleConsole(static options =>
                                 {
                                     options.ColorBehavior = LoggerColorBehavior.Enabled;
                                     options.SingleLine    = false;
                                     options.IncludeScopes = true;
                                 });


        if ( OperatingSystem.IsWindows() ) { builder.AddProvider(name.GetEventLogLoggerProvider()); }
        else if ( OperatingSystem.IsLinux() ) { builder.AddSystemdConsole(static options => options.UseUtcTimestamp = true); }

        return builder.AddFluentMigratorLogger();
    }


    [SupportedOSPlatform("Windows")] public static EventLogLoggerProvider GetEventLogLoggerProvider( this string name )
    {
        return GetEventLogLoggerProvider(name, filter);
        static bool filter( string category, LogLevel level ) => level > LogLevel.Information;
    }
    [SupportedOSPlatform("Windows")] public static EventLogLoggerProvider GetEventLogLoggerProvider( this string name, Func<string, LogLevel, bool> filter ) =>
        new(new EventLogSettings
            {
                SourceName  = name,
                LogName     = name,
                MachineName = GetMachineName(),
                Filter      = filter
            });
    public static string GetMachineName()
    {
    #pragma warning disable RS1035
        try { return Environment.MachineName; }
        catch ( InvalidOperationException ) { return Dns.GetHostName(); }
    #pragma warning restore RS1035
    }


    public static IHealthChecksBuilder AddHealthCheck<TValue>( this IServiceCollection services )
        where TValue : IHealthCheck => services.AddHealthCheck(HealthChecks.Create<TValue>());
    public static IHealthChecksBuilder AddHealthCheck( this IServiceCollection services, HealthCheckRegistration registration ) => services.AddHealthChecks()
                                                                                                                                           .Add(registration);


    public static FluentMigratorConsoleLoggerProvider GetFluentMigratorConsoleLoggerProvider( this FluentMigratorLoggerOptions options )                                       => new(new OptionsWrapper<FluentMigratorLoggerOptions>(options));
    public static ILoggingBuilder                     AddFluentMigratorLogger( this                ILoggingBuilder             services, FluentMigratorLoggerOptions options ) => services.AddProvider(options.GetFluentMigratorConsoleLoggerProvider());
    public static ILoggingBuilder AddFluentMigratorLogger( this ILoggingBuilder services, bool showSql = true, bool showElapsedTime = true ) =>
        services.AddFluentMigratorLogger(new FluentMigratorLoggerOptions
                                         {
                                             ShowElapsedTime = showElapsedTime,
                                             ShowSql         = showSql
                                         });


    public static void MigrationsMsSql( this IMigrationRunnerBuilder migration )
    {
        migration.AddSqlServer2016();
        DbOptions.GetConnectionString(migration);

        migration.ScanIn(typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly())
                 .For.All();
    }
    public static void MigrationsMsSql<TApp>( this IMigrationRunnerBuilder migration )
        where TApp : IAppName
    {
        migration.AddSqlServer2016();
        DbOptions.GetConnectionString(migration);

        migration.ScanIn(typeof(TApp).Assembly, typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly())
                 .For.All();
    }
    public static void MigrationsPostgres( this IMigrationRunnerBuilder migration )
    {
        migration.AddPostgres();
        DbOptions.GetConnectionString(migration);

        migration.ScanIn(typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly())
                 .For.All();
    }
    public static void MigrationsPostgres<TApp>( this IMigrationRunnerBuilder migration )
        where TApp : IAppName
    {
        migration.AddPostgres();
        DbOptions.GetConnectionString(migration);

        migration.ScanIn(typeof(TApp).Assembly, typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly())
                 .For.All();
    }


    public static WebApplicationBuilder AddDatabase<TDatabase>( this WebApplicationBuilder builder, DbOptions options )
        where TDatabase : Database => builder.AddDatabase<TDatabase, UserStore, UserManager, RoleStore, RoleManager, SignInManager, UserValidator, RoleValidator, TokenProvider, UserPasswordValidator, DataProtectorTokenProvider, EmailTokenProvider, PhoneNumberTokenProvider, OtpAuthenticatorTokenProvider>(options);
    public static WebApplicationBuilder AddDatabase<TDatabase, TUserStore, TUserManager, TRoleStore, TRoleManager, TSignInManager, TUserValidator, TRoleValidator, TTokenProvider, TUserPasswordValidator, TDataProtectorTokenProvider, TEmailTokenProvider, TPhoneNumberTokenProvider, TAuthenticatorTokenProvider>( this WebApplicationBuilder builder, DbOptions options )
        where TDatabase : Database
        where TUserStore : UserStore
        where TUserManager : UserManager
        where TRoleStore : RoleStore
        where TRoleManager : RoleManager
        where TSignInManager : SignInManager
        where TUserValidator : UserValidator
        where TRoleValidator : RoleValidator
        where TTokenProvider : TokenProvider
        where TUserPasswordValidator : UserPasswordValidator
        where TDataProtectorTokenProvider : DataProtectorTokenProvider
        where TEmailTokenProvider : EmailTokenProvider
        where TPhoneNumberTokenProvider : PhoneNumberTokenProvider
        where TAuthenticatorTokenProvider : OtpAuthenticatorTokenProvider
    {
        builder.Services.AddSingleton(options);
        builder.Services.AddTransient<IOptions<DbOptions>>(static provider => provider.GetRequiredService<DbOptions>());

        builder.AddOpenTelemetry<TestDatabase>(tracerOtlpExporter => { }, meterOtlpExporter => { });

        builder.AddSerilog(options.LoggerOptions, Validate.ThrowIfNull(options.TelemetrySource), options.SeqConfig, out Logger logger);
        options.Serilogger = logger;

        options.ConfigureFusionCache(builder.Services.AddFusionCache());

        builder.Services.AddSingleton<TDatabase>();
        builder.Services.AddTransient<Database>(static provider => provider.GetRequiredService<TDatabase>());
        builder.Services.AddHealthCheck<TDatabase>();

        builder.Services.AddIdentityServices<TUserStore, TUserManager, TRoleStore, TRoleManager, TSignInManager, TUserValidator, TRoleValidator, TTokenProvider, TUserPasswordValidator, TDataProtectorTokenProvider, TEmailTokenProvider, TPhoneNumberTokenProvider, TAuthenticatorTokenProvider>(options);

        builder.Services.AddDataProtection();

        builder.Services.AddPasswordValidator();

        builder.Services.AddInMemoryTokenCaches();

        options.AddAuthentication(builder);

        builder.Services.AddAuthorizationBuilder()
               .RequireMultiFactorAuthentication();

        return builder;
    }


    public static void AddIdentityServices( this IServiceCollection services, DbOptions options ) => services.AddIdentityServices<UserStore, UserManager, RoleStore, RoleManager, SignInManager, UserValidator, RoleValidator, TokenProvider, UserPasswordValidator, DataProtectorTokenProvider, EmailTokenProvider, PhoneNumberTokenProvider, OtpAuthenticatorTokenProvider>(options);


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/46775832/9530917"> Using ASP.NET Identity in an ASP.NET Core MVC application without Entity Framework and Migrations </see>
    ///     <para>
    ///         <see cref="AUTHENTICATION_SCHEME"/>
    ///     </para>
    /// </summary>
    public static IdentityBuilder AddIdentityServices<TUserStore, TUserManager, TRoleStore, TRoleManager, TSignInManager, TUserValidator, TRoleValidator, TTokenProvider, TUserPasswordValidator, TDataProtectorTokenProvider, TEmailTokenProvider, TPhoneNumberTokenProvider, TAuthenticatorTokenProvider>( this IServiceCollection services, DbOptions options )
        where TUserStore : UserStore
        where TUserManager : UserManager
        where TRoleStore : RoleStore
        where TRoleManager : RoleManager
        where TSignInManager : SignInManager
        where TUserValidator : UserValidator
        where TRoleValidator : RoleValidator
        where TTokenProvider : TokenProvider
        where TUserPasswordValidator : UserPasswordValidator
        where TDataProtectorTokenProvider : DataProtectorTokenProvider
        where TEmailTokenProvider : EmailTokenProvider
        where TPhoneNumberTokenProvider : PhoneNumberTokenProvider
        where TAuthenticatorTokenProvider : OtpAuthenticatorTokenProvider
    {
        services.AddOptions(options.ConfigureIdentityOptions);

        services.AddUserStore<TUserStore>();
        services.AddRoleStore<TRoleStore>();


        return services.AddIdentity<UserRecord, RoleRecord>()
                       .AddUserStore<TUserStore>()
                       .AddUserManager<TUserManager>()
                       .AddRoleStore<TRoleStore>()
                       .AddRoleManager<TRoleManager>()
                       .AddSignInManager<TSignInManager>()
                       .AddUserValidator<TUserValidator>()
                       .AddRoleValidator<TRoleValidator>()
                       .AddPasswordValidator<TUserPasswordValidator>()
                       .AddTokenProvider(TokenOptions.DefaultProvider,              typeof(TDataProtectorTokenProvider))
                       .AddTokenProvider(TokenOptions.DefaultEmailProvider,         typeof(TEmailTokenProvider))
                       .AddTokenProvider(TokenOptions.DefaultPhoneProvider,         typeof(TPhoneNumberTokenProvider))
                       .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, typeof(TAuthenticatorTokenProvider))
                       .AddTokenProvider<TTokenProvider>(nameof(TTokenProvider));
    }


    public static IServiceCollection AddRoleStore<TRoleStore>( this IServiceCollection services )
        where TRoleStore : RoleStore
    {
        services.AddScoped<TRoleStore>();
        services.AddTransient<IRoleStore<RoleRecord>>(static provider => provider.GetRequiredService<TRoleStore>());
        return services;
    }
    public static IServiceCollection AddUserStore<TUserStore>( this IServiceCollection services )
        where TUserStore : UserStore
    {
        services.AddScoped<TUserStore>()
                .AddTransient<IUserStore>(GetUserStore)
                .AddTransient<IUserStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserLoginStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserClaimStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserPasswordStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserSecurityStampStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserTwoFactorStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserEmailStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserLockoutStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserAuthenticatorKeyStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserTwoFactorRecoveryCodeStore<UserRecord>>(GetUserStore)
                .AddTransient<IUserPhoneNumberStore<UserRecord>>(GetUserStore);

        static TUserStore GetUserStore( IServiceProvider provider ) => provider.GetRequiredService<TUserStore>();
        return services;
    }


    public static IServiceCollection AddOptions<TValue>( this IServiceCollection services, Action<TValue> options, string? name = null )
        where TValue : class
    {
        services.AddSingleton<TValue>();
        services.Configure(name ?? Options.DefaultName, options);
        services.AddTransient(getValue);
        return services;

        static IOptions<TValue> getValue( IServiceProvider provider )
        {
            TValue value = provider.GetRequiredService<TValue>();
            return value as IOptions<TValue> ?? Options.Create(value);
        }
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
                                   TokenValidationParameters = configuration.GetTokenValidationParameters(options)
                               };

        jwt.TokenHandlers.TryAdd(DbTokenHandler.Instance);
        return jwt;
    }


    public static IServiceCollection AddDataProtection( this IServiceCollection services )
    {
        DataProtectionServiceCollectionExtensions.AddDataProtection(services);
        ProtectedDataProvider.Register(services);
        return services;
    }


    public static IServiceCollection AddEmailer<TEmailer>( this IServiceCollection services, Action<EmailerOptions> options )
        where TEmailer : Emailer
    {
        services.AddOptions(options);
        services.AddScoped<TEmailer>();
        return services;
    }


    public static IServiceCollection AddPasswordValidator( this IServiceCollection services )
    {
        services.AddTransient<IOptions<PasswordRequirements>>(static provider => PasswordRequirements.Current);
        services.AddScoped<UserPasswordValidator>();
        services.AddTransient<IPasswordValidator<UserRecord>>(static provider => provider.GetRequiredService<UserPasswordValidator>());
        return services;
    }


    public static AuthorizationBuilder RequireMultiFactorAuthentication( this AuthorizationBuilder builder ) { return builder.AddPolicy(nameof(RequireMfa), static policy => policy.Requirements.Add(RequireMfa.Instance)); }
}
