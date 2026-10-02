// Jakar.Extensions :: Jakar.Extensions.Experiments
// 10/02/2026

namespace Jakar.Extensions.Experiments.Benchmarks;


[System.Text.Json.Serialization.JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[System.Text.Json.Serialization.JsonSerializable(typeof(Node))]
[System.Text.Json.Serialization.JsonSerializable(typeof(Node[]))]
[System.Text.Json.Serialization.JsonSerializable(typeof(TestJson))]
public sealed partial class ExperimentsJsonContext : System.Text.Json.Serialization.JsonSerializerContext;
