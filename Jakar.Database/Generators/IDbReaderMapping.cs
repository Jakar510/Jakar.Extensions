// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:04 PM

using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;



namespace Jakar.Database;


[ AttributeUsage( AttributeTargets.Method ) ] public sealed class DbReaderMapping : Attribute { }



[ Generator ]
public sealed class DbReaderMappingGenerator : IIncrementalGenerator
{
    public void Initialize( GeneratorInitializationContext generator ) => generator.RegisterForSyntaxNotifications( MainSyntaxReceiver.Create );
    public void Execute( GeneratorExecutionContext generator )
    {
        if ( generator.SyntaxReceiver is not MainSyntaxReceiver main ) { return; }

        foreach ( MainSyntaxReceiver.Capture capture in main.Captures ) { capture.Execute( ref generator ); }
    }
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> declarations = context.SyntaxProvider.CreateSyntaxProvider( IsSyntaxTargetForGeneration, GetSemanticTargetForGeneration )
                                                                                               .Where( static m => m is not null )
                                                                                               .Collect()!;

        IncrementalValueProvider<(Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right)> values = context.CompilationProvider.Combine( declarations );
        context.RegisterSourceOutput( values, ExecuteHandler );
    }
    private static void ExecuteHandler( SourceProductionContext spc,  (Compilation Compilation, ImmutableArray<ClassDeclarationSyntax> Declaration) source ) { Execute( source.Compilation, source.Declaration, spc ); }
    private static bool IsSyntaxTargetForGeneration( SyntaxNode node, CancellationToken                                                             token ) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };
    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration( GeneratorSyntaxContext context, CancellationToken token )
    {
        // we know the node is a MethodDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

        // loop through all the attributes on the method
        foreach ( AttributeListSyntax attributeListSyntax in methodDeclarationSyntax.AttributeLists )
        {
            foreach ( AttributeSyntax attributeSyntax in attributeListSyntax.Attributes )
            {
                if ( context.SemanticModel.GetSymbolInfo( attributeSyntax )
                            .Symbol is not IMethodSymbol attributeSymbol )
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string           fullName                      = attributeContainingTypeSymbol.ToDisplayString();

                if ( fullName == nameof(LoggerMessageAttribute) ) // Is the attribute the [LoggerMessage] attribute?
                {
                    return methodDeclarationSyntax.Parent as ClassDeclarationSyntax; // return the parent class of the method
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }


    // https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/
    private static void Execute( Compilation compilation, ImmutableArray<ClassDeclarationSyntax> array, SourceProductionContext context )
    {
        if ( array.IsDefaultOrEmpty ) { return; }

        IEnumerable<ClassDeclarationSyntax> distinctClasses = array.Distinct();
        var                                 p               = new Parser( compilation, context.ReportDiagnostic, context.CancellationToken );

        IReadOnlyList<TableClass> logClasses = p.GetLogClasses( distinctClasses );
        if ( logClasses.Count <= 0 ) { return; }

        var    e      = new Emitter();
        string result = e.Emit( logClasses, context.CancellationToken );
        context.AddSource( "LoggerMessage.g.cs", SourceText.From( result, Encoding.UTF8 ) );
    }
}



internal class Parser
{
    private readonly Compilation        _compilation;
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly CancellationToken  _token;
    public Parser( Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken token )
    {
        _compilation      = compilation;
        _reportDiagnostic = reportDiagnostic;
        _token            = token;
    }
    public IReadOnlyList<TableClass> GetLogClasses( IEnumerable<ClassDeclarationSyntax> distinctClasses ) { return Array.Empty<TableClass>(); }
}



internal class TableClass { }



internal class Emitter
{
    public string Emit( IReadOnlyList<TableClass> logClasses, CancellationToken token ) { return null; }
}



public sealed class MainSyntaxReceiver : ISyntaxReceiver
{
    public List<Capture> Captures { get; } = new();
    public static MainSyntaxReceiver Create() => new();
    public void OnVisitSyntaxNode( SyntaxNode syntaxNode )
    {
        if ( syntaxNode is not AttributeSyntax { Name: IdentifierNameSyntax { Identifier.Text: nameof(DbReaderMapping) } } attribute ) { return; }

        var    clazz  = attribute.GetParent<ClassDeclarationSyntax>();
        var    method = attribute.GetParent<MethodDeclarationSyntax>();
        string key    = method.Identifier.Text;
        Captures.Add( new Capture( key, method, clazz ) );
    }



    public sealed record Capture( string Key, MethodDeclarationSyntax Method, ClassDeclarationSyntax Class )
    {
        private const string READER = "reader";
        public void Execute( ref GeneratorExecutionContext generator )
        {
            ClassDeclarationSyntax output = Class.WithMembers( new SyntaxList<MemberDeclarationSyntax>( CreateMethod() ) )
                                                 .NormalizeWhitespace();

            generator.AddSource( $"{Class.Identifier.Text}.g.cs", output.GetText( Encoding.UTF8 ) );
        }
        private MemberDeclarationSyntax CreateMethod()
        {
            var statements = new List<StatementSyntax>();

            PropertyDeclarationSyntax[] properties = Class.DescendantNodes()
                                                          .OfType<PropertyDeclarationSyntax>()
                                                          .ToArray();

            foreach ( PropertyDeclarationSyntax property in properties )
            {
                string name     = property.Identifier.ValueText;
                string variable = name.ToSnakeCase();

                LocalDeclarationStatementSyntax statement = LocalDeclarationStatement( VariableDeclaration( PredefinedType( Token( SyntaxKind.StringKeyword ) ) )
                                                                                          .WithVariables( SingletonSeparatedList( VariableDeclarator( Identifier( variable ) )
                                                                                                                                     .WithInitializer( EqualsValueClause( GetFromReader( property.Type, name ) ) ) ) ) );


                // var getFieldValue = InvocationExpression( MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression,
                //                                                                   IdentifierName( READER ),
                //                                                                   GenericName( Identifier( nameof(DbDataReader.GetFieldValue) ) )
                //                                                                      .WithTypeArgumentList( TypeArgumentList( SingletonSeparatedList<TypeSyntax>( IdentifierName( "Guid" ) ) ) ) ) )
                //    .WithArgumentList( ArgumentList( SingletonSeparatedList( Argument( InvocationExpression( IdentifierName( Identifier( TriviaList(), SyntaxKind.NameOfKeyword, "nameof", "nameof", TriviaList() ) ) )
                //                                                                          .WithArgumentList( ArgumentList( SingletonSeparatedList( Argument( IdentifierName( "CreatedBy" ) ) ) ) ) ) ) ) );
                //
                // var create = ObjectCreationExpression( GenericName( Identifier( "RecordID" ) )
                //                                           .WithTypeArgumentList( TypeArgumentList( SingletonSeparatedList<TypeSyntax>( IdentifierName( "UserRecord" ) ) ) ) )
                //    .WithArgumentList( ArgumentList( SingletonSeparatedList( Argument( getFieldValue ) ) ) );
                //
                // var statement = LocalDeclarationStatement( VariableDeclaration( GenericName( Identifier( "RecordID" ) )
                //                                                                    .WithTypeArgumentList( TypeArgumentList( SingletonSeparatedList<TypeSyntax>( IdentifierName( "UserRecord" ) ) ) ) )
                //                                               .WithVariables( SingletonSeparatedList( VariableDeclarator( Identifier( "createdBy" ) )
                //                                                                                          .WithInitializer( EqualsValueClause( create ) ) ) ) );
            }

            return MethodDeclaration( Method.ReturnType, Method.Identifier )
                  .WithModifiers( Method.Modifiers )
                  .WithParameterList( Method.ParameterList )
                  .WithParameterList( ParameterList( SingletonSeparatedList( Parameter( Identifier( READER ) )
                                                                                .WithType( IdentifierName( nameof(DbDataReader) ) ) ) ) )
                  .WithBody( Block( statements ) );
        }

        private static InvocationExpressionSyntax GetFromReader( TypeSyntax syntax, in string name )
        {
            if ( syntax.IsUnmanaged ) { return GetFromReader_ValueType( syntax.ToString(), name ); }

            if ( syntax is GenericNameSyntax generic )
            {
                string arg = generic.TypeArgumentList.Arguments.Single()
                                    .ToString();

                return GetFromReader_ValueType( arg, name );
            }

            string type = syntax.ToString();

        #pragma warning disable IDE0066
            switch ( type )
            {
                case "string":
                case nameof(String):
                    return GetFromReader_String( name );

                case nameof(Double):
                case nameof(TimeSpan):
                case nameof(TimeOnly):
                case nameof(DateOnly):
                case nameof(DateTime):
                case nameof(DateTimeOffset):
                    return GetFromReader_ValueType( type, name );

                default: throw new OutOfRangeException( type, nameof(type) );
            }
        #pragma warning restore IDE0066
        }
        private static ObjectCreationExpressionSyntax GetFromReader_Object( in GenericNameSyntax syntax, in string name )
        {
            SingletonSeparatedList( Argument( InvocationExpression( MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression,
                                                                                            IdentifierName( READER ),
                                                                                            GenericName( Identifier( nameof(DbDataReader.GetFieldValue) ) )
                                                                                               .WithTypeArgumentList( TypeArgumentList( SingletonSeparatedList<TypeSyntax>( IdentifierName( "Guid" ) ) ) ) ) )
                                                 .WithArgumentList( ArgumentList( SingletonSeparatedList( Argument( LiteralExpression( SyntaxKind.StringLiteralExpression, Literal( "ID" ) ) ) ) ) ) ) );

            return ObjectCreationExpression( GenericName( Identifier( syntax.ToString() ) )
                                                .WithTypeArgumentList( TypeArgumentList( syntax.TypeArgumentList.Arguments ) ) )
               .WithArgumentList( ArgumentList() );
        }
        private static InvocationExpressionSyntax GetFromReader_String( in string name ) =>
            InvocationExpression( MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression, IdentifierName( READER ), IdentifierName( nameof(DbDataReader.GetString) ) ) )
               .WithArgumentList( ArgumentList( SingletonSeparatedList( Argument( LiteralExpression( SyntaxKind.StringLiteralExpression, Literal( name ) ) ) ) ) );
        private static InvocationExpressionSyntax GetFromReader_ValueType( in string type, in string name ) =>
            InvocationExpression( MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression,
                                                          IdentifierName( READER ),
                                                          GenericName( Identifier( nameof(DbDataReader.GetFieldValue) ) )
                                                             .WithTypeArgumentList( TypeArgumentList( SingletonSeparatedList<TypeSyntax>( IdentifierName( type ) ) ) ) ) );
    }
}
