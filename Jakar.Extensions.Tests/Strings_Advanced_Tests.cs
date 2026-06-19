// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.Text;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Strings))]
public class Strings_Advanced_Tests : Assert
{
    // ─── IsBalanced ──────────────────────────────────────────────────────────

    [Test] [TestCase("()",     true)] [TestCase("{}",     true)] [TestCase("[]",     true)]
           [TestCase("(())",   true)] [TestCase("{[]()}",  true)] [TestCase("",        true)]
           [TestCase("(",      false)] [TestCase(")",      false)] [TestCase("([)]",   false)]
           [TestCase("{(})",   false)] [TestCase("(((",    false)]
    public void IsBalanced_DefaultBrackets( string input, bool expected ) =>
        this.AreEqual(expected, input.IsBalanced());

    [Test]
    public void IsBalanced_CustomBracketPairs()
    {
        Dictionary<char, char> customPairs = new()
                                             {
                                                 { '<', '>' },
                                             };

        this.IsTrue("<>".IsBalanced(customPairs));
        this.IsTrue("<<>>".IsBalanced(customPairs));
        this.IsFalse("<".IsBalanced(customPairs));
        this.IsFalse("><".IsBalanced(customPairs));
    }

    [Test]
    public void IsBalanced_MixedContent()
    {
        this.IsTrue("a(b{c}d)e".IsBalanced());
        this.IsFalse("a(b{c)d}e".IsBalanced());
    }

    [Test]
    public void IsBalanced_Span()
    {
        ReadOnlySpan<char> span = "(hello [world])".AsSpan();
        this.IsTrue(span.IsBalanced());

        ReadOnlySpan<char> bad = "((bad".AsSpan();
        this.IsFalse(bad.IsBalanced());
    }


    // ─── ContainsAbout / ContainsExact ───────────────────────────────────────

    [Test] [TestCase("Hello World", "hello",  true)] [TestCase("Hello World", "WORLD",  true)]
           [TestCase("Hello World", "xyz",    false)] [TestCase("",           "a",      false)]
    public void ContainsAbout( string source, string search, bool expected ) =>
        this.AreEqual(expected, source.ContainsAbout(search));

    [Test] [TestCase("Hello World", "Hello",  true)]  [TestCase("Hello World", "hello",  false)]
           [TestCase("Hello World", "World",  true)]  [TestCase("",            "a",      false)]
    public void ContainsExact( string source, string search, bool expected ) =>
        this.AreEqual(expected, source.ContainsExact(search));


    // ─── SplitAndTrimLines ────────────────────────────────────────────────────

    [Test]
    public void SplitAndTrimLines_TrimsWhitespace()
    {
        string   input  = "  hello  \n  world  \n  foo  ";
        string[] result = input.SplitAndTrimLines();
        this.AreEqual(3,       result.Length);
        this.AreEqual("hello", result[0]);
        this.AreEqual("world", result[1]);
        this.AreEqual("foo",   result[2]);
    }

    [Test]
    public void SplitAndTrimLines_CustomSeparator()
    {
        string   input  = "  a  ,  b  ,  c  ";
        string[] result = input.SplitAndTrimLines(',');
        this.AreEqual(3,   result.Length);
        this.AreEqual("a", result[0]);
        this.AreEqual("b", result[1]);
        this.AreEqual("c", result[2]);
    }

    [Test]
    public void SplitAndTrimLines_StringSeparator()
    {
        string   input  = "  x  ||  y  ||  z  ";
        string[] result = input.SplitAndTrimLines("||");
        this.AreEqual(3,   result.Length);
        this.AreEqual("x", result[0]);
        this.AreEqual("y", result[1]);
        this.AreEqual("z", result[2]);
    }

    [Test]
    public void SplitLines_DoesNotTrim()
    {
        string   input  = "  hello  \n  world  ";
        string[] result = input.SplitLines();
        this.AreEqual("  hello  ", result[0]);
        this.AreEqual("  world  ", result[1]);
    }


    // ─── Repeat (string) ─────────────────────────────────────────────────────

    [Test] [TestCase("abc", 0, "")] [TestCase("abc", 1, "abc")] [TestCase("ab", 3, "ababab")] [TestCase("x", 5, "xxxxx")]
    public void Repeat_String( string input, int count, string expected ) =>
        this.AreEqual(expected, input.Repeat(count));

    [Test] [TestCase('-', 0, "")] [TestCase('-', 1, "-")] [TestCase('*', 5, "*****")]
    public void Repeat_Char( char c, int count, string expected ) =>
        this.AreEqual(expected, c.Repeat(count));


    // ─── ReplaceAll / RemoveAll ───────────────────────────────────────────────

    [Test] [TestCase("hello world", "world", "earth", "hello earth")]
           [TestCase("aaa",         "a",     "b",     "bbb")]
           [TestCase("no match",    "xyz",   "abc",   "no match")]
    public void ReplaceAll_String( string input, string old, string newStr, string expected ) =>
        this.AreEqual(expected, input.ReplaceAll(old, newStr));

    [Test] [TestCase("hello", 'l', 'r', "herro")]
           [TestCase("aaa",   'a', 'b', "bbb")]
    public void ReplaceAll_Char( string input, char old, char newChar, string expected ) =>
        this.AreEqual(expected, input.ReplaceAll(old, newChar));

    [Test] [TestCase("hello world", "world", "hello ")] [TestCase("aaa", "a", "")] [TestCase("abc", "x", "abc")]
    public void RemoveAll_String( string input, string old, string expected ) =>
        this.AreEqual(expected, input.RemoveAll(old));

    [Test] [TestCase("hello", 'l', "heo")] [TestCase("aaa", 'a', "")] [TestCase("abc", 'z', "abc")]
    public void RemoveAll_Char( string input, char old, string expected ) =>
        this.AreEqual(expected, input.RemoveAll(old));


    // ─── ToScreamingCase ──────────────────────────────────────────────────────

    [Test] [TestCase("helloWorld",   "HELLO_WORLD")]
           [TestCase("hello world",  "HELLO_WORLD")]
           [TestCase("FOO_BAR",      "FOO_BAR")]
           [TestCase("myVariable",   "MY_VARIABLE")]
    public void ToScreamingCase( string input, string expected ) =>
        this.AreEqual(expected, input.ToScreamingCase());


    // ─── Wrapper ──────────────────────────────────────────────────────────────

    // Wrapper uses PadLeft(padding) then PadRight(padding), where padding is the TOTAL width.
    // So it left-pads the string to reach at least `padding` chars in total length.
    [Test] [TestCase("hi",    '-', 5, "---hi")] // padded left to total width 5
           [TestCase("",      '*', 4, "****")]  // empty → 4 stars
           [TestCase("hello", '-', 3, "hello")] // longer than 3, no padding applied
    public void Wrapper( string input, char c, int padding, string expected ) =>
        this.AreEqual(expected, input.Wrapper(c, padding));


    // ─── ConvertTo<T> ─────────────────────────────────────────────────────────

    [Test]
    public void ConvertTo_Int()
    {
        int result = "42".ConvertTo<int>();
        this.AreEqual(42, result);
    }

    [Test]
    public void ConvertTo_Double()
    {
        double result = "3.14".ConvertTo<double>();
        Assert.That(result, Is.EqualTo(3.14).Within(1e-10));
    }

    [Test]
    public void ConvertTo_Bool()
    {
        bool t = "True".ConvertTo<bool>();
        bool f = "False".ConvertTo<bool>();
        this.IsTrue(t);
        this.IsFalse(f);
    }

    [Test]
    public void ConvertTo_InvalidThrows()
    {
        Throws<Exception>(() => _ = "notAnInt".ConvertTo<int>());
    }


    // ─── ToMemory / ToReadOnlyMemory / ToByteArray ───────────────────────────

    [Test]
    public void ToByteArray_RoundtripsWithEncoding()
    {
        string text  = "Hello";
        byte[] bytes = text.ToByteArray(Encoding.UTF8);
        string back  = Encoding.UTF8.GetString(bytes);
        this.AreEqual(text, back);
    }

    [Test]
    public void ToMemory_ProducesCorrectBytes()
    {
        string       text  = "Hi";
        Memory<byte> mem   = text.ToMemory(Encoding.UTF8);
        byte[]       bytes = mem.ToArray();
        this.AreEqual(Encoding.UTF8.GetBytes(text).Length, bytes.Length);
    }
}
