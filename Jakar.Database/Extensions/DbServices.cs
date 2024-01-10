// Jakar.Extensions :: Jakar.Database
// 1/10/2024  14:10

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.EventLog;



namespace Jakar.Database;


[ SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" ) ]
public static class DbServices
{
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsValid<TRecord>( this TRecord value )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> => value.ID.IsValid();


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static bool IsNotValid<TRecord>( this TRecord value )
        where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord> => value.IsValid() is false;


    public static WebApplicationBuilder AddDefaultDbServices<T, TDatabase>( this WebApplicationBuilder builder, DbInstance dbType, SecuredString connectionString, Action<RedisCacheOptions> configureRedis )
        where T : IAppName
        where TDatabase : Database
    {
        string appName = typeof(T).Name;
        builder.AddDefaultLogging<T>();

        DbOptions dbOptions = new()
                              {
                                  DbType           = dbType,
                                  ConnectionString = connectionString,
                                  AppName          = appName,
                                  TokenAudience    = appName,
                                  TokenIssuer      = appName,
                                  Version          = AppVersion.FromAssembly( typeof(T).Assembly )
                              };

        TokenValidationParameters parameters = builder.GetTokenValidationParameters( dbOptions );

        builder.Services.AddStackExchangeRedisCache( configureRedis );

        builder.Services.AddDatabase<TDatabase>( dbOptions,
                                                 TableCacheOptions.Default,
                                                 configureRedis,
                                                 migration =>
                                                 {
                                                     switch ( dbType )
                                                     {
                                                         case DbInstance.MsSql:
                                                             migration.ConfigureMigrationsMsSql<T>();
                                                             return;

                                                         case DbInstance.Postgres:
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
                           } );

        builder.Services.AddAuthorizationBuilder().RequireMultiFactorAuthentication();

        return builder;
    }


    public static ILoggingBuilder AddDefaultLogging<T>( this WebApplicationBuilder builder )
        where T : IAppName => AddDefaultLogging<T>( builder.Logging, builder.Environment.EnvironmentName == Environments.Development );
    public static ILoggingBuilder AddDefaultLogging<T>( this ILoggingBuilder builder, bool isDevEnvironment )
        where T : IAppName => AddDefaultLogging<T>( builder,
                                                    isDevEnvironment
                                                        ? LogLevel.Trace
                                                        : LogLevel.Information );
    public static ILoggingBuilder AddDefaultLogging<T>( this ILoggingBuilder builder, in LogLevel minimumLevel )
        where T : IAppName => AddDefaultLogging( builder, minimumLevel, typeof(T).Name );
    public static ILoggingBuilder AddDefaultLogging( this ILoggingBuilder builder, in LogLevel minimumLevel, in string name )
    {
        builder.ClearProviders();
        builder.SetMinimumLevel( minimumLevel );
        builder.AddProvider( new DebugLoggerProvider() );

        builder.AddSimpleConsole( options =>
                                  {
                                      options.ColorBehavior = LoggerColorBehavior.Enabled;
                                      options.SingleLine    = false;
                                      options.IncludeScopes = true;
                                  } );


        if ( OperatingSystem.IsWindows() )
        {
            builder.AddProvider( new EventLogLoggerProvider( new EventLogSettings
                                                             {
                                                                 SourceName  = name,
                                                                 LogName     = name,
                                                                 MachineName = GetMachineName(),
                                                                 Filter      = ( category, level ) => level > LogLevel.Information
                                                             } ) );
        }
        else { builder.AddSystemdConsole( options => options.UseUtcTimestamp = true ); }

        builder.AddProvider( GetFluentMigratorConsoleLoggerProvider() );
        return builder;
    }


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


    public static FluentMigratorConsoleLoggerProvider GetFluentMigratorConsoleLoggerProvider( bool showSql = true, bool showElapsedTime = true ) =>
        new(new OptionsWrapper<FluentMigratorLoggerOptions>( new FluentMigratorLoggerOptions
                                                             {
                                                                 ShowElapsedTime = showElapsedTime,
                                                                 ShowSql         = showSql
                                                             } ));
    public static ILoggingBuilder AddFluentMigratorLogger( this ILoggingBuilder collection, bool showSql = true, bool showElapsedTime = true ) => collection.AddProvider( GetFluentMigratorConsoleLoggerProvider( showSql, showElapsedTime ) );


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


    /// <summary> <see href="https://stackoverflow.com/a/46775832/9530917"> Using ASP.NET Identity in an ASP.NET Core MVC application without Entity Framework and Migrations </see>
    ///     <para> <see cref="AuthenticationScheme"/> </para>
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
                         .AddTokenProvider<TokenProvider>( nameof(TokenProvider) )
                         .AddRoleValidator<RoleValidator>()
                         .AddPasswordValidator<UserPasswordValidator>()
                         .AddDefaultTokenProviders();

        static void ConfigureIdentityOptions( IdentityOptions _ ) { }
    }


    public static IServiceCollection AddDatabase<TDatabase>( this IServiceCollection         collection,
                                                             Action<DbOptions>               configureDbOptions,
                                                             Action<TableCacheOptions>       configureTableCacheOptions,
                                                             Action<RedisCacheOptions>       configureRedis,
                                                             Action<IMigrationRunnerBuilder> configureMigration
    )
        where TDatabase : Database => collection.AddDatabase<TDatabase, SqlCacheFactory, TableCacheFactory>( configureDbOptions, configureTableCacheOptions, configureRedis, configureMigration );
    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory>( this IServiceCollection         collection,
                                                                               Action<DbOptions>               configureDbOptions,
                                                                               Action<TableCacheOptions>       configureTableCacheOptions,
                                                                               Action<RedisCacheOptions>       configureRedis,
                                                                               Action<IMigrationRunnerBuilder> configureMigration
    )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory => collection.AddDatabase<TDatabase, TSqlCacheFactory, TableCacheFactory>( configureDbOptions, configureTableCacheOptions, configureRedis, configureMigration );
    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory, TTableCacheFactory>( this IServiceCollection         collection,
                                                                                                   Action<DbOptions>               configureDbOptions,
                                                                                                   Action<TableCacheOptions>       configureTableCacheOptions,
                                                                                                   Action<RedisCacheOptions>       configureRedis,
                                                                                                   Action<IMigrationRunnerBuilder> configureMigration
    )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory
        where TTableCacheFactory : class, ITableCacheFactoryService
    {
        DbOptions dbOptions = new();
        configureDbOptions( dbOptions );
        TableCacheOptions tableCacheOptions = new();
        configureTableCacheOptions( tableCacheOptions );
        return collection.AddDatabase<TDatabase, TSqlCacheFactory, TTableCacheFactory>( dbOptions, tableCacheOptions, configureRedis, configureMigration );
    }


    public static IServiceCollection AddDatabase<TDatabase>( this IServiceCollection collection, DbOptions dbOptions, TableCacheOptions tableCacheOptions, Action<RedisCacheOptions> configureRedis, Action<IMigrationRunnerBuilder> configureMigration )
        where TDatabase : Database => collection.AddDatabase<TDatabase, SqlCacheFactory, TableCacheFactory>( dbOptions, tableCacheOptions, configureRedis, configureMigration );
    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory>( this IServiceCollection         collection,
                                                                               DbOptions                       dbOptions,
                                                                               TableCacheOptions               tableCacheOptions,
                                                                               Action<RedisCacheOptions>       configureRedis,
                                                                               Action<IMigrationRunnerBuilder> configureMigration
    )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory => collection.AddDatabase<TDatabase, TSqlCacheFactory, TableCacheFactory>( dbOptions, tableCacheOptions, configureRedis, configureMigration );
    public static IServiceCollection AddDatabase<TDatabase, TSqlCacheFactory, TTableCacheFactory>( this IServiceCollection         collection,
                                                                                                   DbOptions                       dbOptions,
                                                                                                   TableCacheOptions               tableCacheOptions,
                                                                                                   Action<RedisCacheOptions>       configureRedis,
                                                                                                   Action<IMigrationRunnerBuilder> configureMigration
    )
        where TDatabase : Database
        where TSqlCacheFactory : class, ISqlCacheFactory
        where TTableCacheFactory : class, ITableCacheFactoryService
    {
        collection.AddSingleton( dbOptions );
        collection.AddTransient<IOptions<DbOptions>>( static provider => provider.GetRequiredService<DbOptions>() );

        collection.AddSingleton( tableCacheOptions );
        collection.AddTransient<IOptions<TableCacheOptions>>( static provider => provider.GetRequiredService<TableCacheOptions>() );

        collection.AddStackExchangeRedisCache( configureRedis );

        collection.AddSingleton<ISqlCacheFactory, TSqlCacheFactory>();
        collection.AddSingleton<ITableCacheFactoryService, TTableCacheFactory>();

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
                                                           Action<CookieAuthenticationOptions>? cookie                          = default,
                                                           Action<CookieAuthenticationOptions>? configureApplication            = default,
                                                           Action<CookieAuthenticationOptions>? configureExternal               = default,
                                                           Action<OpenIdConnectOptions>?        configureOpenIdConnect          = default,
                                                           Action<MicrosoftAccountOptions>?     configureMicrosoftAccount       = default,
                                                           Action<GoogleOptions>?               configureGoogle                 = default,
                                                           string                               authenticationScheme            = JwtBearerDefaults.AuthenticationScheme,
                                                           string                               authenticationSchemeDisplayName = nameof(JwtBearerHandler),
                                                           string                               cookieDisplayName               = CookieAuthenticationDefaults.AuthenticationScheme
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

        if ( cookie is not null ) { builder.AddCookie( authenticationScheme, cookieDisplayName, cookie ); }

        if ( configureApplication is not null ) { builder.AddCookie( IdentityConstants.ApplicationScheme, configureApplication ); }

        if ( configureExternal is not null ) { builder.AddCookie( IdentityConstants.ExternalScheme, configureExternal ); }

        if ( configureMicrosoftAccount is not null ) { builder.AddMicrosoftAccount( configureMicrosoftAccount ); }

        if ( configureGoogle is not null ) { builder.AddGoogle( configureGoogle ); }

        if ( configureOpenIdConnect is not null ) { builder.AddOpenIdConnect( configureOpenIdConnect ); }

        return builder;
    }


    public static JwtBearerOptions GetJwtBearerOptions<T>( this IServiceProvider provider )
        where T : IAppName
    {
        JwtBearerOptions? bearer = provider.GetService<JwtBearerOptions>();
        if ( bearer is not null ) { return bearer; }

        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        DbOptions      options       = provider.GetRequiredService<DbOptions>();

        JwtBearerOptions jwt = new()
                               {
                                   Audience                  = typeof(T).Name,
                                   ClaimsIssuer              = typeof(T).Name,
                                   TokenValidationParameters = configuration.GetTokenValidationParameters( options )
                               };

        jwt.TokenHandlers.Add( DbTokenHandler.Instance );
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
        where TTokenizer : Tokenizer
    {
        collection.AddScoped<ITokenService, TTokenizer>();
        return collection;
    }


    public static AuthorizationBuilder RequireMultiFactorAuthentication( this AuthorizationBuilder builder ) => builder.AddPolicy( nameof(RequireMfa), static policy => policy.Requirements.Add( RequireMfa.Instance ) );
}
