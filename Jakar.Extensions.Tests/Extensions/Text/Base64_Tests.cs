// Jakar.Extensions :: Jakar.Extensions.Tests

using System.IO;
using System.Text;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Base64))]
public class Base64_Tests : Assert
{
    private const string HELLO_WORLD      = "Hello, World!";
    private const string HELLO_WORLD_B64  = "SGVsbG8sIFdvcmxkIQ=="; // Base64 of UTF-8 "Hello, World!"
    private const string EMPTY_B64        = "";


    // ─── byte[] roundtrip ────────────────────────────────────────────────────

    [Test]
    public void ByteArray_ToBase64_KnownValue()
    {
        byte[] bytes  = Encoding.UTF8.GetBytes(HELLO_WORLD);
        string result = bytes.ToBase64();
        this.AreEqual(HELLO_WORLD_B64, result);
    }

    [Test]
    public void ByteArray_ToBase64_EmptyArray()
    {
        byte[] bytes  = [];
        string result = bytes.ToBase64();
        this.AreEqual(EMPTY_B64, result);
    }

    [Test]
    public void ByteArray_Roundtrip()
    {
        byte[] original = Encoding.UTF8.GetBytes(HELLO_WORLD);
        string b64      = original.ToBase64();
        byte[] decoded  = b64.FromBase64String();
        this.AreEqual(original.Length, decoded.Length);

        for ( int i = 0; i < original.Length; i++ )
        {
            this.AreEqual(original[i], decoded[i]);
        }
    }


    // ─── string ToBase64 / FromBase64 ────────────────────────────────────────

    [Test] [TestCase("Hello")] [TestCase("Test 123")] [TestCase("特殊文字")] [TestCase("")]
    public void String_ToBase64_Roundtrip( string input )
    {
        string b64     = input.ToBase64(Encoding.UTF8);
        string decoded = Encoding.UTF8.GetString(b64.FromBase64String());
        this.AreEqual(input, decoded);
    }

    [Test]
    public void String_ToBase64_DefaultEncoding_Roundtrip()
    {
        string b64     = HELLO_WORLD.ToBase64();
        string decoded = Encoding.Default.GetString(b64.FromBase64String());
        this.AreEqual(HELLO_WORLD, decoded);
    }


    // ─── ToStreamFromBase64String ─────────────────────────────────────────────

    [Test]
    public void ToStreamFromBase64String_ProducesCorrectBytes()
    {
        byte[] originalBytes = Encoding.UTF8.GetBytes(HELLO_WORLD);
        string b64           = originalBytes.ToBase64();

        using MemoryStream stream = b64.ToStreamFromBase64String();
        byte[]             result = stream.ToArray();

        this.AreEqual(originalBytes.Length, result.Length);

        for ( int i = 0; i < originalBytes.Length; i++ )
        {
            this.AreEqual(originalBytes[i], result[i]);
        }
    }

    [Test]
    public void ToStreamFromBase64String_EmptyString_ProducesEmptyStream()
    {
        string            b64    = EMPTY_B64;
        using MemoryStream stream = b64.ToStreamFromBase64String();
        this.AreEqual(0L, stream.Length);
    }


    // ─── Span / Memory overloads ──────────────────────────────────────────────

    [Test]
    public void Memory_ToBase64_Roundtrip()
    {
        byte[]               original = Encoding.UTF8.GetBytes(HELLO_WORLD);
        ReadOnlyMemory<byte> memory   = original;
        string               b64      = memory.ToBase64();
        byte[]               decoded  = b64.FromBase64String();

        this.AreEqual(original.Length, decoded.Length);
    }

    [Test]
    public void Span_ToBase64_Roundtrip()
    {
        byte[]      original = Encoding.UTF8.GetBytes(HELLO_WORLD);
        Span<byte>  span     = original;
        string      b64      = span.ToBase64();
        byte[]      decoded  = b64.FromBase64String();

        this.AreEqual(original.Length, decoded.Length);
    }


    // ─── JSON roundtrip via Base64 ────────────────────────────────────────────

    [Test]
    public void JsonToBase64_AndBack_Roundtrip()
    {
        SampleRecord original = new("Alice", 30);
        string        b64     = original.ToBase64(Encoding.UTF8);
        SampleRecord  result  = b64.JsonFromBase64String<SampleRecord>(Encoding.UTF8);
        this.AreEqual(original.Name,  result.Name);
        this.AreEqual(original.Age,   result.Age);
    }

    [Test]
    public void JsonToBase64_DefaultEncoding_Roundtrip()
    {
        SampleRecord original = new("Bob", 25);
        string        b64     = original.ToBase64();
        SampleRecord  result  = b64.JsonFromBase64String<SampleRecord>();
        this.AreEqual(original.Name, result.Name);
        this.AreEqual(original.Age,  result.Age);
    }


    // ─── Invalid input ────────────────────────────────────────────────────────

    [Test]
    public void FromBase64String_InvalidInput_ThrowsFormatException()
    {
        Throws<FormatException>(() => _ = "not-valid-base64!!".FromBase64String());
    }


    private sealed record SampleRecord( string Name, int Age );
}
