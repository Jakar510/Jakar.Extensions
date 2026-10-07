// Jakar.SystemTextJson.Generator.Tests
// 10/02/2026

using System.Collections.Immutable;
using Jakar.SystemTextJson.Generator.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Jakar.SystemTextJson.Generator.Tests.GeneratorHarness;



namespace Jakar.SystemTextJson.Generator.Tests;


[TestFixture]
public sealed class CodeFix_Tests
{
    private static (AdhocWorkspace Workspace, Project Project) CreateProject( params (string Name, string Source)[] documents )
    {
        AdhocWorkspace workspace = new();
        Project project = workspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(),
                                                                  VersionStamp.Create(),
                                                                  "TestProject",
                                                                  "TestAssembly",
                                                                  LanguageNames.CSharp,
                                                                  compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable),
                                                                  parseOptions: TestParseOptions,
                                                                  metadataReferences: CreateCompilation().References));

        foreach ( (string name, string source) in documents ) { project = project.AddDocument(name, SourceText.From(source), filePath: name).Project; }

        workspace.TryApplyChanges(project.Solution);
        return ( workspace, workspace.CurrentSolution.GetProject(project.Id)! );
    }


    private static async Task<Solution> ApplyAsync( Project project, Diagnostic diagnostic, string? titleContains = null )
    {
        // Generator diagnostics carry file-path locations (pipeline models can't hold syntax trees), so find the document by path.
        string   path     = diagnostic.Location.GetLineSpan().Path;
        Document document = project.Documents.Single(d => d.FilePath == path);

        List<CodeAction> actions = [];
        CodeFixContext   context = new(document, diagnostic, ( action, _ ) => actions.Add(action), CancellationToken.None);
        await new JsonModelCodeFixProvider().RegisterCodeFixesAsync(context);

        CodeAction action = titleContains is null
                                ? actions.Single()
                                : actions.Single(a => a.Title.Contains(titleContains));

        ImmutableArray<CodeActionOperation> operations = await action.GetOperationsAsync(CancellationToken.None);
        return operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
    }


    [Test]
    public async Task JAKAR_JSON001_AddsTheRegistration()
    {
        const string model = PRELUDE + """
                                       namespace App;
                                       [JsonModel(typeof(AppContext))]
                                       public partial class Invoice;
                                       """;

        const string context = PRELUDE + """
                                         namespace App;
                                         [JsonSerializable(typeof(string))]
                                         public sealed partial class AppContext : JsonSerializerContext;
                                         """;

        (AdhocWorkspace workspace, Project project) = CreateProject(("Invoice.cs", model), ("AppContext.cs", context));
        using AdhocWorkspace disposeWorkspace = workspace;

        Compilation compilation = ( await project.GetCompilationAsync() )!;
        CreateDriver().RunGeneratorsAndUpdateCompilation(compilation, out _, out ImmutableArray<Diagnostic> diagnostics);
        Diagnostic diagnostic = diagnostics.Single(static d => d.Id == "JAKAR_JSON001");

        Solution fixedSolution = await ApplyAsync(project, diagnostic);
        string   text          = ( await fixedSolution.GetProject(project.Id)!.Documents.Single(static d => d.Name == "AppContext.cs").GetTextAsync() ).ToString();

        Assert.That(text, Does.Contain("[JsonSerializable(typeof(string))]"));
        Assert.That(text, Does.Contain("[JsonSerializable(typeof(Invoice))]"));
        Assert.That(text.IndexOf("typeof(Invoice)", StringComparison.Ordinal), Is.GreaterThan(text.IndexOf("typeof(string)", StringComparison.Ordinal)), "added after the existing registrations");
    }


    [Test]
    public async Task JAKAR_JSON006_AddsJsonModelWithTheRegisteringContext()
    {
        string source = PRELUDE + """
                                  namespace App;
                                  public class Invoice : IJsonModel<Invoice>;
                                  """ + "namespace App { " + Context("AppContext", ("Invoice", "Invoice", null)) + " }";

        (AdhocWorkspace workspace, Project project) = CreateProject(("Invoice.cs", source.Replace("namespace App;", "")));
        using AdhocWorkspace disposeWorkspace = workspace;

        Compilation                compilation = ( await project.GetCompilationAsync() )!;
        ImmutableArray<Diagnostic> diagnostics = await compilation.WithAnalyzers([new JsonModelAnalyzer()]).GetAnalyzerDiagnosticsAsync();
        Diagnostic                 diagnostic  = diagnostics.Single(static d => d.Id == "JAKAR_JSON006");

        Solution fixedSolution = await ApplyAsync(project, diagnostic, "AppContext");
        string   text          = ( await fixedSolution.GetProject(project.Id)!.Documents.Single().GetTextAsync() ).ToString();

        Assert.That(text, Does.Contain("[JsonModel(typeof(App.AppContext))]").Or.Contain("[JsonModel(typeof(AppContext))]"));
        Assert.That(text, Does.Contain("public partial class Invoice"));
    }
}
