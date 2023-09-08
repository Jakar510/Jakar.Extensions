/*
// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:04 PM

using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZXing.Common.ReedSolomon;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;



namespace Jakar.Database;


[AttributeUsage( AttributeTargets.Method )] public sealed class DbReaderMapping : Attribute { }



[Generator]
public sealed class DbReaderMappingGenerator : ISourceGenerator
{
    public void Initialize( GeneratorInitializationContext generator ) => generator.RegisterForSyntaxNotifications( MainSyntaxReceiver.Create );
    public void Execute( GeneratorExecutionContext generator )
    {
        if ( generator.SyntaxReceiver is not MainSyntaxReceiver main ) { return; }

        foreach ( MainSyntaxReceiver.Capture capture in main.Captures ) { capture.Execute( ref generator ); }
    }
}



public sealed class MainSyntaxReceiver : ISyntaxReceiver
{
    public static MainSyntaxReceiver Create() => new();


    public List<Capture> Captures { get; } = new();
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

                var statement = LocalDeclarationStatement( VariableDeclaration( PredefinedType( Token( SyntaxKind.StringKeyword ) ) )
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
                var arg = generic.TypeArgumentList.Arguments.Single()
                                 .ToString();

                return GetFromReader_ValueType( arg, name );
            }

            var type = syntax.ToString();

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
        private static InvocationExpressionSyntax GetFromReader_String( in string name )
        {
            return InvocationExpression( MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression, IdentifierName( READER ), IdentifierName( nameof(DbDataReader.GetString) ) ) )
               .WithArgumentList( ArgumentList( SingletonSeparatedList( Argument( LiteralExpression( SyntaxKind.StringLiteralExpression, Literal( name ) ) ) ) ) );
        }
        private static InvocationExpressionSyntax GetFromReader_ValueType( in string type, in string name )
        {
            return InvocationExpression( MemberAccessExpression( SyntaxKind.SimpleMemberAccessExpression,
                                                                 IdentifierName( READER ),
                                                                 GenericName( Identifier( nameof(DbDataReader.GetFieldValue) ) )
                                                                    .WithTypeArgumentList( TypeArgumentList( SingletonSeparatedList<TypeSyntax>( IdentifierName( type ) ) ) ) ) );
        }
    }
}
*/
