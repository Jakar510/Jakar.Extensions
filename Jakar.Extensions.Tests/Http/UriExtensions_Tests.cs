// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(UriExtensions))]
public class UriExtensions_Tests : Assert
{
    [TestCase("https://h/api/", "https://h/api/users/1")]
    [TestCase("https://h/api",  "https://h/api/users/1")]
    [TestCase("https://h/",     "https://h/users/1")]
    public void Paths_AreAppendedToTheBasePath( string baseUri, string expected ) => this.AreEqual(expected, baseUri.GetRoute("users", "1").ToString());

    [Test]
    public void Paths_AreEscaped_AndSlashesSplitLevels()
    {
        this.AreEqual("https://h/a/b/c%20d", "https://h/".GetRoute("a/b", "c d").AbsoluteUri);
        this.AreEqual("https://h/x",         "https://h/".GetRoute("/x/", "", "  ").AbsoluteUri);
    }

    [Test]
    public void Query_UsesAmpersands_AndEscapes()
    {
        Dictionary<string, object?> parameters = new() { ["a"] = 1, ["b"] = "x y&z=1", ["c d"] = true };
        this.AreEqual("https://h/s?a=1&b=x%20y%26z%3D1&c%20d=True", "https://h/s".GetRoute(parameters).AbsoluteUri);
    }

    [Test]
    public void Query_SkipsBlankAndMeaninglessValues()
    {
        Dictionary<string, object?> parameters = new() { ["a"] = null, ["b"] = " ", [" "] = "x", ["c"] = new object(), ["d"] = "kept" };
        this.AreEqual("?d=kept", parameters.Parameterize());
        this.AreEqual("",        new Dictionary<string, object?> { ["a"] = null }.Parameterize());
    }

    [Test]
    public void PathsAreKept_WhenThereAreNoParameters() => this.AreEqual("https://h/v1/items", "https://h/v1".GetRoute(new Dictionary<string, object?>(), "items").AbsoluteUri);

    [Test]
    public void ExistingQueryAndFragment_AreKept()
    {
        Uri route = "https://h/p?x=1#frag".GetRoute(new Dictionary<string, object?> { ["y"] = 2 }, "q");
        this.AreEqual("https://h/p/q?x=1&y=2#frag", route.AbsoluteUri);
    }

    [Test]
    public void Values_UseTheInvariantCulture()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

        try { this.AreEqual("?v=1.5", new Dictionary<string, object?> { ["v"] = 1.5 }.Parameterize()); }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    [Test]
    public void SpanAndStringBuilderForms()
    {
        ReadOnlySpan<string> paths = ["a", "b c"];
        this.AreEqual("/a/b%20c",     paths.Parameterize());
        this.AreEqual("/a/b%20c?k=v", paths.Parameterize(new Dictionary<string, object?> { ["k"] = "v" }));

        StringBuilder sb = new("x");
        sb.Parameterize(new Dictionary<string, object?> { ["k"] = "v", ["j"] = 2 });
        this.AreEqual("x?k=v&j=2", sb.ToString());
    }

    [Test]
    public void NoPathsOrParameters_ReturnsTheBase()
    {
        Uri baseUri = new("https://h/p");
        Assert.AreSame(baseUri, baseUri.GetRoute());
        Assert.AreSame(baseUri, baseUri.GetRoute(new Dictionary<string, object?>()));
    }
}



[TestFixture]
[TestOf(typeof(HeaderCollection))]
public class HeaderCollection_Tests : Assert
{
    [Test]
    public void ContentTypeWithEncoding_UsesTheCharsetParameter()
    {
        HeaderCollection headers = new(MimeTypeNames.Application.JSON, Encoding.UTF8);

        this.AreEqual("application/json; charset=utf-8", headers.ContentType);
        this.AreEqual(Encoding.UTF8.WebName,             headers.Encoding?.WebName);
        this.IsNull(headers.ContentEncoding);
        this.IsFalse(headers.ContainsKey("utf-8"));
        this.IsTrue(headers.ContainsKey("content-type")); // real name, case-insensitive
    }

    [Test]
    public void EncodingWithoutContentType_DefaultsToTextPlain()
    {
        HeaderCollection headers = new() { Encoding = Encoding.Unicode };
        this.AreEqual("text/plain; charset=utf-16", headers.ContentType);
    }

    [Test]
    public void HttpRequestHeader_MapsToWireNames()
    {
        HeaderCollection headers = new();
        headers.Add(HttpRequestHeaderNames_Probe.UserAgent, "tests").Add(HttpRequestHeaderNames_Probe.IfNoneMatch, "\"e1\"");

        this.IsTrue(headers.ContainsKey("User-Agent"));
        this.IsTrue(headers.ContainsKey("If-None-Match"));
    }

    [Test]
    public void Merge_RoutesContentHeadersToTheContent_AndReplaces()
    {
        using System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Post, "https://h/") { Content = new System.Net.Http.StringContent("x") };
        request.Headers.TryAddWithoutValidation("X-Custom", "old");

        new HeaderCollection(MimeTypeNames.Application.JSON, Encoding.UTF8) { ["X-Custom"] = "new", ["X-List"] = new[] { "a", "b" }, ["X-Number"] = 1.5 }.Merge(request);

        this.AreEqual("application/json; charset=utf-8", request.Content.Headers.ContentType!.ToString());
        this.AreEqual("new",                             string.Join(",", request.Headers.GetValues("X-Custom")));
        this.AreEqual("a,b",                             string.Join(",", request.Headers.GetValues("X-List")));
        this.AreEqual("1.5",                             string.Join(",", request.Headers.GetValues("X-Number")));
    }



    private static class HttpRequestHeaderNames_Probe
    {
        public const System.Net.HttpRequestHeader UserAgent   = System.Net.HttpRequestHeader.UserAgent;
        public const System.Net.HttpRequestHeader IfNoneMatch = System.Net.HttpRequestHeader.IfNoneMatch;
    }
}
