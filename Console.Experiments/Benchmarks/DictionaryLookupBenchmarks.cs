// Jakar.Extensions :: Experiments
// 11/18/2023  10:37 PM

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Configs;



namespace Experiments.Benchmarks;
/*

   BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 8.0.100
   [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
   DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


   | Method                                | Items  | Mean             | Error          | StdDev         | Ratio    | RatioSD | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
   |-------------------------------------- |------- |-----------------:|---------------:|---------------:|---------:|--------:|---------:|---------:|---------:|-----------:|------------:|
   | ConstructDictionary                   | 10     |        229.76 ns |       4.526 ns |       4.843 ns | baseline |         |   0.0525 |        - |        - |      440 B |             |
   | ConstructReadOnlyDictionary           | 10     |        283.60 ns |       3.794 ns |       3.363 ns |     +23% |    1.9% |   0.0572 |        - |        - |      480 B |         +9% |
   | ConstructImmutableDictionary          | 10     |      1,269.14 ns |       5.701 ns |       4.760 ns |    +449% |    2.1% |   0.0839 |        - |        - |      712 B |        +62% |
   | ConstructFrozenDictionary             | 10     |      1,377.45 ns |      26.142 ns |      25.675 ns |    +498% |    2.0% |   0.2937 |        - |        - |     2472 B |       +462% |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | ConstructDictionary                   | 100    |      2,185.37 ns |      20.695 ns |      19.358 ns | baseline |         |   0.3738 |        - |        - |     3128 B |             |
   | ConstructReadOnlyDictionary           | 100    |      2,233.82 ns |      24.941 ns |      22.109 ns |      +2% |    1.1% |   0.3777 |        - |        - |     3168 B |         +1% |
   | ConstructImmutableDictionary          | 100    |     15,370.62 ns |      80.179 ns |      71.076 ns |    +604% |    1.2% |   0.7629 |        - |        - |     6472 B |       +107% |
   | ConstructFrozenDictionary             | 100    |     48,969.89 ns |     163.448 ns |     144.892 ns |  +2,142% |    0.9% |   2.8076 |   0.0610 |        - |    23504 B |       +651% |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | ConstructDictionary                   | 1000   |     22,723.81 ns |     190.066 ns |     177.788 ns | baseline |         |   3.6926 |        - |        - |    31016 B |             |
   | ConstructReadOnlyDictionary           | 1000   |     23,869.26 ns |     207.834 ns |     194.408 ns |      +5% |    1.0% |   3.6926 |   0.3357 |        - |    31056 B |         +0% |
   | ConstructImmutableDictionary          | 1000   |    248,178.92 ns |     582.843 ns |     516.675 ns |    +992% |    0.9% |   7.3242 |   0.9766 |        - |    64072 B |       +107% |
   | ConstructFrozenDictionary             | 1000   |    629,648.44 ns |   2,422.428 ns |   2,265.940 ns |  +2,671% |    0.9% |  30.2734 |  30.2734 |  30.2734 |   267661 B |       +763% |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | ConstructDictionary                   | 10000  |    269,127.95 ns |   3,165.665 ns |   2,961.165 ns | baseline |         |  68.3594 |  63.9648 |  63.4766 |   283501 B |             |
   | ConstructReadOnlyDictionary           | 10000  |    276,563.22 ns |   2,243.776 ns |   2,098.829 ns |      +3% |    1.3% |  68.8477 |  63.9648 |  63.9648 |   283540 B |         +0% |
   | ConstructImmutableDictionary          | 10000  |  3,270,118.95 ns |  14,477.608 ns |  12,834.022 ns |  +1,115% |    1.1% |  74.2188 |  35.1563 |        - |   640075 B |       +126% |
   | ConstructFrozenDictionary             | 10000  |  6,061,367.21 ns |  16,636.282 ns |  15,561.589 ns |  +2,152% |    1.1% | 390.6250 | 343.7500 | 328.1250 |  1788338 B |       +531% |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | ConstructDictionary                   | 100000 |  3,525,065.39 ns |  29,651.660 ns |  27,736.181 ns | baseline |         | 101.5625 | 101.5625 | 101.5625 |  3042497 B |             |
   | ConstructReadOnlyDictionary           | 100000 |  3,552,854.86 ns |  36,348.910 ns |  34,000.793 ns |      +1% |    1.2% | 105.4688 | 105.4688 | 105.4688 |  3042633 B |         +0% |
   | ConstructImmutableDictionary          | 100000 | 49,764,892.67 ns | 512,944.979 ns | 479,809.052 ns |  +1,312% |    1.2% | 700.0000 | 600.0000 |        - |  6400146 B |       +110% |
   | ConstructFrozenDictionary             | 100000 | 41,001,641.03 ns | 419,122.355 ns | 392,047.310 ns |  +1,063% |    1.2% | 307.6923 | 307.6923 | 307.6923 | 17326982 B |       +469% |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Found          | 10     |        149.93 ns |       0.808 ns |       0.756 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Found  | 10     |        162.60 ns |       0.294 ns |       0.230 ns |      +8% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Found | 10     |        332.22 ns |       1.375 ns |       1.286 ns |    +122% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Found    | 10     |         57.50 ns |       0.175 ns |       0.163 ns |     -62% |    0.5% |        - |        - |        - |          - |          NA |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Found          | 100    |      1,691.69 ns |       3.998 ns |       3.339 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Found  | 100    |      1,937.33 ns |       7.684 ns |       6.812 ns |     +15% |    0.3% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Found | 100    |      3,486.54 ns |       8.158 ns |       7.631 ns |    +106% |    0.3% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Found    | 100    |        649.06 ns |       1.928 ns |       1.610 ns |     -62% |    0.3% |        - |        - |        - |          - |          NA |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Found          | 1000   |     19,554.15 ns |      67.892 ns |      60.184 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Found  | 1000   |     21,296.33 ns |      58.429 ns |      51.795 ns |      +9% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Found | 1000   |     61,391.55 ns |     505.555 ns |     472.896 ns |    +214% |    0.8% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Found    | 1000   |      6,646.10 ns |      28.513 ns |      25.276 ns |     -66% |    0.6% |        - |        - |        - |          - |          NA |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Found          | 10000  |    235,203.39 ns |     418.919 ns |     349.816 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Found  | 10000  |    246,981.38 ns |   2,728.957 ns |   2,552.668 ns |      +5% |    1.1% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Found | 10000  |    976,226.50 ns |   2,109.698 ns |   1,973.413 ns |    +315% |    0.3% |        - |        - |        - |        1 B |          NA |
   | FrozenDictionary_TryGetValue_Found    | 10000  |    117,081.27 ns |     498.106 ns |     441.558 ns |     -50% |    0.4% |        - |        - |        - |          - |          NA |
   |                                       |        |                  |                |                |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Found          | 100000 |  3,227,624.94 ns |  20,943.157 ns |  17,488.485 ns | baseline |         |        - |        - |        - |        3 B |             |
   | ReadOnlyDictionary_TryGetValue_Found  | 100000 |  3,423,695.70 ns |  13,328.822 ns |  12,467.788 ns |      +6% |    0.6% |        - |        - |        - |        3 B |         +0% |
   | ImmutableDictionary_TryGetValue_Found | 100000 | 15,893,700.89 ns | 121,401.712 ns | 107,619.456 ns |    +393% |    1.1% |        - |        - |        - |       23 B |       +667% |
   | FrozenDictionary_TryGetValue_Found    | 100000 |  2,210,297.54 ns |  19,236.221 ns |  16,063.116 ns |     -32% |    0.8% |        - |        - |        - |        3 B |         +0% |
 */



[ Config( typeof(BenchmarkConfig) ), GroupBenchmarksBy( BenchmarkLogicalGroupRule.ByCategory ), SimpleJob( RuntimeMoniker.HostProcess ), MemoryDiagnoser, SuppressMessage( "ReSharper", "LoopCanBeConvertedToQuery" ) ]
public class DictionaryLookupBenchmarks
{
    private Dictionary<string, int>          _dictionary;
    private FrozenDictionary<string, int>    _frozenDictionary;
    private ImmutableDictionary<string, int> _immutableDictionary;
    private KeyValuePair<string, int>[]      _items;
    private ReadOnlyDictionary<string, int>  _readOnlyDictionary;
    private string[]                         _keys;

    [ Params( 10, 100, 1000, 10_000, 100_000 ) ] public int Items { get; set; }


    [ GlobalSetup ]
    public void GlobalSetup()
    {
        _items = Enumerable.Range( 0, Items ).Select( static _ => new KeyValuePair<string, int>( Guid.NewGuid().ToString(), Random.Shared.Next() ) ).ToArray();
        _keys  = _items.Select( k => k.Key ).ToArray();

        _dictionary          = new Dictionary<string, int>( _items );
        _readOnlyDictionary  = new ReadOnlyDictionary<string, int>( _items.ToDictionary( i => i.Key, i => i.Value ) );
        _immutableDictionary = _items.ToImmutableDictionary();
        _frozenDictionary    = _items.ToFrozenDictionary();
    }


    [ BenchmarkCategory( "Construct" ), Benchmark( Baseline = true ) ] public Dictionary<string, int>          ConstructDictionary()          => new(_items);
    [ BenchmarkCategory( "Construct" ), Benchmark ]                    public ReadOnlyDictionary<string, int>  ConstructReadOnlyDictionary()  => new(_items.ToDictionary( i => i.Key, i => i.Value ));
    [ BenchmarkCategory( "Construct" ), Benchmark ]                    public ImmutableDictionary<string, int> ConstructImmutableDictionary() => _items.ToImmutableDictionary();
    [ BenchmarkCategory( "Construct" ), Benchmark ]                    public FrozenDictionary<string, int>    ConstructFrozenDictionary()    => _items.ToFrozenDictionary();


    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark( Baseline = true ) ]
    public bool Dictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _dictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool ReadOnlyDictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _readOnlyDictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool ImmutableDictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _immutableDictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool FrozenDictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _frozenDictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }
}
