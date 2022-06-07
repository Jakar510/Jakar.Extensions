using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Http;
using Jakar.Extensions.Strings;
using NUnit.Framework;


#nullable enable
namespace Jakar.Extensions.Tests.Http;


[TestFixture]
[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
[SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
public class GetTests : UrlTestsCore
{
    [Test]
    [TestCase("http://google.com")]
    [TestCase("http://1.1.1.1")]
    [TestCase("https://reqbin.com/echo/get/json")]
    [TestCase("https://reqbin.com/echo/get/xml")]
    public async Task Request( string url ) => await RequestCore(url);

    protected override async Task RequestFile( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.URL_ENCODED_CONTENT, _encoding);
        var result  = await url.TryGet(WebResponses.AsFile, headers, token);
        result.FullPath.WriteToConsole();

        using ( result ) { True(result.Exists); }
    }

    protected override async Task RequestMemory( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.URL_ENCODED_CONTENT, _encoding);
        var result  = await url.TryGet(WebResponses.AsMemory, headers, token);

        True(result.Length > 0);
    }

    protected override async Task RequestBytes( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.URL_ENCODED_CONTENT, _encoding);
        var result  = await url.TryGet(WebResponses.AsBytes, headers, token);

        True(result.Length > 0);
    }

    protected override async Task RequestStream( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.URL_ENCODED_CONTENT, _encoding);
        var result  = await url.TryGet(WebResponses.AsStream, headers, token);

        True(await CheckResult(result, Encoding.Latin1));
    }

    protected override async Task Request( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.URL_ENCODED_CONTENT, _encoding);
        var result  = await url.TryGet(_encoding, token, headers);

        False(string.IsNullOrWhiteSpace(result));
    }
}
