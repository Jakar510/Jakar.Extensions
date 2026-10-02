// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:36 AM

namespace Jakar.Extensions;


/// <summary>
///     One request built by <see cref="WebRequester"/>. Every <c>As*</c> / <see cref="CreateResponse{TValue}"/> call sends it (retrying per <see cref="WebRequester.Retries"/>), returns a
///     <see cref="WebResponse{TValue}"/> and disposes the request, so a handler is used once.
/// </summary>
/// <remarks>
///     Network failures and timeouts don't throw: they come back as an unsuccessful <see cref="WebResponse{TValue}"/> carrying the exception.
///     Cancellation requested through the caller's token still throws <see cref="OperationCanceledException"/>.
/// </remarks>
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public readonly struct WebHandler( WebRequester requester, HttpRequestMessage request ) : IAsyncDisposable, IDisposable
{
    public const           string  NO_RESPONSE = "NO RESPONSE";
    public static readonly EventId EventId     = new(69420, nameof(SendAsync));

    private static readonly Action<ILogger, int, string?, Exception?> __logResponse = LoggerMessage.Define<int, string?>(LogLevel.Debug, EventId, "Response StatusCode: {StatusCode} for {Uri}");
    private static readonly Action<ILogger, int, string?, Exception?> __logRetry    = LoggerMessage.Define<int, string?>(LogLevel.Information, EventId, "Retry {Retry} for {Uri}");


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


    /// <summary> Sends the request once, without retries; network failures throw. </summary>
    public async ValueTask<HttpResponseMessage> SendAsync( CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        HttpResponseMessage response = await Client.SendAsync(request, token).ConfigureAwait(false);
        LogResponse(response);
        return response;
    }


    public ValueTask<WebResponse<TValue>> CreateResponse<TValue>( Func<HttpResponseMessage, CancellationToken, ValueTask<TValue>> func, CancellationToken token ) =>
        Execute(func, static ( response, parse, token ) => parse(response, token), HttpCompletionOption.ResponseContentRead, token);
    public ValueTask<WebResponse<TValue>> CreateResponse<TValue, TArg>( Func<HttpResponseMessage, TArg, CancellationToken, ValueTask<TValue>> func, TArg arg, CancellationToken token ) =>
        Execute(( func, arg ), static ( response, state, token ) => state.func(response, state.arg, token), HttpCompletionOption.ResponseContentRead, token);
    public ValueTask<WebResponse<TValue>> CreateResponse<TValue, TArg, TArg2>( Func<HttpResponseMessage, TArg, TArg2, CancellationToken, ValueTask<TValue>> func, TArg arg1, TArg2 arg2, CancellationToken token ) =>
        Execute(( func, arg1, arg2 ), static ( response, state, token ) => state.func(response, state.arg1, state.arg2, token), HttpCompletionOption.ResponseContentRead, token);


    public ValueTask<WebResponse<bool>>   AsBool( CancellationToken         token ) => CreateResponse(AsBool,         token);
    public ValueTask<WebResponse<byte[]>> AsBytes( CancellationToken        token ) => CreateResponse(AsBytes,        token);
    public ValueTask<WebResponse<JToken>> AsJson( CancellationToken         token ) => CreateResponse(AsJson,         token);
    public ValueTask<WebResponse<TValue>> AsJson<TValue>( CancellationToken token ) => CreateResponse(AsJson<TValue>, token);

    // Files are streamed straight from the socket to disk (headers-read), instead of buffering the whole body in memory first.
    public ValueTask<WebResponse<LocalFile>> AsFile( CancellationToken token ) =>
        Execute(0, static ( response, _, token ) => AsFile(response, token), HttpCompletionOption.ResponseHeadersRead, token);
    public ValueTask<WebResponse<LocalFile>> AsFile( string fileNameHeader, CancellationToken token ) =>
        Execute(fileNameHeader, static ( response, header, token ) => AsFile(response, header, token), HttpCompletionOption.ResponseHeadersRead, token);
    public ValueTask<WebResponse<LocalFile>> AsFile( FileInfo path, CancellationToken token ) =>
        Execute(path, static ( response, info, token ) => AsFile(response, info, token), HttpCompletionOption.ResponseHeadersRead, token);
    public ValueTask<WebResponse<LocalFile>> AsFile( LocalFile file, CancellationToken token ) =>
        Execute(file, static ( response, target, token ) => AsFile(response, target, token), HttpCompletionOption.ResponseHeadersRead, token);
    public ValueTask<WebResponse<LocalFile>> AsFile( MimeType type, CancellationToken token ) =>
        Execute(type, static ( response, mime, token ) => AsFile(response, mime, token), HttpCompletionOption.ResponseHeadersRead, token);

    public ValueTask<WebResponse<MemoryStream>>         AsStream( CancellationToken token ) => CreateResponse(AsStream, token);
    public ValueTask<WebResponse<ReadOnlyMemory<byte>>> AsMemory( CancellationToken token ) => CreateResponse(AsMemory, token);
    public ValueTask<WebResponse<string>>               AsString( CancellationToken token ) => CreateResponse(AsString, token);


    /// <summary> Sends the request (with retries) and ignores the body. Failures, including network failures, are returned as errors. </summary>
    public async ValueTask<ErrorOrResult> NoResponse( CancellationToken token )
    {
        WebResponse<bool> response = await Execute(0, static ( _, _, _ ) => ValueTask.FromResult(true), HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);

        return response.IsSuccessStatusCode
                   ? true
                   : response.GetError();
    }


    // ─── Send / retry ────────────────────────────────────────────────────────

    /// <summary> Sends the request, retrying per <see cref="RetryPolicy"/> (see <see cref="WebRequester.RetryPolicy"/> for the rules), and parses the final response with <paramref name="parse"/>. </summary>
    private async ValueTask<WebResponse<TValue>> Execute<TValue, TState>( TState state, Func<HttpResponseMessage, TState, CancellationToken, ValueTask<TValue>> parse, HttpCompletionOption completion, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        RetryPolicy        policy     = RetryPolicy ?? WebRequester.RetryPolicy.None;
        bool               replayable = policy.AllowRetries && IsReplayable(request.Content);
        HttpRequestMessage message    = request;
        int                retry      = 0;

        await using ( this )
        {
            while ( true )
            {
                bool     canRetry = replayable && retry < policy.MaxRetires;
                TimeSpan delay;

                HttpResponseMessage? response = null;

                try
                {
                    try { response = await Client.SendAsync(message, completion, token).ConfigureAwait(false); }
                    catch ( Exception e ) when ( WebRequester.RetryPolicy.IsTransient(e, token) )
                    {
                        telemetrySpan.AddException(e);
                        if ( !canRetry ) { return new WebResponse<TValue>(message, e); }

                        delay = policy.GetDelay(retry + 1);
                        goto Retry;
                    }

                    LogResponse(response);

                    if ( canRetry && WebRequester.RetryPolicy.IsTransient(response.StatusCode) && TryGetRetryDelay(response, policy, retry + 1, out delay) ) { goto Retry; }

                    if ( !response.IsSuccessStatusCode ) { return await WebResponse<TValue>.Create(response, token).ConfigureAwait(false); }

                    try
                    {
                        TValue result = await parse(response, state, token).ConfigureAwait(false);
                        return new WebResponse<TValue>(response, result);
                    }
                    catch ( Exception e ) when ( WebRequester.RetryPolicy.IsTransient(e, token) )
                    {
                        // The connection failed while reading the body: don't try to read it again for the error message.
                        telemetrySpan.AddException(e);
                        if ( !canRetry ) { return new WebResponse<TValue>(response, e, e.Message); }

                        delay = policy.GetDelay(retry + 1);
                    }
                }
                finally { response?.Dispose(); }

            Retry:
                retry++;
                if ( Logger is not null ) { __logRetry(Logger, retry, request.RequestUri?.OriginalString, null); }

                using ( telemetrySpan.SubSpan("RetryDelay") )
                {
                    if ( delay > TimeSpan.Zero ) { await Task.Delay(delay, token).ConfigureAwait(false); }
                }

                // An HttpRequestMessage can only be sent once; the copy shares the (replayable) content, which the original disposes at the end.
                message = CopyForRetry(request);
            }
        }
    }


    /// <summary> Content that can be sent more than once: none, bytes (incl. string, JSON and form content), memory, or multipart made only of those. </summary>
    private static bool IsReplayable( HttpContent? content ) => content switch
                                                                {
                                                                    null                                      => true,
                                                                    ByteArrayContent or ReadOnlyMemoryContent => true,
                                                                    MultipartContent multipart                => multipart.All(IsReplayable),
                                                                    _                                         => false
                                                                };


    private static HttpRequestMessage CopyForRetry( HttpRequestMessage source )
    {
        HttpRequestMessage copy = new(source.Method, source.RequestUri)
                                  {
                                      Content       = source.Content,
                                      Version       = source.Version,
                                      VersionPolicy = source.VersionPolicy
                                  };

        foreach ( KeyValuePair<string, IEnumerable<string>> header in source.Headers ) { copy.Headers.TryAddWithoutValidation(header.Key, header.Value); }

        IDictionary<string, object?> options = copy.Options;
        foreach ( KeyValuePair<string, object?> option in source.Options ) { options[option.Key] = option.Value; }

        return copy;
    }


    /// <summary> The back-off for this retry, or the server's <c>Retry-After</c> when that is longer. Returns <see langword="false"/> (don't retry) when <c>Retry-After</c> exceeds <see cref="WebRequester.RetryPolicy.MaxRetryAfter"/>. </summary>
    private static bool TryGetRetryDelay( HttpResponseMessage response, RetryPolicy policy, int retry, out TimeSpan delay )
    {
        delay = policy.GetDelay(retry);

        RetryConditionHeaderValue? retryAfter = response.Headers.RetryAfter;
        if ( retryAfter is null ) { return true; }

        TimeSpan? requested = retryAfter.Delta ?? retryAfter.Date - DateTimeOffset.UtcNow;
        if ( requested is not { } wait ) { return true; }

        if ( wait > WebRequester.RetryPolicy.MaxRetryAfter ) { return false; }

        if ( wait > delay ) { delay = wait; }

        return true;
    }


    private void LogResponse( HttpResponseMessage response )
    {
        if ( Logger is not null ) { __logResponse(Logger, (int)response.StatusCode, request.RequestUri?.OriginalString, null); }
    }


    // ─── Body readers ────────────────────────────────────────────────────────

    public static async ValueTask<JToken> AsJson( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        await using Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        JToken result = await stream.FromJson(token).ConfigureAwait(false);
        return ThrowIfNull(result);
    }
    /// <summary> Deserializes the body straight into <typeparamref name="TValue"/> with <see cref="Json.Settings"/> (no intermediate <see cref="JToken"/>). </summary>
    public static async ValueTask<TValue> AsJson<TValue>( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        // Buffer first (a no-op for the default completion option) so the synchronous Newtonsoft reader never blocks on the network.
        await response.Content.LoadIntoBufferAsync(token).ConfigureAwait(false);
        await using Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        using StreamReader   textReader = new(stream, leaveOpen: true);
        using JsonTextReader reader     = new(textReader) { CloseInput = false };

        return ThrowIfNull(JsonSerializer.Create(Json.Settings).Deserialize<TValue>(reader));
    }
    public static async ValueTask<bool> AsBool( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string content = await AsString(response, token).ConfigureAwait(false);

        return bool.TryParse(content, out bool result) && result;
    }
    public static async ValueTask<Guid?> AsGuid( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string content = await AsString(response, token).ConfigureAwait(false);

        return Guid.TryParse(content, out Guid result)
                   ? result
                   : Guid.Empty;
    }
    /// <summary> The body as one array (a single copy of the buffered content). </summary>
    public static async ValueTask<byte[]> AsBytes( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(token).ConfigureAwait(false);
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        await using FileStream fs = LocalFile.CreateTempFileAndOpen(out LocalFile file);
        await CopyBody(response, fs, token).ConfigureAwait(false);
        return file;
    }
    /// <summary> Saves the body to a temp file whose extension comes from the <see cref="MimeType"/> named by header <paramref name="fileNameHeader"/> (response or content headers), if present. </summary>
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, string fileNameHeader, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        if ( response.Headers.TryGetValues(fileNameHeader, out IEnumerable<string>? values) || response.Content.Headers.TryGetValues(fileNameHeader, out values) )
        {
            string? value = values.FirstOrDefault();
            if ( value is not null ) { return await AsFile(response, value.ToMimeType(), token).ConfigureAwait(false); }
        }

        return await AsFile(response, token).ConfigureAwait(false);
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, FileInfo path, CancellationToken token ) => await AsFile(response, new LocalFile(path), token).ConfigureAwait(false);
    /// <summary> Saves the body to <paramref name="file"/>, replacing its contents. </summary>
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, LocalFile file, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        await using FileStream fs = file.OpenWrite(FileMode.Create);
        await CopyBody(response, fs, token).ConfigureAwait(false);
        return file;
    }
    public static async ValueTask<LocalFile> AsFile( HttpResponseMessage response, MimeType type, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        await using FileStream fs = LocalFile.CreateTempFileAndOpen(type, out LocalFile file);
        await CopyBody(response, fs, token).ConfigureAwait(false);
        return file;
    }
    /// <summary> The body as a seekable <see cref="MemoryStream"/> over a single copy of the content (fixed size: it can be overwritten but not grown). </summary>
    public static async ValueTask<MemoryStream> AsStream( HttpResponseMessage response, CancellationToken token )
    {
        byte[] bytes = await AsBytes(response, token).ConfigureAwait(false);
        return new MemoryStream(bytes, 0, bytes.Length, true, true);
    }
    public static async ValueTask<ReadOnlyMemory<byte>> AsMemory( HttpResponseMessage response, CancellationToken token ) => await AsBytes(response, token).ConfigureAwait(false);
    public static async ValueTask<string> AsString( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
    }


    private static async ValueTask CopyBody( HttpResponseMessage response, Stream destination, CancellationToken token )
    {
        await using Stream body = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
        await body.CopyToAsync(destination, token).ConfigureAwait(false);
    }
}
