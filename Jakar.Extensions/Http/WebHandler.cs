// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

using Org.BouncyCastle.Asn1.Ocsp;



namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public readonly struct WebHandler( WebRequester requester, HttpRequestMessage request ) : IAsyncDisposable, IDisposable
{
    public const           string  NO_RESPONSE = "NO RESPONSE";
    public static readonly EventId EventId     = new(69420, nameof(SendAsync));


    internal HttpClient          Client         => requester.Client;
    public   HttpContentHeaders? ContentHeaders => request.Content?.Headers;
    internal Encoding            Encoding       => requester.Encoding;
    public   HttpRequestHeaders  Headers        => request.Headers;
    internal ILogger?            Logger         => requester.Logger;
    public   string              Method         => request.Method.Method;
    public   HttpRequestOptions  Options        => request.Options;
    public   Uri                 RequestUri     => request.RequestUri ?? throw new NullReferenceException(nameof(request.RequestUri));
    internal RetryPolicy?        RetryPolicy    => requester.Retries;
    public   AppVersion          Version        { get => request.Version;       set => request.Version = value.ToVersion(); }
    public   HttpVersionPolicy   VersionPolicy  { get => request.VersionPolicy; set => request.VersionPolicy = value; }


    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
    public void Dispose() => request.Dispose();


    public async ValueTask<HttpResponseMessage> SendAsync( CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        HttpResponseMessage response = await Client.SendAsync(request, token)
                                                   .ConfigureAwait(false);

        Logger?.LogDebug(EventId, "Response StatusCode: {StatusCode} for {Uri}", response.StatusCode, request.RequestUri?.OriginalString);
        return response;
    }


    public async ValueTask<WebResponse<TValue>> CreateResponse<TValue>( Func<HttpResponseMessage, CancellationToken, ValueTask<TValue>> func, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        await using ( this )
        {
            using HttpResponseMessage response = await SendAsync(token)
                                                    .ConfigureAwait(false);

            try
            {
                RetryPolicy? policy = RetryPolicy;

                return policy?.AllowRetries is true
                           ? await WebResponse<TValue>.Create(response, func, policy.Value, token)
                                                      .ConfigureAwait(false)
                           : await WebResponse<TValue>.Create(response, func, token)
                                                      .ConfigureAwait(false);
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException(e);

                return await WebResponse<TValue>.Create(response, e, token)
                                                .ConfigureAwait(false);
            }
        }
    }
    public async ValueTask<WebResponse<TValue>> CreateResponse<TValue, TArg>( Func<HttpResponseMessage, TArg, CancellationToken, ValueTask<TValue>> func, TArg arg, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        await using ( this )
        {
            using HttpResponseMessage response = await SendAsync(token)
                                                    .ConfigureAwait(false);

            try
            {
                if ( !response.IsSuccessStatusCode )
                {
                    return await WebResponse<TValue>.Create(response, token)
                                                    .ConfigureAwait(false);
                }

                TValue result = await func(response, arg, token)
                                   .ConfigureAwait(false);

                return new WebResponse<TValue>(response, result);
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException(e);

                return await WebResponse<TValue>.Create(response, e, token)
                                                .ConfigureAwait(false);
            }
        }
    }
    public async ValueTask<WebResponse<TValue>> CreateResponse<TValue, TArg, TArg2>( Func<HttpResponseMessage, TArg, TArg2, CancellationToken, ValueTask<TValue>> func, TArg arg1, TArg2 arg2, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        await using ( this )
        {
            using HttpResponseMessage response = await SendAsync(token)
                                                    .ConfigureAwait(false);

            try
            {
                if ( !response.IsSuccessStatusCode )
                {
                    return await WebResponse<TValue>.Create(response, token)
                                                    .ConfigureAwait(false);
                }

                TValue result = await func(response, arg1, arg2, token)
                                   .ConfigureAwait(false);

                return new WebResponse<TValue>(response, result);
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException(e);

                return await WebResponse<TValue>.Create(response, e, token)
                                                .ConfigureAwait(false);
            }
        }
    }


    public ValueTask<WebResponse<bool>>                 AsBool( CancellationToken         token )                                   => CreateResponse(AsBool,         token);
    public ValueTask<WebResponse<byte[]>>               AsBytes( CancellationToken        token )                                   => CreateResponse(AsBytes,        token);
    public ValueTask<WebResponse<JToken>>               AsJson( CancellationToken         token )                                   => CreateResponse(AsJson,         token);
    public ValueTask<WebResponse<TValue>>               AsJson<TValue>( CancellationToken token )                                   => CreateResponse(AsJson<TValue>, token);
    public ValueTask<WebResponse<LocalFile>>            AsFile( CancellationToken         token )                                   => CreateResponse(AsFile,         token);
    public ValueTask<WebResponse<LocalFile>>            AsFile( string                    fileNameHeader, CancellationToken token ) => CreateResponse(AsFile,         fileNameHeader, token);
    public ValueTask<WebResponse<LocalFile>>            AsFile( FileInfo                  path,           CancellationToken token ) => CreateResponse(AsFile,         path,           token);
    public ValueTask<WebResponse<LocalFile>>            AsFile( LocalFile                 file,           CancellationToken token ) => CreateResponse(AsFile,         file,           token);
    public ValueTask<WebResponse<LocalFile>>            AsFile( MimeType                  type,           CancellationToken token ) => CreateResponse(AsFile,         type,           token);
    public ValueTask<WebResponse<MemoryStream>>         AsStream( CancellationToken       token ) => CreateResponse(AsStream, token);
    public ValueTask<WebResponse<ReadOnlyMemory<byte>>> AsMemory( CancellationToken       token ) => CreateResponse(AsMemory, token);
    public ValueTask<WebResponse<string>>               AsString( CancellationToken       token ) => CreateResponse(AsString, token);


    public async ValueTask<ErrorOrResult> NoResponse( CancellationToken token )
    {
        await using ( this )
        {
            using HttpResponseMessage response = await SendAsync(token)
                                                    .ConfigureAwait(false);

            return response.IsSuccessStatusCode
                       ? true
                       : Errors.Create(response.StatusCode.ToStatus());
        }
    }


    public static async ValueTask<JToken> AsJson( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent content = response.Content;

        await using Stream stream = await content.ReadAsStreamAsync(token)
                                                 .ConfigureAwait(false);

        JToken? result = await stream.FromJson(token)
                                     .ConfigureAwait(false);

        return Validate.ThrowIfNull(result);
    }
    public static async ValueTask<TValue> AsJson<TValue>( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent content = response.Content;

        await using Stream stream = await content.ReadAsStreamAsync(token)
                                                 .ConfigureAwait(false);

        TValue result = await stream.FromJson<TValue>(token)
                                    .ConfigureAwait(false);

        return result;
    }
    public static async ValueTask<bool> AsBool( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string content = await AsString(response, token)
                            .ConfigureAwait(false);

        return bool.TryParse(content, out bool result) && result;
    }
    public static async ValueTask<Guid?> AsGuid( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string content = await AsString(response, token)
                            .ConfigureAwait(false);

        return Guid.TryParse(content, out Guid result)
                   ? result
                   : Guid.Empty;
    }
    public static async ValueTask<byte[]> AsBytes( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        await using MemoryStream stream = await AsStream(response, token)
                                             .ConfigureAwait(false);

        return stream.ToArray();
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        await using MemoryStream stream = await AsStream(response, token)
                                             .ConfigureAwait(false);

        await using FileStream fs = LocalFile.CreateTempFileAndOpen(out LocalFile file);
        await stream.CopyToAsync(fs, token).ConfigureAwait(false);

        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, string fileNameHeader, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        if ( response.Headers.Contains(fileNameHeader) )
        {
            MimeType mimeType = response.Headers.GetValues(fileNameHeader)
                                        .First()
                                        .ToMimeType();

            return await AsFile(response, mimeType, token)
                      .ConfigureAwait(false);
        }


        if ( response.Content.Headers.Contains(fileNameHeader) )
        {
            MimeType mimeType = response.Content.Headers.GetValues(fileNameHeader)
                                        .First()
                                        .ToMimeType();

            return await AsFile(response, mimeType, token)
                      .ConfigureAwait(false);
        }


        await using Stream stream = await AsStream(response, token)
                                       .ConfigureAwait(false);

        await using FileStream fs = LocalFile.CreateTempFileAndOpen(out LocalFile file);

        using ( telemetrySpan.SubSpan("WriteToFile") )
        {
            await stream.CopyToAsync(fs, token)
                        .ConfigureAwait(false);
        }

        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, FileInfo path, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        LocalFile           file          = new(path);
        return await AsFile(response, file, token).ConfigureAwait(false);
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, LocalFile file, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        await using MemoryStream stream = await AsStream(response, token)
                                             .ConfigureAwait(false);

        using ( telemetrySpan.SubSpan("WriteToFile") )
        {
            await stream.CopyToAsync(stream, token)
                        .ConfigureAwait(false);
        }

        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, MimeType type, CancellationToken token )
    {
        await using MemoryStream stream = await AsStream(response, token)
                                             .ConfigureAwait(false);

        await using FileStream fs = LocalFile.CreateTempFileAndOpen(type, out LocalFile file);
        await stream.CopyToAsync(fs, token).ConfigureAwait(false);
        return file;
    }
    public static async ValueTask<MemoryStream> AsStream( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent content = response.Content;

        await using Stream stream = await content.ReadAsStreamAsync(token)
                                                 .ConfigureAwait(false);

        MemoryStream buffer = new((int)stream.Length);

        using ( telemetrySpan.SubSpan("WriteToFile") )
        {
            await stream.CopyToAsync(buffer, token)
                        .ConfigureAwait(false);
        }

        buffer.Seek(0, SeekOrigin.Begin);
        return buffer;
    }
    public static async ValueTask<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response, CancellationToken token ) => await AsBytes(response, token).ConfigureAwait(false);
    public static async ValueTask<string> AsString( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();
        HttpContent content = response.Content;

        return await content.ReadAsStringAsync(token)
                            .ConfigureAwait(false);
    }


/*

    public async ValueTask<TValue> AsJson<TValue>( HttpResponseMessage response, JsonTypeInfo<TValue> info )
    {
        await using MemoryStream stream = await AsStream( response );

        TValue? result = await JsonSerializer.DeserializeAsync( stream, info, token );
        return result ?? throw new NullReferenceException( nameof(JsonSerializer.DeserializeAsync) );
    }

*/
}
