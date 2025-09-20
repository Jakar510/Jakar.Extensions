using Jakar.Extensions.Experiments.Benchmarks;


Console.WriteLine(DateTimeOffset.UtcNow.ToString());

// Console.WriteLine(SpanDuration.ToString(TimeSpan.FromHours(1.1243123), "End. Duration: "));

// BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>();

JsonNet_SystemTextJson_Benchmarks.Test();