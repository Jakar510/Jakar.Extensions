// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class WebHandler : IDisposable
{
    public const string FILE_NAME = "FileName";


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


    public virtual ValueTask<WebResponse<JToken>> AsJson() => AsJson(JsonNet.LoadSettings);
    public virtual async ValueTask<WebResponse<JToken>> AsJson( JsonLoadSettings settings ) => await WebResponse<JToken>.Create(this, settings, AsJson);
    public virtual async ValueTask<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings )
    {
        await using MemoryStream stream = await AsStream(response);
        using var                sr     = new StreamReader(stream, Encoding);
        using JsonReader         reader = new JsonTextReader(sr);
        return await JToken.ReadFromAsync(reader, settings, Token);
    }


    public virtual ValueTask<WebResponse<TResult>> AsJson<TResult>() => AsJson<TResult>(JsonNet.Serializer);
    public virtual ValueTask<WebResponse<TResult>> AsJson<TResult>( JsonSerializer serializer ) => WebResponse<TResult>.Create(this, serializer, AsJson<TResult>);
    public virtual async ValueTask<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
    {
        await using MemoryStream stream = await AsStream(response);
        using var                sr     = new StreamReader(stream, Encoding);
        using JsonReader         reader = new JsonTextReader(sr);

        return serializer.Deserialize<TResult>(reader) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    }


    public virtual async ValueTask NoResponse()
    {
        using ( this )
        {
            using HttpResponseMessage response = await this;
            response.EnsureSuccessStatusCode();
        }
    }


    public virtual ValueTask<WebResponse<bool>> AsBool() => WebResponse<bool>.Create(this, AsBool);
    public virtual async ValueTask<bool> AsBool( HttpResponseMessage response )
    {
        string content = await AsString(response);
        return bool.TryParse(content, out bool result) && result;
    }


    public virtual ValueTask<WebResponse<string>> AsString() => WebResponse<string>.Create(this, AsString);
    public virtual async ValueTask<string> AsString( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;

    #if NETSTANDARD2_1
        return await content.ReadAsStringAsync();
    #else
        return await content.ReadAsStringAsync(Token);
    #endif
    }


    public virtual ValueTask<WebResponse<byte[]>> AsBytes() => WebResponse<byte[]>.Create(this, AsBytes);
    public virtual async ValueTask<byte[]> AsBytes( HttpResponseMessage response )
    {
        await using MemoryStream stream = await AsStream(response);
        return stream.ToArray();
    }


    public virtual ValueTask<WebResponse<ReadOnlyMemory<byte>>> AsMemory() => WebResponse<ReadOnlyMemory<byte>>.Create(this, AsMemory);
    public virtual async ValueTask<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response ) => await AsBytes(response);


    public virtual ValueTask<WebResponse<LocalFile>> AsFile() => WebResponse<LocalFile>.Create(this, AsFile);
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response )
    {
        await using MemoryStream stream = await AsStream(response);
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen(out LocalFile file);
        await stream.CopyToAsync(fs, Token);

        return file;
    }


    public virtual ValueTask<WebResponse<LocalFile>> AsFile( string fileNameHeader ) => WebResponse<LocalFile>.Create(this, fileNameHeader, AsFile);
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, string fileNameHeader )
    {
        if ( response.Headers.Contains(fileNameHeader) )
        {
            var mimeType = response.Headers.GetValues(fileNameHeader)
                                   .First()
                                   .ToMimeType();

            return await AsFile(response, mimeType);
        }


        if ( response.Content.Headers.Contains(fileNameHeader) )
        {
            var mimeType = response.Content.Headers.GetValues(fileNameHeader)
                                   .First()
                                   .ToMimeType();

            return await AsFile(response, mimeType);
        }


        await using Stream     stream = await AsStream(response);
        await using FileStream fs     = LocalFile.CreateTempFileAndOpen(out LocalFile file);
        await stream.CopyToAsync(fs, Token);

        return file;
    }


    public virtual ValueTask<WebResponse<LocalFile>> AsFile( FileInfo path ) => WebResponse<LocalFile>.Create(this, path, AsFile);
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, FileInfo path )
    {
        var file = new LocalFile(path);
        return await AsFile(response, file);
    }


    public virtual ValueTask<WebResponse<LocalFile>> AsFile( LocalFile file ) => WebResponse<LocalFile>.Create(this, file, AsFile);
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, LocalFile file )
    {
        await using MemoryStream stream = await AsStream(response);
        await file.WriteAsync(stream, Token);
        return file;
    }


    public virtual ValueTask<WebResponse<LocalFile>> AsFile( MimeType type ) => WebResponse<LocalFile>.Create(this, type, AsFile);
    public virtual async ValueTask<LocalFile> AsFile( HttpResponseMessage response, MimeType type )
    {
        await using MemoryStream stream = await AsStream(response);
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen(type, out LocalFile file);
        await stream.CopyToAsync(fs, Token);
        return file;
    }


    public virtual ValueTask<WebResponse<MemoryStream>> AsStream() => WebResponse<MemoryStream>.Create(this, AsStream);
    public virtual async ValueTask<MemoryStream> AsStream( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        using HttpContent content = response.Content;

    #if NETSTANDARD2_1
        await using Stream stream = await content.ReadAsStreamAsync();
    #else
        await using Stream stream = await content.ReadAsStreamAsync(Token);
    #endif

        var buffer = new MemoryStream((int)stream.Length);
        await stream.CopyToAsync(buffer, Token);

        buffer.Seek(0, SeekOrigin.Begin);
        return buffer;
    }
}
