// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Linq;



namespace Jakar.Extensions.Tests.Serialization;


/// <summary> Regression tests for bugs found while planning the System.Text.Json migration (AOT-plan.md §2.14). </summary>
[TestFixture]
public sealed class JsonSpan_Tests : Assert
{
    private static string Compact( string json ) => string.Concat(json.Where(static c => !char.IsWhiteSpace(c)));


    [Test]
    public void ToJson_Span_WritesExactlyTheSpan()
    {
        // ArrayPool.Shared.Rent(3) returns a 16-element array; the old implementation serialized all 16.
        ReadOnlySpan<int> values = [1, 2, 3];
        Assert.That(Compact(Json.Serialize(values)), Is.EqualTo("[1,2,3]"));
    }


    [Test]
    public void ToJson_EmptySpan_WritesEmptyArray()
    {
        ReadOnlySpan<string> values = [];
        Assert.That(Compact(Json.Serialize(values)), Is.EqualTo("[]"));
    }


    [Test]
    public void ObservableCollection_ImplicitFromArray_KeepsTheValues()
    {
        ObservableCollection<int> collection = new[] { 1, 2, 3 };
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
    }


    [Test]
    public void ConcurrentObservableCollection_ArrayConstructorAndImplicit_KeepTheValues()
    {
        int[]                               values     = [1, 2, 3];
        ConcurrentObservableCollection<int> fromCtor   = new(values);
        ConcurrentObservableCollection<int> fromCast   = values;
        Assert.That(fromCtor.ToArray(), Is.EqualTo(values));
        Assert.That(fromCast.ToArray(), Is.EqualTo(values));
    }
}
