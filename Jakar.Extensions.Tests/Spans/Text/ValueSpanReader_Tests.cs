// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/06/2026

using System.Buffers;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(ValueSpanReader<>))]
public class ValueSpanReader_Tests : Assert
{
    private static readonly SearchValues<char> __whitespace     = SearchValues.Create(" \t\r\n");
    private static readonly SearchValues<byte> __whitespaceUtf8 = SearchValues.Create(" \t\r\n"u8);
    private static readonly SearchValues<char> __delimiters     = SearchValues.Create(",]");


    // ─── Peek / Read ─────────────────────────────────────────────────────────

    [Test] public void ReadsEveryElement_InOrder()
    {
        ValueSpanReader<char> reader = new("abc");

        this.AreEqual('a', reader.Peek());
        this.AreEqual('a', reader.Read());
        this.IsTrue(reader.TryRead(out char b));
        this.AreEqual('b', b);
        this.IsTrue(reader.TryRead(out char c));
        this.AreEqual('c', c);

        this.IsTrue(reader.End);
        this.IsFalse(reader.TryRead(out _));
        this.IsFalse(reader.TryPeek(out _));
        this.AreEqual(3, reader.Position);
    }

    [Test] public void PeekAndRead_AtEnd_Throw()
    {
        Throws<InvalidOperationException>(static () => new ValueSpanReader<char>("").Peek());
        Throws<InvalidOperationException>(static () => new ValueSpanReader<char>("").Read());
    }

    [Test] public void TryPeekOffset_LooksAhead_WithoutConsuming()
    {
        ValueSpanReader<char> reader = new("abc");
        reader.Advance(1);

        this.IsTrue(reader.TryPeek(1, out char c));
        this.AreEqual('c', c);
        this.IsFalse(reader.TryPeek(2,            out _));
        this.IsFalse(reader.TryPeek(-1,           out _));
        this.IsFalse(reader.TryPeek(int.MaxValue, out _));
        this.AreEqual(1, reader.Position);
    }

    [Test] public void TryReadCount_ReadsOrConsumesNothing()
    {
        ValueSpanReader<char> reader = new("abcd");

        this.IsTrue(reader.TryRead(3, out ReadOnlySpan<char> values));
        this.AreEqual("abc", values.ToString());
        this.IsFalse(reader.TryRead(2,  out _));
        this.IsFalse(reader.TryRead(-1, out _));
        this.AreEqual(3, reader.Position);
    }


    // ─── Exact matches ───────────────────────────────────────────────────────

    [Test] public void TryReadExact_ConsumesOnlyOnMatch()
    {
        ValueSpanReader<byte> reader = new("true,"u8);

        this.IsFalse(reader.TryReadExact("false"u8));
        this.AreEqual(0, reader.Position);
        this.IsTrue(reader.IsNext("tr"u8));
        this.IsTrue(reader.TryReadExact("true"u8));
        this.IsTrue(reader.IsNext((byte)','));
        this.IsTrue(reader.TryReadExact((byte)','));
        this.IsTrue(reader.End);
        this.IsFalse(reader.TryReadExact((byte)','));
    }

    [Test] public void TryReadExact_LongerThanInput_IsFalse()
    {
        ValueSpanReader<char> reader = new("nu");
        this.IsFalse(reader.TryReadExact("null"));
        this.AreEqual(0, reader.Position);
    }


    // ─── Delimited reads ─────────────────────────────────────────────────────

    [Test] public void TryReadTo_SplitsOnDelimiter()
    {
        ValueSpanReader<char> reader = new("key=value");

        this.IsTrue(reader.TryReadTo('=', out ReadOnlySpan<char> key));
        this.AreEqual("key",   key.ToString());
        this.AreEqual("value", reader.Remaining.ToString());

        this.IsFalse(reader.TryReadTo('=', out _));
        this.AreEqual(4, reader.Position);
    }

    [Test] public void TryReadTo_CanStopBeforeDelimiter()
    {
        ValueSpanReader<char> reader = new("ab\"c");
        this.IsTrue(reader.TryReadTo('"', out ReadOnlySpan<char> values, advancePastDelimiter: false));
        this.AreEqual("ab", values.ToString());
        this.AreEqual('"',  reader.Peek());
    }

    [Test] public void TryReadToAny_LeavesTheDelimiter()
    {
        ValueSpanReader<char> reader = new("123]");
        this.IsTrue(reader.TryReadToAny(__delimiters, out ReadOnlySpan<char> number));
        this.AreEqual("123", number.ToString());
        this.AreEqual(']',   reader.Read());
        this.IsFalse(reader.TryReadToAny(__delimiters, out _));
    }


    // ─── Search / skip ───────────────────────────────────────────────────────

    [Test] public void SkipWhile_SkipsWhitespace_ForCharsAndBytes()
    {
        ValueSpanReader<char> chars = new(" \t\r\n{ }");
        this.AreEqual(4,   chars.SkipWhile(__whitespace));
        this.AreEqual('{', chars.Read());
        this.AreEqual(1,   chars.SkipWhile(__whitespace));
        this.AreEqual(0,   chars.SkipWhile(__whitespace));

        ValueSpanReader<byte> bytes = new("  \n"u8);
        this.AreEqual(3, bytes.SkipWhile(__whitespaceUtf8));
        this.IsTrue(bytes.End);
    }

    [Test] public void SkipWhile_SingleValue()
    {
        ValueSpanReader<int> reader = new([0, 0, 0, 7]);
        this.AreEqual(3, reader.SkipWhile(0));
        this.AreEqual(7, reader.Read());
    }

    [Test] public void IndexOf_IsRelativeToPosition()
    {
        ValueSpanReader<char> reader = new("a,b,c");
        reader.Advance(2);

        this.AreEqual(1,  reader.IndexOf(','));
        this.AreEqual(1,  reader.IndexOfAny(__delimiters));
        this.AreEqual(0,  reader.IndexOfAnyExcept(__delimiters));
        this.AreEqual(-1, reader.IndexOf('x'));
        this.AreEqual(2,  reader.Position);
    }


    // ─── Move ────────────────────────────────────────────────────────────────

    [Test] public void RewindToCheckpoint_ReplaysInput()
    {
        ValueSpanReader<char> reader = new("{\"a\":1}");
        reader.Advance(1);

        int mark = reader.Position;
        this.IsTrue(reader.TryReadExact("\"a\":"));
        reader.Rewind(mark);

        this.AreEqual('"', reader.Peek());
        this.AreEqual("{", reader.Consumed.ToString());
        this.AreEqual(6,   reader.RemainingCount);
    }

    [Test] public void SliceBetweenCheckpoints_ReturnsTheToken()
    {
        ValueSpanReader<char> reader = new("[123,4]");
        reader.Advance(1);

        int start = reader.Position;
        reader.TryReadToAny(__delimiters, out _);
        this.AreEqual("123", reader.Slice(start, reader.Position).ToString());
    }

    [Test] public void Advance_And_Rewind_RejectInvalidArguments()
    {
        Throws<ArgumentOutOfRangeException>(static () => new ValueSpanReader<char>("ab").Advance(3));
        Throws<ArgumentOutOfRangeException>(static () => new ValueSpanReader<char>("ab").Advance(-1));
        Throws<ArgumentOutOfRangeException>(static () => new ValueSpanReader<char>("ab").Rewind(1)); // ahead of Position
        Throws<ArgumentOutOfRangeException>(static () => new ValueSpanReader<char>("ab").Rewind(-1));
    }

    [Test] public void Reset_ReturnsToStart()
    {
        ValueSpanReader<char> reader = new("abc");
        reader.Advance(3);
        reader.Reset();
        this.AreEqual(0,   reader.Position);
        this.AreEqual('a', reader.Peek());
    }
}
