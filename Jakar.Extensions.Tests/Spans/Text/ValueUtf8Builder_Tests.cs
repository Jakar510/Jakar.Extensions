// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/06/2026

using System.Globalization;
using System.Text;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(ValueUtf8Builder))]
public class ValueUtf8Builder_Tests : Assert
{
    private static readonly CultureInfo __invariant = CultureInfo.InvariantCulture;


    // ─── Content / chaining / growth ─────────────────────────────────────────

    [Test] public void ToArray_ReturnsOnlyTheContent()
    {
        ValueUtf8Builder sb = new(64);
        sb.Append("abc"u8);
        this.AreEqual("abc"u8.ToArray(), sb.ToArray());
    }

    [Test] public void ToString_DecodesUtf8()
    {
        ValueUtf8Builder sb = new(64);
        sb.Append("héllo €"u8);
        this.AreEqual("héllo €", sb.ToString());
    }

    [Test] public void ChainedCalls_AllApplyToTheSameBuilder()
    {
        using ValueUtf8Builder sb = new(4);
        sb.Append("a"u8).Append((byte)'b').Append('c').Append("d").Append((byte)'-', 2).Append("ef".AsSpan());

        this.AreEqual("abcd--ef", Encoding.UTF8.GetString(sb.Values));
        this.AreEqual(8,          sb.Length);
    }

    [Test] public void Grows_KeepingContent_FromStackAndPool()
    {
        ValueUtf8Builder stack = new(stackalloc byte[4]);
        ValueUtf8Builder pool  = new(4);

        for ( int i = 0; i < 1000; i++ )
        {
            stack.Append((byte)( 'a' + i % 26 ));
            pool.Append((byte)( 'a'  + i % 26 ));
        }

        byte[] expected = new byte[1000];
        for ( int i = 0; i < expected.Length; i++ ) { expected[i] = (byte)( 'a' + i % 26 ); }

        this.AreEqual(expected, stack.ToArray());
        this.AreEqual(expected, pool.ToArray());
    }

    [Test] public void Dispose_IsIdempotent_AndResetsTheBuilder()
    {
        ValueUtf8Builder sb = new(16);
        sb.Append("abc"u8);
        sb.Dispose();
        sb.Dispose();

        this.AreEqual(0, sb.Length);
        this.AreEqual(0, sb.Capacity);
    }

    [Test] public void Reset_ClearsContent_KeepsStorage()
    {
        using ValueUtf8Builder sb = new(32);
        sb.Append("abc"u8);
        int capacity = sb.Capacity;
        sb.Reset().Append("x"u8);

        this.AreEqual("x",      Encoding.UTF8.GetString(sb.Values));
        this.AreEqual(capacity, sb.Capacity);
    }


    // ─── UTF-16 transcoding ──────────────────────────────────────────────────

    [TestCase("")] [TestCase("plain ascii")] [TestCase("héllo wörld")] [TestCase("€ ✓ 日本語")] [TestCase("emoji 😀 pair 👍🏽")]
    public void AppendChars_MatchesEncodingUtf8( string text )
    {
        using ValueUtf8Builder sb = new(stackalloc byte[2]); // force growth mid-transcode
        sb.Append(text.AsSpan());
        this.AreEqual(Encoding.UTF8.GetBytes(text), sb.Values.ToArray());
    }

    [Test] public void AppendChars_GrowsAcrossMultiByteSequences()
    {
        string text = new StringBuilder().Insert(0, "日本😀", 500).ToString();

        using ValueUtf8Builder sb = new(stackalloc byte[16]);
        sb.Append("x"u8).Append(text);
        this.AreEqual(Encoding.UTF8.GetBytes("x" + text), sb.Values.ToArray());
    }

    [TestCase('a')] [TestCase('é')] [TestCase('€')] [TestCase('\u007F')] [TestCase('\u0080')] [TestCase('￿')]
    public void AppendChar_MatchesEncodingUtf8( char c )
    {
        using ValueUtf8Builder sb = new(stackalloc byte[1]);
        sb.Append(c);
        this.AreEqual(Encoding.UTF8.GetBytes(c.ToString()), sb.Values.ToArray());
    }

    [Test] public void LoneSurrogates_BecomeReplacementChar()
    {
        byte[] replacement = [0xEF, 0xBF, 0xBD];

        using ValueUtf8Builder single = new(8);
        single.Append('\uD800');
        this.AreEqual(replacement, single.Values.ToArray());

        using ValueUtf8Builder span = new(8);
        span.Append("a\uDC00b".AsSpan());
        byte[] expected = [(byte)'a', .. replacement, (byte)'b'];
        this.AreEqual(expected, span.Values.ToArray());
    }

    [Test] public void AppendNullString_DoesNothing()
    {
        using ValueUtf8Builder sb = new(8);
        sb.Append((string?)null);
        this.IsTrue(sb.IsEmpty);
    }


    // ─── Formatting ──────────────────────────────────────────────────────────

    [Test] public void AppendUtf8Formattable_FormatsInPlace()
    {
        using ValueUtf8Builder sb = new(stackalloc byte[2]);
        sb.AppendUtf8Formattable(12345).Append(','.ToString()).AppendUtf8Formattable(3.5, "F2", __invariant).Append((byte)',').AppendUtf8Formattable(new DateTime(2026, 10, 6), "yyyy-MM-dd", __invariant);

        this.AreEqual("12345,3.50,2026-10-06", Encoding.UTF8.GetString(sb.Values));
    }

    [Test] public void AppendJoin_WritesSeparators()
    {
        using ValueUtf8Builder sb = new(4);
        sb.AppendJoin((byte)',', [1, 22, 333]).Append(" | "u8).AppendJoin(", "u8, [1.5, 2.25], default, __invariant);

        this.AreEqual("1,22,333 | 1.5, 2.25", Encoding.UTF8.GetString(sb.Values));
    }


    // ─── Edit ────────────────────────────────────────────────────────────────

    [Test] public void Trim_RemovesBothEnds()
    {
        using ValueUtf8Builder sb = new("  abc  "u8);
        sb.Trim((byte)' ');
        this.AreEqual("abc", Encoding.UTF8.GetString(sb.Values));
    }

    [Test] public void Insert_ShiftsContent()
    {
        using ValueUtf8Builder sb = new(stackalloc byte[4]);
        sb.Append("ad"u8).Insert(1, "bc"u8).Insert(0, (byte)'>', 2);
        this.AreEqual(">>abcd", Encoding.UTF8.GetString(sb.Values));
    }

    [Test] public void Replace_OverwritesInPlace()
    {
        using ValueUtf8Builder sb = new("abcdef"u8);
        sb.Replace(0, (byte)'A').Replace(1, (byte)'-', 2).Replace(4, "EF"u8);
        this.AreEqual("A--dEF", Encoding.UTF8.GetString(sb.Values));
    }


    // ─── Output ──────────────────────────────────────────────────────────────

    [Test] public void TryFormat_Utf8_CopiesOrFails()
    {
        using ValueUtf8Builder sb = new("abc"u8);

        Span<byte> big = stackalloc byte[8];
        this.IsTrue(sb.TryFormat(big, out int written, default, null));
        this.AreEqual(3, written);

        Span<byte> small = stackalloc byte[2];
        this.IsFalse(sb.TryFormat(small, out written, default, null));
        this.AreEqual(0, written);
    }

    [Test] public void TryFormat_Utf16_Decodes()
    {
        using ValueUtf8Builder sb = new(16);
        sb.Append("é€");

        Span<char> chars = stackalloc char[8];
        this.IsTrue(sb.TryFormat(chars, out int written, default, null));
        this.AreEqual("é€", chars[..written].ToString());

        Span<char> small = stackalloc char[1];
        this.IsFalse(sb.TryFormat(small, out written, default, null));
        this.AreEqual(0, written);
    }

    [Test] public void TryCopyTo_Disposes()
    {
        ValueUtf8Builder sb   = new("abc"u8);
        Span<byte>       dest = new byte[3];

        this.IsTrue(sb.TryCopyTo(ref dest, out int written));
        this.AreEqual(3, written);
        this.AreEqual(0, sb.Capacity);
    }
}
