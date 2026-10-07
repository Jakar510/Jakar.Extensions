// Jakar.Json.Generator.Tests
// 10/07/2026

global using NUnit.Framework;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;



namespace Jakar.Json.Generator.Tests;


internal static class GeneratorHarness
{
    public static readonly CSharpParseOptions TestParseOptions = new(LanguageVersion.Latest);

    // The test host's trusted assemblies: the BCL, Jakar.Json and Jakar.Spans.
    private static readonly ImmutableArray<MetadataReference> __references = [.. ( (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")! ).Split(Path.PathSeparator).Where(static path => !string.IsNullOrEmpty(path)).Select(static path => MetadataReference.CreateFromFile(path))];


    public const string PRELUDE = """
                                  using System;
                                  using System.Collections.Generic;
                                  using Jakar.Json;

                                  """;


    public static CSharpCompilation CreateCompilation( params string[] sources ) =>
        CSharpCompilation.Create("TestAssembly", sources.Select(static ( source, i ) => CSharpSyntaxTree.ParseText(source, TestParseOptions, $"Source{i}.cs")), __references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));


    public static GeneratorDriver CreateDriver( bool track = false ) => CSharpGeneratorDriver.Create([new JsonGenerator().AsSourceGenerator()], parseOptions: TestParseOptions, driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, track));



    public sealed record Result( Compilation Output, ImmutableArray<Diagnostic> GeneratorDiagnostics, ImmutableArray<Diagnostic> CompileErrors, ImmutableDictionary<string, string> Sources )
    {
        public IEnumerable<string> Ids => GeneratorDiagnostics.Select(static d => d.Id);

        public string Source( string hintStart ) => Sources.Single(pair => pair.Key.StartsWith(hintStart, StringComparison.Ordinal)).Value;
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
        CompilationWithAnalyzers withAnalyzers = output.WithAnalyzers([new JsonAnalyzer()]);
        return await withAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
