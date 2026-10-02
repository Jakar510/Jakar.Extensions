// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:35 AM

using Microsoft.Extensions.DependencyInjection;



namespace Jakar.Extensions;


/*
public class TelemetryHttpClientHandler : HttpClientHandler
{
    private const string TRACE_PARENT = "trace_parent";
    private const string TRACE_STATE  = "trace_state";
    public TelemetryHttpClientHandler() { }

    protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
    {
        Dictionary<string, string> Headers_Params = new Dictionary<string, string>();

        List<string> headersList = HeaderList.Instance.GetHttpHeaders().ToList();

        foreach ( string h in headersList )
        {
            if ( request.Headers.Contains( h ) )
            {
                if ( request.Headers.TryGetValues( h, out IEnumerable<string>? values ) ) { Headers_Params.Add( h, values.First() ); }
            }
        }

        TraceContext traceContext = NRAndroidAgent.NoticeDistributedTrace( null );
        request.Headers.Add( traceContext.TracePayload.HeaderName, traceContext.TracePayload.HeaderValue );
        request.Headers.Add( TRACE_PARENT,                         "00-"               + traceContext.TraceId + "-"                    + traceContext.ParentId + "-00" );
        request.Headers.Add( TRACE_STATE,                          traceContext.Vendor + "=0-2-"              + traceContext.AccountId + "-"                   + traceContext.ApplicationId + "-" + traceContext.ParentId + "----" + DateTimeOffset.Now.ToUnixTimeMilliseconds() );
        using StopWatch     startTime           = StopWatch.Start();
        HttpResponseMessage httpResponseMessage = await base.SendAsync( request, cancellationToken );
        TimeSpan            elapsed             = startTime.Elapsed;

        try
        {
            HttpResponseMessage httpResponseMessage = await base.SendAsync( request, cancellationToken );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            throw;
        }

        NRAndroidAgent.NoticeHttpTransaction( request.RequestUri.ToString(),
                                              request.Method.ToString(),
                                              (int)httpResponseMessage.StatusCode,
                                              startTime,
                                              elapsed,
                                              0,
                                              httpResponseMessage.ToString().Length,
                                              "",
                                              Headers_Params,
                                              null,
                                              traceContext.AsTraceAttributes() );
    }
}
*/



/// <remarks>
///     A requester created from DI (<see cref="Create(IServiceProvider, string)"/>) reloads when its <see cref="WebRequesterOptions"/> change (e.g. <c>appsettings.json</c> edited with
///     <c>reloadOnChange</c>): a new client is built and swapped in atomically, so each request uses one consistent configuration. Requests already running finish on the previous
///     client, which is not disposed (that would cancel them); its idle connections close after the idle timeout and it is collected once unreferenced.
/// </remarks>
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public sealed partial class WebRequester : IAsyncDisposable, IDisposable
{
    private State        __state;
    private IDisposable? __subscription;


    public   Encoding   Encoding => Volatile.Read(ref __state).Encoding;
    internal HttpClient Client   => Volatile.Read(ref __state).Client;
    internal IHostInfo  Host     => Volatile.Read(ref __state).Host;
    internal ILogger?   Logger   => Volatile.Read(ref __state).Logger;


    /// <summary> Headers of the current client. Changes made here are lost when the options reload. </summary>
    public HttpRequestHeaders DefaultRequestHeaders => Client.DefaultRequestHeaders;

    /// <summary> Retry policy. A value set here is replaced when the options reload. </summary>
    public RetryPolicy? Retries { get; set; }

    public TimeSpan Timeout { get => Client.Timeout; set => Client.Timeout = value; }


    /// <summary> Raised after the configuration was reloaded from changed <see cref="WebRequesterOptions"/>. </summary>
    public event Action<WebRequester>? Reloaded;


    public WebRequester( HttpClient client, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => __state = new State(client, host, logger, encoding ?? Encoding.Default);


    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
    public void Dispose()
    {
        Interlocked.Exchange(ref __subscription, null)?.Dispose();
        Client.Dispose();
    }


    /// <summary> Rebuilds the client from <paramref name="options"/> and swaps it in. On failure (e.g. invalid options) the error is logged and the current configuration is kept. </summary>
    private void Reload( IServiceProvider provider, WebRequesterOptions options )
    {
        try
        {
            WebRequester fresh = Build(provider, options);
            Volatile.Write(ref __state, fresh.__state);
            Retries = fresh.Retries;
            Reloaded?.Invoke(this);
        }
        catch ( Exception e ) { Logger?.LogError(e, "Reloading {Options} failed; keeping the previous configuration", nameof(WebRequesterOptions)); }
    }


    public static IServiceCollection AddSingleton( IServiceCollection collection )                                                                 => collection.AddSingleton(Create);
    public static IServiceCollection AddScoped( IServiceCollection    collection )                                                                 => collection.AddScoped(Create);
    public static WebRequester       Get( IServiceProvider            provider )                                                                   => provider.GetRequiredService<WebRequester>();
    public static WebRequester       Create( IHttpClientFactory       factory, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => Create(factory.CreateClient(nameof(WebRequester)), host, logger, encoding);
    public static WebRequester       Create( HttpClient               client,  IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => new(client, host, logger, encoding);
    /// <summary>
    ///     Builds a requester from DI: the default (or, for the overload taking a name, the named) <see cref="WebRequesterOptions"/> (see <see cref="WebRequesterServiceCollectionExtensions.AddWebRequester(IServiceCollection, Action{WebRequesterOptions})"/>),
    ///     the registered <see cref="ILoggerFactory"/> (if any), and <see cref="WebRequesterOptions.BaseAddress"/> or else the registered <see cref="IHostInfo"/>.
    ///     Requests are not retried unless <see cref="WebRequesterOptions.Retry"/> is set.
    /// </summary>
    /// <exception cref="OptionsValidationException"> The options are invalid. </exception>
    public static WebRequester       Create( IServiceProvider         provider ) => Create(provider, Options.DefaultName);
    /// <remarks> When an <see cref="IOptionsMonitor{TOptions}"/> is registered, the requester reloads whenever the named options change (see <see cref="WebRequester"/>). </remarks>
    public static WebRequester Create( IServiceProvider provider, string name )
    {
        IOptionsMonitor<WebRequesterOptions>? monitor   = provider.GetService<IOptionsMonitor<WebRequesterOptions>>();
        WebRequester                          requester = Build(provider, monitor?.Get(name) ?? WebRequesterOptions.Default);

        if ( monitor is not null ) { requester.__subscription = monitor.OnChange(listener); }

        return requester;

        void listener( WebRequesterOptions options, string? changed )
        {
            if ( string.Equals(changed ?? Options.DefaultName, name, StringComparison.Ordinal) ) { requester.Reload(provider, options); }
        }
    }
    private static WebRequester Build( IServiceProvider provider, WebRequesterOptions options )
    {
        IHostInfo host = options.BaseAddress is { } address
                             ? new Builder.HostHolder(address)
                             : provider.GetRequiredService<IHostInfo>();

        using Builder builder = new(host);
        if ( provider.GetService<ILoggerFactory>() is { } factory ) { builder.With_Logger(factory); }

        return builder.Apply(options).Build(); // the requester's client keeps the shared handler alive after the builder is disposed
    }


    /// <summary>
    ///     Serializes <paramref name="value"/> as compact JSON (metadata from <see cref="Json.GetTypeInfo{T}"/>) straight to bytes in
    ///     <paramref name="encoding"/> (default <see cref="Encoding.Default"/>, UTF-8). The body is a byte array, so it can be re-sent by retries.
    ///     Content-Type: <c>application/json; charset={encoding.WebName}</c>.
    /// </summary>
    /// <remarks>
    ///     No byte order mark is ever sent (RFC 8259 §8.1). Non-ASCII text is written as-is (<see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping"/>, as Newtonsoft did):
    ///     the body is never embedded in HTML, and the declared charset carries it.
    /// </remarks>
    public static ByteArrayContent CreateJsonContent<TValue>( TValue value, Encoding? encoding = null ) => CreateJsonContent(value, Json.GetTypeInfo<TValue>(), encoding);

    /// <inheritdoc cref="CreateJsonContent{TValue}(TValue, Encoding)"/>
    public static ByteArrayContent CreateJsonContent<TValue>( TValue value, JsonTypeInfo<TValue> info, Encoding? encoding = null ) =>
        CreateContent(info.Options, ( value, info ), static ( writer, state ) => JsonSerializer.Serialize(writer, state.value, state.info), encoding);

    /// <summary> A JSON array body, written element by element with only the element's metadata (no collection type needs registering). </summary>
    public static ByteArrayContent CreateJsonArrayContent<TValue>( IEnumerable<TValue> values, JsonTypeInfo<TValue> info, Encoding? encoding = null ) =>
        CreateContent(info.Options, ( values, info ), static ( writer, state ) => JsonModel.WriteArray(writer, state.values, state.info), encoding);

    /// <summary> Serializes <paramref name="value"/> as its <b> runtime </b> type (e.g. a <see cref="BaseClass"/>-typed argument holding a <c> UserModel </c>), resolved with <see cref="Json.GetTypeInfo(Type)"/>. </summary>
    public static ByteArrayContent CreateJsonContentOfRuntimeType( object value, Encoding? encoding = null )
    {
        ArgumentNullException.ThrowIfNull(value);
        JsonTypeInfo info = Json.GetTypeInfo(value.GetType());
        return CreateContent(info.Options, ( value, info ), static ( writer, state ) => JsonSerializer.Serialize(writer, state.value, state.info), encoding);
    }

    /// <summary> A JSON array of <paramref name="values"/>, each serialized as its <b> runtime </b> type. </summary>
    public static ByteArrayContent CreateJsonArrayContentOfRuntimeType( IEnumerable<object?> values, Encoding? encoding = null ) =>
        CreateContent(Json.Options,
                      values,
                      static ( writer, values ) =>
                      {
                          writer.WriteStartArray();

                          foreach ( object? value in values )
                          {
                              if ( value is null ) { writer.WriteNullValue(); }
                              else { JsonSerializer.Serialize(writer, value, Json.GetTypeInfo(value.GetType())); }
                          }

                          writer.WriteEndArray();
                      },
                      encoding);


    private static ByteArrayContent CreateContent<TState>( JsonSerializerOptions options, TState state, Action<Utf8JsonWriter, TState> write, Encoding? encoding )
    {
        encoding ??= Encoding.Default;

        // System.Text.Json writes UTF-8 natively; always compact, whatever the context's WriteIndented.
        JsonWriterOptions writerOptions = options.GetWriterOptions(false);
        writerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

        ArrayBufferWriter<byte> buffer = new(256);
        using ( Utf8JsonWriter writer = new(buffer, writerOptions) ) { write(writer, state); }

        byte[] body = encoding.CodePage == Encoding.UTF8.CodePage
                          ? buffer.WrittenSpan.ToArray()
                          : encoding.GetBytes(Encoding.UTF8.GetString(buffer.WrittenSpan)); // GetBytes never writes a preamble

        ByteArrayContent content = new(body);
        content.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeNames.Application.JSON) { CharSet = encoding.WebName };
        return content;
    }


    private Uri        CreateUrl( string  relativePath )                              => new(Host.HostInfo, relativePath);
    private WebHandler CreateHandler( Uri url, HttpMethod method )                    => new(this, new HttpRequestMessage(method, url));
    private WebHandler CreateHandler( Uri url, HttpMethod method, HttpContent value ) => new(this, new HttpRequestMessage(method, url) { Content = value });


    public WebHandler Delete( string relativePath )                                      => Delete(CreateUrl(relativePath));
    public WebHandler Delete( string relativePath, byte[]                      value )   => Delete(relativePath,            new ByteArrayContent(value));
    public WebHandler Delete( string relativePath, in ReadOnlyMemory<byte>     value )   => Delete(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Delete( string relativePath, IDictionary<string, string> value )   => Delete(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Delete( string relativePath, Stream                      value )   => Delete(relativePath,            new StreamContent(value));
    public WebHandler Delete( string relativePath, MultipartFormDataContent    content ) => Delete(relativePath,            (HttpContent)content);
    public WebHandler Delete( string relativePath, MultipartContent            content ) => Delete(relativePath,            (HttpContent)content);
    public WebHandler Delete( string relativePath, string                      value )   => Delete(relativePath,            new StringContent(value, Encoding));
    public WebHandler Delete( string relativePath, BaseClass                   value )   => Delete(relativePath,            CreateJsonContentOfRuntimeType(value, Encoding));
    public WebHandler Delete( string relativePath, IEnumerable<BaseClass>      value )   => Delete(relativePath,            CreateJsonArrayContentOfRuntimeType(value, Encoding));
    public WebHandler Delete( string relativePath, HttpContent                 value )   => Delete(CreateUrl(relativePath), value);
    public WebHandler Delete( Uri    url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Delete, value);
    public WebHandler Delete( Uri    url ) => CreateHandler(url, HttpMethod.Delete);
    public WebHandler Delete<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Delete(relativePath, CreateJsonContent(value, TValue.JsonTypeInfo, Encoding));
    public WebHandler Delete<TValue>( string relativePath, IEnumerable<TValue> value )
        where TValue : IJsonModel<TValue> => Delete(relativePath, CreateJsonArrayContent(value, TValue.JsonTypeInfo, Encoding));


    public WebHandler Get( Uri    url )          => CreateHandler(url, HttpMethod.Get);
    public WebHandler Get( string relativePath ) => Get(CreateUrl(relativePath));


    public WebHandler Patch( string relativePath, byte[]                      value )   => Patch(relativePath,            new ByteArrayContent(value));
    public WebHandler Patch( string relativePath, in ReadOnlyMemory<byte>     value )   => Patch(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Patch( string relativePath, IDictionary<string, string> value )   => Patch(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Patch( string relativePath, Stream                      value )   => Patch(relativePath,            new StreamContent(value));
    public WebHandler Patch( string relativePath, MultipartFormDataContent    content ) => Patch(relativePath,            (HttpContent)content);
    public WebHandler Patch( string relativePath, MultipartContent            content ) => Patch(relativePath,            (HttpContent)content);
    public WebHandler Patch( string relativePath, string                      value )   => Patch(relativePath,            new StringContent(value, Encoding));
    public WebHandler Patch( string relativePath, BaseClass                   value )   => Patch(relativePath,            CreateJsonContentOfRuntimeType(value, Encoding));
    public WebHandler Patch( string relativePath, IEnumerable<BaseClass>      value )   => Patch(relativePath,            CreateJsonArrayContentOfRuntimeType(value, Encoding));
    public WebHandler Patch( string relativePath, HttpContent                 value )   => Patch(CreateUrl(relativePath), value);
    public WebHandler Patch( Uri    url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Patch, value);
    public WebHandler Patch( Uri    url ) => CreateHandler(url, HttpMethod.Patch);
    public WebHandler Patch<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Patch(relativePath, CreateJsonContent(value, TValue.JsonTypeInfo, Encoding));
    public WebHandler Patch<TValue>( string relativePath, IEnumerable<TValue> value )
        where TValue : IJsonModel<TValue> => Patch(relativePath, CreateJsonArrayContent(value, TValue.JsonTypeInfo, Encoding));


    public WebHandler Post( string relativePath, byte[]                      value )   => Post(relativePath,            new ByteArrayContent(value));
    public WebHandler Post( string relativePath, in ReadOnlyMemory<byte>     value )   => Post(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Post( string relativePath, IDictionary<string, string> value )   => Post(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Post( string relativePath, Stream                      value )   => Post(relativePath,            new StreamContent(value));
    public WebHandler Post( string relativePath, MultipartFormDataContent    content ) => Post(relativePath,            (HttpContent)content);
    public WebHandler Post( string relativePath, MultipartContent            content ) => Post(relativePath,            (HttpContent)content);
    public WebHandler Post( string relativePath, string                      value )   => Post(relativePath,            new StringContent(value, Encoding));
    public WebHandler Post( string relativePath, BaseClass                   value )   => Post(relativePath,            CreateJsonContentOfRuntimeType(value, Encoding));
    public WebHandler Post( string relativePath, IEnumerable<BaseClass>      value )   => Post(relativePath,            CreateJsonArrayContentOfRuntimeType(value, Encoding));
    public WebHandler Post( string relativePath, HttpContent                 value )   => Post(CreateUrl(relativePath), value);
    public WebHandler Post( Uri    url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Post, value);
    public WebHandler Post( Uri    url ) => CreateHandler(url, HttpMethod.Post);
    public WebHandler Post<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Post(relativePath, CreateJsonContent(value, TValue.JsonTypeInfo, Encoding));
    public WebHandler Post<TValue>( string relativePath, IEnumerable<TValue> value )
        where TValue : IJsonModel<TValue> => Post(relativePath, CreateJsonArrayContent(value, TValue.JsonTypeInfo, Encoding));


    public WebHandler Put( string relativePath, byte[]                      value )   => Put(relativePath,            new ByteArrayContent(value));
    public WebHandler Put( string relativePath, in ReadOnlyMemory<byte>     value )   => Put(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Put( string relativePath, IDictionary<string, string> value )   => Put(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Put( string relativePath, Stream                      value )   => Put(relativePath,            new StreamContent(value));
    public WebHandler Put( string relativePath, MultipartFormDataContent    content ) => Put(relativePath,            (HttpContent)content);
    public WebHandler Put( string relativePath, MultipartContent            content ) => Put(relativePath,            (HttpContent)content);
    public WebHandler Put( string relativePath, string                      value )   => Put(relativePath,            new StringContent(value, Encoding));
    public WebHandler Put( string relativePath, BaseClass                   value )   => Put(relativePath,            CreateJsonContentOfRuntimeType(value, Encoding));
    public WebHandler Put( string relativePath, IEnumerable<BaseClass>      value )   => Put(relativePath,            CreateJsonArrayContentOfRuntimeType(value, Encoding));
    public WebHandler Put( string relativePath, HttpContent                 value )   => Put(CreateUrl(relativePath), value);
    public WebHandler Put( Uri    url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Put, value);
    public WebHandler Put( Uri    url ) => CreateHandler(url, HttpMethod.Put);
    public WebHandler Put<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Put(relativePath, CreateJsonContent(value, TValue.JsonTypeInfo, Encoding));
    public WebHandler Put<TValue>( string relativePath, IEnumerable<TValue> value )
        where TValue : IJsonModel<TValue> => Put(relativePath, CreateJsonArrayContent(value, TValue.JsonTypeInfo, Encoding));



    /// <summary> The parts of the configuration that are replaced together on reload. </summary>
    private sealed class State( HttpClient client, IHostInfo host, ILogger? logger, Encoding encoding )
    {
        public readonly HttpClient Client   = client;
        public readonly IHostInfo  Host     = host;
        public readonly ILogger?   Logger   = logger;
        public readonly Encoding   Encoding = encoding;
    }
}
