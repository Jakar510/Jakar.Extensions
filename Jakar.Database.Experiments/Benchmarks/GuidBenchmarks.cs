// Jakar.Extensions :: Console.Experiments
// 04/25/2022  9:42 PM

namespace Jakar.Database.Experiments.Benchmarks;
#pragma warning disable CA1822



/*
|      Method |      Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|------------ |----------:|----------:|----------:|-----:|-------:|----------:|
|    AsMemory |  7.389 ns | 0.0413 ns | 0.0386 ns |    1 |      - |         - |
| StringParse | 26.424 ns | 0.2038 ns | 0.1806 ns |    2 |      - |         - |
|    ToString | 30.100 ns | 0.3024 ns | 0.2681 ns |    3 | 0.0114 |      96 B |
|    AsBase64 | 34.268 ns | 0.6563 ns | 0.6139 ns |    4 | 0.0086 |      72 B |
|   SpanParse | 44.538 ns | 0.3594 ns | 0.3186 ns |    5 |      - |         - |
 */
[SimpleJob( RuntimeMoniker.HostProcess )]
[Orderer( SummaryOrderPolicy.FastestToSlowest )]
[RankColumn]
[MemoryDiagnoser]
public class GuidBenchmarks
{
    private const           string GUID  = "0365BC9B-3DE3-4B75-9F7E-2A0F23EFA5A2";
    private static readonly Guid   _guid = Guid.Parse( GUID );
    private static readonly string _b64  = _guid.ToBase64();


    [Benchmark]
    public ReadOnlySpan<byte> TryWriteBytes() => _guid.TryWriteBytes( out ReadOnlySpan<byte> memory )
                                                     ? memory
                                                     : default;
    [Benchmark] public     Guid    StringParse() => Guid.Parse( GUID );
    [Benchmark] public     Guid?   SpanParse()   => _b64.AsGuid();
    [Benchmark] public     string  AsBase64()    => _guid.ToBase64();
    [Benchmark] public new string? ToString()    => _guid.ToString();
}
#pragma warning restore CA1822
