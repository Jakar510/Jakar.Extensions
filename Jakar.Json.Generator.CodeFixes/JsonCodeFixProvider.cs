// Jakar.Json.Generator.CodeFixes
// 10/07/2026

using System;
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



namespace Jakar.Json.Generator.CodeFixes;


/// <summary>
///     <list type="bullet">
///         <item> JJSON001: makes the type (or the containing type the diagnostic names) <see langword="partial"/>. </item>
///         <item> JJSON006: freezes the current member order with <c>[JsonMember(Order = n)]</c>. </item>
///         <item> JJSON015: adds <c>[GenerateJson]</c>. </item>
///     </list>
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(JsonCodeFixProvider))]
[Shared]
public sealed class JsonCodeFixProvider : CodeFixProvider
{
    // Duplicated from Jakar.Json.Generator.JsonDiagnostics: this assembly doesn't reference the generator.
    internal const string NOT_PARTIAL       = "JJSON001";
    internal const string ORDER_AMBIGUOUS   = "JJSON006";
    internal const string MISSING_ATTRIBUTE = "JJSON015";
    internal const string MEMBERS           = "Members";

    private const string GENERATE_JSON = "global::Jakar.Json.GenerateJson";
    private const string JSON_MEMBER   = "global::Jakar.Json.JsonMember";


    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [NOT_PARTIAL, ORDER_AMBIGUOUS, MISSING_ATTRIBUTE];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


    public override async Task RegisterCodeFixesAsync( CodeFixContext context )
    {
        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if ( root is null ) { return; }

        foreach ( Diagnostic diagnostic in context.Diagnostics )
        {
            if ( root.FindToken(diagnostic.Location.SourceSpan.Start).Parent?.FirstAncestorOrSelf<TypeDeclarationSyntax>() is not { } declaration ) { continue; }

            switch ( diagnostic.Id )
            {
                case NOT_PARTIAL:
                    string name = diagnostic.GetMessage().Split('\'').ElementAtOrDefault(1) ?? declaration.Identifier.Text;
                    context.RegisterCodeFix(CodeAction.Create($"Make '{name}' partial", token => MakePartialAsync(context.Document, declaration, name, token), $"{NOT_PARTIAL}:{name}"), diagnostic);
                    break;

                case ORDER_AMBIGUOUS:
                    if ( !diagnostic.Properties.TryGetValue(MEMBERS, out string? members) || string.IsNullOrEmpty(members) ) { continue; }

                    context.RegisterCodeFix(CodeAction.Create("Freeze the member order with [JsonMember(Order = n)]", token => AddOrdersAsync(context.Document, declaration, members!.Split(','), token), ORDER_AMBIGUOUS), diagnostic);
                    break;

                case MISSING_ATTRIBUTE:
                    context.RegisterCodeFix(CodeAction.Create("Add [GenerateJson]", token => AddGenerateJsonAsync(context.Document, declaration, token), MISSING_ATTRIBUTE), diagnostic);
                    break;
            }
        }
    }


    // ─── JJSON001 ────────────────────────────────────────────────────────────

    /// <summary> Adds <see langword="partial"/> to every declaration of the type named <paramref name="name"/> (the declaration or one of its containers), in every document. </summary>
    internal static async Task<Solution> MakePartialAsync( Document document, TypeDeclarationSyntax declaration, string name, CancellationToken token )
    {
        SemanticModel? model = await document.GetSemanticModelAsync(token).ConfigureAwait(false);
        if ( model?.GetDeclaredSymbol(declaration, token) is not INamedTypeSymbol type ) { return document.Project.Solution; }

        INamedTypeSymbol? target = null;

        for ( INamedTypeSymbol? current = type; current is not null; current = current.ContainingType )
        {
            if ( current.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) != name && current.Name != name ) { continue; }

            target = current;
            break;
        }

        target ??= type;
        Solution solution = document.Project.Solution;

        foreach ( IGrouping<SyntaxTree, SyntaxReference> group in target.DeclaringSyntaxReferences.GroupBy(static r => r.SyntaxTree) )
        {
            if ( solution.GetDocument(group.Key) is not { } owner ) { continue; }

            SyntaxNode                  root  = await group.Key.GetRootAsync(token).ConfigureAwait(false);
            List<TypeDeclarationSyntax> nodes = group.Select(r => r.GetSyntax(token)).OfType<TypeDeclarationSyntax>().Where(static d => !d.Modifiers.Any(SyntaxKind.PartialKeyword)).ToList();

            SyntaxNode updated = root.ReplaceNodes(nodes, static ( original, _ ) => original.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
            solution = solution.WithDocumentSyntaxRoot(owner.Id, updated);
        }

        return solution;
    }


    // ─── JJSON006 ────────────────────────────────────────────────────────────

    /// <summary> Numbers the members in their current output order (file path, then position), so moving or renaming a file can't reorder the JSON. </summary>
    internal static async Task<Solution> AddOrdersAsync( Document document, TypeDeclarationSyntax declaration, IReadOnlyList<string> names, CancellationToken token )
    {
        SemanticModel? model = await document.GetSemanticModelAsync(token).ConfigureAwait(false);
        if ( model?.GetDeclaredSymbol(declaration, token) is not INamedTypeSymbol type ) { return document.Project.Solution; }

        List<(SyntaxNode Node, string Path, int Position)> targets = [];

        foreach ( string name in names )
        {
            ISymbol? member = type.GetMembers(name).FirstOrDefault(static m => m is IPropertySymbol or IFieldSymbol);
            if ( member?.DeclaringSyntaxReferences.FirstOrDefault() is not { } reference ) { continue; }

            SyntaxNode node = await reference.GetSyntaxAsync(token).ConfigureAwait(false);
            if ( node is VariableDeclaratorSyntax { Parent.Parent: FieldDeclarationSyntax field } ) { node = field; }

            if ( node is MemberDeclarationSyntax ) { targets.Add(( node, reference.SyntaxTree.FilePath, node.SpanStart )); }
        }

        Solution                    solution = document.Project.Solution;
        int                         order    = 0;
        Dictionary<SyntaxNode, int> orders   = new();

        foreach ( ( SyntaxNode node, _, _ ) in targets.OrderBy(static t => t.Path, StringComparer.Ordinal).ThenBy(static t => t.Position) ) { orders[node] = order++; }

        foreach ( IGrouping<SyntaxTree, SyntaxNode> group in orders.Keys.GroupBy(static n => n.SyntaxTree) )
        {
            if ( solution.GetDocument(group.Key) is not { } owner ) { continue; }

            SyntaxNode root    = await group.Key.GetRootAsync(token).ConfigureAwait(false);
            SyntaxNode updated = root.ReplaceNodes(group, ( original, _ ) => WithOrder((MemberDeclarationSyntax)original, orders[original]));
            Document   result  = owner.WithSyntaxRoot(updated);
            result   = await Simplifier.ReduceAsync(result, Simplifier.Annotation, cancellationToken: token).ConfigureAwait(false);
            result   = await Formatter.FormatAsync(result, Formatter.Annotation, cancellationToken: token).ConfigureAwait(false);
            solution = result.Project.Solution;
        }

        return solution;
    }

    private static MemberDeclarationSyntax WithOrder( MemberDeclarationSyntax member, int order )
    {
        AttributeArgumentSyntax argument = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals("Order"), null, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(order)));

        // An existing [JsonMember(...)] gets the argument; otherwise a new attribute.
        foreach ( AttributeListSyntax list in member.AttributeLists )
        {
            foreach ( AttributeSyntax attribute in list.Attributes )
            {
                string name = attribute.Name.ToString();
                if ( !name.EndsWith("JsonMember", StringComparison.Ordinal) && !name.EndsWith("JsonMemberAttribute", StringComparison.Ordinal) ) { continue; }

                AttributeArgumentListSyntax arguments = attribute.ArgumentList ?? SyntaxFactory.AttributeArgumentList();
                return member.ReplaceNode(attribute, attribute.WithArgumentList(arguments.AddArguments(argument)));
            }
        }

        AttributeListSyntax added = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.ParseName(JSON_MEMBER), SyntaxFactory.AttributeArgumentList(SyntaxFactory.SingletonSeparatedList(argument))))).WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);

        return member.WithAttributeLists(member.AttributeLists.Insert(0, added));
    }


    // ─── JJSON015 ────────────────────────────────────────────────────────────

    internal static async Task<Solution> AddGenerateJsonAsync( Document document, TypeDeclarationSyntax declaration, CancellationToken token )
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
        if ( root is null ) { return document.Project.Solution; }

        AttributeListSyntax list = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.ParseName(GENERATE_JSON)))).WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);

        TypeDeclarationSyntax updated = declaration.WithAttributeLists(declaration.AttributeLists.Add(list));
        if ( !updated.Modifiers.Any(SyntaxKind.PartialKeyword) ) { updated = updated.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space)); }

        Document result = document.WithSyntaxRoot(root.ReplaceNode(declaration, updated));
        result = await Simplifier.ReduceAsync(result, Simplifier.Annotation, cancellationToken: token).ConfigureAwait(false);
        result = await Formatter.FormatAsync(result, Formatter.Annotation, cancellationToken: token).ConfigureAwait(false);
        return result.Project.Solution;
    }
}
