// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


/// <summary> The SPEC.md §6 allocation budget, measured with <see cref="GC.GetAllocatedBytesForCurrentThread"/> after a warm-up. </summary>
[TestFixture]
[NonParallelizable]
public sealed class AllocationTests
{
    private static readonly Invoice __invoice = ModelTests.SampleInvoice();
    private static readonly string  __json    = __invoice.ToJson();
    private static readonly byte[]  __utf8    = __invoice.ToJsonUtf8();


    [Test] public void TryFormat_AllocatesNothing()
    {
        char[] chars = new char[4096];
        byte[] bytes = new byte[4096];

        long allocated = Measure(() =>
                                 {
                                     __invoice.TryFormat(chars, out _, default, null);
                                     __invoice.TryFormat(bytes, out _, "i",     null);
                                 });

        Assert.That(allocated, Is.Zero);
    }

    [Test] public void BufferWriter_AllocatesNothing()
    {
        ArrayBufferWriter<byte> output = new(64 * 1024);

        long allocated = Measure(() =>
                                 {
                                     output.ResetWrittenCount();
                                     __invoice.WriteJson(output);
                                 });

        Assert.That(allocated, Is.Zero);
    }

    [Test] public void ToJson_AllocatesOnlyTheString()
    {
        long expected  = Measure(() => _ = new string('x', __json.Length));
        long allocated = Measure(() => _ = __invoice.ToJson());

        Assert.That(allocated, Is.EqualTo(expected));
    }

    [Test] public void ToJsonUtf8_AllocatesOnlyTheArray()
    {
        long expected  = Measure(() => _ = new byte[__utf8.Length]);
        long allocated = Measure(() => _ = __invoice.ToJsonUtf8());

        Assert.That(allocated, Is.EqualTo(expected));
    }

    [Test] public void FromJson_AllocatesOnlyTheResultGraph()
    {
        // The same graph, built by hand: the invoice, its memo, an exactly sized list, two items and their SKUs.
        long expected = Measure(() => _ = new Invoice(Guid.Empty, 1.10m)
                                          {
                                              Notes = new string('p', "paid in full".Length),
                                              Lines = new List<LineItem>(2)
                                                      {
                                                          new(new string('a', 3), 3, 0.25m),
                                                          new(new string('b', 3), 1, 10m)
                                                      }
                                          });

        Assert.That(Measure(() => _ = Invoice.FromJson(__json)),          Is.EqualTo(expected), "UTF-16");
        Assert.That(Measure(() => _ = Invoice.FromJson(__utf8.AsSpan())), Is.EqualTo(expected), "UTF-8");
    }

    [Test] public void TapeNavigation_AllocatesOnlyTheTapeObject()
    {
        using ( JsonTape warm = JsonTape.Parse(__utf8) ) { _ = warm.Root["lines"][1]["price"].GetDecimal(); }

        long tapeObject = Measure(() => JsonTape.Parse(__json).Dispose());

        using JsonTape tape = JsonTape.Parse(__utf8);

        long navigation = Measure(() =>
                                  {
                                      JsonItem root = tape.Root;
                                      _ = root["lines"][1]["price"].GetDecimal();
                                      _ = root["status"].ValueEquals("Paid");
                                      foreach ( JsonItem line in root["lines"].EnumerateArray() ) { _ = line["quantity"].GetInt32(); }
                                  });

        Assert.That(navigation, Is.Zero);
        Assert.That(tapeObject, Is.LessThanOrEqualTo(128), "one small object; the index and input copy are pooled");
    }


    private static long Measure( Action action )
    {
        for ( int i = 0; i < 5; i++ ) { action(); } // warm-up: JIT, static initializers, pool buckets

        long before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }
}
