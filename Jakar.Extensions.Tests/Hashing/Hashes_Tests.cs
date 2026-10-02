// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Text;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Hashes))]
public class Hashes_Tests : Assert
{
    private const string SAMPLE        = "Hello, World!";
    private const string SAMPLE_ALT    = "Different input";
    private const string EMPTY_STRING  = "";


    // ─── Determinism: same input → same hash ─────────────────────────────────

    [Test]
    public void Hash_MD5_Deterministic()
    {
        string h1 = SAMPLE.Hash_MD5();
        string h2 = SAMPLE.Hash_MD5();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA1_Deterministic()
    {
        string h1 = SAMPLE.Hash_SHA1();
        string h2 = SAMPLE.Hash_SHA1();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA256_Deterministic()
    {
        string h1 = SAMPLE.Hash_SHA256();
        string h2 = SAMPLE.Hash_SHA256();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA384_Deterministic()
    {
        string h1 = SAMPLE.Hash_SHA384();
        string h2 = SAMPLE.Hash_SHA384();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA512_Deterministic()
    {
        string h1 = SAMPLE.Hash_SHA512();
        string h2 = SAMPLE.Hash_SHA512();
        this.AreEqual(h1, h2);
    }


    // ─── Differentiation: different input → different hash ───────────────────

    [Test]
    public void Hash_MD5_DifferentInputs_DifferentHashes()
    {
        string h1 = SAMPLE.Hash_MD5();
        string h2 = SAMPLE_ALT.Hash_MD5();
        this.NotEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA256_DifferentInputs_DifferentHashes()
    {
        string h1 = SAMPLE.Hash_SHA256();
        string h2 = SAMPLE_ALT.Hash_SHA256();
        this.NotEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA512_DifferentInputs_DifferentHashes()
    {
        string h1 = SAMPLE.Hash_SHA512();
        string h2 = SAMPLE_ALT.Hash_SHA512();
        this.NotEqual(h1, h2);
    }


    // ─── Cross-algorithm differentiation ─────────────────────────────────────

    [Test]
    public void DifferentAlgorithms_ProduceDifferentHashes()
    {
        string md5    = SAMPLE.Hash_MD5();
        string sha256 = SAMPLE.Hash_SHA256();
        string sha512 = SAMPLE.Hash_SHA512();

        this.NotEqual(md5,    sha256);
        this.NotEqual(sha256, sha512);
        this.NotEqual(md5,    sha512);
    }


    // ─── Encoding variants ────────────────────────────────────────────────────

    [Test]
    public void Hash_SHA256_WithEncoding_Deterministic()
    {
        string h1 = SAMPLE.Hash_SHA256(Encoding.UTF8);
        string h2 = SAMPLE.Hash_SHA256(Encoding.UTF8);
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA256_DifferentEncodings_ProduceDifferentHashes()
    {
        // Unicode string with non-ASCII characters will differ across encodings
        string input  = "café";
        string utf8   = input.Hash_SHA256(Encoding.UTF8);
        string utf32  = input.Hash_SHA256(Encoding.UTF32);
        this.NotEqual(utf8, utf32);
    }


    // ─── Span overloads ───────────────────────────────────────────────────────

    [Test]
    public void Hash_SHA256_ByteSpan_Deterministic()
    {
        byte[]             bytes = Encoding.UTF8.GetBytes(SAMPLE);
        ReadOnlySpan<byte> span  = bytes;
        string             h1    = span.Hash_SHA256();
        string             h2    = span.Hash_SHA256();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_MD5_ByteSpan_Matches_StringHash()
    {
        // Both paths should produce equivalent hashes for same data with same encoding
        byte[]             bytes     = Encoding.Default.GetBytes(SAMPLE);
        ReadOnlySpan<byte> byteSpan  = bytes;
        string             byteHash  = byteSpan.Hash_MD5();
        string             stringHash = SAMPLE.Hash_MD5(Encoding.Default);
        this.AreEqual(byteHash, stringHash);
    }

    [Test]
    public void Hash_SHA256_CharSpan_Deterministic()
    {
        ReadOnlySpan<char> span = SAMPLE.AsSpan();
        string             h1   = span.Hash_SHA256(Encoding.UTF8);
        string             h2   = span.Hash_SHA256(Encoding.UTF8);
        this.AreEqual(h1, h2);
    }

    [Test]
    public void GetHash_ByteArray_Deterministic()
    {
        byte[] data = Encoding.UTF8.GetBytes(SAMPLE);
        string h1   = data.GetHash();
        string h2   = data.GetHash();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void GetHash_String_Deterministic()
    {
        string h1 = SAMPLE.GetHash();
        string h2 = SAMPLE.GetHash();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void GetHash_String_DifferentInputs()
    {
        string h1 = SAMPLE.GetHash();
        string h2 = SAMPLE_ALT.GetHash();
        this.NotEqual(h1, h2);
    }


    // ─── Empty input ──────────────────────────────────────────────────────────

    [Test]
    public void Hash_SHA256_EmptyString_Deterministic()
    {
        string h1 = EMPTY_STRING.Hash_SHA256();
        string h2 = EMPTY_STRING.Hash_SHA256();
        this.IsTrue(h1.Length > 0);
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_SHA256_EmptyAndNonEmpty_Differ()
    {
        string emptyHash  = EMPTY_STRING.Hash_SHA256();
        string sampleHash = SAMPLE.Hash_SHA256();
        Assert.That(emptyHash, Is.Not.EqualTo(sampleHash));
    }


    // ─── IEnumerable hash ─────────────────────────────────────────────────────

    [Test]
    public void GetHash_Enumerable_Deterministic()
    {
        int[] data = [1, 2, 3, 4, 5];
        int   h1   = data.GetHash();
        int   h2   = data.GetHash();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void GetHash_Enumerable_DifferentOrders_ProduceDifferentHashes()
    {
        int[] data1 = [1, 2, 3];
        int[] data2 = [3, 2, 1];
        int   h1    = data1.GetHash();
        int   h2    = data2.GetHash();
        this.NotEqual(h1, h2);
    }


    // ─── Async overloads ──────────────────────────────────────────────────────

    [Test]
    public async Task HashAsync_MD5_Deterministic()
    {
        byte[]               data = Encoding.UTF8.GetBytes(SAMPLE);
        ReadOnlyMemory<byte> mem  = data;
        string               h1   = await mem.HashAsync_MD5().ConfigureAwait(false);
        string               h2   = await mem.HashAsync_MD5().ConfigureAwait(false);
        this.AreEqual(h1, h2);
    }

    [Test]
    public async Task HashAsync_SHA256_Deterministic()
    {
        byte[]               data = Encoding.UTF8.GetBytes(SAMPLE);
        ReadOnlyMemory<byte> mem  = data;
        string               h1   = await mem.HashAsync_SHA256().ConfigureAwait(false);
        string               h2   = await mem.HashAsync_SHA256().ConfigureAwait(false);
        this.AreEqual(h1, h2);
    }

    [Test]
    public async Task HashAsync_ByteArray_SHA512_Deterministic()
    {
        byte[] data = Encoding.UTF8.GetBytes(SAMPLE);
        string h1   = await data.HashAsync_SHA512().ConfigureAwait(false);
        string h2   = await data.HashAsync_SHA512().ConfigureAwait(false);
        this.AreEqual(h1, h2);
    }


    // ─── UInt128 hash ─────────────────────────────────────────────────────────

    [Test]
    public void Hash_UInt128_ByteSpan_Deterministic()
    {
        byte[]             data = Encoding.UTF8.GetBytes(SAMPLE);
        ReadOnlySpan<byte> span = data;
        UInt128            h1   = span.Hash();
        UInt128            h2   = span.Hash();
        this.AreEqual(h1, h2);
    }

    [Test]
    public void Hash_UInt128_CharSpan_Deterministic()
    {
        ReadOnlySpan<char> span = SAMPLE.AsSpan();
        UInt128            h1   = span.Hash(Encoding.UTF8);
        UInt128            h2   = span.Hash(Encoding.UTF8);
        this.AreEqual(h1, h2);
    }

    private static void NotEqual<T>( T x, T y ) => Assert.That(x, Is.Not.EqualTo(y));
}
