// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(WebRequester))]
public class WebRequester_Tests : Assert
{
    private static readonly Uri                      __host  = new("https://example.test/api/");
    private static readonly WebRequester.RetryPolicy __retry = WebRequester.RetryPolicy.Create(TimeSpan.Zero, TimeSpan.Zero, 3);


    private static WebRequester Create( StubHandler handler, WebRequester.RetryPolicy? retries = null ) =>
        new(new HttpClient(handler), new WebRequester.Builder.HostHolder(__host)) { Retries = retries };



    public sealed record Item( string Name, int Count );



    public sealed class JsonItem( string text ) : BaseClass
    {
        public string Text { get; set; } = text;
    }


    // ─── Basic responses ─────────────────────────────────────────────────────

    [Test]
    public async Task Get_AsString_Success()
    {
        StubHandler            handler  = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.OK, "hello"));
        WebResponse<string>    response = await Create(handler).Get("items").AsString(CancellationToken.None);

        this.IsTrue(response.IsSuccessStatusCode);
        this.AreEqual("hello", response.Payload);
        this.AreEqual("https://example.test/api/items", handler.Requests[0].RequestUri!.ToString());
        this.IsFalse(response.Errors.HasValue); // no "unknown error" on success
    }

    [Test]
    public async Task ErrorStatus_ReturnsTheBodyAsTheError()
    {
        StubHandler         handler  = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.NotFound, "nope"));
        WebResponse<string> response = await Create(handler).Get("missing").AsString(CancellationToken.None);

        this.IsFalse(response.IsSuccessStatusCode);
        this.AreEqual(Status.NotFound, response.StatusCode);
        this.AreEqual("nope",          response.Errors.Text);
    }

    [Test]
    public async Task AsJson_Typed_And_Token()
    {
        StubHandler  handler   = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.OK, """{"Name":"widget","Count":3}"""));
        WebRequester requester = Create(handler);

        WebResponse<Item>   typed = await requester.Get("item").AsJson<Item>(CancellationToken.None);
        WebResponse<JToken> token = await requester.Get("item").AsJson(CancellationToken.None);

        this.AreEqual(new Item("widget", 3), typed.Payload);
        this.AreEqual(3,                     token.Payload!["Count"]!.Value<int>());
    }

    [Test]
    public async Task Bytes_Memory_Stream_ReturnTheBody()
    {
        byte[]       body      = Enumerable.Range(0, 5000).Select(static i => (byte)i).ToArray();
        StubHandler  handler   = new((_, _) => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(body) });
        WebRequester requester = Create(handler);

        WebResponse<byte[]>               bytes  = await requester.Get("b").AsBytes(CancellationToken.None);
        WebResponse<ReadOnlyMemory<byte>> memory = await requester.Get("b").AsMemory(CancellationToken.None);
        WebResponse<MemoryStream>         stream = await requester.Get("b").AsStream(CancellationToken.None);

        this.AreEqual(body, bytes.Payload);
        this.AreEqual(body, memory.Payload.ToArray());
        this.AreEqual(body, stream.Payload!.ToArray());
        this.AreEqual(0L,   stream.Payload.Position);
    }

    [Test]
    public async Task AsFile_LocalFile_WritesTheBodyToThatFile()
    {
        string       path    = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");
        StubHandler  handler = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.OK, "file contents"));
        LocalFile    target  = new(path);

        try
        {
            WebResponse<LocalFile> response = await Create(handler).Get("f").AsFile(target, CancellationToken.None);

            this.IsTrue(response.IsSuccessStatusCode);
            this.AreEqual("file contents", await File.ReadAllTextAsync(path));
        }
        finally { File.Delete(path); }
    }

    [Test]
    public async Task NoResponse_SuccessAndFailure()
    {
        WebRequester ok     = Create(new StubHandler(static ( _, _ ) => new HttpResponseMessage(HttpStatusCode.NoContent)));
        WebRequester failed = Create(new StubHandler(static ( _, _ ) => throw new HttpRequestException("down")));

        this.IsTrue(( await ok.Delete("x").NoResponse(CancellationToken.None) ).HasValue);
        this.IsFalse(( await failed.Delete("x").NoResponse(CancellationToken.None) ).HasValue);
    }


    // ─── Failures become responses ───────────────────────────────────────────

    [Test]
    public async Task NetworkFailure_ReturnsAFailedResponse()
    {
        StubHandler         handler  = new(static ( _, _ ) => throw new HttpRequestException("connection refused"));
        WebResponse<string> response = await Create(handler).Get("x").AsString(CancellationToken.None);

        this.IsFalse(response.IsSuccessStatusCode);
        this.AreEqual(Status.ServiceUnavailable, response.StatusCode);
        Assert.IsInstanceOf<HttpRequestException>(response.Exception?.Value);
        this.AreEqual("https://example.test/api/x", response.URL!.ToString());
    }

    [Test]
    public async Task Timeout_ReturnsRequestTimeout()
    {
        StubHandler         handler  = new(static ( _, _ ) => throw new TaskCanceledException("timed out"));
        WebResponse<string> response = await Create(handler).Get("x").AsString(CancellationToken.None);

        this.AreEqual(Status.RequestTimeout, response.StatusCode);
    }

    [Test]
    public async Task CallerCancellation_StillThrows()
    {
        StubHandler                   handler = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.OK, "x"));
        using CancellationTokenSource cts     = new();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await Create(handler).Get("x").AsString(cts.Token));
    }

    [Test]
    public async Task RealConnectionRefused_ReturnsAFailedResponse()
    {
        using HttpClient    client    = new();
        WebRequester        requester = new(client, new WebRequester.Builder.HostHolder(LoopbackServer.UnusedUrl()));
        WebResponse<string> response  = await requester.Get("x").AsString(CancellationToken.None);

        this.AreEqual(Status.ServiceUnavailable, response.StatusCode);
        Assert.IsNotNull(response.Exception);
    }


    // ─── Retries ─────────────────────────────────────────────────────────────

    [Test]
    public async Task TransientStatus_IsRetried_WithAFreshRequestAndTheSameBody()
    {
        StubHandler handler = new(static ( _, i ) => i < 2
                                                         ? StubHandler.Text(HttpStatusCode.ServiceUnavailable, "busy")
                                                         : StubHandler.Text(HttpStatusCode.OK,                 "done"));

        WebHandler request = Create(handler, __retry).Post("items", "payload");
        request.Headers.Add("X-Trace", "abc");
        WebResponse<string> response = await request.AsString(CancellationToken.None);

        this.AreEqual("done", response.Payload);
        this.AreEqual(3,      handler.Count);
        this.AreEqual(new[] { "payload", "payload", "payload" }, handler.Bodies.ToArray());
        this.AreEqual(3, handler.Requests.Distinct().Count()); // a message can only be sent once
        this.IsTrue(handler.Requests.All(static r => r.Headers.GetValues("X-Trace").Single() == "abc"));
    }

    [Test]
    public async Task NetworkFailure_IsRetried()
    {
        StubHandler handler = new(static ( _, i ) => i == 0
                                                         ? throw new HttpRequestException("reset")
                                                         : StubHandler.Text(HttpStatusCode.OK, "ok"));

        WebResponse<string> response = await Create(handler, __retry).Get("x").AsString(CancellationToken.None);

        this.AreEqual("ok", response.Payload);
        this.AreEqual(2,    handler.Count);
    }

    [Test]
    public async Task RetriesExhausted_ReturnsTheLastResponse()
    {
        StubHandler         handler  = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.BadGateway, "bad"));
        WebResponse<string> response = await Create(handler, __retry).Get("x").AsString(CancellationToken.None);

        this.AreEqual(Status.BadGateway, response.StatusCode);
        this.AreEqual(4,                 handler.Count); // 1 + 3 retries
    }

    [Test]
    public async Task NonTransientStatus_IsNotRetried()
    {
        StubHandler handler = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.BadRequest, "bad"));
        await Create(handler, __retry).Get("x").AsString(CancellationToken.None);
        this.AreEqual(1, handler.Count);
    }

    [Test]
    public async Task StreamBody_IsNotRetried()
    {
        StubHandler handler = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.ServiceUnavailable, "busy"));
        await Create(handler, __retry).Post("x", new MemoryStream([1, 2, 3])).AsString(CancellationToken.None);
        this.AreEqual(1, handler.Count);
    }

    [Test]
    public async Task RetryAfter_TooLong_IsNotWaitedFor()
    {
        StubHandler handler = new(static ( _, _ ) =>
                                  {
                                      HttpResponseMessage response = StubHandler.Text(HttpStatusCode.TooManyRequests, "slow down");
                                      response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromHours(1));
                                      return response;
                                  });

        WebResponse<string> result = await Create(handler, __retry).Get("x").AsString(CancellationToken.None);

        this.AreEqual(Status.TooManyRequests, result.StatusCode);
        this.AreEqual(1,                      handler.Count);
    }

    [Test]
    public async Task RetryAfter_Short_IsHonoured()
    {
        StubHandler handler = new(static ( _, i ) =>
                                  {
                                      if ( i > 0 ) { return StubHandler.Text(HttpStatusCode.OK, "ok"); }

                                      HttpResponseMessage response = StubHandler.Text(HttpStatusCode.TooManyRequests, "slow down");
                                      response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.Zero);
                                      return response;
                                  });

        WebResponse<string> result = await Create(handler, __retry).Get("x").AsString(CancellationToken.None);

        this.AreEqual("ok", result.Payload);
        this.AreEqual(2,    handler.Count);
    }

    [Test]
    public async Task RealServer_RetryResendsTheJsonBody()
    {
        await using LoopbackServer server = new(static ( _, i ) => i == 0
                                                                       ? LoopbackServer.Response.Of(503, "busy")
                                                                       : LoopbackServer.Response.Of(200, "created"));

        using HttpClient client    = new();
        WebRequester     requester = new(client, new WebRequester.Builder.HostHolder(server.Url)) { Retries = __retry };

        WebResponse<string> response = await requester.Post("items", WebRequester.CreateJsonContent(new Item("a", 1))).AsString(CancellationToken.None);

        this.AreEqual("created", response.Payload);
        this.AreEqual(2,         server.Requests.Count);
        this.AreEqual("""{"Name":"a","Count":1}""", server.Requests[0].Text);
        this.AreEqual(server.Requests[0].Text,      server.Requests[1].Text);
    }


    // ─── Request bodies ──────────────────────────────────────────────────────

    [Test]
    public async Task JsonContent_IsCompactUtf8()
    {
        using ByteArrayContent content = WebRequester.CreateJsonContent(new Item("ü", 2));

        this.AreEqual("application/json", content.Headers.ContentType!.MediaType);
        this.AreEqual("utf-8",            content.Headers.ContentType.CharSet);
        this.AreEqual("""{"Name":"ü","Count":2}""", Encoding.UTF8.GetString(await content.ReadAsByteArrayAsync()));
    }


    [TestCase("Default")]
    [TestCase("UTF8")]
    [TestCase("Unicode")]
    [TestCase("UTF32")]
    public async Task JsonContent_InEachEncoding_DeclaresItsCharset_HasNoBom_AndRoundTrips( string name )
    {
        const string TEXT = "héllo 日本語 中文 العربية Ελληνικά 😀🚀   �";
        Encoding encoding = name switch
                            {
                                "Default" => Encoding.Default,
                                "UTF8"    => Encoding.UTF8,
                                "Unicode" => Encoding.Unicode,
                                _         => Encoding.UTF32
                            };

        using ByteArrayContent content = WebRequester.CreateJsonContent(new Item(TEXT, 1), encoding);
        byte[]                 bytes   = await content.ReadAsByteArrayAsync();
        byte[]                 bom     = encoding.GetPreamble();

        this.AreEqual(encoding.WebName, content.Headers.ContentType!.CharSet);
        this.IsTrue(bom.Length == 0 || !bytes.AsSpan().StartsWith(bom));
        this.AreEqual(TEXT, Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(encoding.GetString(bytes))!.Name);
        this.AreEqual(TEXT, Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(await content.ReadAsStringAsync())!.Name); // decoded via the declared charset
    }

    [Test]
    public void JsonContent_DefaultsToEncodingDefault()
    {
        using ByteArrayContent content = WebRequester.CreateJsonContent(new Item("a", 1));
        this.AreEqual(Encoding.Default.WebName, content.Headers.ContentType!.CharSet);
    }

    [Test]
    public async Task JsonBodies_UseTheRequesterEncoding()
    {
        const string TEXT    = "日本語 😀";
        StubHandler  handler = new(static ( _, _ ) => StubHandler.Text(HttpStatusCode.OK, "ok"));
        WebRequester utf16   = new(new HttpClient(handler), new WebRequester.Builder.HostHolder(__host), encoding: Encoding.Unicode);

        await utf16.Post("items", new JsonItem(TEXT)).AsString(CancellationToken.None);

        this.AreEqual("utf-16",                       handler.Requests[0].Content!.Headers.ContentType!.CharSet);
        this.AreEqual($$"""{"Text":"{{TEXT}}"}""",    handler.Bodies[0]); // the stub reads the body via the declared charset
    }


    [Test]
    public async Task JsonContent_HasNoBom_AndRoundTripsAllOfUnicode()
    {
        const string           TEXT    = "héllo 日本語 中文 العربية Ελληνικά 😀🚀 \u0000 �";
        using ByteArrayContent content = WebRequester.CreateJsonContent(new Item(TEXT, 1));
        byte[]                 bytes   = await content.ReadAsByteArrayAsync();

        // RFC 8259 §8.1: no byte order mark (Encoding.UTF8 would write EF BB BF through a StreamWriter).
        this.IsFalse(bytes.AsSpan().StartsWith(Encoding.UTF8.GetPreamble()));

        // A receiver decoding with the declared charset gets every character back.
        this.AreEqual(TEXT, Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(await content.ReadAsStringAsync())!.Name);

        // UTF-8 -> UTF-16 (Encoding.Unicode) -> UTF-8 is lossless, and Encoding.Default reads the same bytes on .NET.
        byte[] utf16 = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, bytes);
        this.AreEqual(bytes,                         Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16));
        this.AreEqual(Encoding.UTF8.GetString(bytes), Encoding.Unicode.GetString(utf16));
        this.AreEqual(Encoding.UTF8.GetString(bytes), Encoding.Default.GetString(bytes));
    }


    // ─── Builder ─────────────────────────────────────────────────────────────

    [Test]
    public void Builder_SharesOneHandler_UntilTheConfigurationChanges()
    {
        using WebRequester.Builder builder = WebRequester.Builder.Create(__host);

        using HttpClient first  = builder.CreateClient("a");
        using HttpClient second = builder.CreateClient("b");
        Assert.AreSame(HandlerOf(first), HandlerOf(second));

        builder.With_MaxConnectionsPerServer(8);
        using HttpClient third = builder.CreateClient("c");
        Assert.AreNotSame(HandlerOf(first), HandlerOf(third));
    }

    [Test]
    public async Task Builder_DisposedAfterBuild_TheRequesterKeepsWorking()
    {
        await using LoopbackServer server = new(static ( _, _ ) => LoopbackServer.Response.Of(200, "ok"));
        WebRequester requester;

        // The WebRequester.Create(IServiceProvider) pattern: build, then dispose the builder.
        using ( WebRequester.Builder builder = WebRequester.Builder.Create(server.Url) )
        {
            builder.With_Retry();
            requester = builder.Build();
        }

        HttpMessageHandler shared = HandlerOf(requester.Client);
        this.AreEqual("ok", ( await requester.Get("x").AsString(CancellationToken.None) ).Payload);
        this.IsFalse(await IsDisposed(shared, server.Url));

        requester.Dispose(); // last owner: now the shared handler is disposed
        this.IsTrue(await IsDisposed(shared, server.Url));
    }

    [Test]
    public async Task SharedHandler_LivesUntilTheBuilderAndEveryClientAreDisposed()
    {
        await using LoopbackServer server  = new(static ( _, _ ) => LoopbackServer.Response.Of(200, "ok"));
        WebRequester.Builder       builder = WebRequester.Builder.Create(server.Url);
        HttpClient                 first   = builder.CreateClient("a");
        HttpClient                 second  = builder.CreateClient("b");
        HttpMessageHandler         shared  = HandlerOf(first);

        builder.Dispose();
        first.Dispose();
        this.AreEqual("ok", await second.GetStringAsync(server.Url));

        second.Dispose();
        this.IsTrue(await IsDisposed(shared, server.Url));
    }

    [Test]
    public async Task ConfigurationChange_KeepsExistingClientsWorking_AndReleasesTheOldHandlerWithThem()
    {
        await using LoopbackServer server  = new(static ( _, _ ) => LoopbackServer.Response.Of(200, "ok"));
        using WebRequester.Builder builder = WebRequester.Builder.Create(server.Url);
        HttpClient                 before  = builder.CreateClient("a");
        HttpMessageHandler         old     = HandlerOf(before);

        builder.With_MaxConnectionsPerServer(4); // new handler for clients created from now on
        using HttpClient after = builder.CreateClient("b");

        Assert.AreNotSame(old, HandlerOf(after));
        this.AreEqual("ok", await before.GetStringAsync(server.Url));

        before.Dispose(); // the old handler's only remaining owner
        this.IsTrue(await IsDisposed(old, server.Url));
        this.AreEqual("ok", await after.GetStringAsync(server.Url));
    }


    [Test]
    public void Builder_Defaults_And_Timeouts()
    {
        using WebRequester.Builder builder = WebRequester.Builder.Create(__host).With_Timeout(TimeSpan.FromSeconds(30)).With_ConnectTimeout(TimeSpan.FromSeconds(3));
        using HttpClient           client  = builder.CreateClient("x");
        SocketsHttpHandler         handler = (SocketsHttpHandler)HandlerOf(client);

        this.AreEqual(TimeSpan.FromSeconds(30),                           client.Timeout);
        this.AreEqual(TimeSpan.FromSeconds(3),                            handler.ConnectTimeout);
        this.AreEqual(DecompressionMethods.All,                           handler.AutomaticDecompression);
        this.AreEqual(WebRequester.Builder.DefaultPooledConnectionLifetime, handler.PooledConnectionLifetime);
    }

    [Test]
    public async Task Builder_DecompressesGzipResponses()
    {
        byte[] compressed;

        using ( MemoryStream buffer = new() )
        {
            await using ( GZipStream gzip = new(buffer, CompressionLevel.Fastest, true) ) { await gzip.WriteAsync(Encoding.UTF8.GetBytes("compressed hello")); }

            compressed = buffer.ToArray();
        }

        await using LoopbackServer server = new((_, _) => new LoopbackServer.Response(200, compressed, new Dictionary<string, string> { ["Content-Encoding"] = "gzip" }));
        using WebRequester.Builder builder   = WebRequester.Builder.Create(server.Url);
        WebRequester               requester = builder.Build();

        WebResponse<string> response = await requester.Get("z").AsString(CancellationToken.None);

        this.AreEqual("compressed hello", response.Payload);
        this.IsTrue(server.Requests[0].Headers["Accept-Encoding"].Contains("gzip"));
    }

    [Test]
    public async Task RealServer_AsFile_StreamsALargeBodyToDisk()
    {
        byte[] body = new byte[1 << 20];
        new Random(1).NextBytes(body);

        await using LoopbackServer server = new((_, _) => new LoopbackServer.Response(200, body));
        using HttpClient           client    = new();
        WebRequester               requester = new(client, new WebRequester.Builder.HostHolder(server.Url));

        WebResponse<LocalFile> response = await requester.Get("download").AsFile(CancellationToken.None);

        try { this.AreEqual(body, await File.ReadAllBytesAsync(response.Payload!.FullPath)); }
        finally { File.Delete(response.Payload!.FullPath); }
    }


    /// <summary> The handler doing the work: builder clients hold a lease (a <see cref="DelegatingHandler"/>) on the shared one. </summary>
    private static HttpMessageHandler HandlerOf( HttpClient client )
    {
        HttpMessageHandler handler = (HttpMessageHandler)typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(client)!;
        return handler is DelegatingHandler { InnerHandler: { } inner } ? inner : handler;
    }
    private static async Task<bool> IsDisposed( HttpMessageHandler handler, Uri url )
    {
        try
        {
            using HttpClient probe = new(handler, false);
            await probe.GetStringAsync(url);
            return false;
        }
        catch ( ObjectDisposedException ) { return true; }
    }
}
