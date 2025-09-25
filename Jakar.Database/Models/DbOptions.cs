// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

using ZiggyCreatures.Caching.Fusion.Backplane.Memory;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
public sealed class DbOptions : IOptions<DbOptions>
{
    public const           string         AUTHENTICATION_TYPE = JwtBearerDefaults.AuthenticationScheme;
    public const           int            COMMAND_TIMEOUT     = 300;
    public const           string         JWT_ALGORITHM       = SecurityAlgorithms.HmacSha512Signature;
    public const           string         JWT_KEY             = "JWT";
    public const           string         USER_EXISTS         = "User Exists";
    public static readonly Uri            Local_433           = new("https://localhost:443");
    public static readonly Uri            Local_80            = new("http://localhost:80");
    public                 AppInformation AppInformation                  { get; set; } = AppInformation.Invalid;
    public                 string         AuthenticationScheme            { get; set; } = DbServices.AUTHENTICATION_SCHEME;
    public                 string         AuthenticationSchemeDisplayName { get; set; } = DbServices.AUTHENTICATION_SCHEME_DISPLAY_NAME;


    public string                                                  AuthenticationType             { get;                                 set; } = AUTHENTICATION_TYPE;
    public TimeSpan                                                ClockSkew                      { get;                                 set; } = TimeSpan.FromMinutes(1);
    public int?                                                    CommandTimeout                 { get;                                 set; } = COMMAND_TIMEOUT;
    public Action<CookieAuthenticationOptions>?                    ConfigureApplicationCookie     { get;                                 set; }
    public Action<AuthenticationOptions>?                          ConfigureAuthentication        { get;                                 set; }
    public Action<RedisBackplaneOptions>?                          ConfigureAuthenticationOptions { get;                                 set; }
    public Action<CookieAuthenticationOptions>?                    ConfigureCookieAuth            { get;                                 set; }
    public Action<CookieAuthenticationOptions>?                    ConfigureExternalCookie        { get;                                 set; }
    public Action<GoogleOptions>?                                  ConfigureGoogle                { get;                                 set; }
    public Action<IdentityOptions>                                 ConfigureIdentityOptions       { get;                                 set; }
    public Action<LoggerProviderBuilder>?                          ConfigureLoggerProviderBuilder { get;                                 set; }
    public Action<MemoryBackplaneOptions>?                         ConfigureMemoryBackplane       { get;                                 set; }
    public Action<OtlpExporterOptions>?                            ConfigureMeterOtlpExporter     { get;                                 set; }
    public Action<MicrosoftAccountOptions>?                        ConfigureMicrosoftAccount      { get;                                 set; }
    public Action<OpenIdConnectOptions>?                           ConfigureOpenIdConnect         { get;                                 set; }
    public Action<OpenTelemetryLoggerOptions>?                     ConfigureOpenTelemetryLogger   { get;                                 set; }
    public Action<RedisBackplaneOptions>?                          ConfigureRedisBackplane        { get;                                 set; }
    public Action<OtlpExporterOptions>?                            ConfigureTracerOtlpExporter    { get;                                 set; }
    public SecuredStringResolverOptions                            ConnectionStringResolver       { get;                                 set; } = (Func<IConfiguration, SecuredString>)GetConnectionString;
    public (LocalFile Pem, SecuredStringResolverOptions Password)? DataProtectorKey               { get;                                 set; }
    public Uri                                                     Domain                         { get;                                 set; } = Local_433;
    public FusionCacheEntryOptions                                 FusionCacheEntryOptions        { get;                                 set; } = new() { Duration = TimeSpan.FromMinutes(2) };
    public string                                                  JWTAlgorithm                   { get;                                 set; } = JWT_ALGORITHM;
    public string                                                  JWTKey                         { get;                                 set; } = JWT_KEY;
    public AppLoggerOptions                                        LoggerOptions                  { get;                                 set; } = new();
    public PasswordRequirements                                    PasswordRequirements           { get => PasswordRequirements.Current; set => PasswordRequirements.Current = value; }
    public SeqConfig                                               SeqConfig                      { get;                                 set; }
    public Logger?                                                 Serilogger                     { get;                                 set; }
    public TelemetrySource?                                        TelemetrySource                { get => TelemetrySource.Current;      set => TelemetrySource.Current = value; }
    public string                                                  TokenAudience                  { get;                                 set; } = string.Empty;
    public string                                                  TokenIssuer                    { get;                                 set; } = string.Empty;
    public string                                                  UserExists                     { get;                                 set; } = USER_EXISTS;
    DbOptions IOptions<DbOptions>.                                 Value                          => this;


    public DbOptions() => ConfigureIdentityOptions = DefaultConfigureIdentityOptions;


    public DbOptions With<TApp>()
        where TApp : IAppID
    {
        AppInformation = new AppInformation(TApp.AppVersion, TApp.AppID, TApp.AppName, null);
        return this;
    }
    public DbOptions With( in AppInformation app )
    {
        AppInformation = app;
        return this;
    }


    private void DefaultConfigureAuthenticationOptions( AuthenticationOptions options )
    {
        options.DefaultAuthenticateScheme = AuthenticationScheme;
        options.DefaultScheme             = AuthenticationScheme;
    }
    public void AddAuthentication( WebApplicationBuilder application )
    {
        AuthenticationBuilder builder = application.Services.AddAuthentication(ConfigureAuthentication ?? DefaultConfigureAuthenticationOptions);

        builder.AddJwtBearer(AuthenticationScheme, AuthenticationSchemeDisplayName, x => Configure(x, application));

        if ( ConfigureCookieAuth is not null ) { builder.AddCookie(AuthenticationScheme, IdentityConstants.BearerScheme, ConfigureCookieAuth); }

        if ( ConfigureApplicationCookie is not null ) { builder.AddCookie(IdentityConstants.ApplicationScheme, ConfigureApplicationCookie); }

        if ( ConfigureExternalCookie is not null ) { builder.AddCookie(IdentityConstants.ExternalScheme, ConfigureExternalCookie); }

        if ( ConfigureMicrosoftAccount is not null ) { builder.AddMicrosoftAccount(ConfigureMicrosoftAccount); }

        if ( ConfigureGoogle is not null ) { builder.AddGoogle(ConfigureGoogle); }

        if ( ConfigureOpenIdConnect is not null ) { builder.AddOpenIdConnect(ConfigureOpenIdConnect); }
    }


    public WebApplicationBuilder AddDatabase<TDatabase>( WebApplicationBuilder builder )
        where TDatabase : Database
    {
        builder.Services.AddSingleton(this);
        builder.Services.AddTransient<IOptions<DbOptions>>(static provider => provider.GetRequiredService<DbOptions>());

        builder.AddOpenTelemetry<TestDatabase>(tracerOtlpExporter => { }, meterOtlpExporter => { });

        builder.AddSerilog(LoggerOptions, Validate.ThrowIfNull(TelemetrySource), SeqConfig, out Logger logger);
        Serilogger = logger;

        ConfigureFusionCache(builder.Services.AddFusionCache());

        builder.Services.AddSingleton<TDatabase>();
        builder.Services.AddTransient<Database>(static provider => provider.GetRequiredService<TDatabase>());
        builder.Services.AddHealthCheck<TDatabase>();

        builder.Services.AddFluentMigratorCore().ConfigureRunner(static runner => runner.MigrationsPostgres());

        AddIdentityServices(builder.Services);

        builder.Services.AddDataProtection();

        builder.Services.AddPasswordValidator();

        builder.Services.AddInMemoryTokenCaches();

        builder.Services.AddEmailer();

        AddAuthentication(builder);

        builder.Services.AddAuthorizationBuilder().RequireMultiFactorAuthentication();
        return builder;
    }
    public WebApplicationBuilder AddDatabase<TDatabase, TApp>( WebApplicationBuilder builder )
        where TDatabase : Database
        where TApp : IAppName
    {
        builder.Services.AddSingleton(this);
        builder.Services.AddTransient<IOptions<DbOptions>>(static provider => provider.GetRequiredService<DbOptions>());

        builder.AddOpenTelemetry<TestDatabase>(tracerOtlpExporter => { }, meterOtlpExporter => { });

        builder.AddSerilog(LoggerOptions, Validate.ThrowIfNull(TelemetrySource), SeqConfig, out Logger logger);
        Serilogger = logger;

        ConfigureFusionCache(builder.Services.AddFusionCache());

        builder.Services.AddSingleton<TDatabase>();
        builder.Services.AddTransient<Database>(static provider => provider.GetRequiredService<TDatabase>());
        builder.Services.AddHealthCheck<TDatabase>();

        builder.Services.AddFluentMigratorCore().ConfigureRunner(static runner => runner.MigrationsPostgres<TApp>());

        AddIdentityServices(builder.Services);

        builder.Services.AddDataProtection();

        builder.Services.AddPasswordValidator();

        builder.Services.AddInMemoryTokenCaches();

        builder.Services.AddEmailer();

        AddAuthentication(builder);

        builder.Services.AddAuthorizationBuilder().RequireMultiFactorAuthentication();
        return builder;
    }


    public void AddIdentityServices( IServiceCollection services ) => services.AddIdentityServices<DataProtectorTokenProvider, EmailTokenProvider, PhoneNumberTokenProvider, OtpAuthenticatorTokenProvider>(Validate.ThrowIfNull(TelemetrySource), ConfigureIdentityOptions);


    private void Configure( JwtBearerOptions options, WebApplicationBuilder application )
    {
        options.TokenHandlers.Add(DbTokenHandler.Instance);
        options.Audience                   = AppInformation.AppName;
        options.Authority                  = AppInformation.AppName;
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters  = application.GetTokenValidationParameters(this);
    }


    public void ConfigureFusionCache( IFusionCacheBuilder builder )
    {
        builder.WithDefaultEntryOptions(FusionCacheEntryOptions);
        builder.WithStackExchangeRedisBackplane(ConfigureRedisBackplane);
        builder.WithMemoryBackplane(ConfigureMemoryBackplane);
        builder.WithLogger(static provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger<FusionCache>());
    }


    private void DefaultConfigureIdentityOptions( IdentityOptions options )
    {
        AppInformation       info         = AppInformation;
        PasswordRequirements requirements = PasswordValidator.Requirements;
        options.Password.RequireDigit                 = requirements.RequireNumber;
        options.Password.RequireLowercase             = requirements.RequireLowerCase;
        options.Password.RequireUppercase             = requirements.RequireUpperCase;
        options.Password.RequireNonAlphanumeric       = requirements.RequireSpecialChar;
        options.Password.RequiredUniqueChars          = requirements.MinLength / 3;
        options.Password.RequiredLength               = requirements.MinLength;
        options.Tokens.AuthenticatorIssuer            = info.AppName;
        options.Tokens.AuthenticatorTokenProvider     = info.AppName;
        options.Tokens.ChangeEmailTokenProvider       = info.AppName;
        options.Tokens.ChangePhoneNumberTokenProvider = info.AppName;
        options.Tokens.EmailConfirmationTokenProvider = info.AppName;
        options.Tokens.PasswordResetTokenProvider     = info.AppName;
        options.Lockout.DefaultLockoutTimeSpan        = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts       = 5;
        options.Lockout.AllowedForNewUsers            = true;
        options.SignIn.RequireConfirmedEmail          = true;
        options.SignIn.RequireConfirmedAccount        = true;
        options.User.RequireUniqueEmail               = true;
        options.User.AllowedUserNameCharacters        = Randoms.ALPHANUMERIC + Randoms.SPECIAL_CHARS;
        options.ClaimsIdentity.EmailClaimType         = ClaimType.Email.ToClaimTypes();
        options.ClaimsIdentity.UserIdClaimType        = ClaimType.UserID.ToClaimTypes();
        options.ClaimsIdentity.UserNameClaimType      = ClaimType.UserName.ToClaimTypes();
        options.ClaimsIdentity.RoleClaimType          = ClaimType.Role.ToClaimTypes();
        options.Stores.ProtectPersonalData            = true;
    }


    public static SecuredString GetConnectionString( IConfiguration          configuration ) => SecuredStringResolverOptions.GetSecuredString(configuration);
    public static void          GetConnectionString( IMigrationRunnerBuilder provider )      => provider.WithGlobalConnectionString(GetConnectionString);
    public static string GetConnectionString( IServiceProvider provider )
    {
        ValueTask<SecuredString> task    = GetConnectionStringAsync(provider);
        SecuredString            secured = task.CallSynchronously();
        string                   value   = secured.ToString();
        return value;
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider )
    {
        using CancellationTokenSource source = new(TimeSpan.FromMinutes(5));
        return await GetConnectionStringAsync(provider, source.Token);
    }
    public static async ValueTask<SecuredString> GetConnectionStringAsync( IServiceProvider provider, CancellationToken token )
    {
        DbOptions      options       = provider.GetRequiredService<IOptions<DbOptions>>().Value;
        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        SecuredString  secure        = await options.GetConnectionStringAsync(configuration, token);
        return secure;
    }
    public async ValueTask<SecuredString> GetConnectionStringAsync( IConfiguration configuration, CancellationToken token ) => await ConnectionStringResolver.GetSecuredStringAsync(configuration, token);
}



// public interface IConnectionStringProvider { }
