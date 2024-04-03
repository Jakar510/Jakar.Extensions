// Jakar.Extensions :: Jakar.Json.Generator
// 05/01/2022  11:30 AM

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Json.Generator;


/// <summary>
///     <para>
///         <see href="https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs"/>
///     </para>
/// </summary>
[Generator, SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public class JsonizerGenerator : ISourceGenerator
{
    public const            string FROM_JSON  = "FromJson";
    public const            string GENERATED  = $"[System.CodeDom.Compiler.GeneratedCode({nameof(JsonizerGenerator)})]";
    private static readonly string _attribute = typeof(JsonizerAttribute).FullName ?? throw new InvalidOperationException();


    private static string ChooseName( ReadOnlySpan<char> fieldName, in TypedConstant overridenNameOpt )
    {
        if ( !overridenNameOpt.IsNull ) { return overridenNameOpt.Value?.ToString() ?? throw new NullReferenceException( nameof(TypedConstant.Value) ); }

        fieldName = fieldName.TrimStart( '_' );

        if ( fieldName.Length == 0 ) { return string.Empty; }

        Span<char> span = stackalloc char[fieldName.Length];
        span[0] = char.ToUpper( fieldName[0] );

        fieldName[1..].CopyTo( span[1..] );

        return span.ToString();
    }


    private static string? ProcessClass( INamedTypeSymbol classSymbol, IEnumerable<IFieldSymbol> fields, in ISymbol? attributeSymbol, GeneratorExecutionContext context )
    {
        ArgumentNullException.ThrowIfNull( attributeSymbol );

        if ( !classSymbol.ContainingSymbol.Equals( classSymbol.ContainingNamespace, SymbolEqualityComparer.Default ) )
        {
            return default; //TODO: issue a diagnostic that it must be top level
        }

        string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

        // begin building the generated source
        var sb = new StringBuilder( $@"
#nullable enable
namespace {namespaceName};


public partial class {classSymbol.Name} 
{{
" );

        // if the class doesn't implement INotifyPropertyChanged already, add it
        // if ( !classSymbol.Interfaces.Contains(notifySymbol, SymbolEqualityComparer.Default) ) { source.Append("public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;"); }

        // create properties for each field 
        foreach ( IFieldSymbol fieldSymbol in fields ) { ProcessField( sb, fieldSymbol, attributeSymbol ); }

        sb.Append( '}' );
        return sb.ToString();
    }
    private static void Execute( in GeneratorExecutionContext context, in SyntaxReceiver receiver, in CancellationToken token )
    {
        // get the added attribute, and INotifyPropertyChanged
        INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName( _attribute ) ?? throw new InvalidOperationException();


        // group the fields by class, and generate the source
        foreach ( IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.Fields.GroupBy<IFieldSymbol, INamedTypeSymbol>( f => f.ContainingType, SymbolEqualityComparer.Default ) )
        {
            string classSource = ProcessClass( group.Key, group, attributeSymbol, context ) ?? throw new InvalidOperationException();
            context.AddSource( $"{group.Key.Name}_autoNotify.cs", SourceText.From( classSource, Encoding.UTF8 ) );
        }
    }

    private static void ProcessField( StringBuilder sb, in IFieldSymbol fieldSymbol, ISymbol attributeSymbol )
    {
        // get the name and type of the field
        string      fieldName = fieldSymbol.Name;
        ITypeSymbol fieldType = fieldSymbol.Type;

        // get the AutoNotify attribute from the field, and any associated data
        AttributeData attributeData = fieldSymbol.GetAttributes().Single( ad => ad.AttributeClass?.Equals( attributeSymbol, SymbolEqualityComparer.Default ) ?? false );

        TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault( kvp => kvp.Key == nameof(JsonizerAttribute) ).Value;

        string propertyName = ChooseName( fieldName, overridenNameOpt );

        if ( propertyName.Length == 0 || propertyName == fieldName )
        {
            //TODO: issue a diagnostic that we can't process this field
            return;
        }

        sb.Append( $@"
    public {fieldType} {propertyName} 
    {{
        get 
        {{
            return this.{fieldName};
        }}
        set
        {{
            this.{fieldName} = value;
            this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof({propertyName})));
        }}
    }}
" );
    }


    public void Initialize( GeneratorInitializationContext context )
    {
        // Register the attribute source
        // context.RegisterForPostInitialization(c => c.AddSource("AutoNotifyAttribute", ATTRIBUTE_TEXT));

        if ( context.CancellationToken.IsCancellationRequested ) { return; }

        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications( () => new SyntaxReceiver() );
    }


    public void Execute( GeneratorExecutionContext context )
    {
        if ( context.SyntaxContextReceiver is not SyntaxReceiver receiver ) { return; }

        Execute( context, receiver, context.CancellationToken );
    }



    /// <summary> Created on demand before each generation pass </summary>
    public sealed class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IFieldSymbol> Fields { get; } = new();


        /// <summary> Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation </summary>
        public void OnVisitSyntaxNode( GeneratorSyntaxContext context )
        {
            // any field with at least one attribute is a candidate for property generation
            if ( context.Node is not FieldDeclarationSyntax { AttributeLists.Count: > 0 } syntax ) { return; }


            foreach ( VariableDeclaratorSyntax variable in syntax.Declaration.Variables )
            {
                // Get the symbol being declared by the field, and keep it if its annotated
                if ( context.SemanticModel.GetDeclaredSymbol( variable ) is not IFieldSymbol fieldSymbol ) { continue; }

                if ( fieldSymbol.GetAttributes().Any( data => data.AttributeClass?.ToDisplayString() == "AutoNotify.AutoNotifyAttribute" ) ) { Fields.Add( fieldSymbol ); }
            }
        }
    }
}



public class JsonSerializationGenerator : ISourceGenerator
{
    public void Initialize( GeneratorInitializationContext context ) => context.RegisterForSyntaxNotifications( () => new JsonSerializationSyntaxReceiver() );

    public void Execute( GeneratorExecutionContext context )
    {
        if ( context.SyntaxReceiver is not JsonSerializationSyntaxReceiver syntaxReceiver ) { return; }

        foreach ( ClassDeclarationSyntax classDeclaration in syntaxReceiver.CandidateClasses )
        {
            string namespaceName = classDeclaration.Parent is NamespaceDeclarationSyntax namespaceDeclaration
                                       ? namespaceDeclaration.Name.ToString()
                                       : string.Empty;

            string className = classDeclaration.Identifier.ToString();
            string source    = GenerateSerializationSource( namespaceName, className, classDeclaration.Members );
            context.AddSource( $"{className}_JsonSerialization", SourceText.From( source, Encoding.UTF8 ) );
        }
    }

    private string GenerateSerializationSource( string namespaceName, string className, SyntaxList<MemberDeclarationSyntax> members )
    {
        List<PropertyDeclarationSyntax> properties = members.OfType<PropertyDeclarationSyntax>().Where( p => p.Modifiers.Any( m => m.ValueText == "public" ) ).ToList();

        var builder = new StringBuilder();
        builder.AppendLine( $"namespace {namespaceName}" );
        builder.AppendLine( "{" );
        builder.AppendLine( $"    public static partial class {className}JsonSerializationExtensions" );
        builder.AppendLine( "    {" );
        builder.AppendLine( $"        public static string ToJson(this {className} obj)" );
        builder.AppendLine( "        {" );
        builder.AppendLine( "            var sb = new System.Text.StringBuilder();" );
        builder.AppendLine( "            sb.Append(\"{\");" );

        for ( int i = 0; i < properties.Count; i++ )
        {
            PropertyDeclarationSyntax property     = properties[i];
            string                    propertyName = property.Identifier.ToString();
            string                    propertyType = property.Type.ToString();
            builder.AppendLine( $"            sb.Append(\"\\\"{propertyName}\\\": \");" );

            if ( propertyType      == "string" ) { builder.AppendLine( $"            sb.Append($\"\\\"{{obj.{propertyName}}}.Replace(\"\\\"\", \"\\\\\\\"\")}}\\\"\");" ); }
            else if ( propertyType == "int" ) { builder.AppendLine( $"            sb.Append(obj.{propertyName}.ToString());" ); }

            if ( i < properties.Count - 1 ) { builder.AppendLine( "            sb.Append(\",\");" ); }
        }

        builder.AppendLine( "            sb.Append(\"}\");" );
        builder.AppendLine( "            return sb.ToString();" );
        builder.AppendLine( "        }" );
        builder.AppendLine( "    }" );
        builder.AppendLine( "}" );

        return builder.ToString();
    }



    private class JsonSerializationSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

        public void OnVisitSyntaxNode( SyntaxNode syntaxNode )
        {
            if ( syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration )
            {
                AttributeSyntax? generateJsonAttribute = classDeclaration.AttributeLists.SelectMany( attrList => attrList.Attributes ).FirstOrDefault( attr => attr.Name.ToString() == "GenerateJsonSerializer" );

                if ( generateJsonAttribute != null ) { CandidateClasses.Add( classDeclaration ); }
            }
        }
    }
}
