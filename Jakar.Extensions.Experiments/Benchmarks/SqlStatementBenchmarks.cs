// using CommunityToolkit.HighPerformance;
// using CommunityToolkit.HighPerformance.Enumerables;


namespace Jakar.Extensions.Experiments.Benchmarks;


/*
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1413/22H2/2022Update/SunValley2)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


|                  Method |       Mean |     Error |    StdDev | Rank |   Gen0 | Allocated |
|------------------------ |-----------:|----------:|----------:|-----:|-------:|----------:|
|      Test_GetEnumerator |   8.568 ns | 0.1966 ns | 0.2487 ns |    1 | 0.0048 |      40 B |
| Test_ValueStringBuilder |  24.925 ns | 0.1898 ns | 0.1776 ns |    2 |      - |         - |
|       Test_Interpolated | 271.129 ns | 2.2262 ns | 1.9735 ns |    3 | 0.0372 |     312 B |
|      Test_StringBuilder | 292.134 ns | 3.9277 ns | 3.4818 ns |    4 | 0.0744 |     624 B |
|               Test_Join | 321.852 ns | 1.2302 ns | 1.0273 ns |    5 | 0.0095 |      80 B |
|                Test_VSB | 452.029 ns | 1.7853 ns | 1.5826 ns |    6 | 0.0095 |      80 B |
|               Test_Span | 481.976 ns | 2.8076 ns | 2.6262 ns |    7 | 0.0277 |     232 B |
 */



[JsonExporterAttribute.Full]
[SimpleJob(RuntimeMoniker.HostProcess)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[MemoryDiagnoser]
public class SqlStatementBenchmarks
{
    public IEnumerable<long> Ids
    {
        get
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( int i in Enumerable.Range(1, 10) ) { yield return i; }
        }
    }
    public string TableName { get; set; } = nameof(TableName);


    [Benchmark] public void Test_ValueStringBuilder()
    {
        using ValueStringBuilder sb = new();
    }


    [Benchmark] public void Test_Join()
    {
        using ValueStringBuilder sb = new();
        sb.AppendJoin(", ", Ids);
    }


    [Benchmark] public void Test_GetEnumerator()
    {
        using IEnumerator<long> sb = Ids.GetEnumerator();
    }


    [Benchmark] public ReadOnlySpan<char> Test_VSB()
    {
        using ValueStringBuilder sb = new("DELETE FROM ");
        sb.Append(TableName);
        sb.Append("WHERE ID in ( ");
        sb.AppendJoin(", ", Ids);
        sb.Append(" )");
        return sb.Result;
    }


    [Benchmark] public string Test_Span() => Test_VSB()
       .ToString();


    [Benchmark] public string Test_Interpolated() => $"DELETE FROM {TableName} WHERE ID in ( {string.Join(',', Ids)} )";


    [Benchmark] public string Test_StringBuilder()
    {
        StringBuilder sb = new($"DELETE FROM {TableName} WHERE ID in ( ");
        sb.AppendJoin(", ", Ids);
        sb.Append(" )");
        return sb.ToString();
    }
}
