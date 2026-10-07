// Jakar.Extensions :: Experiments
// 10/07/2026

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using JJ = Jakar.Json;
using NewtonsoftSerializer = Newtonsoft.Json.JsonSerializer;



namespace Experiments.Benchmarks;


/// <summary>
///     Jakar.Json (<c>[GenerateJson]</c>) against Jakar.SystemTextJson (<c>[JsonModel]</c>, System.Text.Json source generation) and Newtonsoft.Json (reflection), on the same model shape (Jakar.Json SPEC.md §12.5).
///     Jakar.SystemTextJson is the baseline of every category.
///     <para> Each library gets its own model type (<see cref="BenchOrder"/> / <see cref="StjOrder"/>): with both generators on one type, its own FromJson/ToJson would be Jakar.SystemTextJson's (JJSON010 side-by-side mode). Newtonsoft reads and writes <see cref="StjOrder"/>, which is a plain POCO to it. All three write enums as names. </para>
///     Run: <c>dotnet run -c Release --project Jakar.Extensions.Experiments -- --bench --filter *JakarJson*</c>
/// </summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
[CategoriesColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class JakarJson_Benchmarks
{
    private static readonly JsonSerializerSettings __newtonsoftSettings   = new() { Converters = [new StringEnumConverter()] };
    private static readonly NewtonsoftSerializer   __newtonsoftSerializer = NewtonsoftSerializer.Create(__newtonsoftSettings);
    private static readonly UTF8Encoding           __utf8NoBom            = new(false);

    private readonly char[]       __chars  = new char[4 * 1024 * 1024];
    private readonly byte[]       __bytes  = new byte[4 * 1024 * 1024];
    private readonly MemoryStream __output = new(4 * 1024 * 1024);

    private BenchOrder __order    = null!;
    private StjOrder   __stjOrder = null!;
    private string     __json     = "";
    private byte[]     __utf8     = [];


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

        __stjOrder = StjOrder.From(__order);

        // Every library reads the same document. All three accept the others' output; the inputs differ only in whitespace (Jakar.Json writes "name" : value).
        __json = __order.ToJson();
        __utf8 = __order.ToJsonUtf8();

        Verify();
    }

    /// <summary> Fails setup, instead of reporting meaningless numbers, if a library reads the shared document differently. </summary>
    private void Verify()
    {
        string expected = __stjOrder.ToJson();

        Check(nameof(JakarSystemTextJson_FromString), JakarSystemTextJson_FromString().ToJson());
        Check(nameof(JakarSystemTextJson_FromUtf8),   JakarSystemTextJson_FromUtf8().ToJson());
        Check(nameof(Newtonsoft_FromString),          Newtonsoft_FromString()!.ToJson());
        Check(nameof(Newtonsoft_FromUtf8),            Newtonsoft_FromUtf8()!.ToJson());
        Check(nameof(JakarJson_FromString),           StjOrder.From(JakarJson_FromString()).ToJson());
        Check(nameof(JakarJson_FromUtf8),             StjOrder.From(JakarJson_FromUtf8()).ToJson());
        Check(nameof(Newtonsoft_String),              StjOrder.FromJson(Newtonsoft_String()).ToJson());

        decimal sum = __order.Lines.Sum(static line => line.Price);
        if ( JakarJsonTape_SumPrices() != sum || JakarJsonNode_SumPrices() != sum || StjDocument_SumPrices() != sum || StjNode_SumPrices() != sum || NewtonsoftJObject_SumPrices() != sum ) { throw new InvalidOperationException("The navigation benchmarks disagree on the sum of prices."); }

        void Check( string name, string actual )
        {
            if ( actual != expected ) { throw new InvalidOperationException($"{name} doesn't round-trip the benchmark document."); }
        }
    }


    // ─── Serialize ───────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize string")] public string JakarSystemTextJson_String() => __stjOrder.ToJson();
    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public string JakarJson_String()           => __order.ToJson();
    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public string Newtonsoft_String()          => JsonConvert.SerializeObject(__stjOrder, __newtonsoftSettings);

    [Benchmark] [BenchmarkCategory("Serialize string")] public int JakarJson_Chars_IntoBuffer() => __order.TryFormat(__chars, out int written, default, null)
                                                                                                       ? written
                                                                                                       : -1;

    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize UTF-8")] public byte[] JakarSystemTextJson_Utf8() => __stjOrder.ToUtf8Json();
    [Benchmark] [BenchmarkCategory(                 "Serialize UTF-8")] public byte[] JakarJson_Utf8()           => __order.ToJsonUtf8();

    [Benchmark] [BenchmarkCategory("Serialize UTF-8")] public byte[] Newtonsoft_Utf8()
    {
        using MemoryStream stream = new();

        using ( StreamWriter text = new(stream, __utf8NoBom, 4096, true) )
        using ( JsonTextWriter writer = new(text) ) { __newtonsoftSerializer.Serialize(writer, __stjOrder); }

        return stream.ToArray();
    }

    [Benchmark] [BenchmarkCategory("Serialize UTF-8")] public int JakarJson_Utf8_IntoBuffer() => __order.TryFormat(__bytes, out int written, default, null)
                                                                                                     ? written
                                                                                                     : -1;

    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize Stream")] public long JakarSystemTextJson_Stream()
    {
        __output.Position = 0;
        __stjOrder.WriteToAsync(__output).GetAwaiter().GetResult();
        return __output.Position;
    }

    [Benchmark] [BenchmarkCategory("Serialize Stream")] public long JakarJson_Stream()
    {
        __output.Position = 0;
        __order.ToJson(__output);
        return __output.Position;
    }

    [Benchmark] [BenchmarkCategory("Serialize Stream")] public long Newtonsoft_Stream()
    {
        __output.Position = 0;

        using ( StreamWriter text = new(__output, __utf8NoBom, 4096, true) )
        using ( JsonTextWriter writer = new(text) ) { __newtonsoftSerializer.Serialize(writer, __stjOrder); }

        return __output.Position;
    }


    // ─── Deserialize ─────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)] [BenchmarkCategory("Deserialize string")] public StjOrder   JakarSystemTextJson_FromString() => StjOrder.FromJson(__json);
    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public BenchOrder JakarJson_FromString()           => BenchOrder.FromJson(__json);
    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public StjOrder?  Newtonsoft_FromString()          => JsonConvert.DeserializeObject<StjOrder>(__json, __newtonsoftSettings);

    [Benchmark(Baseline = true)] [BenchmarkCategory("Deserialize UTF-8")] public StjOrder   JakarSystemTextJson_FromUtf8() => StjOrder.FromJson(__utf8.AsSpan());
    [Benchmark] [BenchmarkCategory(                 "Deserialize UTF-8")] public BenchOrder JakarJson_FromUtf8()           => BenchOrder.FromJson(__utf8.AsSpan());

    [Benchmark] [BenchmarkCategory("Deserialize UTF-8")] public StjOrder? Newtonsoft_FromUtf8()
    {
        using StreamReader   text   = new(new MemoryStream(__utf8, false), __utf8NoBom, false);
        using JsonTextReader reader = new(text);
        return __newtonsoftSerializer.Deserialize<StjOrder>(reader);
    }


    // ─── Dynamic ─────────────────────────────────────────────────────────────

    [Benchmark(Baseline = true)] [BenchmarkCategory("Navigate (read-only)")] public decimal StjDocument_SumPrices()
    {
        using JsonDocument document = JsonDocument.Parse(__utf8);
        decimal            sum      = 0;
        foreach ( JsonElement line in document.RootElement.GetProperty("Lines").EnumerateArray() ) { sum += line.GetProperty("Price").GetDecimal(); }

        return sum;
    }

    [Benchmark] [BenchmarkCategory("Navigate (read-only)")] public decimal JakarJsonTape_SumPrices()
    {
        using JJ.JsonTape tape = JJ.JsonTape.Parse(__utf8);
        decimal           sum  = 0;
        foreach ( JJ.JsonItem line in tape.Root["Lines"].EnumerateArray() ) { sum += line["Price"].GetDecimal(); }

        return sum;
    }

    [Benchmark(Baseline = true)] [BenchmarkCategory("DOM")] public decimal StjNode_SumPrices()
    {
        System.Text.Json.Nodes.JsonNode root = System.Text.Json.Nodes.JsonNode.Parse(__utf8)!;
        decimal                         sum  = 0;
        foreach ( System.Text.Json.Nodes.JsonNode? line in root["Lines"]!.AsArray() ) { sum += line!["Price"]!.GetValue<decimal>(); }

        return sum;
    }

    [Benchmark] [BenchmarkCategory("DOM")] public decimal JakarJsonNode_SumPrices()
    {
        JJ.JNode root = JJ.JNode.Parse(__utf8.AsSpan())!;
        decimal  sum  = 0;
        foreach ( JJ.JNode? line in root["Lines"]!.AsArray() ) { sum += (decimal)line!["Price"]; }

        return sum;
    }

    [Benchmark] [BenchmarkCategory("DOM")] public decimal NewtonsoftJObject_SumPrices()
    {
        JObject root = JObject.Parse(__json);
        decimal sum  = 0;
        foreach ( JToken line in (JArray)root["Lines"]! ) { sum += (decimal)line["Price"]!; }

        return sum;
    }
}



public enum BenchStatus
{
    Draft,
    Sent,
    Paid
}



// ─── Jakar.Json ──────────────────────────────────────────────────────────────



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



// ─── Jakar.SystemTextJson (and Newtonsoft.Json, by reflection) ───────────────



[JsonModel(typeof(BenchContext))]
public sealed partial class StjOrder
{
    public Guid           Id       { get; set; }
    public string         Customer { get; set; } = "";
    public DateTimeOffset Created  { get; set; }
    public BenchStatus    Status   { get; set; }
    public List<StjLine>  Lines    { get; set; } = [];


    public static StjOrder From( BenchOrder order ) => new()
                                                       {
                                                           Id       = order.Id,
                                                           Customer = order.Customer,
                                                           Created  = order.Created,
                                                           Status   = order.Status,
                                                           Lines = order.Lines.Select(static line => new StjLine
                                                                                                     {
                                                                                                         Sku      = line.Sku,
                                                                                                         Name     = line.Name,
                                                                                                         Quantity = line.Quantity,
                                                                                                         Price    = line.Price,
                                                                                                         Taxable  = line.Taxable
                                                                                                     })
                                                                        .ToList()
                                                       };
}



[JsonModel(typeof(BenchContext))]
public sealed partial class StjLine
{
    public string  Sku      { get; set; } = "";
    public string  Name     { get; set; } = "";
    public int     Quantity { get; set; }
    public decimal Price    { get; set; }
    public bool    Taxable  { get; set; }
}



// Enums as names, like Jakar.Json's default and the Newtonsoft StringEnumConverter above.
[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(StjOrder))]
[JsonSerializable(typeof(StjLine))]
public sealed partial class BenchContext : JsonSerializerContext;
