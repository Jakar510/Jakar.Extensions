// Jakar.Json.Generator
// 10/07/2026

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Json.Generator;


// Pipeline models hold no ISymbol/SyntaxNode/Compilation references and compare by value, so the incremental generator can cache them.



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
/// <param name="TypeParameters"> <c>&lt;T, U&gt;</c>, or empty. </param>
internal readonly record struct TypeDeclInfo( string Keyword, string Name, string TypeParameters, bool IsReadOnly, bool IsRef )
{
    public string Declaration => $"{( IsReadOnly ? "readonly " : "" )}{( IsRef ? "ref " : "" )}partial {Keyword} {Name}{TypeParameters}";
}



internal enum ShapeKind : byte
{
    Unsupported,
    Bool,
    String,
    Char,
    Integer,
    Float,
    Guid,
    DateTime,
    DateTimeOffset,
    DateOnly,
    TimeOnly,
    TimeSpan,
    Uri,
    Version,
    Parsable,
    Model,
    Enum,
    Nullable,
    Array,
    List,
    Enumerable,
    ImmutableArray,
    ImmutableList,
    HashSet,
    FrozenSet,
    SortedSet,
    Queue,
    Stack,
    Dictionary,
    Map,
    Node
}



internal enum MapKind : byte
{
    None,
    Sorted,
    Ordered,
    Immutable,
    ImmutableSorted,
    Frozen,
    Concurrent
}



internal readonly record struct EnumMemberInfo( string Name, string JsonName, ulong Bits );



/// <summary> An enum used by some member, with the names its codec reads and writes (one codec per enum and naming policy). </summary>
internal sealed record EnumInfo( string FullyQualifiedName, string MetadataName, string UnderlyingType, bool IsFlags, int Naming, EquatableArray<EnumMemberInfo> Members )
{
    /// <summary> The generated converter: <c>global::Jakar.Json.Generated.JsonEnums.Ns_Status__1</c>. </summary>
    public string ConverterName => $"global::Jakar.Json.Generated.JsonEnums.{Sanitize(MetadataName)}__{Naming}";

    private static string Sanitize( string name )
    {
        char[] chars = name.ToCharArray();

        for ( int i = 0; i < chars.Length; i++ )
        {
            if ( !char.IsLetterOrDigit(chars[i]) ) { chars[i] = '_'; }
        }

        return new string(chars);
    }
}



/// <summary> How a member's type maps to JSON. Recursive for elements, values and keys. </summary>
internal sealed record TypeShape( ShapeKind  Kind,
                                  string     TypeName, // global::-qualified, without a top-level nullable annotation
                                  bool       IsReferenceType,
                                  bool       IsNullable, // annotated T? (or oblivious) reference type, or Nullable<T>
                                  TypeShape? Element,    // collection elements, map values, Nullable<T>'s T
                                  TypeShape? Key,        // map keys
                                  MapKind    MapKind,
                                  EnumInfo?  Enum,
                                  bool       IsComparable, // can be sorted (keys, set elements): string, or IComparable<T>
                                  string?    Reason ) // why it's unsupported
{
    public static TypeShape Unsupported( string typeName, string reason ) => new(ShapeKind.Unsupported, typeName, false, false, null, null, MapKind.None, null, false, reason);
}



internal enum SetterKind : byte
{
    None,
    Set,
    Init,
    Field,
    ReadOnlyField
}



internal sealed record MemberInfo
{
    public required string        Name                 { get; init; }
    public required string        TypeName             { get; init; } // as declared, with nullable annotation
    public required TypeShape     Shape                { get; init; }
    public required string?       JsonNameOverride     { get; init; }
    public required int           Order                { get; init; }
    public required bool          HasOrder             { get; init; }
    public required bool          CanGet               { get; init; }
    public required SetterKind    Setter               { get; init; }
    public required bool          IsRequiredKeyword    { get; init; }
    public required bool          RequiredAttribute    { get; init; }
    public required string?       Converter            { get; init; } // a converter type that fits; null otherwise
    public required bool          IsExtensionData      { get; init; }
    public required Settings      Settings             { get; init; }
    public required int           BaseDepth            { get; init; } // 0 for the most-base declaring type
    public required string        FilePath             { get; init; }
    public required int           Position             { get; init; }
    public required string        DeclaringType        { get; init; } // global::-qualified
    public required bool          DeclaringIsValueType { get; init; }
    public required LocationInfo? Location             { get; init; }
}



internal readonly record struct ParameterInfo( string Name, string TypeName, string? DefaultLiteral, bool HasDefault, string? Member );



internal enum ConstructionKind : byte
{
    /// <summary> Abstract: only derived types are constructed. </summary>
    None,
    /// <summary> <c>new T(args) { required... }</c> </summary>
    Constructor,
    /// <summary> A struct without an explicit constructor: <c>new T() { required... }</c>. </summary>
    Default
}



internal sealed record ConstructionInfo( ConstructionKind Kind, EquatableArray<ParameterInfo> Parameters, bool SetsRequiredMembers );



internal readonly record struct DerivedInfo( string TypeName, string Tag );



/// <summary> A <c>[GenerateJson]</c> type, as extracted from the syntax tree. </summary>
internal sealed record ModelInfo
{
    public required string                         Name                { get; init; }
    public required string                         FullyQualifiedName  { get; init; } // global::Ns.Outer.Name<T>
    public required string                         HintName            { get; init; }
    public required string?                        Namespace           { get; init; }
    public required EquatableArray<TypeDeclInfo>   ContainingTypes     { get; init; } // outermost first
    public required TypeDeclInfo                   Self                { get; init; }
    public required bool                           IsValueType         { get; init; }
    public required bool                           IsAbstract          { get; init; }
    public required bool                           IsSealed            { get; init; }
    public required Settings                       Settings            { get; init; } // [GenerateJson] on the type
    public required EquatableArray<MemberInfo>     Members             { get; init; }
    public required ConstructionInfo               Construction        { get; init; }
    public required string                         Discriminator       { get; init; }
    public required EquatableArray<DerivedInfo>    Derived             { get; init; } // most-derived first
    public required DerivedInfo?                   OwnTag              { get; init; } // (base discriminator, tag) when a base lists this type
    public required bool                           HasSerializableBase { get; init; } // a base type already has generated members: hide them with 'new'
    public required EquatableArray<string>         Skip                { get; init; } // generated members the type already declares
    public required bool                           OverridesToString   { get; init; }
    public required LocationInfo?                  AttributeLocation   { get; init; }
    public required EquatableArray<DiagnosticInfo> Diagnostics         { get; init; }
    public required bool                           IsFatal             { get; init; }
}
