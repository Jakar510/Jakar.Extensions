// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


/// <summary>
///     RFC 8259 conformance, after the JSONTestSuite corpus (github.com/nst/JSONTestSuite): <c>y_</c> cases must parse, <c>n_</c> cases must fail.
///     Every case runs through all four entry points: the streaming reader (UTF-16 and UTF-8) and the tape (UTF-16 and UTF-8).
/// </summary>
[TestFixture]
public sealed class ConformanceTests
{
    public static readonly string[] Accepted =
    [
        "[]",
        "{}",
        "[[]]",
        "[{}]",
        "{\"a\":[]}",
        "  [1]  ",
        "\t\n\r [1]",
        "1",
        "-0",
        "0",
        "-1",
        "123",
        "1.5",
        "-1.5e10",
        "1E+2",
        "1e-2",
        "1.0E-10",
        "0.0",
        "1e999",
        "-1e-999",
        "123456789012345678901234567890",
        "\"\"",
        "\"a\"",
        "\"\\u0000\"",
        "\"\\\"\\\\\\/\\b\\f\\n\\r\\t\"",
        "\"\\uD834\\uDD1E\"",
        "\"\\uDEAD\"",
        "\"\\uD800\"",
        "\"\u00e9\"",
        "\"\ud83d\ude00\"",
        "\"\\u00e9\"",
        "true",
        "false",
        "null",
        "[true,false,null]",
        "{\"a\":1,\"b\":[1,2,{\"c\":null}]}",
        "{\"\":0}",
        "{\"a\":1,\"a\":2}",
        "[1,2,3,4,5,6,7,8,9,10]",
        "{\"a\" : 1 , \"b\" : 2}",
        "[\"\\u0041\"]",
        "\uFEFF[1]",
        "[-0.0]",
        "[1e1]",
        "[0e0]",
        "[0E+1]",
        "[-123.456e-78]",
        "\"\u2028\u2029\"",
        "[[[[[[[[[[1]]]]]]]]]]"
    ];

    public static readonly string[] Rejected =
    [
        "",
        " ",
        "[",
        "]",
        "{",
        "}",
        "[1,]",
        "[,1]",
        "[1,,2]",
        "{\"a\":1,}",
        "{,}",
        "{\"a\"}",
        "{\"a\" 1}",
        "{\"a\":}",
        "{a:1}",
        "{'a':1}",
        "['a']",
        "[1 2]",
        "[1] [2]",
        "[1]x",
        "01",
        "-01",
        "+1",
        ".5",
        "1.",
        "1.e5",
        "-",
        "1e",
        "1e+",
        "0x10",
        "NaN",
        "Infinity",
        "-Infinity",
        "[--1]",
        "[1.2.3]",
        "[Inf]",
        "tru",
        "nul",
        "fals",
        "True",
        "NULL",
        "\"",
        "\"abc",
        "\"\\x\"",
        "\"\\u12\"",
        "\"\\u12G4\"",
        "\"\\\"",
        "\"a\tb\"",
        "\"a\nb\"",
        "\"\u0001\"",
        "\"\ud800\"",
        "\"\udc00 \"",
        "[\"a\"\"b\"]",
        "/* c */ 1",
        "// c\n1",
        "{\"a\":1}}",
        "[1]]",
        "[[]",
        "[{]",
        "{[}",
        "{\"a\":1 \"b\":2}",
        "[1,]]",
        "\"\\uDEAD",
        "1 2",
        "[1e1.0]",
        "[0.1.2]",
        "[1e1e1]",
        "[-]",
        "[.123]",
        "[012]"
    ];


    [TestCaseSource(nameof(Accepted))] public void Accepts( string json )
    {
        Assert.That(ReadsUtf16(json),                                                 Is.True, "reader, UTF-16");
        Assert.That(ReadsUtf8(json),                                                  Is.True, "reader, UTF-8");
        Assert.That(JsonTape.TryParse(json, out JsonTape? tape, out JsonError error), Is.True, $"tape, UTF-16: {error}");
        tape!.Dispose();
        Assert.That(JsonTape.TryParse(Encoding.UTF8.GetBytes(json), out tape, out error), Is.True, $"tape, UTF-8: {error}");
        tape!.Dispose();
    }

    [TestCaseSource(nameof(Rejected))] public void Rejects( string json )
    {
        Assert.That(ReadsUtf16(json),                                   Is.False, "reader, UTF-16");
        Assert.That(JsonTape.TryParse(json, out JsonTape? tape, out _), Is.False, "tape, UTF-16");
        tape?.Dispose();

        // A raw lone surrogate has no UTF-8 form (Encoding.UTF8 substitutes U+FFFD, which is valid), so those cases are UTF-16 only.
        if ( !Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(json)).Equals(json, StringComparison.Ordinal) ) { return; }

        Assert.That(ReadsUtf8(json),                                                  Is.False, "reader, UTF-8");
        Assert.That(JsonTape.TryParse(Encoding.UTF8.GetBytes(json), out tape, out _), Is.False, "tape, UTF-8");
        tape?.Dispose();
    }

    [Test] public void InvalidUtf8_IsRejected()
    {
        byte[][] cases = [[(byte)'"', 0xC3, (byte)'"'], [(byte)'"', 0xFF, (byte)'"'], [(byte)'"', 0xED, 0xA0, 0x80, (byte)'"'], [(byte)'[', 0xC0, 0xAF, (byte)']']];

        foreach ( byte[] bytes in cases )
        {
            JsonReader<byte> reader = new(bytes);
            Assert.That(reader.TrySkipValue(), Is.False);
            Assert.That(reader.Error.Kind,     Is.EqualTo(JsonErrorKind.InvalidUtf8));
            reader.Dispose();
        }
    }

    [Test] public void Depth_IsBounded_WithoutRecursion()
    {
        string deep = new string('[', 100_000) + new string(']', 100_000);

        JsonReader<char> reader = new(deep);
        Assert.That(reader.TrySkipValue(), Is.False);
        Assert.That(reader.Error.Kind,     Is.EqualTo(JsonErrorKind.DepthExceeded));
        reader.Dispose();

        Assert.That(JsonTape.TryParse(deep, out _, out JsonError error), Is.False);
        Assert.That(error.Kind,                                          Is.EqualTo(JsonErrorKind.DepthExceeded));

        string limit = new string('[', 64) + new string(']', 64);
        Assert.That(ReadsUtf16(limit), Is.True, "64 levels is the default limit");
    }

    [Test] public void ErrorPositions_HaveLineAndColumn()
    {
        JsonReader<char> reader = new("{\n  \"a\": x}");
        Assert.That(reader.TrySkipValue(), Is.False);
        Assert.That(reader.Error,          Is.EqualTo(new JsonError(JsonErrorKind.UnexpectedToken, 9, 2, 8)));
        reader.Dispose();
    }


    private static bool ReadsUtf16( string json )
    {
        JsonReader<char> reader = new(json);

        try { return reader.TrySkipValue() && reader.TryReadEnd(); }
        finally { reader.Dispose(); }
    }

    private static bool ReadsUtf8( string json )
    {
        byte[]           bytes  = Encoding.UTF8.GetBytes(json);
        JsonReader<byte> reader = new(bytes);

        try { return reader.TrySkipValue() && reader.TryReadEnd(); }
        finally { reader.Dispose(); }
    }
}
