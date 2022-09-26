using Microsoft.AspNetCore.Authorization;



namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static WebApplicationBuilder AddAuthorization( this WebApplicationBuilder builder )
    {
        builder.Services.AddAuthorization();
        return builder;
    }
    public static WebApplicationBuilder AddAuthorization( this WebApplicationBuilder builder, Action<AuthorizationOptions> configure )
    {
        builder.Services.AddAuthorization(configure);
        return builder;
    }


    public static WebApplicationBuilder AddAuthorizationCore( this WebApplicationBuilder builder )
    {
        builder.Services.AddAuthorizationCore();
        return builder;
    }
    public static WebApplicationBuilder AddAuthorizationCore( this WebApplicationBuilder builder, Action<AuthorizationOptions> configure )
    {
        builder.Services.AddAuthorizationCore(configure);
        return builder;
    }


    public static AuthenticationBuilder AddAuthentication( this WebApplicationBuilder builder ) => builder.Services.AddAuthentication();
    public static AuthenticationBuilder AddAuthentication( this WebApplicationBuilder builder, string                        defaultScheme ) => builder.Services.AddAuthentication(defaultScheme);
    public static AuthenticationBuilder AddAuthentication( this WebApplicationBuilder builder, Action<AuthenticationOptions> configureOptions ) => builder.Services.AddAuthentication(configureOptions);


    public static WebApplicationBuilder AddAuthenticationCore( this WebApplicationBuilder builder )
    {
        builder.Services.AddAuthenticationCore();
        return builder;
    }
    public static WebApplicationBuilder AddAuthenticationCore( this WebApplicationBuilder builder, Action<AuthenticationOptions> configureOptions )
    {
        builder.Services.AddAuthenticationCore(configureOptions);
        return builder;
    }
}
