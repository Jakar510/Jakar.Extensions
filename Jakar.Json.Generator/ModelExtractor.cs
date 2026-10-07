// Jakar.Json.Generator
// 10/07/2026

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Jakar.Json.Generator;


internal static class ModelExtractor
{
    private const string STJ = "System.Text.Json.Serialization.";


    public static ModelInfo Extract( GeneratorAttributeSyntaxContext ctx, CancellationToken token )
    {
        INamedTypeSymbol      symbol            = (INamedTypeSymbol)ctx.TargetSymbol;
        TypeDeclarationSyntax declaration       = (TypeDeclarationSyntax)ctx.TargetNode;
        AttributeData         attribute         = ctx.Attributes[0];
        KnownTypes            known             = new(ctx.SemanticModel.Compilation);
        LocationInfo?         attributeLocation = LocationInfo.From(attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation() ?? declaration.Identifier.GetLocation());
        string                displayName       = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        List<DiagnosticInfo> diagnostics = [];
        bool                 fatal       = false;

        // JJSON001: the type and every containing type must be partial.
        for ( INamedTypeSymbol? current = symbol; current is not null; current = current.ContainingType )
        {
            if ( IsPartial(current, token) ) { continue; }

            diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.NotPartial, attributeLocation, current.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            fatal = true;
            break;
        }

        // JJSON010: Jakar.SystemTextJson's generator would emit the same helpers.
        if ( symbol.GetAttributes().Any(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == "Jakar.Extensions.JsonModelAttribute") )
        {
            diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.BothGenerators, attributeLocation, displayName));
            fatal = true;
        }

        if ( symbol.IsStatic || symbol.TypeKind is not (TypeKind.Class or TypeKind.Struct) || symbol.IsRefLikeType )
        {
            diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.InvalidMemberType, attributeLocation, displayName, displayName, "only classes, structs and records can be [GenerateJson] types"));
            fatal = true;
        }

        Settings settings      = Settings.From(attribute);
        string   discriminator = attribute.NamedArguments.FirstOrDefault(static a => a.Key == "Discriminator").Value.Value as string ?? "$type";

        // ─── Members ─────────────────────────────────────────────────────────
        List<MemberInfo> members = CollectMembers(symbol, known, diagnostics, ref fatal, token);

        // ─── Construction ────────────────────────────────────────────────────
        ConstructionInfo construction = symbol.IsAbstract
                                            ? new ConstructionInfo(ConstructionKind.None, EquatableArray<ParameterInfo>.Empty, false)
                                            : ChooseConstructor(symbol, declaration, members, attributeLocation, diagnostics, ref fatal, token);

        // Members bound to constructor parameters are read even without a setter; the rest need one.
        HashSet<string> bound = new(construction.Parameters.Where(static p => p.Member is not null).Select(static p => p.Member!), StringComparer.Ordinal);

        foreach ( MemberInfo member in members )
        {
            if ( member.Setter is SetterKind.None or SetterKind.ReadOnlyField && !bound.Contains(member.Name) && !member.IsExtensionData ) { diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.NotRead, member.Location ?? attributeLocation, member.Name)); }
        }

        // JJSON006: members of one declaring type spread over several files, with no explicit order.
        foreach ( IGrouping<string, MemberInfo> group in members.GroupBy(static m => m.DeclaringType) )
        {
            if ( group.Select(static m => m.FilePath).Distinct().Count() < 2 || group.All(static m => m.HasOrder) ) { continue; }

            DiagnosticInfo info = DiagnosticInfo.Create(JsonDiagnostics.OrderAmbiguous, attributeLocation, displayName) with { Properties = new EquatableArray<DiagnosticProperty>([new DiagnosticProperty(JsonDiagnostics.MEMBERS, string.Join(",", group.Where(static m => !m.HasOrder).Select(static m => m.Name)))]) };

            diagnostics.Add(info);
            break;
        }

        // ─── Polymorphism ────────────────────────────────────────────────────
        List<DerivedInfo> derived = CollectDerived(symbol, known, attributeLocation, diagnostics, ref fatal);
        DerivedInfo?      ownTag  = FindOwnTag(symbol);

        bool hasSerializableBase = false;

        for ( INamedTypeSymbol? current = symbol.BaseType; current is not null; current = current.BaseType )
        {
            if ( known.IsModel(current) ) { hasSerializableBase = true; }
        }

        token.ThrowIfCancellationRequested();

        return new ModelInfo
               {
                   Name               = symbol.Name,
                   FullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                   HintName           = HintName(symbol),
                   Namespace = symbol.ContainingNamespace is { IsGlobalNamespace: false } ns
                                   ? ns.ToDisplayString()
                                   : null,
                   ContainingTypes     = new EquatableArray<TypeDeclInfo>(GetContainingTypes(symbol, token)),
                   Self                = GetDeclInfo(symbol, token),
                   IsValueType         = symbol.IsValueType,
                   IsAbstract          = symbol.IsAbstract,
                   IsSealed            = symbol.IsSealed || symbol.IsValueType,
                   Settings            = settings,
                   Members             = new EquatableArray<MemberInfo>(members),
                   Construction        = construction,
                   Discriminator       = discriminator,
                   Derived             = new EquatableArray<DerivedInfo>(derived),
                   OwnTag              = ownTag,
                   HasSerializableBase = hasSerializableBase,
                   Skip                = new EquatableArray<string>(FindHandWritten(symbol, attributeLocation, diagnostics)),
                   OverridesToString   = OverridesToString(symbol),
                   AttributeLocation   = attributeLocation,
                   Diagnostics         = new EquatableArray<DiagnosticInfo>(diagnostics),
                   IsFatal             = fatal
               };
    }


    // ─── Members ─────────────────────────────────────────────────────────────

    private static List<MemberInfo> CollectMembers( INamedTypeSymbol symbol, KnownTypes known, List<DiagnosticInfo> diagnostics, ref bool fatal, CancellationToken token )
    {
        // Most-derived first, so an override or 'new' member hides the base one; BaseDepth orders base members first in the output.
        List<INamedTypeSymbol> chain = [];

        for ( INamedTypeSymbol? current = symbol; current is not null && current.SpecialType is not (SpecialType.System_Object or SpecialType.System_ValueType); current = current.BaseType ) { chain.Add(current); }

        List<MemberInfo> members = [];
        HashSet<string>  names   = new(StringComparer.Ordinal);

        for ( int level = 0; level < chain.Count; level++ )
        {
            INamedTypeSymbol type      = chain[level];
            int              baseDepth = chain.Count - 1 - level;
            bool             own       = level == 0;

            foreach ( ISymbol candidate in type.GetMembers() )
            {
                token.ThrowIfCancellationRequested();

                // Positional record properties are compiler-declared, but they're members like any other; implicit fields (backing fields) aren't.
                if ( candidate.IsStatic || candidate is not (IPropertySymbol or IFieldSymbol) || candidate.IsImplicitlyDeclared && candidate is not IPropertySymbol || !names.Add(candidate.Name) ) { continue; }

                MemberInfo? member = candidate switch
                                     {
                                         IPropertySymbol { IsIndexer: false, ExplicitInterfaceImplementations.Length: 0 } property => FromProperty(property, own, baseDepth, known, diagnostics, ref fatal),
                                         IFieldSymbol { IsConst     : false, AssociatedSymbol                       : null } field => FromField(field, own, baseDepth, known, diagnostics, ref fatal),
                                         _                                                                                         => null
                                     };

                if ( member is not null ) { members.Add(member); }
            }
        }

        return members;
    }

    private static MemberInfo? FromProperty( IPropertySymbol property, bool own, int baseDepth, KnownTypes known, List<DiagnosticInfo> diagnostics, ref bool fatal )
    {
        Attributes attributes = Attributes.Read(property);
        if ( attributes.Ignore || !Included(property, attributes, own) ) { return null; }

        bool canGet = property.GetMethod is { } getter && Accessible(getter, own);

        SetterKind setter = property.SetMethod is { } set && Accessible(set, own)
                                ? set.IsInitOnly
                                      ? SetterKind.Init
                                      : SetterKind.Set
                                : SetterKind.None;

        return Build(property, property.Type, canGet, setter, property.IsRequired, attributes, baseDepth, known, diagnostics, ref fatal);
    }

    private static MemberInfo? FromField( IFieldSymbol field, bool own, int baseDepth, KnownTypes known, List<DiagnosticInfo> diagnostics, ref bool fatal )
    {
        Attributes attributes = Attributes.Read(field);
        if ( attributes.Ignore || !Included(field, attributes, own) ) { return null; }

        SetterKind setter = field.IsReadOnly
                                ? SetterKind.ReadOnlyField
                                : SetterKind.Field;

        return Build(field, field.Type, true, setter, field.IsRequired, attributes, baseDepth, known, diagnostics, ref fatal);
    }

    /// <summary> Public members, plus any member marked [JsonMember] / [JsonPropertyName] / [JsonInclude] that generated code can reach. </summary>
    private static bool Included( ISymbol member, in Attributes attributes, bool own )
    {
        if ( member.DeclaredAccessibility == Accessibility.Public ) { return true; }

        if ( !attributes.Marked ) { return false; }

        return own || member.DeclaredAccessibility != Accessibility.Private;
    }

    private static bool Accessible( IMethodSymbol accessor, bool own ) => own || accessor.DeclaredAccessibility != Accessibility.Private;

    private static MemberInfo Build( ISymbol member, ITypeSymbol type, bool canGet, SetterKind setter, bool requiredKeyword, in Attributes attributes, int baseDepth, KnownTypes known, List<DiagnosticInfo> diagnostics, ref bool fatal )
    {
        Location?     location  = member.Locations.FirstOrDefault(static l => l.IsInSource);
        LocationInfo? info      = LocationInfo.From(location);
        TypeShape     shape     = TypeShapes.Analyze(type, known);
        string?       converter = null;

        if ( attributes.Converter is not null )
        {
            converter = ResolveConverter(attributes.Converter, type, known);
            if ( converter is null ) { diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.InvalidConverter, info, attributes.Converter.ToDisplayString(), member.Name, TypeShapes.Name(type))); }
        }
        else if ( attributes.ExtensionData )
        {
            if ( !IsExtensionDataType(type, known) )
            {
                diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.Contradictory, info, $"Extension data member '{member.Name}' must be an OrderedDictionary<string, JNode?> or a JObjectNode"));
                fatal = true;
            }
        }
        else if ( shape.Kind == ShapeKind.Unsupported )
        {
            string reason = shape.Reason ?? "";

            if ( reason == "TYPE_PARAMETER" ) { diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.GenericUnsupported,                                  info, member.Name, shape.TypeName)); }
            else if ( reason.StartsWith("INVALID:", StringComparison.Ordinal) ) { diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.InvalidMemberType, info, member.Name, shape.TypeName, reason.Substring(8))); }
            else { diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.UnsupportedMember,                                                                info, member.Name, shape.TypeName, reason)); }

            fatal = true;
        }

        return new MemberInfo
               {
                   Name                 = member.Name,
                   TypeName             = type.ToDisplayString(TypeShapes.Format),
                   Shape                = shape,
                   JsonNameOverride     = attributes.Name,
                   Order                = attributes.Order ?? 0,
                   HasOrder             = attributes.Order.HasValue,
                   CanGet               = canGet,
                   Setter               = setter,
                   IsRequiredKeyword    = requiredKeyword,
                   RequiredAttribute    = attributes.Required,
                   Converter            = converter,
                   IsExtensionData      = attributes.ExtensionData,
                   Settings             = attributes.Settings,
                   BaseDepth            = baseDepth,
                   FilePath             = location?.SourceTree?.FilePath ?? "",
                   Position             = location?.SourceSpan.Start     ?? 0,
                   DeclaringType        = member.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                   DeclaringIsValueType = member.ContainingType.IsValueType,
                   Location             = info
               };
    }

    private static bool IsExtensionDataType( ITypeSymbol type, KnownTypes known )
    {
        ITypeSymbol plain = type.WithNullableAnnotation(NullableAnnotation.None);
        if ( SymbolEqualityComparer.Default.Equals(plain, known.JObjectNode) ) { return true; }

        return plain is INamedTypeSymbol { IsGenericType: true } named && SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, known.OrderedDictionary) && named.TypeArguments[0].SpecialType == SpecialType.System_String && SymbolEqualityComparer.Default.Equals(named.TypeArguments[1].WithNullableAnnotation(NullableAnnotation.None), known.JNode);
    }

    /// <summary> The converter expression for a member: the converter itself, or wrapped for <c>T?</c> when it converts <c>T</c>. <see langword="null"/> when it fits neither. </summary>
    private static string? ResolveConverter( INamedTypeSymbol converter, ITypeSymbol memberType, KnownTypes known )
    {
        string name = converter.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if ( known.Implements(converter, known.Converter, memberType) ) { return name; }

        if ( memberType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable && known.Implements(converter, known.Converter, nullable.TypeArguments[0]) ) { return $"global::Jakar.Json.Converters.JsonNullableConverter<{TypeShapes.Name(nullable.TypeArguments[0])}, {name}>"; }

        return null;
    }



    /// <summary> The attributes Jakar.Json reads on a member: its own, plus System.Text.Json's for migration (SPEC.md §4.4). </summary>
    private readonly struct Attributes
    {
        public bool              Ignore        { get; private init; }
        public bool              Marked        { get; private init; }
        public string?           Name          { get; private init; }
        public int?              Order         { get; private init; }
        public bool              Required      { get; private init; }
        public bool              ExtensionData { get; private init; }
        public INamedTypeSymbol? Converter     { get; private init; }
        public Settings          Settings      { get; private init; }


        public static Attributes Read( ISymbol member )
        {
            Attributes result = default;
            bool       ours   = false;

            foreach ( AttributeData attribute in member.GetAttributes() )
            {
                string? name = attribute.AttributeClass is null
                                   ? null
                                   : TypeShapes.MetadataName(attribute.AttributeClass);

                switch ( name )
                {
                    case JsonGenerator.JSON_MEMBER_ATTRIBUTE:
                        ours = true;

                        result = result with
                                 {
                                     Marked = true,
                                     Settings = Settings.From(attribute)
                                 };

                        foreach ( KeyValuePair<string, TypedConstant> argument in attribute.NamedArguments )
                        {
                            result = argument.Key switch
                                     {
                                         "Name" => result with { Name = argument.Value.Value as string },
                                         "Order" => result with
                                                    {
                                                        Order = argument.Value.Value is int order
                                                                    ? order
                                                                    : 0
                                                    },
                                         "Ignore"        => result with { Ignore = argument.Value.Value is true },
                                         "Required"      => result with { Required = argument.Value.Value is true },
                                         "ExtensionData" => result with { ExtensionData = argument.Value.Value is true },
                                         "Converter"     => result with { Converter = argument.Value.Value as INamedTypeSymbol },
                                         _               => result
                                     };
                        }

                        break;

                    // System.Text.Json attributes: honored unless [JsonMember] says otherwise.
                    case STJ + "JsonPropertyNameAttribute" when attribute.ConstructorArguments.Length == 1:
                        result = result with
                                 {
                                     Marked = true,
                                     Name = result.Name ?? attribute.ConstructorArguments[0].Value as string
                                 };

                        break;

                    case STJ + "JsonPropertyOrderAttribute" when attribute.ConstructorArguments.Length == 1:
                        result = result with { Order = result.Order ?? attribute.ConstructorArguments[0].Value as int? };
                        break;

                    case STJ + "JsonRequiredAttribute":
                        result = result with { Required = true };
                        break;

                    case STJ + "JsonExtensionDataAttribute":
                        result = result with
                                 {
                                     Marked = true,
                                     ExtensionData = true
                                 };

                        break;

                    case STJ + "JsonIncludeAttribute":
                        result = result with { Marked = true };
                        break;

                    case STJ + "JsonIgnoreAttribute" when !ours:
                        // JsonIgnoreCondition: Never = 0, Always = 1, WhenWritingDefault = 2, WhenWritingNull = 3.
                        int condition = attribute.NamedArguments.FirstOrDefault(static a => a.Key == "Condition").Value.Value is int c
                                            ? c
                                            : 1;

                        result = condition switch
                                 {
                                     1 => result with { Ignore = true },
                                     2 => result with { Settings = result.Settings with { DefaultValues = Values.DEFAULTS_OMIT } },
                                     3 => result with { Settings = result.Settings with { NullValues = Values.NULLS_OMIT } },
                                     _ => result
                                 };

                        break;
                }
            }

            // [JsonMember(Ignore = false)] overrides an STJ [JsonIgnore]; [JsonMember(Ignore = true)] always wins.
            if ( ours && !member.GetAttributes().Any(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == JsonGenerator.JSON_MEMBER_ATTRIBUTE && a.NamedArguments.Any(static n => n.Key == "Ignore" && n.Value.Value is true)) ) { result = result with { Ignore = false }; }

            return result;
        }
    }



    // ─── Construction ────────────────────────────────────────────────────────

    private static ConstructionInfo ChooseConstructor( INamedTypeSymbol symbol, TypeDeclarationSyntax declaration, List<MemberInfo> members, LocationInfo? location, List<DiagnosticInfo> diagnostics, ref bool fatal, CancellationToken token )
    {
        ImmutableArray<IMethodSymbol> constructors = symbol.InstanceConstructors;
        bool                          isRecord     = symbol.IsRecord;

        bool IsCopyConstructor( IMethodSymbol ctor ) => isRecord && ctor.Parameters.Length == 1 && SymbolEqualityComparer.Default.Equals(ctor.Parameters[0].Type.WithNullableAnnotation(NullableAnnotation.None), symbol.WithNullableAnnotation(NullableAnnotation.None));

        IMethodSymbol? chosen = constructors.FirstOrDefault(static c => c.GetAttributes().Any(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == STJ + "JsonConstructorAttribute"));

        // The primary constructor: declared by the type declaration itself.
        chosen ??= constructors.FirstOrDefault(c => !c.IsImplicitlyDeclared && c.DeclaringSyntaxReferences.Any(r => r.GetSyntax(token) is TypeDeclarationSyntax));

        if ( chosen is null )
        {
            IMethodSymbol[] accessible = constructors.Where(c => c.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal && !IsCopyConstructor(c) && !c.IsImplicitlyDeclared).ToArray();

            chosen = accessible.Length == 1
                         ? accessible[0]
                         : constructors.FirstOrDefault(static c => c.Parameters.Length == 0 && c.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal);
        }

        if ( chosen is null || chosen.Parameters.Length == 0 && chosen.IsImplicitlyDeclared && symbol.IsValueType )
        {
            if ( symbol.IsValueType ) { return new ConstructionInfo(ConstructionKind.Default, EquatableArray<ParameterInfo>.Empty, false); }

            if ( chosen is null )
            {
                diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.NoConstructor, location, $"'{symbol.Name}' has no constructor [GenerateJson] can use: add a parameterless one, a single public one, or mark one [JsonConstructor]"));
                fatal = true;
                return new ConstructionInfo(ConstructionKind.None, EquatableArray<ParameterInfo>.Empty, false);
            }
        }

        bool                setsRequired = chosen.GetAttributes().Any(static a => a.AttributeClass?.Name == "SetsRequiredMembersAttribute");
        List<ParameterInfo> parameters   = [];

        foreach ( IParameterSymbol parameter in chosen.Parameters )
        {
            MemberInfo? member = members.FirstOrDefault(m => string.Equals(m.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

            if ( member is not null && !SameType(member, parameter) ) { member = null; }

            string? defaultLiteral = parameter.HasExplicitDefaultValue
                                         ? DefaultLiteral(parameter)
                                         : null;

            if ( member is null && !parameter.HasExplicitDefaultValue )
            {
                diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.NoConstructor, location, $"Constructor parameter '{parameter.Name}' of '{symbol.Name}' matches no member of the same type, so it can't be read from JSON; add a matching property, give it a default value, or mark another constructor [JsonConstructor]"));
                fatal = true;
            }

            parameters.Add(new ParameterInfo(parameter.Name, parameter.Type.ToDisplayString(TypeShapes.Format), defaultLiteral, parameter.HasExplicitDefaultValue, member?.Name));
        }

        return new ConstructionInfo(ConstructionKind.Constructor, new EquatableArray<ParameterInfo>(parameters), setsRequired);


        static bool SameType( MemberInfo member, IParameterSymbol parameter ) => TypeShapes.Name(parameter.Type) == member.Shape.TypeName || parameter.Type.ToDisplayString(TypeShapes.Format) == member.TypeName;
    }

    private static string DefaultLiteral( IParameterSymbol parameter )
    {
        object? value = parameter.ExplicitDefaultValue;
        string  type  = TypeShapes.Name(parameter.Type);

        if ( value is null ) { return "default!"; }

        if ( parameter.Type.TypeKind == TypeKind.Enum || parameter.Type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableEnum && nullableEnum.TypeArguments[0].TypeKind == TypeKind.Enum ) { return $"({type})({SymbolDisplay.FormatPrimitive(value, false, false)})"; }

        return value switch
               {
                   string text => SymbolDisplay.FormatLiteral(text, true),
                   char c      => SymbolDisplay.FormatLiteral(c,    true),
                   bool b => b
                                 ? "true"
                                 : "false",
                   _ => $"({type})({Convert.ToString(value, CultureInfo.InvariantCulture)}{( value is double or float ? "d" : value is decimal ? "m" : "" )})"
               };
    }


    // ─── Polymorphism ────────────────────────────────────────────────────────

    private static List<DerivedInfo> CollectDerived( INamedTypeSymbol symbol, KnownTypes known, LocationInfo? location, List<DiagnosticInfo> diagnostics, ref bool fatal )
    {
        List<(INamedTypeSymbol Type, string Tag)> found = [];

        foreach ( AttributeData attribute in symbol.GetAttributes() )
        {
            if ( attribute.AttributeClass is null || TypeShapes.MetadataName(attribute.AttributeClass) != JsonGenerator.JSON_DERIVED_ATTRIBUTE || attribute.ConstructorArguments.Length != 2 ) { continue; }

            if ( attribute.ConstructorArguments[0].Value is not INamedTypeSymbol derivedType || attribute.ConstructorArguments[1].Value is not string tag ) { continue; }

            if ( !DerivesFrom(derivedType, symbol) )
            {
                diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.InvalidDerived, location, $"[JsonDerived(typeof({derivedType.Name}), \"{tag}\")] on '{symbol.Name}': '{derivedType.Name}' doesn't derive from '{symbol.Name}'"));
                fatal = true;
                continue;
            }

            if ( !known.IsModel(derivedType) )
            {
                diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.InvalidDerived, location, $"[JsonDerived(typeof({derivedType.Name}), \"{tag}\")] on '{symbol.Name}': '{derivedType.Name}' needs [GenerateJson] too"));
                fatal = true;
                continue;
            }

            if ( found.Any(f => f.Tag == tag) )
            {
                diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.InvalidDerived, location, $"'{symbol.Name}' uses the discriminator \"{tag}\" twice"));
                fatal = true;
                continue;
            }

            found.Add(( derivedType, tag ));
        }

        // Most-derived first, so 'value switch' tests the narrowest type first.
        return found.OrderByDescending(static f => Depth(f.Type)).Select(static f => new DerivedInfo(f.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), f.Tag)).ToList();

        static int Depth( INamedTypeSymbol type )
        {
            int depth = 0;
            for ( INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType ) { depth++; }

            return depth;
        }
    }

    private static bool DerivesFrom( INamedTypeSymbol type, INamedTypeSymbol baseType )
    {
        for ( INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType )
        {
            if ( SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, baseType.OriginalDefinition) ) { return true; }
        }

        return false;
    }

    /// <summary> The nearest base type that lists this type in a [JsonDerived]: (that base's discriminator, this type's tag). </summary>
    private static DerivedInfo? FindOwnTag( INamedTypeSymbol symbol )
    {
        for ( INamedTypeSymbol? current = symbol.BaseType; current is not null; current = current.BaseType )
        {
            foreach ( AttributeData attribute in current.GetAttributes() )
            {
                if ( attribute.AttributeClass is null || TypeShapes.MetadataName(attribute.AttributeClass) != JsonGenerator.JSON_DERIVED_ATTRIBUTE || attribute.ConstructorArguments.Length != 2 ) { continue; }

                if ( attribute.ConstructorArguments[0].Value is INamedTypeSymbol listed && SymbolEqualityComparer.Default.Equals(listed, symbol) && attribute.ConstructorArguments[1].Value is string tag )
                {
                    AttributeData? generate      = current.GetAttributes().FirstOrDefault(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == JsonGenerator.GENERATE_JSON_ATTRIBUTE);
                    string         discriminator = generate?.NamedArguments.FirstOrDefault(static a => a.Key == "Discriminator").Value.Value as string ?? "$type";
                    return new DerivedInfo(discriminator, tag);
                }
            }
        }

        return null;
    }


    // ─── Hand-written members ────────────────────────────────────────────────

    /// <summary> Generated members the type already declares itself: (key, display name). Keys are matched by the emitter. </summary>
    private static List<string> FindHandWritten( INamedTypeSymbol symbol, LocationInfo? location, List<DiagnosticInfo> diagnostics )
    {
        List<string> skip = [];

        foreach ( IMethodSymbol method in symbol.GetMembers().OfType<IMethodSymbol>() )
        {
            if ( method.IsImplicitlyDeclared || method.MethodKind != MethodKind.Ordinary ) { continue; }

            string? key = Key(method);
            if ( key is null || skip.Contains(key) ) { continue; }

            skip.Add(key);
            diagnostics.Add(DiagnosticInfo.Create(JsonDiagnostics.HandWritten, location, symbol.Name, method.Name));
        }

        foreach ( IPropertySymbol property in symbol.GetMembers().OfType<IPropertySymbol>() )
        {
            if ( property is { IsStatic: true, Name: "DefaultWriterOptions" or "DefaultReaderOptions" } && !property.IsImplicitlyDeclared ) { skip.Add(property.Name); }
        }

        return skip;


        static string? Key( IMethodSymbol method )
        {
            string first = method.Parameters.Length > 0
                               ? ParameterKind(method.Parameters[0].Type)
                               : "";

            return ( method.Name, method.IsStatic, method.Parameters.Length, first ) switch
                   {
                       ("WriteJson", true, 2, _)          => "WriteJson",
                       ("TryReadJson", true, 2, _)        => "TryReadJson",
                       ("TryFormat", false, 4, "chars")   => "TryFormat:chars",
                       ("TryFormat", false, 4, "bytes")   => "TryFormat:bytes",
                       ("ToString", false, 2, "string")   => "ToString:format",
                       ("Parse", true, 2, "chars")        => "Parse:chars",
                       ("Parse", true, 2, "string")       => "Parse:string",
                       ("Parse", true, 2, "bytes")        => "Parse:bytes",
                       ("TryParse", true, 3, "chars")     => "TryParse:chars",
                       ("TryParse", true, 3, "string")    => "TryParse:string",
                       ("TryParse", true, 3, "bytes")     => "TryParse:bytes",
                       ("ToJson", false, _, "")           => "ToJson",
                       ("ToJson", false, 1, "options")    => "ToJson",
                       ("ToJson", false, _, "stream")     => "ToJson:stream",
                       ("ToJsonUtf8", false, _, _)        => "ToJsonUtf8",
                       ("ToJsonAsync", false, _, _)       => "ToJsonAsync",
                       ("WriteJson", false, _, _)         => "WriteJson:buffer",
                       ("FromJson", true, _, "string")    => "FromJson:string",
                       ("FromJson", true, _, "chars")     => "FromJson:chars",
                       ("FromJson", true, _, "bytes")     => "FromJson:bytes",
                       ("FromJson", true, _, "stream")    => "FromJson:stream",
                       ("FromJsonAsync", true, _, _)      => "FromJsonAsync",
                       ("TryFromJson", true, _, "string") => "TryFromJson:string",
                       ("TryFromJson", true, _, "chars")  => "TryFromJson:chars",
                       ("TryFromJson", true, _, "bytes")  => "TryFromJson:bytes",
                       _                                  => null
                   };
        }

        static string ParameterKind( ITypeSymbol type )
        {
            if ( type.SpecialType == SpecialType.System_String ) { return "string"; }

            if ( type is not INamedTypeSymbol named ) { return "other"; }

            ITypeSymbol? argument = named.TypeArguments.Length == 1
                                        ? named.TypeArguments[0]
                                        : null;

            return named.Name switch
                   {
                       "ReadOnlySpan" or "Span" when argument?.SpecialType == SpecialType.System_Char => "chars",
                       "ReadOnlySpan" or "Span" when argument?.SpecialType == SpecialType.System_Byte => "bytes",
                       "Stream"                                                                       => "stream",
                       "Nullable" when argument?.Name == "JsonWriterOptions"                          => "options",
                       _                                                                              => "other"
                   };
        }
    }

    private static bool OverridesToString( INamedTypeSymbol symbol )
    {
        if ( symbol.GetMembers("ToString").Any(static m => m is IMethodSymbol { Parameters.Length: 0, IsImplicitlyDeclared: false }) ) { return true; }

        for ( INamedTypeSymbol? current = symbol.BaseType; current is not null && current.SpecialType is not (SpecialType.System_Object or SpecialType.System_ValueType); current = current.BaseType )
        {
            if ( current.GetMembers("ToString").Any(static m => m is IMethodSymbol { Parameters.Length: 0, IsSealed: true }) ) { return true; }
        }

        return false;
    }


    // ─── Declarations ────────────────────────────────────────────────────────

    private static bool IsPartial( INamedTypeSymbol symbol, CancellationToken token )
    {
        foreach ( SyntaxReference reference in symbol.DeclaringSyntaxReferences )
        {
            if ( reference.GetSyntax(token) is not TypeDeclarationSyntax declaration || !declaration.Modifiers.Any(SyntaxKind.PartialKeyword) ) { return false; }
        }

        return symbol.DeclaringSyntaxReferences.Length > 0;
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

        string parameters = symbol.TypeParameters.Length == 0
                                ? ""
                                : $"<{string.Join(", ", symbol.TypeParameters.Select(static p => p.Name))}>";

        bool isReadOnly = declaration?.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) ?? false;
        bool isRef      = declaration?.Modifiers.Any(SyntaxKind.RefKeyword)      ?? false;
        return new TypeDeclInfo(keyword, symbol.Name, parameters, isReadOnly, isRef);
    }

    private static string HintName( INamedTypeSymbol symbol )
    {
        char[] chars = TypeShapes.MetadataName(symbol).ToCharArray();

        for ( int i = 0; i < chars.Length; i++ )
        {
            if ( !char.IsLetterOrDigit(chars[i]) && chars[i] != '.' ) { chars[i] = '_'; }
        }

        return new string(chars) + ".Json.g.cs";
    }
}
