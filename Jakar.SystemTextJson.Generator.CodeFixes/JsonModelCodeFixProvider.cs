// Jakar.SystemTextJson.Generator.CodeFixes
// 10/02/2026

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;



namespace Jakar.SystemTextJson.Generator.CodeFixes;


/// <summary>
///     <list type="bullet">
///         <item> JAKAR_JSON001: adds <c> [JsonSerializable(typeof(T))] </c> to the type's context. </item>
///         <item> JAKAR_JSON006: adds <c> [JsonModel] </c> (with the context that registers the type, unless the assembly has a default). </item>
///     </list>
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(JsonModelCodeFixProvider))]
[Shared]
public sealed class JsonModelCodeFixProvider : CodeFixProvider
{
    // Duplicated from Jakar.SystemTextJson.Generator.JsonModelDiagnostics: this assembly doesn't reference the generator.
    internal const string NOT_REGISTERED        = "JAKAR_JSON001";
    internal const string MISSING_ATTRIBUTE     = "JAKAR_JSON006";
    internal const string CONTEXT_METADATA_NAME = "ContextMetadataName";
    internal const string TYPE_NAME             = "TypeName";

    private const string JSON_SERIALIZABLE    = "global::System.Text.Json.Serialization.JsonSerializable";
    private const string JSON_SERIALIZER_CTX  = "System.Text.Json.Serialization.JsonSerializerContext";
    private const string JSON_MODEL           = "global::Jakar.Extensions.JsonModel";
    private const string JSON_MODEL_CONTEXT   = "Jakar.Extensions.JsonModelContextAttribute";


    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [NOT_REGISTERED, MISSING_ATTRIBUTE];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


    public override async Task RegisterCodeFixesAsync( CodeFixContext context )
    {
        foreach ( Diagnostic diagnostic in context.Diagnostics )
        {
            switch ( diagnostic.Id )
            {
                case NOT_REGISTERED:
                    RegisterAddSerializable(context, diagnostic);
                    break;

                case MISSING_ATTRIBUTE:
                    await RegisterAddJsonModelAsync(context, diagnostic).ConfigureAwait(false);
                    break;
            }
        }
    }


    // ─── JAKAR_JSON001 ────────────────────────────────────────────────────────

    private static void RegisterAddSerializable( CodeFixContext context, Diagnostic diagnostic )
    {
        if ( !diagnostic.Properties.TryGetValue(CONTEXT_METADATA_NAME, out string? contextName) || string.IsNullOrEmpty(contextName) ) { return; }

        if ( !diagnostic.Properties.TryGetValue(TYPE_NAME, out string? typeName) || string.IsNullOrEmpty(typeName) ) { return; }

        string display = typeName!.Replace("global::", "");

        context.RegisterCodeFix(CodeAction.Create($"Add [JsonSerializable(typeof({display}))] to the context",
                                                  token => AddSerializableAsync(context.Document.Project.Solution, context.Document.Project.Id, contextName!, typeName, token),
                                                  $"{NOT_REGISTERED}:{contextName}:{typeName}"),
                                diagnostic);
    }


    internal static async Task<Solution> AddSerializableAsync( Solution solution, ProjectId projectId, string contextMetadataName, string typeName, CancellationToken token )
    {
        Project?     project     = solution.GetProject(projectId);
        Compilation? compilation = project is null ? null : await project.GetCompilationAsync(token).ConfigureAwait(false);
        if ( compilation?.GetTypeByMetadataName(contextMetadataName) is not { } contextType ) { return solution; }

        // Prefer the declaration that already carries [JsonSerializable] attributes.
        ClassDeclarationSyntax? target = null;

        foreach ( SyntaxReference reference in contextType.DeclaringSyntaxReferences )
        {
            if ( await reference.GetSyntaxAsync(token).ConfigureAwait(false) is not ClassDeclarationSyntax declaration ) { continue; }

            target ??= declaration;
            if ( declaration.AttributeLists.SelectMany(static list => list.Attributes).Any(IsJsonSerializable) ) { target = declaration; break; }
        }

        if ( target is null || solution.GetDocument(target.SyntaxTree) is not { } document ) { return solution; }

        AttributeListSyntax list = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.ParseName(JSON_SERIALIZABLE), SyntaxFactory.ParseAttributeArgumentList($"(typeof({typeName}))"))))
                                                .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);

        int index = target.AttributeLists.Count;

        for ( int i = target.AttributeLists.Count - 1; i >= 0; i-- )
        {
            if ( !target.AttributeLists[i].Attributes.Any(IsJsonSerializable) ) { continue; }

            index = i + 1;
            break;
        }

        SyntaxNode? root = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
        if ( root is null ) { return solution; }

        SyntaxNode updated = root.ReplaceNode(target, target.WithAttributeLists(target.AttributeLists.Insert(index, list)));
        Document   result  = document.WithSyntaxRoot(updated);
        result = await Simplifier.ReduceAsync(result, Simplifier.Annotation, cancellationToken: token).ConfigureAwait(false);
        result = await Formatter.FormatAsync(result, Formatter.Annotation, cancellationToken: token).ConfigureAwait(false);
        return result.Project.Solution;
    }


    private static bool IsJsonSerializable( AttributeSyntax attribute )
    {
        string name = attribute.Name.ToString();
        return name.EndsWith("JsonSerializable", System.StringComparison.Ordinal) || name.EndsWith("JsonSerializableAttribute", System.StringComparison.Ordinal);
    }


    // ─── JAKAR_JSON006 ────────────────────────────────────────────────────────

    private static async Task RegisterAddJsonModelAsync( CodeFixContext context, Diagnostic diagnostic )
    {
        SyntaxNode?    root  = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        SemanticModel? model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if ( root is null || model is null ) { return; }

        if ( root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } declaration ) { return; }

        if ( model.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } type ) { return; }

        bool hasDefault = model.Compilation.Assembly.GetAttributes().Any(static a => a.AttributeClass?.ToDisplayString() == JSON_MODEL_CONTEXT);

        if ( hasDefault )
        {
            context.RegisterCodeFix(CodeAction.Create("Add [JsonModel]", token => AddJsonModelAsync(context.Document, declaration, null, token), $"{MISSING_ATTRIBUTE}:default"), diagnostic);
            return;
        }

        List<INamedTypeSymbol> contexts = FindContextsRegistering(model.Compilation, type, context.CancellationToken);

        foreach ( INamedTypeSymbol contextType in contexts )
        {
            string name = contextType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            context.RegisterCodeFix(CodeAction.Create($"Add [JsonModel(typeof({contextType.Name}))]", token => AddJsonModelAsync(context.Document, declaration, name, token), $"{MISSING_ATTRIBUTE}:{name}"), diagnostic);
        }

        if ( contexts.Count == 0 ) { context.RegisterCodeFix(CodeAction.Create("Add [JsonModel] (then pass its JsonSerializerContext)", token => AddJsonModelAsync(context.Document, declaration, null, token), $"{MISSING_ATTRIBUTE}:none"), diagnostic); }
    }


    private static List<INamedTypeSymbol> FindContextsRegistering( Compilation compilation, INamedTypeSymbol type, CancellationToken token )
    {
        List<INamedTypeSymbol> results = [];

        foreach ( ISymbol symbol in compilation.GetSymbolsWithName(static _ => true, SymbolFilter.Type, token) )
        {
            if ( symbol is not INamedTypeSymbol candidate || !DerivesFrom(candidate, JSON_SERIALIZER_CTX) ) { continue; }

            foreach ( AttributeData attribute in candidate.GetAttributes() )
            {
                if ( attribute.AttributeClass?.Name != "JsonSerializableAttribute" || attribute.ConstructorArguments.Length != 1 ) { continue; }

                if ( !SymbolEqualityComparer.Default.Equals(attribute.ConstructorArguments[0].Value as ITypeSymbol, type) ) { continue; }

                results.Add(candidate);
                break;
            }
        }

        return results;
    }


    private static bool DerivesFrom( INamedTypeSymbol type, string metadataName )
    {
        for ( INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType )
        {
            if ( current.ToDisplayString() == metadataName ) { return true; }
        }

        return false;
    }


    internal static async Task<Document> AddJsonModelAsync( Document document, TypeDeclarationSyntax declaration, string? contextName, CancellationToken token )
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
        if ( root is null ) { return document; }

        AttributeSyntax attribute = contextName is null
                                        ? SyntaxFactory.Attribute(SyntaxFactory.ParseName(JSON_MODEL))
                                        : SyntaxFactory.Attribute(SyntaxFactory.ParseName(JSON_MODEL), SyntaxFactory.ParseAttributeArgumentList($"(typeof({contextName}))"));

        AttributeListSyntax list    = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute)).WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
        TypeDeclarationSyntax updated = declaration.WithAttributeLists(declaration.AttributeLists.Add(list));

        // [JsonModel] needs a partial type.
        if ( !updated.Modifiers.Any(SyntaxKind.PartialKeyword) ) { updated = updated.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space)); }

        Document result = document.WithSyntaxRoot(root.ReplaceNode(declaration, updated));
        result = await Simplifier.ReduceAsync(result, Simplifier.Annotation, cancellationToken: token).ConfigureAwait(false);
        return await Formatter.FormatAsync(result, Formatter.Annotation, cancellationToken: token).ConfigureAwait(false);
    }
}
