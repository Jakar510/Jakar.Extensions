﻿// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://www.stevejgordon.co.uk/httpclient-connection-pooling--dotnet-core"/>
///     </para>
/// </summary>
public partial class WebRequester
{
// ReSharper disable once ClassWithMembersNeverInherited.Global
    public ref struct Builder
    {
        private readonly WebHeaders                 _headers = new();
        private readonly IHostInfo                  _hostInfo;
        private          Encoding                   _encoding                     = Encoding.Default;
        private          bool?                      _useProxy                     = default;
        private          IWebProxy?                 _proxy                        = default;
        private          bool?                      _allowAutoRedirect            = default;
        private          int?                       _maxAutomaticRedirections     = default;
        private          ICredentials?              _defaultProxyCredentials      = default;
        private          ICredentials?              _credentials                  = default;
        private          bool?                      _preAuthenticate              = default;
        private          bool?                      _useCookies                   = default;
        private          CookieContainer?           _cookieContainer              = default;
        private          int?                       _maxResponseHeadersLength     = default;
        private          int?                       _maxConnectionsPerServer      = default;
        private          AuthenticationHeaderValue? _authenticationHeader         = default;
        private          TimeSpan?                  _connectTimeout               = default;
        private          RetryPolicy?               _retryPolicy                  = default;
        private          int?                       _maxResponseContentBufferSize = default;


    #if NETSTANDARD2_1
        private SslProtocols?              _sslProtocols                = default;
        private ClientCertificateOption?   _clientCertificateOptions    = default;
        private X509CertificateCollection? _clientCertificates          = default;
        private DecompressionMethods?      _automaticDecompression      = default;
        private long?                      _maxRequestContentBufferSize = default;

    #else
        private SslClientAuthenticationOptions? _sslOptions = default;
        private TimeSpan?                       _responseDrainTimeout = default;
        private HttpKeepAlivePingPolicy?        _keepAlivePingPolicy = default;
        private TimeSpan?                       _keepAlivePingTimeout = default;
        private TimeSpan?                       _keepAlivePingDelay = default;
        private TimeSpan?                       _pooledConnectionLifetime = default;
        private TimeSpan?                       _pooledConnectionIdleTimeout = default;
        private int?                            _maxResponseDrainSize = default;

    #endif


        public Builder( IHostInfo value ) => _hostInfo = value;


        public static Builder Create( IHostInfo       value ) => new(value);
        public static Builder Create( Uri             value ) => new(new HostHolder( value ));
        public static Builder Create( Func<IHostInfo> value ) => new(new MethodHolder( value ));


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
            _sslProtocols                = default;
            _clientCertificateOptions    = default;
            _clientCertificates          = default;
            _automaticDecompression      = default;
            _maxRequestContentBufferSize = default;

        #else
            _sslOptions = default;
            _responseDrainTimeout = default;
            _connectTimeout = default;
            _keepAlivePingPolicy = default;
            _keepAlivePingTimeout = default;
            _keepAlivePingDelay = default;
            _pooledConnectionLifetime = default;
            _pooledConnectionIdleTimeout = default;
            _maxResponseDrainSize = default;

        #endif

            return this;
        }


        private HttpClient GetClient()
        {
            var client = new HttpClient( GetHandler() );
            foreach ( (string? key, IEnumerable<string>? value) in _headers ) { client.DefaultRequestHeaders.Add( key, value ); }

            client.DefaultRequestHeaders.Authorization = _authenticationHeader;
            if ( _connectTimeout.HasValue ) { client.Timeout = _connectTimeout.Value; }

            if ( _maxResponseContentBufferSize.HasValue ) { client.MaxResponseContentBufferSize = _maxResponseContentBufferSize.Value; }

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

            if ( _clientCertificates is not null ) { handler.ClientCertificates.AddRange( _clientCertificates ); }

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
        public WebRequester Build() => new(GetClient(), _hostInfo, _retryPolicy, _encoding);


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


        public Builder With_Retry() => With_Retry( new RetryPolicy( true ) );
        public Builder With_Retry( RetryPolicy policy )
        {
            _retryPolicy = policy;
            return this;
        }
        public Builder With_Retry( int      maxRetires ) => With_Retry( new RetryPolicy( true,                                 maxRetires ) );
        public Builder With_Retry( TimeSpan delay, TimeSpan scale, int maxRetires = 3 ) => With_Retry( new RetryPolicy( delay, scale, maxRetires ) );


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
        public Builder With_Cookie( params Cookie[] value )
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


        public Builder With_Timeout( int    minutes ) => With_Timeout( TimeSpan.FromMinutes( minutes ) );
        public Builder With_Timeout( float  seconds ) => With_Timeout( TimeSpan.FromSeconds( seconds ) );
        public Builder With_Timeout( double milliseconds ) => With_Timeout( TimeSpan.FromMilliseconds( milliseconds ) );
        public Builder With_Timeout( TimeSpan value )
        {
            _connectTimeout = value;
            return this;
        }


    #if NET6_0_OR_GREATER
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
            _keepAlivePingDelay = pingDelay;
            _keepAlivePingTimeout = pingTimeout;
            _keepAlivePingPolicy = policy;
            return this;
        }


        public Builder With_SSL( SslClientAuthenticationOptions value )
        {
            _sslOptions = value;
            return this;
        }


        public Builder With_PooledConnectionIdleTimeout( int    pingDelayMinutes ) => With_PooledConnectionIdleTimeout( TimeSpan.FromMinutes( pingDelayMinutes ) );
        public Builder With_PooledConnectionIdleTimeout( float  pingDelaySeconds ) => With_PooledConnectionIdleTimeout( TimeSpan.FromSeconds( pingDelaySeconds ) );
        public Builder With_PooledConnectionIdleTimeout( double pingDelayMilliseconds ) => With_PooledConnectionIdleTimeout( TimeSpan.FromMilliseconds( pingDelayMilliseconds ) );
        public Builder With_PooledConnectionIdleTimeout( TimeSpan value )
        {
            _pooledConnectionIdleTimeout = value;
            return this;
        }


        public Builder With_PooledConnectionLifetime( int    pingDelayMinutes ) => With_PooledConnectionLifetime( TimeSpan.FromMinutes( pingDelayMinutes ) );
        public Builder With_PooledConnectionLifetime( float  pingDelaySeconds ) => With_PooledConnectionLifetime( TimeSpan.FromSeconds( pingDelaySeconds ) );
        public Builder With_PooledConnectionLifetime( double pingDelayMilliseconds ) => With_PooledConnectionLifetime( TimeSpan.FromMilliseconds( pingDelayMilliseconds ) );
        public Builder With_PooledConnectionLifetime( TimeSpan value )
        {
            _pooledConnectionLifetime = value;
            return this;
        }


        public Builder With_ResponseDrainTimeout( int    pingDelayMinutes ) => With_ResponseDrainTimeout( TimeSpan.FromMinutes( pingDelayMinutes ) );
        public Builder With_ResponseDrainTimeout( float  pingDelaySeconds ) => With_ResponseDrainTimeout( TimeSpan.FromSeconds( pingDelaySeconds ) );
        public Builder With_ResponseDrainTimeout( double pingDelayMilliseconds ) => With_ResponseDrainTimeout( TimeSpan.FromMilliseconds( pingDelayMilliseconds ) );
        public Builder With_ResponseDrainTimeout( TimeSpan value )
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
            _clientCertificates.Add( value );
            return this;
        }
        public Builder With_ClientCertificates( params X509Certificate[] value )
        {
            _clientCertificates ??= new X509CertificateCollection();
            _clientCertificates.AddRange( value );
            return this;
        }
        public Builder With_ClientCertificates( IEnumerable<X509Certificate> value )
        {
            _clientCertificates ??= new X509CertificateCollection();
            foreach ( X509Certificate certificate in value ) { _clientCertificates.Add( certificate ); }

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



        public sealed record HostHolder( Uri HostInfo ) : IHostInfo;



        public sealed record MethodHolder : IHostInfo
        {
            private readonly Func<IHostInfo> _value;

            public Uri HostInfo => _value()
               .HostInfo;

            public MethodHolder( Func<IHostInfo> value ) => _value = value;
        }
    }
}
