// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:04 PM

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Jakar.Database;


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



/*
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
*/
