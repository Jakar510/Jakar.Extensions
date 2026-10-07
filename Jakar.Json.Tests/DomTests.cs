// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


[TestFixture]
public sealed class JsonTapeTests
{
    private const string DOC = """{ "id" : 7, "name" : "a\"bé", "lines" : [ { "qty" : 3, "price" : 1.10 }, {"qty":-2,"price":2e3} ], "ok" : true, "none" : null, "tag" : "t" }""";


    [TestCase(false)] [TestCase(true)] public void Navigates_BothEncodings( bool utf8 )
    {
        using JsonTape tape = utf8
                                  ? JsonTape.Parse(Encoding.UTF8.GetBytes(DOC))
                                  : JsonTape.Parse(DOC);

        JsonItem root = tape.Root;

        Assert.That(root.Kind,                                                              Is.EqualTo(JsonTokenKind.Object));
        Assert.That(root.GetPropertyCount(),                                                Is.EqualTo(6));
        Assert.That(root["id"].GetInt32(),                                                  Is.EqualTo(7));
        Assert.That(root["name"].GetString(),                                               Is.EqualTo("a\"bé"));
        Assert.That(root["name"].ValueEquals("a\"bé"),                                      Is.True);
        Assert.That(root["lines"].GetArrayLength(),                                         Is.EqualTo(2));
        Assert.That(root["lines"][1]["qty"].GetInt32(),                                     Is.EqualTo(-2));
        Assert.That(root["lines"][0]["price"].GetDecimal(),                                 Is.EqualTo(1.10m));
        Assert.That(root["lines"][1]["price"].GetDouble(),                                  Is.EqualTo(2000));
        Assert.That(root["lines"][1]["price"].TryGetInteger(out int _),                     Is.False, "2e3 isn't an integer");
        Assert.That(root["ok"].GetBoolean(),                                                Is.True);
        Assert.That(root["none"].IsNull,                                                    Is.True);
        Assert.That(root.TryGetProperty("tag", out JsonItem tag) && tag.GetString() == "t", Is.True, "escaped name");
        Assert.That(root.TryGetProperty("tag"u8,   out _),                                  Is.True);
        Assert.That(root.TryGetProperty("missing", out _),                                  Is.False);
        Assert.That(root["lines"][0]["price"].GetRawText(),                                 Is.EqualTo("1.10"));

        int sum = 0;
        foreach ( JsonItem line in root["lines"].EnumerateArray() ) { sum += line["qty"].GetInt32(); }

        Assert.That(sum, Is.EqualTo(1));

        int names = 0;
        foreach ( JsonTapeProperty property in root.EnumerateObject() ) { names += property.GetName().Length; }

        Assert.That(names, Is.EqualTo("idnamelinesoknonetag".Length));
    }

    [Test] public void Deserialize_ReadsModelsStraightFromTheTape()
    {
        Invoice invoice = ModelTests.SampleInvoice();

        using JsonTape tape = JsonTape.Parse(invoice.ToJsonUtf8());
        ModelTests.AssertSame(invoice, tape.Root.Deserialize<Invoice>());
        Assert.That(tape.Root["lines"][1].Deserialize<LineItem>().Sku, Is.EqualTo("B\"2"));

        using JsonTape zoo = JsonTape.Parse(new Zoo { Animals = [new Cat { Name = "c" }] }.ToJson());
        Assert.That(zoo.Root.Deserialize<Zoo>().Animals[0], Is.TypeOf<Cat>(), "polymorphic lookahead through the tape");
    }

    [Test] public void Disposal_InvalidatesItems()
    {
        JsonTape disposed = JsonTape.Parse("[1]");
        JsonItem stale    = disposed.Root;
        disposed.Dispose();
        disposed.Dispose();

        Assert.Throws<ObjectDisposedException>(() => _ = stale.Kind);
    }

    [Test] public void Streams_AndSizeLimit()
    {
        using ( JsonTape tape = JsonTape.Parse(new MemoryStream(Encoding.UTF8.GetBytes(DOC))) ) { Assert.That(tape.Root["id"].GetInt32(), Is.EqualTo(7)); }

        JsonReadException error = Assert.Throws<JsonReadException>(() => JsonTape.Parse(new MemoryStream(new byte[200]), new JsonReaderOptions { MaxDocumentBytes = 100 }))!;
        Assert.That(error.Error.Kind, Is.EqualTo(JsonErrorKind.DocumentTooLarge));
    }

    [Test] public async Task StreamsAsync()
    {
        using JsonTape tape = await JsonTape.ParseAsync(new MemoryStream(Encoding.UTF8.GetBytes(DOC)));
        Assert.That(tape.Root["ok"].GetBoolean(), Is.True);
    }
}



[TestFixture]
public sealed class JNodeTests
{
    [Test] public void EditsAndPaths()
    {
        JObjectNode order = JObjectNode.Parse("""{"status":"new","lines":[{"qty":1,"price":1.10}],"memo":null}""");
        order["status"]            = "paid";
        order["lines"]![0]!["qty"] = 3;

        Assert.That((int)order["lines"]![0]!["qty"],                             Is.EqualTo(3));
        Assert.That((string?)order["memo"] is null && order.ContainsKey("memo"), Is.True, "JSON null");
        Assert.That(order.ContainsKey("missing"),                                Is.False);
        Assert.That(order["lines"]![0]!["qty"]!.Path,                            Is.EqualTo("$.lines[0].qty"));
        Assert.That(order.ToJson(),                                              Is.EqualTo("""{"status" : "paid","lines" : [{"qty" : 3,"price" : 1.10}],"memo" : null}"""));
    }

    [Test] public void CloneAndDeepEquals()
    {
        JObjectNode order = JObjectNode.Parse("""{"a":1,"b":[1.0,{"c":"d"}]}""");
        JObjectNode copy  = order.DeepClone();

        Assert.That(JNode.DeepEquals(order, copy), Is.True);
        copy["a"] = 2;
        Assert.That(JNode.DeepEquals(order, copy), Is.False);

        Assert.That(JNode.DeepEquals(JNode.Parse("{\"a\":1,\"b\":[1.0]}"),             JNode.Parse("{\"b\":[1e0],\"a\":1}")),             Is.True, "member order and number spelling don't matter");
        Assert.That(JNode.DeepEquals(new JValueNode(1L),                               JValueNode.FromNumberText("1.00")),                Is.True);
        Assert.That(JNode.DeepEquals(new JValueNode(1m),                               new JValueNode(1.0)),                              Is.True);
        Assert.That(JNode.DeepEquals(JValueNode.FromNumberText("9223372036854775808"), JValueNode.FromNumberText("9223372036854775809")), Is.False);
        Assert.That(JNode.DeepEquals(JValueNode.FromNumberText("1e-30"),               new JValueNode(0)),                                Is.False);
    }

    [Test] public void ParentRules_PreventCycles()
    {
        JObjectNode order = JObjectNode.Parse("""{"lines":[1]}""");
        Assert.Throws<InvalidOperationException>(() => order["again"] = order["lines"], "two parents");

        JArrayNode loop = new();
        Assert.Throws<InvalidOperationException>(() => loop.Add(loop), "itself");

        JArrayNode outer = new(), inner = new();
        outer.Add(inner);
        Assert.Throws<InvalidOperationException>(() => inner.Add(outer), "an ancestor");
    }

    [Test] public void Detach_MovesNodes()
    {
        JObjectNode doc = JObjectNode.Parse("""{"a":1,"b":[10,20,30],"c":3}""");
        JNode       b   = doc["b"]!.Detach();

        Assert.That(b.Parent,     Is.Null);
        Assert.That(doc.ToJson(), Is.EqualTo("""{"a" : 1,"c" : 3}"""));

        JNode twenty = b[1]!.Detach();
        Assert.That(b.ToJson(),  Is.EqualTo("[10,30]"));
        Assert.That((int)twenty, Is.EqualTo(20));
        Assert.That(b.Detach(),  Is.SameAs(b), "a root is a no-op");

        doc["moved"] = twenty;
        Assert.That(twenty.Path, Is.EqualTo("$.moved"));

        JObjectNode other = new();
        other["x"] = doc["a"]!.Detach();
        Assert.That(doc.ToJson(),   Is.EqualTo("""{"c" : 3,"moved" : 20}"""));
        Assert.That(other.ToJson(), Is.EqualTo("""{"x" : 1}"""));
    }

    [Test] public void Values_AndConversions()
    {
        Assert.That(JNode.Parse("null"),                              Is.Null);
        Assert.That((double)JNode.Parse("2.5")!,                      Is.EqualTo(2.5));
        Assert.That(JNode.Parse("1.10")!.ToJson(),                    Is.EqualTo("1.10"), "number text is kept");
        Assert.That(new JValueNode(0.1).ToJson(),                     Is.EqualTo("0.1"));
        Assert.That(new JValueNode(5).GetValue<decimal>(),            Is.EqualTo(5m));
        Assert.That((Guid)(JNode)Guid.Empty,                          Is.EqualTo(Guid.Empty));
        Assert.That(JValueNode.FromNumberText("-0.5e+3").GetDouble(), Is.EqualTo(-500));

        DateTime utc = new(2026, 10, 7, 1, 2, 3, DateTimeKind.Utc);
        Assert.That((DateTime)(JNode)utc, Is.EqualTo(utc));

        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new JValueNode(double.NaN));
        Assert.Throws<FormatException>(() => _             = (int)JNode.Parse("1.5")!);
        Assert.Throws<InvalidCastException>(() => _        = (int)(JNode?)null);
        Assert.Throws<FormatException>(() => JValueNode.FromNumberText("01"));
    }

    [Test] public void DuplicateNames_AreRejected()
    {
        JsonReadException error = Assert.Throws<JsonReadException>(() => JNode.Parse("{\"a\":1,\"a\":2}"))!;
        Assert.That(error.Error.Kind,     Is.EqualTo(JsonErrorKind.DuplicateMember));
        Assert.That(error.Error.Position, Is.EqualTo(7));

        Assert.That(JNode.TryParse("{\"a\":1,\"a\":2}", out _, out JsonError dup), Is.False);
        Assert.That(dup.Kind,                                                      Is.EqualTo(JsonErrorKind.DuplicateMember));
    }

    [Test] public void ModelsToAndFromTrees()
    {
        Invoice invoice = ModelTests.SampleInvoice();
        JNode   tree    = JNode.FromModel(invoice)!;

        Assert.That(tree.ToJson(), Is.EqualTo(invoice.ToJson()), "JNodeWriter builds the same document");
        ModelTests.AssertSame(invoice, tree.ToModel<Invoice>());

        JNode zoo = JNode.FromModel(new Zoo
                                    {
                                        Animals =
                                        [
                                            new Puppy
                                            {
                                                Name  = "p",
                                                Weeks = 2
                                            }
                                        ]
                                    })!;

        Assert.That(zoo.ToModel<Zoo>().Animals[0], Is.TypeOf<Puppy>(), "polymorphic lookahead through the DOM");
    }

    [Test] public void ReadsFromAnyReader()
    {
        const string JSON = """{"a":[1,"x",true,null,{"b":2.50}]}""";

        JsonReader<byte> reader = new(Encoding.UTF8.GetBytes(JSON));
        Assert.That(JNode.TryRead(ref reader, out JNode? node), Is.True);
        reader.Dispose();

        Assert.That(node!.ToJson(), Is.EqualTo("""{"a" : [1,"x",true,null,{"b" : 2.50}]}"""));
    }

    [Test] public void ParsedDocuments_AreAFixedPoint()
    {
        string canonical = JNode.Parse("""{ "a" : [ 1.50 , "é\n" ] , "b" : { } }""")!.ToJson();
        Assert.That(JNode.Parse(canonical)!.ToJson(), Is.EqualTo(canonical));
    }
}
