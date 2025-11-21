// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;



namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://www.stevejgordon.co.uk/httpclient-connection-pooling--dotnet-core"/>
///     </para>
/// </summary>
public partial class WebRequester
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class Builder( IHostInfo value ) : IHttpClientFactory
    {
        private readonly IHostInfo                       __hostInfo = value;
        private readonly WebHeaders                      __headers  = [];
        private          AuthenticationHeaderValue?      __authenticationHeader;
        private          bool?                           __allowAutoRedirect;
        private          bool?                           __preAuthenticate;
        private          bool?                           __useCookies;
        private          bool?                           __useProxy;
        private          CookieContainer?                __cookieContainer;
        private          Encoding                        __encoding = Encoding.Default;
        private          HttpKeepAlivePingPolicy?        __keepAlivePingPolicy;
        private          ICredentials?                   __credentials;
        private          ICredentials?                   __defaultProxyCredentials;
        private          ILogger?                        __logger;
        private          int?                            __maxAutomaticRedirections;
        private          int?                            __maxConnectionsPerServer;
        private          int?                            __maxResponseContentBufferSize;
        private          int?                            __maxResponseDrainSize;
        private          int?                            __maxResponseHeadersLength;
        private          IWebProxy?                      __proxy;
        private          RetryPolicy?                    __retryPolicy;
        private          SslClientAuthenticationOptions? __sslOptions;
        private          TimeSpan?                       __connectTimeout;
        private          TimeSpan?                       __keepAlivePingDelay;
        private          TimeSpan?                       __keepAlivePingTimeout;
        private          TimeSpan?                       __pooledConnectionIdleTimeout;
        private          TimeSpan?                       __pooledConnectionLifetime;
        private          TimeSpan?                       __responseDrainTimeout;


        public static Builder Create( IHostInfo       value ) => new(value);
        public static Builder Create( Uri             value ) => Create(new HostHolder(value));
        public static Builder Create( Func<IHostInfo> value ) => Create(new HostHolder(value));
        public static Builder Create( Func<Uri>       value ) => Create(new HostHolder(value));


        [Pure] public Builder Reset() => Create(__hostInfo);


        protected virtual HttpClient GetClient()
        {
            HttpClient client = new(GetHandler());
            foreach ( ( string key, IEnumerable<string> value ) in __headers ) { client.DefaultRequestHeaders.Add(key, value); }

            client.DefaultRequestHeaders.Authorization = __authenticationHeader;
            if ( __connectTimeout.HasValue ) { client.Timeout = __connectTimeout.Value; }

            if ( __maxResponseContentBufferSize.HasValue ) { client.MaxResponseContentBufferSize = __maxResponseContentBufferSize.Value; }

            return client;
        }


        protected virtual HttpMessageHandler GetHandler()
        {
            SocketsHttpHandler handler = new();

            if ( __connectTimeout.HasValue ) { handler.ConnectTimeout = __connectTimeout.Value; }

            if ( __keepAlivePingPolicy.HasValue ) { handler.KeepAlivePingPolicy = __keepAlivePingPolicy.Value; }

            if ( __keepAlivePingTimeout.HasValue ) { handler.KeepAlivePingTimeout = __keepAlivePingTimeout.Value; }

            if ( __keepAlivePingDelay.HasValue ) { handler.KeepAlivePingDelay = __keepAlivePingDelay.Value; }

            if ( __sslOptions is not null ) { handler.SslOptions = __sslOptions; }

            if ( __maxResponseDrainSize.HasValue ) { handler.MaxResponseDrainSize = __maxResponseDrainSize.Value; }

            if ( __responseDrainTimeout.HasValue ) { handler.ResponseDrainTimeout = __responseDrainTimeout.Value; }

            if ( __pooledConnectionLifetime.HasValue ) { handler.PooledConnectionLifetime = __pooledConnectionLifetime.Value; }

            if ( __pooledConnectionIdleTimeout.HasValue ) { handler.PooledConnectionIdleTimeout = __pooledConnectionIdleTimeout.Value; }

            if ( __maxResponseHeadersLength.HasValue ) { handler.MaxResponseHeadersLength = __maxResponseHeadersLength.Value; }

            if ( __maxConnectionsPerServer.HasValue ) { handler.MaxConnectionsPerServer = __maxConnectionsPerServer.Value; }

            if ( __maxAutomaticRedirections.HasValue ) { handler.MaxAutomaticRedirections = __maxAutomaticRedirections.Value; }

            if ( __allowAutoRedirect.HasValue ) { handler.AllowAutoRedirect = __allowAutoRedirect.Value; }

            if ( __useProxy.HasValue ) { handler.UseProxy = __useProxy.Value; }

            if ( __proxy is not null ) { handler.Proxy = __proxy; }

            if ( __defaultProxyCredentials is not null ) { handler.DefaultProxyCredentials = __defaultProxyCredentials; }

            if ( __credentials is not null ) { handler.Credentials = __credentials; }

            if ( __preAuthenticate.HasValue ) { handler.PreAuthenticate = __preAuthenticate.Value; }

            if ( __useCookies.HasValue ) { handler.UseCookies = __useCookies.Value; }

            if ( __cookieContainer is not null ) { handler.CookieContainer = __cookieContainer; }

            return handler;
        }
        public WebRequester Build()                     => new(GetClient(), __hostInfo, __logger, __encoding) { Retries = __retryPolicy };
        public HttpClient   CreateClient( string name ) => GetClient();


        public Builder With_Logger( ILogger logger )
        {
            __logger = logger;
            return this;
        }


        public Builder With_Header( string name, IEnumerable<string?> values )
        {
            __headers.Add(name, values);
            return this;
        }
        public Builder With_Header( string name, string? value )
        {
            __headers.Add(name, value);
            return this;
        }


        public Builder With_MaxResponseContentBufferSize( int value )
        {
            __maxResponseContentBufferSize = Math.Max(0, value);
            return this;
        }


        public Builder With_Retry()                                                        => With_Retry(RetryPolicy.Default);
        public Builder With_Retry( ushort   maxRetires )                                   => With_Retry(RetryPolicy.Create(maxRetires));
        public Builder With_Retry( TimeSpan delay, TimeSpan scale, ushort maxRetires = 3 ) => With_Retry(new RetryPolicy(delay, scale, maxRetires));
        public Builder With_Retry( RetryPolicy policy )
        {
            __retryPolicy = policy;
            return this;
        }


        public Builder With_Encoding( Encoding value )
        {
            __encoding = value;
            return this;
        }


        public Builder With_Proxy( IWebProxy value )
        {
            __proxy    = value;
            __useProxy = true;
            return this;
        }
        public Builder With_Proxy( IWebProxy value, ICredentials credentials )
        {
            __proxy                   = value;
            __useProxy                = true;
            __defaultProxyCredentials = credentials;
            return this;
        }


        public Builder With_MaxResponseHeadersLength( int value )
        {
            __maxResponseHeadersLength = value;
            return this;
        }
        public Builder With_MaxConnectionsPerServer( int value )
        {
            __maxConnectionsPerServer = value;
            return this;
        }
        public Builder With_MaxRedirects( int value )
        {
            __maxAutomaticRedirections = value;
            __allowAutoRedirect        = value > 0;
            return this;
        }


        public Builder With_SslOptions( SslClientAuthenticationOptions value )
        {
            __sslOptions = value;
            return this;
        }
        public Builder With_Ssl( RemoteCertificateValidationCallback value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.RemoteCertificateValidationCallback = value;
            return this;
        }
        public Builder With_Ssl( Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> value ) => With_Ssl(( object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors ) => value((HttpRequestMessage)sender, certificate as X509Certificate2, chain, sslPolicyErrors));
        public Builder With_Ssl( X509ChainPolicy value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.CertificateChainPolicy = value;
            return this;
        }
        public Builder With_Ssl( CipherSuitesPolicy value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.CipherSuitesPolicy = value;
            return this;
        }
        public Builder With_Ssl( SslStreamCertificateContext value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.ClientCertificateContext = value;
            return this;
        }
        public Builder With_Ssl( bool AllowRenegotiation, bool AllowTlsResume )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.AllowTlsResume     = AllowTlsResume;
            options.AllowRenegotiation = AllowRenegotiation;
            return this;
        }
        public Builder With_Ssl( X509CertificateCollection value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.ClientCertificates = value;
            return this;
        }
        public Builder With_Ssl( List<SslApplicationProtocol> value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.ApplicationProtocols = value;
            return this;
        }
        public Builder With_Ssl( params ReadOnlySpan<SslApplicationProtocol> value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.ApplicationProtocols = [..value];
            return this;
        }
        public Builder With_Ssl( Uri targetHost )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.TargetHost = targetHost.ToString();
            return this;
        }
        public Builder With_Ssl( SslProtocols value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.EnabledSslProtocols = value;
            return this;
        }
        public Builder With_Ssl( EncryptionPolicy value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.EncryptionPolicy = value;
            return this;
        }
        public Builder With_Ssl( X509RevocationMode value )
        {
            SslClientAuthenticationOptions options = __sslOptions ??= new SslClientAuthenticationOptions();
            options.CertificateRevocationCheckMode = value;
            return this;
        }


        public Builder With_Credentials( string scheme, string? value ) => With_Credentials(new AuthenticationHeaderValue(scheme, value));
        public Builder With_Credentials( AuthenticationHeaderValue? value )
        {
            __authenticationHeader = value;
            return this;
        }
        public Builder With_Credentials( ICredentials? value ) => With_Credentials(value, value is not null);
        public Builder With_Credentials( ICredentials? value, bool preAuthenticate )
        {
            __credentials     = value;
            __preAuthenticate = preAuthenticate;
            return this;
        }


        public Builder With_Cookie( Uri url, Cookie value )
        {
            __cookieContainer ??= new CookieContainer();
            __cookieContainer.Add(url, value);
            __useCookies = true;
            return this;
        }
        public Builder With_Cookie( params ReadOnlySpan<Cookie> value )
        {
            CookieContainer container = __cookieContainer ??= new CookieContainer();
            foreach ( Cookie cookie in value ) { container.Add(cookie); }

            __useCookies = true;
            return this;
        }
        public Builder With_Cookie( CookieContainer value )
        {
            __cookieContainer = value;
            __useCookies      = true;
            return this;
        }


        public Builder With_Timeout( int    minutes )      => With_Timeout(TimeSpan.FromMinutes(minutes));
        public Builder With_Timeout( float  seconds )      => With_Timeout(TimeSpan.FromSeconds(seconds));
        public Builder With_Timeout( double milliseconds ) => With_Timeout(TimeSpan.FromMilliseconds(milliseconds));
        public Builder With_Timeout( TimeSpan value )
        {
            __connectTimeout = value;
            return this;
        }


        public Builder With_MaxResponseDrainSize( int value )
        {
            __maxResponseDrainSize = value;
            return this;
        }


        public Builder With_KeepAlive( int pingDelayMinutes, int pingTimeoutMinutes, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive(TimeSpan.FromMinutes(pingDelayMinutes), TimeSpan.FromMinutes(pingTimeoutMinutes), policy);
        public Builder With_KeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive(TimeSpan.FromSeconds(pingDelaySeconds), TimeSpan.FromSeconds(pingTimeoutSeconds), policy);
        public Builder With_KeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
            With_KeepAlive(TimeSpan.FromMilliseconds(pingDelayMilliseconds), TimeSpan.FromMilliseconds(pingTimeoutMilliseconds), policy);
        public Builder With_KeepAlive( TimeSpan pingDelay, TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
        {
            __keepAlivePingDelay   = pingDelay;
            __keepAlivePingTimeout = pingTimeout;
            __keepAlivePingPolicy  = policy;
            return this;
        }


        public Builder With_SSL( SslClientAuthenticationOptions value )
        {
            __sslOptions = value;
            return this;
        }


        public Builder With_PooledConnectionIdleTimeout( int    minutes )      => With_PooledConnectionIdleTimeout(TimeSpan.FromMinutes(minutes));
        public Builder With_PooledConnectionIdleTimeout( float  seconds )      => With_PooledConnectionIdleTimeout(TimeSpan.FromSeconds(seconds));
        public Builder With_PooledConnectionIdleTimeout( double milliseconds ) => With_PooledConnectionIdleTimeout(TimeSpan.FromMilliseconds(milliseconds));
        public Builder With_PooledConnectionIdleTimeout( TimeSpan value )
        {
            __pooledConnectionIdleTimeout = value;
            return this;
        }


        public Builder With_PooledConnectionLifetime( int    minutes )      => With_PooledConnectionLifetime(TimeSpan.FromMinutes(minutes));
        public Builder With_PooledConnectionLifetime( float  seconds )      => With_PooledConnectionLifetime(TimeSpan.FromSeconds(seconds));
        public Builder With_PooledConnectionLifetime( double milliseconds ) => With_PooledConnectionLifetime(TimeSpan.FromMilliseconds(milliseconds));
        public Builder With_PooledConnectionLifetime( TimeSpan value )
        {
            __pooledConnectionLifetime = value;
            return this;
        }


        public Builder With_ResponseDrainTimeout( int    minutes )      => With_ResponseDrainTimeout(TimeSpan.FromMinutes(minutes));
        public Builder With_ResponseDrainTimeout( float  seconds )      => With_ResponseDrainTimeout(TimeSpan.FromSeconds(seconds));
        public Builder With_ResponseDrainTimeout( double milliseconds ) => With_ResponseDrainTimeout(TimeSpan.FromMilliseconds(milliseconds));
        public Builder With_ResponseDrainTimeout( TimeSpan value )
        {
            __responseDrainTimeout = value;
            return this;
        }



        public sealed class HostHolder( OneOf<Uri, Func<Uri>, Func<IHostInfo>> hostInfo ) : IHostInfo
        {
            private readonly OneOf<Uri, Func<Uri>, Func<IHostInfo>> __hostInfo = hostInfo;

            public Uri HostInfo => __hostInfo.Match(static x => x,
                                                    static x => x(),
                                                    static x => x()
                                                       .HostInfo);
        }
    }
}
