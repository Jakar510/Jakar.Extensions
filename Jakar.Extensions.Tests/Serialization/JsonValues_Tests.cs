// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Collections.Generic;



namespace Jakar.Extensions.Tests;


/// <summary> Typed get / add / update / delete helpers for <see cref="JNode"/> trees, and the AdditionalData bag helpers. </summary>
[TestFixture]
public sealed class JsonValues_Tests
{
    private static JObjectNode Object() => "{\"Name\":\"widget\",\"Count\":3,\"Price\":\"1.5\",\"Missing\":null,\"Point\":{\"X\":1,\"Y\":2},\"List\":[{\"X\":10,\"Y\":0},{\"X\":20,\"Y\":0}]}".FromJson().AsObject();


    // ─── Reading ──────────────────────────────────────────────────────────────

    [Test] public void Node_Get_TryGet_GetOrDefault()
    {
        JObjectNode obj = Object();

        Assert.That(obj.TryGetValue("Name", out string? name),      Is.True);
        Assert.That(name,                                          Is.EqualTo("widget"));
        Assert.That(obj.TryGetValue("Count", out int count),        Is.True);
        Assert.That(count,                                         Is.EqualTo(3));
        Assert.That(obj.TryGetValue("Price", out decimal price),    Is.True, "numbers in strings parse");
        Assert.That(price,                                         Is.EqualTo(1.5m));
        Assert.That(obj.TryGetValue("Name", out int _),             Is.False, "wrong shape → false");
        Assert.That(obj.Get<Point>("Point"),                        Is.EqualTo(new Point(1, 2)));
        Assert.That(obj.Get<Point>("Missing"),                      Is.Null, "present JSON null → default");
        Assert.That(obj.TryGet("Missing", out Point? _),            Is.False);
        Assert.That(obj.TryGet("Nope",    out Point? _),            Is.False);
        Assert.That(obj.TryGet("Name",    out Point? _),            Is.False, "wrong shape → false");
        Assert.That(obj.GetOrDefault("Nope", new Point(4, 2)),      Is.EqualTo(new Point(4, 2)));
        Assert.That(obj["List"].TryGet(1, out Point? second),       Is.True);
        Assert.That(second,                                         Is.EqualTo(new Point(20, 0)));
        Assert.That(obj["List"].TryGet(5, out Point? _),            Is.False);
        Assert.That(() => obj.Get<Point>("Nope"),                   Throws.TypeOf<KeyNotFoundException>());
        Assert.That(() => obj.Get<Point>("Name"),                   Throws.TypeOf<JsonReadException>());
        Assert.That(obj["Point"].As<Point>(),                       Is.EqualTo(new Point(1, 2)));
    }


    // ─── Writing ──────────────────────────────────────────────────────────────

    [Test] public void Node_Add_Update_Set_Remove()
    {
        JObjectNode obj = Object();

        Assert.That(obj.TryAdd("Name", "other"),             Is.False);
        Assert.That(obj.TryAdd("New",  7),                   Is.True);
        Assert.That(obj.TryUpdate("Nope",  1),               Is.False);
        Assert.That(obj.TryUpdate("Count", 4),               Is.True);
        Assert.That(obj.TryAdd("Other", new Point(7, 8)),    Is.True);
        Assert.That(obj.TryUpdate("Other", new Point(8, 9)), Is.True);
        obj.Set("Point", new Point(5, 6));

        Assert.That((int)obj["New"],           Is.EqualTo(7));
        Assert.That((int)obj["Count"],         Is.EqualTo(4));
        Assert.That(obj.Get<Point>("Point"),   Is.EqualTo(new Point(5, 6)));
        Assert.That(obj.Get<Point>("Other"),   Is.EqualTo(new Point(8, 9)));

        Assert.That(obj.Remove("New", out JNode? removed), Is.True);
        Assert.That((int)removed,                          Is.EqualTo(7));
        Assert.That(obj.ContainsKey("New"),                Is.False);
    }


    [Test] public void Node_WritingAParentedNode_ClonesIt()
    {
        JObjectNode source = Object();
        JObjectNode target = [];
        JNode       point  = source["Point"]!;

        target["Copied"] = null;
        Assert.That(target.TryUpdate("Copied", point), Is.True);

        Assert.That(target["Copied"],                          Is.Not.SameAs(point));
        Assert.That(JNode.DeepEquals(target["Copied"], point), Is.True);
        Assert.That(point.Parent,                              Is.SameAs(source), "the original stays where it was");
    }


    [Test] public void Array_Add_Insert_Set()
    {
        JArrayNode array = [];

        array.Add(new Point(1, 1));
        array.Add(new Point(3, 3));
        array.Insert(1, new Point(2, 2));
        array.Set(0, new Point(0, 0));
        Assert.That(array.ToJson(false), Is.EqualTo("[{\"X\" : 0,\"Y\" : 0},{\"X\" : 2,\"Y\" : 2},{\"X\" : 3,\"Y\" : 3}]"));
    }


    // ─── AdditionalData bags ──────────────────────────────────────────────────

    [Test] public void Bag_TypedAccess()
    {
        Bag bag = new();
        bag.Set("Count", 3);
        bag.Set("Point", new Point(1, 2));

        Assert.That((int)bag.Get("Count"),          Is.EqualTo(3));
        Assert.That(bag.Get<Point>("Point"),        Is.EqualTo(new Point(1, 2)));
        Assert.That(bag.Contains("Count"),          Is.True);
        Assert.That(bag.Contains("count"),          Is.False, "ordinal keys");
        Assert.That(bag.Remove("Count"),            Is.True);
        Assert.That(bag.Get("Count"),               Is.Null);
        Assert.That(bag.TryGet("Point", out Point? point), Is.True);
        Assert.That(point,                          Is.EqualTo(new Point(1, 2)));
    }


    [Test] public void StringBag_RoundTripsThroughText()
    {
        StringBag bag = new();
        bag.Set("Point", new Point(1, 2));
        bag.Set("Other", new Point(3, 4));

        Assert.That(bag.AdditionalData,       Does.Contain("\"Point\" : {\"X\" : 1,\"Y\" : 2}"));
        Assert.That(bag.Get<Point>("Point"),  Is.EqualTo(new Point(1, 2)));
        Assert.That(bag.Remove("Other"),      Is.True);
        Assert.That(bag.Contains("Other"),    Is.False);
        Assert.That(bag.Contains("Point"),    Is.True);
    }



    private sealed class Bag : IJsonModel
    {
        public JObjectNode? AdditionalData { get; set; }
    }



    private sealed class StringBag : IJsonStringModel
    {
        public string? AdditionalData { get; set; }
    }
}
