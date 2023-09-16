// Jakar.Extensions :: Jakar.Database
// 09/16/2023  12:08 PM

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Database;


internal record DbRecordClassDescription
{
    private readonly ClassDeclarationSyntax   _declaration;
    private readonly ImmutableArray<Property> _properties;
    private readonly Method                   _method;
    private readonly string                   _className;
    internal         SyntaxTree               MethodTree => _method.Declaration.SyntaxTree;
    internal         string                   MethodName => _method.Name;


    private DbRecordClassDescription( ClassDeclarationSyntax declaration, Method method ) : this( declaration, method, Property.Create( declaration ) ) { }
    private DbRecordClassDescription( ClassDeclarationSyntax declaration, Method method, ImmutableArray<Property> properties )
    {
        _declaration = declaration;
        _method      = method;
        _properties  = properties;
        _className   = declaration.Identifier.ValueText;
    }
    public static bool TryCreate( ClassDeclarationSyntax declaration, [ NotNullWhen( true ) ] out DbRecordClassDescription? result )
    {
        result = TryCreate( declaration );
        return result is not null;
    }
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static DbRecordClassDescription? TryCreate( ClassDeclarationSyntax declaration )
    {
        return Method.TryCreate( declaration, out Method? method )
                   ? new DbRecordClassDescription( declaration, method.Value )
                   : default;
    }


    public void Emit( in SourceProductionContext context ) => Emit( context, context.CancellationToken );
    public void Emit( in SourceProductionContext context, CancellationToken token )
    {
        using var result = new ValueStringBuilder();

        foreach ( Property property in _properties )
        {
            // token.ThrowIfCancellationRequested();
            if ( token.IsCancellationRequested ) { return; }
        }

        if ( token.IsCancellationRequested ) { return; }

        context.AddSource( $"{_className}.g.cs", SourceText.From( result.ToString(), Encoding.UTF8 ) );
    }



    public readonly record struct Property( PropertyDeclarationSyntax Declaration, string Name, string Variable )
    {
        public static Property Create( PropertyDeclarationSyntax declaration )
        {
            ArgumentNullException.ThrowIfNull( declaration );
            string name     = declaration.Identifier.ValueText;
            string variable = name.ToSnakeCase();
            return new Property( declaration, name, variable );
        }
        public static ImmutableArray<Property> Create( ClassDeclarationSyntax declaration )
        {
            ArgumentNullException.ThrowIfNull( declaration );

            return declaration.DescendantNodes()
                              .OfType<PropertyDeclarationSyntax>()
                              .Select( Create )
                              .ToImmutableArray();
        }
    }



    public readonly record struct Method( MethodDeclarationSyntax Declaration, string Name )
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static bool Check( SyntaxNode node ) => node is AttributeSyntax { Name: IdentifierNameSyntax { Identifier.Text: nameof(DbReaderMapping) } };


        public static Method Create( ClassDeclarationSyntax declaration ) => Create( (AttributeSyntax)declaration.DescendantNodes( Check )
                                                                                                                 .Single() );
        public static bool TryCreate( ClassDeclarationSyntax declaration, [ NotNullWhen( true ) ] out Method? result )
        {
            ImmutableArray<SyntaxNode> nodes = declaration.DescendantNodes( Check )
                                                          .ToImmutableArray();

            switch ( nodes.Length )
            {
                case > 1: throw new InvalidOperationException( $"{declaration.Identifier.ValueText} must only have exactly one method with {nameof(DbReaderMapping)}" );

                case 1 when nodes[0] is AttributeSyntax attribute:
                    result = Create( attribute );
                    return true;

                default:
                    result = default;
                    return false;
            }
        }


        public static bool TryCreate( SyntaxNode node, [ NotNullWhen( true ) ] out Method? result )
        {
            if ( node is not AttributeSyntax { Name: IdentifierNameSyntax { Identifier.Text: nameof(DbReaderMapping) } } attribute )
            {
                result = default;
                return false;
            }

            result = Create( attribute );
            return true;
        }
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
        public static Method Create( AttributeSyntax node )
        {
            ArgumentNullException.ThrowIfNull( node );
            Debug.Assert( Check( node ) );
            var    method = node.GetParent<MethodDeclarationSyntax>();
            string key    = method.Identifier.Text;
            return new Method( method, key );
        }
    }
}
