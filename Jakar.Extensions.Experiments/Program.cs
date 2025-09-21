using Jakar.Extensions.Experiments.Benchmarks;


Console.WriteLine(DateTimeOffset.UtcNow.ToString());

// Console.WriteLine(SpanDuration.ToString(TimeSpan.FromHours(1.1243123), "End. Duration: "));


TestJson.Print();


// BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>();


BenchmarkRunner.Run<JsonNet_SystemTextJson_Benchmarks>(new DebugInProcessConfig());


// JsonNet_SystemTextJson_Benchmarks.Test();
