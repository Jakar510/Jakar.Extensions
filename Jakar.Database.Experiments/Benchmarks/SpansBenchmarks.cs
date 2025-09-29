// Jakar.Extensions :: Console.Experiments
// 05/10/2022  10:26 AM

#pragma warning disable IDE0302 // Collection init can be simplified



namespace Jakar.Database.Experiments.Benchmarks;


/*
   BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 8.0.100
   [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
   DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


   | Method             | Value                | Mean        | Error     | StdDev    | Median      | Rank | Allocated |
   |------------------- |--------------------- |------------:|----------:|----------:|------------:|-----:|----------:|
   | RemoveAll_Params   |                      |          NA |        NA |        NA |          NA |    ? |        NA |
   | RemoveAll_Single   |                      |          NA |        NA |        NA |          NA |    ? |        NA |
   | Replace            | abcde(...)vwxyz [26] |          NA |        NA |        NA |          NA |    ? |        NA |
   | Slice_False        | abcde(...)vwxyz [26] |          NA |        NA |        NA |          NA |    ? |        NA |
   | Slice_True         | abcde(...)vwxyz [26] |          NA |        NA |        NA |          NA |    ? |        NA |
   | EndsWith           |                      |   0.0071 ns | 0.0076 ns | 0.0067 ns |   0.0039 ns |    1 |         - |
   | AsSpan             | abcde(...)vwxyz [26] |   0.0156 ns | 0.0070 ns | 0.0065 ns |   0.0129 ns |    2 |         - |
   | AsSpan             |                      |   0.0204 ns | 0.0051 ns | 0.0048 ns |   0.0187 ns |    3 |         - |
   | AsSpan             | abcde(...)FA5A2 [64] |   0.0241 ns | 0.0087 ns | 0.0081 ns |   0.0201 ns |    3 |         - |
   | StartsWith         | abcde(...)vwxyz [26] |   0.2096 ns | 0.0097 ns | 0.0086 ns |   0.2078 ns |    4 |         - |
   | StartsWith         |                      |   0.2347 ns | 0.0019 ns | 0.0016 ns |   0.2346 ns |    5 |         - |
   | StartsWith         | abcde(...)FA5A2 [64] |   0.2441 ns | 0.0137 ns | 0.0121 ns |   0.2377 ns |    5 |         - |
   | EndsWith           | abcde(...)FA5A2 [64] |   0.4007 ns | 0.0099 ns | 0.0088 ns |   0.4039 ns |    6 |         - |
   | EndsWith           | abcde(...)vwxyz [26] |   0.4063 ns | 0.0105 ns | 0.0093 ns |   0.4039 ns |    6 |         - |
   | Contains_span      |                      |   1.4179 ns | 0.0150 ns | 0.0140 ns |   1.4164 ns |    7 |         - |
   | IsNullOrWhiteSpace |                      |   1.6110 ns | 0.0104 ns | 0.0093 ns |   1.6107 ns |    8 |         - |
   | Contains_span      | abcde(...)vwxyz [26] |   1.8265 ns | 0.0148 ns | 0.0138 ns |   1.8214 ns |    9 |         - |
   | IsNullOrWhiteSpace | abcde(...)FA5A2 [64] |   2.1009 ns | 0.0178 ns | 0.0166 ns |   2.0966 ns |   10 |         - |
   | IsNullOrWhiteSpace | abcde(...)vwxyz [26] |   2.3368 ns | 0.0798 ns | 0.2341 ns |   2.2570 ns |   11 |         - |
   | Contains_span      | abcde(...)FA5A2 [64] |   2.5321 ns | 0.0132 ns | 0.0110 ns |   2.5301 ns |   12 |         - |
   | Contains_value     |                      |   3.4902 ns | 0.0213 ns | 0.0199 ns |   3.4866 ns |   13 |         - |
   | Slice_False        |                      |   3.7527 ns | 0.0533 ns | 0.0498 ns |   3.7441 ns |   14 |         - |
   | Slice_True         |                      |   3.7545 ns | 0.0324 ns | 0.0270 ns |   3.7616 ns |   14 |         - |
   | Contains_value     | abcde(...)vwxyz [26] |   5.1589 ns | 0.0088 ns | 0.0073 ns |   5.1589 ns |   15 |         - |
   | Slice_False        | abcde(...)FA5A2 [64] |   7.1880 ns | 0.0395 ns | 0.0350 ns |   7.1809 ns |   16 |         - |
   | Slice_True         | abcde(...)FA5A2 [64] |   7.3984 ns | 0.1436 ns | 0.1764 ns |   7.3304 ns |   17 |         - |
   | Join               |                      |   9.8892 ns | 0.0286 ns | 0.0268 ns |   9.8829 ns |   18 |         - |
   | Replace            |                      |   9.9447 ns | 0.0498 ns | 0.0416 ns |   9.9396 ns |   18 |         - |
   | Join               | abcde(...)vwxyz [26] |  10.8704 ns | 0.0612 ns | 0.0478 ns |  10.8714 ns |   19 |         - |
   | ContainsAny        | abcde(...)FA5A2 [64] |  11.2435 ns | 0.0869 ns | 0.0813 ns |  11.2348 ns |   20 |         - |
   | ContainsNone       | abcde(...)FA5A2 [64] |  11.2620 ns | 0.1026 ns | 0.0960 ns |  11.2604 ns |   20 |         - |
   | ContainsAny        |                      |  12.2297 ns | 0.1214 ns | 0.1136 ns |  12.2281 ns |   21 |         - |
   | Contains_value     | abcde(...)FA5A2 [64] |  12.2374 ns | 0.0750 ns | 0.0664 ns |  12.2206 ns |   21 |         - |
   | ContainsNone       |                      |  12.2901 ns | 0.1715 ns | 0.1605 ns |  12.2819 ns |   21 |         - |
   | Join               | abcde(...)FA5A2 [64] |  13.2329 ns | 0.0453 ns | 0.0354 ns |  13.2366 ns |   22 |         - |
   | ContainsAny        | abcde(...)vwxyz [26] |  15.0015 ns | 0.0787 ns | 0.0736 ns |  15.0313 ns |   23 |         - |
   | ContainsNone       | abcde(...)vwxyz [26] |  15.0596 ns | 0.1511 ns | 0.1413 ns |  15.0790 ns |   23 |         - |
   | RemoveAll_Single   | abcde(...)vwxyz [26] |  21.7163 ns | 0.3174 ns | 0.2969 ns |  21.7303 ns |   24 |         - |
   | RemoveAll_Single   | abcde(...)FA5A2 [64] |  41.1281 ns | 0.8482 ns | 1.0416 ns |  40.9053 ns |   25 |         - |
   | RemoveAll_Params   | abcde(...)vwxyz [26] |  73.5365 ns | 1.5021 ns | 2.7842 ns |  72.0227 ns |   26 |         - |
   | Replace            | abcde(...)FA5A2 [64] | 115.7890 ns | 0.3652 ns | 0.2851 ns | 115.8691 ns |   27 |         - |
   | RemoveAll_Params   | abcde(...)FA5A2 [64] | 171.1779 ns | 1.9381 ns | 1.8129 ns | 171.1040 ns |   28 |         - |

 */



[SimpleJob( RuntimeMoniker.HostProcess ), Orderer( SummaryOrderPolicy.FastestToSlowest ), RankColumn, MemoryDiagnoser]
public class SpansBenchmarks
{
    private const string ALPHABET        = Randoms.ALPHANUMERIC;
    private const string EMPTY           = "        ";
    private const string EMPTY_WITH_TABS = $"\t\t{EMPTY}";
    private const string NEW_VALUE       = "----NEW-VALUE";
    private const string OLD             = "0365BC9B";
    private const string TEST            = $"{ALPHABET}_{EMPTY_WITH_TABS}_{OLD}-3DE3-4B75-9F7E-2A0F23EFA5A2";


    [Params( "", ALPHABET, TEST )] public string Value { get; set; } = string.Empty;

    [Benchmark] public bool Contains_span()  => Value.Contains( '2' );
    [Benchmark] public bool Contains_value() => Spans.Contains( Value, NEW_VALUE );

    [Benchmark]
    public bool ContainsAny()
    {
        ReadOnlySpan<char> buffer = ['1', '3', 'F', 'A'];

        return Spans.ContainsAny( Value, buffer );
    }

    [Benchmark]
    public bool ContainsNone()
    {
        ReadOnlySpan<char> buffer = ['1', '3', 'F', 'A'];

        return Spans.ContainsNone( Value, buffer );
    }

    [Benchmark] public bool EndsWith()           => Spans.EndsWith( Value, '1' );
    [Benchmark] public bool IsNullOrWhiteSpace() => Spans.IsNullOrWhiteSpace( Value );
    [Benchmark] public bool StartsWith()         => Spans.StartsWith( Value, '1' );


    [Benchmark] public ReadOnlySpan<char> AsSpan() => Value;

    [Benchmark] public ReadOnlySpan<char> Join() => Spans.Join<char>( Value, NEW_VALUE );

    [Benchmark]
    public ReadOnlySpan<char> RemoveAll_Params()
    {
        Span<char> span = stackalloc char[Value.Length];
        Value.CopyTo( span );

        Span<char> result = span.RemoveAll('1', '3', 'F', 'A');
        return result.ToArray();
    }


    [Benchmark] public void               RemoveAll_Single() => Spans.RemoveAll( Value, '1' );
    [Benchmark] public ReadOnlySpan<char> Replace()          => Spans.Replace<char>( Value, OLD, NEW_VALUE );


    [Benchmark] public ReadOnlySpan<char> Slice_False() => Spans.Slice( Value, 'z', '4', false );
    [Benchmark] public ReadOnlySpan<char> Slice_True()  => Spans.Slice( Value, 'z', '4', true );
}
