// Jakar.Extensions :: Experiments
// 11/18/2023  10:43 PM

namespace Jakar.Extensions.Experiments.Benchmarks;


[MarkdownExporterAttribute.GitHub]
public class BenchmarkConfig : ManualConfig
{
    // private readonly FileStream _stream = new("Benchmark.log", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
    public BenchmarkConfig() => SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);

    // AddLogger( new ConsoleLogger(), new StreamLogger( new StreamWriter( _stream ) ) );
    // AddExporter( new JsonExporter() );
    // AddColumnProvider( new CompositeColumnProvider() );
}
