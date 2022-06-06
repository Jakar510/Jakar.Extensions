// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM


#nullable enable
using System.Net.Http.Headers;



namespace Jakar.Extensions.Http;


#if NET6_0
using System.Net.Http;
using System.Net.Security;
using Enumerations;



public readonly struct HttpRequestBuilder
{
    private readonly Encoding           _encoding;
    private readonly Uri                _url;
    private readonly CancellationToken  _token;
    private readonly SocketsHttpHandler _handler = new();
    public readonly  HeaderCollection   headers  = new();
    private          HttpClient         _Client => new(_handler);


    public HttpRequestBuilder( Uri url, Encoding encoding, in CancellationToken token )
    {
        _url      = url;
        _encoding = encoding;
        _token    = token;
    }
    public static HttpRequestBuilder Create( Uri url, in CancellationToken token                                = default ) => new(url, Encoding.Default, token);
    public static HttpRequestBuilder Create( Uri url, Encoding             encoding, in CancellationToken token = default ) => new(url, encoding, token);


    public HttpRequestBuilder With_Headers( HttpContentHeaders collection )
    {
        headers.Merge(collection);
        return this;
    }
    public HttpRequestBuilder With_Headers( HttpHeaders collection )
    {
        headers.Merge(collection);
        return this;
    }
    public HttpRequestBuilder With_Headers( HeaderCollection collection )
    {
        headers.Merge(collection);
        return this;
    }


    public HttpRequestBuilder With_Proxy( IWebProxy value )
    {
        _handler.Proxy    = value;
        _handler.UseProxy = true;
        return this;
    }
    public HttpRequestBuilder With_Proxy( IWebProxy value, ICredentials credentials )
    {
        _handler.Proxy                   = value;
        _handler.UseProxy                = true;
        _handler.DefaultProxyCredentials = credentials;
        return this;
    }


    public HttpRequestBuilder With_MaxRedirects( int value )
    {
        _handler.MaxAutomaticRedirections = value;
        _handler.AllowAutoRedirect        = value > 0;
        return this;
    }


    public HttpRequestBuilder With_Credentials( ICredentials value, bool preAuthenticate = true )
    {
        _handler.Credentials     = value;
        _handler.PreAuthenticate = preAuthenticate;
        return this;
    }


    public HttpRequestBuilder With_Cookie( Uri url, Cookie value )
    {
        _handler.CookieContainer.Add(url, value);
        _handler.UseCookies = true;
        return this;
    }
    public HttpRequestBuilder With_Cookies( params Cookie[] value )
    {
        CookieContainer container = _handler.CookieContainer;
        foreach ( Cookie cookie in value ) { container.Add(cookie); }

        _handler.UseCookies = true;
        return this;
    }
    public HttpRequestBuilder With_Cookies( CookieContainer value )
    {
        _handler.CookieContainer = value;
        _handler.UseCookies      = true;
        return this;
    }


    public HttpRequestBuilder With_Timeout( int    minutes ) => With_Timeout(TimeSpan.FromMinutes(minutes));
    public HttpRequestBuilder With_Timeout( float  seconds ) => With_Timeout(TimeSpan.FromSeconds(seconds));
    public HttpRequestBuilder With_Timeout( double milliseconds ) => With_Timeout(TimeSpan.FromMilliseconds(milliseconds));
    public HttpRequestBuilder With_Timeout( in TimeSpan value )
    {
        _handler.ConnectTimeout = value;
        return this;
    }


    public HttpRequestBuilder With_SSL( SslClientAuthenticationOptions value )
    {
        _handler.SslOptions = value;
        return this;
    }


    public HttpRequestBuilder With_KeepAlive( int pingDelayMinutes, int pingTimeoutMinutes, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromMinutes(pingDelayMinutes), TimeSpan.FromMinutes(pingTimeoutMinutes), policy);
    public HttpRequestBuilder With_KeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromSeconds(pingDelaySeconds), TimeSpan.FromSeconds(pingTimeoutSeconds), policy);
    public HttpRequestBuilder With_KeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        With_KeepAlive(TimeSpan.FromMilliseconds(pingDelayMilliseconds), TimeSpan.FromMilliseconds(pingTimeoutMilliseconds), policy);
    public HttpRequestBuilder With_KeepAlive( in TimeSpan pingDelay, in TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
    {
        _handler.KeepAlivePingDelay   = pingDelay;
        _handler.KeepAlivePingTimeout = pingTimeout;
        _handler.KeepAlivePingPolicy  = policy;
        return this;
    }


    private HttpContent Update( in HttpContent content )
    {
        if ( headers is null ) { return content; }

        foreach ( ( string key, object v ) in headers ) { content.Headers.Add(key, v.ToString()); }

        return content;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] private Handler CreateHandler( Task<HttpResponseMessage> task ) => new(task, _encoding, _token);


    public Handler Post( byte[] value )
    {
        var content = new ByteArrayContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Post( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Post( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Post( Stream value )
    {
        var content = new StreamContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, content, _token);
        return CreateHandler(task);
    }
    public Handler Post( MultipartFormDataContent content )
    {
        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Post( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Post( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Post( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }


    public Handler Put( byte[] value )
    {
        var content = new ByteArrayContent(value);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( Stream value )
    {
        var                       content = new StreamContent(value);
        Task<HttpResponseMessage> task    = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( MultipartFormDataContent content )
    {
        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }
    public Handler Put( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return CreateHandler(task);
    }


    public Handler Delete( byte[] value )
    {
        var content = new ByteArrayContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = content
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( Stream value )
    {
        var content = new StreamContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( MultipartFormDataContent content )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Delete()
    {
        Task<HttpResponseMessage> task = _Client.DeleteAsync(_url, _token);
        return CreateHandler(task);
    }


    public Handler Patch( byte[] value )
    {
        var content = new ByteArrayContent(value);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = content
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( Stream value )
    {
        var content = new StreamContent(value);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( MultipartFormDataContent content )
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }
    public Handler Patch( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Patch, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }


    public Handler Get()
    {
        var                       request = new HttpRequestMessage(HttpMethod.Get, _url);
        Task<HttpResponseMessage> task    = _Client.SendAsync(request, _token);
        return CreateHandler(task);
    }



    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public readonly struct Handler
    {
        private readonly  Task<HttpResponseMessage> _request;
        internal readonly Encoding                  encoding;
        internal readonly CancellationToken         token;


        public Handler( Task<HttpResponseMessage> request, Encoding encoding, in CancellationToken token )
        {
            this._request = request;
            this.encoding = encoding;
            this.token    = token;
        }


        public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _request.GetAwaiter();


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
        public async Task<ResponseData<TResult>> AsJson<TResult>( JsonSerializer serializer ) => await ResponseData<TResult>.Create(this, serializer, AsJson<TResult>);
        public async Task<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            await using Stream s       = await content.ReadAsStreamAsync(token);
            using var          sr      = new StreamReader(s, encoding);
            using JsonReader   reader  = new JsonTextReader(sr);

            return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
        }


        public async Task<ResponseData<string>> AsString() => await ResponseData<string>.Create(this, AsString);
        public async Task<string> AsString( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            return await content.ReadAsStringAsync(token);
        }


        public async Task<ResponseData<byte[]>> AsBytes() => await ResponseData<byte[]>.Create(this, AsBytes);
        public async Task<byte[]> AsBytes( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            return await content.ReadAsByteArrayAsync(token);
        }


        public async Task<ResponseData<ReadOnlyMemory<byte>>> AsMemory() => await ResponseData<ReadOnlyMemory<byte>>.Create(this, AsMemory);
        public async Task<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent content = response.Content;
            byte[]            bytes   = await content.ReadAsByteArrayAsync(token);

            return bytes;
        }


        public async Task<ResponseData<LocalFile>> AsFile() => await ResponseData<LocalFile>.Create(this, AsFile);
        public async Task<LocalFile> AsFile( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent      content = response.Content;
            await using FileStream stream  = LocalFile.CreateTempFileAndOpen(out LocalFile file);
            await using Stream     sr      = await content.ReadAsStreamAsync(token);
            await sr.CopyToAsync(stream, token);

            return file;
        }


        public async Task<ResponseData<MemoryStream>> AsStream() => await ResponseData<MemoryStream>.Create(this, AsStream);
        public async Task<MemoryStream> AsStream( HttpResponseMessage response )
        {
            response.EnsureSuccessStatusCode();
            using HttpContent  content = response.Content;
            await using Stream stream  = await content.ReadAsStreamAsync(token);
            var                result  = new MemoryStream();

            await stream.CopyToAsync(result, token);
            return result;
        }
    }
}



#endif
