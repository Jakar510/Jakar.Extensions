// Jakar.Extensions :: Jakar.Database
// 05/22/2023  11:38 AM

namespace Jakar.Database;


public static class WebAppExtensions
{
    /// <summary> My config is Pascal Case </summary>
    /// <param name="services"> </param>
    /// <returns> </returns>
    public static IMvcBuilder UseNewtonsoftJson( this IMvcBuilder services )
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                                            {
                                                ContractResolver = new DefaultContractResolver()
                                            };

        return services.AddJsonOptions( options => options.JsonSerializerOptions.PropertyNamingPolicy = null ).AddNewtonsoftJson( options => options.SerializerSettings.ContractResolver = new DefaultContractResolver() );
    }
    public static WebApplication UseUrls( this WebApplication app, params string[] urls )
    {
        foreach ( string url in urls ) { app.Urls.Add( url ); }

        return app;
    }

    /// <summary> Specify the urls the web host will listen on. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="urls"> The urls the hosted application will listen on. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static void UseUrls( this WebApplicationBuilder builder, params string[] urls )
    {
        if ( urls is null ) { throw new ArgumentNullException( nameof(urls) ); }

        builder.WebHost.UseSetting( WebHostDefaults.ServerUrlsKey, string.Join( ';', urls ) );
    }

    /// <summary> Specify the webroot directory to be used by the web host. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="webRoot"> Path to the root directory used by the web server. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static void UseWebRoot( this WebApplicationBuilder builder, string webRoot )
    {
        if ( webRoot is null ) { throw new ArgumentNullException( nameof(webRoot) ); }

        builder.WebHost.UseSetting( WebHostDefaults.WebRootKey, webRoot );
    }
}
