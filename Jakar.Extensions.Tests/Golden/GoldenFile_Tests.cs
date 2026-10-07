// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

// Release gate for AOT-plan decision 2 ("11.0 must read JSON written by 10.x"):
//   Golden/v10/*.json was written by Jakar.Extensions 10.x (Newtonsoft) from GoldenSamples (see the generator note in GoldenSamples.cs).
//   - Read:  11.0 deserializes each file, re-serializes it, and must get the same JSON back.
//   - Write: 11.0 serializes the same deterministic samples and must produce the same JSON 10.x did.
// Known, intended differences are listed per file with the reason; anything else fails.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions.Tests.Golden;


[TestFixture]
public sealed class GoldenFile_Tests
{
    private static readonly string __directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Golden", "v10");


    /// <summary> File name → (read with 11.0, write with 11.0). </summary>
    private static readonly Dictionary<string, Func<string, string>> __readers = new()
                                                                                 {
                                                                                     ["AppVersion"]          = static json => AppVersion.FromJson(json).ToJson(),
                                                                                     ["AppInformation"]      = static json => AppInformation.FromJson(json).ToJson(),
                                                                                     ["Pair"]                = static json => Pair.FromJson(json).ToJson(),
                                                                                     ["StringTags"]          = static json => StringTags.FromJson(json).ToJson(),
                                                                                     ["Error"]               = static json => Json.Serialize(json.FromJson<Error>()),
                                                                                     ["Errors"]              = static json => Errors.FromJson(json).ToJson(),
                                                                                     ["Alert"]               = static json => Alert.FromJson(json).ToJson(),
                                                                                     ["ErrorResponse"]       = static json => ErrorResponse.FromJson(json).ToJson(),
                                                                                     ["FileMetaData"]        = static json => FileMetaData.FromJson(json).ToJson(),
                                                                                     ["LoginRequest"]        = static json => LoginRequest.FromJson(json).ToJson(),
                                                                                     ["LoginRequestVersion"] = static json => LoginRequestVersion.FromJson(json).ToJson(),
                                                                                     ["UserAddress"]         = static json => UserAddress.FromJson(json).ToJson(),
                                                                                     ["GroupModel"]          = static json => GroupModel.FromJson(json).ToJson(),
                                                                                     ["RoleModel"]           = static json => RoleModel.FromJson(json).ToJson(),
                                                                                     ["UserModel"]           = static json => UserModel.FromJson(json).ToJson()
                                                                                 };


    /// <summary> Property names (at any depth) that may differ, with the reason. Keep this list short and justified. </summary>
    private static readonly Dictionary<string, string> __knownDifferences = new()
                                                                            {
                                                                                // Computed, read-only values Newtonsoft wrote and nothing reads back. The STJ converters for Pair / StringTags write only the data.
                                                                                ["HasValue"] = "Pair.HasValue: computed from Value",
                                                                                ["IsEmpty"]  = "StringTags.IsEmpty: computed from Tags/Entries"
                                                                            };


    /// <summary> Properties where JSON null and [] mean the same thing (both read back as empty). A default StringTags wrote null for its arrays in 10.x and [] in 11.0. </summary>
    private static readonly HashSet<string> __nullEqualsEmpty = ["Tags", "Entries"];


    public static IEnumerable<string> Files() => __readers.Keys;


    [TestCaseSource(nameof(Files))] public void Reads10xJson( string name )
    {
        string legacy = File.ReadAllText(Path.Combine(__directory, $"{name}.json"));
        string again  = __readers[name](legacy);
        AssertSameJson(name, legacy, again);
    }


    [TestCaseSource(nameof(Files))] public void WritesThe10xShape( string name )
    {
        string legacy  = File.ReadAllText(Path.Combine(__directory, $"{name}.json"));
        string current = GoldenSamples.All().Single(x => x.Name == name).Json;
        AssertSameJson(name, legacy, current);
    }


    private static void AssertSameJson( string name, string expected, string actual )
    {
        JsonNode? left  = Strip(JsonNode.Parse(expected));
        JsonNode? right = Strip(JsonNode.Parse(actual));

        Assert.That(JsonNode.DeepEquals(left, right), Is.True, $"{name}: JSON differs.\n--- 10.x ---\n{left?.ToJsonString()}\n--- 11.0 ---\n{right?.ToJsonString()}");
    }


    private static JsonNode? Strip( JsonNode? node )
    {
        switch ( node )
        {
            case JsonObject obj:
                foreach ( string key in obj.Select(static pair => pair.Key).Where(__knownDifferences.ContainsKey).ToArray() ) { obj.Remove(key); }

                foreach ( string key in obj.Where(static pair => pair.Value is null && __nullEqualsEmpty.Contains(pair.Key)).Select(static pair => pair.Key).ToArray() ) { obj[key] = new JsonArray(); }

                foreach ( KeyValuePair<string, JsonNode?> pair in obj.ToArray() ) { Strip(pair.Value); }

                break;

            case JsonArray array:
                foreach ( JsonNode? item in array ) { Strip(item); }

                break;
        }

        return node;
    }
}
