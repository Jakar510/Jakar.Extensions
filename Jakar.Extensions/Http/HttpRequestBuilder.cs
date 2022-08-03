// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


#nullable enable
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;



namespace Jakar.Extensions;


#if NET6_0



public class HttpRequestBuilder
{
    private readonly IHostInfo?         _hostInfo;
    private readonly Uri?               _host;
    private readonly Encoding           _encoding;
    private readonly SocketsHttpHandler _handler = new();
    private          HttpClient         _Client => new(_handler);
    private          Uri                _Host   => _hostInfo?.HostInfo ?? _host ?? throw new ArgumentNullException(nameof(_Host));


    public HttpRequestBuilder( Encoding  encoding ) => _encoding = encoding;
    public HttpRequestBuilder( IHostInfo host ) : this(host, Encoding.Default) { }
    public HttpRequestBuilder( IHostInfo host, Encoding encoding ) : this(encoding) => _hostInfo = host;
    public HttpRequestBuilder( Uri       host ) : this(host, Encoding.Default) { }
    public HttpRequestBuilder( Uri       host, Encoding encoding ) : this(encoding) => _host = host;


    public static HttpRequestBuilder Create() => new(Encoding.Default);
    public static HttpRequestBuilder Create( Uri       host ) => new(host);
    public static HttpRequestBuilder Create( Uri       host, Encoding encoding ) => new(host, encoding);
    public static HttpRequestBuilder Create( IHostInfo host ) => new(host);
    public static HttpRequestBuilder Create( IHostInfo host, Encoding encoding ) => new(host, encoding);


    public virtual HttpRequestBuilder With_Proxy( IWebProxy value )
    {
        _handler.Proxy    = value;
        _handler.UseProxy = true;
        return this;
    }
    public virtual HttpRequestBuilder With_Proxy( IWebProxy value, ICredentials credentials )
    {
        _handler.Proxy                   = value;
        _handler.UseProxy                = true;
        _handler.DefaultProxyCredentials = credentials;
        return this;
    }


    public virtual HttpRequestBuilder With_MaxRedirects( int value )
    {
        _handler.MaxAutomaticRedirections = value;
        _handler.AllowAutoRedirect        = value > 0;
        return this;
    }


    public virtual HttpRequestBuilder With_Credentials( ICredentials value, bool preAuthenticate = true )
    {
        _handler.Credentials     = value;
        _handler.PreAuthenticate = preAuthenticate;
        return this;
    }


    public virtual HttpRequestBuilder With_Cookie( Uri url, Cookie value )
    {
        _handler.CookieContainer.Add(url, value);
        _handler.UseCookies = true;
        return this;
    }
    public virtual HttpRequestBuilder With_Cookie( params Cookie[] value )
    {
        CookieContainer container = _handler.CookieContainer;
        foreach ( Cookie cookie in value ) { container.Add(cookie); }

        _handler.UseCookies = true;
        return this;
    }
    public virtual HttpRequestBuilder With_Cookie( CookieContainer value )
    {
        _handler.CookieContainer = value;
        _handler.UseCookies      = true;
        return this;
    }


    public virtual HttpRequestBuilder With_Timeout( int    minutes ) => With_Timeout(TimeSpan.FromMinutes(minutes));
    public virtual HttpRequestBuilder With_Timeout( float  seconds ) => With_Timeout(TimeSpan.FromSeconds(seconds));
    public virtual HttpRequestBuilder With_Timeout( double milliseconds ) => With_Timeout(TimeSpan.FromMilliseconds(milliseconds));
    public virtual HttpRequestBuilder With_Timeout( in TimeSpan value )
    {
        _handler.ConnectTimeout = value;
        return this;
    }


    public virtual HttpRequestBuilder With_SSL( SslClientAuthenticationOptions value )
    {
        _handler.SslOptions = value;
        return this;
    }


    public virtual HttpRequestBuilder With_KeepAlive( int pingDelayMinutes, int pingTimeoutMinutes, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromMinutes(pingDelayMinutes), TimeSpan.FromMinutes(pingTimeoutMinutes), policy);
    public virtual HttpRequestBuilder With_KeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromSeconds(pingDelaySeconds), TimeSpan.FromSeconds(pingTimeoutSeconds), policy);
    public virtual HttpRequestBuilder With_KeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromMilliseconds(pingDelayMilliseconds), TimeSpan.FromMilliseconds(pingTimeoutMilliseconds), policy);
    public virtual HttpRequestBuilder With_KeepAlive( in TimeSpan pingDelay, in TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
    {
        _handler.KeepAlivePingDelay   = pingDelay;
        _handler.KeepAlivePingTimeout = pingTimeout;
        _handler.KeepAlivePingPolicy  = policy;
        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Handler CreateHandler( Uri url, HttpMethod method, in CancellationToken token ) => CreateHandler(new HttpRequestMessage(method, url), token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual Handler CreateHandler( Uri url, HttpMethod method, HttpContent value, in CancellationToken token ) => CreateHandler(new HttpRequestMessage(method, url)
                                                                                                                                          {
                                                                                                                                              Content = value
                                                                                                                                          },
                                                                                                                                          token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Handler CreateHandler( HttpRequestMessage request, in CancellationToken token ) => new(_Client, request, _encoding, token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Uri CreateUrl( string                     path ) => new(_Host, path);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] protected virtual Uri CreateUrl( Uri                        path ) => new(_Host, path);


    public Handler Post( Uri    url,  HttpContent                 value,   in CancellationToken token ) => CreateHandler(url, HttpMethod.Post, value, token);
    public Handler Post( string path, HttpContent                 value,   in CancellationToken token ) => Post(CreateUrl(path), value,                                              token);
    public Handler Post( string path, byte[]                      value,   in CancellationToken token ) => Post(path,            new ByteArrayContent(value),                        token);
    public Handler Post( string path, ReadOnlyMemory<byte>        value,   in CancellationToken token ) => Post(path,            new ReadOnlyMemoryContent(value),                   token);
    public Handler Post( string path, IDictionary<string, string> value,   in CancellationToken token ) => Post(path,            new FormUrlEncodedContent(value),                   token);
    public Handler Post( string path, Stream                      value,   in CancellationToken token ) => Post(path,            new StreamContent(value),                           token);
    public Handler Post( string path, MultipartContent            content, in CancellationToken token ) => Post(path,            (HttpContent)content,                               token);
    public Handler Post( string path, string                      value,   in CancellationToken token ) => Post(path,            new StringContent(value.ToPrettyJson(), _encoding), token);
    public Handler Post( string path, BaseClass                   value,   in CancellationToken token ) => Post(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Post( string path, BaseRecord                  value,   in CancellationToken token ) => Post(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);


    public Handler Put( Uri    url,  HttpContent                 value,   in CancellationToken token ) => CreateHandler(url, HttpMethod.Put, value, token);
    public Handler Put( string path, HttpContent                 value,   in CancellationToken token ) => Put(CreateUrl(path), value,                                              token);
    public Handler Put( string path, byte[]                      value,   in CancellationToken token ) => Put(path,            new ByteArrayContent(value),                        token);
    public Handler Put( string path, ReadOnlyMemory<byte>        value,   in CancellationToken token ) => Put(path,            new ReadOnlyMemoryContent(value),                   token);
    public Handler Put( string path, IDictionary<string, string> value,   in CancellationToken token ) => Put(path,            new FormUrlEncodedContent(value),                   token);
    public Handler Put( string path, Stream                      value,   in CancellationToken token ) => Put(path,            new StreamContent(value),                           token);
    public Handler Put( string path, MultipartContent            content, in CancellationToken token ) => Put(path,            (HttpContent)content,                               token);
    public Handler Put( string path, string                      value,   in CancellationToken token ) => Put(path,            new StringContent(value.ToPrettyJson(), _encoding), token);
    public Handler Put( string path, BaseClass                   value,   in CancellationToken token ) => Put(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
    public Handler Put( string path, BaseRecord                  value,   in CancellationToken token ) => Put(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);


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
    public Handler Delete( string path, BaseRecord                  value,   in CancellationToken token ) => Delete(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);
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
    public Handler Patch( string path, BaseRecord                  value,   in CancellationToken token ) => Patch(path,            new JsonContent(value.ToPrettyJson(), _encoding),   token);


    public Handler Get( Uri    url,  CancellationToken token ) => CreateHandler(url, HttpMethod.Get, token);
    public Handler Get( string path, CancellationToken token ) => Get(CreateUrl(path), token);



    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public readonly struct Handler : IDisposable
    {
        private readonly  HttpClient         _client;
        private readonly  HttpRequestMessage _request;
        internal readonly Encoding           encoding;
        internal readonly CancellationToken  token;


        public string              Method         => _request.Method.Method;
        public HttpRequestOptions  Options        => _request.Options;
        public HttpRequestHeaders  Headers        => _request.Headers;
        public HttpContentHeaders? ContentHeaders => _request.Content?.Headers;
        public AppVersion Version
        {
            get => _request.Version;
            set => _request.Version = value.ToVersion();
        }
        public HttpVersionPolicy VersionPolicy
        {
            get => _request.VersionPolicy;
            set => _request.VersionPolicy = value;
        }


        public Handler( HttpClient client, HttpRequestMessage request, Encoding encoding, in CancellationToken token )
        {
            _client       = client;
            _request      = request;
            this.encoding = encoding;
            this.token    = token;
        }


        public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _client.SendAsync(_request, token).GetAwaiter();


        public Task<ResponseData<JToken>> AsJson() => AsJson(JsonNet.LoadSettings);
        public async Task<ResponseData<JToken>> AsJson( JsonLoadSettings settings ) => await ResponseData<JToken>.Create(this, settings, AsJson);
        public async Task<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            await using Stream s       = await content.ReadAsStreamAsync(token);
            using var          sr      = new StreamReader(s, encoding);
            using JsonReader   reader  = new JsonTextReader(sr);
            return await JToken.ReadFromAsync(reader, settings, token);
        }


        public Task<ResponseData<TResult>> AsJson<TResult>() => AsJson<TResult>(JsonNet.Serializer);
        public Task<ResponseData<TResult>> AsJson<TResult>( JsonSerializer serializer ) => ResponseData<TResult>.Create(this, serializer, AsJson<TResult>);
        public async Task<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            await using Stream s       = await content.ReadAsStreamAsync(token);
            using var          sr      = new StreamReader(s, encoding);
            using JsonReader   reader  = new JsonTextReader(sr);

            return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
        }


        public Task<ResponseData<string>> AsString() => ResponseData<string>.Create(this, AsString);
        public async Task<string> AsString( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            return await content.ReadAsStringAsync(token);
        }


        public Task<ResponseData<byte[]>> AsBytes() => ResponseData<byte[]>.Create(this, AsBytes);
        public async Task<byte[]> AsBytes( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            return await content.ReadAsByteArrayAsync(token);
        }


        public Task<ResponseData<ReadOnlyMemory<byte>>> AsMemory() => ResponseData<ReadOnlyMemory<byte>>.Create(this, AsMemory);
        public async Task<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            byte[]            bytes   = await content.ReadAsByteArrayAsync(token);

            return bytes;
        }


        public Task<ResponseData<LocalFile>> AsFile() => ResponseData<LocalFile>.Create(this, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent      content = response.Content;
            await using FileStream stream  = LocalFile.CreateTempFileAndOpen(out LocalFile file);
            await using Stream     sr      = await content.ReadAsStreamAsync(token);
            await sr.CopyToAsync(stream, token);

            return file;
        }
        public Task<ResponseData<LocalFile>> AsFile( string path ) => ResponseData<LocalFile>.Create(this, path, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response, string path )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            var                file    = new LocalFile(path);
            await using Stream stream  = await content.ReadAsStreamAsync(token);
            await file.WriteAsync(stream, token);

            return file;
        }
        public Task<ResponseData<LocalFile>> AsFile( LocalFile file ) => ResponseData<LocalFile>.Create(this, file, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response, LocalFile file )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            await using Stream stream  = await content.ReadAsStreamAsync(token);
            await file.WriteAsync(stream, token);

            return file;
        }
        public Task<ResponseData<LocalFile>> AsFile( MimeType type ) => ResponseData<LocalFile>.Create(this, type, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response, MimeType type )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent      content = response.Content;
            await using FileStream stream  = LocalFile.CreateTempFileAndOpen(type, out LocalFile file);
            await using Stream     sr      = await content.ReadAsStreamAsync(token);
            await sr.CopyToAsync(stream, token);

            return file;
        }


        public Task<ResponseData<MemoryStream>> AsStream() => ResponseData<MemoryStream>.Create(this, AsStream);
        public async Task<MemoryStream> AsStream( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            await using Stream stream  = await content.ReadAsStreamAsync(token);
            var                result  = new MemoryStream();

            await stream.CopyToAsync(result, token);
            return result;
        }


        public void Dispose()
        {
            _request.Dispose();
            _request.Dispose();
        }
    }
}



#endif
