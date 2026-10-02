// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(WebRequesterOptions))]
public class WebRequesterOptions_Tests : Assert
{
    private static IConfiguration Configuration( Dictionary<string, string?> values ) => new ConfigurationBuilder().AddInMemoryCollection(values).Build();


    [Test]
    public void BindsFromConfiguration()
    {
        IConfiguration configuration = Configuration(new Dictionary<string, string?>
                                                     {
                                                         ["WebRequester:BaseAddress"]                = "https://api.example.test/v2/",
                                                         ["WebRequester:Timeout"]                    = "00:00:30",
                                                         ["WebRequester:ConnectTimeout"]             = "00:00:05",
                                                         ["WebRequester:PooledConnectionLifetime"]   = "00:02:00",
                                                         ["WebRequester:MaxConnectionsPerServer"]    = "16",
                                                         ["WebRequester:MaxAutomaticRedirections"]   = "0",
                                                         ["WebRequester:AutomaticDecompression"]     = "GZip, Brotli",
                                                         ["WebRequester:Encoding"]                   = "utf-16",
                                                         ["WebRequester:Retry:MaxRetries"]           = "5",
                                                         ["WebRequester:Retry:Delay"]                = "00:00:01",
                                                         ["WebRequester:DefaultHeaders:X-Api-Version"] = "2"
                                                     });

        ServiceCollection services = new();
        services.AddWebRequester(configuration.GetSection("WebRequester"));
        using ServiceProvider provider = services.BuildServiceProvider();

        WebRequesterOptions options = provider.GetRequiredService<IOptions<WebRequesterOptions>>().Value;

        this.AreEqual(new Uri("https://api.example.test/v2/"),               options.BaseAddress);
        this.AreEqual(TimeSpan.FromSeconds(30),                              options.Timeout);
        this.AreEqual(16,                                                    options.MaxConnectionsPerServer);
        this.AreEqual(DecompressionMethods.GZip | DecompressionMethods.Brotli, options.AutomaticDecompression);
        this.AreEqual(Encoding.Unicode.WebName,                              options.GetEncoding().WebName);
        this.AreEqual((ushort)5,                                             options.Retry!.MaxRetries);
        this.AreEqual(TimeSpan.FromSeconds(1),                               options.Retry.Delay);
        this.AreEqual(WebRequester.RetryPolicy.Default.Scale,                options.Retry.Scale); // unset keeps the default
        this.AreEqual("2",                                                   options.DefaultHeaders["x-api-version"]);
    }

    [Test]
    public void RequesterFromDi_AppliesTheOptions()
    {
        ServiceCollection services = new();

        services.AddWebRequester(static options =>
                                 {
                                     options.BaseAddress             = new Uri("https://api.example.test/");
                                     options.Timeout                 = TimeSpan.FromSeconds(12);
                                     options.ConnectTimeout          = TimeSpan.FromSeconds(2);
                                     options.MaxConnectionsPerServer = 7;
                                     options.AutomaticDecompression  = DecompressionMethods.GZip;
                                     options.Encoding                = "utf-16";
                                     options.Retry                   = new WebRequesterOptions.RetryOptions { MaxRetries = 1 };
                                     options.DefaultHeaders["X-Key"] = "secret";
                                 });

        using ServiceProvider provider  = services.BuildServiceProvider();
        WebRequester          requester = provider.GetRequiredService<WebRequester>();
        SocketsHttpHandler    handler   = HandlerOf(requester.Client);

        Assert.AreSame(requester, provider.GetRequiredService<WebRequester>()); // singleton
        this.AreEqual(TimeSpan.FromSeconds(12),  requester.Timeout);
        this.AreEqual(TimeSpan.FromSeconds(2),   handler.ConnectTimeout);
        this.AreEqual(7,                         handler.MaxConnectionsPerServer);
        this.AreEqual(DecompressionMethods.GZip, handler.AutomaticDecompression);
        this.AreEqual(Encoding.Unicode.WebName,  requester.Encoding.WebName);
        this.AreEqual((ushort)1,                 requester.Retries!.Value.MaxRetires);
        this.AreEqual("secret",                  string.Join(",", requester.DefaultRequestHeaders.GetValues("X-Key")));
    }

    [Test]
    public void Defaults_WhenNothingIsConfigured()
    {
        ServiceCollection services = new();
        services.AddSingleton<IHostInfo>(new WebRequester.Builder.HostHolder(new Uri("https://host.test/")));
        services.AddWebRequester();

        using ServiceProvider provider  = services.BuildServiceProvider();
        WebRequester          requester = provider.GetRequiredService<WebRequester>();
        SocketsHttpHandler    handler   = HandlerOf(requester.Client);

        this.IsNull(requester.Retries); // no retries unless WebRequesterOptions.Retry is set
        this.AreEqual(DecompressionMethods.All,                              handler.AutomaticDecompression);
        this.AreEqual(WebRequester.Builder.DefaultPooledConnectionLifetime, handler.PooledConnectionLifetime);
        this.AreEqual(Encoding.Default.WebName,                              requester.Encoding.WebName);
    }

    [Test]
    public void NamedRequesters_AreKeyedSingletons_WithTheirOwnOptions()
    {
        ServiceCollection services = new();
        services.AddWebRequester("github", static o => o.BaseAddress = new Uri("https://api.github.test/"));
        services.AddWebRequester("billing", static o =>
                                            {
                                                o.BaseAddress = new Uri("https://billing.test/");
                                                o.Timeout     = TimeSpan.FromSeconds(3);
                                            });

        using ServiceProvider provider = services.BuildServiceProvider();
        WebRequester          github   = provider.GetRequiredKeyedService<WebRequester>("github");
        WebRequester          billing  = provider.GetRequiredKeyedService<WebRequester>("billing");

        Assert.AreNotSame(github, billing);
        Assert.AreSame(github, provider.GetRequiredKeyedService<WebRequester>("github"));
        this.AreEqual(new Uri("https://api.github.test/"), github.Host.HostInfo);
        this.AreEqual(TimeSpan.FromSeconds(3),             billing.Timeout);
        this.IsNull(provider.GetService<WebRequester>()); // no default requester was registered
    }

    [Test]
    public void BaseAddress_FallsBackToTheRegisteredHostInfo()
    {
        ServiceCollection services = new();
        services.AddSingleton<IHostInfo>(new WebRequester.Builder.HostHolder(new Uri("https://fallback.test/")));
        services.AddWebRequester();

        using ServiceProvider provider = services.BuildServiceProvider();
        this.AreEqual(new Uri("https://fallback.test/"), provider.GetRequiredService<WebRequester>().Host.HostInfo);
    }

    [TestCase("BaseAddress",             "relative/path")]
    [TestCase("Timeout",                 "-00:00:01")]
    [TestCase("MaxConnectionsPerServer", "0")]
    [TestCase("Encoding",                "not-a-real-encoding")]
    public void InvalidOptions_FailValidation( string key, string value )
    {
        ServiceCollection services = new();
        services.AddSingleton<IHostInfo>(new WebRequester.Builder.HostHolder(new Uri("https://host.test/")));
        services.AddWebRequester(Configuration(new Dictionary<string, string?> { [key] = value }));

        using ServiceProvider provider = services.BuildServiceProvider();
        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<WebRequester>());
    }

    [Test]
    public void ConfigureBuilder_RunsLast_ForCodeOnlySettings()
    {
        ServiceCollection services = new();

        services.AddWebRequester(static o =>
                                 {
                                     o.BaseAddress      = new Uri("https://h.test/");
                                     o.Timeout          = TimeSpan.FromSeconds(10);
                                     o.ConfigureBuilder = static builder => builder.With_Timeout(TimeSpan.FromSeconds(99)).With_Header("X-From-Code", "yes");
                                 });

        using ServiceProvider provider  = services.BuildServiceProvider();
        WebRequester          requester = provider.GetRequiredService<WebRequester>();

        this.AreEqual(TimeSpan.FromSeconds(99), requester.Timeout);
        this.AreEqual("yes",                    string.Join(",", requester.DefaultRequestHeaders.GetValues("X-From-Code")));
    }

    [Test]
    public async Task ConfiguredRequester_SendsItsDefaultHeaders()
    {
        await using LoopbackServer server = new(static ( _, _ ) => LoopbackServer.Response.Of(200, "ok"));

        ServiceCollection services = new();
        services.AddWebRequester(o =>
                                 {
                                     o.BaseAddress                       = server.Url;
                                     o.DefaultHeaders["X-Api-Version"] = "2";
                                 });

        await using ServiceProvider provider = services.BuildServiceProvider();
        WebResponse<string>         response = await provider.GetRequiredService<WebRequester>().Get("ping").AsString(CancellationToken.None);

        this.AreEqual("ok", response.Payload);
        this.AreEqual("2",  server.Requests[0].Headers["X-Api-Version"]);
    }


    // ─── Live reload ─────────────────────────────────────────────────────────

    private static (ServiceProvider Provider, IConfigurationRoot Configuration) Reloadable( Dictionary<string, string?> values, string? name = null )
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        ServiceCollection  services      = new();

        if ( name is null ) { services.AddWebRequester(configuration.GetSection("Web")); }
        else { services.AddWebRequester(name, configuration.GetSection("Web")); }

        return ( services.BuildServiceProvider(), configuration );
    }

    [Test]
    public void ConfigurationChange_ReloadsTheSameRequesterInstance()
    {
        ( ServiceProvider provider, IConfigurationRoot configuration ) = Reloadable(new Dictionary<string, string?>
                                                                                     {
                                                                                         ["Web:BaseAddress"]             = "https://one.test/",
                                                                                         ["Web:Timeout"]                 = "00:00:10",
                                                                                         ["Web:MaxConnectionsPerServer"] = "4"
                                                                                     });

        using ( provider )
        {
            WebRequester requester = provider.GetRequiredService<WebRequester>();
            HttpClient   before    = requester.Client;
            int          reloads   = 0;
            requester.Reloaded += _ => reloads++;

            configuration["Web:BaseAddress"]             = "https://two.test/";
            configuration["Web:Timeout"]                 = "00:00:45";
            configuration["Web:MaxConnectionsPerServer"] = "9";
            configuration["Web:Retry:MaxRetries"]        = "2";
            configuration.Reload();

            Assert.AreSame(requester, provider.GetRequiredService<WebRequester>()); // same singleton, new configuration
            Assert.AreNotSame(before, requester.Client);
            this.AreEqual(1,                          reloads);
            this.AreEqual(new Uri("https://two.test/"), requester.Host.HostInfo);
            this.AreEqual(TimeSpan.FromSeconds(45),   requester.Timeout);
            this.AreEqual(9,                          HandlerOf(requester.Client).MaxConnectionsPerServer);
            this.AreEqual((ushort)2,                  requester.Retries!.Value.MaxRetires);
        }
    }

    [Test]
    public void InvalidReload_IsIgnored_AndKeepsTheCurrentConfiguration()
    {
        ( ServiceProvider provider, IConfigurationRoot configuration ) = Reloadable(new Dictionary<string, string?>
                                                                                     {
                                                                                         ["Web:BaseAddress"] = "https://one.test/",
                                                                                         ["Web:Timeout"]     = "00:00:10"
                                                                                     });

        using ( provider )
        {
            WebRequester requester = provider.GetRequiredService<WebRequester>();
            HttpClient   before    = requester.Client;

            configuration["Web:MaxConnectionsPerServer"] = "0"; // invalid
            Assert.DoesNotThrow(configuration.Reload);           // never throws on the configuration thread

            Assert.AreSame(before, requester.Client);
            this.AreEqual(TimeSpan.FromSeconds(10), requester.Timeout);
        }
    }

    [Test]
    public void NamedRequesters_ReloadOnlyForTheirOwnName()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                                                                                            {
                                                                                                ["A:BaseAddress"] = "https://a.test/",
                                                                                                ["B:BaseAddress"] = "https://b.test/"
                                                                                            }).Build();

        ServiceCollection services = new();
        services.AddWebRequester("a", configuration.GetSection("A"));
        services.AddWebRequester("b", configuration.GetSection("B"));

        using ServiceProvider provider = services.BuildServiceProvider();
        WebRequester          a        = provider.GetRequiredKeyedService<WebRequester>("a");
        WebRequester          b        = provider.GetRequiredKeyedService<WebRequester>("b");
        HttpClient            bClient  = b.Client;

        configuration["A:BaseAddress"] = "https://a2.test/";
        configuration.Reload();

        this.AreEqual(new Uri("https://a2.test/"), a.Host.HostInfo);
        this.AreEqual(new Uri("https://b.test/"),  b.Host.HostInfo);
        Assert.AreNotSame(bClient, b.Client); // the whole configuration reloaded, so b rebuilt too, from its unchanged section
    }

    [Test]
    public async Task RequestInFlight_DuringReload_CompletesOnTheOldClient()
    {
        TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);

        await using LoopbackServer slow = new((_, _) =>
                                              {
                                                  release.Task.Wait();
                                                  return LoopbackServer.Response.Of(200, "slow");
                                              });

        await using LoopbackServer fast = new(static ( _, _ ) => LoopbackServer.Response.Of(200, "fast"));

        ( ServiceProvider provider, IConfigurationRoot configuration ) = Reloadable(new Dictionary<string, string?> { ["Web:BaseAddress"] = slow.Url.ToString() });

        using ( provider )
        {
            WebRequester                  requester = provider.GetRequiredService<WebRequester>();
            Task<WebResponse<string>>     inFlight  = requester.Get("x").AsString(CancellationToken.None).AsTask();

            while ( slow.Requests.Count == 0 ) { await Task.Delay(10); }

            configuration["Web:BaseAddress"] = fast.Url.ToString();
            configuration.Reload();

            WebResponse<string> next = await requester.Get("y").AsString(CancellationToken.None);
            release.SetResult();

            this.AreEqual("fast", next.Payload);
            this.AreEqual("slow", ( await inFlight ).Payload); // not cancelled by the reload
        }
    }

    [Test]
    public void Dispose_StopsReloading()
    {
        ( ServiceProvider provider, IConfigurationRoot configuration ) = Reloadable(new Dictionary<string, string?> { ["Web:BaseAddress"] = "https://one.test/" });

        using ( provider )
        {
            WebRequester requester = provider.GetRequiredService<WebRequester>();
            int          reloads   = 0;
            requester.Reloaded += _ => reloads++;

            requester.Dispose();
            configuration["Web:BaseAddress"] = "https://two.test/";
            configuration.Reload();

            this.AreEqual(0, reloads);
        }
    }


    private static SocketsHttpHandler HandlerOf( HttpClient client )
    {
        HttpMessageHandler handler = (HttpMessageHandler)typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(client)!;
        return (SocketsHttpHandler)( (DelegatingHandler)handler ).InnerHandler!;
    }
}
