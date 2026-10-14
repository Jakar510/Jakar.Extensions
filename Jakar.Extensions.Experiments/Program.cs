// dotnet run -c Release -- [--bench] [BenchmarkDotNet arguments, e.g. --filter *JakarJson*]
// dotnet run -c Release -- --profile [JakarJson_Benchmarks method] [lines] [seconds]   (a hot loop for dotnet-trace; see JsonProfile)

if ( args is ["--profile", ..] )
{
    Experiments.JsonProfile.Run(args[1..]);
    return;
}

BenchmarkSwitcher.FromAssembly(typeof(JakarJson_Benchmarks).Assembly)
                 .Run(args is ["--bench", ..]
                          ? args[1..]
                          : args);


// SharedUserModel model = new();
// Console.WriteLine(model.ToJson());                     // [JsonModel] part: System.Text.Json
// Console.WriteLine(Jakar.Json.JsonCodec.ToJson(model)); // [GenerateJson] part: Jakar.Json


// Console.WriteLine(SharedUserModel.FromJson(Jakar.Json.JsonCodec.ToJson(model)).UserID == Jakar.Json.JsonCodec.FromJson<SharedUserModel>(model.ToJson()).UserID); // each reads the other's output
