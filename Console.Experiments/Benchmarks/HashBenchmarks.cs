// Jakar.Extensions :: Experiments
// 08/20/2023  9:51 PM

using HashidsNet;



namespace Experiments.Benchmarks;

/*
|  Method |       Mean |   Error |  StdDev | Rank |   Gen0 | Allocated |
|-------- |-----------:|--------:|--------:|-----:|-------:|----------:|
| GetHash |   881.6 ns | 8.81 ns | 8.24 ns |    1 | 0.0238 |     200 B |
| GetLong | 1,307.2 ns | 7.26 ns | 6.06 ns |    2 | 0.0172 |     152 B |
*/

[SimpleJob( RuntimeMoniker.HostProcess )]
[Orderer( SummaryOrderPolicy.FastestToSlowest )]
[RankColumn]
[MemoryDiagnoser]
public class HashBenchmarks
{
    private readonly Hashids _hasher = new("49C7BCFE-D7B4-46FA-BF33-5B1D1032339A", 12);
    private const    long    VALUE   = 69;
    private const    string  HASH    = "xOEDj6A8g15z";


    [Benchmark] public long GetLong() => _hasher.DecodeSingleLong( HASH );
    [Benchmark] public string GetHash() => _hasher.EncodeLong( VALUE );
}