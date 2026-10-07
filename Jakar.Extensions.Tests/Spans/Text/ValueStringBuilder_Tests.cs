// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Buffers;
using System.Collections.Generic;
using System.Globalization;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(ValueStringBuilder))]
public class ValueStringBuilder_Tests : Assert
{
    private static readonly CultureInfo __invariant = CultureInfo.InvariantCulture;
    private static readonly int[]       __ints      = [1, 22, 333];


    // ─── Content / chaining / growth ─────────────────────────────────────────

    [Test]
    public void ToString_ReturnsOnlyTheContent()
    {
        ValueStringBuilder sb = new(64);
        sb.Append("abc");
        this.AreEqual("abc", sb.ToString());
    }

    [Test]
    public void ChainedCalls_AllApplyToTheSameBuilder()
    {
        using ValueStringBuilder sb = new(4);
        sb.Append("a").Append('b').Append("cd".AsSpan()).Append('-', 2).Append("ef", "gh");

        this.AreEqual("abcd--efgh", sb.Values.ToString());
        this.AreEqual(10,           sb.Length);
    }

    [Test]
    public void ChainOnATemporary_Works() => this.AreEqual("\"x\".y", new ValueStringBuilder().Append('"').Append("x").Append('"').Append('.').Append("y").ToString());

    [Test]
    public void Grows_KeepingContent_FromStackAndPool()
    {
        ValueStringBuilder stack = new(stackalloc char[4]);
        ValueStringBuilder pool  = new(4);

        for ( int i = 0; i < 1000; i++ )
        {
            stack.Append((char)( 'a' + i % 26 ));
            pool.Append((char)( 'a' + i % 26 ));
        }

        string expected = string.Create(1000, 0, static ( span, _ ) => { for ( int i = 0; i < span.Length; i++ ) { span[i] = (char)( 'a' + i % 26 ); } });
        this.AreEqual(expected, stack.ToString());
        this.AreEqual(expected, pool.ToString());
    }

    [Test]
    public void InitialContentConstructor()
    {
        ValueStringBuilder sb = new("DELETE FROM ");
        sb.Append("table");
        this.AreEqual("DELETE FROM table", sb.ToString());
    }

    [Test]
    public void SpanProperties_ExposeContent_RawCharsExposesCapacity()
    {
        using ValueStringBuilder sb = new(stackalloc char[16]);
        sb.Append("hello");

        this.AreEqual("hello", sb.Span.ToString());
        this.AreEqual("hello", sb.Result.ToString());
        this.AreEqual("hello", sb.AsSpan().ToString());
        this.AreEqual(16,      sb.RawChars.Length);
        this.AreEqual(11,      sb.Next.Length);
        this.AreEqual('e',     sb[1]);
        this.AreEqual('o',     sb[^1]);
        this.AreEqual("ell",   sb[1..4].ToString());
        this.AreEqual("llo",   sb.Slice(2).ToString());
    }

    [Test]
    public void Reset_ClearsContent()
    {
        using ValueStringBuilder sb = new(16);
        sb.Append("abc").Reset().Append("x");
        this.AreEqual("x", sb.Values.ToString());
    }

    [Test]
    public void Length_IsClamped()
    {
        ValueStringBuilder sb = new(stackalloc char[8]); // not a using variable: Length is assigned
        sb.Append("abcdef");
        sb.Length = 3;
        this.AreEqual("abc", sb.Values.ToString());
        sb.Length = 100;
        this.AreEqual(8, sb.Length);
        sb.Length = -1;
        this.AreEqual(0, sb.Length);
        sb.Dispose();
    }


    // ─── Insert / Replace / Trim ─────────────────────────────────────────────

    [Test]
    public void Insert_AtStartMiddleAndEnd_GrowingAsNeeded()
    {
        using ValueStringBuilder sb = new(stackalloc char[4]);
        sb.Append("ace").Insert(1, 'b').Insert(3, "d".AsSpan()).Insert(0, '>', 2).Insert(7, "!!".AsSpan());

        this.AreEqual(">>abcde!!", sb.Values.ToString());
    }

    [Test]
    public void Insert_OutOfRange_Throws()
    {
        Throws<ArgumentOutOfRangeException>(static () =>
                                                   {
                                                       using ValueStringBuilder sb = new(8);
                                                       sb.Append("ab").Insert(3, 'x');
                                                   });
    }

    [Test]
    public void Replace_OverwritesWithinContent()
    {
        using ValueStringBuilder sb = new(16);
        sb.Append("abcdef").Replace(0, 'X').Replace(1, '-', 2).Replace(4, "YZ".AsSpan());
        this.AreEqual("X--dYZ", sb.Values.ToString());
    }

    [Test]
    public void Trim_Variants()
    {
        using ValueStringBuilder a = new(32);
        a.Append("  hello  ").Trim(' ');
        this.AreEqual("hello", a.Values.ToString());

        using ValueStringBuilder b = new(32);
        b.Append("-_x_-").TrimStart('-', '_');
        this.AreEqual("x_-", b.Values.ToString());

        using ValueStringBuilder c = new(32);
        c.Append("-_x_-").TrimEnd('-', '_');
        this.AreEqual("-_x", c.Values.ToString());

        using ValueStringBuilder d = new(32);
        d.Append(" x ").Trim(ReadOnlySpan<char>.Empty); // empty set trims nothing
        this.AreEqual(" x ", d.Values.ToString());

        using ValueStringBuilder e = new(32);
        e.Append("aaaa").Trim('a');
        this.AreEqual("", e.Values.ToString());
    }


    // ─── Formatting ──────────────────────────────────────────────────────────

    [Test]
    public void AppendFormat_MatchesStringFormat()
    {
        using ValueStringBuilder sb = new(stackalloc char[8]);
        sb.AppendFormat("{0} + {1} = {2}", 1, 2, 3, __invariant);
        this.AreEqual(string.Format(__invariant, "{0} + {1} = {2}", 1, 2, 3), sb.Values.ToString());
    }

    [TestCase("[{0,6}]")]
    [TestCase("[{0,-6}]")]
    [TestCase("[{0:N2}]")]
    [TestCase("[{0,10:N2}|{0,-10:F1}]")]
    [TestCase("{{literal}} {0}")]
    [TestCase("}}{0}{{")]
    [TestCase("{0 , 4 }")]
    public void AppendFormat_AlignmentFormatsAndEscapes( string format )
    {
        using ValueStringBuilder sb = new(stackalloc char[4]);
        sb.AppendFormat(format, 1234.5678, __invariant);
        this.AreEqual(string.Format(__invariant, format, 1234.5678), sb.Values.ToString());
    }

    [Test]
    public void AppendFormat_ParamsOverload()
    {
        using ValueStringBuilder sb = new(16);
        sb.AppendFormat("{3}{2}{1}{0}", __invariant, 1, 2, 3, 4);
        this.AreEqual("4321", sb.Values.ToString());
    }

    [TestCase("{")]
    [TestCase("}")]
    [TestCase("{0")]
    [TestCase("{x}")]
    [TestCase("{0,}")]
    [TestCase("{0:{}")]
    public void AppendFormat_InvalidFormat_Throws( string format ) => Throws<FormatException>(() =>
                                                                                                     {
                                                                                                         using ValueStringBuilder sb = new(16);
                                                                                                         sb.AppendFormat(format, 1);
                                                                                                     });

    [Test]
    public void AppendFormat_IndexOutOfRange_Throws() => Throws<FormatException>(static () =>
                                                                                         {
                                                                                             using ValueStringBuilder sb = new(16);
                                                                                             sb.AppendFormat("{1}", 1);
                                                                                         });

    [Test]
    public void AppendFormat_UsesCustomFormatter()
    {
        using ValueStringBuilder sb = new(16);
        sb.AppendFormat("<{0,4}>", 7, new Upper());
        this.AreEqual("<  #7>", sb.Values.ToString());
    }

    [Test]
    public void AppendSpanFormattable_GrowsInsteadOfFailing()
    {
        using ValueStringBuilder sb = new(stackalloc char[2]);
        DateTime                 now = new(2026, 9, 30, 12, 34, 56, DateTimeKind.Utc);
        sb.AppendSpanFormattable(now, "o", __invariant).AppendSpanFormattable(Guid.Empty, default);

        this.AreEqual(now.ToString("o", __invariant) + Guid.Empty, sb.Values.ToString());
    }


    // ─── Join ────────────────────────────────────────────────────────────────

    [Test]
    public void AppendJoin_Strings()
    {
        using ValueStringBuilder sb = new(stackalloc char[2]);
        sb.AppendJoin(',', "a", null!, "ccc").Append('|').AppendJoin(", ".AsSpan(), "x", "y");
        this.AreEqual("a,,ccc|x, y", sb.Values.ToString());
    }

    [Test]
    public void AppendJoin_Formattables_DoNotLoopWhenFull()
    {
        using ValueStringBuilder sb   = new(stackalloc char[3]);
        ReadOnlySpan<int>        ints = [10000, 20000, 30000];
        sb.AppendJoin(';', ints).Append('|').AppendJoin(" - ".AsSpan(), ints, "N0", __invariant);

        this.AreEqual("10000;20000;30000|10,000 - 20,000 - 30,000", sb.Values.ToString());
    }

    [Test]
    public void AppendJoin_Enumerable()
    {
        using ValueStringBuilder sb = new(4);
        sb.AppendJoin(',', new List<int> { 1, 2, 3 }).Append(' ').AppendJoin("::".AsSpan(), new List<double> { 1.5, 2.5 }, default, __invariant);
        this.AreEqual("1,2,3 1.5::2.5", sb.Values.ToString());
    }


    // ─── Lifetime ────────────────────────────────────────────────────────────

    [Test]
    public void ToStringThenUsingDispose_DoesNotReturnTheArrayTwice()
    {
        const int SIZE = 3000; // a bucket no other test uses

        using ( ValueStringBuilder sb = new(SIZE) )
        {
            sb.Append("hello");
            this.AreEqual("hello", sb.ToString());
        }

        char[] first  = ArrayPool<char>.Shared.Rent(SIZE);
        char[] second = ArrayPool<char>.Shared.Rent(SIZE);

        try { Assert.AreNotSame(first, second); }
        finally
        {
            ArrayPool<char>.Shared.Return(first);
            ArrayPool<char>.Shared.Return(second);
        }
    }

    [Test]
    public void TryCopyTo_And_TryFormat()
    {
        Span<char>         destination = new char[8]; // TryCopyTo's scoped-ref signature can't take a stackalloc span here
        ValueStringBuilder sb          = new(8);
        sb.Append("abc");

        this.IsTrue(sb.TryFormat(destination, out int formatted, default, null));
        this.AreEqual(3, formatted);

        this.IsTrue(sb.TryCopyTo(ref destination, out int copied));
        this.AreEqual(3,     copied);
        this.AreEqual("abc", destination[..3].ToString());
        this.AreEqual(0,     sb.Length); // disposed
    }

    [Test]
    public void GetPinnableReference_Terminate_WritesAfterLengthWithoutChangingIt()
    {
        using ValueStringBuilder sb = new(stackalloc char[3]);
        sb.Append("abc");
        ref char start = ref sb.GetPinnableReference(true);

        this.AreEqual('a',  start);
        this.AreEqual(3,    sb.Length);
        this.AreEqual('\0', sb.RawChars[3]);
    }

    [Test]
    public void IsNullOrWhiteSpace_UsesContent()
    {
        using ValueStringBuilder blank = new(64);
        blank.Append("   ");
        this.IsTrue(blank.IsNullOrWhiteSpace());

        using ValueStringBuilder text = new(64);
        text.Append(" x ");
        this.IsFalse(text.IsNullOrWhiteSpace());
    }


    // ─── Zero allocation ─────────────────────────────────────────────────────

    [Test]
    public void BuildingFromAStackBuffer_AllocatesNothing()
    {
        build(); // warm up JIT, statics, culture data and the pool

        long before = GC.GetAllocatedBytesForCurrentThread();
        int  length = build();
        long after  = GC.GetAllocatedBytesForCurrentThread();

        this.AreEqual(0L, after - before);
        this.IsTrue(length > 64); // the workload grows past the stack buffer into a pooled array
        return;

        static int build()
        {
            using ValueStringBuilder sb   = new(stackalloc char[256]);
            ReadOnlySpan<int>        ints = __ints; // not a collection expression: in Debug builds `[1, 22, 333]` allocates an array on every call

            sb.Append("id=").AppendSpanFormattable(12345, default, __invariant)
              .Append(';').Append(' ', 2)
              .AppendFormat("[{0,8:F2}|{1,-6}]", 3.14159, 2.5, __invariant)
              .AppendJoin(',', ints)
              .AppendJoin(", ".AsSpan(), "a", "b", "c")
              .Insert(0, '>')
              .AppendSpanFormattable(new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc), "O", __invariant)
              .TrimEnd('Z')
              .Replace(1, 'I');

            return sb.Length;
        }
    }

    [Test]
    public void BuildingFromAPooledArray_AllocatesNothing_AfterWarmUp()
    {
        build();

        long before = GC.GetAllocatedBytesForCurrentThread();
        build();
        long after = GC.GetAllocatedBytesForCurrentThread();

        this.AreEqual(0L, after - before);
        return;

        static void build()
        {
            using ValueStringBuilder sb = new();
            for ( int i = 0; i < 200; i++ ) { sb.AppendSpanFormattable(i, default, __invariant).Append(' '); }
        }
    }



    private sealed class Upper : IFormatProvider, ICustomFormatter
    {
        public object? GetFormat( Type? formatType ) => formatType == typeof(ICustomFormatter)
                                                            ? this
                                                            : null;
        public string Format( string? format, object? arg, IFormatProvider? formatProvider ) => $"#{arg}";
    }
}
