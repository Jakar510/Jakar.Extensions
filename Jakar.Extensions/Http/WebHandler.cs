// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public readonly struct WebHandler( WebRequester requester, HttpRequestMessage request ) : IDisposable
{
    public const           string             NO_RESPONSE = "NO RESPONSE";
    public static readonly EventId            EventId     = new(69420, nameof(SendAsync));
    private readonly       HttpRequestMessage __request    = request;
    private readonly       WebRequester       __requester  = requester;


    internal HttpClient          Client         => __requester.Client;
    public   HttpContentHeaders? ContentHeaders => __request.Content?.Headers;
    internal Encoding            Encoding       => __requester.Encoding;
    public   HttpRequestHeaders  Headers        => __request.Headers;
    internal ILogger?            Logger         => __requester.Logger;
    public   string              Method         => __request.Method.Method;
    public   HttpRequestOptions  Options        => __request.Options;
    public   Uri                 RequestUri     => __request.RequestUri ?? throw new NullReferenceException( nameof(__request.RequestUri) );
    internal RetryPolicy?        RetryPolicy    => __requester.Retries;
    public   AppVersion          Version        { get => __request.Version;       set => __request.Version = value.ToVersion(); }
    public   HttpVersionPolicy   VersionPolicy  { get => __request.VersionPolicy; set => __request.VersionPolicy = value; }


    public void Dispose() => __request.Dispose();


    public async ValueTask<HttpResponseMessage> SendAsync( CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        HttpResponseMessage response      = await Client.SendAsync( __request, token );
        Logger?.LogDebug( EventId, "Response StatusCode: {StatusCode} for {Uri}", response.StatusCode, __request.RequestUri?.OriginalString );
        return response;
    }


    public async ValueTask<WebResponse<TValue>> CreateResponse<TValue>( Func<HttpResponseMessage, CancellationToken, ValueTask<TValue>> func, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( this )
        {
            using HttpResponseMessage response = await SendAsync( token ).ConfigureAwait( false );

            try
            {
                RetryPolicy? policy = RetryPolicy;

                return policy?.AllowRetries is true
                           ? await WebResponse<TValue>.Create( response, func, policy.Value, token )
                           : await WebResponse<TValue>.Create( response, func, token );
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException( e );
                return await WebResponse<TValue>.Create( response, e, token );
            }
        }
    }
    public async ValueTask<WebResponse<TValue>> CreateResponse<TValue, TArg>( Func<HttpResponseMessage, TArg, CancellationToken, ValueTask<TValue>> func, TArg arg, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( this )
        {
            using HttpResponseMessage response = await SendAsync( token ).ConfigureAwait( false );

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await WebResponse<TValue>.Create( response, token ); }

                TValue result = await func( response, arg, token );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException( e );
                return await WebResponse<TValue>.Create( response, e, token );
            }
        }
    }
    public async ValueTask<WebResponse<TValue>> CreateResponse<TValue, TArg, TArg2>( Func<HttpResponseMessage, TArg, TArg2, CancellationToken, ValueTask<TValue>> func, TArg arg1, TArg2 arg2, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( this )
        {
            using HttpResponseMessage response = await SendAsync( token ).ConfigureAwait( false );

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await WebResponse<TValue>.Create( response, token ); }

                TValue result = await func( response, arg1, arg2, token );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException( e );
                return await WebResponse<TValue>.Create( response, e, token );
            }
        }
    }


    public ValueTask<WebResponse<bool>>                 AsBool( CancellationToken          token )                             => CreateResponse( AsBool,  token );
    public ValueTask<WebResponse<byte[]>>               AsBytes( CancellationToken         token )                             => CreateResponse( AsBytes, token );
    public ValueTask<WebResponse<JToken>>               AsJson( CancellationToken          token )                             => AsJson( JsonNet.LoadSettings, token );
    public ValueTask<WebResponse<JToken>>               AsJson( JsonLoadSettings           settings, CancellationToken token ) => CreateResponse( AsJson, settings, Encoding, token );
    public ValueTask<WebResponse<TResult>>              AsJson<TResult>( CancellationToken token )                               => AsJson<TResult>( JsonNet.Serializer, token );
    public ValueTask<WebResponse<TResult>>              AsJson<TResult>( JsonSerializer    serializer, CancellationToken token ) => CreateResponse( AsJson<TResult>, serializer, Encoding, token );
    public ValueTask<WebResponse<LocalFile>>            AsFile( CancellationToken          token )                                   => CreateResponse( AsFile, token );
    public ValueTask<WebResponse<LocalFile>>            AsFile( string                     fileNameHeader, CancellationToken token ) => CreateResponse( AsFile, fileNameHeader, token );
    public ValueTask<WebResponse<LocalFile>>            AsFile( FileInfo                   path,           CancellationToken token ) => CreateResponse( AsFile, path,           token );
    public ValueTask<WebResponse<LocalFile>>            AsFile( LocalFile                  file,           CancellationToken token ) => CreateResponse( AsFile, file,           token );
    public ValueTask<WebResponse<LocalFile>>            AsFile( MimeType                   type,           CancellationToken token ) => CreateResponse( AsFile, type,           token );
    public ValueTask<WebResponse<MemoryStream>>         AsStream( CancellationToken        token ) => CreateResponse( AsStream, token );
    public ValueTask<WebResponse<ReadOnlyMemory<byte>>> AsMemory( CancellationToken        token ) => CreateResponse( AsMemory, token );
    public ValueTask<WebResponse<string>>               AsString( CancellationToken        token ) => CreateResponse( AsString, token );


    public async ValueTask<ErrorOrResult> NoResponse( CancellationToken token )
    {
        using ( this )
        {
            using HttpResponseMessage response = await SendAsync( token );

            return response.IsSuccessStatusCode
                       ? true
                       : Errors.Create( response.StatusCode.ToStatus() );
        }
    }
    public static async ValueTask<JToken> AsJson( HttpResponseMessage response, JsonLoadSettings settings, Encoding encoding, CancellationToken token )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        await using MemoryStream stream        = await AsStream( response, token );
        using StreamReader       sr            = new(stream, encoding);
        await using JsonReader   reader        = new JsonTextReader( sr );
        return await JToken.ReadFromAsync( reader, settings, token );
    }
    public static async ValueTask<TResult> AsJson<TResult>( HttpResponseMessage response, JsonSerializer serializer, Encoding encoding, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent        content = response.Content;
        await using Stream stream  = await content.ReadAsStreamAsync( token );
        TResult            result  = stream.FromJson<TResult>( encoding );
        return result;
    }
    public static async ValueTask<bool> AsBool( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        string              content       = await AsString( response, token );
        return bool.TryParse( content, out bool result ) && result;
    }
    public static async ValueTask<Guid?> AsGuid( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        string              content       = await AsString( response, token );

        return Guid.TryParse( content, out Guid result )
                   ? result
                   : Guid.Empty;
    }
    public static async ValueTask<byte[]> AsBytes( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        await using MemoryStream stream        = await AsStream( response, token );
        return stream.ToArray();
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        await using MemoryStream stream        = await AsStream( response, token );
        await using FileStream   fs            = LocalFile.CreateTempFileAndOpen( out LocalFile file );
        await stream.CopyToAsync( fs, token );

        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, string fileNameHeader, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        if ( response.Headers.Contains( fileNameHeader ) )
        {
            MimeType mimeType = response.Headers.GetValues( fileNameHeader ).First().ToMimeType();

            return await AsFile( response, mimeType, token );
        }


        if ( response.Content.Headers.Contains( fileNameHeader ) )
        {
            MimeType mimeType = response.Content.Headers.GetValues( fileNameHeader ).First().ToMimeType();

            return await AsFile( response, mimeType, token );
        }


        await using Stream     stream = await AsStream( response, token );
        await using FileStream fs     = LocalFile.CreateTempFileAndOpen( out LocalFile file );
        using ( telemetrySpan.SubSpan("WriteToFile") ) { await stream.CopyToAsync( fs, token ); }

        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, FileInfo path, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        LocalFile           file          = new(path);
        return await AsFile( response, file, token );
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, LocalFile file, CancellationToken token )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        await using MemoryStream stream        = await AsStream( response, token );
        using ( telemetrySpan.SubSpan("WriteToFile") ) { await stream.CopyToAsync( stream, token ); }

        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, MimeType type, CancellationToken token )
    {
        await using MemoryStream stream = await AsStream( response, token );
        await using FileStream   fs     = LocalFile.CreateTempFileAndOpen( type, out LocalFile file );
        await stream.CopyToAsync( fs, token );
        return file;
    }
    public static async ValueTask<MemoryStream> AsStream( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent        content = response.Content;
        await using Stream stream  = await content.ReadAsStreamAsync( token );
        MemoryStream       buffer  = new((int)stream.Length);
        using ( telemetrySpan.SubSpan("WriteToFile") ) { await stream.CopyToAsync( buffer, token ); }

        buffer.Seek( 0, SeekOrigin.Begin );
        return buffer;
    }
    public static async ValueTask<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response, CancellationToken token ) => await AsBytes( response, token );
    public static async ValueTask<string> AsString( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent content = response.Content;
        return await content.ReadAsStringAsync( token );
    }


/*

    public async ValueTask<TResult> AsJson<TResult>( HttpResponseMessage response, JsonTypeInfo<TResult> info )
    {
        await using MemoryStream stream = await AsStream( response );

        TResult? result = await JsonSerializer.DeserializeAsync( stream, info, token );
        return result ?? throw new NullReferenceException( nameof(JsonSerializer.DeserializeAsync) );
    }

*/
}
