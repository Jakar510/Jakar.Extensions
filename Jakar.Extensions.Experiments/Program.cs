// dotnet run -c Release -- [--bench] [BenchmarkDotNet arguments, e.g. --filter *Hash*]
// The Jakar.Json benchmarks are in the Jakar.Json repository (Jakar.Json.Benchmarks).

BenchmarkSwitcher.FromAssembly(typeof(BenchmarkConfig).Assembly)
                 .Run(args is ["--bench", ..]
                          ? args[1..]
                          : args);
