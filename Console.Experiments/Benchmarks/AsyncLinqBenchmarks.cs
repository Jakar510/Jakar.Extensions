// Jakar.Extensions :: Console.Experiments
// 09/15/2022  11:39 AM

namespace Experiments.Benchmarks;


/*
|         Method |      Max |     Mean |   Error |  StdDev | Rank |   Gen0 | Allocated |
|--------------- |--------- |---------:|--------:|--------:|-----:|-------:|----------:|
|      WhereTask |   100000 | 216.3 ns | 4.12 ns | 4.05 ns |    1 | 0.0544 |     456 B |
|      WhereTask | 10000000 | 218.5 ns | 2.75 ns | 2.82 ns |    1 | 0.0544 |     456 B |
|      WhereTask |      100 | 225.6 ns | 2.58 ns | 2.28 ns |    2 | 0.0544 |     456 B |
| WhereValueTask | 10000000 | 226.5 ns | 4.43 ns | 4.93 ns |    2 | 0.0458 |     384 B |
| WhereValueTask |      100 | 227.0 ns | 4.04 ns | 4.97 ns |    2 | 0.0458 |     384 B |
| WhereValueTask |   100000 | 233.2 ns | 4.70 ns | 6.59 ns |    3 | 0.0458 |     384 B |
 */



[ SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), RankColumn, MemoryDiagnoser ]
public class AsyncLinqBenchmarks
{
    // private readonly Dictionary<long, Guid> _dict = new();
    private static readonly AsyncEnumerator<long> _data = AsyncLinq.Range( 0L, 10_000 ).AsAsyncEnumerable();


    // [Benchmark]
    // public async Task<List<long>> WhereTask()
    // {
    //     var results = await _data.Where(x => x > 50)
    //                              .Where(x => x % 5 == 0)
    //                              .ToList();
    //
    //     results.Count.WriteToConsole();
    //     return results;
    // }
    [ Benchmark ] public ValueTask<List<long>> WhereValueTask() => _data.Where( x => x > 0 ).Where( x => x % 5 == 0 ).ToList();


    [ GlobalSetup ]
    public void Setup()
    {
        // for ( long i = 0; i < 10_000; i++ ) { _dict[i] = Guid.NewGuid(); }
    }

    // [Benchmark] public void Pairs() => _dict.ForEach(( KeyValuePair<long, Guid>           x ) => { });
    // [Benchmark] public void Keys() => _dict.ForEach(( long                                x ) => { });
    // [Benchmark] public void Values() => _dict.ForEach(( Guid                              x ) => { });
    // [Benchmark] public Task PairsAsync() => _dict.ForEachAsync(( KeyValuePair<long, Guid> x ) => x.TaskFromResult());
    // [Benchmark] public Task KeysAsync() => _dict.ForEachAsync(( long                      x ) => x.TaskFromResult());
    // [Benchmark] public Task ValuesAsync() => _dict.ForEachAsync(( Guid                    x ) => x.TaskFromResult());


    // [Benchmark]
    // public List<Guid> RandomValues() => _dict.RandomValues(Random.Shared)
    //                                          .Take(5)
    //                                          .ToList();
    // [Benchmark]
    // public List<long> RandomKeys() => _dict.RandomKeys(Random.Shared)
    //                                        .Take(5)
    //                                        .ToList();
}
