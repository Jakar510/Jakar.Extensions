// TrueLogic :: Experiments
// 03/29/2023  5:39 PM

using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;



namespace Experiments.Benchmarks;


/*
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1413/22H2/2022Update/SunValley2)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=7.0.202
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


|             Method |     Mean |   Error |  StdDev | Rank |   Gen0 | Allocated |
|------------------- |---------:|--------:|--------:|-----:|-------:|----------:|
|          Test_Join | 213.4 ns | 1.37 ns | 1.21 ns |    1 | 0.0210 |     176 B |
|  Test_Interpolated | 261.4 ns | 3.22 ns | 3.02 ns |    2 | 0.0372 |     312 B |
| Test_StringBuilder | 271.1 ns | 5.35 ns | 7.67 ns |    3 | 0.0725 |     608 B |
 */



[SimpleJob( RuntimeMoniker.HostProcess )]
[Orderer( SummaryOrderPolicy.FastestToSlowest )]
[RankColumn]
[MemoryDiagnoser]
public class SqlStatementBenchmarks
{
    private string TableName { get; set; } = "Users";
    private IEnumerable<long> ids
    {
        get
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach ( int i in Enumerable.Range( 1, 10 ) ) { yield return i; }
        }
    }


    [Benchmark]
    public string Test_Span()
    {
        Span<long> span = stackalloc long[ids.Count()];
        foreach ( (int i, long item) in ids.Enumerate( 0 ) ) { span[i] = item; }

        // var sb = new ValueStringBuilder( span.Length * 20 + span.Length * 2 + 1 );
        // sb.AppendJoin();


        Span<char> result     = stackalloc char[span.Length * 20 + span.Length * 2 + 1];
        int        index      = 0;
        var        enumerator = span.GetEnumerator();
        enumerator.MoveNext();

        do
        {
            if ( enumerator.Current.TryFormat( result[index..], out int charsWritten ) ) { index += charsWritten; }

            result[++index] = ',';
            result[++index] = '0';
        } while ( enumerator.MoveNext() );

        return result.ToString();
    }

    [Benchmark] public string Test_Interpolated() => $"DELETE FROM {TableName} WHERE ID in ( {string.Join( ',', ids )} )";


    [Benchmark]
    public string Test_StringBuilder()
    {
        var sb = new StringBuilder( $"DELETE FROM {TableName} WHERE ID in ( " );
        sb.AppendJoin( ',', ids );
        sb.Append( " )" );
        return sb.ToString();
    }
}
