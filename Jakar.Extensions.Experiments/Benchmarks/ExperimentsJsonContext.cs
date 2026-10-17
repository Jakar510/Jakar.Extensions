// Jakar.Extensions :: Jakar.Extensions.Experiments
// 10/02/2026

namespace Experiments.Benchmarks;


[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Node))]
[JsonSerializable(typeof(Node[]))]
[JsonSerializable(typeof(TestJson))]
public sealed partial class ExperimentsJsonContext : JsonSerializerContext;
