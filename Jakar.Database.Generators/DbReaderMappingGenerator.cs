// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:04 PM

namespace Jakar.Database.Generators;


[ AttributeUsage( AttributeTargets.Method ) ] public sealed class DbReaderMapping : Attribute { }



[ Generator ]
public sealed class DbReaderMappingGenerator : IIncrementalGenerator
{
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        context.RegisterSourceOutput( context.CompilationProvider.Combine( context.SyntaxProvider.CreateSyntaxProvider( IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration )
                                                                                  .Where( static m => m is not null )
                                                                                  .Collect() ),
                                      ExecuteHandler );
    }
    private static void ExecuteHandler( SourceProductionContext context, (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax?> Declaration) source ) => Execute( source.Compilation, source.Declaration, context );


    private static bool IsSyntaxTargetForGeneration( SyntaxNode node, CancellationToken token ) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };


    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration( GeneratorSyntaxContext context, CancellationToken token )
    {
        if ( context.Node is not MethodDeclarationSyntax declaration ) { throw new ExpectedValueTypeException( $"{nameof(context)}.{nameof(GeneratorSyntaxContext.Node)}", context.Node, typeof(MethodDeclarationSyntax) ); }

        foreach ( AttributeSyntax attributeSyntax in declaration.AttributeLists.SelectMany( attributeListSyntax => attributeListSyntax.Attributes ) )
        {
            if ( context.SemanticModel.GetSymbolInfo( attributeSyntax, token )
                        .Symbol is IMethodSymbol attributeSymbol &&
                 attributeSymbol.ContainingType.ToDisplayString() == nameof(DbReaderMapping) ) // Is the attribute the [DbReaderMapping] attribute?
            {
                return declaration.Parent as ClassDeclarationSyntax; // return the parent class of the method
            }
        }

        return default;
    }


    // https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/
    private static void Execute( Compilation compilation, ImmutableArray<ClassDeclarationSyntax?> array, SourceProductionContext context )
    {
        if ( array.IsDefaultOrEmpty ) { return; }

        ImmutableArray<DbRecordClassDescription> dbRecords = GetClasses( context, array );

        if ( dbRecords.Length <= 0 ) { return; }

        foreach ( DbRecordClassDescription description in dbRecords ) { description.Emit( context ); }
    }

    private static ImmutableArray<DbRecordClassDescription> GetClasses( in SourceProductionContext context, ImmutableArray<ClassDeclarationSyntax?> declarations )
    {
        var list = new List<DbRecordClassDescription>();

        foreach ( ClassDeclarationSyntax? declaration in declarations )
        {
            if ( context.CancellationToken.IsCancellationRequested ) { return default; }

            if ( DbRecordClassDescription.TryCreate( declaration, out DbRecordClassDescription? result ) is false ) { continue; }

            context.ReportDiagnostic( Found( declaration, result ) );
            list.Add( result );
        }

        return list.ToImmutableArray();
    }
    private static Diagnostic Found( BaseTypeDeclarationSyntax declaration, DbRecordClassDescription description )
    {
        const string ID         = $"{nameof(DbReaderMappingGenerator)}.{nameof(Found)}";
        var          descriptor = new DiagnosticDescriptor( ID, $"Found method '{description.MethodName}' in {declaration.Identifier.ValueText}", string.Empty, nameof(DbReaderMappingGenerator), DiagnosticSeverity.Info, true );
        var          location   = Location.Create( declaration.SyntaxTree, default );

        return Diagnostic.Create( descriptor, location );
    }
}