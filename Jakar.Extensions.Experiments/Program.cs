// dotnet run -c Release -- [--bench] [BenchmarkDotNet arguments, e.g. --filter *Hash*]
// The Jakar.Json benchmarks are in the Jakar.Json repository (Jakar.Json.Benchmarks).

BenchmarkSwitcher.FromAssembly(typeof(BenchmarkConfig).Assembly)
                 .Run(args is ["--bench", ..]
                          ? args[1..]
                          : args);


// SharedUserModel model = new();
// Console.WriteLine(model.ToJson());                     // [JsonModel] part: System.Text.Json
// Console.WriteLine(Jakar.Json.JsonCodec.ToJson(model)); // [GenerateJson] part: Jakar.Json


// Console.WriteLine(SharedUserModel.FromJson(Jakar.Json.JsonCodec.ToJson(model)).UserID == Jakar.Json.JsonCodec.FromJson<SharedUserModel>(model.ToJson()).UserID); // each reads the other's output
