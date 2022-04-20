// Jakar.Extensions :: Jakar.Extensions.Tests
// 04/20/2022  4:34 PM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Exceptions.Networking;
using Jakar.Extensions.Http;
using Jakar.Extensions.Models;
using Jakar.Extensions.Strings;
using NUnit.Framework;



namespace Jakar.Extensions.Tests.Http;


[TestFixture]
public sealed class ErrorTests : UrlTests
{
    [Test]
    [TestCase("http://httpstat.us/400")]
    public void Request( string url )
    {
        async Task TryRequest() => await RequestError(new Uri(url), _Token);
        ThrowsAsync<ProtocolErrorException>(TryRequest);
    }


    private static async Task RequestError( Uri url, CancellationToken token )
    {
        var    headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        string result  = await url.TryGet(WebResponses.AsString, _encoding, headers, token);

        False(string.IsNullOrWhiteSpace(result));
    }
}
