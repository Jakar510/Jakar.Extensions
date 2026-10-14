// Jakar.Json.Tests
// 10/13/2026

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jakar.Json.Tests;


/// <summary>
///     <c>[JsonModel]</c> picks the implementation per type (SPEC.md §7.7): with a context, Jakar.Json behind <see cref="IJsonModel{TSelf}"/> when it can (hybrid), System.Text.Json when it can't
///     (JJSON016) or when asked (<c>Implementation = SystemTextJson</c>); without one, Jakar.Json only. The context's options are mirrored into Jakar.Json's settings.
/// </summary>
[TestFixture]
public sealed class JsonModelModesTests
{
    private static Hybrid Sample => new()
                                    {
                                        DisplayName = "a \"b\"",
                                        Count       = 3,
                                        Tags        = ["x", "y"],
                                        Status      = ModeStatus.Paid
                                    };


    // ─── Hybrid: Jakar.Json behind IJsonModel ────────────────────────────────

    [Test] public void Hybrid_ImplementsIJsonModelWithJakarJson()
    {
        Hybrid hybrid = Sample;
        string json   = hybrid.ToJson();

        Assert.That(json,                                 Is.EqualTo("""{"displayName" : "a \"b\"","count" : 3,"tags" : ["x","y"],"status" : "Paid"}"""), "Jakar.Json's layout, with the context's camelCase and string enums");
        Assert.That(json,                                 Is.EqualTo(JsonCodec.ToJson(hybrid)));
        Assert.That(hybrid.ToUtf8Json(),                  Is.EqualTo(JsonCodec.ToJsonUtf8(hybrid)));
        Assert.That(Hybrid.FromJson(json),                Is.EqualTo(hybrid));
        Assert.That(Hybrid.FromJson(hybrid.ToUtf8Json()), Is.EqualTo(hybrid));
        Assert.That(Hybrid.TryFromJson(json, out Hybrid? back) && back == hybrid, Is.True);
        Assert.That(Generic(hybrid),                      Is.EqualTo(hybrid), "generic IJsonModel code reaches the same implementation");

        static T Generic<T>( T value )
            where T : IJsonModel<T> => T.FromJson(value.ToJson());
    }

    [Test] public void Hybrid_JsonTypeInfoStaysSystemTextJson_AndBothReadEachOther()
    {
        Hybrid hybrid = Sample;
        string stj    = JsonSerializer.Serialize(hybrid, Hybrid.JsonTypeInfo);

        Assert.That(stj,                                                     Does.StartWith("{\"displayName\":"), "System.Text.Json's own layout");
        Assert.That(Hybrid.FromJson(stj),                                    Is.EqualTo(hybrid), "Jakar.Json reads System.Text.Json's output");
        Assert.That(JsonSerializer.Deserialize(hybrid.ToJson(), Hybrid.JsonTypeInfo), Is.EqualTo(hybrid), "and the other way round");
        Assert.That(Hybrid.FromJson("""{ "DISPLAYNAME": "c", "count": "4", "status": 1, }"""), Is.EqualTo(new Hybrid { DisplayName = "c", Count = 4, Status = ModeStatus.Sent }),
                    "mirrored: case-insensitive names, numbers from strings, trailing commas; enum numbers still read");
    }

    [Test] public void Hybrid_KeepsTheIJsonModelContract()
    {
        // Jakar.Json's read error is a System.Text.Json JsonException: IJsonModel's contract, whichever serializer ran.
        JsonException error = Assert.Throws(Is.InstanceOf<JsonException>(), static () => Hybrid.FromJson("""{"count": x}"""))! as JsonException ?? throw new AssertionException("not a JsonException");
        Assert.That(error,             Is.TypeOf<JsonReadException>());
        Assert.That(error.Path,        Is.EqualTo("$.count"));
        Assert.That(error.LineNumber,  Is.Zero, "zero-based, as System.Text.Json reports it");
        Assert.That(( (JsonReadException)error ).Error.Line, Is.EqualTo(1), "Error keeps Jakar.Json's one-based position");

        Assert.Throws(Is.InstanceOf<JsonException>(), static () => Hybrid.FromJson("null"u8));
        Assert.That(Hybrid.TryFromJson("{",          out _), Is.False);
        Assert.That(Hybrid.TryFromJson((string?)null, out _), Is.False);
    }

    [Test] public void Hybrid_IndentedFollowsTheContext_AndCanBeOverridden()
    {
        Hybrid hybrid = Sample;

        Assert.That(hybrid.ToJson(indented: true), Does.StartWith("{\n  \"displayName\" : "), "WriteIndented's layout: two spaces");
        Assert.That(hybrid.ToJson(indented: false), Is.EqualTo(hybrid.ToJson()));
        Assert.That(hybrid.ToString(),              Is.EqualTo(typeof(Hybrid).ToString()), "System.Text.Json never overrides ToString(), so neither does a mirrored model");
    }

    [Test] public async Task Hybrid_Streams()
    {
        Hybrid       hybrid = Sample;
        MemoryStream stream = new();

        await hybrid.WriteToAsync(stream);
        Assert.That(stream.ToArray(), Is.EqualTo(hybrid.ToUtf8Json()));

        stream.Position = 0;
        Assert.That(await Hybrid.FromJsonAsync(stream), Is.EqualTo(hybrid));

        await Assert.ThatAsync(async () => await Hybrid.FromJsonAsync(new MemoryStream("{"u8.ToArray())), Throws.InstanceOf<JsonException>());
    }

    [Test] public void Hybrid_ConstructorParametersAreOptional_LikeSystemTextJson()
    {
        Assert.That(Position.FromJson("""{"x":1}"""),           Is.EqualTo(new Position(1, 0)), "a missing parameter is its default, as in System.Text.Json");
        Assert.That(JsonCodec.TryFromJson<LooseCoordinate>("""{"X":1}""", out _), Is.False, "without a context, Jakar.Json requires it");
    }


    // ─── System.Text.Json: asked for, or fallen back to ──────────────────────

    [Test] public void ImplementationSystemTextJson_IsSystemTextJson()
    {
        StjOnly value = new() { Name = "n" };

        Assert.That(value.ToJson(),                                                 Is.EqualTo(JsonSerializer.Serialize(value, StjOnly.JsonTypeInfo)));
        Assert.That(IsJakarJson(typeof(StjOnly)), Is.False, "no Jakar.Json code");
    }

    [Test] public void Fallback_UsesSystemTextJson_WhenJakarJsonCantMatchIt()
    {
        Fallback value = new() { Name = "n" };

        Assert.That(value.ToJson(),                                                   Is.EqualTo("""{"name":"N!"}"""), "the member's System.Text.Json converter ran");
        Assert.That(Fallback.FromJson("""{"name":"N!"}""").Name,                      Is.EqualTo("n"));
        Assert.That(IsJakarJson(typeof(Fallback)), Is.False, "JJSON016: no Jakar.Json code");
    }


    private static bool IsJakarJson( Type type ) => type.GetInterfaces().Any(static i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IJsonSerializable<>));


    // ─── No context: Jakar.Json only ─────────────────────────────────────────

    [Test] public void NoContext_IsJakarJsonOnly()
    {
        Assert.That(typeof(IJsonSerializable<LooseCoordinate>).IsAssignableFrom(typeof(LooseCoordinate)), Is.True);
        Assert.That(typeof(LooseCoordinate).GetInterfaces().Any(static i => i.Name.StartsWith("IJsonModel", StringComparison.Ordinal)), Is.False);
        Assert.That(LooseCoordinate.FromJson(new LooseCoordinate(1, 2).ToJson()), Is.EqualTo(new LooseCoordinate(1, 2)), "Jakar.Json's own helpers");
    }
}



public enum ModeStatus
{
    Draft,
    Sent,
    Paid
}



[JsonModel(typeof(ModesContext))]
public sealed partial class Hybrid : IEquatable<Hybrid>
{
    public string       DisplayName { get; set; } = "";
    public int          Count       { get; set; }
    public List<string> Tags        { get; set; } = [];
    public ModeStatus   Status      { get; set; }


    public          bool Equals( Hybrid? other ) => other is not null && DisplayName == other.DisplayName && Count == other.Count && Tags.SequenceEqual(other.Tags) && Status == other.Status;
    public override bool Equals( object? obj )   => obj is Hybrid other && Equals(other);
    public override int  GetHashCode()           => HashCode.Combine(DisplayName, Count, Status);
    public static   bool operator ==( Hybrid? left, Hybrid? right ) => Equals(left, right);
    public static   bool operator !=( Hybrid? left, Hybrid? right ) => !Equals(left, right);
}



[JsonModel(typeof(ModesContext))] public sealed partial record Position( int X, int Y );



[JsonModel(typeof(ModesContext), Implementation = JsonModelImplementation.SystemTextJson)]
public sealed partial class StjOnly
{
    public string Name { get; set; } = "";
}



/// <summary> A member converter Jakar.Json can't run: the model falls back to System.Text.Json (JJSON016). </summary>
[JsonModel(typeof(ModesContext))]
public sealed partial class Fallback
{
    [JsonConverter(typeof(ShoutConverter))] public string Name { get; set; } = "";
}



public sealed class ShoutConverter : JsonConverter<string>
{
    public override string Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options ) => reader.GetString()!.TrimEnd('!').ToLowerInvariant();
    public override void   Write( Utf8JsonWriter    writer, string value,       JsonSerializerOptions options ) => writer.WriteStringValue(value.ToUpperInvariant() + "!");
}



[JsonModel] public sealed partial record LooseCoordinate( int X, int Y );



[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
                             PropertyNameCaseInsensitive = true,
                             NumberHandling = JsonNumberHandling.AllowReadingFromString,
                             AllowTrailingCommas = true,
                             UseStringEnumConverter = true)]
[JsonSerializable(typeof(Hybrid))]
[JsonSerializable(typeof(Position))]
[JsonSerializable(typeof(StjOnly))]
[JsonSerializable(typeof(Fallback))]
public sealed partial class ModesContext : JsonSerializerContext;
