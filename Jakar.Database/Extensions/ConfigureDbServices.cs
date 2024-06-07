// Jakar.Extensions :: Jakar.Database
// 06/02/2024  15:06

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;



namespace Jakar.Database;


public abstract class ConfigureDbServices<TApp, TDatabase, TSqlCacheFactory, TTableCacheFactory>
    where TApp : IAppName
    where TDatabase : Database
    where TSqlCacheFactory : class, ISqlCacheFactory
    where TTableCacheFactory : class, ITableCache
{
    public          string            AppName                         { get; }       = typeof(TApp).Name;
    public          string            AuthenticationScheme            { get; init; } = DbServices.AUTHENTICATION_SCHEME;
    public          string            AuthenticationSchemeDisplayName { get; init; } = DbServices.AUTHENTICATION_SCHEME_DISPLAY_NAME;
    public          DbOptions         DbOptions                       { get; init; } = new();
    public          TableCacheOptions TableCacheOptions               { get; init; } = TableCacheOptions.Default;
    public abstract bool              UseApplicationCookie            { get; }
    public abstract bool              UseAuth                         { get; }
    public abstract bool              UseAuthCookie                   { get; }
    public abstract bool              UseExternalCookie               { get; }
    public abstract bool              UseGoogleAccount                { get; }
    public virtual  bool              UseMemoryCache                  { get; } = true;
    public abstract bool              UseMicrosoftAccount             { get; }
    public abstract bool              UseOpenIdConnect                { get; }
    public abstract bool              UseRedis                        { get; }


    public virtual void Redis( RedisCacheOptions                       options ) { }
    public virtual void Jwt( JwtBearerOptions                          options ) { }
    public virtual void Authentication( AuthenticationOptions          options ) { }
    public virtual void AuthCookie( CookieAuthenticationOptions        options ) { }
    public virtual void ApplicationCookie( CookieAuthenticationOptions options ) { }
    public virtual void ExternalCookie( CookieAuthenticationOptions    options ) { }
    public virtual void OpenIdConnect( OpenIdConnectOptions            options ) { }
    public virtual void MicrosoftAccount( MicrosoftAccountOptions      options ) { }
    public virtual void Google( GoogleOptions                          options ) { }
    public virtual void MemoryCache( MemoryCacheOptions                options ) { }
    public virtual void Identity( IdentityOptions                      options ) { }
    public virtual void Migration( IMigrationRunnerBuilder             options ) { }
    public virtual void Metrics( MeterProviderBuilder                  metrics ) { }
    public virtual void Tracing( TracerProviderBuilder                 tracing ) { }


    public DbOptions GetDbOptions()
    {
        DbOptions.AppName       = AppName;
        DbOptions.TokenAudience = AppName;
        DbOptions.TokenIssuer   = AppName;
        return DbOptions;
    }


    protected internal virtual WebApplicationBuilder Configure( WebApplicationBuilder builder )
    {
        builder.Services.AddInMemoryTokenCaches();

        builder.Services.AddSingleton( DbOptions );
        builder.Services.AddTransient( DbOptions.Get );

        builder.Services.AddSingleton( TableCacheOptions );
        builder.Services.AddTransient( TableCacheOptions.Get );

        builder.Services.AddStackExchangeRedisCache( Redis );
        builder.Services.AddMemoryCache( MemoryCache );

        builder.Services.AddSingleton<ISqlCacheFactory, TSqlCacheFactory>();
        builder.Services.AddSingleton<ITableCache, TTableCacheFactory>();

        builder.Services.AddSingleton<TDatabase>();
        builder.Services.AddTransient<Database>( static provider => provider.GetRequiredService<TDatabase>() );
        builder.Services.AddHealthCheck<TDatabase>();

        builder.Services.AddFluentMigratorCore().ConfigureRunner( Migration );

        builder.Services.AddIdentityServices( Identity );
        builder.Services.AddDataProtection();
        builder.Services.AddIdentityServices();
        builder.Services.AddPasswordValidator();
        builder.Services.AddTokenizer();
        builder.Services.AddEmailer();

        AddAuthentication( builder );

        builder.Services.AddAuthorizationBuilder().RequireMultiFactorAuthentication();
        return builder;
    }

    private void ConfigureAuthentication( AuthenticationOptions options )
    {
        options.DefaultAuthenticateScheme = AuthenticationScheme;
        options.DefaultScheme             = AuthenticationScheme;
        Authentication( options );
    }
    private void AddAuthentication( WebApplicationBuilder application )
    {
        AuthenticationBuilder builder = application.Services.AddAuthentication( ConfigureAuthentication );

        builder.AddJwtBearer( AuthenticationScheme, AuthenticationSchemeDisplayName, ConfigureOptions );

        if ( UseAuthCookie ) { builder.AddCookie( AuthenticationScheme, IdentityConstants.BearerScheme, AuthCookie ); }

        if ( UseApplicationCookie ) { builder.AddCookie( IdentityConstants.ApplicationScheme, ApplicationCookie ); }

        if ( UseExternalCookie ) { builder.AddCookie( IdentityConstants.ExternalScheme, ExternalCookie ); }

        if ( UseMicrosoftAccount ) { builder.AddMicrosoftAccount( MicrosoftAccount ); }

        if ( UseGoogleAccount ) { builder.AddGoogle( Google ); }

        if ( UseOpenIdConnect ) { builder.AddOpenIdConnect( OpenIdConnect ); }

        return;

        void ConfigureOptions( JwtBearerOptions bearer )
        {
            bearer.TokenHandlers.Add( DbTokenHandler.Instance );
            bearer.Audience                   = AppName;
            bearer.Authority                  = AppName;
            bearer.UseSecurityTokenValidators = true;
            bearer.TokenValidationParameters  = application.GetTokenValidationParameters( DbOptions );
            Jwt( bearer );
        }
    }
}
