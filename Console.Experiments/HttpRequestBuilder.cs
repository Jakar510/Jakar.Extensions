// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.FileSystemExtensions;
using Jakar.Extensions.Models.Base.Classes;
using Jakar.Extensions.Models.Base.Records;
using Jakar.Extensions.Strings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



// ReSharper disable once CheckNamespace
namespace Jakar.Extensions.Http;


public sealed class HttpRequestBuilder
{
    private readonly Encoding           _encoding;
    private readonly Uri                _url;
    private readonly CancellationToken  _token;
    private readonly SocketsHttpHandler _handler = new();
    private          HeaderCollection?  _headers;
    private          HttpClient         _Client => new(_handler);


    private HttpRequestBuilder( Uri url, Encoding encoding, in CancellationToken token )
    {
        _url      = url;
        _encoding = encoding;
        _token    = token;
    }
    public static HttpRequestBuilder Create( Uri url, in CancellationToken token                                = default ) => new(url, Encoding.Default, token);
    public static HttpRequestBuilder Create( Uri url, Encoding             encoding, in CancellationToken token = default ) => new(url, encoding, token);


    public HttpRequestBuilder With_Headers( HeaderCollection value )
    {
        _headers = value;
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
        if ( _headers is null ) { return content; }

        foreach ( ( string? key, object v ) in _headers ) { content.Headers.Add(key, v.ToString()); }

        return content;
    }


    public Handler Post( byte[] value )
    {
        var content = new ByteArrayContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( Stream value )
    {
        var content = new StreamContent(value);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, content, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( MultipartFormDataContent content )
    {
        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Post( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PostAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }


    public Handler Put( byte[] value )
    {
        var content = new ByteArrayContent(value);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( Stream value )
    {
        var                       content = new StreamContent(value);
        Task<HttpResponseMessage> task    = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( MultipartFormDataContent content )
    {
        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Put( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        Task<HttpResponseMessage> task = _Client.PutAsync(_url, Update(content), _token);
        return new Handler(task, _encoding, _token);
    }


    public Handler Delete( byte[] value )
    {
        var content = new ByteArrayContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = content
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( Stream value )
    {
        var content = new StreamContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( MultipartFormDataContent content )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        Task<HttpResponseMessage> task = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }
    public Handler Delete()
    {
        Task<HttpResponseMessage> task = _Client.DeleteAsync(_url, _token);
        return new Handler(task, _encoding, _token);
    }


    public Handler Get()
    {
        var                       request = new HttpRequestMessage(HttpMethod.Get, _url);
        Task<HttpResponseMessage> task    = _Client.SendAsync(request, _token);
        return new Handler(task, _encoding, _token);
    }



    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public readonly struct Handler
    {
        private readonly Task<HttpResponseMessage> _request;
        private readonly Encoding                  _encoding;
        private readonly CancellationToken         _token;


        public Handler( Task<HttpResponseMessage> request, Encoding encoding, in CancellationToken token )
        {
            _request  = request;
            _encoding = encoding;
            _token    = token;
        }


        public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _request.GetAwaiter();


        public async Task<JToken> AsJson() => await AsJson(JsonNet.LoadSettings);
        public async Task<JToken> AsJson( JsonLoadSettings settings )
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent  content = reply.Content;
            await using Stream s       = await content.ReadAsStreamAsync(_token);
            using var          sr      = new StreamReader(s, _encoding);
            using JsonReader   reader  = new JsonTextReader(sr);
            return await JToken.ReadFromAsync(reader, settings, _token);
        }


        public async Task<TResult> AsJson<TResult>() => await AsJson<TResult>(JsonNet.Serializer);
        public async Task<TResult> AsJson<TResult>( JsonSerializer serializer )
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent  content = reply.Content;
            await using Stream s       = await content.ReadAsStreamAsync(_token);
            using var          sr      = new StreamReader(s, _encoding);
            using JsonReader   reader  = new JsonTextReader(sr);

            return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
        }


        public async Task<string> AsString()
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent content = reply.Content;
            return await content.ReadAsStringAsync(_token);
        }


        public async Task<byte[]> AsBytes()
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent content = reply.Content;
            return await content.ReadAsByteArrayAsync(_token);
        }
        public async Task<ReadOnlyMemory<byte>> AsMemory()
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent content = reply.Content;
            byte[]            bytes   = await content.ReadAsByteArrayAsync(_token);

            return bytes;
        }


        public async Task<LocalFile> AsFile()
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent      content = reply.Content;
            await using FileStream stream  = LocalFile.CreateTempFileAndOpen(out LocalFile file);
            await using Stream     sr      = await content.ReadAsStreamAsync(_token);
            await sr.CopyToAsync(stream, _token);

            return file;
        }


        public async Task<MemoryStream> AsStream()
        {
            using HttpResponseMessage reply = await _request;
            reply.EnsureSuccessStatusCode();
            using HttpContent  content = reply.Content;
            await using Stream stream  = await content.ReadAsStreamAsync(_token);
            var                sr      = new MemoryStream();

            await stream.CopyToAsync(sr, _token);
            return sr;
        }
    }
}
