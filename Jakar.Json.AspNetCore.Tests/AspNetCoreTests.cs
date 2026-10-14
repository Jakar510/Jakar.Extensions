// Jakar.Json.AspNetCore.Tests
// 10/14/2026

using System.Buffers;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;



namespace Jakar.Json.AspNetCore.Tests;


/// <summary> End to end over Kestrel (loopback, any free port): MVC controllers and minimal APIs, with Jakar.Json for [JsonModel] types and System.Text.Json for the rest. </summary>
[TestFixture]
public sealed class AspNetCoreTests
{
    // Jakar.Json writes " : " between names and values; System.Text.Json writes ":". That tells the serializers apart in a response.
    private const string JAKAR_SEPARATOR = "\" : ";


    // ─── Registry ────────────────────────────────────────────────────────────

    [Test] public void Registry_FindsModelsAndTheirCollections()
    {
        foreach ( Type type in new[] { typeof(TestOrder), typeof(TestOrder[]), typeof(List<TestOrder>), typeof(IEnumerable<TestOrder>), typeof(IReadOnlyList<TestOrder>), typeof(IReadOnlyCollection<TestOrder>), typeof(IList<TestOrder>), typeof(ICollection<TestOrder>) } )
        {
            Assert.That(JsonCodecRegistry.TryGet(type, out JsonTypeCodec? codec), Is.True, type.ToString());
            Assert.That(codec!.Type, Is.EqualTo(type));
        }

        foreach ( Type type in new[] { typeof(PlainDto), typeof(List<PlainDto>), typeof(Dictionary<string, TestOrder>), typeof(string), typeof(object) } ) { Assert.That(JsonCodecRegistry.IsRegistered(type), Is.False, type.ToString()); }
    }

    [Test] public void Codec_RoundTripsThroughABufferWriter()
    {
        Assert.That(JsonCodecRegistry.TryGet(typeof(List<TestOrder>), out JsonTypeCodec? codec), Is.True);

        ArrayBufferWriter<byte> output = new();
        codec!.Write(new[] { Samples.Order(1), Samples.Order(2) }, output); // any IEnumerable<T> is written
        object? read = codec.Read(output.WrittenSpan);

        Assert.That(read, Is.TypeOf<List<TestOrder>>());
        Assert.That((List<TestOrder>)read!, Is.EqualTo(new[] { Samples.Order(1), Samples.Order(2) }));
        Assert.That(codec.Read("null"u8), Is.Null);
    }


    // ─── MVC ─────────────────────────────────────────────────────────────────

    [Test] public async Task Mvc_ReadsAndWritesModelsWithJakarJson()
    {
        await using Server server = await Server.StartMvcAsync();

        using HttpResponseMessage response = await server.Client.PostAsync("/orders/echo", Json(JsonCodec.ToJson(Samples.Order())));
        string                    body     = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode,                       Is.EqualTo(HttpStatusCode.OK), body);
        Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        Assert.That(body,                                      Does.Contain(JAKAR_SEPARATOR), "written by Jakar.Json");
        Assert.That(JsonCodec.FromJson<TestOrder>(body),        Is.EqualTo(Samples.Order()));
    }

    [Test] public async Task Mvc_ReadsSystemTextJsonOutput()
    {
        await using Server server = await Server.StartMvcAsync();

        // Compact JSON, as System.Text.Json writes it (no spaces), is read just the same. Names are the model's own: context-less [JsonModel] types read them as written (case-sensitive).
        using HttpResponseMessage response = await server.Client.PostAsync("/orders/echo", Json("""{"Id":7,"Customer":"Ada","Total":12.50,"Lines":[{"Sku":"A-1","Quantity":2,"Price":1.25},{"Sku":"B-2","Quantity":1,"Price":10}]}"""));

        Assert.That(response.StatusCode,                                                   Is.EqualTo(HttpStatusCode.OK), await response.Content.ReadAsStringAsync());
        Assert.That(JsonCodec.FromJson<TestOrder>(await response.Content.ReadAsStringAsync()), Is.EqualTo(Samples.Order()));
    }

    [Test] public async Task Mvc_WritesCollections()
    {
        await using Server server = await Server.StartMvcAsync();

        string body = await server.Client.GetStringAsync("/orders");

        Assert.That(body,                                      Does.Contain(JAKAR_SEPARATOR));
        Assert.That(JsonCodec.FromJsonList<TestOrder>(body),   Is.EqualTo(new[] { Samples.Order(1), Samples.Order(2) }));
    }

    [Test] public async Task Mvc_MalformedJsonIsA400WithThePath()
    {
        await using Server server = await Server.StartMvcAsync();

        using HttpResponseMessage response = await server.Client.PostAsync("/orders/echo", Json("""{"Id":7,"Customer":"Ada","Total":"oops"}"""));
        string                    body     = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(body,                Does.Contain("$.Total"), body);
    }

    [Test] public async Task Mvc_OtherTypesStayOnSystemTextJson()
    {
        await using Server server = await Server.StartMvcAsync();

        using HttpResponseMessage response = await server.Client.PostAsync("/orders/plain", Json("""{"id":3,"name":"x"}"""));
        string                    body     = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), body);
        Assert.That(body,                Is.EqualTo("""{"id":3,"name":"x"}"""));
    }

    [Test] public async Task Mvc_NullIsStill204()
    {
        await using Server server = await Server.StartMvcAsync();

        using HttpResponseMessage response = await server.Client.GetAsync("/orders/none");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test] public async Task Mvc_OtherCharsetsGoToSystemTextJson()
    {
        await using Server server = await Server.StartMvcAsync();

        StringContent content = new("""{"Id":7,"Customer":"Ada","Total":12.50,"Lines":[]}""", Encoding.Unicode);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-16" };

        using HttpResponseMessage response = await server.Client.PostAsync("/orders/echo", content);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), await response.Content.ReadAsStringAsync());
        Assert.That(JsonCodec.FromJson<TestOrder>(await response.Content.ReadAsStringAsync()).Customer, Is.EqualTo("Ada"));
    }


    // ─── Minimal APIs ────────────────────────────────────────────────────────

    [Test] public async Task Minimal_JsonBodyAndResults()
    {
        await using Server server = await Server.StartMinimalAsync();

        using HttpResponseMessage response = await server.Client.PostAsync("/api/echo", Json(JsonCodec.ToJson(Samples.Order())));
        string                    body     = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode,                      Is.EqualTo(HttpStatusCode.Created), body);
        Assert.That(response.Headers.Location?.ToString(),    Is.EqualTo("/api/orders/7"));
        Assert.That(body,                                     Does.Contain(JAKAR_SEPARATOR));
        Assert.That(JsonCodec.FromJson<TestOrder>(body),       Is.EqualTo(Samples.Order()));
    }

    [Test] public async Task Minimal_FilterWritesPlainReturnValues()
    {
        await using Server server = await Server.StartMinimalAsync();

        string one  = await server.Client.GetStringAsync("/api/orders/5");
        string many = await server.Client.GetStringAsync("/api/orders");
        string dto  = await server.Client.GetStringAsync("/api/plain");

        Assert.That(one,                                     Does.Contain(JAKAR_SEPARATOR));
        Assert.That(JsonCodec.FromJson<TestOrder>(one),       Is.EqualTo(Samples.Order(5)));
        Assert.That(JsonCodec.FromJsonList<TestOrder>(many),  Is.EqualTo(new[] { Samples.Order(1), Samples.Order(2) }));
        Assert.That(dto,                                     Is.EqualTo("""{"id":3,"name":"x"}"""), "not a [JsonModel]: System.Text.Json");
    }

    [Test] public async Task Minimal_JsonBodyErrors()
    {
        await using Server server = await Server.StartMinimalAsync();

        using HttpResponseMessage malformed = await server.Client.PostAsync("/api/echo", Json("""{"Id":"x"}"""));
        using HttpResponseMessage notJson   = await server.Client.PostAsync("/api/echo", new StringContent("{}", Encoding.UTF8, "text/plain"));
        using HttpResponseMessage empty     = await server.Client.PostAsync("/api/echo", Json(""));
        using HttpResponseMessage optional  = await server.Client.PostAsync("/api/optional", Json(""));

        Assert.That(malformed.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(notJson.StatusCode,   Is.EqualTo(HttpStatusCode.UnsupportedMediaType));
        Assert.That(empty.StatusCode,     Is.EqualTo(HttpStatusCode.BadRequest), "a required body");
        Assert.That(optional.StatusCode,  Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await optional.Content.ReadAsStringAsync(), Is.EqualTo("none"));
    }

    [Test] public async Task Minimal_BodyLargerThanMaxDocumentBytesIsA413()
    {
        await using Server server = await Server.StartMinimalAsync(options => options.ReaderOptions = new JsonReaderOptions { MaxDocumentBytes = 64 });

        using HttpResponseMessage response = await server.Client.PostAsync("/api/echo", Json(JsonCodec.ToJson(Samples.Order())));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.RequestEntityTooLarge));
    }

    [Test] public async Task Minimal_WriterOptionsApply()
    {
        await using Server server = await Server.StartMinimalAsync(options => options.WriterOptions = new JsonWriterOptions { Indented = true });

        string body = await server.Client.GetStringAsync("/api/orders/5");

        Assert.That(body, Does.Contain("\n"));
    }


    private static StringContent Json( string json ) => new(json, Encoding.UTF8, "application/json");



    private sealed class Server( WebApplication app, HttpClient client ) : IAsyncDisposable
    {
        public HttpClient Client { get; } = client;


        public static Task<Server> StartMvcAsync() => StartAsync(static builder => builder.Services.AddControllers().AddApplicationPart(typeof(OrdersController).Assembly).AddJakarJson(),
                                                                 static app => app.MapControllers());

        public static Task<Server> StartMinimalAsync( Action<JakarJsonOptions>? configure = null ) => StartAsync(builder => builder.Services.AddJakarJson(configure),
                                                                                                                  static app =>
                                                                                                                  {
                                                                                                                      RouteGroupBuilder api = app.MapGroup("/api").WithJakarJson();

                                                                                                                      api.MapPost("/echo",     static ( JsonBody<TestOrder> order ) => JakarResults.Created($"/api/orders/{order.Value.Id}", order.Value));
                                                                                                                      api.MapPost("/optional", static ( JsonBody<TestOrder>? order ) => order is null ? "none" : "some");
                                                                                                                      api.MapGet("/orders/{id:int}", static ( int id ) => Samples.Order(id));
                                                                                                                      api.MapGet("/orders",    static () => new List<TestOrder> { Samples.Order(1), Samples.Order(2) });
                                                                                                                      api.MapGet("/plain",     static () => new PlainDto(3, "x"));
                                                                                                                  });

        private static async Task<Server> StartAsync( Action<WebApplicationBuilder> configure, Action<WebApplication> map )
        {
            WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
            builder.WebHost.UseUrls("http://127.0.0.1:0");
            builder.Logging.ClearProviders();
            configure(builder);

            WebApplication app = builder.Build();
            map(app);
            await app.StartAsync();

            return new Server(app, new HttpClient { BaseAddress = new Uri(app.Urls.First()) });
        }

        public async ValueTask DisposeAsync()
        {
            Client.Dispose();
            await app.StopAsync();
            await app.DisposeAsync();
        }
    }
}



[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpPost("echo")]  public ActionResult<TestOrder>       Echo( TestOrder order ) => order;
    [HttpGet]           public ActionResult<List<TestOrder>> All()                   => new List<TestOrder> { Samples.Order(1), Samples.Order(2) };
    [HttpGet("none")]   public ActionResult<TestOrder?>      None()                  => (TestOrder?)null;
    [HttpPost("plain")] public ActionResult<PlainDto>        Plain( PlainDto dto )   => dto;
}
