// Jakar.Extensions :: Jakar.Database
// 09/16/2023  12:08 PM

using System.Globalization;
using System.Text;



namespace Jakar.Database.Generators;


internal record DbRecordClassDescription
{
    public string                   ClassName   => Declaration.Identifier.ValueText;
    public ClassDeclarationSyntax   Declaration { get; }
    public ImmutableArray<Property> Properties  { get; }


    private DbRecordClassDescription( ClassDeclarationSyntax declaration ) : this( declaration, Property.Create( declaration ) ) { }
    private DbRecordClassDescription( ClassDeclarationSyntax declaration, ImmutableArray<Property> properties )
    {
        Declaration = declaration;
        Properties  = properties;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static DbRecordClassDescription Create( ClassDeclarationSyntax declaration ) => new(declaration);


    public void Emit( in SourceProductionContext context ) => Emit( context, context.CancellationToken );
    public void Emit( in SourceProductionContext context, in CancellationToken token )
    {
        StringBuilder builder = new StringBuilder( 1000 );

        builder.Append( $$"""
                          public partial record {{ClassName}}
                          {
                              [ Pure ]
                              public static {{ClassName}} Create( {{nameof(DbDataReader)}} reader )
                              {
                          """ );

        foreach ( Property property in Properties )
        {
            if ( token.IsCancellationRequested ) { return; }

            property.Handle( ref builder, ClassName );
        }

        if ( token.IsCancellationRequested ) { return; }

        builder.Append( $$"""
                          
                                  return new {{ClassName}}( {{string.Join( ",", Properties.Select( static x => x.Variable ) )}} );
                              }
                              [ Pure ]
                              public static async IAsyncEnumerable<{{ClassName}}> CreateAsync( {{nameof(DbDataReader)}} reader, [ EnumeratorCancellation ] CancellationToken token = default )
                              {
                                  while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
                              }
                          }
                          """ );

        context.AddSource( $"{ClassName}.g.cs", builder.ToString() );
    }


    /*
    public readonly record struct Method( MethodDeclarationSyntax Declaration, string Name )
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static bool Check( SyntaxNode node ) => node is AttributeSyntax { Name: IdentifierNameSyntax { Identifier.Text: nameof(DbReaderMapping) } };


        public static Method Create( ClassDeclarationSyntax declaration ) => Create( (AttributeSyntax)declaration.DescendantNodes( Check ).Single() );
        public static bool TryCreate( [ NotNullWhen( true ) ] ClassDeclarationSyntax? declaration, [ NotNullWhen( true ) ] out Method? result )
        {
            if ( declaration is null )
            {
                result = default;
                return false;
            }

            ImmutableArray<SyntaxNode> nodes = declaration.DescendantNodes( Check ).ToImmutableArray();

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


        public static bool TryCreate( [ NotNullWhen( true ) ] SyntaxNode? node, [ NotNullWhen( true ) ] out Method? result )
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
    */
}



public sealed record Property( PropertyDeclarationSyntax Declaration, string Name, string Variable, string TypeName )
{
    public TypeSyntax Type => Declaration.Type;
    public static Property Create( PropertyDeclarationSyntax declaration )
    {
        string typeName = declaration.Type.ToString();
        string name     = declaration.Identifier.ValueText;
        return new Property( declaration, name, ToSnakeCase( name, CultureInfo.InvariantCulture ), typeName );
    }
    public static ImmutableArray<Property> Create( ClassDeclarationSyntax declaration ) => declaration.DescendantNodes().OfType<PropertyDeclarationSyntax>().Select( Create ).ToImmutableArray();


    public void Handle( ref StringBuilder builder, in string className )
    {
        // if ( Type.IsUnmanaged ) { }

        if ( Type is GenericNameSyntax generic && generic.ToString().Contains( "RecordID" ) )
        {
            // string arg = generic.TypeArgumentList.Arguments.Single().ToString();

            builder.Append( $"""
                                     var {Variable} = new RecordID<{className}>( reader.GetFieldValue<Guid>( "{Name}" ) );
                             """ );
        }
        else
        {
            builder.Append( $"""
                                     var {Variable} = ({TypeName})reader.GetValue<object?>( "{Name}" );
                             """ );
        }
    }
    public static string ToSnakeCase( string value, CultureInfo cultureInfo )
    {
        if ( string.IsNullOrWhiteSpace( value ) ) { return value; }

        StringBuilder              builder          = new StringBuilder( value.Length + Math.Max( 2, value.Length / 5 ) );
        UnicodeCategory? previousCategory = default;

        for ( int currentIndex = 0; currentIndex < value.Length; currentIndex++ )
        {
            char currentChar = value[currentIndex];

            switch ( currentChar )
            {
                case '_':
                    builder.Append( '_' );
                    previousCategory = null;
                    continue;

                case '.':
                    builder.Append( '.' );
                    previousCategory = null;
                    continue;
            }

            UnicodeCategory currentCategory = char.GetUnicodeCategory( currentChar );

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch ( currentCategory )
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if ( previousCategory is UnicodeCategory.SpaceSeparator or UnicodeCategory.LowercaseLetter || previousCategory is not UnicodeCategory.DecimalDigitNumber && previousCategory is not null && currentIndex > 0 && currentIndex + 1 < value.Length && char.IsLower( value[currentIndex + 1] ) ) { builder.Append( '_' ); }

                    currentChar = char.ToLower( currentChar, cultureInfo );
                    break;

                case UnicodeCategory.LowercaseLetter:
                    if ( previousCategory is UnicodeCategory.SpaceSeparator ) { builder.Append( '_' ); }

                    break;

                default:
                    if ( previousCategory is not null ) { previousCategory = UnicodeCategory.SpaceSeparator; }

                    continue;
            }

            builder.Append( currentChar );
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }
}
