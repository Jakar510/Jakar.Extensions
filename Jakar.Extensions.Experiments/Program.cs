// dotnet run -c Release -- [--bench] [BenchmarkDotNet arguments, e.g. --filter *JakarJson*]

using Experiments;
using Experiments.Benchmarks;


// BenchmarkSwitcher.FromAssembly(typeof(JakarJson_Benchmarks).Assembly)
//                  .Run(args is ["--bench", ..]
//                           ? args[1..]
//                           : args);


SharedUserModel model = new();
Console.WriteLine(model.ToJson());
