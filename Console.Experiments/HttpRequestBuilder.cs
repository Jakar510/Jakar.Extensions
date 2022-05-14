// Jakar.Extensions :: Jakar.Extensions
// 05/03/2022  9:01 AM

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
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


public sealed class HttpRequestBuilder : HttpRequestBuilder.IResponses
{
    private readonly Encoding                   _encoding;
    private readonly Uri                        _url;
    private readonly CancellationToken          _token;
    private          HeaderCollection?          _headers;
    private          Task<HttpResponseMessage>? _request;
    private readonly SocketsHttpHandler         _handler = new();
    private          HttpClient                 _Client => new(_handler);


    private HttpRequestBuilder( Uri url, Encoding encoding, in CancellationToken token )
    {
        _url      = url;
        _encoding = encoding;
        _token    = token;
    }
    public static HttpRequestBuilder Create( Uri url, in CancellationToken token                                = default ) => new(url, Encoding.Default, token);
    public static HttpRequestBuilder Create( Uri url, Encoding             encoding, in CancellationToken token = default ) => new(url, encoding, token);


    public HttpRequestBuilder WithHeaders( HeaderCollection value )
    {
        _headers = value;
        return this;
    }


    public HttpRequestBuilder WithProxy( IWebProxy value )
    {
        _handler.Proxy    = value;
        _handler.UseProxy = true;
        return this;
    }
    public HttpRequestBuilder WithProxy( IWebProxy value, ICredentials credentials )
    {
        _handler.Proxy                   = value;
        _handler.UseProxy                = true;
        _handler.DefaultProxyCredentials = credentials;
        return this;
    }


    public HttpRequestBuilder WithMaxRedirects( int value )
    {
        _handler.MaxAutomaticRedirections = value;
        _handler.AllowAutoRedirect        = value > 0;
        return this;
    }


    public HttpRequestBuilder WithCredentials( ICredentials value, bool preAuthenticate = true )
    {
        _handler.Credentials     = value;
        _handler.PreAuthenticate = preAuthenticate;
        return this;
    }


    public HttpRequestBuilder WithCookie( Uri url, Cookie value )
    {
        _handler.CookieContainer.Add(url, value);
        _handler.UseCookies = true;
        return this;
    }
    public HttpRequestBuilder WithCookies( params Cookie[] value )
    {
        CookieContainer container = _handler.CookieContainer;
        foreach ( Cookie cookie in value ) { container.Add(cookie); }

        _handler.UseCookies = true;
        return this;
    }
    public HttpRequestBuilder WithCookies( CookieContainer value )
    {
        _handler.CookieContainer = value;
        _handler.UseCookies      = true;
        return this;
    }


    public HttpRequestBuilder WithTimeout( int    minutes ) => WithTimeout(TimeSpan.FromMinutes(minutes));
    public HttpRequestBuilder WithTimeout( float  seconds ) => WithTimeout(TimeSpan.FromSeconds(seconds));
    public HttpRequestBuilder WithTimeout( double milliseconds ) => WithTimeout(TimeSpan.FromMilliseconds(milliseconds));
    public HttpRequestBuilder WithTimeout( in TimeSpan value )
    {
        _handler.ConnectTimeout = value;
        return this;
    }


    public HttpRequestBuilder WithSSL( SslClientAuthenticationOptions value )
    {
        _handler.SslOptions = value;
        return this;
    }


    public HttpRequestBuilder WithKeepAlive( int pingDelayMinutes, int pingTimeoutMinutes, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        WithKeepAlive(TimeSpan.FromMinutes(pingDelayMinutes), TimeSpan.FromMinutes(pingTimeoutMinutes), policy);
    public HttpRequestBuilder WithKeepAlive( float pingDelaySeconds, float pingTimeoutSeconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        WithKeepAlive(TimeSpan.FromSeconds(pingDelaySeconds), TimeSpan.FromSeconds(pingTimeoutSeconds), policy);
    public HttpRequestBuilder WithKeepAlive( double pingDelayMilliseconds, double pingTimeoutMilliseconds, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests ) =>
        WithKeepAlive(TimeSpan.FromMilliseconds(pingDelayMilliseconds), TimeSpan.FromMilliseconds(pingTimeoutMilliseconds), policy);
    public HttpRequestBuilder WithKeepAlive( in TimeSpan pingDelay, in TimeSpan pingTimeout, HttpKeepAlivePingPolicy policy = HttpKeepAlivePingPolicy.WithActiveRequests )
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


    public IResponses Post( byte[] value )
    {
        var content = new ByteArrayContent(value);

        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Post( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);

        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Post( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);

        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Post( Stream value )
    {
        var content = new StreamContent(value);

        _request = _Client.PostAsync(_url, content, _token);
        return this;
    }
    public IResponses Post( MultipartFormDataContent content )
    {
        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Post( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);

        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Post( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Post( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        _request = _Client.PostAsync(_url, Update(content), _token);
        return this;
    }


    public IResponses Put( byte[] value )
    {
        var content = new ByteArrayContent(value);

        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);

        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);

        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( Stream value )
    {
        var content = new StreamContent(value);
        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( MultipartFormDataContent content )
    {
        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);

        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }
    public IResponses Put( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);

        _request = _Client.PutAsync(_url, Update(content), _token);
        return this;
    }


    public IResponses Delete( byte[] value )
    {
        var content = new ByteArrayContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( ReadOnlyMemory<byte> value )
    {
        var content = new ReadOnlyMemoryContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( IDictionary<string, string> value )
    {
        var content = new FormUrlEncodedContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = content
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( Stream value )
    {
        var content = new StreamContent(value);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( MultipartFormDataContent content )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( string value )
    {
        var content = new StringContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( BaseClass value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete( BaseRecord value )
    {
        var content = new JsonContent(value.ToPrettyJson(), _encoding);


        var request = new HttpRequestMessage(HttpMethod.Delete, _url)
                      {
                          Content = Update(content)
                      };

        _request = _Client.SendAsync(request, _token);
        return this;
    }
    public IResponses Delete()
    {
        _request = _Client.DeleteAsync(_url, _token);
        return this;
    }


    public IResponses Get()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _url);
        _request = _Client.SendAsync(request, _token);
        return this;
    }



    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IResponses
    {
        public Task<JToken> AsJson();
        public Task<JToken> AsJson( JsonLoadSettings settings );

        public Task<TResult> AsJson<TResult>();
        public Task<TResult> AsJson<TResult>( JsonSerializer settings );

        public Task<string> AsString();
        public Task<byte[]> AsBytes();
        public Task<ReadOnlyMemory<byte>> AsMemory();

        public Task<LocalFile> AsFile();
        public Task<MemoryStream> AsStream();
    }



    async Task<JToken> IResponses.AsJson() => await ( (IResponses)this ).AsJson(JsonNet.LoadSettings);
    async Task<JToken> IResponses.AsJson( JsonLoadSettings settings )
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent  content = reply.Content;
        await using Stream s       = await content.ReadAsStreamAsync(_token);
        using var          sr      = new StreamReader(s, _encoding);
        using JsonReader   reader  = new JsonTextReader(sr);
        return await JToken.ReadFromAsync(reader, settings, _token);
    }


    async Task<TResult> IResponses.AsJson<TResult>() => await ( (IResponses)this ).AsJson<TResult>(JsonNet.Serializer);
    async Task<TResult> IResponses.AsJson<TResult>( JsonSerializer serializer )
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent  content = reply.Content;
        await using Stream s       = await content.ReadAsStreamAsync(_token);
        using var          sr      = new StreamReader(s, _encoding);
        using JsonReader   reader  = new JsonTextReader(sr);

        return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    }


    async Task<string> IResponses.AsString()
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent content = reply.Content;
        return await content.ReadAsStringAsync(_token);
    }


    async Task<byte[]> IResponses.AsBytes()
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent content = reply.Content;
        return await content.ReadAsByteArrayAsync(_token);
    }
    async Task<ReadOnlyMemory<byte>> IResponses.AsMemory()
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent content = reply.Content;
        byte[]            bytes   = await content.ReadAsByteArrayAsync(_token);

        return bytes;
    }


    async Task<LocalFile> IResponses.AsFile()
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent      content = reply.Content;
        await using FileStream stream  = LocalFile.CreateTempFileAndOpen(out LocalFile file);
        await using Stream     sr      = await content.ReadAsStreamAsync(_token);
        await sr.CopyToAsync(stream, _token);

        return file;
    }


    async Task<MemoryStream> IResponses.AsStream()
    {
        ArgumentNullException.ThrowIfNull(_request);
        using HttpResponseMessage reply = await _request;
        reply.EnsureSuccessStatusCode();
        using HttpContent  content = reply.Content;
        await using Stream stream  = await content.ReadAsStreamAsync(_token);
        var                sr      = new MemoryStream();

        await stream.CopyToAsync(sr, _token);
        return sr;
    }
}
