// Jakar.Extensions :: Experiments
// 08/20/2023  9:51 PM


namespace Jakar.Extensions.Experiments.Benchmarks;


/*
   BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 8.0.100
   [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
   DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


   | Method  | Mean       | Error   | StdDev  | Rank | Gen0   | Allocated |
   |-------- |-----------:|--------:|--------:|-----:|-------:|----------:|
   | GetHash |   859.6 ns | 3.31 ns | 3.09 ns |    1 | 0.0238 |     200 B |
   | GetLong | 1,246.8 ns | 6.15 ns | 5.14 ns |    2 | 0.0172 |     152 B |
*/



[JsonExporterAttribute.Full]
[MarkdownExporterAttribute.GitHub]
[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
public class HashBenchmarks
{
    private const    string  HASH     = "xOEDj6A8g15z";
    private const    long    VALUE    = 69;
    private readonly Hashids __hasher = new("49C7BCFE-D7B4-46FA-BF33-5B1D1032339A", 12);


    [Benchmark] public long   GetLong() => __hasher.DecodeSingleLong(HASH);
    [Benchmark] public string GetHash() => __hasher.EncodeLong(VALUE);
}
