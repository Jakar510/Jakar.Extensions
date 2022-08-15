// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


#nullable enable
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;



namespace Jakar.Extensions;


/// <summary>
/// <para><see href="https://www.stevejgordon.co.uk/httpclient-connection-pooling-in-dotnet-core"/></para>
/// </summary>

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class WebRequestBuilder
{
    protected Encoding                   _encoding = Encoding.Default;
    protected bool?                      _useProxy;
    protected IWebProxy?                 _proxy;
    protected bool?                      _allowAutoRedirect;
    protected int?                       _maxAutomaticRedirections;
    protected ICredentials?              _defaultProxyCredentials;
    protected ICredentials?              _credentials;
    protected bool?                      _preAuthenticate;
    protected bool?                      _useCookies;
    protected CookieContainer?           _cookieContainer;
    protected int?                       _maxResponseHeadersLength;
    protected int?                       _maxConnectionsPerServer;
    protected AuthenticationHeaderValue? _authenticationHeader;

#if NETSTANDARD2_1
    protected SslProtocols?              _sslProtocols;
    protected ClientCertificateOption?   _clientCertificateOptions;
    protected X509CertificateCollection? _clientCertificates;
    protected DecompressionMethods?      _automaticDecompression;
    protected long?            _maxRequestContentBufferSize;

#else
    protected SslClientAuthenticationOptions? _sslOptions;
    protected TimeSpan?                       _responseDrainTimeout;
    protected TimeSpan?                       _connectTimeout;
    protected HttpKeepAlivePingPolicy?        _keepAlivePingPolicy;
    protected TimeSpan?                       _keepAlivePingTimeout;
    protected TimeSpan?                       _keepAlivePingDelay;
    protected TimeSpan?                       _pooledConnectionLifetime;
    protected TimeSpan?                       _pooledConnectionIdleTimeout;
    protected int?                            _maxResponseDrainSize;

#endif


    protected          IHostInfo? _hostInfo;
    protected          Uri?       _host;
    protected internal Uri        Host => _hostInfo?.HostInfo ?? _host ?? throw new ArgumentNullException(nameof(Host));


    public static WebRequestBuilder Create() => new();
    public static WebRequestBuilder Create( Encoding  value ) => Create().With_Encoding(value);
    public static WebRequestBuilder Create( Uri       value ) => Create().With_Host(value);
    public static WebRequestBuilder Create( Uri       value, Encoding encoding ) => Create(encoding).With_Host(value);
    public static WebRequestBuilder Create( IHostInfo value ) => Create().With_Host(value);
    public static WebRequestBuilder Create( IHostInfo value, Encoding encoding ) => Create(encoding).With_Host(value);


    public virtual void Reset()
    {
        _useCookies               = default;
        _preAuthenticate          = default;
        _cookieContainer          = default;
        _credentials              = default;
        _defaultProxyCredentials  = default;
        _maxAutomaticRedirections = default;
        _allowAutoRedirect        = default;
        _proxy                    = default;
        _useProxy                 = default;

    #if NETSTANDARD2_1
        _sslProtocols = default;
    #else
        _connectTimeout       = default;
        _sslOptions           = default;
        _keepAlivePingDelay   = default;
        _keepAlivePingPolicy  = default;
        _keepAlivePingTimeout = default;
    #endif
    }


    protected virtual HttpClient GetClient()
    {
        var client = new HttpClient(GetHandler());
        client.DefaultRequestHeaders.Authorization = _authenticationHeader;
        return client;
    }
    protected virtual HttpMessageHandler GetHandler()
    {
    #if NETSTANDARD2_1
        var handler = new HttpClientHandler();


        if ( _sslProtocols.HasValue ) { handler.SslProtocols = _sslProtocols.Value; }

        if ( _maxRequestContentBufferSize.HasValue ) { handler.MaxRequestContentBufferSize = _maxRequestContentBufferSize.Value; }

        if ( _automaticDecompression.HasValue ) { handler.AutomaticDecompression = _automaticDecompression.Value; }

        if ( _clientCertificateOptions.HasValue ) { handler.ClientCertificateOptions = _clientCertificateOptions.Value; }

        if ( _clientCertificates is not null ) { handler.ClientCertificates.AddRange(_clientCertificates); }

    #else
        var handler = new SocketsHttpHandler();


        if ( _connectTimeout.HasValue ) { handler.ConnectTimeout = _connectTimeout.Value; }

        if ( _keepAlivePingPolicy.HasValue ) { handler.KeepAlivePingPolicy = _keepAlivePingPolicy.Value; }

        if ( _keepAlivePingTimeout.HasValue ) { handler.KeepAlivePingTimeout = _keepAlivePingTimeout.Value; }

        if ( _keepAlivePingDelay.HasValue ) { handler.KeepAlivePingDelay = _keepAlivePingDelay.Value; }

        if ( _sslOptions is not null ) { handler.SslOptions = _sslOptions; }

        if ( _maxResponseDrainSize.HasValue ) { handler.MaxResponseDrainSize = _maxResponseDrainSize.Value; }

        if ( _responseDrainTimeout.HasValue ) { handler.ResponseDrainTimeout = _responseDrainTimeout.Value; }

        if ( _pooledConnectionLifetime.HasValue ) { handler.PooledConnectionLifetime = _pooledConnectionLifetime.Value; }

        if ( _pooledConnectionIdleTimeout.HasValue ) { handler.PooledConnectionIdleTimeout = _pooledConnectionIdleTimeout.Value; }

    #endif


        if ( _maxResponseHeadersLength.HasValue ) { handler.MaxResponseHeadersLength = _maxResponseHeadersLength.Value; }

        if ( _maxConnectionsPerServer.HasValue ) { handler.MaxConnectionsPerServer = _maxConnectionsPerServer.Value; }

        if ( _maxAutomaticRedirections.HasValue ) { handler.MaxAutomaticRedirections = _maxAutomaticRedirections.Value; }

        if ( _allowAutoRedirect.HasValue ) { handler.AllowAutoRedirect = _allowAutoRedirect.Value; }


        if ( _useProxy.HasValue ) { handler.UseProxy = _useProxy.Value; }

        if ( _proxy is not null ) { handler.Proxy = _proxy; }

        if ( _defaultProxyCredentials is not null ) { handler.DefaultProxyCredentials = _defaultProxyCredentials; }


        if ( _credentials is not null ) { handler.Credentials = _credentials; }

        if ( _preAuthenticate.HasValue ) { handler.PreAuthenticate = _preAuthenticate.Value; }


        if ( _useCookies.HasValue ) { handler.UseCookies = _useCookies.Value; }

        if ( _cookieContainer is not null ) { handler.CookieContainer = _cookieContainer; }

        return handler;
    }


    public virtual WebRequestBuilder With_Host( Uri value )
    {
        _host = value;
        return this;
    }
    public virtual WebRequestBuilder With_Host( IHostInfo value )
    {
        _hostInfo = value;
        return this;
    }
    public virtual WebRequestBuilder With_Encoding( Encoding value )
    {
        _encoding = value;
        return this;
    }
    public virtual WebRequestBuilder With_Proxy( IWebProxy value )
    {
        _proxy    = value;
        _useProxy = true;
        return this;
    }
    public virtual WebRequestBuilder With_Proxy( IWebProxy value, ICredentials credentials )
    {
        _proxy                   = value;
        _useProxy                = true;
        _defaultProxyCredentials = credentials;
        return this;
    }


    public virtual WebRequestBuilder With_MaxResponseHeadersLength( int value )
    {
        _maxResponseHeadersLength = value;
        return this;
    }
    public virtual WebRequestBuilder With_MaxConnectionsPerServer( int value )
    {
        _maxConnectionsPerServer = value;
        return this;
    }
    public virtual WebRequestBuilder With_MaxRedirects( int value )
    {
        _maxAutomaticRedirections = value;
        _allowAutoRedirect        = value > 0;
        return this;
    }


    public virtual WebRequestBuilder With_Credentials( AuthenticationHeaderValue? value )
    {
        _authenticationHeader = value;
        return this;
    }
    public virtual WebRequestBuilder With_Credentials( ICredentials? value )
    {
        _credentials     = value;
        _preAuthenticate = value is not null;
        return this;
    }
    public virtual WebRequestBuilder With_Credentials( ICredentials? value, bool preAuthenticate )
    {
        _credentials     = value;
        _preAuthenticate = preAuthenticate;
        return this;
    }


    public virtual WebRequestBuilder With_Cookie( Uri url, Cookie value )
    {
        _cookieContainer ??= new CookieContainer();
        _cookieContainer.Add(url, value);
        _useCookies = true;
        return this;
    }
    public virtual WebRequestBuilder With_Cookie( params Cookie[] value )
    {
        CookieContainer container = _cookieContainer ??= new CookieContainer();
        foreach ( Cookie cookie in value ) { container.Add(cookie); }

        _useCookies = true;
        return this;
    }
    public virtual WebRequestBuilder With_Cookie( CookieContainer value )
    {
        _cookieContainer = value;
        _useCookies      = true;
        return this;
    }


#if NET6_0
    public virtual WebRequestBuilder With_Timeout( int    minutes ) => With_Timeout(TimeSpan.FromMinutes(minutes));
    public virtual WebRequestBuilder With_Timeout( float  seconds ) => With_Timeout(TimeSpan.FromSeconds(seconds));
    public virtual WebRequestBuilder With_Timeout( double milliseconds ) => With_Timeout(TimeSpan.FromMilliseconds(milliseconds));
    public virtual WebRequestBuilder With_Timeout( in TimeSpan value )
    {
        _connectTimeout = value;
        return this;
    }

    public virtual WebRequestBuilder With_MaxResponseDrainSize( int value )
    {
        _maxResponseDrainSize = value;
        return this;
    }


    public virtual WebRequestBuilder With_KeepAlive( int pingDelayMinutes, int pingTimeoutMinutes, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromMinutes(pingDelayMinutes), TimeSpan.FromMinutes(pingTimeoutMinutes), policy);
    public virtual WebRequestBuilder With_KeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromSeconds(pingDelaySeconds), TimeSpan.FromSeconds(pingTimeoutSeconds), policy);
    public virtual WebRequestBuilder With_KeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromMilliseconds(pingDelayMilliseconds), TimeSpan.FromMilliseconds(pingTimeoutMilliseconds), policy);
    public virtual WebRequestBuilder With_KeepAlive( in TimeSpan pingDelay, in TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
    {
        _keepAlivePingDelay   = pingDelay;
        _keepAlivePingTimeout = pingTimeout;
        _keepAlivePingPolicy  = policy;
        return this;
    }
#endif


#if NET6_0
    public virtual WebRequestBuilder With_SSL( SslClientAuthenticationOptions value )
    {
        _sslOptions = value;
        return this;
    }


    public virtual WebRequestBuilder With_PooledConnectionIdleTimeout( int    pingDelayMinutes ) => With_PooledConnectionIdleTimeout(TimeSpan.FromMinutes(pingDelayMinutes));
    public virtual WebRequestBuilder With_PooledConnectionIdleTimeout( float  pingDelaySeconds ) => With_PooledConnectionIdleTimeout(TimeSpan.FromSeconds(pingDelaySeconds));
    public virtual WebRequestBuilder With_PooledConnectionIdleTimeout( double pingDelayMilliseconds ) => With_PooledConnectionIdleTimeout(TimeSpan.FromMilliseconds(pingDelayMilliseconds));
    public virtual WebRequestBuilder With_PooledConnectionIdleTimeout( in TimeSpan value )
    {
        _pooledConnectionIdleTimeout = value;
        return this;
    }


    public virtual WebRequestBuilder With_PooledConnectionLifetime( int    pingDelayMinutes ) => With_PooledConnectionLifetime(TimeSpan.FromMinutes(pingDelayMinutes));
    public virtual WebRequestBuilder With_PooledConnectionLifetime( float  pingDelaySeconds ) => With_PooledConnectionLifetime(TimeSpan.FromSeconds(pingDelaySeconds));
    public virtual WebRequestBuilder With_PooledConnectionLifetime( double pingDelayMilliseconds ) => With_PooledConnectionLifetime(TimeSpan.FromMilliseconds(pingDelayMilliseconds));
    public virtual WebRequestBuilder With_PooledConnectionLifetime( in TimeSpan value )
    {
        _pooledConnectionLifetime = value;
        return this;
    }


    public virtual WebRequestBuilder With_ResponseDrainTimeout( int    pingDelayMinutes ) => With_ResponseDrainTimeout(TimeSpan.FromMinutes(pingDelayMinutes));
    public virtual WebRequestBuilder With_ResponseDrainTimeout( float  pingDelaySeconds ) => With_ResponseDrainTimeout(TimeSpan.FromSeconds(pingDelaySeconds));
    public virtual WebRequestBuilder With_ResponseDrainTimeout( double pingDelayMilliseconds ) => With_ResponseDrainTimeout(TimeSpan.FromMilliseconds(pingDelayMilliseconds));
    public virtual WebRequestBuilder With_ResponseDrainTimeout( in TimeSpan value )
    {
        _responseDrainTimeout = value;
        return this;
    }

#else
    public virtual WebRequestBuilder With_MaxRequestContentBufferSize( int value )
    {
        _maxRequestContentBufferSize = value;
        return this;
    }


    public virtual WebRequestBuilder With_ClientCertificateOptions( ClientCertificateOption value )
    {
        _clientCertificateOptions = value;
        return this;
    }
    public virtual WebRequestBuilder With_ClientCertificates( X509Certificate value )
    {
        _clientCertificates ??= new X509CertificateCollection();
        _clientCertificates.Add(value);
        return this;
    }
    public virtual WebRequestBuilder With_ClientCertificates( params X509Certificate[] value )
    {
        _clientCertificates ??= new X509CertificateCollection();
        _clientCertificates.AddRange(value);
        return this;
    }
    public virtual WebRequestBuilder With_ClientCertificates( IEnumerable<X509Certificate> value )
    {
        _clientCertificates ??= new X509CertificateCollection();
        foreach ( X509Certificate certificate in value ) { _clientCertificates.Add(certificate); }

        return this;
    }
    public virtual WebRequestBuilder With_ClientCertificates( X509CertificateCollection value )
    {
        _clientCertificates = value;
        return this;
    }


    public virtual WebRequestBuilder With_AutomaticDecompression( DecompressionMethods value )
    {
        _automaticDecompression = value;
        return this;
    }


    public virtual WebRequestBuilder With_SSL( SslProtocols value )
    {
        _sslProtocols = value;
        return this;
    }
#endif

    public virtual WebRequester Build() => new(GetClient(), _encoding, Host);
}