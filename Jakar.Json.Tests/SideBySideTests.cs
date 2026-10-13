// Jakar.Json.Tests
// 10/07/2026

namespace Jakar.Json.Tests;


/// <summary> <c>[GenerateJson]</c> next to <c>[JsonModel]</c> (JJSON010): the type's own helpers are System.Text.Json's, Jakar.Json is reached through <see cref="JsonCodec"/> and the interfaces. </summary>
[TestFixture]
public sealed class SideBySideTests
{
    private static Dual Sample => new()
                                  {
                                      Name  = "a \"b\"",
                                      Count = 3,
                                      Tags  = ["x", "y"]
                                  };


    [Test] public void TheTypesHelpers_AreJsonModels()
    {
        Dual dual = Sample;

        const string QUOTE = "\\" + "u0022"; // System.Text.Json escapes a quote as a \u-escape
        Assert.That(dual.ToJson(),                Is.EqualTo($$"""{"Name":"a {{QUOTE}}b{{QUOTE}}","Count":3,"Tags":["x","y"]}"""), "System.Text.Json's escaping and separators");
        Assert.That(Dual.FromJson(dual.ToJson()), Is.EqualTo(dual));
        Assert.That(Dual.TryFromJson("{", out _), Is.False);
        Assert.That(dual.ToString(),              Is.EqualTo(typeof(Dual).ToString()), "neither generator overrides ToString() unless [JsonModel(GenerateToString = true)] asks");
    }

    [Test] public void JakarJson_ThroughTheCodecAndInterfaces()
    {
        Dual   dual = Sample;
        string json = JsonCodec.ToJson(dual);

        Assert.That(json,                                                            Is.EqualTo("""{"Name" : "a \"b\"","Count" : 3,"Tags" : ["x","y"]}"""));
        Assert.That(JsonCodec.FromJson<Dual>(json),                                  Is.EqualTo(dual));
        Assert.That(JsonCodec.FromJson<Dual>(Encoding.UTF8.GetBytes(json).AsSpan()), Is.EqualTo(dual));
        Assert.That(Dual.Parse(json, CultureInfo.InvariantCulture),                  Is.EqualTo(dual));
        Assert.That(dual.ToString("i", null),                                        Is.EqualTo(JsonCodec.ToJson(dual, JsonWriterOptions.Indent)));
        Assert.That(Generic(dual),                                                   Is.EqualTo(dual));

        Span<char> buffer = stackalloc char[128];
        Assert.That(dual.TryFormat(buffer, out int written, default, null), Is.True);
        Assert.That(buffer[..written].ToString(),                           Is.EqualTo(json));

        static T Generic<T>( T value )
            where T : IJsonSerializable<T> => JsonCodec.FromJson<T>(JsonCodec.ToJson(value));
    }

    [Test] public void EachReadsTheOthersOutput()
    {
        Dual dual = Sample;

        Assert.That(Dual.FromJson(JsonCodec.ToJson(dual)),   Is.EqualTo(dual), "System.Text.Json reads Jakar.Json");
        Assert.That(JsonCodec.FromJson<Dual>(dual.ToJson()), Is.EqualTo(dual), "Jakar.Json reads System.Text.Json");
    }

    [Test] public async Task Streams_AreNotAmbiguous()
    {
        Dual         dual   = Sample;
        MemoryStream stream = new(Encoding.UTF8.GetBytes(dual.ToJson()));

        Assert.That(await Dual.FromJsonAsync(stream), Is.EqualTo(dual), "binds to [JsonModel]'s FromJsonAsync, the only one emitted");

        stream.Position = 0;
        Assert.That(await JsonCodec.FromJsonAsync<Dual>(stream), Is.EqualTo(dual));
    }
}



[GenerateJson]
[JsonModel(typeof(SideBySideContext))]
public sealed partial class Dual : IEquatable<Dual>
{
    public string       Name  { get; set; } = "";
    public int          Count { get; set; }
    public List<string> Tags  { get; set; } = [];


    public          bool Equals( Dual?   other ) => other is not null && Name == other.Name && Count == other.Count && Tags.SequenceEqual(other.Tags);
    public override bool Equals( object? obj )   => obj is Dual other && Equals(other);
    public override int  GetHashCode()           => HashCode.Combine(Name, Count, Tags.Count);
}



[System.Text.Json.Serialization.JsonSerializable(typeof(Dual))] public sealed partial class SideBySideContext : System.Text.Json.Serialization.JsonSerializerContext;
