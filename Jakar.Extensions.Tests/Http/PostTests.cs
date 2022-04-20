using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Http;
using Jakar.Extensions.Strings;
using NUnit.Framework;


namespace Jakar.Extensions.Tests.Http;


[TestFixture]
[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
[SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
public class PostTests : UrlTestsCore
{
    [Test]
    [TestCase("https://reqbin.com/echo/post/json")]
    [TestCase("https://reqbin.com/echo/post/xml")]
    public async Task Request( string url ) => await RequestCore(url);

    protected override async Task RequestFile( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        var result  = await url.TryPost(WebResponses.AsFile, _Payload, headers, _encoding, token);
        result.FullPath.WriteToConsole();

        using ( result ) { True(result.Exists); }
    }

    protected override async Task RequestMemory( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        var result  = await url.TryPost(WebResponses.AsMemory, _Payload, headers, _encoding, token);

        True(result.Length > 0);
    }

    protected override async Task RequestBytes( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        var result  = await url.TryPost(WebResponses.AsBytes, _Payload, headers, _encoding, token);

        True(result.Length > 0);
    }

    protected override async Task RequestStream( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        var result  = await url.TryPost(WebResponses.AsStream, _Payload, headers, _encoding, token);

        True(await CheckResult(result, Encoding.Latin1));
    }

    protected override async Task Request( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection(MimeTypeNames.Application.JSON, _encoding);
        var result  = await url.TryPost(WebResponses.AsString, _Payload, headers, _encoding, token);

        False(string.IsNullOrWhiteSpace(result));
    }
}
