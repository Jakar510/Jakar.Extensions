#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;



namespace Jakar.Extensions.Tests.Http;

[Obsolete]
[TestFixture]
[SuppressMessage( "ReSharper", "SuggestVarOrType_SimpleTypes" )]
[SuppressMessage( "ReSharper", "SuggestVarOrType_BuiltInTypes" )]
[SuppressMessage( "ReSharper", "SuggestVarOrType_Elsewhere" )]
public class DeleteTests : UrlTestsCore
{
    [Test] [TestCase( "https://reqbin.com/echo/delete/json" )] public async Task Request( string url ) => await RequestCore( url );

    protected override async Task RequestFile( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection( MimeTypeNames.Application.JSON, _encoding );
        var result  = await url.TryDelete( WebResponses.AsFile, _Payload, headers, _encoding, token );
        result.FullPath.WriteToConsole();

        using (result) { True( result.Exists ); }
    }

    protected override async Task RequestMemory( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection( MimeTypeNames.Application.JSON, _encoding );
        var result  = await url.TryDelete( WebResponses.AsMemory, _Payload, headers, _encoding, token );

        True( result.Length > 0 );
    }

    protected override async Task RequestBytes( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection( MimeTypeNames.Application.JSON, _encoding );
        var result  = await url.TryDelete( WebResponses.AsBytes, _Payload, headers, _encoding, token );

        True( result.Length > 0 );
    }

    protected override async Task RequestStream( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection( MimeTypeNames.Application.JSON, _encoding );
        var result  = await url.TryDelete( WebResponses.AsStream, _Payload, headers, _encoding, token );

        True( await CheckResult( result, Encoding.Latin1 ) );
    }

    protected override async Task Request( Uri url, CancellationToken token )
    {
        var headers = new HeaderCollection( MimeTypeNames.Application.JSON, _encoding );
        var result  = await url.TryDelete( WebResponses.AsString, _Payload, headers, _encoding, token );

        False( string.IsNullOrWhiteSpace( result ) );
    }
}
