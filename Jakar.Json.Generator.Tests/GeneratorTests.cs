// Jakar.Json.Generator.Tests
// 10/07/2026

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Jakar.Json.Generator.Tests.GeneratorHarness;



namespace Jakar.Json.Generator.Tests;


[TestFixture]
public sealed class GeneratorTests
{
    // ─── Output ──────────────────────────────────────────────────────────────

    [Test] public void Record_GeneratesCompilingCode()
    {
        Result result = Run(PRELUDE +
                            """
                            namespace Shop;

                            [GenerateJson(Naming = JsonNaming.CamelCase)]
                            public sealed partial record Item( string Sku, int Quantity );
                            """);

        Assert.That(result.GeneratorDiagnostics, Is.Empty);
        Assert.That(result.CompileErrors,        Is.Empty, string.Join("\n", result.CompileErrors));

        string source = result.Source("Shop.Item");
        Assert.That(source, Does.Contain("partial record Item : global::Jakar.Json.IJsonSerializable<global::Shop.Item>"));
        Assert.That(source, Does.Contain("new(\"\\\"sku\\\" : \", \"\\\"sku\\\" : \"u8)"), "pre-encoded camelCase name");
        Assert.That(source, Does.Contain("public static bool TryReadJson<TReader>"));
        Assert.That(source, Does.Contain("public bool TryFormat( global::System.Span<char> destination"));
        Assert.That(source, Does.Contain("public override string ToString()"));
    }

    [Test] public void AssemblyDefaults_AndTypeOverrides_Resolve()
    {
        Result result = Run(PRELUDE +
                            """
                            [assembly: JsonDefaults(Naming = JsonNaming.SnakeCaseLower, Indented = JsonToggle.On)]

                            [GenerateJson]
                            public sealed partial class A { public int FirstValue { get; set; } }

                            [GenerateJson(Naming = JsonNaming.KebabCaseUpper, Indented = JsonToggle.Off)]
                            public sealed partial class B { public int FirstValue { get; set; } }
                            """);

        Assert.That(result.CompileErrors, Is.Empty);
        Assert.That(result.Source("A."),  Does.Contain("\\\"first_value\\\""));
        Assert.That(result.Source("A."),  Does.Contain("Indented = true"));
        Assert.That(result.Source("B."),  Does.Contain("\\\"FIRST-VALUE\\\""));
        Assert.That(result.Source("B."),  Does.Contain("Indented = false"));
    }

    [Test] public void Enums_GetOneConverterPerNamingPolicy()
    {
        Result result = Run(PRELUDE +
                            """
                            public enum Color { Red, DarkBlue }

                            [GenerateJson(EnumNaming = JsonNaming.CamelCase)]
                            public sealed partial class A { public Color C { get; set; } public Dictionary<Color, int> Counts { get; set; } = []; }

                            [GenerateJson]
                            public sealed partial class B { public Color C { get; set; } }

                            [GenerateJson(Enums = JsonEnumFormat.Number)]
                            public sealed partial class N { public Color C { get; set; } }
                            """);

        Assert.That(result.CompileErrors, Is.Empty, string.Join("\n", result.CompileErrors));

        string enums = result.Source("Jakar.Json.Enums");
        Assert.That(enums,               Does.Contain("internal readonly struct Color__2"));
        Assert.That(enums,               Does.Contain("internal readonly struct Color__1"));
        Assert.That(enums,               Does.Contain("\"darkBlue\""));
        Assert.That(result.Source("N."), Does.Contain("JsonEnumNumberConverter<global::Color, int>"));
    }

    [Test] public void Polymorphism_DispatchesMostDerivedFirst()
    {
        Result result = Run(PRELUDE +
                            """
                            [GenerateJson]
                            [JsonDerived(typeof(Dog), "dog")]
                            [JsonDerived(typeof(Puppy), "puppy")]
                            public abstract partial class Animal { public string Name { get; set; } = ""; }

                            [GenerateJson] public partial class Dog : Animal { }
                            [GenerateJson] public sealed partial class Puppy : Dog { }
                            """);

        Assert.That(result.CompileErrors, Is.Empty, string.Join("\n", result.CompileErrors));

        string animal = result.Source("Animal.");
        Assert.That(animal.IndexOf("case global::Puppy", StringComparison.Ordinal), Is.LessThan(animal.IndexOf("case global::Dog", StringComparison.Ordinal)));
        Assert.That(result.Source("Puppy."),                                        Does.Contain("writer.WriteString(\"puppy\");"));
    }


    // ─── Diagnostics ─────────────────────────────────────────────────────────

    [Test] public void JJSON001_NotPartial()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed class A { }\npublic class Outer { [GenerateJson] public sealed partial class Inner { } }");
        Assert.That(result.Ids.Count(static id => id == "JJSON001"), Is.EqualTo(2));
    }

    [Test] public void JJSON002_UnconstrainedTypeParameter()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed partial class A<T> { public T Value { get; set; } = default!; }");
        Assert.That(result.Ids, Does.Contain("JJSON002"));

        Result ok = Run(PRELUDE + "[GenerateJson] public sealed partial class A<T> where T : IJsonSerializable<T> { public T? Value { get; set; } }");
        Assert.That(ok.GeneratorDiagnostics, Is.Empty);
        Assert.That(ok.CompileErrors,        Is.Empty, string.Join("\n", ok.CompileErrors));
    }

    [Test] public void JJSON003_UnsupportedMember()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed partial class A { public System.Threading.Thread? T { get; set; } }");
        Assert.That(result.Ids, Does.Contain("JJSON003"));
    }

    [Test] public void JJSON004_UnboundConstructorParameter()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed partial class A { public A( int unmatched ) { } public int Value { get; set; } }");
        Assert.That(result.Ids, Does.Contain("JJSON004"));
    }

    [Test] public void JJSON005_NameCollision()
    {
        Result exact = Run(PRELUDE + "[GenerateJson] public sealed partial class A { [JsonMember(Name = \"x\")] public int One { get; set; } [JsonMember(Name = \"x\")] public int Two { get; set; } }");
        Assert.That(exact.Ids, Does.Contain("JJSON005"));

        Result folded = Run(PRELUDE + "[GenerateJson(NameMatching = JsonNameMatching.OrdinalIgnoreCase)] public sealed partial class A { public int Value { get; set; } public int value { get; set; } }");
        Assert.That(folded.Ids, Does.Contain("JJSON005"));
    }

    [Test] public void JJSON006_MembersAcrossFiles()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed partial class A { public int One { get; set; } }", PRELUDE + "public sealed partial class A { public int Two { get; set; } }");
        Assert.That(result.Ids,                                                                              Does.Contain("JJSON006"));
        Assert.That(result.GeneratorDiagnostics.First(static d => d.Id == "JJSON006").Properties["Members"], Is.EqualTo("One,Two"));

        Result ordered = Run(PRELUDE + "[GenerateJson] public sealed partial class A { [JsonMember(Order = 1)] public int One { get; set; } }", PRELUDE + "public sealed partial class A { [JsonMember(Order = 2)] public int Two { get; set; } }");
        Assert.That(ordered.Ids, Does.Not.Contain("JJSON006"));
    }

    [Test] public void JJSON007_HandWrittenMembersAreKept()
    {
        Result result = Run(PRELUDE +
                            """
                            [GenerateJson]
                            public sealed partial class A
                            {
                                public int Value { get; set; }
                                public static A FromJson( string json, JsonReaderOptions? options = null ) => new();
                            }
                            """);

        Assert.That(result.Ids,           Does.Contain("JJSON007"));
        Assert.That(result.CompileErrors, Is.Empty, string.Join("\n", result.CompileErrors));
    }

    [Test] public void JJSON008_ContradictorySettings()
    {
        Result capture = Run(PRELUDE + "[GenerateJson(UnknownMembers = JsonUnknownMembers.Capture)] public sealed partial class A { public int Value { get; set; } }");
        Assert.That(capture.Ids, Does.Contain("JJSON008"));

        Result required = Run(PRELUDE + "[GenerateJson(NullValues = JsonNullValues.Omit)] public sealed partial class A { [JsonMember(Required = true)] public string? Value { get; set; } }");
        Assert.That(required.Ids, Does.Contain("JJSON008"));

        Result extension = Run(PRELUDE + "[GenerateJson] public sealed partial class A { [JsonMember(ExtensionData = true)] public Dictionary<string, int>? Extra { get; set; } }");
        Assert.That(extension.Ids, Does.Contain("JJSON008"));
    }

    [Test] public void JJSON009_ConverterForTheWrongType()
    {
        Result result = Run(PRELUDE +
                            """
                            public readonly struct IntConverter : IJsonConverter<int>
                            {
                                public static void Write<TWriter>( ref TWriter writer, scoped in int value ) where TWriter : IJsonWriter, allows ref struct => writer.WriteInteger(value);
                                public static bool TryRead<TReader>( ref TReader reader, out int value ) where TReader : IJsonReader, allows ref struct => reader.TryReadInteger(out value);
                            }

                            [GenerateJson] public sealed partial class A { [JsonMember(Converter = typeof(IntConverter))] public string Value { get; set; } = ""; }
                            [GenerateJson] public sealed partial class B { [JsonMember(Converter = typeof(IntConverter))] public int? Value { get; set; } }
                            """);

        Assert.That(result.GeneratorDiagnostics.Where(static d => d.Id == "JJSON009"), Has.Exactly(1).Items, "int? reuses the int converter");
        Assert.That(result.Source("B."),                                               Does.Contain("JsonNullableConverter<int, global::IntConverter>"));
    }

    [Test] public void JJSON010_BothGenerators()
    {
        Result result = Run(PRELUDE +
                            """
                            namespace Jakar.Extensions { public sealed class JsonModelAttribute : System.Attribute { } }

                            [GenerateJson, Jakar.Extensions.JsonModel] public sealed partial class A { }
                            """);

        Assert.That(result.Ids, Does.Contain("JJSON010"));
    }

    [Test] public void JJSON011_ComputedMember()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed partial class A { public int Value { get; set; } public int Double => Value * 2; }");
        Assert.That(result.Ids,           Does.Contain("JJSON011"));
        Assert.That(result.CompileErrors, Is.Empty);
    }

    [Test] public void JJSON012_NoJsonForm()
    {
        Result result = Run(PRELUDE + "[GenerateJson] public sealed partial class A { public object? Any { get; set; } public Action? Callback { get; set; } }");
        Assert.That(result.Ids.Count(static id => id == "JJSON012"), Is.EqualTo(2));
    }

    [Test] public void JJSON013_DeterminismWeakened()
    {
        Result result = Run(PRELUDE +
                            """
                            [GenerateJson(UnorderedCollections = JsonUnorderedCollections.Enumeration)] public sealed partial class A { public HashSet<int> Set { get; set; } = []; }
                            [GenerateJson(LocalDateTimes = JsonLocalDateTimes.WriteOffset)] public sealed partial class B { public DateTime When { get; set; } }
                            [GenerateJson] public sealed partial class C { public HashSet<B> Unsortable { get; set; } = []; }
                            """);

        Assert.That(result.Ids.Count(static id => id == "JJSON013"), Is.EqualTo(3));
        Assert.That(result.CompileErrors,                            Is.Empty, string.Join("\n", result.CompileErrors));
    }

    [Test] public void JJSON014_InvalidDerived()
    {
        Result result = Run(PRELUDE +
                            """
                            [GenerateJson]
                            [JsonDerived(typeof(Unrelated), "u")]
                            [JsonDerived(typeof(Plain), "p")]
                            public abstract partial class Base { }
                            [GenerateJson] public sealed partial class Unrelated { }
                            public sealed class Plain : Base { }
                            """);

        Assert.That(result.Ids.Count(static id => id == "JJSON014"), Is.EqualTo(2));
    }

    [Test] public async Task JJSON015_InterfaceWithoutAttribute()
    {
        ImmutableArray<Diagnostic> diagnostics = await AnalyzeAsync(PRELUDE + "public abstract class A : IJsonSerializable<A> { }");
        Assert.That(diagnostics.Select(static d => d.Id), Does.Contain("JJSON015"));
    }


    // ─── Incrementality ──────────────────────────────────────────────────────

    [Test] public void EditingAnUnrelatedFile_DoesNotReExtractModels()
    {
        SyntaxTree model     = CSharpSyntaxTree.ParseText(PRELUDE + "[GenerateJson] public sealed partial record A( int Value );", TestParseOptions, "Model.cs");
        SyntaxTree unrelated = CSharpSyntaxTree.ParseText("public static class Other { public static int X => 1; }",               TestParseOptions, "Other.cs");

        CSharpCompilation compilation = CreateCompilation().AddSyntaxTrees(model, unrelated);
        GeneratorDriver   driver      = CreateDriver(track: true).RunGenerators(compilation);

        CSharpCompilation edited = compilation.ReplaceSyntaxTree(unrelated, CSharpSyntaxTree.ParseText("public static class Other { public static int X => 2; }", TestParseOptions, "Other.cs"));
        driver = driver.RunGenerators(edited);

        GeneratorRunResult             run     = driver.GetRunResult().Results[0];
        List<IncrementalStepRunReason> reasons = run.TrackedOutputSteps.SelectMany(static step => step.Value).SelectMany(static output => output.Outputs).Select(static output => output.Reason).ToList();

        Assert.That(reasons, Is.Not.Empty);
        Assert.That(reasons, Is.All.EqualTo(IncrementalStepRunReason.Cached).Or.EqualTo(IncrementalStepRunReason.Unchanged));
    }
}
