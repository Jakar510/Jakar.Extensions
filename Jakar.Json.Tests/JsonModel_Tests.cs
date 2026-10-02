// Jakar.Json.Tests
// 10/02/2026

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Jakar.Extensions;



namespace Jakar.Json.Tests;


[TestFixture]
public sealed class JsonModel_Tests
{
    private static string Compact( string json ) => string.Concat(json.Where(static c => !char.IsWhiteSpace(c)));

    private static JsonTypeInfo<TOther> ViaInterface<T, TOther>()
        where T : IJsonModel<T> => T.GetTypeInfo<TOther>();

    private static T FromJsonViaInterface<T>( string json )
        where T : IJsonModel<T> => T.FromJson(json);


    // ─── Generated members ────────────────────────────────────────────────────

    [Test]
    public void Generated_JsonTypeInfo_ComesFromTheContext() => Assert.That(Invoice.JsonTypeInfo, Is.SameAs(TestJsonContext.Default.Invoice));


    [Test]
    public void Generated_RoundTrip_Class()
    {
        Invoice invoice = new() { Id = "A-1", Total = 12.5m };
        string  json    = invoice.ToJson();

        Assert.That(Invoice.FromJson(json),                           Is.EqualTo(invoice));
        Assert.That(Invoice.FromJson(Encoding.UTF8.GetBytes(json)),   Is.EqualTo(invoice));
        Assert.That(Invoice.TryFromJson(json, out Invoice? parsed),   Is.True);
        Assert.That(parsed,                                           Is.EqualTo(invoice));
        Assert.That(FromJsonViaInterface<Invoice>(json),              Is.EqualTo(invoice));
    }


    [Test]
    public async Task Generated_FromJsonAsync()
    {
        await using MemoryStream stream = new("""{"Id":"B","Total":3}"""u8.ToArray());
        Invoice                  value  = await Invoice.FromJsonAsync(stream);
        Assert.That(value, Is.EqualTo(new Invoice { Id = "B", Total = 3 }));
    }


    [Test]
    public void Generated_RoundTrip_ReadOnlyRecordStruct_WithToString()
    {
        Point point = new(3, 4);
        Assert.That(Compact(point.ToString()),                Is.EqualTo("""{"X":3,"Y":4}"""));
        Assert.That(Point.FromJson(point.ToJson()),          Is.EqualTo(point));
        Assert.That(Point.TryFromJson("""{"X":1,"Y":2}""", out Point parsed), Is.True);
        Assert.That(parsed, Is.EqualTo(new Point(1, 2)));
    }


    [Test]
    public void Generated_RoundTrip_NestedRecord() => Assert.That(Outer.Note.FromJson(new Outer.Note("hi").ToJson()), Is.EqualTo(new Outer.Note("hi")));


    [Test]
    public void TryFromJson_RejectsBlankMalformedAndNullRoot()
    {
        Assert.That(Invoice.TryFromJson((string?)null, out _), Is.False);
        Assert.That(Invoice.TryFromJson("  ",           out _), Is.False);
        Assert.That(Invoice.TryFromJson("{",            out _), Is.False);
        Assert.That(Invoice.TryFromJson("null",         out _), Is.False);
    }


    [Test]
    public void FromJson_NullRoot_Throws() => Assert.That(() => Invoice.FromJson("null"), Throws.InstanceOf<JsonException>());


    [Test]
    public void ToJson_IndentedOverride()
    {
        Invoice invoice = new() { Id = "A", Total = 1 };
        Assert.That(invoice.ToJson(),     Does.Not.Contain("\n"));
        Assert.That(invoice.ToJson(true), Does.Contain("\n"));
    }


    [Test]
    public void ExtensionData_SurvivesRoundTrip()
    {
        Bag bag = Bag.FromJson("""{"Name":"n","Extra":5,"Nested":{"A":true}}""");
        Assert.That(bag.AdditionalData,                         Is.Not.Null);
        Assert.That(bag.AdditionalData!["Extra"].GetInt32(),       Is.EqualTo(5));
        Assert.That(Compact(bag.ToJson()),                       Is.EqualTo("""{"Name":"n","Extra":5,"Nested":{"A":true}}"""));
    }


    // ─── Lookup ───────────────────────────────────────────────────────────────

    [Test]
    public void Registry_HasEveryAccessibleModel()
    {
        Assert.That(JsonModelRegistry.TryGet(out JsonTypeInfo<Invoice>? invoice), Is.True);
        Assert.That(invoice, Is.SameAs(Invoice.JsonTypeInfo));
        Assert.That(JsonModelRegistry.IsRegistered<Point>(),      Is.True);
        Assert.That(JsonModelRegistry.IsRegistered<Outer.Note>(), Is.True);
        Assert.That(JsonModelRegistry.IsRegistered<Code>(),       Is.False);
    }


    [Test]
    public void GetTypeInfo_ResolvesOtherShapesFromTheSameContext()
    {
        JsonTypeInfo<Dictionary<string, Invoice>> info = ViaInterface<Invoice, Dictionary<string, Invoice>>();
        Assert.That(info, Is.SameAs(TestJsonContext.Default.DictionaryStringInvoice));
    }


    [Test]
    public void GetRequiredTypeInfo_Unregistered_ThrowsWithReadableName()
    {
        NotSupportedException? e = Assert.Throws<NotSupportedException>(static () => TestJsonContext.Default.Options.GetRequiredTypeInfo<List<Invoice>>());
        Assert.That(e!.Message, Does.Contain("'List<Invoice>'"));
        Assert.That(e.Message,  Does.Contain("[JsonSerializable(typeof(List<Invoice>))]"));
    }


    // ─── SerializeAsStringJsonConverter ───────────────────────────────────────

    [Test]
    public void SerializeAsString_RoundTrip()
    {
        string json = JsonSerializer.Serialize(new Code(7), TestJsonContext.Default.Code);
        Assert.That(json,                                                         Is.EqualTo("\"C-7\""));
        Assert.That(JsonSerializer.Deserialize("\"C-7\"", TestJsonContext.Default.Code), Is.EqualTo(new Code(7)));
    }


    [Test]
    public void SerializeAsString_IsLenientOnRead()
    {
        Assert.That(JsonSerializer.Deserialize("\"  \"", TestJsonContext.Default.Code), Is.EqualTo(default(Code)));
        Assert.That(JsonSerializer.Deserialize("42",     TestJsonContext.Default.Code), Is.EqualTo(new Code(42)));
        Assert.That(() => JsonSerializer.Deserialize("\"nope\"", TestJsonContext.Default.Code), Throws.InstanceOf<JsonException>());
    }


    [Test]
    public void SerializeAsString_WorksAsDictionaryKey()
    {
        Dictionary<Code, int> map  = new() { [new Code(1)] = 10 };
        string                json = JsonSerializer.Serialize(map, TestJsonContext.Default.DictionaryCodeInt32);
        Assert.That(json, Is.EqualTo("""{"C-1":10}"""));
        Assert.That(JsonSerializer.Deserialize(json, TestJsonContext.Default.DictionaryCodeInt32)![new Code(1)], Is.EqualTo(10));
    }


    [Test]
    public void SerializeAsString_EscapedString() => Assert.That(JsonSerializer.Deserialize("\"C\\u002D9\"", TestJsonContext.Default.Code), Is.EqualTo(new Code(9)));
}
