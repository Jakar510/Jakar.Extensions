using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Http;
using Jakar.Extensions.Models;
using Jakar.Extensions.Strings;
using NUnit.Framework;



#nullable enable
namespace Jakar.Extensions.Tests.Http;


public abstract class UrlTests : Assert
{
    protected                 CancellationTokenSource? _source;
    protected                 CancellationToken        _Token => _source?.Token ?? throw new NullReferenceException(nameof(_source));
    protected static readonly AppVersion               _version = new(1, 0, 0);
    protected static          string                   _Payload => _version.ToJson();
    protected static readonly Encoding                 _encoding = Encoding.Default;

    [SetUp] public void Setup() => _source = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    [TearDown]
    public void Teardown()
    {
        _source?.Cancel();
        _source?.Dispose();
        _source = null;
    }
}



public abstract class UrlTestsCore : UrlTests
{
    protected async Task RequestCore( string url ) => await RequestCore(new Uri(url), _Token);

    private async Task RequestCore( Uri link, CancellationToken token )
    {
        await Request(link, token);
        await RequestBytes(link, token);
        await RequestStream(link, token);
        await RequestMemory(link, token);
        await RequestFile(link, token);
    }


    protected abstract Task Request( Uri       url, CancellationToken token );
    protected abstract Task RequestStream( Uri url, CancellationToken token );
    protected abstract Task RequestBytes( Uri  url, CancellationToken token );
    protected abstract Task RequestMemory( Uri url, CancellationToken token );
    protected abstract Task RequestFile( Uri   url, CancellationToken token );


    protected static async Task<bool> CheckResult( Stream stream, Encoding encoding )
    {
        using var sr     = new StreamReader(stream ?? throw new NullReferenceException(nameof(stream)), encoding);
        string    result = await sr.ReadToEndAsync().ConfigureAwait(false);

        return string.IsNullOrWhiteSpace(result.Trim());
    }
}
