// Jakar.SystemTextJson.Generator
// 10/02/2026

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Jakar.SystemTextJson.Generator;


/// <summary>
///     Implements <c> IJsonModel&lt;TSelf&gt; </c> on every <see langword="partial"/> type marked <c> [JsonModel] </c>: the <c> JsonTypeInfo </c> property (from the type's
///     <c> JsonSerializerContext </c>), the static <c> FromJson </c> / <c> TryFromJson </c> / <c> FromJsonAsync </c> methods, and a lazy <c> JsonModelRegistry </c> registration.
/// </summary>
/// <remarks>
///     Generators can't see each other's output, so this can't add <c> [JsonSerializable] </c> for System.Text.Json: the type must be registered in its context by hand.
///     The generator reads the context's <c> [JsonSerializable] </c> attributes (user-written, so visible) to check that, and to use the exact property name System.Text.Json generates.
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class JsonModelGenerator : IIncrementalGenerator
{
    internal const string JSON_MODEL_ATTRIBUTE         = "Jakar.Extensions.JsonModelAttribute";
    internal const string JSON_MODEL_CONTEXT_ATTRIBUTE = "Jakar.Extensions.JsonModelContextAttribute";
    internal const string JSON_SERIALIZABLE_ATTRIBUTE  = "System.Text.Json.Serialization.JsonSerializableAttribute";
    internal const string JSON_SERIALIZER_CONTEXT      = "System.Text.Json.Serialization.JsonSerializerContext";
    internal const string VERSION                      = "1.0.0";


    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        IncrementalValuesProvider<ModelInfo> models = context.SyntaxProvider.ForAttributeWithMetadataName(JSON_MODEL_ATTRIBUTE, static ( node, _ ) => node is TypeDeclarationSyntax, static ( ctx, token ) => ModelExtractor.Extract(ctx, token));

        IncrementalValueProvider<ImmutableArray<ContextInfo>> contexts = context.SyntaxProvider.ForAttributeWithMetadataName(JSON_SERIALIZABLE_ATTRIBUTE, static ( node, _ ) => node is ClassDeclarationSyntax, static ( ctx, token ) => ContextExtractor.Extract(ctx, token)).Collect();

        IncrementalValueProvider<DefaultContextInfo> defaultContext = context.CompilationProvider.Select(static ( compilation, _ ) => ContextExtractor.GetDefault(compilation));

        IncrementalValuesProvider<(ModelInfo Model, (ImmutableArray<ContextInfo> Contexts, DefaultContextInfo Default) Lookup)> combined = models.Combine(contexts.Combine(defaultContext));

        context.RegisterSourceOutput(combined, static ( spc, pair ) => Emitter.EmitModel(spc, pair.Model, pair.Lookup.Contexts, pair.Lookup.Default));

        IncrementalValueProvider<ImmutableArray<RegistrationEntry>> registrations = combined.Select(static ( pair, _ ) => Emitter.GetRegistration(pair.Model, pair.Lookup.Contexts, pair.Lookup.Default)).Where(static entry => entry.HasValue).Select(static ( entry, _ ) => entry!.Value).Collect();

        context.RegisterSourceOutput(registrations, static ( spc, entries ) => Emitter.EmitRegistrations(spc, entries));
    }
}



internal static class ModelExtractor
{
    public static ModelInfo Extract( GeneratorAttributeSyntaxContext ctx, CancellationToken token )
    {
        INamedTypeSymbol      symbol            = (INamedTypeSymbol)ctx.TargetSymbol;
        TypeDeclarationSyntax declaration       = (TypeDeclarationSyntax)ctx.TargetNode;
        AttributeData         attribute         = ctx.Attributes[0];
        LocationInfo?         attributeLocation = LocationInfo.From(attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation() ?? declaration.Identifier.GetLocation());
        string                displayName       = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        List<DiagnosticInfo> diagnostics = [];
        bool                 fatal       = false;

        // JAKAR_JSON002: the type and every containing type must be partial.
        for ( INamedTypeSymbol? current = symbol; current is not null; current = current.ContainingType )
        {
            if ( IsPartial(current, token) ) { continue; }

            diagnostics.Add(DiagnosticInfo.Create(JsonModelDiagnostics.NotPartial, attributeLocation, current.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            fatal = true;
            break;
        }

        // JAKAR_JSON003: open generic types (or types nested in one) can't be source generated.
        for ( INamedTypeSymbol? current = symbol; current is not null; current = current.ContainingType )
        {
            if ( current.TypeParameters.Length == 0 ) { continue; }

            diagnostics.Add(DiagnosticInfo.Create(JsonModelDiagnostics.OpenGeneric, attributeLocation, displayName));
            fatal = true;
            break;
        }

        // JAKAR_JSON004: an explicit context must be a JsonSerializerContext.
        string? explicitContext         = null;
        string? explicitContextMetadata = null;

        if ( attribute.ConstructorArguments.Length == 1 )
        {
            if ( attribute.ConstructorArguments[0].Value is INamedTypeSymbol contextType && DerivesFrom(contextType, JsonModelGenerator.JSON_SERIALIZER_CONTEXT) )
            {
                explicitContext         = contextType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                explicitContextMetadata = GetMetadataName(contextType);
            }
            else
            {
                string name = attribute.ConstructorArguments[0].Value is ITypeSymbol other
                                  ? other.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                                  : "null";

                diagnostics.Add(DiagnosticInfo.Create(JsonModelDiagnostics.InvalidContext, attributeLocation, $"'{name}' isn't a JsonSerializerContext; [JsonModel] needs the source-generated context that registers '{displayName}'"));
                fatal = true;
            }
        }

        bool generateToString = false;

        foreach ( KeyValuePair<string, TypedConstant> argument in attribute.NamedArguments )
        {
            if ( argument is { Key: "GenerateToString", Value.Value: bool value } ) { generateToString = value; }
        }

        // JAKAR_JSON005: a hand-written JsonTypeInfo wins.
        bool handWritten = symbol.GetMembers("JsonTypeInfo").Any(static member => member is IPropertySymbol { IsStatic: true } && !member.IsImplicitlyDeclared);
        if ( handWritten ) { diagnostics.Add(DiagnosticInfo.Create(JsonModelDiagnostics.HandWritten, attributeLocation, displayName)); }

        token.ThrowIfCancellationRequested();

        return new ModelInfo
               {
                   Name               = symbol.Name,
                   FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                   MetadataName       = GetMetadataName(symbol),
                   Namespace = symbol.ContainingNamespace is { IsGlobalNamespace: false } ns
                                   ? ns.ToDisplayString()
                                   : null,
                   ContainingTypes         = new EquatableArray<TypeDeclInfo>(GetContainingTypes(symbol, token)),
                   Self                    = GetDeclInfo(symbol, token),
                   IsValueType             = symbol.IsValueType,
                   IsAccessibleInAssembly  = IsAccessibleInAssembly(symbol),
                   ExplicitContext         = explicitContext,
                   ExplicitContextMetadata = explicitContextMetadata,
                   EmitJsonTypeInfo        = !handWritten,
                   EmitFromJsonString      = !HasMethod(symbol, "FromJson",      IsString),
                   EmitFromJsonUtf8        = !HasMethod(symbol, "FromJson",      IsByteSpan),
                   EmitTryFromJsonString   = !HasMethod(symbol, "TryFromJson",   IsString),
                   EmitTryFromJsonUtf8     = !HasMethod(symbol, "TryFromJson",   IsByteSpan),
                   EmitFromJsonAsync       = !HasMethod(symbol, "FromJsonAsync", IsStream),
                   EmitToString            = generateToString && !OverridesToString(symbol),
                   AttributeLocation       = attributeLocation,
                   Diagnostics             = new EquatableArray<DiagnosticInfo>(diagnostics),
                   IsFatal                 = fatal
               };
    }


    private static bool IsPartial( INamedTypeSymbol symbol, CancellationToken token )
    {
        foreach ( SyntaxReference reference in symbol.DeclaringSyntaxReferences )
        {
            if ( reference.GetSyntax(token) is not TypeDeclarationSyntax declaration || !declaration.Modifiers.Any(SyntaxKind.PartialKeyword) ) { return false; }
        }

        return symbol.DeclaringSyntaxReferences.Length > 0;
    }


    internal static bool DerivesFrom( INamedTypeSymbol type, string metadataName )
    {
        for ( INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType )
        {
            if ( GetMetadataName(current) == metadataName ) { return true; }
        }

        return false;
    }


    /// <summary> <c> Ns.Outer+Inner </c>, as <c> Compilation.GetTypeByMetadataName </c> expects. </summary>
    internal static string GetMetadataName( INamedTypeSymbol symbol )
    {
        string name = symbol.MetadataName;

        for ( INamedTypeSymbol? current = symbol.ContainingType; current is not null; current = current.ContainingType ) { name = $"{current.MetadataName}+{name}"; }

        return symbol.ContainingNamespace is { IsGlobalNamespace: false } ns
                   ? $"{ns.ToDisplayString()}.{name}"
                   : name;
    }


    private static TypeDeclInfo[] GetContainingTypes( INamedTypeSymbol symbol, CancellationToken token )
    {
        List<TypeDeclInfo> types = [];
        for ( INamedTypeSymbol? current = symbol.ContainingType; current is not null; current = current.ContainingType ) { types.Insert(0, GetDeclInfo(current, token)); }

        return [.. types];
    }


    private static TypeDeclInfo GetDeclInfo( INamedTypeSymbol symbol, CancellationToken token )
    {
        TypeDeclarationSyntax? declaration = symbol.DeclaringSyntaxReferences.Select(reference => reference.GetSyntax(token)).OfType<TypeDeclarationSyntax>().FirstOrDefault();

        string keyword = declaration switch
                         {
                             RecordDeclarationSyntax record when record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword) => "record struct",
                             RecordDeclarationSyntax                                                                          => "record",
                             StructDeclarationSyntax                                                                          => "struct",
                             InterfaceDeclarationSyntax                                                                       => "interface",
                             _                                                                                                => "class"
                         };

        bool isReadOnly = declaration?.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) ?? false;
        bool isRef      = declaration?.Modifiers.Any(SyntaxKind.RefKeyword)      ?? false;
        return new TypeDeclInfo(keyword, symbol.Name, isReadOnly, isRef);
    }


    /// <summary> Whether code in this assembly can name the type: needed for the module initializer that registers it. </summary>
    private static bool IsAccessibleInAssembly( INamedTypeSymbol symbol )
    {
        for ( INamedTypeSymbol? current = symbol; current is not null; current = current.ContainingType )
        {
            if ( current.IsFileLocal ) { return false; }

            switch ( current.DeclaredAccessibility )
            {
                case Accessibility.Public:
                case Accessibility.Internal:
                case Accessibility.ProtectedOrInternal:
                    continue;

                default:
                    return false;
            }
        }

        return true;
    }


    private static bool IsString( IParameterSymbol parameter ) => parameter.Type.SpecialType == SpecialType.System_String;

    private static bool IsByteSpan( IParameterSymbol parameter ) => parameter.Type is INamedTypeSymbol { Name: "ReadOnlySpan", ContainingNamespace.Name: "System", TypeArguments.Length: 1 } span && span.TypeArguments[0].SpecialType == SpecialType.System_Byte;

    private static bool IsStream( IParameterSymbol parameter ) => parameter.Type is INamedTypeSymbol { Name: "Stream", ContainingNamespace: { Name: "IO", ContainingNamespace.Name: "System" } };


    /// <summary> Whether the type or a base type already declares <paramref name="name"/> with this first parameter (emitting it again would collide or hide it). </summary>
    private static bool HasMethod( INamedTypeSymbol symbol, string name, Func<IParameterSymbol, bool> firstParameter )
    {
        for ( INamedTypeSymbol? current = symbol; current is not null; current = current.BaseType )
        {
            foreach ( ISymbol member in current.GetMembers(name) )
            {
                if ( member is IMethodSymbol { Parameters.Length: > 0 } method && firstParameter(method.Parameters[0]) ) { return true; }
            }
        }

        return false;
    }


    private static bool OverridesToString( INamedTypeSymbol symbol )
    {
        for ( INamedTypeSymbol? current = symbol; current is not null; current = current.BaseType )
        {
            if ( current.SpecialType is SpecialType.System_Object or SpecialType.System_ValueType or SpecialType.System_Enum ) { return false; }

            if ( current.GetMembers("ToString").Any(static member => member is IMethodSymbol { Parameters.Length: 0, IsImplicitlyDeclared: false }) ) { return true; }
        }

        return false;
    }
}



internal static class ContextExtractor
{
    public static ContextInfo Extract( GeneratorAttributeSyntaxContext ctx, CancellationToken token )
    {
        INamedTypeSymbol   symbol        = (INamedTypeSymbol)ctx.TargetSymbol;
        List<Registration> registrations = [];

        // All [JsonSerializable] attributes on the symbol, across every partial declaration.
        foreach ( AttributeData attribute in symbol.GetAttributes() )
        {
            token.ThrowIfCancellationRequested();
            if ( attribute.AttributeClass is null || ModelExtractor.GetMetadataName(attribute.AttributeClass) != JsonModelGenerator.JSON_SERIALIZABLE_ATTRIBUTE ) { continue; }

            if ( attribute.ConstructorArguments.Length != 1 || attribute.ConstructorArguments[0].Value is not ITypeSymbol type ) { continue; }

            string? propertyName = null;

            foreach ( KeyValuePair<string, TypedConstant> argument in attribute.NamedArguments )
            {
                if ( argument is { Key: "TypeInfoPropertyName", Value.Value: string name } ) { propertyName = name; }
            }

            registrations.Add(new Registration(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), propertyName ?? GetDefaultPropertyName(type)));
        }

        return new ContextInfo(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), ModelExtractor.GetMetadataName(symbol), new EquatableArray<Registration>(registrations));
    }


    /// <summary> The property name System.Text.Json generates when <c> TypeInfoPropertyName </c> isn't set: the type name, <c> ElementArray </c> for arrays, <c> ListInvoice </c> for generics. </summary>
    internal static string GetDefaultPropertyName( ITypeSymbol type ) => type switch
                                                                         {
                                                                             IArrayTypeSymbol array                                                                                           => $"{GetDefaultPropertyName(array.ElementType)}Array",
                                                                             INamedTypeSymbol { IsGenericType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable => $"Nullable{GetDefaultPropertyName(nullable.TypeArguments[0])}",
                                                                             INamedTypeSymbol { IsGenericType: true } generic                                                                 => generic.Name + string.Concat(generic.TypeArguments.Select(GetDefaultPropertyName)),
                                                                             _                                                                                                                => type.Name
                                                                         };


    public static DefaultContextInfo GetDefault( Compilation compilation )
    {
        foreach ( AttributeData attribute in compilation.Assembly.GetAttributes() )
        {
            if ( attribute.AttributeClass is null || ModelExtractor.GetMetadataName(attribute.AttributeClass) != JsonModelGenerator.JSON_MODEL_CONTEXT_ATTRIBUTE ) { continue; }

            if ( attribute.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Value is INamedTypeSymbol context )
            {
                return ModelExtractor.DerivesFrom(context, JsonModelGenerator.JSON_SERIALIZER_CONTEXT)
                           ? new DefaultContextInfo(context.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), ModelExtractor.GetMetadataName(context), true,  null)
                           : new DefaultContextInfo(null,                                                              null,                                    false, context.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            return new DefaultContextInfo(null, null, false, "null");
        }

        return DefaultContextInfo.None;
    }
}
