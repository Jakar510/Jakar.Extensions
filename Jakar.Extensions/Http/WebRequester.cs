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



[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public sealed partial class WebRequester( HttpClient client, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) : IDisposable
{
    public readonly   Encoding   Encoding = encoding ?? Encoding.Default;
    internal readonly HttpClient Client   = client;
    private readonly  IHostInfo  __host   = host;
    internal readonly ILogger?   Logger   = logger;


    public HttpRequestHeaders DefaultRequestHeaders => Client.DefaultRequestHeaders;
    public RetryPolicy?       Retries               { get;                   set; }
    public TimeSpan           Timeout               { get => Client.Timeout; set => Client.Timeout = value; }


    public void Dispose() => Client.Dispose();


    public static IServiceCollection AddSingleton( IServiceCollection collection )                                                                 => collection.AddSingleton(Create);
    public static IServiceCollection AddScoped( IServiceCollection    collection )                                                                 => collection.AddScoped(Create);
    public static WebRequester       Get( IServiceProvider            provider )                                                                   => provider.GetRequiredService<WebRequester>();
    public static WebRequester       Create( IServiceProvider         provider )                                                                   => Create(GetHttpClientFactory(provider), IHostInfo.Get(provider), GetLogger(provider));
    public static WebRequester       Create( IHttpClientFactory       factory, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => Create(factory.CreateClient(),         host,                    logger, encoding);
    public static WebRequester       Create( HttpClient               client,  IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => new(client, host, logger, encoding);
    private static ILogger GetLogger( IServiceProvider provider ) => provider.GetRequiredService<ILoggerFactory>()
                                                                             .CreateLogger(nameof(WebRequester));
    private static IHttpClientFactory GetHttpClientFactory( IServiceProvider provider ) => provider.GetRequiredService<IHttpClientFactory>();


    private Uri        CreateUrl( string  relativePath )                              => new(__host.HostInfo, relativePath);
    private WebHandler CreateHandler( Uri url, HttpMethod method )                    => new(this, new HttpRequestMessage(method, url));
    private WebHandler CreateHandler( Uri url, HttpMethod method, HttpContent value ) => new(this, new HttpRequestMessage(method, url) { Content = value });


    public WebHandler Delete<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Delete(relativePath, new JsonContent(value.ToJson(), Encoding));
    public WebHandler Delete<TValue>( string relativePath, TValue      value, JsonTypeInfo<TValue> info ) => Delete(relativePath, new JsonContent(value.ToJson(info), Encoding));
    public WebHandler Delete( string         relativePath, HttpContent value ) => Delete(CreateUrl(relativePath), value);
    public WebHandler Delete( Uri            url,          HttpContent value ) => CreateHandler(url, HttpMethod.Delete, value);
    public WebHandler Delete( Uri            url )                                               => CreateHandler(url, HttpMethod.Delete);
    public WebHandler Delete( string         relativePath )                                      => Delete(CreateUrl(relativePath));
    public WebHandler Delete( string         relativePath, byte[]                      value )   => Delete(relativePath, new ByteArrayContent(value));
    public WebHandler Delete( string         relativePath, in ReadOnlyMemory<byte>     value )   => Delete(relativePath, new ReadOnlyMemoryContent(value));
    public WebHandler Delete( string         relativePath, IDictionary<string, string> value )   => Delete(relativePath, new FormUrlEncodedContent(value));
    public WebHandler Delete( string         relativePath, Stream                      value )   => Delete(relativePath, new StreamContent(value));
    public WebHandler Delete( string         relativePath, MultipartContent            content ) => Delete(relativePath, (HttpContent)content);
    public WebHandler Delete( string         relativePath, string                      value )   => Delete(relativePath, new StringContent(value.ToJson(JakarExtensionsContext.Default.String), Encoding));


    public WebHandler Get( Uri    url )          => CreateHandler(url, HttpMethod.Get);
    public WebHandler Get( string relativePath ) => Get(CreateUrl(relativePath));


    public WebHandler Patch<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Patch(relativePath, new JsonContent(value.ToJson(), Encoding));
    public WebHandler Patch<TValue>( string relativePath, TValue                      value, JsonTypeInfo<TValue> info ) => Patch(relativePath, new JsonContent(value.ToJson(info), Encoding));
    public WebHandler Patch( Uri            url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Patch, value);
    public WebHandler Patch( string         relativePath, HttpContent                 value )   => Patch(CreateUrl(relativePath), value);
    public WebHandler Patch( string         relativePath, byte[]                      value )   => Patch(relativePath,            new ByteArrayContent(value));
    public WebHandler Patch( string         relativePath, in ReadOnlyMemory<byte>     value )   => Patch(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Patch( string         relativePath, IDictionary<string, string> value )   => Patch(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Patch( string         relativePath, Stream                      value )   => Patch(relativePath,            new StreamContent(value));
    public WebHandler Patch( string         relativePath, MultipartContent            content ) => Patch(relativePath,            (HttpContent)content);
    public WebHandler Patch( string         relativePath, string                      value )   => Patch(relativePath,            new StringContent(value.ToJson(JakarExtensionsContext.Default.String), Encoding));


    public WebHandler Post<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Post(relativePath, new JsonContent(value.ToJson(), Encoding));
    public WebHandler Post<TValue>( string relativePath, TValue                      value, JsonTypeInfo<TValue> info ) => Post(relativePath, new JsonContent(value.ToJson(info), Encoding));
    public WebHandler Post( Uri            url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Post, value);
    public WebHandler Post( string         relativePath, HttpContent                 value )   => Post(CreateUrl(relativePath), value);
    public WebHandler Post( string         relativePath, byte[]                      value )   => Post(relativePath,            new ByteArrayContent(value));
    public WebHandler Post( string         relativePath, in ReadOnlyMemory<byte>     value )   => Post(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Post( string         relativePath, IDictionary<string, string> value )   => Post(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Post( string         relativePath, Stream                      value )   => Post(relativePath,            new StreamContent(value));
    public WebHandler Post( string         relativePath, MultipartContent            content ) => Post(relativePath,            (HttpContent)content);
    public WebHandler Post( string         relativePath, string                      value )   => Post(relativePath,            new StringContent(value.ToJson(JakarExtensionsContext.Default.String), Encoding));


    public WebHandler Put<TValue>( string relativePath, TValue value )
        where TValue : IJsonModel<TValue> => Put(relativePath, new JsonContent(value.ToJson(), Encoding));
    public WebHandler Put<TValue>( string relativePath, TValue                      value, JsonTypeInfo<TValue> info ) => Put(relativePath, new JsonContent(value.ToJson(info), Encoding));
    public WebHandler Put( Uri            url,          HttpContent                 value )   => CreateHandler(url, HttpMethod.Put, value);
    public WebHandler Put( string         relativePath, HttpContent                 value )   => Put(CreateUrl(relativePath), value);
    public WebHandler Put( string         relativePath, byte[]                      value )   => Put(relativePath,            new ByteArrayContent(value));
    public WebHandler Put( string         relativePath, in ReadOnlyMemory<byte>     value )   => Put(relativePath,            new ReadOnlyMemoryContent(value));
    public WebHandler Put( string         relativePath, IDictionary<string, string> value )   => Put(relativePath,            new FormUrlEncodedContent(value));
    public WebHandler Put( string         relativePath, Stream                      value )   => Put(relativePath,            new StreamContent(value));
    public WebHandler Put( string         relativePath, MultipartContent            content ) => Put(relativePath,            (HttpContent)content);
    public WebHandler Put( string         relativePath, string                      value )   => Put(relativePath,            new StringContent(value.ToJson(JakarExtensionsContext.Default.String), Encoding));
}
