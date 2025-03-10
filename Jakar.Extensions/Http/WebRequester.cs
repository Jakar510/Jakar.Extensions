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



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public sealed partial class WebRequester( HttpClient client, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) : IDisposable
{
    private readonly HttpClient   _client  = client;
    private readonly IHostInfo    _host    = host;
    private readonly ILogger?     _logger  = logger;
    public readonly  Encoding     Encoding = encoding = Encoding.Default;
    private          RetryPolicy? _retryPolicy;


    public RetryPolicy?       Retries               { get => _retryPolicy; set => _retryPolicy = value; }
    public HttpRequestHeaders DefaultRequestHeaders { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _client.DefaultRequestHeaders; }
    public TimeSpan           Timeout               { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _client.Timeout; set => _client.Timeout = value; }


    public static  IServiceCollection AddSingleton( IServiceCollection       collection )                                                                 => collection.AddSingleton( Create );
    public static  IServiceCollection AddScoped( IServiceCollection          collection )                                                                 => collection.AddScoped( Create );
    public static  WebRequester       Get( IServiceProvider                  provider )                                                                   => provider.GetRequiredService<WebRequester>();
    public static  WebRequester       Create( IServiceProvider               provider )                                                                   => Create( GetHttpClientFactory( provider ), IHostInfo.Get( provider ), GetLogger( provider ) );
    public static  WebRequester       Create( IHttpClientFactory             factory, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => Create( factory.CreateClient(),           host,                      logger, encoding );
    public static  WebRequester       Create( HttpClient                     client,  IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => new(client, host, logger, encoding);
    private static ILogger            GetLogger( IServiceProvider            provider ) => provider.GetRequiredService<ILoggerFactory>().CreateLogger( nameof(WebRequester) );
    private static IHttpClientFactory GetHttpClientFactory( IServiceProvider provider ) => provider.GetRequiredService<IHttpClientFactory>();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private Uri        CreateUrl( string  relativePath )                                    => new(_host.HostInfo, relativePath);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] private WebHandler CreateHandler( Uri url, HttpMethod method, CancellationToken token ) => CreateHandler( new HttpRequestMessage( method, url ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private WebHandler CreateHandler( Uri url, HttpMethod method, HttpContent value, CancellationToken token )
    {
        _logger?.LogDebug( "Starting a {Method} request to {Uri}", method.Method, url.ToString() );
        return CreateHandler( new HttpRequestMessage( method, url ) { Content = value }, token );
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] private WebHandler CreateHandler( HttpRequestMessage request, CancellationToken token ) => new(_logger, _client, request, Encoding, _retryPolicy, token);


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, HttpContent                 value, CancellationToken token ) => Delete( CreateUrl( relativePath ), value, token );
    public                                                      WebHandler Delete( Uri       url,          HttpContent                 value, CancellationToken token ) => CreateHandler( url, HttpMethod.Delete, value, token );
    public                                                      WebHandler Delete( Uri       url,          CancellationToken           token )                            => CreateHandler( url, HttpMethod.Delete, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, CancellationToken           token )                            => Delete( CreateUrl( relativePath ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, byte[]                      value,   CancellationToken token ) => Delete( relativePath,              new ByteArrayContent( value ),                       token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, ReadOnlyMemory<byte>        value,   CancellationToken token ) => Delete( relativePath,              new ReadOnlyMemoryContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, IDictionary<string, string> value,   CancellationToken token ) => Delete( relativePath,              new FormUrlEncodedContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, Stream                      value,   CancellationToken token ) => Delete( relativePath,              new StreamContent( value ),                          token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, MultipartContent            content, CancellationToken token ) => Delete( relativePath,              (HttpContent)content,                                token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, string                      value,   CancellationToken token ) => Delete( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, BaseClass                   value,   CancellationToken token ) => Delete( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, BaseRecord                  value,   CancellationToken token ) => Delete( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, IEnumerable<BaseRecord>     value,   CancellationToken token ) => Delete( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string    relativePath, IEnumerable<BaseClass>      value,   CancellationToken token ) => Delete( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete<T>( string relativePath, ImmutableArray<T>           value,   CancellationToken token ) => Delete( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );


    public                                                      WebHandler Get( Uri    url,          CancellationToken token ) => CreateHandler( url, HttpMethod.Get, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Get( string relativePath, CancellationToken token ) => Get( CreateUrl( relativePath ), token );


    public                                                      WebHandler Patch( Uri       url,          HttpContent                 value,   CancellationToken token ) => CreateHandler( url, HttpMethod.Patch, value, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, HttpContent                 value,   CancellationToken token ) => Patch( CreateUrl( relativePath ), value,                                               token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, byte[]                      value,   CancellationToken token ) => Patch( relativePath,              new ByteArrayContent( value ),                       token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, ReadOnlyMemory<byte>        value,   CancellationToken token ) => Patch( relativePath,              new ReadOnlyMemoryContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, IDictionary<string, string> value,   CancellationToken token ) => Patch( relativePath,              new FormUrlEncodedContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, Stream                      value,   CancellationToken token ) => Patch( relativePath,              new StreamContent( value ),                          token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, MultipartContent            content, CancellationToken token ) => Patch( relativePath,              (HttpContent)content,                                token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, string                      value,   CancellationToken token ) => Patch( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, BaseClass                   value,   CancellationToken token ) => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, BaseRecord                  value,   CancellationToken token ) => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, IEnumerable<BaseRecord>     value,   CancellationToken token ) => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string    relativePath, IEnumerable<BaseClass>      value,   CancellationToken token ) => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch<T>( string relativePath, ImmutableArray<T>           value,   CancellationToken token ) => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );


    public                                                      WebHandler Post( Uri       url,          HttpContent                 value,   CancellationToken token ) => CreateHandler( url, HttpMethod.Post, value, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, HttpContent                 value,   CancellationToken token ) => Post( CreateUrl( relativePath ), value,                                               token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, byte[]                      value,   CancellationToken token ) => Post( relativePath,              new ByteArrayContent( value ),                       token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, ReadOnlyMemory<byte>        value,   CancellationToken token ) => Post( relativePath,              new ReadOnlyMemoryContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, IDictionary<string, string> value,   CancellationToken token ) => Post( relativePath,              new FormUrlEncodedContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, Stream                      value,   CancellationToken token ) => Post( relativePath,              new StreamContent( value ),                          token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, MultipartContent            content, CancellationToken token ) => Post( relativePath,              (HttpContent)content,                                token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, string                      value,   CancellationToken token ) => Post( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, BaseClass                   value,   CancellationToken token ) => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, BaseRecord                  value,   CancellationToken token ) => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, IEnumerable<BaseClass>      value,   CancellationToken token ) => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string    relativePath, IEnumerable<BaseRecord>     value,   CancellationToken token ) => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post<T>( string relativePath, ImmutableArray<T>           value,   CancellationToken token ) => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );


    public                                                      WebHandler Put( Uri       url,          HttpContent                 value,   CancellationToken token ) => CreateHandler( url, HttpMethod.Put, value, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, HttpContent                 value,   CancellationToken token ) => Put( CreateUrl( relativePath ), value,                                               token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, byte[]                      value,   CancellationToken token ) => Put( relativePath,              new ByteArrayContent( value ),                       token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, ReadOnlyMemory<byte>        value,   CancellationToken token ) => Put( relativePath,              new ReadOnlyMemoryContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, IDictionary<string, string> value,   CancellationToken token ) => Put( relativePath,              new FormUrlEncodedContent( value ),                  token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, Stream                      value,   CancellationToken token ) => Put( relativePath,              new StreamContent( value ),                          token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, MultipartContent            content, CancellationToken token ) => Put( relativePath,              (HttpContent)content,                                token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, string                      value,   CancellationToken token ) => Put( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, BaseClass                   value,   CancellationToken token ) => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, BaseRecord                  value,   CancellationToken token ) => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, IEnumerable<BaseClass>      value,   CancellationToken token ) => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string    relativePath, IEnumerable<BaseRecord>     value,   CancellationToken token ) => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put<T>( string relativePath, ImmutableArray<T>           value,   CancellationToken token ) => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ),   token );


    public void Dispose() => _client.Dispose();
}
