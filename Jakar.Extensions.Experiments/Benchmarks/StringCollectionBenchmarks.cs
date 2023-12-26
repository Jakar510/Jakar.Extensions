// Jakar.Extensions :: Experiments
// 08/24/2023  8:37 PM

namespace Jakar.Extensions.Experiments.Benchmarks;


/*
   BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 11 (10.0.22621.2428/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 7.0.402
   [Host]     : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2
   DefaultJob : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2


   | Method                    | Count  | Mean      | Error     | StdDev    | Rank | Allocated |
   |-------------------------- |------- |----------:|----------:|----------:|-----:|----------:|
   | CheckImmutableArraySpan   | 10     |  4.869 ns | 0.0593 ns | 0.0526 ns |    1 |         - |
   | CheckArraySpan            | 10     |  4.893 ns | 0.0774 ns | 0.0647 ns |    1 |         - |
   | CheckArraySpan            | 1000   |  5.158 ns | 0.0974 ns | 0.0863 ns |    2 |         - |
   | CheckImmutableArraySpan   | 100000 |  5.205 ns | 0.0673 ns | 0.0597 ns |    2 |         - |
   | CheckImmutableArraySpan   | 1000   |  5.211 ns | 0.1170 ns | 0.1094 ns |    2 |         - |
   | CheckImmutableArraySpan   | 10000  |  5.217 ns | 0.0987 ns | 0.0875 ns |    2 |         - |
   | CheckArraySpan            | 10000  |  5.261 ns | 0.1310 ns | 0.1225 ns |    2 |         - |
   | CheckArraySpan            | 100000 |  5.261 ns | 0.0719 ns | 0.0601 ns |    2 |         - |
   | CheckMemorySpan           | 10     |  6.397 ns | 0.0431 ns | 0.0382 ns |    3 |         - |
   | CheckMemorySpan           | 100000 |  6.738 ns | 0.1439 ns | 0.1346 ns |    4 |         - |
   | CheckMemorySpan           | 10000  |  6.803 ns | 0.1600 ns | 0.1712 ns |    4 |         - |
   | CheckMemorySpan           | 1000   |  6.834 ns | 0.1584 ns | 0.2115 ns |    4 |         - |
   | CheckImmutableArrayString | 10     |  7.391 ns | 0.0479 ns | 0.0449 ns |    5 |         - |
   | CheckImmutableArrayString | 10000  |  7.904 ns | 0.1738 ns | 0.1626 ns |    6 |         - |
   | CheckArrayString          | 10     |  7.963 ns | 0.0756 ns | 0.0590 ns |    6 |         - |
   | CheckImmutableArrayString | 100000 |  7.963 ns | 0.1819 ns | 0.2022 ns |    6 |         - |
   | CheckImmutableArrayString | 1000   |  7.982 ns | 0.1846 ns | 0.1813 ns |    6 |         - |
   | CheckArrayString          | 1000   |  8.440 ns | 0.1996 ns | 0.1960 ns |    7 |         - |
   | CheckArrayString          | 100000 |  8.540 ns | 0.1974 ns | 0.2027 ns |    7 |         - |
   | CheckMemoryString         | 1000   |  9.211 ns | 0.1494 ns | 0.1247 ns |    8 |         - |
   | CheckArrayString          | 10000  |  9.394 ns | 0.1699 ns | 0.1419 ns |    8 |         - |
   | CheckMemoryString         | 10     |  9.412 ns | 0.2175 ns | 0.2750 ns |    8 |         - |
   | CheckMemoryString         | 100000 | 10.125 ns | 0.1824 ns | 0.1523 ns |    9 |         - |
   | CheckMemoryString         | 10000  | 10.276 ns | 0.2321 ns | 0.3177 ns |    9 |         - |

*/



[ SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), RankColumn, MemoryDiagnoser ]
public class StringCollectionBenchmarks
{
    private ImmutableArray<string> _array;
    private int                    _count;
    private ReadOnlyMemory<string> _memory;
    private string[]               _values = GetArray( 10 );


    [ Params( 10, 1000, 10_000, 100_000 ) ]
    public int Count
    {
        get => _count;
        set
        {
            _count  = value;
            _values = GetArray( 10000 );
            _array  = ImmutableArray.Create( _values );
            _memory = _values;
        }
    }


    [ Benchmark ] public bool CheckArraySpan()            => AreSpanEqual( _values, _values );
    [ Benchmark ] public bool CheckArrayString()          => AreStringEqual( _values, _values );
    [ Benchmark ] public bool CheckImmutableArraySpan()   => AreSpanEqual( _array, _array );
    [ Benchmark ] public bool CheckImmutableArrayString() => AreStringEqual( _array, _array );
    [ Benchmark ] public bool CheckMemorySpan()           => AreSpanEqual( _memory, _memory );
    [ Benchmark ] public bool CheckMemoryString()         => AreStringEqual( _memory, _memory );


    private static bool AreStringEqual( in ReadOnlyMemory<string> left, in ReadOnlyMemory<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( string parameter in left.Span )
        {
            foreach ( string otherParameter in right.Span )
            {
                if ( string.Equals( parameter, otherParameter, StringComparison.Ordinal ) is false ) { return false; }
            }
        }

        return true;
    }
    private static bool AreSpanEqual( in ReadOnlyMemory<string> left, in ReadOnlyMemory<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( ReadOnlySpan<char> parameter in left.Span )
        {
            foreach ( ReadOnlySpan<char> otherParameter in right.Span )
            {
                if ( parameter.SequenceEqual( otherParameter ) is false ) { return false; }
            }
        }

        return true;
    }
    private static bool AreStringEqual( in ImmutableArray<string> left, in ImmutableArray<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( string parameter in left.AsSpan() )
        {
            foreach ( string otherParameter in right.AsSpan() )
            {
                if ( string.Equals( parameter, otherParameter, StringComparison.Ordinal ) is false ) { return false; }
            }
        }

        return true;
    }
    private static bool AreSpanEqual( in ImmutableArray<string> left, in ImmutableArray<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( ReadOnlySpan<char> parameter in left.AsSpan() )
        {
            foreach ( ReadOnlySpan<char> otherParameter in right.AsSpan() )
            {
                if ( parameter.SequenceEqual( otherParameter ) is false ) { return false; }
            }
        }

        return true;
    }
    private static bool AreStringEqual( in string[] left, in string[] right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( string parameter in left.AsSpan() )
        {
            foreach ( string otherParameter in right.AsSpan() )
            {
                if ( string.Equals( parameter, otherParameter, StringComparison.Ordinal ) is false ) { return false; }
            }
        }

        return true;
    }
    private static bool AreSpanEqual( in string[] left, in string[] right )
    {
        if ( left.Length != right.Length ) { return false; }

        foreach ( ReadOnlySpan<char> parameter in left.AsSpan() )
        {
            foreach ( ReadOnlySpan<char> otherParameter in right.AsSpan() )
            {
                if ( parameter.SequenceEqual( otherParameter ) is false ) { return false; }
            }
        }

        return true;
    }


    private static string[] GetArray( int count ) => GetValues( count ).ToArray( count );
    private static IEnumerable<string> GetValues( int count )
    {
        for ( int i = 0; i < count; i++ ) { yield return Randoms.RandomString( Random.Shared.Next( 10, 50 ) ); }
    }
}
