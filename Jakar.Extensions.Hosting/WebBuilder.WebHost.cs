// Jakar.Extensions :: Jakar.Extensions.Hosting
// 06/09/2022  1:19 PM


namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static WebApplicationBuilder UseSetting( this WebApplicationBuilder builder, string key, string value )
    {
        builder.WebHost.UseSetting( key, value );
        return builder;
    }

    /// <summary> Set whether startup errors should be captured in the configuration settings of the web host. When enabled, startup exceptions will be caught and an error page will be returned. If disabled, startup exceptions will be propagated. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="captureStartupErrors"> <c> true </c> to use startup error page; otherwise <c> false </c> . </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder CaptureStartupErrors( this WebApplicationBuilder builder, bool captureStartupErrors )
    {
        builder.UseSetting( WebHostDefaults.CaptureStartupErrorsKey,
                            captureStartupErrors
                                ? "true"
                                : "false" );

        return builder;
    }


    /// <summary> Indicate whether the host should listen on the URLs configured on the <see cref="WebApplicationBuilder"/> instead of those configured on the <see cref="IServer"/> . </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="preferHostingUrls"> <c> true </c> to prefer URLs configured on the <see cref="WebApplicationBuilder"/> ; otherwise <c> false </c> . </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder PreferHostingUrls( this WebApplicationBuilder builder, bool preferHostingUrls )
    {
        return builder.UseSetting( WebHostDefaults.PreferHostingUrlsKey,
                                   preferHostingUrls
                                       ? "true"
                                       : "false" );
    }


    /// <summary> Specify if startup status messages should be suppressed. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="suppressStatusMessages"> <c> true </c> to suppress writing of hosting startup status messages; otherwise <c> false </c> . </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder SuppressStatusMessages( this WebApplicationBuilder builder, bool suppressStatusMessages )
    {
        return builder.UseSetting( WebHostDefaults.SuppressStatusMessagesKey,
                                   suppressStatusMessages
                                       ? "true"
                                       : "false" );
    }
    /// <summary> Use the given configuration settings on the web host. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="configuration"> The <see cref="IConfiguration"/> containing settings to be used. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseConfiguration( this WebApplicationBuilder builder, IConfiguration configuration )
    {
        builder.WebHost.UseConfiguration( configuration );
        return builder;
    }


    /// <summary> Specify the content root directory to be used by the web host. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="contentRoot"> Path to root directory of the application. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseContentRoot( this WebApplicationBuilder builder, string contentRoot )
    {
        if ( contentRoot is null ) { throw new ArgumentNullException( nameof(contentRoot) ); }

        return builder.UseSetting( WebHostDefaults.ContentRootKey, contentRoot );
    }


    /// <summary> Specify the environment to be used by the web host. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="environment"> The environment to host the application in. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseEnvironment( this WebApplicationBuilder builder, string environment )
    {
        if ( environment is null ) { throw new ArgumentNullException( nameof(environment) ); }

        return builder.UseSetting( WebHostDefaults.EnvironmentKey, environment );
    }


    /// <summary> Specify the server to be used by the web host. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="server"> The <see cref="IServer"/> to be used. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseServer( this WebApplicationBuilder builder, IServer server )
    {
        if ( server is null ) { throw new ArgumentNullException( nameof(server) ); }

        // It would be nicer if this was transient but we need to pass in the factory instance directly
        return builder.AddSingleton( server );
    }


    /// <summary> Specify the amount of time to wait for the web host to shutdown. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="timeout"> The amount of time to wait for server shutdown. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseShutdownTimeout( this WebApplicationBuilder builder, TimeSpan timeout )
    {
        return builder.UseSetting( WebHostDefaults.ShutdownTimeoutKey, ((int)timeout.TotalSeconds).ToString( CultureInfo.InvariantCulture ) );
    }


    /// <summary> Specify the urls the web host will listen on. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="urls"> The urls the hosted application will listen on. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseUrls( this WebApplicationBuilder builder, params string[] urls )
    {
        if ( urls is null ) { throw new ArgumentNullException( nameof(urls) ); }

        return builder.UseSetting( WebHostDefaults.ServerUrlsKey, string.Join( ';', urls ) );
    }


    /// <summary> Specify the webroot directory to be used by the web host. </summary>
    /// <param name="builder"> The <see cref="WebApplicationBuilder"/> to configure. </param>
    /// <param name="webRoot"> Path to the root directory used by the web server. </param>
    /// <returns> The <see cref="WebApplicationBuilder"/> . </returns>
    public static WebApplicationBuilder UseWebRoot( this WebApplicationBuilder builder, string webRoot )
    {
        if ( webRoot is null ) { throw new ArgumentNullException( nameof(webRoot) ); }

        return builder.UseSetting( WebHostDefaults.WebRootKey, webRoot );
    }
}
