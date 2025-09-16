// Jakar.Extensions :: Jakar.Database
// 06/02/2024  15:06

using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.Memory;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.OpenTelemetry;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
public abstract class ConfigureDbServices<TSelf, TApp, TDatabase>
    where TApp : IAppName
    where TDatabase : Database
    where TSelf : ConfigureDbServices<TSelf, TApp, TDatabase>, new()
{
    public          string                  AppName                         { get; }       = TApp.AppName;
    public          string                  AuthenticationScheme            { get; init; } = DbServices.AUTHENTICATION_SCHEME;
    public          string                  AuthenticationSchemeDisplayName { get; init; } = DbServices.AUTHENTICATION_SCHEME_DISPLAY_NAME;
    public          DbOptions               DbOptions                       { get; init; } = new();
    public virtual  FusionCacheEntryOptions FusionCacheEntryOptions         { get; }       = new() { Duration = TimeSpan.FromMinutes(2) };
    public abstract AppLoggerOptions        LoggerOptions                   { get; }
    public abstract SeqConfig               SeqConfig                       { get; }
    public abstract TelemetrySource         TelemetrySource                 { get; }
    public abstract bool                    UseApplicationCookie            { get; }
    public abstract bool                    UseAuth                         { get; }
    public abstract bool                    UseAuthCookie                   { get; }
    public abstract bool                    UseExternalCookie               { get; }
    public abstract bool                    UseGoogleAccount                { get; }
    public virtual  bool                    UseMemoryCache                  => true;
    public abstract bool                    UseMicrosoftAccount             { get; }
    public abstract bool                    UseOpenIdConnect                { get; }
    public abstract bool                    UseRedis                        { get; }


    public DbOptions GetDbOptions()
    {
        DbOptions.AppName       = AppName;
        DbOptions.TokenAudience = AppName;
        DbOptions.TokenIssuer   = AppName;
        return DbOptions;
    }


    public static void Setup( WebApplicationBuilder builder )
    {
        TSelf instance = new();
        instance.Configure(builder);
    }
    protected internal virtual WebApplicationBuilder Configure( WebApplicationBuilder builder )
    {
        builder.Services.AddOpenTelemetry().WithMetrics(Configure).WithTracing(Configure).WithLogging();

        builder.AddOpenTelemetry<TestDatabase>(tracerOtlpExporter => { }, meterOtlpExporter => { });
        builder.AddSerilog(LoggerOptions, TelemetrySource, SeqConfig, out _);

        Configure(builder.Services.AddFusionCache());

        builder.Services.AddInMemoryTokenCaches();

        builder.Services.AddSingleton(DbOptions);
        builder.Services.AddTransient(DbOptions.Get);

        builder.Services.AddSingleton<TDatabase>();
        builder.Services.AddTransient<Database>(static provider => provider.GetRequiredService<TDatabase>());
        builder.Services.AddHealthCheck<TDatabase>();

        builder.Services.AddFluentMigratorCore().ConfigureRunner(Configure);

        builder.Services.AddIdentityServices(Configure);
        builder.Services.AddDataProtection();
        builder.Services.AddIdentityServices();
        builder.Services.AddPasswordValidator();
        builder.Services.AddTokenizer();
        builder.Services.AddEmailer();

        AddAuthentication(builder);

        builder.Services.AddAuthorizationBuilder().RequireMultiFactorAuthentication();
        return builder;
    }


    protected virtual void Configure( JwtBearerOptions options, WebApplicationBuilder application )
    {
        options.TokenHandlers.Add(DbTokenHandler.Instance);
        options.Audience                   = AppName;
        options.Authority                  = AppName;
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters  = application.GetTokenValidationParameters(DbOptions);
    }


    protected abstract void Configure( RedisBackplaneOptions  options );
    protected abstract void Configure( MemoryBackplaneOptions options );
    protected virtual void Configure( IFusionCacheBuilder builder )
    {
        builder.WithDefaultEntryOptions(FusionCacheEntryOptions);
        builder.WithStackExchangeRedisBackplane(Configure);
        builder.WithMemoryBackplane(Configure);
        builder.WithLogger(static provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger<FusionCache>());
    }


    protected virtual void Configure( NpgsqlMetricsOptions                     options ) { }
    protected virtual void Configure( FusionCacheMetricsInstrumentationOptions options ) { }
    protected virtual void Configure( TracerProviderBuilder builder )
    {
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
        builder.AddFusionCacheInstrumentation();
    }
    protected virtual void Configure( MeterProviderBuilder builder )
    {
        builder.AddRuntimeInstrumentation();
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
        builder.AddFusionCacheInstrumentation(Configure);
        builder.AddNpgsqlInstrumentation(Configure);
    }


    protected virtual void Configure( AuthenticationOptions options )
    {
        options.DefaultAuthenticateScheme = AuthenticationScheme;
        options.DefaultScheme             = AuthenticationScheme;
    }
    private void AddAuthentication( WebApplicationBuilder application )
    {
        AuthenticationBuilder builder = application.Services.AddAuthentication(Configure);

        builder.AddJwtBearer(AuthenticationScheme, AuthenticationSchemeDisplayName, x => Configure(x, application));

        if ( UseAuthCookie ) { builder.AddCookie(AuthenticationScheme, IdentityConstants.BearerScheme, Configure); }

        if ( UseApplicationCookie ) { builder.AddCookie(IdentityConstants.ApplicationScheme, ConfigureApplicationCookie); }

        if ( UseExternalCookie ) { builder.AddCookie(IdentityConstants.ExternalScheme, ConfigureExternalCookie); }

        if ( UseMicrosoftAccount ) { builder.AddMicrosoftAccount(Configure); }

        if ( UseGoogleAccount ) { builder.AddGoogle(Configure); }

        if ( UseOpenIdConnect ) { builder.AddOpenIdConnect(Configure); }
    }


    protected virtual void Configure( CookieAuthenticationOptions                  options ) { }
    protected virtual void ConfigureApplicationCookie( CookieAuthenticationOptions options ) { }
    protected virtual void ConfigureExternalCookie( CookieAuthenticationOptions    options ) { }
    protected virtual void Configure( OpenIdConnectOptions                         options ) { }
    protected virtual void Configure( MicrosoftAccountOptions                      options ) { }
    protected virtual void Configure( GoogleOptions                                options ) { }
    protected virtual void Configure( IdentityOptions                              options ) { }
    protected virtual void Configure( IMigrationRunnerBuilder                      options ) { }
}
