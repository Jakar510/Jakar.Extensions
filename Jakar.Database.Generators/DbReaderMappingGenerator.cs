// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:04 PM

namespace Jakar.Database.Generators;


/*
public interface IDbReaderMapping<out TRecord>
    where TRecord : IDbReaderMapping<TRecord>
{
    public abstract static string                    TableName { get; }
    public abstract static TRecord                   Create( DbDataReader      reader );
    public abstract static IAsyncEnumerable<TRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default );


    public DynamicParameters ToDynamicParameters();
}
*/

// [ AttributeUsage( AttributeTargets.Class ) ] public sealed class DbReaderMapping : Attribute { }



[Generator]
public sealed class DbReaderMappingGenerator : IIncrementalGenerator
{
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> provider = context.SyntaxProvider.CreateSyntaxProvider( IsSyntaxTargetForGeneration, Transform ).Where( static m => m is not null ).Collect();

        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> sources = context.CompilationProvider.Combine( provider );
        context.RegisterSourceOutput( sources, ExecuteHandler );
    }
    private static ClassDeclarationSyntax Transform( GeneratorSyntaxContext       x,       CancellationToken                                                             _ )      => (ClassDeclarationSyntax)x.Node;
    private static void                   ExecuteHandler( SourceProductionContext context, (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax> Declaration) source ) => Execute( source.Compilation, source.Declaration, context );


    private static bool IsSyntaxTargetForGeneration( SyntaxNode node, CancellationToken token )
    {
        if ( node is not ClassDeclarationSyntax syntax || syntax.BaseList is null ) { return false; }

        string className = syntax.Identifier.ValueText;

        foreach ( BaseTypeSyntax baseType in syntax.BaseList.Types )
        {
            if ( baseType.Type is GenericNameSyntax { Identifier.Text: "IDbReaderMapping", TypeArgumentList.Arguments: [IdentifierNameSyntax arg] } && arg.Identifier.ValueText == className ) { return true; }
        }

        return false;
    }


    // https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/
    private static void Execute( in Compilation compilation, in ImmutableArray<ClassDeclarationSyntax> array, in SourceProductionContext context )
    {
        if ( array.IsDefaultOrEmpty ) { return; }

        ImmutableArray<DbRecordClassDescription> dbRecords = GetClasses( context, array );

        if ( dbRecords.Length <= 0 ) { return; }

        foreach ( DbRecordClassDescription description in dbRecords ) { description.Emit( context ); }
    }


    private static ImmutableArray<DbRecordClassDescription> GetClasses( in SourceProductionContext context, in ImmutableArray<ClassDeclarationSyntax> declarations )
    {
        List<DbRecordClassDescription> list = new List<DbRecordClassDescription>( declarations.Length );

        foreach ( ClassDeclarationSyntax declaration in declarations )
        {
            if ( context.CancellationToken.IsCancellationRequested ) { return default; }

            DbRecordClassDescription result = DbRecordClassDescription.Create( declaration );
            context.ReportDiagnostic( Found( declaration, result ) );
            list.Add( result );
        }

        return list.ToImmutableArray();
    }
    private static Diagnostic Found( SyntaxNode declaration, DbRecordClassDescription description )
    {
        const string ID         = $"{nameof(DbReaderMappingGenerator)}.{nameof(Found)}";
        DiagnosticDescriptor          descriptor = new DiagnosticDescriptor( ID, $"Found Class '{description.ClassName}'", string.Empty, nameof(DbReaderMappingGenerator), DiagnosticSeverity.Info, true );
        Location          location   = Location.Create( declaration.SyntaxTree, default );

        return Diagnostic.Create( descriptor, location );
    }
}
