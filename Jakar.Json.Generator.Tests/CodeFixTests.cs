// Jakar.Json.Generator.Tests
// 10/07/2026

using System.Collections.Immutable;
using Jakar.Json.Generator.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Jakar.Json.Generator.Tests.GeneratorHarness;



namespace Jakar.Json.Generator.Tests;


[TestFixture]
public sealed class CodeFixTests
{
    private static (AdhocWorkspace Workspace, Project Project) CreateProject( params (string Name, string Source)[] documents )
    {
        AdhocWorkspace workspace = new();
        Project project = workspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "TestProject", "TestAssembly", LanguageNames.CSharp, compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable), parseOptions: TestParseOptions, metadataReferences: CreateCompilation().References));

        foreach ( ( string name, string source ) in documents ) { project = project.AddDocument(name, SourceText.From(source), filePath: name).Project; }

        workspace.TryApplyChanges(project.Solution);
        return ( workspace, workspace.CurrentSolution.GetProject(project.Id)! );
    }

    private static async Task<Solution> ApplyAsync( Project project, Diagnostic diagnostic )
    {
        // Generator diagnostics carry file-path locations (pipeline models can't hold syntax trees), so find the document by path.
        string   path     = diagnostic.Location.GetLineSpan().Path;
        Document document = project.Documents.Single(d => d.FilePath == path);

        List<CodeAction> actions = [];
        CodeFixContext   context = new(document, diagnostic, ( action, _ ) => actions.Add(action), CancellationToken.None);
        await new JsonCodeFixProvider().RegisterCodeFixesAsync(context);

        ImmutableArray<CodeActionOperation> operations = await actions.Single().GetOperationsAsync(CancellationToken.None);
        return operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
    }

    private static async Task<Diagnostic> GeneratorDiagnosticAsync( Project project, string id )
    {
        Compilation compilation = ( await project.GetCompilationAsync() )!;
        CreateDriver().RunGeneratorsAndUpdateCompilation(compilation, out _, out ImmutableArray<Diagnostic> diagnostics);
        return diagnostics.First(d => d.Id == id);
    }

    private static async Task<string> TextAsync( Solution solution, string name ) => ( await solution.Projects.Single().Documents.Single(d => d.Name == name).GetTextAsync() ).ToString();


    [Test] public async Task JJSON001_MakesTheTypePartial()
    {
        ( AdhocWorkspace workspace, Project project ) = CreateProject(( "A.cs", PRELUDE + "[GenerateJson]\npublic sealed class A { public int Value { get; set; } }" ));
        using AdhocWorkspace dispose = workspace;

        Solution fixedSolution = await ApplyAsync(project, await GeneratorDiagnosticAsync(project, "JJSON001"));
        string   text          = await TextAsync(fixedSolution, "A.cs");

        Assert.That(text, Does.Contain("public sealed partial class A"));

        Compilation compilation = ( await fixedSolution.Projects.Single().GetCompilationAsync() )!;
        CreateDriver().RunGeneratorsAndUpdateCompilation(compilation, out Compilation output, out ImmutableArray<Diagnostic> diagnostics);
        Assert.That(diagnostics.Select(static d => d.Id),                                              Does.Not.Contain("JJSON001"));
        Assert.That(output.GetDiagnostics().Where(static d => d.Severity == DiagnosticSeverity.Error), Is.Empty);
    }

    [Test] public async Task JJSON001_MakesTheContainingTypePartial()
    {
        ( AdhocWorkspace workspace, Project project ) = CreateProject(( "A.cs", PRELUDE + "public static class Outer { [GenerateJson] public sealed partial class Inner { } }" ));
        using AdhocWorkspace dispose = workspace;

        Solution fixedSolution = await ApplyAsync(project, await GeneratorDiagnosticAsync(project, "JJSON001"));
        Assert.That(await TextAsync(fixedSolution, "A.cs"), Does.Contain("public static partial class Outer"));
    }

    [Test] public async Task JJSON006_FreezesTheOrder()
    {
        ( AdhocWorkspace workspace, Project project ) = CreateProject(( "A1.cs", PRELUDE + "[GenerateJson] public sealed partial class A { public int One { get; set; } [JsonMember(Name = \"z\")] public int Zed { get; set; } }" ), ( "A2.cs", PRELUDE + "public sealed partial class A { public int Two { get; set; } }" ));

        using AdhocWorkspace dispose = workspace;

        Solution fixedSolution = await ApplyAsync(project, await GeneratorDiagnosticAsync(project, "JJSON006"));
        string   first         = await TextAsync(fixedSolution, "A1.cs");
        string   second        = await TextAsync(fixedSolution, "A2.cs");

        Assert.That(first,  Does.Contain("[JsonMember(Order = 0)]"));
        Assert.That(first,  Does.Contain("[JsonMember(Name = \"z\", Order = 1)]"), "an existing [JsonMember] gets the argument");
        Assert.That(second, Does.Contain("[JsonMember(Order = 2)]"));
    }

    [Test] public async Task JJSON015_AddsTheAttribute()
    {
        ( AdhocWorkspace workspace, Project project ) = CreateProject(( "A.cs", PRELUDE + "public sealed class A : IJsonSerializable<A> { public int Value { get; set; } }" ));
        using AdhocWorkspace dispose = workspace;

        Compilation                compilation = ( await project.GetCompilationAsync() )!;
        ImmutableArray<Diagnostic> found       = await compilation.WithAnalyzers([new JsonAnalyzer()]).GetAnalyzerDiagnosticsAsync();

        Solution fixedSolution = await ApplyAsync(project, found.Single(static d => d.Id == "JJSON015"));
        string   text          = await TextAsync(fixedSolution, "A.cs");

        Assert.That(text, Does.Contain("[GenerateJson]"));
        Assert.That(text, Does.Contain("public sealed partial class A"));

        Compilation fixedCompilation = ( await fixedSolution.Projects.Single().GetCompilationAsync() )!;
        CreateDriver().RunGeneratorsAndUpdateCompilation(fixedCompilation, out Compilation output, out _);
        Assert.That(output.GetDiagnostics().Where(static d => d.Severity == DiagnosticSeverity.Error), Is.Empty, "the generated members now implement the interface");
    }
}
