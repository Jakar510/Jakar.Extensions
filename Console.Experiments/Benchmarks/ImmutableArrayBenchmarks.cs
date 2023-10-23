// Jakar.Extensions :: Experiments
// 08/26/2023  2:02 PM

using BenchmarkDotNet.Engines;



namespace Experiments.Benchmarks;


/*
   BenchmarkDotNet v0.13.7, Windows 11 (10.0.22621.2134/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 7.0.400
   [Host]     : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
   DefaultJob : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2


|       Method |  Size |           Mean |         Error |        StdDev |    Gen0 |    Gen1 | Allocated |
|------------- |------ |---------------:|--------------:|--------------:|--------:|--------:|----------:|
| ForEachArray |    10 |       7.918 ns |     0.0661 ns |     0.0552 ns |       - |       - |         - |
|     ForArray |    10 |       7.937 ns |     0.0420 ns |     0.0372 ns |       - |       - |         - |
|      ForList |    10 |      36.903 ns |     0.2696 ns |     0.2522 ns |       - |       - |         - |
|  SelectArray |    10 |     100.080 ns |     1.1859 ns |     0.9902 ns |  0.0057 |       - |      48 B |
|  ForEachList |    10 |     158.456 ns |     0.4125 ns |     0.3657 ns |       - |       - |         - |
|  GetNewArray |    10 |     165.574 ns |     2.4416 ns |     2.1644 ns |  0.0229 |       - |     192 B |
| ForEachArray |  1000 |     246.804 ns |     0.7225 ns |     0.6405 ns |       - |       - |         - |
|     ForArray |  1000 |     248.102 ns |     2.2041 ns |     1.9539 ns |       - |       - |         - |
|   GetNewList |    10 |     363.051 ns |     6.6313 ns |     5.8785 ns |  0.0868 |       - |     728 B |
|   SelectList |    10 |     371.383 ns |     2.4624 ns |     2.3033 ns |  0.0153 |       - |     128 B |
| ForEachArray | 10000 |   2,355.587 ns |     8.4736 ns |     7.9262 ns |       - |       - |         - |
|     ForArray | 10000 |   2,364.278 ns |    11.0070 ns |     9.1914 ns |       - |       - |         - |
|  SelectArray |  1000 |   5,496.155 ns |    45.9745 ns |    43.0046 ns |       - |       - |      48 B |
|  ForEachList |  1000 |  10,442.248 ns |    23.6994 ns |    19.7900 ns |       - |       - |         - |
|  GetNewArray |  1000 |  10,813.844 ns |   203.7906 ns |   190.6259 ns |  0.9613 |       - |    8112 B |
|      ForList |  1000 |  15,227.748 ns |   278.7159 ns |   320.9696 ns |       - |       - |         - |
|   GetNewList |  1000 |  25,015.520 ns |   322.6518 ns |   301.8087 ns |  6.6833 |  0.1831 |   56168 B |
|   SelectList |  1000 |  25,919.353 ns |   230.1193 ns |   215.2538 ns |       - |       - |     128 B |
|  SelectArray | 10000 |  54,336.898 ns |   129.1159 ns |   114.4579 ns |       - |       - |      48 B |
|  GetNewArray | 10000 | 107,276.660 ns |   370.0046 ns |   346.1026 ns |  9.5215 |       - |   80112 B |
|  ForEachList | 10000 | 114,763.675 ns |   756.0200 ns |   707.1816 ns |       - |       - |         - |
|   GetNewList | 10000 | 251,535.526 ns | 3,473.8447 ns | 3,079.4728 ns | 66.4063 | 14.6484 |  560168 B |
|   SelectList | 10000 | 271,561.805 ns | 5,427.2554 ns | 5,573.3917 ns |       - |       - |     128 B |
|      ForList | 10000 | 311,052.326 ns | 2,776.6904 ns | 2,461.4637 ns |       - |       - |         - |
 */



[ SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), MemoryDiagnoser ]

// [RankColumn]
public class ImmutableArrayBenchmarks
{
    private static readonly Random _random = new(69);
    private static readonly Dictionary<int, ImmutableArray<double>> _array = new()
                                                                             {
                                                                                 GetArray( 10 ),
                                                                                 GetArray( 1000 ),
                                                                                 GetArray( 10_000 )
                                                                             };
    private static readonly Dictionary<int, ImmutableList<double>> _list = new()
                                                                           {
                                                                               GetList( 10 ),
                                                                               GetList( 1000 ),
                                                                               GetList( 10_000 )
                                                                           };
    private static readonly Consumer _consumer = new();


    [ Params( 10, 1000, 10_000 ) ] public int Size { get; set; }

    private static KeyValuePair<int, ImmutableArray<double>> GetArray( int size )
    {
        ImmutableArray<double> array = ImmutableArray.CreateRange( Enumerable.Range( 0, size )
                                                                             .Select( i => _random.NextDouble() ) );

        return new KeyValuePair<int, ImmutableArray<double>>( size, array );
    }
    private static KeyValuePair<int, ImmutableList<double>> GetList( int size )
    {
        ImmutableList<double> array = ImmutableList.CreateRange( Enumerable.Range( 0, size )
                                                                           .Select( i => _random.NextDouble() ) );

        return new KeyValuePair<int, ImmutableList<double>>( size, array );
    }


    [ Benchmark ]
    public ImmutableArray<double> GetNewArray() => ImmutableArray.CreateRange( Enumerable.Range( 0, Size )
                                                                                         .Select( i => _random.NextDouble() ) );

    [ Benchmark ]
    public ImmutableList<double> GetNewList() => ImmutableList.CreateRange( Enumerable.Range( 0, Size )
                                                                                      .Select( i => _random.NextDouble() ) );


    [ Benchmark ]
    public void ForArray()
    {
        ImmutableArray<double> array = _array[Size];

        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < array.Length; i++ ) { _ = array[i]; }
    }
    [ Benchmark ]
    public void ForEachArray()
    {
        ImmutableArray<double> array = _array[Size];

        foreach ( double d in array ) { _ = d; }
    }


    [ Benchmark ]
    public void ForList()
    {
        ImmutableList<double> array = _list[Size];

        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < array.Count; i++ ) { _ = array[i]; }
    }
    [ Benchmark ]
    public void ForEachList()
    {
        ImmutableList<double> array = _list[Size];
        foreach ( double d in array ) { _ = d; }
    }


    [ Benchmark ]
    public void SelectArray()
    {
        ImmutableArray<double> array = _array[Size];

        array.Select( i => i )
             .Consume( _consumer );
    }
    [ Benchmark ]
    public void SelectList()
    {
        ImmutableList<double> array = _list[Size];

        array.Select( i => i )
             .Consume( _consumer );
    }
}
