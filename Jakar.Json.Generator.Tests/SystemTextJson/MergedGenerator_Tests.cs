// Jakar.Json.Generator.Tests
// 10/07/2026

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Jakar.Json.Generator.Tests.SystemTextJson.GeneratorHarness;



namespace Jakar.Json.Generator.Tests.SystemTextJson;


/// <summary>
///     One attribute, one generator, a mode per type (SPEC.md §7.7): no context → Jakar.Json only; a context → IJsonModel, implemented by Jakar.Json when it can (hybrid),
///     System.Text.Json otherwise (JJSON016) or when asked; <c>Implementation = JakarJson</c> makes a fallback an error (JJSON017).
/// </summary>
[TestFixture]
public sealed class MergedGenerator_Tests
{
    private const string MODELS = """
                                  [JsonModel] public sealed partial class Loose { public int Value { get; set; } }
                                  [JsonModel(typeof(Ctx))] public sealed partial class Hybrid { public int Value { get; set; } public Inner? Child { get; set; } }
                                  [JsonModel(typeof(Ctx))] public sealed partial class Inner { public string Name { get; set; } = ""; }
                                  [JsonModel(typeof(Ctx), Implementation = JsonModelImplementation.SystemTextJson)] public sealed partial class Forced { public int Value { get; set; } }
                                  [JsonModel(typeof(Ctx))] public sealed partial class Blocked { [JsonConverter(typeof(JsonStringEnumConverter))] public int Value { get; set; } }
                                  [JsonModel(typeof(Ctx))] public sealed partial class Outer { public Blocked? Item { get; set; } }
                                  """;


    private static string Contexts() => Context("Ctx", ( "Hybrid", "Hybrid", null ), ( "Inner", "Inner", null ), ( "Forced", "Forced", null ), ( "Blocked", "Blocked", null ), ( "Outer", "Outer", null ));

    private static Result RunModels() => Run(PRELUDE + "using Jakar.Json;\n" + Contexts() + MODELS);


    [Test] public void EachType_GetsItsMode()
    {
        Result result = RunModels();

        Assert.That(result.CompileErrors, Is.Empty, string.Join("\n", result.CompileErrors));

        Assert.That(Hints(result, "Loose"),   Is.EqualTo(new[] { "Loose.Json.g.cs" }),                           "no context: Jakar.Json only");
        Assert.That(Hints(result, "Hybrid"),  Is.EqualTo(new[] { "Hybrid.Json.g.cs", "Hybrid.JsonModel.g.cs" }), "context + Jakar.Json can: hybrid");
        Assert.That(Hints(result, "Forced"),  Is.EqualTo(new[] { "Forced.JsonModel.g.cs" }),                     "Implementation = SystemTextJson");
        Assert.That(Hints(result, "Blocked"), Is.EqualTo(new[] { "Blocked.JsonModel.g.cs" }),                    "a member converter Jakar.Json can't run: fallback");
        Assert.That(Hints(result, "Outer"),   Is.EqualTo(new[] { "Outer.JsonModel.g.cs" }),                      "a member whose model fell back makes its owner fall back too");

        Assert.That(result.Registrations, Does.Contain("Hybrid").And.Contain("Forced").And.Contain("Blocked").And.Not.Contain("Loose"));
    }

    [Test] public void Fallbacks_AreReported_AsInfo()
    {
        Result result = RunModels();

        Diagnostic[] fallbacks = [.. result.GeneratorDiagnostics.Where(static d => d.Id == "JJSON016")];

        Assert.That(fallbacks.Select(static d => d.GetMessage()), Has.Exactly(1).Contains("'Blocked'").And.Exactly(1).Contains("'Outer'"));
        Assert.That(fallbacks.Select(static d => d.Severity),     Is.All.EqualTo(DiagnosticSeverity.Info));
        Assert.That(fallbacks.Single(static d => d.GetMessage().Contains("'Blocked'")).GetMessage(), Does.Contain("[JsonConverter]"), "the reason");
        Assert.That(result.Ids,                                   Does.Not.Contain("JJSON003"), "the reason isn't an error: the type still works, through System.Text.Json");
    }

    [Test] public void Hybrid_RoutesIJsonModelThroughJakarJson()
    {
        Result result = RunModels();

        string model = result.Sources["Hybrid.JsonModel.g.cs"];
        Assert.That(model, Does.Contain("JsonTypeInfo => global::Ctx.Default.Hybrid"), "metadata stays System.Text.Json's");
        Assert.That(model, Does.Contain("FromJson( string json ) => global::Jakar.Json.JsonCodec.FromJson<global::Hybrid>(json)"));
        Assert.That(model, Does.Contain("static string global::Jakar.Extensions.IJsonModel<global::Hybrid>.SerializeJson("));

        string jakar = result.Sources["Hybrid.Json.g.cs"];
        Assert.That(jakar, Does.Contain("IJsonSerializable<global::Hybrid>").And.Contain("TryReadJson").And.Contain(" Parse("));
        Assert.That(jakar, Does.Not.Contain(" FromJson(").And.Not.Contain(" ToJson("), "the type's helpers are IJsonModel's");
        Assert.That(jakar, Does.Not.Contain("ToString()"),                             "System.Text.Json never overrides ToString(), so a mirrored model doesn't either");

        Assert.That(result.Sources["Forced.JsonModel.g.cs"],  Does.Contain("global::Jakar.Extensions.JsonModel.FromJson(json, JsonTypeInfo)").And.Not.Contain("global::Jakar.Json.JsonCodec"));
        Assert.That(result.Sources["Blocked.JsonModel.g.cs"], Does.Not.Contain("global::Jakar.Json.JsonCodec"));
    }

    [Test] public void ContextOptions_AreMirrored()
    {
        Result result = Run(PRELUDE +
                            "using Jakar.Json;\n" +
                            """
                            [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower, WriteIndented = true, PropertyNameCaseInsensitive = true, UseStringEnumConverter = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
                            """ +
                            Context("Ctx", ( "Mirrored", "Mirrored", null )) +
                            """
                            public enum Kind { A, B }
                            [JsonModel(typeof(Ctx))] public sealed partial class Mirrored { public string? DisplayName { get; set; } public Kind Kind { get; set; } }
                            [JsonModel(typeof(Ctx), Naming = JsonNaming.PascalCase)] public sealed partial class Overridden { public int Value { get; set; } }
                            """);

        Assert.That(result.CompileErrors, Is.Empty, string.Join("\n", result.CompileErrors));

        string mirrored = result.Sources["Mirrored.Json.g.cs"];
        Assert.That(mirrored, Does.Contain("\"display_name\""),                      "PropertyNamingPolicy");
        Assert.That(mirrored, Does.Contain("Indented = true"),                       "WriteIndented");
        Assert.That(mirrored, Does.Contain("IndentChar = (global::Jakar.Json.JsonIndentChar)2, IndentSize = 2"), "System.Text.Json's indentation: two spaces");
        Assert.That(mirrored, Does.Contain("OrdinalIgnoreCase").Or.Contain("IgnoreCase"), "PropertyNameCaseInsensitive");
        Assert.That(mirrored, Does.Contain("JsonEnums.Kind__1").And.Not.Contain("JsonEnumNumberConverter"), "UseStringEnumConverter");
        Assert.That(mirrored, Does.Contain("is not null"),                           "WhenWritingNull omits nulls");

        Assert.That(result.Sources["Overridden.Json.g.cs"], Does.Contain("\"Value\""), "the type's own settings win over its context's");
    }

    [Test] public void ImplementationJakarJson_MakesAFallbackAnError()
    {
        Result result = Run(PRELUDE +
                            "using Jakar.Json;\n" +
                            Context("Ctx", ( "Strict", "Strict", null )) +
                            """
                            [JsonModel(typeof(Ctx), Implementation = JsonModelImplementation.JakarJson)]
                            public sealed partial class Strict { [JsonConverter(typeof(JsonStringEnumConverter))] public int Value { get; set; } }
                            """);

        Diagnostic error = result.GeneratorDiagnostics.Single(static d => d.Id == "JJSON017");
        Assert.That(error.Severity,     Is.EqualTo(DiagnosticSeverity.Error));
        Assert.That(error.GetMessage(), Does.Contain("[JsonConverter]"));
        Assert.That(result.Ids,         Does.Not.Contain("JJSON016"));
    }

    [Test] public void IsIncremental()
    {
        // An edit unrelated to the models re-runs nothing.
        GeneratorDriver   driver = CreateDriver();
        CSharpCompilation first  = CreateCompilation(PRELUDE + "using Jakar.Json;\n" + Contexts(), MODELS);
        driver = driver.RunGenerators(first);

        CSharpCompilation  second = first.AddSyntaxTrees(CSharpSyntaxTree.ParseText("public static class Unrelated { }", TestParseOptions, "Source2.cs"));
        GeneratorRunResult run    = driver.RunGenerators(second).GetRunResult().Results.Single();

        IncrementalStepRunReason[] reasons = [.. run.TrackedOutputSteps.SelectMany(static step => step.Value).SelectMany(static output => output.Outputs).Select(static output => output.Reason)];

        Assert.That(reasons, Is.Not.Empty, "outputs are tracked");
        Assert.That(reasons, Is.All.EqualTo(IncrementalStepRunReason.Cached).Or.EqualTo(IncrementalStepRunReason.Unchanged));
    }


    private static string[] Hints( Result result, string type ) => [.. result.Sources.Keys.Where(key => key.StartsWith(type + ".", StringComparison.Ordinal)).Order(StringComparer.Ordinal)];
}
