// Jakar.Json.Generator
// 10/07/2026

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;



namespace Jakar.Json.Generator;


/// <summary> The well-known types shape analysis needs, looked up once per compilation. </summary>
internal sealed class KnownTypes( Compilation compilation )
{
    public readonly INamedTypeSymbol? SpanFormattable   = compilation.GetTypeByMetadataName("System.ISpanFormattable");
    public readonly INamedTypeSymbol? SpanParsable      = compilation.GetTypeByMetadataName("System.ISpanParsable`1");
    public readonly INamedTypeSymbol? Comparable        = compilation.GetTypeByMetadataName("System.IComparable`1");
    public readonly INamedTypeSymbol? Serializable      = compilation.GetTypeByMetadataName(JsonGenerator.SERIALIZABLE_INTERFACE);
    public readonly INamedTypeSymbol? Converter         = compilation.GetTypeByMetadataName(JsonGenerator.CONVERTER_INTERFACE);
    public readonly INamedTypeSymbol? GenerateJson      = compilation.GetTypeByMetadataName(JsonGenerator.GENERATE_JSON_ATTRIBUTE);
    public readonly INamedTypeSymbol? JNode             = compilation.GetTypeByMetadataName("Jakar.Json.JNode");
    public readonly INamedTypeSymbol? JObjectNode       = compilation.GetTypeByMetadataName("Jakar.Json.JObjectNode");
    public readonly INamedTypeSymbol? OrderedDictionary = compilation.GetTypeByMetadataName("System.Collections.Generic.OrderedDictionary`2");


    public bool Implements( ITypeSymbol type, INamedTypeSymbol? generic, ITypeSymbol argument ) => generic is not null && type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, generic) && SymbolEqualityComparer.Default.Equals(i.TypeArguments[0].WithNullableAnnotation(NullableAnnotation.None), argument.WithNullableAnnotation(NullableAnnotation.None)));

    public bool Implements( ITypeSymbol type, INamedTypeSymbol? plain ) => plain is not null && type.AllInterfaces.Contains(plain, SymbolEqualityComparer.Default);

    /// <summary> <c>[GenerateJson]</c> in this compilation (its generated interface isn't visible yet), or an <c>IJsonSerializable&lt;T&gt;</c> from anywhere. </summary>
    public bool IsModel( ITypeSymbol type ) => Implements(type, Serializable, type) || ( GenerateJson is not null && type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, GenerateJson)) );

    public bool IsParsable( ITypeSymbol type ) => Implements(type, SpanFormattable) && Implements(type, SpanParsable, type);

    public bool IsComparable( ITypeSymbol type ) => type.SpecialType == SpecialType.System_String || type.TypeKind == TypeKind.Enum || Implements(type, Comparable, type);
}



internal static class TypeShapes
{
    public static readonly SymbolDisplayFormat Format = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);


    /// <summary> The type's name for generated code, without a top-level <c>?</c> on reference types. </summary>
    public static string Name( ITypeSymbol type ) => type.IsReferenceType
                                                         ? type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(Format)
                                                         : type.ToDisplayString(Format);


    public static TypeShape Analyze( ITypeSymbol type, KnownTypes known, int depth = 0 )
    {
        string name     = Name(type);
        bool   nullable = type.IsReferenceType && type.NullableAnnotation != NullableAnnotation.NotAnnotated; // annotated, or oblivious code

        if ( depth > 16 ) { return TypeShape.Unsupported(name, "it nests too deeply"); }

        TypeShape Simple( ShapeKind kind ) => new(kind, name, type.IsReferenceType, nullable, null, null, MapKind.None, null, known.IsComparable(type), null);

        // ─── Nullable<T> ──────────────────────────────────────────────────────
        if ( type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableValue )
        {
            TypeShape inner = Analyze(nullableValue.TypeArguments[0], known, depth + 1);

            return inner.Kind == ShapeKind.Unsupported
                       ? inner
                       : new TypeShape(ShapeKind.Nullable, name, false, true, inner, null, MapKind.None, null, inner.IsComparable, null);
        }

        // ─── Type parameters: static dispatch needs a constraint ──────────────
        if ( type is ITypeParameterSymbol parameter )
        {
            if ( parameter.ConstraintTypes.Any(c => c is INamedTypeSymbol { IsGenericType: true } named && SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, known.Serializable) && SymbolEqualityComparer.Default.Equals(named.TypeArguments[0], parameter)) ) { return Simple(ShapeKind.Model); }

            bool formattable = parameter.ConstraintTypes.Any(c => SymbolEqualityComparer.Default.Equals(c,                                                                               known.SpanFormattable) || known.Implements(c, known.SpanFormattable));
            bool parsable    = parameter.ConstraintTypes.Any(c => c is INamedTypeSymbol { IsGenericType: true } named && SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, known.SpanParsable) && SymbolEqualityComparer.Default.Equals(named.TypeArguments[0], parameter));
            if ( formattable && parsable ) { return Simple(ShapeKind.Parsable); }

            return TypeShape.Unsupported(name, "TYPE_PARAMETER");
        }

        // ─── Special types ────────────────────────────────────────────────────
        switch ( type.SpecialType )
        {
            case SpecialType.System_Boolean:
                return Simple(ShapeKind.Bool);

            case SpecialType.System_String:
                return Simple(ShapeKind.String);

            case SpecialType.System_Char:
                return Simple(ShapeKind.Char);

            case SpecialType.System_SByte or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_IntPtr or SpecialType.System_UIntPtr:
                return Simple(ShapeKind.Integer);

            case SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal:
                return Simple(ShapeKind.Float);

            case SpecialType.System_Object:
                return TypeShape.Unsupported(name, "INVALID:object has no fixed JSON shape");
        }

        if ( type.TypeKind == TypeKind.Enum && type is INamedTypeSymbol enumType ) { return Simple(ShapeKind.Enum) with { Enum = EnumInfoFor(enumType) }; }

        if ( type.TypeKind is TypeKind.Delegate ) { return TypeShape.Unsupported(name, "INVALID:a delegate"); }

        if ( type.TypeKind is TypeKind.Pointer or TypeKind.FunctionPointer ) { return TypeShape.Unsupported(name, "INVALID:a pointer"); }

        if ( type.IsRefLikeType ) { return TypeShape.Unsupported(name, "INVALID:a ref struct"); }

        if ( type is IArrayTypeSymbol array )
        {
            if ( array.Rank != 1 ) { return TypeShape.Unsupported(name, "multi-dimensional arrays aren't supported"); }

            return Sequence(ShapeKind.Array, array.ElementType);
        }

        string metadata = MetadataName(type);

        switch ( metadata )
        {
            case "System.Int128" or "System.UInt128" or "System.Numerics.BigInteger":
                return Simple(ShapeKind.Integer);

            case "System.Half":
                return Simple(ShapeKind.Float);

            case "System.Guid":
                return Simple(ShapeKind.Guid);

            case "System.DateTime":
                return Simple(ShapeKind.DateTime);

            case "System.DateTimeOffset":
                return Simple(ShapeKind.DateTimeOffset);

            case "System.DateOnly":
                return Simple(ShapeKind.DateOnly);

            case "System.TimeOnly":
                return Simple(ShapeKind.TimeOnly);

            case "System.TimeSpan":
                return Simple(ShapeKind.TimeSpan);

            case "System.Uri":
                return Simple(ShapeKind.Uri);

            case "System.Version":
                return Simple(ShapeKind.Version);

            case "Jakar.Json.JNode" or "Jakar.Json.JObjectNode" or "Jakar.Json.JArrayNode" or "Jakar.Json.JValueNode":
                return Simple(ShapeKind.Node);
        }

        if ( type is INamedTypeSymbol { IsGenericType: true } generic )
        {
            ImmutableArrayOf args = new(generic.TypeArguments);

            switch ( MetadataName(generic.OriginalDefinition) )
            {
                case "System.Collections.Generic.List`1":
                    return Sequence(ShapeKind.List, args[0]);

                case "System.Collections.Generic.IEnumerable`1":
                case "System.Collections.Generic.ICollection`1":
                case "System.Collections.Generic.IList`1":
                case "System.Collections.Generic.IReadOnlyCollection`1":
                case "System.Collections.Generic.IReadOnlyList`1":
                    return Sequence(ShapeKind.Enumerable, args[0]);

                case "System.Collections.Immutable.ImmutableArray`1":
                    return Sequence(ShapeKind.ImmutableArray, args[0]);

                case "System.Collections.Immutable.ImmutableList`1":
                    return Sequence(ShapeKind.ImmutableList, args[0]);

                case "System.Collections.Generic.HashSet`1":
                case "System.Collections.Generic.ISet`1":
                case "System.Collections.Generic.IReadOnlySet`1":
                    return Sequence(ShapeKind.HashSet, args[0]);

                case "System.Collections.Frozen.FrozenSet`1":
                    return Sequence(ShapeKind.FrozenSet, args[0]);

                case "System.Collections.Generic.SortedSet`1":
                    return Sequence(ShapeKind.SortedSet, args[0]);

                case "System.Collections.Generic.Queue`1":
                    return Sequence(ShapeKind.Queue, args[0]);

                case "System.Collections.Generic.Stack`1":
                    return Sequence(ShapeKind.Stack, args[0]);

                case "System.Collections.Generic.Dictionary`2":
                case "System.Collections.Generic.IDictionary`2":
                case "System.Collections.Generic.IReadOnlyDictionary`2":
                    return Map(MapKind.None, args[0], args[1]);

                case "System.Collections.Generic.SortedDictionary`2":
                    return Map(MapKind.Sorted, args[0], args[1]);

                case "System.Collections.Generic.OrderedDictionary`2":
                    return Map(MapKind.Ordered, args[0], args[1]);

                case "System.Collections.Immutable.ImmutableDictionary`2":
                    return Map(MapKind.Immutable, args[0], args[1]);

                case "System.Collections.Immutable.ImmutableSortedDictionary`2":
                    return Map(MapKind.ImmutableSorted, args[0], args[1]);

                case "System.Collections.Frozen.FrozenDictionary`2":
                    return Map(MapKind.Frozen, args[0], args[1]);

                case "System.Collections.Concurrent.ConcurrentDictionary`2":
                    return Map(MapKind.Concurrent, args[0], args[1]);
            }
        }

        if ( known.IsModel(type) ) { return Simple(ShapeKind.Model); }

        if ( known.IsParsable(type) ) { return Simple(ShapeKind.Parsable); }

        return TypeShape.Unsupported(name, "it isn't a [GenerateJson] type, a supported BCL type or collection, or ISpanFormattable + ISpanParsable");


        TypeShape Sequence( ShapeKind kind, ITypeSymbol elementType )
        {
            TypeShape element = Analyze(elementType, known, depth + 1);

            return element.Kind == ShapeKind.Unsupported
                       ? element
                       : new TypeShape(kind, name, type.IsReferenceType, nullable, element, null, MapKind.None, null, false, null);
        }

        TypeShape Map( MapKind kind, ITypeSymbol keyType, ITypeSymbol valueType )
        {
            TypeShape key = AnalyzeKey(keyType, known);
            if ( key.Kind == ShapeKind.Unsupported ) { return TypeShape.Unsupported(name, $"its key type '{key.TypeName}' can't be a JSON member name (use string, an enum, or an ISpanFormattable + ISpanParsable type)"); }

            TypeShape value = Analyze(valueType, known, depth + 1);

            return value.Kind == ShapeKind.Unsupported
                       ? value
                       : new TypeShape(ShapeKind.Map, name, true, nullable, value, key, kind, null, false, null) with
                         {
                             Kind = kind == MapKind.None
                                        ? ShapeKind.Dictionary
                                        : ShapeKind.Map
                         };
        }
    }


    /// <summary> Map keys: strings, enums, or ISpanFormattable + ISpanParsable types (integers, Guid, ...). </summary>
    private static TypeShape AnalyzeKey( ITypeSymbol type, KnownTypes known )
    {
        string name = Name(type);

        if ( type.SpecialType == SpecialType.System_String ) { return new TypeShape(ShapeKind.String, name, true, false, null, null, MapKind.None, null, true, null); }

        if ( type.TypeKind == TypeKind.Enum && type is INamedTypeSymbol enumType ) { return new TypeShape(ShapeKind.Enum, name, false, false, null, null, MapKind.None, EnumInfoFor(enumType), true, null); }

        if ( !type.IsReferenceType && type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T && known.IsParsable(type) ) { return new TypeShape(ShapeKind.Parsable, name, false, false, null, null, MapKind.None, null, known.IsComparable(type), null); }

        if ( type.IsReferenceType && known.IsParsable(type) ) { return new TypeShape(ShapeKind.Parsable, name, true, false, null, null, MapKind.None, null, known.IsComparable(type), null); }

        return TypeShape.Unsupported(name, "unsupported key");
    }


    /// <summary> The enum's members with their raw names and any name override; the naming policy is applied when the codec is emitted. </summary>
    public static EnumInfo EnumInfoFor( INamedTypeSymbol type )
    {
        List<EnumMemberInfo> members = [];
        HashSet<ulong>       seen    = [];

        foreach ( IFieldSymbol field in type.GetMembers().OfType<IFieldSymbol>() )
        {
            if ( !field.HasConstantValue || field.ConstantValue is null ) { continue; }

            ulong bits = ToBits(field.ConstantValue);
            if ( !seen.Add(bits) ) { continue; } // an alias of an earlier name: the first name wins, deterministically

            string? overrideName = null;

            foreach ( AttributeData attribute in field.GetAttributes() )
            {
                string? attributeName = attribute.AttributeClass is null
                                            ? null
                                            : MetadataName(attribute.AttributeClass);

                if ( attributeName is "Jakar.Json.JsonEnumNameAttribute" or "System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute" && attribute.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Value is string name )
                {
                    overrideName = name;
                    if ( attributeName == "Jakar.Json.JsonEnumNameAttribute" ) { break; } // ours wins
                }
            }

            // JsonName holds the override (or "" for none) until the naming policy is applied.
            members.Add(new EnumMemberInfo(field.Name, overrideName ?? "", bits));
        }

        bool flags = type.GetAttributes().Any(static a => a.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");

        return new EnumInfo(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), MetadataName(type), type.EnumUnderlyingType!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), flags, 0, new EquatableArray<EnumMemberInfo>(members));
    }

    /// <summary> The enum's names under a naming policy (overrides are kept verbatim). </summary>
    public static EnumInfo WithNaming( EnumInfo info, int naming ) => info with
                                                                      {
                                                                          Naming = naming,
                                                                          Members = new EquatableArray<EnumMemberInfo>(info.Members.Select(m => m.JsonName.Length > 0 && info.Naming == 0
                                                                                                                                                    ? m
                                                                                                                                                    : m with { JsonName = Naming.Apply(m.Name, naming) }))
                                                                      };

    private static ulong ToBits( object value ) => value switch
                                                   {
                                                       sbyte v  => unchecked ((ulong)v),
                                                       short v  => unchecked ((ulong)v),
                                                       int v    => unchecked ((ulong)v),
                                                       long v   => unchecked ((ulong)v),
                                                       byte v   => v,
                                                       ushort v => v,
                                                       uint v   => v,
                                                       ulong v  => v,
                                                       _        => 0
                                                   };


    /// <summary> <c>Ns.Outer+Name`1</c>, as <c>Compilation.GetTypeByMetadataName</c> expects. </summary>
    public static string MetadataName( ITypeSymbol symbol )
    {
        string name = symbol.MetadataName;

        for ( INamedTypeSymbol? current = symbol.ContainingType; current is not null; current = current.ContainingType ) { name = $"{current.MetadataName}+{name}"; }

        return symbol.ContainingNamespace is { IsGlobalNamespace: false } ns
                   ? $"{ns.ToDisplayString()}.{name}"
                   : name;
    }



    private readonly struct ImmutableArrayOf( System.Collections.Immutable.ImmutableArray<ITypeSymbol> items )
    {
        public ITypeSymbol this[ int index ] => items[index];
    }
}
