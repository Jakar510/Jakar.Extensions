// Jakar.Extensions :: Experiments
// 10/23/2023  6:06 PM

namespace Experiments.Benchmarks;


/*
[SimpleJob( RuntimeMoniker.HostProcess )]
[Orderer( SummaryOrderPolicy.FastestToSlowest )]
[RankColumn]
[MemoryDiagnoser]
public class ParameterBenchmarks
{
    private static readonly Parameters<Guid, long, double, DateTimeOffset> _parameters = new(Guid.Empty, 1, 2.0, DateTimeOffset.UtcNow);


    [Benchmark]
    public void TestValueStringBuilder()
    {
        string      format   = ", ";
        CultureInfo provider = CultureInfo.CurrentCulture;
        using var   builder  = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( Guid.Empty, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( 1, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( 2.0, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( DateTimeOffset.UtcNow, default, provider );
    }
    [Benchmark] public string                                         GetString()     => _parameters.ToString( ", ", CultureInfo.CurrentCulture );
    [Benchmark] public Parameters<Guid, long, double, DateTimeOffset> GetParameters() => new(Guid.Empty, 1, 2.0, DateTimeOffset.UtcNow);
}
*/
