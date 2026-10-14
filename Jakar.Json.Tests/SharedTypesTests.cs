// Jakar.Json.Tests
// 10/07/2026

using System.Text.Json;



namespace Jakar.Json.Tests;


/// <summary> Member types that models shared with System.Text.Json need: init-only members of generic bases, <see cref="IJsonCollection{TSelf,T}"/>, and <see cref="JsonElement"/> extension data. </summary>
[TestFixture]
public sealed class SharedTypesTests
{
    [Test] public void InitOnlyMember_OfAClosedGenericBase()
    {
        Keyed keyed = new()
                      {
                          Id   = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e"),
                          Tags = ["a"],
                          Name = "n"
                      };

        string json = keyed.ToJson();
        Assert.That(json, Is.EqualTo("""{"Id" : "0f8fad5b-d9cb-469f-a165-70867728950e","Tags" : ["a"],"Name" : "n"}"""));

        Keyed back = Keyed.FromJson(json);
        Assert.That(back.Id,                                 Is.EqualTo(keyed.Id), "set through Base<TSelf, TId>'s open definition ([UnsafeAccessor] on a closed generic type fails at run time)");
        Assert.That(back.Tags,                               Is.EqualTo(new[] { "a" }));
        Assert.That(Keyed.FromJson("""{"Name":"n"}""").Tags, Is.EqualTo(new[] { "default" }), "an absent init-only member keeps its initializer");
    }

    [Test] public void JsonCollection_WritesWhatItChooses_AndIsBuiltAtOnce()
    {
        Holder holder = new() { ViewBag = ViewBag.FromJsonElements([3, 1, 2]) };
        holder.ViewBag.Hidden = true;

        Assert.That(holder.ViewBag.Visible, Is.Empty,                                               "the view hides everything");
        Assert.That(holder.ToJson(),        Is.EqualTo("""{"ViewBag" : [3,1,2],"Maybe" : null}"""), "the contents, not the view");

        Holder back = Holder.FromJson("""{"ViewBag":[5,6],"Maybe":[7]}""");
        Assert.That(back.ViewBag.Items,                              Is.EqualTo(new[] { 5, 6 }));
        Assert.That(back.ViewBag.Builds,                             Is.EqualTo(1), "one FromJsonElements call, not one Add per element");
        Assert.That(back.Maybe!.Items,                               Is.EqualTo(new[] { 7 }));
        Assert.That(Holder.TryFromJson("""{"ViewBag":{}}""", out _), Is.False);
    }

    [Test] public void ExtensionData_DictionaryOfJsonElement()
    {
        const string JSON = """{"Name" : "x","extra" : {"n" : [1,2.50,1e400,"s",null,true]},"flag" : false}""";

        Shared shared = Shared.FromJson(JSON);
        Assert.That(shared.Extra!.Keys,                                     Is.EqualTo(new[] { "extra", "flag" }));
        Assert.That(shared.Extra["extra"].GetProperty("n")[1].GetRawText(), Is.EqualTo("2.50"),  "numbers keep their text");
        Assert.That(shared.Extra["extra"].GetProperty("n")[2].GetRawText(), Is.EqualTo("1e400"), "even beyond double");
        Assert.That(shared.ToJson(),                                        Is.EqualTo(JSON),    "round trip");

        Shared built = new()
                       {
                           Name  = "y",
                           Extra = new Dictionary<string, JsonElement> { ["e"] = JsonElement.Parse("""{ "k" : [ 1 , "v" ] }""") }
                       };

        Assert.That(built.ToJson(), Is.EqualTo("""{"Name" : "y","e" : {"k" : [1,"v"]}}"""), "an element System.Text.Json parsed, written by Jakar.Json");
    }
}



public abstract class GenericBase<TSelf, TId>
    where TSelf : GenericBase<TSelf, TId>
    where TId : struct, IEquatable<TId>
{
    public TId          Id   { get; init; }
    public List<string> Tags { get; init; } = ["default"];
}



[JsonModel]
public sealed partial class Keyed : GenericBase<Keyed, Guid>
{
    public string Name { get; set; } = "";
}



/// <summary> A collection whose enumeration is a filtered view, like the observable collections. </summary>
public sealed class ViewBag : IJsonCollection<ViewBag, int>
{
    private readonly List<int> __items = [];

    public bool Hidden { get; set; }
    public int  Builds { get; private init; }

    public IReadOnlyList<int> Items   => __items;
    public IEnumerable<int>   Visible => __items.Where(_ => !Hidden);


    public void WriteJsonElements<TWriter, TConverter>( ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<int>
    {
        foreach ( int item in __items ) { TConverter.Write(ref writer, in item); }
    }

    public static ViewBag FromJsonElements( ReadOnlySpan<int> elements )
    {
        ViewBag bag = new() { Builds = 1 };
        bag.__items.AddRange(elements);
        return bag;
    }
}



[JsonModel]
public sealed partial class Holder
{
    public ViewBag  ViewBag { get; set; } = ViewBag.FromJsonElements([]);
    public ViewBag? Maybe   { get; set; }
}



[JsonModel]
public sealed partial class Shared
{
    public string Name { get; set; } = "";

    [JsonMember(ExtensionData = true)] public Dictionary<string, JsonElement>? Extra { get; set; }
}
