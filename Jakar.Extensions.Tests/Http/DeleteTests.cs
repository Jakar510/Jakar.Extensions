using System;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Http;
using NUnit.Framework;


namespace Jakar.Extensions.Tests.Http;


[TestFixture]
public class DeleteTests : UrlTests
{
    [Test]
    [TestCase("https://reqbin.com/echo/delete/json")]
    public async Task Request( string url ) => await Request(new Uri(url), _Token);

    protected override async Task Request( Uri url, CancellationToken token )
    {
        var    headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        string reply   = await url.TryDelete(WebResponses.AsString, _Payload, headers, _encoding, token);

        False(string.IsNullOrWhiteSpace(reply));
    }
}