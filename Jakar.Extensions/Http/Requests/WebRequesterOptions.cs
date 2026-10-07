// Jakar.Extensions :: Jakar.Extensions
// 10/01/2026

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;



namespace Jakar.Extensions;


/// <summary>
///     Settings for a <see cref="WebRequester"/> built from dependency injection (see <see cref="WebRequesterServiceCollectionExtensions.AddWebRequester(IServiceCollection, IConfiguration)"/>).
///     Every property is optional: unset values keep the <see cref="WebRequester.Builder"/> defaults. All but <see cref="ConfigureBuilder"/> can be bound from configuration, e.g.
///     <code>
///     "WebRequester": {
///       "BaseAddress": "https://api.example.com/",
///       "Timeout": "00:00:30",
///       "MaxConnectionsPerServer": 16,
///       "Retry": { "MaxRetries": 3, "Delay": "00:00:01", "Scale": "00:00:01" },
///       "DefaultHeaders": { "X-Api-Version": "2" }
///     }
///     </code>
/// </summary>
public sealed class WebRequesterOptions
{
    public static WebRequesterOptions Default { get; set; } = new();

    /// <summary> Base address for relative paths. When <see langword="null"/>, the registered <see cref="IHostInfo"/> is used. </summary>
    public Uri? BaseAddress { get; set; }

    /// <summary> Whole-request timeout (<see cref="HttpClient.Timeout"/>). </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary> Time allowed to establish a connection. </summary>
    public TimeSpan? ConnectTimeout { get; set; }

    /// <summary> How long a pooled connection may be reused (default <see cref="WebRequester.Builder.DefaultPooledConnectionLifetime"/>), so DNS changes are picked up. </summary>
    public TimeSpan? PooledConnectionLifetime { get; set; }

    /// <summary> How long an idle pooled connection is kept. </summary>
    public TimeSpan? PooledConnectionIdleTimeout { get; set; }

    public int? MaxConnectionsPerServer { get; set; }

    /// <summary> Maximum automatic redirects; 0 disables redirects. </summary>
    public int? MaxAutomaticRedirections { get; set; }

    public int? MaxResponseContentBufferSize { get; set; }

    public int? MaxResponseHeadersLength { get; set; }

    /// <summary> Response encodings decompressed automatically (default <see cref="DecompressionMethods.All"/>). Configuration accepts e.g. <c>"GZip, Brotli"</c>. </summary>
    public DecompressionMethods? AutomaticDecompression { get; set; }

    /// <summary> Text/JSON body encoding by name (<see cref="System.Text.Encoding.WebName"/>, e.g. <c>"utf-8"</c>, <c>"utf-16"</c>); default <see cref="System.Text.Encoding.Default"/>. </summary>
    public string? Encoding { get; set; }

    /// <summary> Retries; when <see langword="null"/>, requests are not retried. <see cref="RetryOptions"/> defaults to <see cref="WebRequester.RetryPolicy.Default"/>'s values. </summary>
    public RetryOptions? Retry { get; set; }

    /// <summary> Headers sent with every request (e.g. API version or key). </summary>
    public Dictionary<string, string> DefaultHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool? UseCookies { get; set; }

    public TimeSpan? KeepAlivePingDelay { get; set; }

    public TimeSpan? KeepAlivePingTimeout { get; set; }

    public HttpKeepAlivePingPolicy? KeepAlivePingPolicy { get; set; }

    /// <summary> Code-only settings that configuration can't express (SSL callbacks, proxy, credentials, cookies, ...). Applied after the other options. </summary>
    public Action<WebRequester.Builder>? ConfigureBuilder { get; set; }


    /// <summary> The configured <see cref="Encoding"/>, or <see cref="System.Text.Encoding.Default"/>. </summary>
    public Encoding GetEncoding() => string.IsNullOrWhiteSpace(Encoding)
                                         ? System.Text.Encoding.Default
                                         : System.Text.Encoding.GetEncoding(Encoding);


    /// <summary> Problems with these options, if any (used by <see cref="WebRequesterServiceCollectionExtensions.AddWebRequester(IServiceCollection, string, Action{WebRequesterOptions})"/> validation). </summary>
    public IEnumerable<string> Validate()
    {
        if ( BaseAddress is { IsAbsoluteUri: false } ) { yield return $"{nameof(BaseAddress)} must be an absolute URI."; }

        if ( Timeout <= TimeSpan.Zero && Timeout != System.Threading.Timeout.InfiniteTimeSpan ) { yield return $"{nameof(Timeout)} must be positive or infinite."; }

        if ( ConnectTimeout <= TimeSpan.Zero && ConnectTimeout != System.Threading.Timeout.InfiniteTimeSpan ) { yield return $"{nameof(ConnectTimeout)} must be positive or infinite."; }

        if ( MaxConnectionsPerServer <= 0 ) { yield return $"{nameof(MaxConnectionsPerServer)} must be positive."; }

        if ( MaxAutomaticRedirections < 0 ) { yield return $"{nameof(MaxAutomaticRedirections)} cannot be negative."; }

        if ( Retry is { Delay: var delay, Scale: var scale } && ( delay < TimeSpan.Zero || scale < TimeSpan.Zero ) ) { yield return $"{nameof(Retry)} delays cannot be negative."; }

        if ( string.IsNullOrWhiteSpace(Encoding) ) { yield break; }

        string? encodingError = null;

        try { System.Text.Encoding.GetEncoding(Encoding); }
        catch ( ArgumentException ) { encodingError = $"{nameof(Encoding)} '{Encoding}' is not a known encoding."; }

        if ( encodingError is not null ) { yield return encodingError; }
    }



    /// <summary> Bindable form of <see cref="WebRequester.RetryPolicy"/>. </summary>
    public sealed class RetryOptions
    {
        /// <summary> Retries after the first attempt. </summary>
        public ushort MaxRetries { get; set; } = WebRequester.RetryPolicy.Default.MaxRetires;

        public TimeSpan Delay { get; set; } = WebRequester.RetryPolicy.Default.Delay;

        public TimeSpan Scale { get; set; } = WebRequester.RetryPolicy.Default.Scale;

        public WebRequester.RetryPolicy ToPolicy() => new(Delay, Scale, MaxRetries);
    }
}



public static class WebRequesterServiceCollectionExtensions
{
    /// <summary> Signals <see cref="IOptionsMonitor{TOptions}"/> when a configuration section reloads. </summary>
    private sealed class ConfigurationChangeTokenSource( string name, IConfiguration section ) : IOptionsChangeTokenSource<WebRequesterOptions>
    {
        public string       Name             => name;
        public IChangeToken GetChangeToken() => section.GetReloadToken();
    }



    /// <summary> Registers a singleton <see cref="WebRequester"/> configured by <paramref name="configure"/>. </summary>
    public static OptionsBuilder<WebRequesterOptions> AddWebRequester( this IServiceCollection services, Action<WebRequesterOptions>? configure = null ) => services.AddWebRequester(Options.DefaultName, configure);

    /// <summary> Registers a singleton <see cref="WebRequester"/> bound to <paramref name="section"/> (e.g. <c>configuration.GetSection("WebRequester")</c>). </summary>
    public static OptionsBuilder<WebRequesterOptions> AddWebRequester( this IServiceCollection services, IConfiguration section ) => services.AddWebRequester(Options.DefaultName, section);

    /// <summary> Registers a <see cref="WebRequester"/> bound to <paramref name="section"/>; a non-default <paramref name="name"/> registers a keyed singleton (resolve with <c>[FromKeyedServices(name)]</c>). </summary>
    /// <remarks> The requester reloads when <paramref name="section"/> changes (e.g. <c>appsettings.json</c> with <c>reloadOnChange</c>). </remarks>
    public static OptionsBuilder<WebRequesterOptions> AddWebRequester( this IServiceCollection services, string name, IConfiguration section )
    {
        ArgumentNullException.ThrowIfNull(section);

        // Lets IOptionsMonitor see configuration reloads (Configure(...) alone does not).
        services.AddSingleton<IOptionsChangeTokenSource<WebRequesterOptions>>(new ConfigurationChangeTokenSource(name, section));

        return services.AddWebRequester(name, options => section.Bind(options)); // a lambda (not a method group) so the configuration binding generator can intercept it
    }

    /// <summary>
    ///     Registers a singleton <see cref="WebRequester"/> (one shared connection pool) configured by <paramref name="configure"/>; a non-default <paramref name="name"/> registers a keyed
    ///     singleton (resolve with <c>[FromKeyedServices(name)]</c>). The options are validated when the requester is first created; chain <c>.ValidateOnStart()</c> to validate at startup.
    /// </summary>
    public static OptionsBuilder<WebRequesterOptions> AddWebRequester( this IServiceCollection services, string name, Action<WebRequesterOptions>? configure )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);

        OptionsBuilder<WebRequesterOptions> options = services.AddOptions<WebRequesterOptions>(name);
        if ( configure is not null ) { options.Configure(configure); }

        // Validation happens in WebRequester.Builder.Apply, not as an options validator: a registered validator makes IOptionsMonitor throw while handling a configuration
        // reload (on the file-watcher thread, before any listener runs). Invalid options still throw when the requester is created; an invalid reload is logged and ignored.

        if ( name == Options.DefaultName ) { services.TryAddSingleton(static provider => WebRequester.Create(provider)); }
        else { services.TryAddKeyedSingleton(name, static ( provider, key ) => WebRequester.Create(provider, (string)key!)); }

        return options;
    }
}
