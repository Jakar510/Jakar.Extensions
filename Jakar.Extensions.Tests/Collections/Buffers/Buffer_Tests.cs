// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Buffers;
using System.Collections.Generic;
using System.Linq;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Buffer<>))]
public class Buffer_Tests : Assert
{
    [Test]
    public void Add_GrowsPastCapacity_KeepingContentAndLength()
    {
        Buffer<int> buffer = new(2);

        try
        {
            for ( int i = 0; i < 1000; i++ ) { buffer.Add(i); }

            this.AreEqual(1000, buffer.Length);
            this.AreEqual(Enumerable.Range(0, 1000).ToArray(), buffer.Values.ToArray());
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void AddSpan_AddCount_AddRange_Grow()
    {
        Buffer<int> buffer = new(1);

        try
        {
            buffer.Add([1, 2, 3]);
            buffer.Add(9, 3);
            buffer.AddRange(new List<int> { 4, 5 });
            buffer.AddRange(new HashSet<int> { 6 });
            buffer.AddRange(new[] { 7 });
            buffer.AddRange(Enumerable.Range(8, 2));

            this.AreEqual(new[] { 1, 2, 3, 9, 9, 9, 4, 5, 6, 7, 8, 9 }, buffer.Values.ToArray());
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void Insert_ShiftsUp_AtStartMiddleAndEnd()
    {
        Buffer<char> buffer = new(2);

        try
        {
            buffer.Add("ace".AsSpan());
            buffer.Insert(1, 'b');
            buffer.Insert(3, "d".AsSpan());
            buffer.Insert(0, '>', 2);
            buffer.Insert(buffer.Length, '!');

            this.AreEqual(">>abcde!", buffer.Values.ToString());
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void RemoveAt_IncludingTheLastElement()
    {
        Buffer<string> buffer = new(4);

        try
        {
            buffer.Add("a");
            buffer.Add("b");
            buffer.Add("c");

            this.IsTrue(buffer.RemoveAt(2));
            this.IsTrue(buffer.RemoveAt(0));
            this.IsFalse(buffer.RemoveAt(5));
            this.AreEqual(new[] { "b" }, buffer.Values.ToArray());
            this.IsTrue(buffer.Remove("b"));
            this.IsFalse(buffer.Remove("missing"));
            this.AreEqual(0, buffer.Length);
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void Trim_RemovesOnlyTheMatchingEnds()
    {
        Buffer<char> buffer = new("  ab  ".AsSpan());
        buffer.Length = 6;

        try
        {
            buffer.TrimEnd(' ');
            this.AreEqual("  ab", buffer.Values.ToString());
            buffer.TrimStart(' ');
            this.AreEqual("ab", buffer.Values.ToString());

            buffer.Clear();
            buffer.Add("-_x_-".AsSpan());
            buffer.Trim('-', '_');
            this.AreEqual("x", buffer.Values.ToString());

            buffer.Clear();
            buffer.Add("zzz".AsSpan());
            buffer.Trim('z');
            this.AreEqual(0, buffer.Length);
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void Searches_OnAnEmptyBuffer_ReturnNotFound()
    {
        Buffer<int> buffer = new(4);

        try
        {
            this.AreEqual(-1, buffer.IndexOf(1));
            this.AreEqual(-1, buffer.LastIndexOf(1));
            this.AreEqual(-1, buffer.FindIndex(static _ => true));
            this.AreEqual(-1, buffer.FindLastIndex(static _ => true));
            this.AreEqual(0,  buffer.Find(static _ => true));
            this.IsFalse(buffer.Contains(1));
            this.IsFalse(buffer.Remove(1));
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void Searches_FindTheRightIndices()
    {
        Buffer<char> chars = new("abcabc".AsSpan());
        Buffer<int>  ints  = new([5, 6, 7, 5]);
        Buffer<long> longs = new([1L, 2L, 3L]);
        Buffer<string> strings = new(["x", "y", "x"]);

        try
        {
            this.AreEqual(1,  chars.IndexOf('b'));
            this.AreEqual(4,  chars.IndexOf('b', 2));
            this.AreEqual(4,  chars.LastIndexOf('b'));
            this.AreEqual(-1, chars.IndexOf('z'));
            this.AreEqual(3,  ints.LastIndexOf(5));
            this.AreEqual(2,  ints.IndexOf(7, 1, 3));
            this.AreEqual(1,  longs.IndexOf(2L));
            this.AreEqual(2,  strings.LastIndexOf("x"));
            this.AreEqual(2,  ints.FindIndex(static x => x > 6));
            this.AreEqual(3,  ints.FindLastIndex(static x => x == 5));
            this.AreEqual(7,  ints.Find(static x => x > 6));
        }
        finally
        {
            chars.Dispose();
            ints.Dispose();
            longs.Dispose();
            strings.Dispose();
        }
    }

    [Test]
    public void FindAll_IncludesTheEndOfTheRange()
    {
        Buffer<int> buffer = new([1, 2, 3, 4]);

        try
        {
            using ArrayBuffer<int> all   = buffer.FindAll(static _ => true);
            using ArrayBuffer<int> range = buffer.FindAll(static _ => true, 1, 3);

            this.AreEqual(new[] { 1, 2, 3, 4 }, all.Values.ToArray());
            this.AreEqual(new[] { 2, 3, 4 },    range.Values.ToArray());
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void Dispose_IsIdempotent_AndDefaultIsSafe()
    {
        const int SIZE = 2500; // a bucket no other test uses

        Buffer<char> buffer = new(SIZE);
        buffer.Dispose();
        buffer.Dispose();

        Buffer<char> empty = default;
        empty.Dispose();
        this.AreEqual(0, empty.Capacity);
        this.IsTrue(empty.Values.IsEmpty);

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
    public void BufferWriter_ExposesFreeSpace()
    {
        Buffer<byte> buffer = new(4);

        try
        {
            IBufferWriter<byte> writer = buffer;
            Memory<byte>        memory = buffer.GetMemory(16);

            this.IsTrue(memory.Length >= 16);
            memory.Span[0] = 42;
            buffer.Advance(1);

            Span<byte> span = buffer.GetSpan(4);
            span[0] = 43;
            buffer.Advance(1);

            this.AreEqual(new byte[] { 42, 43 }, buffer.Values.ToArray());
            Assert.IsNotNull(writer);
        }
        finally { buffer.Dispose(); }
    }

    [Test]
    public void AddingToAPreSizedBuffer_AllocatesNothing()
    {
        fill();

        long before = GC.GetAllocatedBytesForCurrentThread();
        fill();
        long after = GC.GetAllocatedBytesForCurrentThread();

        this.AreEqual(0L, after - before);
        return;

        static void fill()
        {
            Buffer<int> buffer = new(16);

            try
            {
                for ( int i = 0; i < 1000; i++ ) { buffer.Add(i); }

                _ = buffer.IndexOf(999);
                buffer.Insert(0, -1);
                buffer.RemoveAt(0);
                buffer.TrimEnd(999);
            }
            finally { buffer.Dispose(); }
        }
    }
}
