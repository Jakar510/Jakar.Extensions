// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

using System.Net.Http;
using System.Net.Http.Headers;



namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class WebHandler : IDisposable
{
    private readonly   HttpClient         _client;
    private readonly   HttpRequestMessage _request;
    protected internal CancellationToken  Token    { get; }
    protected internal Encoding           Encoding { get; }


    public string              Method         => _request.Method.Method;
    public HttpRequestHeaders  Headers        => _request.Headers;
    public HttpContentHeaders? ContentHeaders => _request.Content?.Headers;
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


    public WebHandler( HttpClient client, HttpRequestMessage request, Encoding encoding, CancellationToken token )
    {
        _request = request;
        _client  = client;
        Encoding = encoding;
        Token    = token;
    }
    public virtual void Dispose() => _request.Dispose();


    public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _client.SendAsync(_request, Token)
                                                                   .GetAwaiter();


    public virtual Task<WebResponse<JToken>> AsJson() => AsJson(JsonNet.LoadSettings);
    public virtual async Task<WebResponse<JToken>> AsJson( JsonLoadSettings settings ) => await WebResponse<JToken>.Create(this, settings, AsJson);
    public virtual async Task<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
    #if NET6_0
        await using Stream stream = await content.ReadAsStreamAsync(Token);
    #else
            await using Stream stream = await content.ReadAsStreamAsync();
    #endif
        using var        sr     = new StreamReader(stream, Encoding);
        using JsonReader reader = new JsonTextReader(sr);
        return await JToken.ReadFromAsync(reader, settings, Token);
    }


    public virtual Task<WebResponse<TResult>> AsJson<TResult>() => AsJson<TResult>(JsonNet.Serializer);
    public virtual Task<WebResponse<TResult>> AsJson<TResult>( JsonSerializer serializer ) => WebResponse<TResult>.Create(this, serializer, AsJson<TResult>);
    public virtual async Task<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
    #if NET6_0
        await using Stream stream = await content.ReadAsStreamAsync(Token);
    #else
            await using Stream stream = await content.ReadAsStreamAsync();
    #endif
        using var        sr     = new StreamReader(stream, Encoding);
        using JsonReader reader = new JsonTextReader(sr);

        return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    }


    public virtual Task<WebResponse<string>> AsString() => WebResponse<string>.Create(this, AsString);
    public virtual async Task<string> AsString( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
    #if NET6_0
        return await content.ReadAsStringAsync(Token);
    #else
        return await content.ReadAsStringAsync();
    #endif
    }


    public virtual Task<WebResponse<byte[]>> AsBytes() => WebResponse<byte[]>.Create(this, AsBytes);
    public virtual async Task<byte[]> AsBytes( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
    #if NET6_0
        return await content.ReadAsByteArrayAsync(Token);
    #else
            return await content.ReadAsByteArrayAsync();
    #endif
    }


    public virtual Task<WebResponse<ReadOnlyMemory<byte>>> AsMemory() => WebResponse<ReadOnlyMemory<byte>>.Create(this, AsMemory);
    public virtual async Task<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;

    #if NET6_0
        byte[] bytes = await content.ReadAsByteArrayAsync(Token);
    #else
            byte[] bytes = await content.ReadAsByteArrayAsync();
    #endif

        return bytes;
    }


    public virtual Task<WebResponse<LocalFile>> AsFile() => WebResponse<LocalFile>.Create(this, AsFile);
    public virtual async Task<LocalFile> AsFile( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent      content = response.Content;
        await using FileStream stream  = LocalFile.CreateTempFileAndOpen(out LocalFile file);
        await stream.CopyToAsync(stream, Token);

        return file;
    }


    public virtual Task<WebResponse<LocalFile>> AsFile( string relativePath ) => WebResponse<LocalFile>.Create(this, relativePath, AsFile);
    public virtual async Task<LocalFile> AsFile( HttpResponseMessage response, string relativePath )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
        var               file    = new LocalFile(relativePath);
    #if NET6_0
        await using Stream stream = await content.ReadAsStreamAsync(Token);
    #else
            await using Stream stream = await content.ReadAsStreamAsync();
    #endif
        await file.WriteAsync(stream, Token);

        return file;
    }


    public virtual Task<WebResponse<LocalFile>> AsFile( LocalFile file ) => WebResponse<LocalFile>.Create(this, file, AsFile);
    public virtual async Task<LocalFile> AsFile( HttpResponseMessage response, LocalFile file )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
    #if NET6_0
        await using Stream stream = await content.ReadAsStreamAsync(Token);
    #else
            await using Stream stream = await content.ReadAsStreamAsync();
    #endif
        await file.WriteAsync(stream, Token);

        return file;
    }


    public virtual Task<WebResponse<LocalFile>> AsFile( MimeType type ) => WebResponse<LocalFile>.Create(this, type, AsFile);
    public virtual async Task<LocalFile> AsFile( HttpResponseMessage response, MimeType type )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent      content = response.Content;
        await using FileStream stream  = LocalFile.CreateTempFileAndOpen(type, out LocalFile file);
        await stream.CopyToAsync(stream, Token);

        return file;
    }


    public virtual Task<WebResponse<MemoryStream>> AsStream() => WebResponse<MemoryStream>.Create(this, AsStream);
    public virtual async Task<MemoryStream> AsStream( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;
    #if NET6_0
        await using Stream stream = await content.ReadAsStreamAsync(Token);
    #else
            await using Stream stream = await content.ReadAsStreamAsync();
    #endif
        var result = new MemoryStream();

        await stream.CopyToAsync(result, Token);
        return result;
    }
}
