// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


#nullable enable
namespace Jakar.Extensions;


/// <summary>
/// <para><see href="https://www.stevejgordon.co.uk/httpclient-connection-pooling-in-dotnet-core"/></para>
/// </summary>
public partial class WebRequester
{
// ReSharper disable once ClassWithMembersNeverInherited.Global
    [SuppressMessage("ReSharper", "MemberNeverOverridden.Global")]
    [SuppressMessage("ReSharper", "ClassWithMembersNeverInherited.Global")]
    public struct Builder
    {
        private Encoding                   _encoding                 = Encoding.Default;
        private bool?                      _useProxy                 = default;
        private IWebProxy?                 _proxy                    = default;
        private bool?                      _allowAutoRedirect        = default;
        private int?                       _maxAutomaticRedirections = default;
        private ICredentials?              _defaultProxyCredentials  = default;
        private ICredentials?              _credentials              = default;
        private bool?                      _preAuthenticate          = default;
        private bool?                      _useCookies               = default;
        private CookieContainer?           _cookieContainer          = default;
        private int?                       _maxResponseHeadersLength = default;
        private int?                       _maxConnectionsPerServer  = default;
        private AuthenticationHeaderValue? _authenticationHeader     = default;


    #if NETSTANDARD2_1
    private SslProtocols?              _sslProtocols = default;
    private ClientCertificateOption?   _clientCertificateOptions = default;
    private X509CertificateCollection? _clientCertificates = default;
    private DecompressionMethods?      _automaticDecompression = default;
    private long?                      _maxRequestContentBufferSize = default;

    #else
        private SslClientAuthenticationOptions? _sslOptions                  = default;
        private TimeSpan?                       _responseDrainTimeout        = default;
        private TimeSpan?                       _connectTimeout              = default;
        private HttpKeepAlivePingPolicy?        _keepAlivePingPolicy         = default;
        private TimeSpan?                       _keepAlivePingTimeout        = default;
        private TimeSpan?                       _keepAlivePingDelay          = default;
        private TimeSpan?                       _pooledConnectionLifetime    = default;
        private TimeSpan?                       _pooledConnectionIdleTimeout = default;
        private int?                            _maxResponseDrainSize        = default;

    #endif


        private  IHostInfo? _hostInfo = default;
        internal Uri        Host => _hostInfo?.HostInfo ?? throw new ArgumentNullException(nameof(Host));


        public Builder() { }


        public static Builder Create() => new();
        public static Builder Create( Uri value ) => Create()
           .With_Host(value);
        public static Builder Create( IHostInfo value ) => Create()
           .With_Host(value);


        public Builder Reset()
        {
            _encoding                 = Encoding.Default;
            _useProxy                 = default;
            _proxy                    = default;
            _allowAutoRedirect        = default;
            _maxAutomaticRedirections = default;
            _defaultProxyCredentials  = default;
            _credentials              = default;
            _preAuthenticate          = default;
            _useCookies               = default;
            _cookieContainer          = default;
            _maxResponseHeadersLength = default;
            _maxConnectionsPerServer  = default;
            _authenticationHeader     = default;


        #if NETSTANDARD2_1
        _sslProtocols = default;
        _clientCertificateOptions = default;
        _clientCertificates = default;
        _automaticDecompression = default;
        _maxRequestContentBufferSize = default;

        #else
            _sslOptions                  = default;
            _responseDrainTimeout        = default;
            _connectTimeout              = default;
            _keepAlivePingPolicy         = default;
            _keepAlivePingTimeout        = default;
            _keepAlivePingDelay          = default;
            _pooledConnectionLifetime    = default;
            _pooledConnectionIdleTimeout = default;
            _maxResponseDrainSize        = default;

        #endif

            return this;
        }


        private HttpClient GetClient()
        {
            var client = new HttpClient(GetHandler());
            client.DefaultRequestHeaders.Authorization = _authenticationHeader;
            return client;
        }
        private HttpMessageHandler GetHandler()
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
        public WebRequester Build() => new(GetClient(), _hostInfo ?? throw new InvalidOperationException($"Must call {nameof(With_Host)}"), _encoding);



        internal readonly struct Placeholder : IHostInfo
        {
            public Uri HostInfo { get; }


            public Placeholder( Uri hostInfo ) => HostInfo = hostInfo;
        }



        public Builder With_Host( Uri value )
        {
            _hostInfo = new Placeholder(value);
            return this;
        }
        public Builder With_Host( IHostInfo value )
        {
            _hostInfo = value;
            return this;
        }
        public Builder With_Encoding( Encoding value )
        {
            _encoding = value;
            return this;
        }
        public Builder With_Proxy( IWebProxy value )
        {
            _proxy    = value;
            _useProxy = true;
            return this;
        }
        public Builder With_Proxy( IWebProxy value, ICredentials credentials )
        {
            _proxy                   = value;
            _useProxy                = true;
            _defaultProxyCredentials = credentials;
            return this;
        }


        public Builder With_MaxResponseHeadersLength( int value )
        {
            _maxResponseHeadersLength = value;
            return this;
        }
        public Builder With_MaxConnectionsPerServer( int value )
        {
            _maxConnectionsPerServer = value;
            return this;
        }
        public Builder With_MaxRedirects( int value )
        {
            _maxAutomaticRedirections = value;
            _allowAutoRedirect        = value > 0;
            return this;
        }


        public Builder With_Credentials( AuthenticationHeaderValue? value )
        {
            _authenticationHeader = value;
            return this;
        }
        public Builder With_Credentials( ICredentials? value )
        {
            _credentials     = value;
            _preAuthenticate = value is not null;
            return this;
        }
        public Builder With_Credentials( ICredentials? value, bool preAuthenticate )
        {
            _credentials     = value;
            _preAuthenticate = preAuthenticate;
            return this;
        }


        public Builder With_Cookie( Uri url, Cookie value )
        {
            _cookieContainer ??= new CookieContainer();
            _cookieContainer.Add(url, value);
            _useCookies = true;
            return this;
        }
        public Builder With_Cookie( params Cookie[] value )
        {
            CookieContainer container = _cookieContainer ??= new CookieContainer();
            foreach ( Cookie cookie in value ) { container.Add(cookie); }

            _useCookies = true;
            return this;
        }
        public Builder With_Cookie( CookieContainer value )
        {
            _cookieContainer = value;
            _useCookies      = true;
            return this;
        }


    #if NET6_0
        public Builder With_Timeout( int    minutes ) => With_Timeout(TimeSpan.FromMinutes(minutes));
        public Builder With_Timeout( float  seconds ) => With_Timeout(TimeSpan.FromSeconds(seconds));
        public Builder With_Timeout( double milliseconds ) => With_Timeout(TimeSpan.FromMilliseconds(milliseconds));
        public Builder With_Timeout( in TimeSpan value )
        {
            _connectTimeout = value;
            return this;
        }

        public Builder With_MaxResponseDrainSize( int value )
        {
            _maxResponseDrainSize = value;
            return this;
        }


        public Builder With_KeepAlive( int pingDelayMinutes, int pingTimeoutMinutes, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive(TimeSpan.FromMinutes(pingDelayMinutes), TimeSpan.FromMinutes(pingTimeoutMinutes), policy);
        public Builder With_KeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive(TimeSpan.FromSeconds(pingDelaySeconds), TimeSpan.FromSeconds(pingTimeoutSeconds), policy);
        public Builder With_KeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive(TimeSpan.FromMilliseconds(pingDelayMilliseconds), TimeSpan.FromMilliseconds(pingTimeoutMilliseconds), policy);
        public Builder With_KeepAlive( in TimeSpan pingDelay, in TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
        {
            _keepAlivePingDelay   = pingDelay;
            _keepAlivePingTimeout = pingTimeout;
            _keepAlivePingPolicy  = policy;
            return this;
        }
    #endif


    #if NET6_0
        public Builder With_SSL( SslClientAuthenticationOptions value )
        {
            _sslOptions = value;
            return this;
        }


        public Builder With_PooledConnectionIdleTimeout( int    pingDelayMinutes ) => With_PooledConnectionIdleTimeout(TimeSpan.FromMinutes(pingDelayMinutes));
        public Builder With_PooledConnectionIdleTimeout( float  pingDelaySeconds ) => With_PooledConnectionIdleTimeout(TimeSpan.FromSeconds(pingDelaySeconds));
        public Builder With_PooledConnectionIdleTimeout( double pingDelayMilliseconds ) => With_PooledConnectionIdleTimeout(TimeSpan.FromMilliseconds(pingDelayMilliseconds));
        public Builder With_PooledConnectionIdleTimeout( in TimeSpan value )
        {
            _pooledConnectionIdleTimeout = value;
            return this;
        }


        public Builder With_PooledConnectionLifetime( int    pingDelayMinutes ) => With_PooledConnectionLifetime(TimeSpan.FromMinutes(pingDelayMinutes));
        public Builder With_PooledConnectionLifetime( float  pingDelaySeconds ) => With_PooledConnectionLifetime(TimeSpan.FromSeconds(pingDelaySeconds));
        public Builder With_PooledConnectionLifetime( double pingDelayMilliseconds ) => With_PooledConnectionLifetime(TimeSpan.FromMilliseconds(pingDelayMilliseconds));
        public Builder With_PooledConnectionLifetime( in TimeSpan value )
        {
            _pooledConnectionLifetime = value;
            return this;
        }


        public Builder With_ResponseDrainTimeout( int    pingDelayMinutes ) => With_ResponseDrainTimeout(TimeSpan.FromMinutes(pingDelayMinutes));
        public Builder With_ResponseDrainTimeout( float  pingDelaySeconds ) => With_ResponseDrainTimeout(TimeSpan.FromSeconds(pingDelaySeconds));
        public Builder With_ResponseDrainTimeout( double pingDelayMilliseconds ) => With_ResponseDrainTimeout(TimeSpan.FromMilliseconds(pingDelayMilliseconds));
        public Builder With_ResponseDrainTimeout( in TimeSpan value )
        {
            _responseDrainTimeout = value;
            return this;
        }

    #else
    public Builder With_MaxRequestContentBufferSize( int value )
    {
        _maxRequestContentBufferSize = value;
        return this;
    }


    public Builder With_ClientCertificateOptions( ClientCertificateOption value )
    {
        _clientCertificateOptions = value;
        return this;
    }
    public Builder With_ClientCertificates( X509Certificate value )
    {
        _clientCertificates ??= new X509CertificateCollection();
        _clientCertificates.Add(value);
        return this;
    }
    public Builder With_ClientCertificates( params X509Certificate[] value )
    {
        _clientCertificates ??= new X509CertificateCollection();
        _clientCertificates.AddRange(value);
        return this;
    }
    public Builder With_ClientCertificates( IEnumerable<X509Certificate> value )
    {
        _clientCertificates ??= new X509CertificateCollection();
        foreach ( X509Certificate certificate in value ) { _clientCertificates.Add(certificate); }

        return this;
    }
    public Builder With_ClientCertificates( X509CertificateCollection value )
    {
        _clientCertificates = value;
        return this;
    }


    public Builder With_AutomaticDecompression( DecompressionMethods value )
    {
        _automaticDecompression = value;
        return this;
    }


    public Builder With_SSL( SslProtocols value )
    {
        _sslProtocols = value;
        return this;
    }
    #endif
    }
}
