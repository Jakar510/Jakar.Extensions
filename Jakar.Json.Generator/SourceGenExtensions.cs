// Jakar.Extensions :: Jakar.Json.Generator
// 04/26/2022  1:54 PM

#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Jakar.Json.Generator;


public static class SourceGenExtensions
{
    private static bool IsPartial( this MemberDeclarationSyntax syntax ) => syntax.Modifiers.Any( m => m.IsKind( SyntaxKind.PartialKeyword ) );
    private static bool IsPublic( this  MemberDeclarationSyntax syntax ) => syntax.Modifiers.Any( m => m.IsKind( SyntaxKind.PublicKeyword ) );
    public static bool IsRecordStruct( this RecordDeclarationSyntax record )
    {
        if ( record.IsKind( SyntaxKind.RecordStructDeclaration ) ) { return true; }

        return record.Modifiers.Any( m => m.IsKind( SyntaxKind.RecordStructDeclaration ) );
    }
    public static bool IsSyntaxTarget( this SyntaxNode                syntax ) => syntax is RecordDeclarationSyntax r && r.IsPartial() && r.IsPublic() && r.IsRecordStruct();
    public static void Finalize( this       GeneratorExecutionContext context, in IMethodSymbol method, in string source ) => context.AddSource( $"{method.ContainingType.Name}.g.cs", source );
}
