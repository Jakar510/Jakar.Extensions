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
    public readonly   Encoding   Encoding = encoding ?? Encoding.Default;
    internal readonly HttpClient Client   = client;
    private readonly  IHostInfo  _host    = host;
    internal readonly ILogger?   Logger   = logger;


    public HttpRequestHeaders DefaultRequestHeaders { get => Client.DefaultRequestHeaders; }
    public RetryPolicy?       Retries               { get;                   set; }
    public TimeSpan           Timeout               { get => Client.Timeout; set => Client.Timeout = value; }


    public void Dispose() => Client.Dispose();


    public static  IServiceCollection AddSingleton( IServiceCollection       collection )                                                                 => collection.AddSingleton( Create );
    public static  IServiceCollection AddScoped( IServiceCollection          collection )                                                                 => collection.AddScoped( Create );
    public static  WebRequester       Get( IServiceProvider                  provider )                                                                   => provider.GetRequiredService<WebRequester>();
    public static  WebRequester       Create( IServiceProvider               provider )                                                                   => Create( GetHttpClientFactory( provider ), IHostInfo.Get( provider ), GetLogger( provider ) );
    public static  WebRequester       Create( IHttpClientFactory             factory, IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => Create( factory.CreateClient(),           host,                      logger, encoding );
    public static  WebRequester       Create( HttpClient                     client,  IHostInfo host, ILogger? logger = null, Encoding? encoding = null ) => new(client, host, logger, encoding);
    private static ILogger            GetLogger( IServiceProvider            provider ) => provider.GetRequiredService<ILoggerFactory>().CreateLogger( nameof(WebRequester) );
    private static IHttpClientFactory GetHttpClientFactory( IServiceProvider provider ) => provider.GetRequiredService<IHttpClientFactory>();


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private Uri        CreateUrl( string  relativePath )                              => new(_host.HostInfo, relativePath);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] private WebHandler CreateHandler( Uri url, HttpMethod method )                    => new(this, new HttpRequestMessage( method, url ));
    [MethodImpl( MethodImplOptions.AggressiveInlining )] private WebHandler CreateHandler( Uri url, HttpMethod method, HttpContent value ) => new(this, new HttpRequestMessage( method, url ) { Content = value });


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, HttpContent value ) => Delete( CreateUrl( relativePath ), value );
    public                                                      WebHandler Delete( Uri            url,          HttpContent value ) => CreateHandler( url, HttpMethod.Delete, value );
    public                                                      WebHandler Delete( Uri            url )                                               => CreateHandler( url, HttpMethod.Delete );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath )                                      => Delete( CreateUrl( relativePath ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, byte[]                      value )   => Delete( relativePath, new ByteArrayContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, in ReadOnlyMemory<byte>     value )   => Delete( relativePath, new ReadOnlyMemoryContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, IDictionary<string, string> value )   => Delete( relativePath, new FormUrlEncodedContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, Stream                      value )   => Delete( relativePath, new StreamContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, MultipartContent            content ) => Delete( relativePath, (HttpContent)content );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, string                      value )   => Delete( relativePath, new StringContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, BaseClass                   value )   => Delete( relativePath, new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, BaseRecord                  value )   => Delete( relativePath, new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, IEnumerable<BaseRecord>     value )   => Delete( relativePath, new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete( string         relativePath, IEnumerable<BaseClass>      value )   => Delete( relativePath, new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Delete<TValue>( string relativePath, in ImmutableArray<TValue>   value )   => Delete( relativePath, new JsonContent( value.ToPrettyJson(), Encoding ) );


    public                                                      WebHandler Get( Uri    url )          => CreateHandler( url, HttpMethod.Get );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Get( string relativePath ) => Get( CreateUrl( relativePath ) );


    public                                                      WebHandler Patch( Uri            url,          HttpContent                 value )   => CreateHandler( url, HttpMethod.Patch, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, HttpContent                 value )   => Patch( CreateUrl( relativePath ), value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, byte[]                      value )   => Patch( relativePath,              new ByteArrayContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, in ReadOnlyMemory<byte>     value )   => Patch( relativePath,              new ReadOnlyMemoryContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, IDictionary<string, string> value )   => Patch( relativePath,              new FormUrlEncodedContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, Stream                      value )   => Patch( relativePath,              new StreamContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, MultipartContent            content ) => Patch( relativePath,              (HttpContent)content );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, string                      value )   => Patch( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, BaseClass                   value )   => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, BaseRecord                  value )   => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, IEnumerable<BaseRecord>     value )   => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch( string         relativePath, IEnumerable<BaseClass>      value )   => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Patch<TValue>( string relativePath, in ImmutableArray<TValue>   value )   => Patch( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );


    public                                                      WebHandler Post( Uri            url,          HttpContent                 value )   => CreateHandler( url, HttpMethod.Post, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, HttpContent                 value )   => Post( CreateUrl( relativePath ), value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, byte[]                      value )   => Post( relativePath,              new ByteArrayContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, in ReadOnlyMemory<byte>     value )   => Post( relativePath,              new ReadOnlyMemoryContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, IDictionary<string, string> value )   => Post( relativePath,              new FormUrlEncodedContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, Stream                      value )   => Post( relativePath,              new StreamContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, MultipartContent            content ) => Post( relativePath,              (HttpContent)content );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, string                      value )   => Post( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, BaseClass                   value )   => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, BaseRecord                  value )   => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, IEnumerable<BaseClass>      value )   => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post( string         relativePath, IEnumerable<BaseRecord>     value )   => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Post<TValue>( string relativePath, in ImmutableArray<TValue>   value )   => Post( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );


    public                                                      WebHandler Put( Uri            url,          HttpContent                 value )   => CreateHandler( url, HttpMethod.Put, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, HttpContent                 value )   => Put( CreateUrl( relativePath ), value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, byte[]                      value )   => Put( relativePath,              new ByteArrayContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, in ReadOnlyMemory<byte>     value )   => Put( relativePath,              new ReadOnlyMemoryContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, IDictionary<string, string> value )   => Put( relativePath,              new FormUrlEncodedContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, Stream                      value )   => Put( relativePath,              new StreamContent( value ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, MultipartContent            content ) => Put( relativePath,              (HttpContent)content );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, string                      value )   => Put( relativePath,              new StringContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, BaseClass                   value )   => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, BaseRecord                  value )   => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, IEnumerable<BaseClass>      value )   => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put( string         relativePath, IEnumerable<BaseRecord>     value )   => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebHandler Put<TValue>( string relativePath, in ImmutableArray<TValue>   value )   => Put( relativePath,              new JsonContent( value.ToPrettyJson(), Encoding ) );
}
