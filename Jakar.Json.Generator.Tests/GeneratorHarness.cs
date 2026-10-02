// Jakar.Json.Generator.Tests
// 10/02/2026

global using NUnit.Framework;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;



namespace Jakar.Json.Generator.Tests;


internal static class GeneratorHarness
{
    public static readonly CSharpParseOptions TestParseOptions = new(LanguageVersion.Latest);

    private static readonly ImmutableArray<MetadataReference> __references = [
        .. ( (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")! ).Split(Path.PathSeparator)
                                                                       .Where(static path => !string.IsNullOrEmpty(path))
                                                                       .Select(static path => MetadataReference.CreateFromFile(path))
    ];


    /// <summary> Usings plus a hand-written stand-in for a source-generated context (System.Text.Json's generator doesn't run here). </summary>
    public const string PRELUDE = """
                                  using System;
                                  using System.IO;
                                  using System.Threading;
                                  using System.Threading.Tasks;
                                  using System.Diagnostics.CodeAnalysis;
                                  using System.Text.Json;
                                  using System.Text.Json.Serialization;
                                  using System.Text.Json.Serialization.Metadata;
                                  using Jakar.Extensions;

                                  """;


    /// <summary> A fake context: <c> [JsonSerializable] </c> attributes plus the properties System.Text.Json would generate. </summary>
    public static string Context( string name, params (string Type, string Property, string? TypeInfoPropertyName)[] registrations ) => Context(name, "public", registrations);


    public static string Context( string name, string accessibility, params (string Type, string Property, string? TypeInfoPropertyName)[] registrations )
    {
        string attributes = string.Concat(registrations.Select(static r => r.TypeInfoPropertyName is null
                                                                               ? $"[JsonSerializable(typeof({r.Type}))]\n"
                                                                               : $"[JsonSerializable(typeof({r.Type}), TypeInfoPropertyName = \"{r.TypeInfoPropertyName}\")]\n"));

        string properties = string.Concat(registrations.Select(static r => $"    public JsonTypeInfo<{r.Type}> {r.Property} => null!;\n"));

        return $$"""
                 {{attributes}}{{accessibility}} sealed class {{name}} : JsonSerializerContext
                 {
                     public {{name}}() : base(null) { }
                     public static {{name}} Default { get; } = new();
                 {{properties}}    protected override JsonSerializerOptions? GeneratedSerializerOptions => null;
                     public override JsonTypeInfo? GetTypeInfo( Type type ) => null;
                 }

                 """;
    }


    public static CSharpCompilation CreateCompilation( params string[] sources ) =>
        CSharpCompilation.Create("TestAssembly",
                                 sources.Select(static (source, i) => CSharpSyntaxTree.ParseText(source, TestParseOptions, $"Source{i}.cs")),
                                 __references,
                                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));


    public static GeneratorDriver CreateDriver() => CSharpGeneratorDriver.Create([new JsonModelGenerator().AsSourceGenerator()], parseOptions: TestParseOptions, driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, true));


    public sealed record Result( Compilation Output, ImmutableArray<Diagnostic> GeneratorDiagnostics, ImmutableArray<Diagnostic> CompileErrors, ImmutableDictionary<string, string> Sources )
    {
        public string Model( string hintStart ) => Sources.Single(pair => pair.Key.StartsWith(hintStart, StringComparison.Ordinal)).Value;

        public string? Registrations => Sources.TryGetValue("JsonModelRegistrations.g.cs", out string? text)
                                            ? text
                                            : null;

        public IEnumerable<string> Ids => GeneratorDiagnostics.Select(static d => d.Id);
    }


    public static Result Run( params string[] sources )
    {
        CSharpCompilation compilation = CreateCompilation(sources);
        GeneratorDriver   driver      = CreateDriver().RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out ImmutableArray<Diagnostic> diagnostics);

        ImmutableDictionary<string, string> generated = driver.GetRunResult().Results.SelectMany(static r => r.GeneratedSources).ToImmutableDictionary(static s => s.HintName, static s => s.SourceText.ToString());

        ImmutableArray<Diagnostic> errors = [.. output.GetDiagnostics().Where(static d => d.Severity == DiagnosticSeverity.Error)];
        return new Result(output, diagnostics, errors, generated);
    }


    public static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync( params string[] sources )
    {
        CSharpCompilation compilation = CreateCompilation(sources);
        CreateDriver().RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out _);
        CompilationWithAnalyzers withAnalyzers = output.WithAnalyzers([new JsonModelAnalyzer()]);
        return await withAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
