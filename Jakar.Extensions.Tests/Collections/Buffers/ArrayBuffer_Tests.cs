// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.Linq;
using ZLinq;



namespace Jakar.Extensions.Tests.Buffers;


[TestFixture]
[TestOf(typeof(ArrayBuffer<>))]
public class ArrayBuffer_Tests : Assert
{
    /// <summary> An iterator, so ZLinq can't report its count (and neither can Order() over it). </summary>
    private static IEnumerable<TValue> Uncounted<TValue>( IEnumerable<TValue> source )
    {
        foreach ( TValue value in source ) { yield return value; }
    }


    [Test] public void ToArrayBuffer_OrderedStrings_HoldsOnlyTheItems()
    {
        string[] names = ["b", "a"];
        var      query = Uncounted(names).AsValueEnumerable().Order();
        this.IsFalse(query.TryGetNonEnumeratedCount(out _));

        using ArrayBuffer<string> buffer = query.ToArrayBuffer();

        this.AreEqual(2,                     buffer.Length);
        this.AreEqual(new[] { "a", "b" },    buffer.Values.ToArray());
        this.AreEqual(new[] { "a", "b" },    buffer.Array.ToArray());
        this.IsFalse(buffer.Values.ToArray().Contains(null));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(64)]
    [TestCase(65)]
    [TestCase(1000)]
    public void ToArrayBuffer_UnknownCount_Ordered( int count )
    {
        int[] source   = Enumerable.Range(0, count).Reverse().ToArray();
        var   query    = Uncounted(source).AsValueEnumerable().Order();
        int[] expected = Enumerable.Range(0, count).ToArray();
        this.IsFalse(query.TryGetNonEnumeratedCount(out _));

        using ArrayBuffer<int> buffer = query.ToArrayBuffer();

        this.AreEqual(count,    buffer.Length);
        this.AreEqual(expected, buffer.Values.ToArray());
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(64)]
    [TestCase(65)]
    [TestCase(1000)]
    public void ToArrayBuffer_UnknownCount_Where( int count )
    {
        int[] source = Enumerable.Range(0, count).ToArray();
        var   query  = source.AsValueEnumerable().Where(static _ => true);
        this.IsFalse(query.TryGetNonEnumeratedCount(out _));

        using ArrayBuffer<int> buffer = query.ToArrayBuffer();

        this.AreEqual(count,  buffer.Length);
        this.AreEqual(source, buffer.Values.ToArray());
    }

    [TestCase(0)]
    [TestCase(2)]
    [TestCase(1000)]
    public void ToArrayBuffer_KnownCount( int count )
    {
        int[] source = Enumerable.Range(0, count).ToArray();
        var   query  = source.AsValueEnumerable();
        this.IsTrue(query.TryGetNonEnumeratedCount(out _));

        using ArrayBuffer<int> buffer = query.ToArrayBuffer();

        this.AreEqual(count,  buffer.Length);
        this.AreEqual(source, buffer.Values.ToArray());
    }

    [Test] public void Create_FromIEnumerable_UnknownCount()
    {
        using ArrayBuffer<int> buffer = ArrayBuffer<int>.Create(Enumerable.Range(0, 100).Where(static x => x % 2 == 0));

        this.AreEqual(50,                                                     buffer.Length);
        this.AreEqual(Enumerable.Range(0, 50).Select(static x => x * 2).ToArray(), buffer.Values.ToArray());
    }
}
