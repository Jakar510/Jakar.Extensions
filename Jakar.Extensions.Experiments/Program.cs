using Jakar.Extensions.Experiments.Benchmarks;


Console.WriteLine(DateTimeOffset.UtcNow.ToString());

// Console.WriteLine(SpanDuration.ToString(TimeSpan.FromHours(1.1243123), "End. Duration: "));


BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>();

// BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>(new DebugInProcessConfig());


// JsonNet_SystemTextJson_Benchmarks.Test();


// TestJson test = new();
// string   json = test.ToJson();
// Console.WriteLine();
// Console.WriteLine(json);
// Console.WriteLine();
