// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public readonly record struct WebHandler : IDisposable
{
    public static readonly EventId            EventId = new(69420, nameof(SendAsync));
    private readonly       HttpClient         _client;
    private readonly       HttpRequestMessage _request;
    private readonly       ILogger?           _logger;


    public   HttpContentHeaders? ContentHeaders => _request.Content?.Headers;
    internal Encoding            Encoding       { get; }
    public   HttpRequestHeaders  Headers        => _request.Headers;
    public   string              Method         => _request.Method.Method;
    public   Uri                 RequestUri     => _request.RequestUri ?? throw new NullReferenceException( nameof(_request.RequestUri) );
    internal RetryPolicy?        RetryPolicy    { get; }
    internal CancellationToken   Token          { get; }
    public   AppVersion          Version        { get => _request.Version; set => _request.Version = value.ToVersion(); }


    public WebHandler( ILogger? logger, HttpClient client, HttpRequestMessage request, Encoding encoding, RetryPolicy? retryPolicy, CancellationToken token )
    {
        _logger     = logger;
        _request    = request;
        _client     = client;
        Encoding    = encoding;
        Token       = token;
        RetryPolicy = retryPolicy;
    }
    public void Dispose() => _request.Dispose();


    public async ValueTask<HttpResponseMessage> SendAsync()
    {
        HttpResponseMessage response = await _client.SendAsync( _request, Token );
        _logger?.LogDebug( EventId, "Response StatusCode: {StatusCode} for {Uri}", response.StatusCode, _request.RequestUri?.OriginalString );
        return response;
    }
    public ValueTaskAwaiter<HttpResponseMessage> GetAwaiter() => SendAsync().GetAwaiter();


    public async ValueTask NoResponse()
    {
        using ( this )
        {
            using HttpResponseMessage response = await this;
            response.EnsureSuccessStatusCode();
        }
    }
    public async ValueTask<bool> AsBool( HttpResponseMessage response )
    {
        string content = await AsString( response );
        return bool.TryParse( content, out bool result ) && result;
    }
    public async ValueTask<Guid?> AsGuid( HttpResponseMessage response )
    {
        string content = await AsString( response );

        return Guid.TryParse( content, out Guid result )
                   ? result
                   : Guid.Empty;
    }
    public async ValueTask<byte[]> AsBytes( HttpResponseMessage response )
    {
        await using MemoryStream stream = await AsStream( response );
        return stream.ToArray();
    }
    public async ValueTask<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings )
    {
        await using MemoryStream stream = await AsStream( response );
        using StreamReader       sr     = new StreamReader( stream, Encoding );
        await using JsonReader   reader = new JsonTextReader( sr );

        return await JToken.ReadFromAsync( reader, settings, Token );
    }
    public async ValueTask<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer )
    {
        await using MemoryStream stream = await AsStream( response );
        using StreamReader       sr     = new StreamReader( stream, Encoding );
        await using JsonReader   reader = new JsonTextReader( sr );
        TResult?                 result = serializer.Deserialize<TResult>( reader );
        return result ?? throw new NullReferenceException( nameof(JsonConvert.DeserializeObject) );
    }
    public async ValueTask<LocalFile> AsFile( HttpResponseMessage response )
    {
        await using MemoryStream stream = await AsStream( response );
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen( out LocalFile file );
        await stream.CopyToAsync( fs, Token );

        return file;
    }
    public async ValueTask<LocalFile> AsFile( HttpResponseMessage response, string fileNameHeader )
    {
        if ( response.Headers.Contains( fileNameHeader ) )
        {
            MimeType mimeType = response.Headers.GetValues( fileNameHeader ).First().ToMimeType();

            return await AsFile( response, mimeType );
        }


        if ( response.Content.Headers.Contains( fileNameHeader ) )
        {
            MimeType mimeType = response.Content.Headers.GetValues( fileNameHeader ).First().ToMimeType();

            return await AsFile( response, mimeType );
        }


        await using Stream     stream = await AsStream( response );
        await using FileStream fs     = LocalFile.CreateTempFileAndOpen( out LocalFile file );
        await stream.CopyToAsync( fs, Token );

        return file;
    }
    public async ValueTask<LocalFile> AsFile( HttpResponseMessage response, FileInfo path )
    {
        LocalFile file = new LocalFile( path );
        return await AsFile( response, file );
    }
    public async ValueTask<LocalFile> AsFile( HttpResponseMessage response, LocalFile file )
    {
        await using MemoryStream stream = await AsStream( response );
        await file.WriteAsync( stream, Token );
        return file;
    }
    public async ValueTask<LocalFile> AsFile( HttpResponseMessage response, MimeType type )
    {
        await using MemoryStream stream = await AsStream( response );
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen( type, out LocalFile file );
        await stream.CopyToAsync( fs, Token );
        return file;
    }
    public async ValueTask<MemoryStream> AsStream( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        HttpContent        content = response.Content;
        await using Stream stream  = await content.ReadAsStreamAsync( Token );
        MemoryStream       buffer  = new MemoryStream( (int)stream.Length );
        await stream.CopyToAsync( buffer, Token );

        buffer.Seek( 0, SeekOrigin.Begin );
        return buffer;
    }
    public async ValueTask<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response ) => await AsBytes( response );
    public async ValueTask<string> AsString( HttpResponseMessage response )
    {
        response.EnsureSuccessStatusCode();
        HttpContent content = response.Content;
        return await content.ReadAsStringAsync( Token );
    }


/*

    public async ValueTask<TResult> AsJson<TResult>( HttpResponseMessage response, JsonTypeInfo<TResult> info )
    {
        await using MemoryStream stream = await AsStream( response );

        TResult? result = await JsonSerializer.DeserializeAsync( stream, info, Token );
        return result ?? throw new NullReferenceException( nameof(JsonSerializer.DeserializeAsync) );
    }

*/


    public       ValueTask<WebResponse<bool>>                 AsBool()                                     => WebResponse<bool>.Create( this, AsBool );
    public       ValueTask<WebResponse<byte[]>>               AsBytes()                                    => WebResponse<byte[]>.Create( this, AsBytes );
    public       ValueTask<WebResponse<JToken>>               AsJson()                                     => AsJson( JsonNet.LoadSettings );
    public async ValueTask<WebResponse<JToken>>               AsJson( JsonLoadSettings settings )          => await WebResponse<JToken>.Create( this, settings, AsJson );
    public       ValueTask<WebResponse<TResult>>              AsJson<TResult>()                            => AsJson<TResult>( JsonNet.Serializer );
    public       ValueTask<WebResponse<TResult>>              AsJson<TResult>( JsonSerializer serializer ) => WebResponse<TResult>.Create( this, serializer, AsJson<TResult> );
    public       ValueTask<WebResponse<LocalFile>>            AsFile()                                     => WebResponse<LocalFile>.Create( this, AsFile );
    public       ValueTask<WebResponse<LocalFile>>            AsFile( string    fileNameHeader )           => WebResponse<LocalFile>.Create( this, fileNameHeader, AsFile );
    public       ValueTask<WebResponse<LocalFile>>            AsFile( FileInfo  path )                     => WebResponse<LocalFile>.Create( this, path,           AsFile );
    public       ValueTask<WebResponse<LocalFile>>            AsFile( LocalFile file )                     => WebResponse<LocalFile>.Create( this, file,           AsFile );
    public       ValueTask<WebResponse<LocalFile>>            AsFile( MimeType  type )                     => WebResponse<LocalFile>.Create( this, type,           AsFile );
    public       ValueTask<WebResponse<MemoryStream>>         AsStream()                                   => WebResponse<MemoryStream>.Create( this, AsStream );
    public       ValueTask<WebResponse<ReadOnlyMemory<byte>>> AsMemory()                                   => WebResponse<ReadOnlyMemory<byte>>.Create( this, AsMemory );
    public       ValueTask<WebResponse<string>>               AsString()                                   => WebResponse<string>.Create( this, AsString );


    public HttpVersionPolicy  VersionPolicy { get => _request.VersionPolicy; set => _request.VersionPolicy = value; }
    public HttpRequestOptions Options       => _request.Options;
}
