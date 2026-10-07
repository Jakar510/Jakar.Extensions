// Jakar.SystemTextJson.Generator
// 10/02/2026

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;



namespace Jakar.SystemTextJson.Generator;


/// <summary>
///     <list type="bullet">
///         <item> JAKAR_JSON006: a type lists <c> IJsonModel&lt;T&gt; </c> directly, has no <c> [JsonModel] </c>, and doesn't implement <c> JsonTypeInfo </c>. </item>
///         <item> JAKAR_JSON007: an <c> IJsonModel.AdditionalData </c> implementation lacks System.Text.Json's <c> [JsonExtensionData] </c>. </item>
///     </list>
/// </summary>
/// <remarks> A symbol analyzer rather than part of the generator, so the generator's pipeline stays attribute-driven (cheap per keystroke). </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class JsonModelAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [JsonModelDiagnostics.MissingAttribute, JsonModelDiagnostics.MissingExtensionData];


    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static start =>
                                               {
                                                   INamedTypeSymbol? jsonModel      = start.Compilation.GetTypeByMetadataName("Jakar.Extensions.IJsonModel`1");
                                                   INamedTypeSymbol? bag            = start.Compilation.GetTypeByMetadataName("Jakar.Extensions.IJsonModel");
                                                   INamedTypeSymbol? attribute      = start.Compilation.GetTypeByMetadataName(JsonModelGenerator.JSON_MODEL_ATTRIBUTE);
                                                   INamedTypeSymbol? extensionData  = start.Compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonExtensionDataAttribute");
                                                   IPropertySymbol?  additionalData = bag?.GetMembers("AdditionalData").OfType<IPropertySymbol>().FirstOrDefault();

                                                   if ( jsonModel is not null && attribute is not null ) { start.RegisterSymbolAction(ctx => AnalyzeType(ctx, jsonModel, attribute), SymbolKind.NamedType); }

                                                   if ( bag is not null && extensionData is not null && additionalData is not null ) { start.RegisterSymbolAction(ctx => AnalyzeProperty(ctx, bag, additionalData, extensionData), SymbolKind.Property); }
                                               });
    }


    private static void AnalyzeType( SymbolAnalysisContext ctx, INamedTypeSymbol jsonModel, INamedTypeSymbol attribute )
    {
        INamedTypeSymbol type = (INamedTypeSymbol)ctx.Symbol;
        if ( type.TypeKind is not (TypeKind.Class or TypeKind.Struct) || type.IsAbstract || type.IsStatic ) { return; }

        if ( !type.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, jsonModel) && SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], type)) ) { return; }

        if ( type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)) ) { return; }

        if ( type.GetMembers("JsonTypeInfo").Any(static member => member is IPropertySymbol { IsStatic: true }) ) { return; }

        Location? location = type.Locations.FirstOrDefault(static l => l.IsInSource);
        if ( location is not null ) { ctx.ReportDiagnostic(Diagnostic.Create(JsonModelDiagnostics.MissingAttribute, location, type.Name)); }
    }


    private static void AnalyzeProperty( SymbolAnalysisContext ctx, INamedTypeSymbol bag, IPropertySymbol additionalData, INamedTypeSymbol extensionData )
    {
        IPropertySymbol property = (IPropertySymbol)ctx.Symbol;
        if ( property.Name != "AdditionalData" || property.IsStatic ) { return; }

        INamedTypeSymbol type = property.ContainingType;
        if ( !type.AllInterfaces.Contains(bag, SymbolEqualityComparer.Default) ) { return; }

        // The implementation itself, or an override of it (STJ reads the attributes on the declared property).
        bool implements = SymbolEqualityComparer.Default.Equals(type.FindImplementationForInterfaceMember(additionalData), property) || property.IsOverride;
        if ( !implements ) { return; }

        if ( property.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, extensionData)) ) { return; }

        Location? location = property.Locations.FirstOrDefault(static l => l.IsInSource);
        if ( location is not null ) { ctx.ReportDiagnostic(Diagnostic.Create(JsonModelDiagnostics.MissingExtensionData, location, type.Name)); }
    }
}
