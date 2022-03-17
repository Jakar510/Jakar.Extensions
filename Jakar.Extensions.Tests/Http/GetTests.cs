using System;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Http;
using NUnit.Framework;


namespace Jakar.Extensions.Tests.Http;


[TestFixture]
public class GetTests : UrlTests
{
    [Test]
    [TestCase("http://google.com")]
    [TestCase("http://1.1.1.1")]
    [TestCase("https://reqbin.com/echo/get/json")]
    [TestCase("https://reqbin.com/echo/get/xml")]
    public async Task Request( string url ) => await Request(new Uri(url), _Token);

    protected override async Task Request( Uri url, CancellationToken token )
    {
        string reply = await url.TryGet(_encoding, token);

        False(string.IsNullOrWhiteSpace(reply));
    }
}