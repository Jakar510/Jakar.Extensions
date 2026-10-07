// Jakar.SystemTextJson.Generator
// 10/02/2026

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.SystemTextJson.Generator;


// Pipeline models hold no ISymbol/SyntaxNode/Compilation references, and compare by value, so the incremental generator can cache them.



internal readonly record struct LocationInfo( string FilePath, TextSpan Span, LinePositionSpan LineSpan )
{
    public Location ToLocation() => Location.Create(FilePath, Span, LineSpan);

    public static LocationInfo? From( Location? location ) => location is null || location.SourceTree is null
                                                                  ? null
                                                                  : new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
}



internal readonly record struct DiagnosticProperty( string Key, string Value );



internal sealed record DiagnosticInfo( DiagnosticDescriptor Descriptor, LocationInfo? Location, EquatableArray<string> Arguments, EquatableArray<DiagnosticProperty> Properties )
{
    public static DiagnosticInfo Create( DiagnosticDescriptor descriptor, LocationInfo? location, params string[] arguments ) => new(descriptor, location, new EquatableArray<string>(arguments), EquatableArray<DiagnosticProperty>.Empty);


    public Diagnostic ToDiagnostic()
    {
        System.Collections.Immutable.ImmutableDictionary<string, string?>.Builder properties = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<string, string?>();
        foreach ( DiagnosticProperty property in Properties ) { properties[property.Key] = property.Value; }

        // ReSharper disable once CoVariantArrayConversion
        return Diagnostic.Create(Descriptor, Location?.ToLocation(), properties.ToImmutable(), System.Linq.Enumerable.ToArray<object?>(Arguments));
    }
}



/// <param name="Keyword"> <c> class </c>, <c> struct </c>, <c> record </c> or <c> record struct </c>. </param>
internal readonly record struct TypeDeclInfo( string Keyword, string Name, bool IsReadOnly, bool IsRef )
{
    public string Declaration => $"{( IsReadOnly ? "readonly " : "" )}{( IsRef ? "ref " : "" )}partial {Keyword} {Name}";
}



/// <summary> A <c> [JsonModel] </c> type, as extracted from the syntax tree. </summary>
internal sealed record ModelInfo
{
    public required string                         Name                    { get; init; }
    public required string                         FullyQualifiedName      { get; init; } // global::Ns.Outer.Name
    public required string                         MetadataName            { get; init; } // Ns.Outer+Name
    public required string?                        Namespace               { get; init; }
    public required EquatableArray<TypeDeclInfo>   ContainingTypes         { get; init; } // outermost first
    public required TypeDeclInfo                   Self                    { get; init; }
    public required bool                           IsValueType             { get; init; }
    public required bool                           IsAccessibleInAssembly  { get; init; }
    public required string?                        ExplicitContext         { get; init; } // global::Ns.Ctx, or null for the assembly default
    public required string?                        ExplicitContextMetadata { get; init; }
    public required bool                           EmitJsonTypeInfo        { get; init; }
    public required bool                           EmitFromJsonString      { get; init; }
    public required bool                           EmitFromJsonUtf8        { get; init; }
    public required bool                           EmitTryFromJsonString   { get; init; }
    public required bool                           EmitTryFromJsonUtf8     { get; init; }
    public required bool                           EmitFromJsonAsync       { get; init; }
    public required bool                           EmitToString            { get; init; }
    public required LocationInfo?                  AttributeLocation       { get; init; }
    public required EquatableArray<DiagnosticInfo> Diagnostics             { get; init; }
    public required bool                           IsFatal                 { get; init; }
}



internal readonly record struct Registration( string TypeName, string PropertyName );



/// <summary> A class carrying <c> [JsonSerializable] </c> attributes (normally a <c> JsonSerializerContext </c>). </summary>
internal sealed record ContextInfo( string FullyQualifiedName, string MetadataName, EquatableArray<Registration> Registrations );



/// <summary> <c> [assembly: JsonModelContext(typeof(...))] </c>, if any. </summary>
internal sealed record DefaultContextInfo( string? FullyQualifiedName, string? MetadataName, bool IsValid, string? InvalidName )
{
    public static readonly DefaultContextInfo None = new(null, null, false, null);
}



/// <summary> A model ready to be registered in <c> JsonModelRegistry </c>. </summary>
internal readonly record struct RegistrationEntry( string FullyQualifiedName );
