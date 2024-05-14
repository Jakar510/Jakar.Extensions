// Jakar.Extensions :: Jakar.Database
// 1/10/2024  14:10


using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static class DbServices
{
    public const string AUTHENTICATION_SCHEME              = JwtBearerDefaults.AuthenticationScheme;
    public const string AUTHENTICATION_SCHEME_DISPLAY_NAME = $"Jwt.{AUTHENTICATION_SCHEME}";


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValid<TRecord>( this TRecord value )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> => value.ID.IsValid();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNotValid<TRecord>( this TRecord value )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> => value.IsValid() is false;


    public static WebApplicationBuilder AddDefaultDbServices<T, TDatabase>( this WebApplicationBuilder           builder,
                                                                            DbTypeInstance                       dbType,
                                                                            SecuredStringResolverOptions         connectionStringResolver,
                                                                            Action<RedisCacheOptions>            configureRedis,
                                                                            AppVersion?                          version                         = default,
                                                                            Action<JwtBearerOptions>?            configureJwt                    = default,
                                                                            Action<AuthenticationOptions>?       configureAuth                   = default,
                                                                            Action<CookieAuthenticationOptions>? authCookie                      = default,
                                                                            Action<CookieAuthenticationOptions>? configureApplication            = default,
                                                                            Action<CookieAuthenticationOptions>? configureExternal               = default,
                                                                            Action<OpenIdConnectOptions>?        configureOpenIdConnect          = default,
                                                                            Action<MicrosoftAccountOptions>?     configureMicrosoftAccount       = default,
                                                                            Action<GoogleOptions>?               configureGoogle                 = default,
                                                                            Action<MemoryCacheOptions>?          configureMemoryCache            = default,
                                                                            string                               authenticationScheme            = AUTHENTICATION_SCHEME,
                                                                            string                               authenticationSchemeDisplayName = AUTHENTICATION_SCHEME_DISPLAY_NAME
    )
        where T : IAppName
        where TDatabase : Database
    {
        string appName = typeof(T).Name;
        builder.AddDefaultLogging<T>();

        DbOptions dbOptions = new()
                              {
                                  DbTypeInstance           = dbType,
                                  AppName                  = appName,
                                  TokenAudience            = appName,
                                  TokenIssuer              = appName,
                                  ConnectionStringResolver = connectionStringResolver,
                                  Version                  = version ?? AppVersion.FromAssembly<T>()
                              };

        TokenValidationParameters parameters = builder.GetTokenValidationParameters( dbOptions );

        builder.Services.AddStackExchangeRedisCache( configureRedis );

        builder.Services.AddDatabase<TDatabase>( dbOptions,
                                                 TableCacheOptions.Default,
                                                 configureRedis,
                                                 configureMemoryCache ?? ConfigureMemoryCache,
                                                 migration =>
                                                 {
                                                     switch ( dbType )
                                                     {
                                                         case DbTypeInstance.MsSql:
                                                             migration.ConfigureMigrationsMsSql<T>();
                                                             return;

                                                         case DbTypeInstance.Postgres:
                                                             migration.ConfigureMigrationsPostgres<T>();
                                                             return;

                                                         default: throw new OutOfRangeException( nameof(dbType), dbType );
                                                     }
                                                 } );

        builder.Services.AddDataProtection();
        builder.Services.AddIdentityServices();
        builder.Services.AddPasswordValidator();
        builder.Services.AddTokenizer();
        builder.Services.AddEmailer();

        AddAuthentication( builder.Services,
                           jwt =>
                           {
                               jwt.Audience                   = appName;
                               jwt.Authority                  = appName;
                               jwt.UseSecurityTokenValidators = true;
                               jwt.TokenValidationParameters  = parameters;
                               configureJwt?.Invoke( jwt );
                           },
                           configureAuth,
                           authCookie,
                           configureApplication,
                           configureExternal,
                           configureOpenIdConnect,
                           configureMicrosoftAccount,
                           configureGoogle,
                           authenticationScheme,
                           authenticationSchemeDisplayName );

        builder.Services.AddAuthorizationBuilder().RequireMultiFactorAuthentication();

        return builder;
    }


    private static void ConfigureMemoryCache( MemoryCacheOptions obj ) { }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static LogLevel GetLogLevel( this bool isDevEnvironment ) =>
        isDevEnvironment
            ? LogLevel.Trace
            : LogLevel.Information;
    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder )
        where T : IAppName => AddDefaultLogging<T>( builder.Logging, builder.Environment.IsDevelopment() );
    public static ILoggingBuilder AddDefaultLogging<T>( this ILoggingBuilder builder, bool isDevEnvironment )
        where T : IAppName => AddDefaultLogging<T>( builder, isDevEnvironment.GetLogLevel() );
    public static ILoggingBuilder AddDefaultLogging<T>( this ILoggingBuilder builder, in LogLevel minimumLevel )
        where T : IAppName => AddDefaultLogging( builder, minimumLevel, typeof(T).Name );
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
        static bool Filter( string category, LogLevel level ) => level > LogLevel.Information;
    }
    [SupportedOSPlatform( "Windows" )]
    public static EventLogLoggerProvider GetEventLogLoggerProvider( this string name, Func<string, LogLevel, bool> filter ) =>
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


    public static IHealthChecksBuilder AddHealthCheck<T>( this IServiceCollection collection )
        where T : IHealthCheck => collection.AddHealthCheck( HealthChecks.Create<T>() );
    public static IHealthChecksBuilder AddHealthCheck( this IServiceCollection collection, HealthCheckRegistration registration ) => collection.AddHealthChecks().Add( registration );


    public static FluentMigratorConsoleLoggerProvider GetFluentMigratorConsoleLoggerProvider( this FluentMigratorLoggerOptions options )                                         => new(new OptionsWrapper<FluentMigratorLoggerOptions>( options ));
    public static ILoggingBuilder                     AddFluentMigratorLogger( this                ILoggingBuilder             collection, FluentMigratorLoggerOptions options ) => collection.AddProvider( options.GetFluentMigratorConsoleLoggerProvider() );
    public static ILoggingBuilder AddFluentMigratorLogger( this ILoggingBuilder collection, bool showSql = true, bool showElapsedTime = true ) => collection.AddFluentMigratorLogger( new FluentMigratorLoggerOptions
                                                                                                                                                                                      {
                                                                                                                                                                                          ShowElapsedTime =
                                                                                                                                                                                              showElapsedTime,
                                                                                                                                                                                          ShowSql = showSql
                                                                                                                                                                                      } );


    public static void ConfigureMigrationsMsSql( this IMigrationRunnerBuilder configureMigration )
    {
        configureMigration.AddSqlServer2016();
        DbOptions.GetConnectionString( configureMigration );
        configureMigration.ScanIn( typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void ConfigureMigrationsMsSql<T>( this IMigrationRunnerBuilder configureMigration )
        where T : IAppName
    {
        configureMigration.AddSqlServer2016();
        DbOptions.GetConnectionString( configureMigration );
        configureMigration.ScanIn( typeof(T).Assembly, typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void ConfigureMigrationsPostgres( this IMigrationRunnerBuilder configureMigration )
    {
        configureMigration.AddPostgres();
        DbOptions.GetConnectionString( configureMigration );
        configureMigration.ScanIn( typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }
    public static void ConfigureMigrationsPostgres<T>( this IMigrationRunnerBuilder configureMigration )
        where T : IAppName
    {
        configureMigration.AddPostgres();
        DbOptions.GetConnectionString( configureMigration );
        configureMigration.ScanIn( typeof(T).Assembly, typeof(Database).Assembly, Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() ).For.All();
    }


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/46775832/9530917"> Using ASP.NET Identity in an ASP.NET Core MVC application without Entity Framework and Migrations </see>
    ///     <para>
    ///         <see cref="AUTHENTICATION_SCHEME"/>
    ///     </para>
    /// </summary>
    /// <returns> </returns>
    public static IdentityBuilder AddIdentityServices( this IServiceCollection collection, Action<IdentityOptions>? options = default )
    {
        collection.AddOptions<IdentityOptions>().Configure( options ?? ConfigureIdentityOptions );

        RoleStore.Register( collection );
        UserStore.Register( collection );

        return collection.AddIdentity<UserRecord, RoleRecord>()
                         .AddUserStore<UserStore>()
                         .AddUserManager<UserRecordManager>()
                         .AddRoleStore<RoleStore>()
                         .AddRoleManager<RoleManager>()
                         .AddSignInManager<SignInManager>()
                         .AddUserValidator<UserValidator>()
                         .AddRoleValidator<RoleValidator>()
                         .AddPasswordValidator<UserPasswordValidator>()
                         .AddDefaultTokenProviders()
                         .AddTokenProvider<TokenProvider>( nameof(TokenProvider) );

        static void ConfigureIdentityOptions( IdentityOptions _ ) { }
    }


    public static IServiceCollection AddDatabase<TDatabase>( this IServiceCollection collection, Action<DbOptions> configureDbOptions, Action<TableCacheOptions> configureTableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<MemoryCacheOptions> configureMemoryCache, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database => collection.AddDatabase<TDatabase, SqlCacheFactory, TableCacheFactory>( configureDbOptions, configureTableCacheOptions, configureRedis, configureMemoryCache, configureMigration );


    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory>( this IServiceCollection collection, Action<DbOptions> configureDbOptions, Action<TableCacheOptions> configureTableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<MemoryCacheOptions> configureMemoryCache, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory => collection.AddDatabase<TDatabase, TSqlCacheFactory, TableCacheFactory>( configureDbOptions, configureTableCacheOptions, configureRedis, configureMemoryCache, configureMigration );


    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory, TTableCacheFactory>( this IServiceCollection collection, Action<DbOptions> configureDbOptions, Action<TableCacheOptions> configureTableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<MemoryCacheOptions> configureMemoryCache, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory
        where TTableCacheFactory : class, ITableCache
    {
        DbOptions dbOptions = new();
        configureDbOptions( dbOptions );
        TableCacheOptions tableCacheOptions = new();
        configureTableCacheOptions( tableCacheOptions );
        return collection.AddDatabase<TDatabase, TSqlCacheFactory, TTableCacheFactory>( dbOptions, tableCacheOptions, configureRedis, configureMemoryCache, configureMigration );
    }


    public static IServiceCollection AddDatabase<TDatabase>( this IServiceCollection collection, DbOptions dbOptions, TableCacheOptions tableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<MemoryCacheOptions> configureMemoryCache, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database => collection.AddDatabase<TDatabase, SqlCacheFactory, TableCacheFactory>( dbOptions, tableCacheOptions, configureRedis, configureMemoryCache, configureMigration );


    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory>( this IServiceCollection collection, DbOptions dbOptions, TableCacheOptions tableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<MemoryCacheOptions> configureMemoryCache, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory => collection.AddDatabase<TDatabase, TSqlCacheFactory, TableCacheFactory>( dbOptions, tableCacheOptions, configureRedis, configureMemoryCache, configureMigration );


    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory, TTableCacheFactory>( this IServiceCollection collection, DbOptions dbOptions, TableCacheOptions tableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<MemoryCacheOptions> configureMemoryCache, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory
        where TTableCacheFactory : class, ITableCache
    {
        collection.AddInMemoryTokenCaches();

        collection.AddSingleton( dbOptions );
        collection.AddTransient<IOptions<DbOptions>>( static provider => provider.GetRequiredService<DbOptions>() );

        collection.AddSingleton( tableCacheOptions );
        collection.AddTransient<IOptions<TableCacheOptions>>( static provider => provider.GetRequiredService<TableCacheOptions>() );

        collection.AddStackExchangeRedisCache( configureRedis );
        collection.AddMemoryCache( configureMemoryCache );

        collection.AddSingleton<ISqlCacheFactory, TSqlCacheFactory>();
        collection.AddSingleton<ITableCache, TTableCacheFactory>();

        collection.AddSingleton<TDatabase>();
        collection.AddTransient<Database>( static provider => provider.GetRequiredService<TDatabase>() );
        collection.AddHealthCheck<TDatabase>();

        collection.AddFluentMigratorCore().ConfigureRunner( configureMigration );
        return collection;
    }


    public static IServiceCollection AddOptions<T>( this IServiceCollection collection, Action<T> options )
        where T : class, IOptions<T> => collection.AddOptions( options, Options.DefaultName );
    public static IServiceCollection AddOptions<T>( this IServiceCollection collection, Action<T> options, string name )
        where T : class, IOptions<T>
    {
        collection.AddSingleton<T>();
        collection.Configure( name, options );
        collection.AddTransient<IOptions<T>>( static provider => provider.GetRequiredService<T>() );
        return collection;
    }


    public static AuthenticationBuilder AddAuthentication( this IServiceCollection              collection,
                                                           Action<JwtBearerOptions>             configureJwt,
                                                           Action<AuthenticationOptions>?       configureAuth                   = default,
                                                           Action<CookieAuthenticationOptions>? authCookie                      = default,
                                                           Action<CookieAuthenticationOptions>? configureApplication            = default,
                                                           Action<CookieAuthenticationOptions>? configureExternal               = default,
                                                           Action<OpenIdConnectOptions>?        configureOpenIdConnect          = default,
                                                           Action<MicrosoftAccountOptions>?     configureMicrosoftAccount       = default,
                                                           Action<GoogleOptions>?               configureGoogle                 = default,
                                                           string                               authenticationScheme            = AUTHENTICATION_SCHEME,
                                                           string                               authenticationSchemeDisplayName = AUTHENTICATION_SCHEME_DISPLAY_NAME
    )

    {
        AuthenticationBuilder builder = collection.AddAuthentication( options =>
                                                                      {
                                                                          options.DefaultAuthenticateScheme = authenticationScheme;
                                                                          options.DefaultScheme             = authenticationScheme;
                                                                          configureAuth?.Invoke( options );
                                                                      } );

        builder.AddJwtBearer( authenticationScheme,
                              authenticationSchemeDisplayName,
                              bearer =>
                              {
                                  bearer.TokenHandlers.Add( DbTokenHandler.Instance );
                                  configureJwt.Invoke( bearer );
                              } );

        if ( authCookie is not null ) { builder.AddCookie( authenticationScheme, IdentityConstants.BearerScheme, authCookie ); }

        if ( configureApplication is not null ) { builder.AddCookie( IdentityConstants.ApplicationScheme, configureApplication ); }

        if ( configureExternal is not null ) { builder.AddCookie( IdentityConstants.ExternalScheme, configureExternal ); }

        if ( configureMicrosoftAccount is not null ) { builder.AddMicrosoftAccount( configureMicrosoftAccount ); }

        if ( configureGoogle is not null ) { builder.AddGoogle( configureGoogle ); }

        if ( configureOpenIdConnect is not null ) { builder.AddOpenIdConnect( configureOpenIdConnect ); }

        return builder;
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


    public static IServiceCollection AddDataProtection( this IServiceCollection collection )
    {
        DataProtectionServiceCollectionExtensions.AddDataProtection( collection );
        ProtectedDataProvider.Register( collection );
        return collection;
    }
    public static IServiceCollection AddEmailer( this IServiceCollection collection ) => collection.AddEmailer( static options => { } );
    public static IServiceCollection AddEmailer( this IServiceCollection collection, Action<Emailer.Options> configure )
    {
        collection.AddOptions( configure );
        collection.AddScoped<Emailer>();
        return collection;
    }


    public static IServiceCollection AddPasswordValidator( this IServiceCollection collection ) => collection.AddPasswordValidator( static requirements => { } );
    public static IServiceCollection AddPasswordValidator( this IServiceCollection collection, Action<PasswordRequirements> configure )
    {
        collection.AddOptions( configure );
        collection.AddScoped<IPasswordValidator<UserRecord>, UserPasswordValidator>();
        return collection;
    }


    public static IServiceCollection AddTokenizer( this IServiceCollection collection ) => collection.AddTokenizer<Tokenizer>();
    public static IServiceCollection AddTokenizer<TTokenizer>( this IServiceCollection collection )
        where TTokenizer : class, ITokenService
    {
        collection.AddScoped<ITokenService, TTokenizer>();
        return collection;
    }


    public static AuthorizationBuilder RequireMultiFactorAuthentication( this AuthorizationBuilder builder ) => builder.AddPolicy( nameof(RequireMfa), static policy => policy.Requirements.Add( RequireMfa.Instance ) );
}
