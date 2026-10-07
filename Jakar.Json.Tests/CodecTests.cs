// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


[TestFixture]
public sealed class CodecTests
{
    private static readonly LineItem[] __items = [new("a", 1, 1m), new("b", 2, 2.5m), new("c", 3, 0m)];


    [Test] public void RootArrays_NeedNoWrapper()
    {
        string json = JsonCodec.ToJsonArray<LineItem>(__items);
        Assert.That(json, Is.EqualTo("""[{"sku" : "a","quantity" : 1,"price" : 1},{"sku" : "b","quantity" : 2,"price" : 2.5},{"sku" : "c","quantity" : 3,"price" : 0}]"""));

        Assert.That(JsonCodec.FromJsonArray<LineItem>(json),                        Is.EqualTo(__items));
        Assert.That(JsonCodec.FromJsonList<LineItem>(Encoding.UTF8.GetBytes(json)), Is.EqualTo(__items));
        Assert.That(JsonCodec.ToJsonArray((IEnumerable<LineItem>)__items.ToList()), Is.EqualTo(json));
        Assert.That(JsonCodec.ToJsonArrayUtf8<LineItem>(__items),                   Is.EqualTo(Encoding.UTF8.GetBytes(json)));

        using JsonPooledArray<LineItem> pooled = JsonCodec.FromJsonArrayPooled<LineItem>(Encoding.UTF8.GetBytes(json));
        Assert.That(pooled.Span.ToArray(), Is.EqualTo(__items));

        Assert.Throws<JsonReadException>(() => JsonCodec.FromJsonArray<LineItem>("[null]"), "a null element isn't a LineItem");
    }

    [Test] public void Streams_RoundTrip()
    {
        Invoice      invoice = ModelTests.SampleInvoice();
        MemoryStream stream  = new();
        invoice.ToJson(stream);

        Assert.That(stream.ToArray(), Is.EqualTo(invoice.ToJsonUtf8()));

        stream.Position = 0;
        ModelTests.AssertSame(invoice, Invoice.FromJson(stream));

        Assert.That(JsonCodec.FromJson<Invoice>(new StringReader(invoice.ToJson())).Total, Is.EqualTo(invoice.Total));

        StringWriter text = new();
        JsonCodec.ToJson(invoice, text);
        Assert.That(text.ToString(), Is.EqualTo(invoice.ToJson()));
    }

    [Test] public async Task Streams_Async()
    {
        Invoice      invoice = ModelTests.SampleInvoice();
        MemoryStream stream  = new();
        await invoice.ToJsonAsync(stream);

        stream.Position = 0;
        ModelTests.AssertSame(invoice, await Invoice.FromJsonAsync(stream));
    }

    [Test] public void LargeOutput_FlushesInChunks()
    {
        Collections big = new() { List = Enumerable.Range(0, 20_000).Select(static i => $"item-{i}").ToList() };

        MemoryStream stream = new();
        big.ToJson(stream);
        Assert.That(Encoding.UTF8.GetString(stream.ToArray()), Is.EqualTo(big.ToJson()));
    }

    [Test] public void BufferWriters_AndBuilders()
    {
        LineItem item = __items[1];

        ArrayBufferWriter<byte> bytes = new();
        item.WriteJson(bytes);
        Assert.That(bytes.WrittenSpan.ToArray(), Is.EqualTo(item.ToJsonUtf8()));

        ArrayBufferWriter<char> chars = new();
        JsonCodec.WriteJson(item, chars);
        Assert.That(chars.WrittenSpan.ToString(), Is.EqualTo(item.ToJson()));

        ValueStringBuilder builder = new(64);
        builder.Append("x=");
        JsonCodec.WriteJson(item, ref builder);
        Assert.That(builder.ToString(), Is.EqualTo("x=" + item.ToJson()));
    }

    [Test] public void Sequences_SingleAndMultiSegment()
    {
        byte[]                 json   = ModelTests.SampleInvoice().ToJsonUtf8();
        ReadOnlySequence<byte> single = new(json);
        Assert.That(JsonCodec.FromJson<Invoice>(single).Total, Is.EqualTo(1.10m));

        Segment                first = new(json.AsMemory(0, 10));
        Segment                last  = first.Append(json.AsMemory(10));
        ReadOnlySequence<byte> multi = new(first, 0, last, last.Memory.Length);
        Assert.That(JsonCodec.FromJson<Invoice>(multi).Total, Is.EqualTo(1.10m));
    }

    [Test] public void TryFromJson_ReportsErrors_WithoutThrowing()
    {
        Assert.That(LineItem.TryFromJson((string?)null, out _), Is.False);
        Assert.That(LineItem.TryFromJson("",            out _), Is.False);
        Assert.That(LineItem.TryFromJson("null",        out _), Is.False);
        Assert.That(LineItem.TryFromJson("{\"sku\":1}", out _), Is.False);

        Assert.That(JsonCodec.TryFromJson<LineItem>("""{"sku":"a","quantity":1,"price":1} x""".AsSpan(), out _, out JsonError error), Is.False);
        Assert.That(error.Kind,                                                                                                       Is.EqualTo(JsonErrorKind.TrailingData));
    }

    [Test] public void Ndjson_ReadsLineByLine()
    {
        const string LINES = "{\"sku\":\"a\",\"quantity\":1,\"price\":1}\r\n\n   \n{\"sku\":\"b\",\"quantity\":2,\"price\":2.5}\n{\"sku\":\"c\",\"quantity\":3,\"price\":0}";

        List<LineItem> items = JsonCodec.ReadLines<LineItem>(new MemoryStream(Encoding.UTF8.GetBytes(LINES))).ToList();
        Assert.That(items, Is.EqualTo(__items));

        MemoryStream output = new();
        JsonCodec.WriteLines<LineItem>(output, __items);
        string written = Encoding.UTF8.GetString(output.ToArray());

        Assert.That(written.Split('\n', StringSplitOptions.RemoveEmptyEntries),                 Has.Length.EqualTo(3));
        Assert.That(JsonCodec.ReadLines<LineItem>(new MemoryStream(output.ToArray())).ToList(), Is.EqualTo(__items));
    }

    [Test] public async Task Ndjson_Async_AndLongStreams()
    {
        StringBuilder text = new();
        for ( int i = 0; i < 5_000; i++ ) { text.Append("{\"sku\":\"").Append(i).Append("\",\"quantity\":").Append(i).Append(",\"price\":1}\n"); }

        int count = 0;

        await foreach ( LineItem item in JsonCodec.ReadLinesAsync<LineItem>(new MemoryStream(Encoding.UTF8.GetBytes(text.ToString()))) )
        {
            Assert.That(item.Quantity, Is.EqualTo(count));
            count++;
        }

        Assert.That(count, Is.EqualTo(5_000));
    }

    [Test] public void Ndjson_ErrorsCarryTheLineNumber()
    {
        const string LINES = "{\"sku\":\"a\",\"quantity\":1,\"price\":1}\n{\"sku\":\"b\",\"quantity\":x}\n";

        JsonReadException error = Assert.Throws<JsonReadException>(() => JsonCodec.ReadLines<LineItem>(new MemoryStream(Encoding.UTF8.GetBytes(LINES))).ToList())!;
        Assert.That(error.Error.Line,   Is.EqualTo(2));
        Assert.That(error.Error.Column, Is.EqualTo(23), "the x, within its line");
    }

    [Test] public void Ndjson_LineLimit()
    {
        string line = "{\"sku\":\"" + new string('x', 5000) + "\",\"quantity\":1,\"price\":1}\n";

        JsonReadException error = Assert.Throws<JsonReadException>(() => JsonCodec.ReadLines<LineItem>(new MemoryStream(Encoding.UTF8.GetBytes(line)), new JsonReaderOptions { MaxDocumentBytes = 1024 }).ToList())!;
        Assert.That(error.Error.Kind, Is.EqualTo(JsonErrorKind.DocumentTooLarge));
    }

    [Test] public void WriterOptions_UnsetValues_MeanTheDefaults()
    {
        // default, and initializers that leave MaxDepth / IndentSize out, must not mean "depth 0" or "indent 0".
        JsonWriterOptions[] options = [default, new() { Indented = true }, new() { Escaping = JsonEscaping.AsciiOnly }];

        foreach ( JsonWriterOptions option in options )
        {
            Assert.That(option.MaxDepth,   Is.EqualTo(JsonWriterOptions.DEFAULT_MAX_DEPTH));
            Assert.That(option.IndentSize, Is.EqualTo(1));
            Assert.That(Invoice.FromJson(ModelTests.SampleInvoice().ToJson(option)).Total, Is.EqualTo(1.10m), "nested output writes and reads back");
        }

        Assert.That(new JsonWriterOptions { MaxDepth = 5000 }.MaxDepth, Is.EqualTo(JsonWriterOptions.MAX_DEPTH_LIMIT));
        Assert.That(new LineItem("a", 1, 1m).ToJson(new JsonWriterOptions { Indented = true }), Is.EqualTo("{\n\t\"sku\" : \"a\",\n\t\"quantity\" : 1,\n\t\"price\" : 1\n}"));
    }

    [Test] public void Writer_RejectsMisuse()
    {
        Assert.Throws<JsonWriteException>(static () =>
                                          {
                                              JsonWriter writer = new(JsonWriterOptions.Default);
                                              writer.WriteStartObject();
                                              writer.WriteInteger(1); // a value with no name
                                          });

        Assert.Throws<JsonWriteException>(static () =>
                                          {
                                              JsonWriter writer = new(JsonWriterOptions.Default);
                                              writer.WriteNull();
                                              writer.WriteNull(); // two roots
                                          });

        Assert.Throws<JsonWriteException>(static () =>
                                          {
                                              JsonWriter writer = new(new JsonWriterOptions { MaxDepth = 2 });
                                              writer.WriteStartArray();
                                              writer.WriteStartArray();
                                              writer.WriteStartArray();
                                          });
    }

    [Test] public void Writer_Escaping()
    {
        Assert.That(Write(JsonEscaping.Minimal),   Is.EqualTo("\"<a&b> \u00e9 \\uD800 / \\n\""), "a lone surrogate is escaped, uppercase hex");
        Assert.That(Write(JsonEscaping.AsciiOnly), Is.EqualTo("\"<a&b> \\u00E9 \\uD800 / \\n\""));
        Assert.That(Write(JsonEscaping.HtmlSafe),  Is.EqualTo("\"\\u003Ca\\u0026b\\u003E \u00e9 \\uD800 / \\n\""));

        static string Write( JsonEscaping escaping )
        {
            JsonWriter writer = new(new JsonWriterOptions { Escaping = escaping });
            writer.WriteString("<a&b> \u00e9 \ud800 / \n");
            return writer.ToString();
        }
    }

    [Test] public void Writer_CanonicalNumbers()
    {
        Assert.That(Write(1e20),      Is.EqualTo("1e+20"));
        Assert.That(Write(1e-7),      Is.EqualTo("1e-7"));
        Assert.That(Write(-0.0),      Is.EqualTo("-0"));
        Assert.That(Write(0.1),       Is.EqualTo("0.1"));
        Assert.That(Write(1.5e300),   Is.EqualTo("1.5e+300"));
        Assert.That(Write(123456789), Is.EqualTo("123456789"));

        static string Write( double value )
        {
            JsonWriter writer = new(JsonWriterOptions.Default);
            writer.WriteFloat(value);
            return writer.ToString();
        }
    }



    private sealed class Segment : ReadOnlySequenceSegment<byte>
    {
        public Segment( ReadOnlyMemory<byte> memory ) => Memory = memory;

        public Segment Append( ReadOnlyMemory<byte> memory )
        {
            Segment next = new(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = next;
            return next;
        }
    }
}
