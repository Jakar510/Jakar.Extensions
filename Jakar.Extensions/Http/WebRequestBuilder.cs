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
public class WebRequestBuilder
{
    protected Encoding                   _encoding = Encoding.Default;
    protected IHostInfo?                 _hostInfo;
    protected Uri?                       _host;
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


    protected internal Uri                                   Host       => _hostInfo?.HostInfo ?? _host ?? throw new ArgumentNullException(nameof(Host));
    public             HttpHeaders?                          Headers    { get; set; }
    public             ConcurrentDictionary<string, object?> Properties { get; } = new();


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
        handler.Properties.Add(Properties);


        if ( _sslProtocols.HasValue ) { handler.SslProtocols = _sslProtocols.Value; }

        if ( _maxRequestContentBufferSize.HasValue ) { handler.MaxRequestContentBufferSize = _maxRequestContentBufferSize.Value; }

        if ( _automaticDecompression.HasValue ) { handler.AutomaticDecompression = _automaticDecompression.Value; }

        if ( _clientCertificateOptions.HasValue ) { handler.ClientCertificateOptions = _clientCertificateOptions.Value; }

        if ( _clientCertificates is not null ) { handler.ClientCertificates.AddRange(_clientCertificates); }

    #else
        var handler = new SocketsHttpHandler();
        handler.Properties.Add(Properties);


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


    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Handler CreateHandler( Uri url, HttpMethod method, in CancellationToken token ) => CreateHandler(new HttpRequestMessage(method, url), token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual Handler CreateHandler( Uri url, HttpMethod method, HttpContent value, in CancellationToken token ) => CreateHandler(new HttpRequestMessage(method, url)
                                                                                                                                          {
                                                                                                                                              Content = value
                                                                                                                                          },
                                                                                                                                          token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Handler CreateHandler( HttpRequestMessage request, in CancellationToken token ) => new(this, request, token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Uri CreateUrl( string                     path ) => new(Host, path);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Uri CreateUrl( Uri                        path ) => new(Host, path);


    public Handler Post( Uri    url,  HttpContent                 value,   in CancellationToken token ) => CreateHandler(url, HttpMethod.Post, value, token);
    public Handler Post( string path, HttpContent                 value,   in CancellationToken token ) => Post(CreateUrl(path), value,                                              token);
    public Handler Post( string path, byte[]                      value,   in CancellationToken token ) => Post(path,            new ByteArrayContent(value),                        token);
    public Handler Post( string path, ReadOnlyMemory<byte>        value,   in CancellationToken token ) => Post(path,            new ReadOnlyMemoryContent(value),                   token);
    public Handler Post( string path, IDictionary<string, string> value,   in CancellationToken token ) => Post(path,            new FormUrlEncodedContent(value),                   token);
    public Handler Post( string path, Stream                      value,   in CancellationToken token ) => Post(path,            new StreamContent(value),                           token);
    public Handler Post( string path, MultipartContent            content, in CancellationToken token ) => Post(path,            (HttpContent)content,                               token);
    public Handler Post( string path, string                      value,   in CancellationToken token ) => Post(path,            new StringContent(value.ToPrettyJson(), _encoding), token);
    public Handler Post( string path, BaseClass                   value,   in CancellationToken token ) => Post(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Post( string path, IEnumerable<BaseClass>      value,   in CancellationToken token ) => Post(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Post( string path, BaseRecord                  value,   in CancellationToken token ) => Post(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Post( string path, IEnumerable<BaseRecord>     value,   in CancellationToken token ) => Post(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);


    public Handler Put( Uri    url,  HttpContent                 value,   in CancellationToken token ) => CreateHandler(url, HttpMethod.Put, value, token);
    public Handler Put( string path, HttpContent                 value,   in CancellationToken token ) => Put(CreateUrl(path), value,                                              token);
    public Handler Put( string path, byte[]                      value,   in CancellationToken token ) => Put(path,            new ByteArrayContent(value),                        token);
    public Handler Put( string path, ReadOnlyMemory<byte>        value,   in CancellationToken token ) => Put(path,            new ReadOnlyMemoryContent(value),                   token);
    public Handler Put( string path, IDictionary<string, string> value,   in CancellationToken token ) => Put(path,            new FormUrlEncodedContent(value),                   token);
    public Handler Put( string path, Stream                      value,   in CancellationToken token ) => Put(path,            new StreamContent(value),                           token);
    public Handler Put( string path, MultipartContent            content, in CancellationToken token ) => Put(path,            (HttpContent)content,                               token);
    public Handler Put( string path, string                      value,   in CancellationToken token ) => Put(path,            new StringContent(value.ToPrettyJson(), _encoding), token);
    public Handler Put( string path, BaseClass                   value,   in CancellationToken token ) => Put(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Put( string path, IEnumerable<BaseClass>      value,   in CancellationToken token ) => Put(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Put( string path, BaseRecord                  value,   in CancellationToken token ) => Put(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Put( string path, IEnumerable<BaseRecord>     value,   in CancellationToken token ) => Put(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);


    public Handler Delete( Uri    url,  in CancellationToken        token ) => CreateHandler(url, HttpMethod.Delete, token);
    public Handler Delete( string path, in CancellationToken        token ) => Delete(CreateUrl(path),                               token);
    public Handler Delete( string path, HttpContent                 value,   in CancellationToken token ) => Delete(CreateUrl(path), value,                                              token);
    public Handler Delete( string path, byte[]                      value,   in CancellationToken token ) => Delete(path,            new ByteArrayContent(value),                        token);
    public Handler Delete( string path, ReadOnlyMemory<byte>        value,   in CancellationToken token ) => Delete(path,            new ReadOnlyMemoryContent(value),                   token);
    public Handler Delete( string path, IDictionary<string, string> value,   in CancellationToken token ) => Delete(path,            new FormUrlEncodedContent(value),                   token);
    public Handler Delete( string path, Stream                      value,   in CancellationToken token ) => Delete(path,            new StreamContent(value),                           token);
    public Handler Delete( string path, MultipartContent            content, in CancellationToken token ) => Delete(path,            (HttpContent)content,                               token);
    public Handler Delete( string path, string                      value,   in CancellationToken token ) => Delete(path,            new StringContent(value.ToPrettyJson(), _encoding), token);
    public Handler Delete( string path, BaseClass                   value,   in CancellationToken token ) => Delete(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Delete( string path, IEnumerable<BaseClass>      value,   in CancellationToken token ) => Delete(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Delete( string path, BaseRecord                  value,   in CancellationToken token ) => Delete(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Delete( string path, IEnumerable<BaseRecord>     value,   in CancellationToken token ) => Delete(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Delete( Uri    url,  HttpContent                 value,   in CancellationToken token ) => CreateHandler(url, HttpMethod.Delete, value, token);


    public Handler Patch( Uri    url,  HttpContent                 value,   in CancellationToken token ) => CreateHandler(url, HttpMethod.Patch, value, token);
    public Handler Patch( string path, HttpContent                 value,   in CancellationToken token ) => Patch(CreateUrl(path), value,                                              token);
    public Handler Patch( string path, byte[]                      value,   in CancellationToken token ) => Patch(path,            new ByteArrayContent(value),                        token);
    public Handler Patch( string path, ReadOnlyMemory<byte>        value,   in CancellationToken token ) => Patch(path,            new ReadOnlyMemoryContent(value),                   token);
    public Handler Patch( string path, IDictionary<string, string> value,   in CancellationToken token ) => Patch(path,            new FormUrlEncodedContent(value),                   token);
    public Handler Patch( string path, Stream                      value,   in CancellationToken token ) => Patch(path,            new StreamContent(value),                           token);
    public Handler Patch( string path, MultipartContent            content, in CancellationToken token ) => Patch(path,            (HttpContent)content,                               token);
    public Handler Patch( string path, string                      value,   in CancellationToken token ) => Patch(path,            new StringContent(value.ToPrettyJson(), _encoding), token);
    public Handler Patch( string path, BaseClass                   value,   in CancellationToken token ) => Patch(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Patch( string path, IEnumerable<BaseClass>      value,   in CancellationToken token ) => Patch(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Patch( string path, BaseRecord                  value,   in CancellationToken token ) => Patch(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Patch( string path, IEnumerable<BaseRecord>     value,   in CancellationToken token ) => Patch(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);


    public Handler Get( Uri    url,  CancellationToken token ) => CreateHandler(url, HttpMethod.Get, token);
    public Handler Get( string path, CancellationToken token ) => Get(CreateUrl(path), token);



    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public readonly struct Handler : IDisposable
    {
        private readonly  HttpClient         _client;
        private readonly  HttpRequestMessage _request;
        internal readonly Encoding           encoding;
        internal readonly CancellationToken  token;


        public HttpRequestHeaders  DefaultRequestHeaders => _client.DefaultRequestHeaders;
        public string              Method                => _request.Method.Method;
        public HttpRequestHeaders  Headers               => _request.Headers;
        public HttpContentHeaders? ContentHeaders        => _request.Content?.Headers;
        public AppVersion Version
        {
            get => _request.Version;
            set => _request.Version = value.ToVersion();
        }
    #if NET6_0
        public HttpVersionPolicy VersionPolicy
        {
            get => _request.VersionPolicy;
            set => _request.VersionPolicy = value;
        }
        public HttpRequestOptions Options => _request.Options;
    #endif


        public Handler( WebRequestBuilder builder, HttpRequestMessage request, in CancellationToken token )
        {
            _request   = request;
            _client    = builder.GetClient();
            encoding   = builder._encoding;
            this.token = token;

            if ( builder.Headers is null ) { return; }

            foreach ( ( string? key, IEnumerable<string>? value ) in builder.Headers ) { Headers.Add(key, value); }
        }


        public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _client.SendAsync(_request, token).GetAwaiter();


        public Task<Response<JToken>> AsJson() => AsJson(JsonNet.LoadSettings);
        public async Task<Response<JToken>> AsJson( JsonLoadSettings settings ) => await Response<JToken>.Create(this, settings, AsJson);
        public async Task<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
        #if NET6_0
            await using Stream stream = await content.ReadAsStreamAsync(token);
        #else
            await using Stream stream = await content.ReadAsStreamAsync();
        #endif
            using var        sr     = new StreamReader(stream, encoding);
            using JsonReader reader = new JsonTextReader(sr);
            return await JToken.ReadFromAsync(reader, settings, token);
        }


        public Task<Response<TResult>> AsJson<TResult>() => AsJson<TResult>(JsonNet.Serializer);
        public Task<Response<TResult>> AsJson<TResult>( JsonSerializer serializer ) => Response<TResult>.Create(this, serializer, AsJson<TResult>);
        public async Task<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
        #if NET6_0
            await using Stream stream = await content.ReadAsStreamAsync(token);
        #else
            await using Stream stream = await content.ReadAsStreamAsync();
        #endif
            using var        sr     = new StreamReader(stream, encoding);
            using JsonReader reader = new JsonTextReader(sr);

            return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
        }


        public Task<Response<string>> AsString() => Response<string>.Create(this, AsString);
        public async Task<string> AsString( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
        #if NET6_0
            return await content.ReadAsStringAsync(token);
        #else
            return await content.ReadAsStringAsync();
        #endif
        }


        public Task<Response<byte[]>> AsBytes() => Response<byte[]>.Create(this, AsBytes);
        public async Task<byte[]> AsBytes( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
        #if NET6_0
            return await content.ReadAsByteArrayAsync(token);
        #else
            return await content.ReadAsByteArrayAsync();
        #endif
        }


        public Task<Response<ReadOnlyMemory<byte>>> AsMemory() => Response<ReadOnlyMemory<byte>>.Create(this, AsMemory);
        public async Task<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;

        #if NET6_0
            byte[] bytes = await content.ReadAsByteArrayAsync(token);
        #else
            byte[] bytes = await content.ReadAsByteArrayAsync();
        #endif

            return bytes;
        }


        public Task<Response<LocalFile>> AsFile() => Response<LocalFile>.Create(this, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent      content = response.Content;
            await using FileStream stream  = LocalFile.CreateTempFileAndOpen(out LocalFile file);
            await stream.CopyToAsync(stream, token);

            return file;
        }


        public Task<Response<LocalFile>> AsFile( string path ) => Response<LocalFile>.Create(this, path, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response, string path )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            var               file    = new LocalFile(path);
        #if NET6_0
            await using Stream stream = await content.ReadAsStreamAsync(token);
        #else
            await using Stream stream = await content.ReadAsStreamAsync();
        #endif
            await file.WriteAsync(stream, token);

            return file;
        }


        public Task<Response<LocalFile>> AsFile( LocalFile file ) => Response<LocalFile>.Create(this, file, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response, LocalFile file )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
        #if NET6_0
            await using Stream stream = await content.ReadAsStreamAsync(token);
        #else
            await using Stream stream = await content.ReadAsStreamAsync();
        #endif
            await file.WriteAsync(stream, token);

            return file;
        }


        public Task<Response<LocalFile>> AsFile( MimeType type ) => Response<LocalFile>.Create(this, type, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response, MimeType type )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent      content = response.Content;
            await using FileStream stream  = LocalFile.CreateTempFileAndOpen(type, out LocalFile file);
            await stream.CopyToAsync(stream, token);

            return file;
        }


        public Task<Response<MemoryStream>> AsStream() => Response<MemoryStream>.Create(this, AsStream);
        public async Task<MemoryStream> AsStream( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
        #if NET6_0
            await using Stream stream = await content.ReadAsStreamAsync(token);
        #else
            await using Stream stream = await content.ReadAsStreamAsync();
        #endif
            var result = new MemoryStream();

            await stream.CopyToAsync(result, token);
            return result;
        }


        public void Dispose()
        {
            _request.Dispose();
            _request.Dispose();
        }
    }



    public readonly struct Response<T>
    {
        public const string UNKNOWN_ERROR = "Unknown Error";


        public              T?         Payload           { get; init; } = default;
        public              string?    Method            { get; init; } = default;
        public              Uri?       URL               { get; init; } = default;
        public              JToken?    ErrorMessage      { get; init; } = default;
        public              Status     StatusCode        { get; init; } = Status.NotSet;
        public              string?    StatusDescription { get; init; } = default;
        public              string?    ContentEncoding   { get; init; } = default;
        public              string?    Server            { get; init; } = default;
        public              string?    ContentType       { get; init; } = default;
        [JsonIgnore] public Exception? Exception         { get; init; } = default;


        public Response( HttpResponseMessage response, in string    error ) : this(response, default, default, error) { }
        public Response( HttpResponseMessage response, in Exception e, in string error ) : this(response, default, e, error) { }
        public Response( HttpResponseMessage response, in T         payload ) : this(response, payload, default) { }
        public Response( HttpResponseMessage response, in T? payload, Exception? exception, in string? error = default )
        {
            HttpRequestHeaders? requestHeaders = response.RequestMessage?.Headers;
            HttpContentHeaders? contentHeaders = response.RequestMessage?.Content?.Headers;


            ErrorMessage      = ResponseData.ParseError(error ?? exception?.Message);
            Payload           = payload;
            Exception         = exception;
            StatusCode        = response.StatusCode.ToStatus();
            StatusDescription = response.ReasonPhrase ?? StatusCode.ToStringFast();
            URL               = response.RequestMessage?.RequestUri;
            Method            = response.RequestMessage?.Method.Method;
            ContentType       = contentHeaders?.ContentType?.MediaType;
            ContentEncoding   = contentHeaders?.ContentEncoding.ToJson();
            Server            = requestHeaders?.From;
        }

        public override string ToString() => this.ToJson(Formatting.Indented);


        internal static Response<T> None( HttpResponseMessage response ) => new(response, "NO RESPONSE");
        internal static Response<T> None( HttpResponseMessage response, Exception e ) => new(response, e, "NO RESPONSE");


        public static async Task<Response<T>> Create<TArg>( Handler handler, TArg arg, Func<HttpResponseMessage, TArg, Task<T>> func )
        {
            using ( handler )
            {
                using HttpResponseMessage response = await handler;

                try
                {
                    if ( !response.IsSuccessStatusCode ) { return await Create(handler, response); }

                    T result = await func(response, arg);
                    return new Response<T>(response, result);
                }
                catch ( HttpRequestException e ) { return await Create(handler, response, e); }
            }
        }
        public static async Task<Response<T>> Create( Handler handler, Func<HttpResponseMessage, Task<T>> func )
        {
            using ( handler )
            {
                using HttpResponseMessage response = await handler;

                try
                {
                    if ( !response.IsSuccessStatusCode ) { return await Create(handler, response); }

                    T result = await func(response);
                    return new Response<T>(response, result);
                }
                catch ( HttpRequestException e ) { return await Create(handler, response, e); }
            }
        }
        private static async Task<Response<T>> Create( Handler handler, HttpResponseMessage response )
        {
        #if NET6_0
            await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.token);
        #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
        #endif

            string error;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if ( stream is null ) { error = UNKNOWN_ERROR; }
            else
            {
                using var reader       = new StreamReader(stream);
                string    errorMessage = await reader.ReadToEndAsync();

                if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response); }

                error = $"Error Message: '{errorMessage}'";
            }

            return new Response<T>(response, error);
        }
        private static async Task<Response<T>> Create( Handler handler, HttpResponseMessage response, Exception e )
        {
        #if NET6_0
            await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.token);
        #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
        #endif

            string error;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if ( stream is null ) { error = UNKNOWN_ERROR; }
            else
            {
                using var reader       = new StreamReader(stream);
                string    errorMessage = await reader.ReadToEndAsync();

                if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response, e); }

                error = $"Error Message: '{errorMessage}'";
            }

            return new Response<T>(response, e, error);
        }
    }
}
