using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Extensions.Models;
using Jakar.Extensions.Strings;
using NUnit.Framework;


namespace Jakar.Extensions.Tests.Http;


public abstract class UrlTests : Assert
{
    protected                 CancellationTokenSource? _source;
    protected                 CancellationToken        _Token => _source?.Token ?? throw new NullReferenceException(nameof(_source));
    protected static readonly AppVersion               _version = new(1, 0, 0);
    protected static          string                   _Payload => _version.ToJson();
    protected static readonly Encoding                 _encoding = Encoding.Default;


    protected abstract Task Request( Uri url, CancellationToken token );

    [SetUp]
    public void Setup() { _source = new CancellationTokenSource(TimeSpan.FromSeconds(30)); }

    [TearDown]
    public void Teardown()
    {
        _source?.Cancel();
        _source?.Dispose();
        _source = null;
    }
}