// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://www.stevejgordon.co.uk/httpclient-connection-pooling--dotnet-core"/>
///     </para>
/// </summary>
public partial class WebRequester
{
    [SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
    public class Builder( IHostInfo value ) : IHttpClientFactory
    {
        private readonly IHostInfo                       _hostInfo = value;
        private readonly WebHeaders                      _headers  = [];
        private          AuthenticationHeaderValue?      _authenticationHeader;
        private          bool?                           _allowAutoRedirect;
        private          bool?                           _preAuthenticate;
        private          bool?                           _useCookies;
        private          bool?                           _useProxy;
        private          CookieContainer?                _cookieContainer;
        private          Encoding                        _encoding = Encoding.Default;
        private          HttpKeepAlivePingPolicy?        _keepAlivePingPolicy;
        private          ICredentials?                   _credentials;
        private          ICredentials?                   _defaultProxyCredentials;
        private          ILogger?                        _logger;
        private          int?                            _maxAutomaticRedirections;
        private          int?                            _maxConnectionsPerServer;
        private          int?                            _maxResponseContentBufferSize;
        private          int?                            _maxResponseDrainSize;
        private          int?                            _maxResponseHeadersLength;
        private          IWebProxy?                      _proxy;
        private          RetryPolicy?                    _retryPolicy;
        private          SslClientAuthenticationOptions? _sslOptions;
        private          TimeSpan?                       _connectTimeout;
        private          TimeSpan?                       _keepAlivePingDelay;
        private          TimeSpan?                       _keepAlivePingTimeout;
        private          TimeSpan?                       _pooledConnectionIdleTimeout;
        private          TimeSpan?                       _pooledConnectionLifetime;
        private          TimeSpan?                       _responseDrainTimeout;


        public static Builder Create( IHostInfo       value ) => new(value);
        public static Builder Create( Uri             value ) => Create( new HostHolder( value ) );
        public static Builder Create( Func<IHostInfo> value ) => Create( new HostHolder( value ) );
        public static Builder Create( Func<Uri>       value ) => Create( new HostHolder( value ) );


        [Pure] public Builder Reset() => Create( _hostInfo );


        protected virtual HttpClient GetClient()
        {
            HttpClient client = new(GetHandler());
            foreach ( (string key, IEnumerable<string> value) in _headers ) { client.DefaultRequestHeaders.Add( key, value ); }

            client.DefaultRequestHeaders.Authorization = _authenticationHeader;
            if ( _connectTimeout.HasValue ) { client.Timeout = _connectTimeout.Value; }

            if ( _maxResponseContentBufferSize.HasValue ) { client.MaxResponseContentBufferSize = _maxResponseContentBufferSize.Value; }

            return client;
        }
        protected virtual HttpMessageHandler GetHandler()
        {
            SocketsHttpHandler handler = new();

            if ( _connectTimeout.HasValue ) { handler.ConnectTimeout = _connectTimeout.Value; }

            if ( _keepAlivePingPolicy.HasValue ) { handler.KeepAlivePingPolicy = _keepAlivePingPolicy.Value; }

            if ( _keepAlivePingTimeout.HasValue ) { handler.KeepAlivePingTimeout = _keepAlivePingTimeout.Value; }

            if ( _keepAlivePingDelay.HasValue ) { handler.KeepAlivePingDelay = _keepAlivePingDelay.Value; }

            if ( _sslOptions is not null ) { handler.SslOptions = _sslOptions; }

            if ( _maxResponseDrainSize.HasValue ) { handler.MaxResponseDrainSize = _maxResponseDrainSize.Value; }

            if ( _responseDrainTimeout.HasValue ) { handler.ResponseDrainTimeout = _responseDrainTimeout.Value; }

            if ( _pooledConnectionLifetime.HasValue ) { handler.PooledConnectionLifetime = _pooledConnectionLifetime.Value; }

            if ( _pooledConnectionIdleTimeout.HasValue ) { handler.PooledConnectionIdleTimeout = _pooledConnectionIdleTimeout.Value; }

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
        public WebRequester Build()                     => new(GetClient(), _hostInfo, _logger, _encoding) { Retries = _retryPolicy };
        public HttpClient   CreateClient( string name ) => GetClient();


        public Builder With_Logger( ILogger logger )
        {
            _logger = logger;
            return this;
        }


        public Builder With_Header( string name, IEnumerable<string?> values )
        {
            _headers.Add( name, values );
            return this;
        }
        public Builder With_Header( string name, string? value )
        {
            _headers.Add( name, value );
            return this;
        }


        public Builder With_MaxResponseContentBufferSize( int value )
        {
            _maxResponseContentBufferSize = Math.Max( 0, value );
            return this;
        }


        public Builder With_Retry()                                                        => With_Retry( RetryPolicy.Default );
        public Builder With_Retry( ushort   maxRetires )                                   => With_Retry( RetryPolicy.Create( maxRetires ) );
        public Builder With_Retry( TimeSpan delay, TimeSpan scale, ushort maxRetires = 3 ) => With_Retry( new RetryPolicy( delay, scale, maxRetires ) );
        public Builder With_Retry( RetryPolicy policy )
        {
            _retryPolicy = policy;
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


        public Builder With_Credentials( string scheme, string? value ) => With_Credentials( new AuthenticationHeaderValue( scheme, value ) );
        public Builder With_Credentials( AuthenticationHeaderValue? value )
        {
            _authenticationHeader = value;
            return this;
        }
        public Builder With_Credentials( ICredentials? value ) => With_Credentials( value, value is not null );
        public Builder With_Credentials( ICredentials? value, bool preAuthenticate )
        {
            _credentials     = value;
            _preAuthenticate = preAuthenticate;
            return this;
        }


        public Builder With_Cookie( Uri url, Cookie value )
        {
            _cookieContainer ??= new CookieContainer();
            _cookieContainer.Add( url, value );
            _useCookies = true;
            return this;
        }
        public Builder With_Cookie( params ReadOnlySpan<Cookie> value )
        {
            CookieContainer container = _cookieContainer ??= new CookieContainer();
            foreach ( Cookie cookie in value ) { container.Add( cookie ); }

            _useCookies = true;
            return this;
        }
        public Builder With_Cookie( CookieContainer value )
        {
            _cookieContainer = value;
            _useCookies      = true;
            return this;
        }


        public Builder With_Timeout( int    minutes )      => With_Timeout( TimeSpan.FromMinutes( minutes ) );
        public Builder With_Timeout( float  seconds )      => With_Timeout( TimeSpan.FromSeconds( seconds ) );
        public Builder With_Timeout( double milliseconds ) => With_Timeout( TimeSpan.FromMilliseconds( milliseconds ) );
        public Builder With_Timeout( TimeSpan value )
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
            With_KeepAlive( TimeSpan.FromMinutes( pingDelayMinutes ), TimeSpan.FromMinutes( pingTimeoutMinutes ), policy );
        public Builder With_KeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive( TimeSpan.FromSeconds( pingDelaySeconds ), TimeSpan.FromSeconds( pingTimeoutSeconds ), policy );
        public Builder With_KeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive( TimeSpan.FromMilliseconds( pingDelayMilliseconds ), TimeSpan.FromMilliseconds( pingTimeoutMilliseconds ), policy );
        public Builder With_KeepAlive( TimeSpan pingDelay, TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
        {
            _keepAlivePingDelay   = pingDelay;
            _keepAlivePingTimeout = pingTimeout;
            _keepAlivePingPolicy  = policy;
            return this;
        }


        public Builder With_SSL( SslClientAuthenticationOptions value )
        {
            _sslOptions = value;
            return this;
        }


        public Builder With_PooledConnectionIdleTimeout( int    minutes )      => With_PooledConnectionIdleTimeout( TimeSpan.FromMinutes( minutes ) );
        public Builder With_PooledConnectionIdleTimeout( float  seconds )      => With_PooledConnectionIdleTimeout( TimeSpan.FromSeconds( seconds ) );
        public Builder With_PooledConnectionIdleTimeout( double milliseconds ) => With_PooledConnectionIdleTimeout( TimeSpan.FromMilliseconds( milliseconds ) );
        public Builder With_PooledConnectionIdleTimeout( TimeSpan value )
        {
            _pooledConnectionIdleTimeout = value;
            return this;
        }


        public Builder With_PooledConnectionLifetime( int    minutes )      => With_PooledConnectionLifetime( TimeSpan.FromMinutes( minutes ) );
        public Builder With_PooledConnectionLifetime( float  seconds )      => With_PooledConnectionLifetime( TimeSpan.FromSeconds( seconds ) );
        public Builder With_PooledConnectionLifetime( double milliseconds ) => With_PooledConnectionLifetime( TimeSpan.FromMilliseconds( milliseconds ) );
        public Builder With_PooledConnectionLifetime( TimeSpan value )
        {
            _pooledConnectionLifetime = value;
            return this;
        }


        public Builder With_ResponseDrainTimeout( int    minutes )      => With_ResponseDrainTimeout( TimeSpan.FromMinutes( minutes ) );
        public Builder With_ResponseDrainTimeout( float  seconds )      => With_ResponseDrainTimeout( TimeSpan.FromSeconds( seconds ) );
        public Builder With_ResponseDrainTimeout( double milliseconds ) => With_ResponseDrainTimeout( TimeSpan.FromMilliseconds( milliseconds ) );
        public Builder With_ResponseDrainTimeout( TimeSpan value )
        {
            _responseDrainTimeout = value;
            return this;
        }



        public sealed class HostHolder( OneOf<Uri, Func<Uri>, Func<IHostInfo>> hostInfo ) : IHostInfo
        {
            private readonly OneOf<Uri, Func<Uri>, Func<IHostInfo>> _hostInfo = hostInfo;
            public           Uri                                    HostInfo => _hostInfo.Match( static x => x, static x => x(), static x => x().HostInfo );
        }
    }
}
