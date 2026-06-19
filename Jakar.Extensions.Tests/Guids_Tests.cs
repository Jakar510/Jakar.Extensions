// Jakar.Extensions :: Jakar.Extensions.Tests



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Guids))]
public class Guids_Tests : Assert
{
    private static readonly Guid SAMPLE_GUID = new("5BE6F8AE-33D3-4E82-84EC-B7B89064F36A");


    // ─── AsGuid (string) ──────────────────────────────────────────────────────

    [Test]
    public void AsGuid_String_KnownGuid()
    {
        string input  = SAMPLE_GUID.ToString();
        Guid?  result = input.AsGuid();
        this.NotNull(result);
        this.AreEqual(SAMPLE_GUID, result!.Value);
    }

    [Test]
    public void AsGuid_String_InvalidReturnsEmpty()
    {
        Guid? result = "not-a-guid".AsGuid();
        // Invalid strings that aren't valid base64-encoded GUIDs return Guid.Empty
        this.NotNull(result);
        this.AreEqual(Guid.Empty, result!.Value);
    }

    [Test]
    public void AsGuid_EmptyString_ReturnsEmpty()
    {
        Guid? result = string.Empty.AsGuid();
        this.NotNull(result);
        this.AreEqual(Guid.Empty, result!.Value);
    }

    [Test]
    public void TryAsGuid_ValidString_ReturnsTrue()
    {
        string input   = SAMPLE_GUID.ToString();
        bool   success = input.TryAsGuid(out Guid? result);
        this.IsTrue(success);
        this.NotNull(result);
        this.AreEqual(SAMPLE_GUID, result!.Value);
    }


    // ─── ToBase64 / AsGuid roundtrip ─────────────────────────────────────────

    [Test]
    public void ToBase64_Roundtrip()
    {
        Guid   original = Guid.NewGuid();
        string b64      = original.ToBase64();

        this.IsTrue(b64.Length > 0);

        // The result should be parseable back (via AsGuid which handles URL-safe base64)
        Guid? recovered = b64.AsGuid();
        this.NotNull(recovered);
        this.AreEqual(original, recovered!.Value);
    }

    [Test]
    public void NewBase64_ProducesNonEmptyString()
    {
        string result = Guids.NewBase64();
        this.IsTrue(result.Length > 0);
    }

    [Test]
    public void NewBase64_ProducesUniqueValues()
    {
        string first  = Guids.NewBase64();
        string second = Guids.NewBase64();
        this.NotEqual(first, second);
    }

    [Test]
    public void NewBase64_SpecificGuid_Roundtrip()
    {
        string b64  = SAMPLE_GUID.NewBase64();
        Guid?  back = b64.AsGuid();
        this.NotNull(back);
        this.AreEqual(SAMPLE_GUID, back!.Value);
    }


    // ─── ToHex ────────────────────────────────────────────────────────────────

    [Test]
    public void ToHex_ProducesHexString()
    {
        string hex = SAMPLE_GUID.ToHex();
        this.IsTrue(hex.Length > 0);
        // Hex strings consist only of 0-9 and A-F
        foreach ( char c in hex )
        {
            this.IsTrue(char.IsAsciiHexDigit(c));
        }
    }

    [Test]
    public void ToHex_DifferentGuids_DifferentHex()
    {
        Guid   g1   = Guid.NewGuid();
        Guid   g2   = Guid.NewGuid();
        string hex1 = g1.ToHex();
        string hex2 = g2.ToHex();
        this.NotEqual(hex1, hex2);
    }


    // ─── AsLong / AsULong roundtrips ──────────────────────────────────────────

    [Test]
    public void AsLong_Roundtrip()
    {
        Guid                    original        = Guid.NewGuid();
        (long Lower, long Upper) pair           = original.AsLong();
        Guid                    recovered       = pair.AsGuid();
        this.AreEqual(original, recovered);
    }

    [Test]
    public void AsULong_Roundtrip()
    {
        Guid                       original  = Guid.NewGuid();
        (ulong Lower, ulong Upper) pair      = original.AsULong();
        Guid                       recovered = pair.AsGuid();
        this.AreEqual(original, recovered);
    }

    [Test]
    public void AsLong_KnownGuid_OutParams()
    {
        bool success = SAMPLE_GUID.AsLong(out long lower, out long upper);
        this.IsTrue(success);
        // Reconstruct and verify roundtrip
        Guid recovered = ( lower, upper ).AsGuid();
        this.AreEqual(SAMPLE_GUID, recovered);
    }

    [Test]
    public void AsLong_ULong_OutParams_Roundtrip()
    {
        Guid original = Guid.NewGuid();
        bool success  = original.AsLong(out ulong lower, out ulong upper);
        this.IsTrue(success);
        Guid recovered = ( lower, upper ).AsGuid();
        this.AreEqual(original, recovered);
    }


    // ─── Int128 / UInt128 roundtrips ──────────────────────────────────────────

    [Test]
    public void AsInt128_Roundtrip()
    {
        Guid   original  = Guid.NewGuid();
        Int128 int128    = original.AsInt128();
        Guid   recovered = int128.AsGuid();
        this.AreEqual(original, recovered);
    }

    [Test]
    public void AsUInt128_Roundtrip()
    {
        Guid    original  = Guid.NewGuid();
        UInt128 uint128   = original.AsUInt128();
        Guid    recovered = uint128.AsGuid();
        this.AreEqual(original, recovered);
    }


    // ─── long → Guid ─────────────────────────────────────────────────────────

    [Test] [TestCase(0L)] [TestCase(1L)] [TestCase(long.MaxValue)] [TestCase(long.MinValue)]
    public void Long_AsGuid_DoesNotThrow( long value ) => DoesNotThrow(() => _ = value.AsGuid());

    [Test] [TestCase(0UL)] [TestCase(1UL)] [TestCase(ulong.MaxValue)]
    public void ULong_AsGuid_DoesNotThrow( ulong value ) => DoesNotThrow(() => _ = value.AsGuid());


    // ─── TryWriteBytes ────────────────────────────────────────────────────────

    [Test]
    public void TryWriteBytes_ReturnsBuffer()
    {
        bool success = SAMPLE_GUID.TryWriteBytes(out Buffer<byte> buffer);
        using ( buffer )
        {
            this.IsTrue(success);
            this.AreEqual(16, buffer.Length);
        }
    }

    private static void NotEqual<T>( T x, T y ) => Assert.That(x, Is.Not.EqualTo(y));
}
