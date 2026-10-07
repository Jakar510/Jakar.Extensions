// Jakar.SystemTextJson.Generator.Tests
// 10/02/2026

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Jakar.SystemTextJson.Generator.Tests.GeneratorHarness;



namespace Jakar.SystemTextJson.Generator.Tests;


[TestFixture]
public sealed class JsonModelGenerator_Tests
{
    private static string Errors( Result result ) => string.Join(Environment.NewLine, result.CompileErrors.Select(static e => e.ToString()));


    [Test] public void Class_GeneratesEverything_AndCompiles()
    {
        Result result = Run(PRELUDE +
                            """
                            namespace App;
                            [JsonModel(typeof(AppContext))]
                            public sealed partial class Invoice { public decimal Total { get; init; } }
                            """     +
                            Context("AppContext", ( "Invoice", "Invoice", null )));

        Assert.That(result.GeneratorDiagnostics, Is.Empty);
        Assert.That(result.CompileErrors,        Is.Empty, Errors(result));

        string source = result.Model("App.Invoice");
        Assert.That(source,               Does.Contain("partial class Invoice : global::Jakar.Extensions.IJsonModel<global::App.Invoice>"));
        Assert.That(source,               Does.Contain("JsonTypeInfo => global::App.AppContext.Default.Invoice;"));
        Assert.That(source,               Does.Contain("public static global::App.Invoice FromJson( string json )"));
        Assert.That(source,               Does.Contain("FromJson( global::System.ReadOnlySpan<byte> utf8Json )"));
        Assert.That(source,               Does.Contain("out global::App.Invoice? result"));
        Assert.That(source,               Does.Contain("FromJsonAsync("));
        Assert.That(source,               Does.Not.Contain("ToString"));
        Assert.That(result.Registrations, Does.Contain("JsonModelRegistry.Register<global::App.Invoice>(static () => global::App.Invoice.JsonTypeInfo);"));
    }


    [Test] public void TypeInfoPropertyName_IsHonoured()
    {
        Result result = Run(PRELUDE +
                            """
                            [JsonModel(typeof(Ctx))]
                            public partial class Invoice;
                            """     +
                            Context("Ctx", ( "Invoice", "MyInvoice", "MyInvoice" )));

        Assert.That(result.CompileErrors,    Is.Empty, Errors(result));
        Assert.That(result.Model("Invoice"), Does.Contain("JsonTypeInfo => global::Ctx.Default.MyInvoice;"));
    }


    [Test] public void ReadOnlyRecordStruct_UsesNonNullableOut_AndKeepsModifiers()
    {
        Result result = Run(PRELUDE +
                            """
                            namespace App;
                            [JsonModel(typeof(Ctx), GenerateToString = true)]
                            public readonly partial record struct Point( int X, int Y );
                            """     +
                            Context("Ctx", ( "Point", "Point", null )));

        Assert.That(result.CompileErrors, Is.Empty, Errors(result));
        string source = result.Model("App.Point");
        Assert.That(source, Does.Contain("readonly partial record struct Point"));
        Assert.That(source, Does.Contain("out global::App.Point result"));
        Assert.That(source, Does.Not.Contain("out global::App.Point? result"));
        Assert.That(source, Does.Contain("public override string ToString()"));
    }


    [Test] public void NestedType_EmitsContainingDeclarations()
    {
        Result result = Run(PRELUDE +
                            """
                            namespace App;
                            public static partial class Outer
                            {
                                internal sealed partial class Middle
                                {
                                    [JsonModel(typeof(Ctx))]
                                    public sealed partial record Note( string Text );
                                }
                            }
                            """     +
                            Context("Ctx", "internal", ( "Outer.Middle.Note", "Note", null )));

        Assert.That(result.GeneratorDiagnostics,           Is.Empty);
        Assert.That(result.CompileErrors,                  Is.Empty, Errors(result));
        Assert.That(result.Model("App.Outer_Middle_Note"), Does.Contain("partial class Outer").And.Contain("partial class Middle").And.Contain("partial record Note"));
        Assert.That(result.Registrations,                  Does.Contain("global::App.Outer.Middle.Note"));
    }


    [Test] public void PrivateNestedType_IsGenerated_ButNotRegistered()
    {
        Result result = Run(PRELUDE                                                 +
                            """
                            public partial class Outer
                            {
                                [JsonModel(typeof(Ctx))]
                                private sealed partial class Hidden;

                            """                                                     +
                            Context("Ctx", "private", ( "Hidden", "Hidden", null )) +
                            "}");

        Assert.That(result.CompileErrors, Is.Empty, Errors(result));
        Assert.That(result.Sources.Keys,  Has.Some.StartsWith("Outer_Hidden"));
        Assert.That(result.Registrations, Is.Null);
    }


    [Test] public void BaseClassStatics_AreNotEmittedAgain()
    {
        Result result = Run(PRELUDE +
                            """
                            public abstract class ModelBase<TSelf> where TSelf : ModelBase<TSelf>, IJsonModel<TSelf>
                            {
                                public static TSelf FromJson( string json ) => JsonModel.FromJson(json, TSelf.JsonTypeInfo);
                                public static bool TryFromJson( [NotNullWhen(true)] string? json, [NotNullWhen(true)] out TSelf? result ) => JsonModel.TryFromJson(json, TSelf.JsonTypeInfo, out result);
                                public override string ToString() => "base";
                            }

                            [JsonModel(typeof(Ctx), GenerateToString = true)]
                            public sealed partial class Invoice : ModelBase<Invoice>;
                            """     +
                            Context("Ctx", ( "Invoice", "Invoice", null )));

        Assert.That(result.CompileErrors,                                               Is.Empty, Errors(result));
        Assert.That(result.Output.GetDiagnostics().Where(static d => d.Id == "CS0108"), Is.Empty);

        string source = result.Model("Invoice");
        Assert.That(source, Does.Not.Contain("FromJson( string json )"));
        Assert.That(source, Does.Not.Contain("TryFromJson( [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? json"));
        Assert.That(source, Does.Contain("FromJson( global::System.ReadOnlySpan<byte> utf8Json )"));
        Assert.That(source, Does.Not.Contain("ToString"), "the base already overrides ToString");
    }


    [Test] public void AssemblyDefaultContext_IsUsed()
    {
        Result result = Run(PRELUDE +
                            """
                            [assembly: JsonModelContext(typeof(Ctx))]
                            [JsonModel]
                            public partial class Invoice;
                            """     +
                            Context("Ctx", ( "Invoice", "Invoice", null )));

        Assert.That(result.GeneratorDiagnostics, Is.Empty);
        Assert.That(result.CompileErrors,        Is.Empty, Errors(result));
        Assert.That(result.Model("Invoice"),     Does.Contain("global::Ctx.Default.Invoice"));
    }


    // ─── Diagnostics ──────────────────────────────────────────────────────────

    [Test] public void JAKAR_JSON001_NotRegistered_StillCompiles()
    {
        Result result = Run(PRELUDE +
                            """
                            namespace App;
                            [JsonModel(typeof(Ctx))]
                            public partial class Invoice;
                            [JsonModel(typeof(Ctx))]
                            public partial class Other;
                            """     +
                            Context("Ctx", ( "Other", "Other", null )));

        Diagnostic diagnostic = result.GeneratorDiagnostics.Single();
        Assert.That(diagnostic.Id,                                                     Is.EqualTo("JAKAR_JSON001"));
        Assert.That(diagnostic.Properties[JsonModelDiagnostics.CONTEXT_METADATA_NAME], Is.EqualTo("App.Ctx"));
        Assert.That(diagnostic.Properties[JsonModelDiagnostics.TYPE_NAME],             Is.EqualTo("global::App.Invoice"));
        Assert.That(result.CompileErrors,                                              Is.Empty, Errors(result));
        Assert.That(result.Model("App.Invoice"),                                       Does.Contain("throw new global::System.InvalidOperationException"));
        Assert.That(result.Registrations,                                              Does.Not.Contain("global::App.Invoice>"));
    }


    [Test] public void JAKAR_JSON002_NotPartial()
    {
        Result result = Run(PRELUDE +
                            """
                            public class Outer
                            {
                                [JsonModel(typeof(Ctx))]
                                public partial class Inner;
                            }
                            """     +
                            Context("Ctx", ( "Outer.Inner", "Inner", null )));

        Assert.That(result.Ids,                                                 Is.EqualTo(["JAKAR_JSON002"]));
        Assert.That(result.Sources.Keys.Where(static k => k.Contains("Inner")), Is.Empty);
    }


    [Test] public void JAKAR_JSON003_OpenGeneric()
    {
        Result result = Run(PRELUDE +
                            """
                            [JsonModel(typeof(Ctx))]
                            public partial class Box<T>;
                            """     +
                            Context("Ctx"));

        Assert.That(result.Ids, Is.EqualTo(["JAKAR_JSON003"]));
    }


    [TestCase("[JsonModel(typeof(string))]", "isn't a JsonSerializerContext")] [TestCase("[JsonModel]", "no [assembly: JsonModelContext")]
    public void JAKAR_JSON004_InvalidOrMissingContext( string attribute, string message )
    {
        Result result = Run(PRELUDE + $"{attribute}\npublic partial class Invoice;\n");

        Diagnostic diagnostic = result.GeneratorDiagnostics.Single();
        Assert.That(diagnostic.Id,           Is.EqualTo("JAKAR_JSON004"));
        Assert.That(diagnostic.GetMessage(), Does.Contain(message));
    }


    [Test] public void JAKAR_JSON004_InvalidAssemblyDefault()
    {
        Result result = Run(PRELUDE +
                            """
                            [assembly: JsonModelContext(typeof(string))]
                            [JsonModel]
                            public partial class Invoice;
                            """);

        Assert.That(result.GeneratorDiagnostics.Single().GetMessage(), Does.Contain("JsonModelContext(typeof(string))"));
    }


    [Test] public void JAKAR_JSON005_HandWritten()
    {
        Result result = Run(PRELUDE +
                            """
                            [JsonModel(typeof(Ctx))]
                            public partial class Invoice
                            {
                                public static JsonTypeInfo<Invoice> JsonTypeInfo => Ctx.Default.Invoice;
                            }
                            """     +
                            Context("Ctx", ( "Invoice", "Invoice", null )));

        Assert.That(result.Ids,              Is.EqualTo(["JAKAR_JSON005"]));
        Assert.That(result.CompileErrors,    Is.Empty, Errors(result));
        Assert.That(result.Model("Invoice"), Does.Not.Contain("JsonTypeInfo =>"));
    }


    // ─── Analyzer ─────────────────────────────────────────────────────────────

    [Test] public async Task JAKAR_JSON006_InterfaceWithoutAttribute()
    {
        var diagnostics = await AnalyzeAsync(PRELUDE +
                                             """
                                             public partial class Invoice : IJsonModel<Invoice>;

                                             public partial class HandWritten : IJsonModel<HandWritten>
                                             {
                                                 public static JsonTypeInfo<HandWritten> JsonTypeInfo => null!;
                                             }
                                             """);

        Assert.That(diagnostics.Select(static d => d.Id), Is.EqualTo(["JAKAR_JSON006"]));
        Assert.That(diagnostics[0].GetMessage(),          Does.Contain("'Invoice'"));
    }


    [Test] public async Task JAKAR_JSON007_AdditionalDataWithoutExtensionData()
    {
        var diagnostics = await AnalyzeAsync(PRELUDE +
                                             """
                                             using System.Collections.Generic;
                                             public class Good : IJsonModel { [JsonExtensionData] public Dictionary<string, JsonElement>? AdditionalData { get; set; } }
                                             public class Bad  : IJsonModel { public Dictionary<string, JsonElement>? AdditionalData { get; set; } }
                                             """);

        Assert.That(diagnostics.Select(static d => d.Id), Is.EqualTo(["JAKAR_JSON007"]));
        Assert.That(diagnostics[0].GetMessage(),          Does.Contain("'Bad.AdditionalData'"));
    }


    // ─── Incrementality ───────────────────────────────────────────────────────

    [Test] public void UnrelatedEdit_IsFullyCached()
    {
        CSharpCompilation compilation = CreateCompilation(PRELUDE +
                                                          """
                                                          [JsonModel(typeof(Ctx))]
                                                          public partial class Invoice;
                                                          """     +
                                                          Context("Ctx", ( "Invoice", "Invoice", null )),
                                                          "public static class Unrelated { public static int Value => 1; }");

        GeneratorDriver driver = CreateDriver().RunGenerators(compilation);

        SyntaxTree        unrelated = compilation.SyntaxTrees.Last();
        CSharpCompilation edited    = compilation.ReplaceSyntaxTree(unrelated, CSharpSyntaxTree.ParseText("public static class Unrelated { public static int Value => 2; }", TestParseOptions, unrelated.FilePath));

        GeneratorRunResult run = driver.RunGenerators(edited).GetRunResult().Results.Single();

        IncrementalStepRunReason[] reasons = [.. run.TrackedOutputSteps.SelectMany(static step => step.Value).SelectMany(static step => step.Outputs).Select(static output => output.Reason)];
        Assert.That(reasons, Is.Not.Empty);
        Assert.That(reasons, Is.All.AnyOf(IncrementalStepRunReason.Cached, IncrementalStepRunReason.Unchanged));
    }
}
