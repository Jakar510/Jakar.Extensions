// Jakar.Extensions :: Jakar.Json.Generator
// 05/01/2022  11:30 AM

#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Json.Generator;


/// <summary>
///     <para>
///         <see cref = "https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs" />
///     </para>
/// </summary>
[Generator]
[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public class JsonizerGenerator : ISourceGenerator
{
    public const            string FROM_JSON  = "FromJson";
    public const            string GENERATED  = $"[System.CodeDom.Compiler.GeneratedCode({nameof(JsonizerGenerator)})]";
    private static readonly string _attribute = typeof(JsonizerAttribute).FullName ?? throw new InvalidOperationException();

    private static string ChooseName( string fieldName, in TypedConstant overridenNameOpt )
    {
        if (!overridenNameOpt.IsNull) { return overridenNameOpt.Value.ToString(); }

        fieldName = fieldName.TrimStart( '_' );

        return fieldName.Length switch
               {
                   0 => string.Empty,
                   1 => fieldName.ToUpper(),
                   _ => fieldName[..1]
                           .ToUpper() + fieldName[1..]
               };
    }
    private static void Execute( in GeneratorExecutionContext context, in SyntaxReceiver receiver, in CancellationToken token )
    {
        // get the added attribute, and INotifyPropertyChanged
        INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName( _attribute ) ?? throw new InvalidOperationException();


        // group the fields by class, and generate the source
        foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.Fields.GroupBy<IFieldSymbol, INamedTypeSymbol>( f => f.ContainingType, SymbolEqualityComparer.Default ))
        {
            string classSource = ProcessClass( group.Key, group.ToList(), attributeSymbol, context ) ?? throw new InvalidOperationException();
            context.AddSource( $"{group.Key.Name}_autoNotify.cs", SourceText.From( classSource, Encoding.UTF8 ) );
        }
    }


    private static string? ProcessClass( INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, in ISymbol? attributeSymbol, GeneratorExecutionContext context )
    {
        ArgumentNullException.ThrowIfNull( attributeSymbol );

        if (!classSymbol.ContainingSymbol.Equals( classSymbol.ContainingNamespace, SymbolEqualityComparer.Default ))
        {
            return default; //TODO: issue a diagnostic that it must be top level
        }

        string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

        // begin building the generated source
        var source = new StringBuilder( $@"
#nullable enable
namespace {namespaceName}
{{
    public partial class {classSymbol.Name} 
    {{
" );

        // if the class doesn't implement INotifyPropertyChanged already, add it
        // if ( !classSymbol.Interfaces.Contains(notifySymbol, SymbolEqualityComparer.Default) ) { source.Append("public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;"); }

        // create properties for each field 
        foreach (IFieldSymbol fieldSymbol in fields) { ProcessField( source, fieldSymbol, attributeSymbol ); }

        source.Append( "    }" );
        source.Append( '\n' );
        source.Append( '}' );
        return source.ToString();
    }

    private static void ProcessField( StringBuilder source, in IFieldSymbol fieldSymbol, ISymbol attributeSymbol )
    {
        // get the name and type of the field
        string      fieldName = fieldSymbol.Name;
        ITypeSymbol fieldType = fieldSymbol.Type;

        // get the AutoNotify attribute from the field, and any associated data
        AttributeData attributeData = fieldSymbol.GetAttributes()
                                                 .Single( ad => ad.AttributeClass?.Equals( attributeSymbol, SymbolEqualityComparer.Default ) ?? false );

        TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault( kvp => kvp.Key == nameof(JsonizerAttribute) )
                                                      .Value;

        string propertyName = ChooseName( fieldName, overridenNameOpt );

        if (propertyName.Length == 0 || propertyName == fieldName)
        {
            //TODO: issue a diagnostic that we can't process this field
            return;
        }

        source.Append( $@"
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

        // Register a syntax receiver that will be created for each generation pass
        context.CancellationToken.ThrowIfCancellationRequested();
        context.RegisterForSyntaxNotifications( () => new SyntaxReceiver() );
    }


    public void Execute( GeneratorExecutionContext context )
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver) { return; }

        Execute( context, receiver, context.CancellationToken );
    }



    /// <summary>
    ///     Created on demand before each generation pass
    /// </summary>
    public sealed class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IFieldSymbol> Fields { get; } = new();


        /// <summary>
        ///     Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode( GeneratorSyntaxContext context )
        {
            // any field with at least one attribute is a candidate for property generation
            if (context.Node is not FieldDeclarationSyntax { AttributeLists.Count: > 0 } syntax) { return; }


            foreach (VariableDeclaratorSyntax variable in syntax.Declaration.Variables)
            {
                // Get the symbol being declared by the field, and keep it if its annotated
                if (context.SemanticModel.GetDeclaredSymbol( variable ) is not IFieldSymbol fieldSymbol) { continue; }

                if (fieldSymbol.GetAttributes()
                               .Any( data => data.AttributeClass?.ToDisplayString() == "AutoNotify.AutoNotifyAttribute" )) { Fields.Add( fieldSymbol ); }
            }
        }
    }
}
