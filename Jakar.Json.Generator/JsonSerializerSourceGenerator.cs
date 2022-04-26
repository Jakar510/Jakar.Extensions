using Microsoft.CodeAnalysis;



namespace Jakar.Json.Generator;


public interface IJson
{
    public string ToJson();
}



[Generator]
public class JsonSerializerSourceGenerator : ISourceGenerator
{
    public const string FROM_JSON = "FromJson";
    public const string AUTO      = " // Auto-generated code";


    public void Execute( GeneratorExecutionContext context )
    {
        IMethodSymbol? mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);
    }
    public void Execute( in Compilation compilation )
    {
        IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
    }

    private void Finalize( in GeneratorExecutionContext context, in IMethodSymbol method, in string source ) => context.AddSource($"{method.ContainingType.Name}.g.cs", source);

    public void Initialize( GeneratorInitializationContext context )
    {
        // No initialization required for this one
    }
}
