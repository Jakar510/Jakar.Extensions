// Jakar.Extensions :: Experiments
// 10/07/2026

using JJ = Jakar.Json;



namespace Jakar.Extensions.Experiments.Benchmarks;


/// <summary>
///     Jakar.Json against System.Text.Json source generation and Newtonsoft.Json (Jakar.Json SPEC.md §12.5), on the same model.
///     Run: <c>dotnet run -c Release --project Jakar.Extensions.Experiments -- --bench --filter *JakarJson*</c>
/// </summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
[CategoriesColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class JakarJson_Benchmarks
{
    private readonly char[] __chars = new char[4 * 1024 * 1024];
    private readonly byte[] __bytes = new byte[4 * 1024 * 1024];

    private BenchOrder __order = null!;
    private string     __json  = "";
    private byte[]     __utf8  = [];


    /// <summary> Lines per order: ≈ 200 B, 10 KB and 1 MB documents. </summary>
    [Params(1, 75, 7_500)] public int Lines { get; set; }


    [GlobalSetup] public void Setup()
    {
        Random random = new(510);

        __order = new BenchOrder
                  {
                      Id       = new Guid(Enumerable.Range(0, 16).Select(i => (byte)random.Next(256)).ToArray()),
                      Customer = "Ada Lovelace",
                      Created  = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero),
                      Status   = BenchStatus.Paid,
                      Lines = Enumerable.Range(0, Lines)
                                        .Select(i => new BenchLine
                                                     {
                                                         Sku      = $"SKU-{i:D6}",
                                                         Name     = $"Item \"{i}\" — Ø",
                                                         Quantity = random.Next(1, 100),
                                                         Price    = Math.Round((decimal)random.NextDouble() * 100, 2),
                                                         Taxable  = i % 2 == 0
                                                     })
                                        .ToList()
                  };

        __json = __order.ToJson();
        __utf8 = __order.ToJsonUtf8();
    }


    // ─── Serialize ───────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize UTF-8")] public byte[] StjSourceGen_Utf8() => JsonSerializer.SerializeToUtf8Bytes(__order, BenchContext.Default.BenchOrder);
    [Benchmark] [BenchmarkCategory(                 "Serialize UTF-8")] public byte[] JakarJson_Utf8()    => __order.ToJsonUtf8();
    [Benchmark] [BenchmarkCategory(                 "Serialize UTF-8")] public int JakarJson_Utf8_IntoBuffer() => __order.TryFormat(__bytes, out int written, default, null)
                                                                                                                      ? written
                                                                                                                      : -1;

    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize string")] public string StjSourceGen_String() => JsonSerializer.Serialize(__order, BenchContext.Default.BenchOrder);
    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public string JakarJson_String()    => __order.ToJson();
    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public int JakarJson_Chars_IntoBuffer() => __order.TryFormat(__chars, out int written, default, null)
                                                                                                                        ? written
                                                                                                                        : -1;
    [Benchmark] [BenchmarkCategory("Serialize string")] public string Newtonsoft_String() => JsonConvert.SerializeObject(__order);


    // ─── Deserialize ─────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)] [BenchmarkCategory("Deserialize UTF-8")] public BenchOrder? StjSourceGen_FromUtf8() => JsonSerializer.Deserialize(__utf8, BenchContext.Default.BenchOrder);
    [Benchmark] [BenchmarkCategory(                 "Deserialize UTF-8")] public BenchOrder  JakarJson_FromUtf8()    => BenchOrder.FromJson(__utf8.AsSpan());

    [Benchmark(Baseline = true)] [BenchmarkCategory("Deserialize string")] public BenchOrder? StjSourceGen_FromString() => JsonSerializer.Deserialize(__json, BenchContext.Default.BenchOrder);
    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public BenchOrder  JakarJson_FromString()    => BenchOrder.FromJson(__json);
    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public BenchOrder? Newtonsoft_FromString()   => JsonConvert.DeserializeObject<BenchOrder>(__json);


    // ─── Navigation ──────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)] [BenchmarkCategory("Navigate")] public decimal StjDocument_SumPrices()
    {
        using JsonDocument document = JsonDocument.Parse(__utf8);
        decimal            sum      = 0;
        foreach ( JsonElement line in document.RootElement.GetProperty("Lines").EnumerateArray() ) { sum += line.GetProperty("Price").GetDecimal(); }

        return sum;
    }

    [Benchmark] [BenchmarkCategory("Navigate")] public decimal JakarJsonTape_SumPrices()
    {
        using JJ.JsonTape tape = JJ.JsonTape.Parse(__utf8);
        decimal           sum  = 0;
        foreach ( JJ.JsonItem line in tape.Root["Lines"].EnumerateArray() ) { sum += line["Price"].GetDecimal(); }

        return sum;
    }
}



public enum BenchStatus
{
    Draft,
    Sent,
    Paid
}



[JJ.GenerateJson]
public sealed partial class BenchOrder
{
    public Guid            Id       { get; set; }
    public string          Customer { get; set; } = "";
    public DateTimeOffset  Created  { get; set; }
    public BenchStatus     Status   { get; set; }
    public List<BenchLine> Lines    { get; set; } = [];
}



[JJ.GenerateJson]
public sealed partial class BenchLine
{
    public string  Sku      { get; set; } = "";
    public string  Name     { get; set; } = "";
    public int     Quantity { get; set; }
    public decimal Price    { get; set; }
    public bool    Taxable  { get; set; }
}



// The same model through System.Text.Json's source generator (enums as names, like Jakar.Json's default).
[JsonSourceGenerationOptions(UseStringEnumConverter = true)] [JsonSerializable(typeof(BenchOrder))] public sealed partial class BenchContext : JsonSerializerContext;
