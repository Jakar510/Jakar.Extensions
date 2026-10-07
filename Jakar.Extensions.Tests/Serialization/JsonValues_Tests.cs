// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;



namespace Jakar.Extensions.Tests.Serialization;


/// <summary> Typed get / add / update / delete helpers for JsonNode and JsonElement (AOT-plan §2.11), and the AdditionalData bag helpers. </summary>
[TestFixture]
public sealed class JsonValues_Tests
{
    private static JsonObject Object() => "{\"Name\":\"widget\",\"Count\":3,\"Price\":\"1.5\",\"Missing\":null,\"Point\":{\"X\":1,\"Y\":2},\"List\":[10,20]}".FromJson().AsObject();


    // ─── JsonNode ─────────────────────────────────────────────────────────────

    [Test] public void Node_Get_TryGet_GetOrDefault()
    {
        JsonObject obj = Object();

        Assert.That(obj.Get<string>("Name"),                    Is.EqualTo("widget"));
        Assert.That(obj.Get<int>("Count"),                      Is.EqualTo(3));
        Assert.That(obj.Get<decimal>("Price"),                  Is.EqualTo(1.5m), "numbers in strings read (decision 2)");
        Assert.That(obj.Get<Point>("Point"),                    Is.EqualTo(new Point(1, 2)));
        Assert.That(obj.TryGet("Missing", out string? missing), Is.True, "present JSON null → true + default");
        Assert.That(missing,                                    Is.Null);
        Assert.That(obj.TryGet("Nope", out int _),              Is.False);
        Assert.That(obj.TryGet("Name", out int _),              Is.False, "wrong shape → false");
        Assert.That(obj.GetOrDefault("Nope", 42),               Is.EqualTo(42));
        Assert.That(obj.GetOrDefault("Name", 42),               Is.EqualTo(42));
        Assert.That(obj["List"].TryGet(1, out int second),      Is.True);
        Assert.That(second,                                     Is.EqualTo(20));
        Assert.That(obj["List"].TryGet(5, out int _),           Is.False);
        Assert.That(() => obj.Get<int>("Nope"),                 Throws.TypeOf<KeyNotFoundException>());
        Assert.That(() => obj.Get<int>("Name"),                 Throws.InstanceOf<JsonException>());
    }


    [Test] public void Node_Add_Update_Set_Remove()
    {
        JsonObject obj = Object();

        Assert.That(obj.TryAdd("Name", "other"), Is.False);
        Assert.That(obj.TryAdd("New",  7),       Is.True);
        Assert.That(obj.TryUpdate("Nope",  1),   Is.False);
        Assert.That(obj.TryUpdate("Count", 4),   Is.True);
        obj.Set("Point", new Point(5, 6));

        Assert.That(obj.Get<int>("New"),     Is.EqualTo(7));
        Assert.That(obj.Get<int>("Count"),   Is.EqualTo(4));
        Assert.That(obj.Get<Point>("Point"), Is.EqualTo(new Point(5, 6)));

        Assert.That(obj.Remove("New", out JsonNode? removed), Is.True);
        Assert.That(removed!.GetValue<int>(),                 Is.EqualTo(7));
        Assert.That(obj.ContainsKey("New"),                   Is.False);
    }


    [Test] public void Node_WritingAParentedNode_ClonesIt()
    {
        JsonObject source = Object();
        JsonObject target = new();
        JsonNode   point  = source["Point"]!;

        target.Set("Copied", point, JakarExtensionsContext.Default.JsonNode);

        Assert.That(target["Copied"],                             Is.Not.SameAs(point));
        Assert.That(JsonNode.DeepEquals(target["Copied"], point), Is.True);
        Assert.That(point.Parent,                                 Is.SameAs(source), "the original stays where it was");
    }


    [Test] public void Array_Add_Insert_Set()
    {
        JsonArray array = [1, 3];

        array.Insert(1, 2);
        array.Set(0, 0);
        Assert.That(array.ToJson(false), Is.EqualTo("[0,2,3]"));
    }


    // ─── JsonElement ──────────────────────────────────────────────────────────

    [Test] public void Element_Reads()
    {
        JsonElement element = Json.SerializeToElement(Object());

        Assert.That(element.Get<string>("Name"),                                                Is.EqualTo("widget"));
        Assert.That(element.Contains("name"),                                                   Is.False, "ordinal by default");
        Assert.That(element.Contains("name", StringComparison.OrdinalIgnoreCase),               Is.True);
        Assert.That(element.TryGet("count", out int count, StringComparison.OrdinalIgnoreCase), Is.True);
        Assert.That(count,                                                                      Is.EqualTo(3));
        Assert.That(element.GetOrDefault("Nope", "fallback"),                                   Is.EqualTo("fallback"));
        Assert.That(element.Get<Point>("Point"),                                                Is.EqualTo(new Point(1, 2)));
    }


    [Test] public void Element_DuplicateKeys_LastWins()
    {
        JsonElement element = "{\"A\":1,\"A\":2}".FromJson<JsonElement>();
        Assert.That(element.Get<int>("A"), Is.EqualTo(2));
    }


    [Test] public void Element_Writes_ReplaceTheElement()
    {
        JsonElement element = Json.SerializeToElement(new Point(1, 2));

        Assert.That(element.TryAdd("X", 9),     Is.False);
        Assert.That(element.TryAdd("Z", 3),     Is.True);
        Assert.That(element.TryUpdate("X", 10), Is.True);
        Assert.That(element.Remove("Y"),        Is.True);
        Assert.That(element.Remove("Y"),        Is.False);

        Assert.That(JsonNode.DeepEquals(JsonNode.Parse(element.ToJson(false)), JsonNode.Parse("{\"X\":10,\"Z\":3}")), Is.True, element.ToJson(false));

        JsonElement array = "[1,2,3]".FromJson<JsonElement>();
        array.Add(4);
        Assert.That(array.RemoveAt(0),   Is.True);
        Assert.That(array.ToJson(false), Is.EqualTo("[2,3,4]"));
    }


    [Test] public void Element_ToJsonObject_AndBack()
    {
        JsonObject obj = Json.SerializeToElement(new Point(1, 2)).ToJsonObject()!;
        obj.Set("X", 5);
        Assert.That(obj.ToJsonElement().As<Point>(), Is.EqualTo(new Point(5, 2)));
    }


    [Test] public void Unregistered_Type_ThrowsActionableError()
    {
        NotSupportedException? e = Assert.Throws<NotSupportedException>(static () => Json.GetTypeInfo<Unregistered>());
        Assert.That(e!.Message, Does.Contain("Unregistered").And.Contain("[JsonSerializable"));
    }


    // ─── AdditionalData bags ──────────────────────────────────────────────────

    [Test] public void Bag_TypedAccess_IsCaseInsensitive()
    {
        Bag bag = new();
        bag.Set("Count", 3);
        bag.Set("Point", new Point(1, 2));

        Assert.That(bag.Get<int>("count"),          Is.EqualTo(3));
        Assert.That(bag.Get<Point>("POINT"),        Is.EqualTo(new Point(1, 2)));
        Assert.That(bag.Contains("count"),          Is.True);
        Assert.That(bag.Remove("COUNT"),            Is.True);
        Assert.That(bag.TryGet("Count", out int _), Is.False);
    }


    [Test] public void StringBag_RoundTripsThroughText()
    {
        StringBag bag = new();
        bag.Set("Count", 3);
        bag.Set("Name",  "x");

        Assert.That(bag.AdditionalData,    Does.Contain("\"Count\":3"));
        Assert.That(bag.Get<int>("count"), Is.EqualTo(3));
        Assert.That(bag.Remove("Name"),    Is.True);
        Assert.That(bag.Contains("Name"),  Is.False);
    }



    private sealed class Unregistered;



    private sealed class Bag : IJsonModel
    {
        [System.Text.Json.Serialization.JsonExtensionData] public Dictionary<string, JsonElement>? AdditionalData { get; set; }
    }



    private sealed class StringBag : IJsonStringModel
    {
        public string? AdditionalData { get; set; }
    }
}
