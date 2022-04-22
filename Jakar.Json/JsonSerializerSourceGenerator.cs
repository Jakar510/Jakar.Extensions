using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Jakar.Json;


public interface IJson
{
    public string ToJson();
}



[Generator]
public class JsonSerializerSourceGenerator : ISourceGenerator
{
    public const string FROM_JSON = "FromJson";
    public const string AUTO      = " // Auto-generated code";


    public void Execute( GeneratorExecutionContext context )
    {
        IMethodSymbol? mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
    }
    public void Execute( in Compilation compilation )
    {
        IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
    }
    private static bool IsPartial( MemberDeclarationSyntax syntax ) => syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    private static bool IsPublic( MemberDeclarationSyntax  syntax ) => syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
    public static bool IsRecordStruct( RecordDeclarationSyntax record )
    {
        if ( record.IsKind(SyntaxKind.RecordStructDeclaration) ) { return true; }

        return record.Modifiers.Any(m => m.IsKind(SyntaxKind.RecordStructDeclaration));
    }
    private static bool IsSyntaxTarget( SyntaxNode syntax ) => syntax is RecordDeclarationSyntax r && r.IsPartial() && r.IsPublic() && r.IsRecordStruct();


    private void Finalize( in GeneratorExecutionContext context, in IMethodSymbol method, in string source ) => context.AddSource($"{method.ContainingType.Name}.g.cs", source);

    public void Initialize( GeneratorInitializationContext context )
    {
        // No initialization required for this one
    }
}
