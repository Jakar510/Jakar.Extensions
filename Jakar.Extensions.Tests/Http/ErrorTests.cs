// Jakar.Extensions :: Jakar.Extensions.Tests
// 04/20/2022  4:34 PM

#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;



namespace Jakar.Extensions.Tests.Http;


[Obsolete]
[TestFixture]
public sealed class ErrorTests : UrlTests
{
    [Test]
    [TestCase( "http://httpstat.us/400" )]
    public void Request( string url )
    {
        async Task TryRequest() => await RequestError( new Uri( url ), _Token );

        ThrowsAsync<ProtocolErrorException>( TryRequest );
    }


    private static async Task RequestError( Uri url, CancellationToken token )
    {
        var    headers = new HeaderCollection( MimeTypeNames.Application.JSON, _encoding );
        string result  = await url.TryGet( WebResponses.AsString, _encoding, headers, token );

        False( string.IsNullOrWhiteSpace( result ) );
    }
}
