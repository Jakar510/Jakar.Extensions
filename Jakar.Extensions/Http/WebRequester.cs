﻿// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:35 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public sealed partial class WebRequester( HttpClient client, IHostInfo host, Encoding encoding, ILogger? logger = default, RetryPolicy? retryPolicy = default ) : IDisposable
{
    private readonly HttpClient   _client      = client;
    private readonly IHostInfo    _host        = host;
    private readonly ILogger?     _logger      = logger;
    private readonly RetryPolicy? _retryPolicy = retryPolicy;


    public HttpRequestHeaders DefaultRequestHeaders { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _client.DefaultRequestHeaders; }
    public Encoding           Encoding              { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = encoding;
    public TimeSpan           Timeout               { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _client.Timeout; set => _client.Timeout = value; }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public WebRequester( HttpClient client, IHostInfo host, ILogger? logger = default, RetryPolicy? retryPolicy = default ) : this( client, host, Encoding.Default, logger, retryPolicy ) { }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static WebRequester Create<T>( IHttpClientFactory factory, IHostInfo host, ILogger? logger = default, RetryPolicy? retryPolicy = default )                                     => Create<T>( factory, host, Encoding.Default, logger, retryPolicy );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static WebRequester Create<T>( IHttpClientFactory factory, IHostInfo host, Encoding encoding,         ILogger?     logger      = default, RetryPolicy? retryPolicy = default ) => Create( factory.CreateClient( typeof(T).Name ), host, encoding, logger, retryPolicy );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static WebRequester Create( IHttpClientFactory    factory, IHostInfo host, ILogger? logger = default, RetryPolicy? retryPolicy = default )                                     => Create( factory,                host, Encoding.Default, logger, retryPolicy );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static WebRequester Create( IHttpClientFactory    factory, IHostInfo host, Encoding encoding,         ILogger?     logger      = default, RetryPolicy? retryPolicy = default ) => Create( factory.CreateClient(), host, encoding,         logger, retryPolicy );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static WebRequester Create( HttpClient            client,  IHostInfo host, ILogger? logger = default, RetryPolicy? retryPolicy = default )                                     => new(client, host, logger, retryPolicy);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static WebRequester Create( HttpClient            client,  IHostInfo host, Encoding encoding,         ILogger?     logger      = default, RetryPolicy? retryPolicy = default ) => new(client, host, encoding, logger, retryPolicy);


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
