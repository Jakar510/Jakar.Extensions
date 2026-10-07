// Jakar.SystemTextJson.Tests
// 10/02/2026

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Jakar.Extensions;



namespace Jakar.SystemTextJson.Tests;


[TestFixture]
public sealed class JsonArray_Tests
{
    private static string Compact( string json ) => string.Concat(json.Where(static c => !char.IsWhiteSpace(c)));

    private static string Numbers( int count ) => $"[{string.Join(',', Enumerable.Range(0, count))}]";


    // ─── RentedArray ──────────────────────────────────────────────────────────

    [Test] public void RentedArray_ExposesExactlyCount()
    {
        using RentedArray<int> values = JsonModel.FromJsonArrayPooled("[1,2,3]", TestJsonContext.Default.Int32);
        Assert.That(values.Count,         Is.EqualTo(3));
        Assert.That(values.Span.Length,   Is.EqualTo(3));
        Assert.That(values.Memory.Length, Is.EqualTo(3));
        Assert.That(values.ToArray(),     Is.EqualTo([1, 2, 3]));
    }


    [Test] public void RentedArray_DisposeIsIdempotent_AndAccessAfterDisposeThrows()
    {
        RentedArray<int> values = JsonModel.FromJsonArrayPooled("[1,2,3]", TestJsonContext.Default.Int32);
        values.Dispose();
        values.Dispose();
        Assert.That(values.IsDisposed,       Is.True);
        Assert.That(() => _ = values.Span,   Throws.TypeOf<ObjectDisposedException>());
        Assert.That(() => _ = values.Memory, Throws.TypeOf<ObjectDisposedException>());
    }


    [Test] public void RentedArray_Empty_DisposesWithoutTouchingThePool()
    {
        RentedArray<int> values = JsonModel.FromJsonArrayPooled("[]", TestJsonContext.Default.Int32);
        Assert.That(values.IsEmpty, Is.True);
        values.Dispose();
        Assert.That(values.IsDisposed, Is.True);
    }


    [Test] public void RentedArray_DoubleDispose_NeverHandsOutTheSameArrayTwice()
    {
        // If Dispose returned the array twice, two later renters would share it.
        RentedArray<string> values = JsonModel.FromJsonArrayPooled("""["a","b"]""", TestJsonContext.Default.String);
        values.Dispose();
        values.Dispose();

        string[] first  = System.Buffers.ArrayPool<string>.Shared.Rent(16);
        string[] second = System.Buffers.ArrayPool<string>.Shared.Rent(16);

        try { Assert.That(second, Is.Not.SameAs(first)); }
        finally
        {
            System.Buffers.ArrayPool<string>.Shared.Return(first);
            System.Buffers.ArrayPool<string>.Shared.Return(second);
        }
    }


    // ─── Reader ───────────────────────────────────────────────────────────────

    [TestCase(0)] [TestCase(1)] [TestCase(16)] [TestCase(17)] [TestCase(33)] [TestCase(1024)] public void Reader_GrowsAcrossRentals( int count )
    {
        using RentedArray<int> values = JsonModel.FromJsonArrayPooled(Numbers(count), TestJsonContext.Default.Int32);
        Assert.That(values.ToArray(), Is.EqualTo([.. Enumerable.Range(0, count)]));
    }


    [Test] public void Reader_NullElements_AreDefault()
    {
        using RentedArray<Invoice> invoices = JsonModel.FromJsonArrayPooled("""[{"Id":"a"},null]""", Invoice.JsonTypeInfo);
        Assert.That(invoices.Count,   Is.EqualTo(2));
        Assert.That(invoices.Span[1], Is.Null);

        using RentedArray<int?> numbers = JsonModel.FromJsonArrayPooled("[1,null]", TestJsonContext.Default.NullableInt32);
        Assert.That(numbers.ToArray(), Is.EqualTo(new int?[] { 1, null }));
    }


    [TestCase("null")] [TestCase("{}")] [TestCase("\"text\"")] [TestCase("[1,2] 3")] [TestCase("[1,2")] [TestCase("")] [TestCase("[1,\"x\"]")]
    public void Reader_InvalidInput_Throws( string json ) => Assert.That(() => JsonModel.FromJsonArrayPooled(json, TestJsonContext.Default.Int32).Dispose(), Throws.InstanceOf<JsonException>());


    [Test] public void Reader_SkipsUtf8Bom()
    {
        byte[] utf8 = [0xEF, 0xBB, 0xBF, .. "[1,2]"u8];
        Assert.That(JsonModel.FromJsonArray(utf8, TestJsonContext.Default.Int32), Is.EqualTo([1, 2]));
    }


    [Test] public void Reader_HonoursTheContextsTolerance()
    {
        // TestJsonContext allows trailing commas and skips comments.
        Assert.That(JsonModel.FromJsonArray("[1, /* two */ 2,]", TestJsonContext.Default.Int32), Is.EqualTo([1, 2]));

        // The default options allow neither.
        Assert.That(() => JsonModel.FromJsonArray("[1,2,]", JsonSerializerOptions.Default.GetRequiredTypeInfo<int>()), Throws.InstanceOf<JsonException>());
    }


    [Test] public void Reader_BuildsAnyCollection()
    {
        const string json = """[{"Id":"a","Total":1},{"Id":"b","Total":2}]""";

        ImmutableArray<Invoice> immutable = JsonModel.FromJsonArray(json, Invoice.JsonTypeInfo, static ImmutableArray<Invoice> ( ReadOnlySpan<Invoice> span ) => [.. span]);
        List<Invoice>           list      = JsonModel.FromJsonList(json, Invoice.JsonTypeInfo);
        Invoice[]               array     = JsonModel.FromJsonArray(Encoding.UTF8.GetBytes(json), Invoice.JsonTypeInfo);

        Assert.That(immutable.Select(static i => i.Id), Is.EqualTo(["a", "b"]));
        Assert.That(list.Select(static i => i.Id),      Is.EqualTo(["a", "b"]));
        Assert.That(array.Select(static i => i.Id),     Is.EqualTo(["a", "b"]));
    }


    [Test] public void Reader_BuildCallbackThrows_StillDisposes() => Assert.That(() => JsonModel.FromJsonArray<int, int>("[1]", TestJsonContext.Default.Int32, static _ => throw new InvalidOperationException()), Throws.InvalidOperationException);


    [Test] public async Task Reader_Async_SmallReads()
    {
        await using Stream stream = new TrickleStream(Encoding.UTF8.GetBytes(Numbers(200)));
        int[]              values = await JsonModel.FromJsonArrayAsync(stream, TestJsonContext.Default.Int32);
        Assert.That(values, Is.EqualTo([.. Enumerable.Range(0, 200)]));
    }


    [Test] public void Reader_Async_Cancellation()
    {
        using CancellationTokenSource source = new();
        source.Cancel();
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(Numbers(10)));
        Assert.That(async () => await JsonModel.FromJsonArrayAsync(stream, TestJsonContext.Default.Int32, source.Token), Throws.InstanceOf<OperationCanceledException>());
    }


    [Test] public void ReadArrayElements_FromInsideAReader()
    {
        Utf8JsonReader reader = new("""{"Items":[1,2,3],"After":true}"""u8);
        reader.Read(); // {
        reader.Read(); // "Items"
        reader.Read(); // [

        using RentedArray<int> values = JsonModel.ReadArrayElements(ref reader, TestJsonContext.Default.Int32);
        Assert.That(values.ToArray(),   Is.EqualTo([1, 2, 3]));
        Assert.That(reader.TokenType,   Is.EqualTo(JsonTokenType.EndArray));
        Assert.That(reader.Read(),      Is.True);
        Assert.That(reader.GetString(), Is.EqualTo("After"));
    }


    // ─── Writer ───────────────────────────────────────────────────────────────

    [Test] public void Writer_AllShapesWriteTheSameJson()
    {
        Invoice[] invoices =
        [
            new()
            {
                Id    = "a",
                Total = 1
            },
            new()
            {
                Id    = "b",
                Total = 2
            }
        ];

        string expected = """[{"Id":"a","Total":1},{"Id":"b","Total":2}]""";

        Assert.That(Compact(invoices.ToJson()),                                Is.EqualTo(expected));
        Assert.That(Compact(new ReadOnlySpan<Invoice>(invoices).ToJson()),     Is.EqualTo(expected));
        Assert.That(Compact(invoices.ToList().ToJson()),                       Is.EqualTo(expected));
        Assert.That(Compact(invoices.ToImmutableArray().ToJson()),             Is.EqualTo(expected));
        Assert.That(Compact(invoices.Where(static _ => true).ToJson()),        Is.EqualTo(expected));
        Assert.That(Compact(( (IEnumerable<Invoice>)[.. invoices] ).ToJson()), Is.EqualTo(expected));
    }


    [Test] public void Writer_ListWithSpareCapacity_WritesOnlyCount()
    {
        List<int> list = new(32)
                         {
                             1,
                             2,
                             3
                         };

        Assert.That(Compact(JsonModel.ToJson(list, TestJsonContext.Default.Int32)), Is.EqualTo("[1,2,3]"));
    }


    [Test] public void Writer_DefaultImmutableArray_WritesEmptyArray() => Assert.That(Compact(default(ImmutableArray<Invoice>).ToJson()), Is.EqualTo("[]"));


    [Test] public void Writer_ArraySource_WinsOverEnumeration()
    {
        FilteredList list = new([1, 2, 3, 4], static i => i % 2 == 0);
        Assert.That(list.ToArray(),                                                 Is.EqualTo([2, 4]));
        Assert.That(Compact(JsonModel.ToJson(list, TestJsonContext.Default.Int32)), Is.EqualTo("[1,2,3,4]"));
    }


    [Test] public void Writer_IndentedOverride()
    {
        Assert.That(JsonModel.ToJson(new[] { 1, 2 }.AsSpan(), TestJsonContext.Default.Int32),       Does.Not.Contain("\n"));
        Assert.That(JsonModel.ToJson(new[] { 1, 2 }.AsSpan(), TestJsonContext.Default.Int32, true), Does.Contain("\n"));
    }



    /// <summary> Returns at most 7 bytes per read, so the async reader has to resume mid-token. </summary>
    private sealed class TrickleStream( byte[] data ) : MemoryStream(data)
    {
        public override int Read( byte[] buffer, int offset, int count ) => base.Read(buffer, offset, Math.Min(count, 7));

        public override ValueTask<int> ReadAsync( Memory<byte> buffer, CancellationToken cancellationToken = default ) => base.ReadAsync(buffer[..Math.Min(buffer.Length, 7)], cancellationToken);

        public override Task<int> ReadAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken ) => base.ReadAsync(buffer, offset, Math.Min(count, 7), cancellationToken);
    }
}
