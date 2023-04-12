// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:35 PM

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Web;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static partial class DbExtensions
{
    public static IHealthChecksBuilder AddHealthCheck<T>( this WebApplicationBuilder builder ) where T : IHealthCheck => builder.AddHealthCheck( WebBuilder.CreateHealthCheck<T>() );
    public static IHealthChecksBuilder AddHealthCheck( this WebApplicationBuilder builder, HealthCheckRegistration registration ) =>
        builder.Services.AddHealthChecks()
               .Add( registration );


    public static ILoggingBuilder AddFluentMigratorLogger( this ILoggingBuilder builder, bool showSql = true, bool showElapsedTime = true ) =>
        builder.AddProvider( new FluentMigratorConsoleLoggerProvider( new OptionsWrapper<FluentMigratorLoggerOptions>( new FluentMigratorLoggerOptions
                                                                                                                       {
                                                                                                                           ShowElapsedTime = showElapsedTime,
                                                                                                                           ShowSql         = showSql,
                                                                                                                       } ) ) );


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/46775832/9530917"> Using ASP.NET Identity in an ASP.NET Core MVC application without Entity Framework and Migrations </see>
    /// <para><see cref="AuthenticationScheme"/></para>
    /// </summary> 
    /// <returns> </returns>
    public static IdentityBuilder AddIdentity( this WebApplicationBuilder          builder,
                                               Action<AuthenticationOptions>       configureAuthentication,
                                               Action<CookieAuthenticationOptions> configureApplication,
                                               Action<CookieAuthenticationOptions> configureExternal,
                                               Action<OpenIdConnectOptions>?       configureOpenIdConnect        = default,
                                               Action<MicrosoftAccountOptions>?    configureMicrosoftAccount     = default,
                                               Action<GoogleOptions>?              configureGoogle               = default,
                                               Action<IdentityOptions>?            setupAction                   = default,
                                               Action<PasswordRequirements>?       configurePasswordRequirements = default
    )
    {
        // https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationsListBlade


        /*
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "ClientId": "<Application_Client_ID>",
    "TenantId": "<Directory_Tenant_ID>",
    "Audience": "api://<Application_Client_ID>"
  },
  "Graph": {
    "BaseUrl": "https://graph.microsoft.com/v1.0",
    "Scopes": "User.Read"
  }
}
*/


        builder.AddPasswordValidator( configurePasswordRequirements ?? (( PasswordRequirements options ) => { }) );


        builder.AddOptions<IdentityOptions>()
               .Configure( setupAction ?? (( IdentityOptions options ) => { }) );


        AuthenticationBuilder auth = builder.AddAuthentication( configureAuthentication )
                                            .AddCookie( IdentityConstants.ApplicationScheme, configureApplication )
                                            .AddCookie( IdentityConstants.ExternalScheme,    configureExternal );

        if ( configureMicrosoftAccount is not null ) { auth.AddMicrosoftAccount( configureMicrosoftAccount ); }

        if ( configureGoogle is not null ) { auth.AddGoogle( configureGoogle ); }

        if ( configureOpenIdConnect is not null ) { auth.AddOpenIdConnect( configureOpenIdConnect ); }

        auth.AddMicrosoftIdentityWebApi( builder.Configuration.GetSection( "AzureAd" ) )
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        // .AddMicrosoftGraph( builder.Configuration.GetSection( "Graph" ) );

        builder.AddAuthorization( options => options.AddPolicy( nameof(RequireMfa), policy => policy.Requirements.Add( new RequireMfa() ) ) );


        RoleStore.Register( builder );
        UserStore.Register( builder );


        return builder.Services.AddIdentity<UserRecord, RoleRecord>()
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
    }


    public static WebApplicationBuilder AddDatabase<T>( this WebApplicationBuilder builder, Action<DbOptions> configure ) where T : Database
    {
        builder.AddOptions( configure );
        builder.AddSingleton<T>();
        builder.AddSingleton<Database>( provider => provider.GetRequiredService<T>() );
        builder.AddHealthCheck<T>();
        return builder;
    }


    public static WebApplicationBuilder AddEmailer( this WebApplicationBuilder builder )
    {
        builder.AddOptions<Emailer.Options>();
        return builder.AddScoped<Emailer>();
    }
    public static WebApplicationBuilder AddEmailer( this WebApplicationBuilder builder, Action<Emailer.Options> configure )
    {
        builder.AddOptions<Emailer.Options>()
               .Configure( configure );

        return builder.AddScoped<Emailer>();
    }


    public static WebApplicationBuilder AddPasswordValidator( this WebApplicationBuilder builder ) => builder.AddPasswordValidator( ( PasswordRequirements requirements ) => { } );
    public static WebApplicationBuilder AddPasswordValidator( this WebApplicationBuilder builder, Action<PasswordRequirements> configure )
    {
        builder.AddOptions<PasswordRequirements>()
               .Configure( configure );

        return builder.AddScoped<IPasswordValidator<UserRecord>, UserPasswordValidator>();
    }


    public static WebApplicationBuilder AddTokenizer( this             WebApplicationBuilder builder ) => builder.AddTokenizer<Tokenizer>();
    public static WebApplicationBuilder AddTokenizer<TTokenizer>( this WebApplicationBuilder builder ) where TTokenizer : Tokenizer => builder.AddScoped<ITokenService, TTokenizer>();
}



public sealed class RequireMfa : IAuthorizationRequirement { }
