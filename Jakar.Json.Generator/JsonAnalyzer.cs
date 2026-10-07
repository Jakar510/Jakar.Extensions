// Jakar.Json.Generator
// 10/07/2026

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;



namespace Jakar.Json.Generator;


/// <summary> JJSON015: a type lists <c>IJsonSerializable&lt;T&gt;</c> itself, has no <c>[GenerateJson]</c>, and doesn't implement <c>WriteJson</c> / <c>TryReadJson</c>. </summary>
/// <remarks> A symbol analyzer rather than part of the generator, so the generator's pipeline stays attribute-driven (cheap per keystroke). </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class JsonAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [JsonDiagnostics.MissingAttribute];


    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static start =>
                                               {
                                                   INamedTypeSymbol? serializable = start.Compilation.GetTypeByMetadataName(JsonGenerator.SERIALIZABLE_INTERFACE);
                                                   INamedTypeSymbol? attribute    = start.Compilation.GetTypeByMetadataName(JsonGenerator.GENERATE_JSON_ATTRIBUTE);

                                                   if ( serializable is not null && attribute is not null ) { start.RegisterSymbolAction(ctx => Analyze(ctx, serializable, attribute), SymbolKind.NamedType); }
                                               });
    }


    private static void Analyze( SymbolAnalysisContext ctx, INamedTypeSymbol serializable, INamedTypeSymbol attribute )
    {
        INamedTypeSymbol type = (INamedTypeSymbol)ctx.Symbol;
        if ( type.TypeKind is not (TypeKind.Class or TypeKind.Struct) || type.IsStatic ) { return; }

        if ( !type.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, serializable) && SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], type)) ) { return; }

        if ( type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)) ) { return; }

        if ( type.GetMembers("WriteJson").Any(static m => m is IMethodSymbol { IsStatic: true }) && type.GetMembers("TryReadJson").Any(static m => m is IMethodSymbol { IsStatic: true }) ) { return; }

        Location? location = type.Locations.FirstOrDefault(static l => l.IsInSource);
        if ( location is not null ) { ctx.ReportDiagnostic(Diagnostic.Create(JsonDiagnostics.MissingAttribute, location, type.Name)); }
    }
}
