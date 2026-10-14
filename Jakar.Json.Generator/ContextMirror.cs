// Jakar.Json.Generator
// 10/13/2026

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;



namespace Jakar.Json.Generator;


/// <summary>
///     A context-backed <c>[JsonModel]</c> mirrors its System.Text.Json context so Jakar.Json's JSON means what System.Text.Json's would (SPEC.md §7.7):
///     the context's <c>[JsonSourceGenerationOptions]</c> become a settings layer between the type's own settings and the assembly defaults.
/// </summary>
/// <param name="Settings"> The mirrored layer. </param>
/// <param name="IncludeFields"> System.Text.Json serializes public fields only with <c>IncludeFields</c> (or <c>[JsonInclude]</c>). </param>
/// <param name="OptionalConstructorParameters"> System.Text.Json fills a missing constructor parameter with its default unless <c>RespectRequiredConstructorParameters</c>. </param>
/// <param name="ConverterTargets"> Types the context's <c>Converters</c> handle: Jakar.Json can't run those converters, so members of these types make the model fall back. </param>
/// <param name="Blocker"> An option Jakar.Json has no equivalent for, or <see langword="null"/>. </param>
internal sealed record ContextMirror( Settings Settings, bool IncludeFields, bool OptionalConstructorParameters, IReadOnlyCollection<ITypeSymbol> ConverterTargets, string? Blocker )
{
    private const string OPTIONS_ATTRIBUTE   = "System.Text.Json.Serialization.JsonSourceGenerationOptionsAttribute";
    private const string CONVERTER_GENERIC   = "System.Text.Json.Serialization.JsonConverter`1";
    private const string STRING_ENUM_FACTORY = "System.Text.Json.Serialization.JsonStringEnumConverter";
    private const string STRING_ENUM_GENERIC = "System.Text.Json.Serialization.JsonStringEnumConverter`1";


    /// <summary> No context: Jakar.Json's own semantics. </summary>
    public static readonly ContextMirror None = new(default, true, false, [], null);


    /// <summary> A context is mirrored (not <see cref="None"/>). </summary>
    public bool IsActive => !ReferenceEquals(this, None);


    public static ContextMirror Read( INamedTypeSymbol context )
    {
        AttributeData? options = context.GetAttributes().FirstOrDefault(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == OPTIONS_ATTRIBUTE);

        // JsonSerializerDefaults: General = 0, Web = 1 (camelCase, case-insensitive, numbers from strings).
        bool web = options is { ConstructorArguments.Length: 1 } && options.ConstructorArguments[0].Value is 1;

        Settings settings = new()
                            {
                                Naming = web
                                             ? Values.NAMING_CAMEL
                                             : Values.NAMING_AS_DECLARED,
                                NameMatching = web
                                                   ? Values.MATCHING_IGNORE_CASE
                                                   : 1,
                                NullValues       = 1, // Write
                                DefaultValues    = 1, // Write
                                UnknownMembers   = Values.UNKNOWN_SKIP,
                                DuplicateMembers = Values.DUPLICATES_LAST_WINS,
                                Enums            = Values.ENUMS_NUMBER,
                                EnumNaming       = Values.NAMING_AS_DECLARED,
                                NumbersFromStrings = web
                                                         ? Values.NUMBERS_ALLOW_STRINGS
                                                         : 1,
                                LargeIntegers       = 1, // Number
                                NonFiniteFloats     = 1, // Error
                                Indented            = Values.OFF,
                                IndentChar          = Values.INDENT_SPACE,
                                IndentSize          = 2,
                                AllowComments       = Values.OFF,
                                AllowTrailingCommas = Values.OFF,
                                GenerateToString    = Values.OFF // System.Text.Json never overrides ToString(); [JsonModel(GenerateToString = On)] still can
                            };

        bool              includeFields = false;
        bool              optionalCtor  = true;
        List<ITypeSymbol> targets       = [];
        string?           blocker       = null;

        if ( options is null ) { return new ContextMirror(settings, includeFields, optionalCtor, targets, blocker); }

        foreach ( KeyValuePair<string, TypedConstant> argument in options.NamedArguments )
        {
            object? value = argument.Value.Kind == TypedConstantKind.Array
                                ? null
                                : argument.Value.Value;

            switch ( argument.Key )
            {
                case "WriteIndented":
                    settings = settings with
                               {
                                   Indented = value is true
                                                  ? Values.ON
                                                  : Values.OFF
                               };

                    break;

                case "IndentCharacter" when value is char c:
                    settings = settings with
                               {
                                   IndentChar = c == '\t'
                                                    ? 1
                                                    : Values.INDENT_SPACE
                               };

                    break;

                case "IndentSize" when value is int size && size > 0:
                    settings = settings with { IndentSize = size };
                    break;

                case "PropertyNameCaseInsensitive":
                    settings = settings with
                               {
                                   NameMatching = value is true
                                                      ? Values.MATCHING_IGNORE_CASE
                                                      : 1
                               };

                    break;

                case "PropertyNamingPolicy" when value is int policy:
                    // JsonKnownNamingPolicy: Unspecified, CamelCase, SnakeCaseLower, SnakeCaseUpper, KebabCaseLower, KebabCaseUpper.
                    settings = settings with
                               {
                                   Naming = policy switch
                                            {
                                                1 => Values.NAMING_CAMEL,
                                                2 => Values.NAMING_SNAKE_LOWER,
                                                3 => Values.NAMING_SNAKE_UPPER,
                                                4 => Values.NAMING_KEBAB_LOWER,
                                                5 => Values.NAMING_KEBAB_UPPER,
                                                _ => settings.Naming
                                            }
                               };

                    break;

                case "DictionaryKeyPolicy" when value is int policy && policy != 0:
                    blocker ??= "the context's DictionaryKeyPolicy renames dictionary keys, which Jakar.Json doesn't";
                    break;

                case "DefaultIgnoreCondition" when value is int condition:
                    // JsonIgnoreCondition: Never = 0, Always = 1, WhenWritingDefault = 2, WhenWritingNull = 3.
                    settings = condition switch
                               {
                                   2 => settings with
                                        {
                                            DefaultValues = Values.DEFAULTS_OMIT,
                                            NullValues = Values.NULLS_OMIT
                                        },
                                   3 => settings with { NullValues = Values.NULLS_OMIT },
                                   _ => settings
                               };

                    break;

                case "IgnoreReadOnlyProperties" or "IgnoreReadOnlyFields" when value is true:
                    blocker ??= $"the context's {argument.Key} drops members Jakar.Json writes";
                    break;

                case "IncludeFields":
                    includeFields = value is true;
                    break;

                case "NumberHandling" when value is int handling:
                    // JsonNumberHandling flags: AllowReadingFromString = 1, WriteAsString = 2, AllowNamedFloatingPointLiterals = 4.
                    if ( ( handling & 2 ) != 0 ) { blocker ??= "the context's NumberHandling.WriteAsString writes numbers as strings"; }

                    settings = settings with
                               {
                                   NumbersFromStrings = ( handling & 1 ) != 0
                                                            ? Values.NUMBERS_ALLOW_STRINGS
                                                            : 1,
                                   NonFiniteFloats = ( handling & 4 ) != 0
                                                         ? Values.NON_FINITE_AS_STRING
                                                         : 1
                               };

                    break;

                case "ReadCommentHandling" when value is int comments:
                    // JsonCommentHandling: Disallow = 0, Skip = 1, Allow = 2.
                    settings = settings with
                               {
                                   AllowComments = comments == 0
                                                       ? Values.OFF
                                                       : Values.ON
                               };

                    break;

                case "AllowTrailingCommas":
                    settings = settings with
                               {
                                   AllowTrailingCommas = value is true
                                                             ? Values.ON
                                                             : Values.OFF
                               };

                    break;

                case "AllowDuplicateProperties" when value is false:
                    settings = settings with { DuplicateMembers = Values.DUPLICATES_ERROR };
                    break;

                case "MaxDepth" when value is int depth && depth > 0:
                    settings = settings with { MaxDepth = depth };
                    break;

                case "UseStringEnumConverter" when value is true:
                    settings = settings with { Enums = Values.ENUMS_NAME };
                    break;

                case "UnmappedMemberHandling" when value is int unmapped && unmapped == 1:
                    settings = settings with { UnknownMembers = Values.UNKNOWN_ERROR };
                    break;

                case "PreferredObjectCreationHandling" when value is int creation && creation == 1:
                    blocker ??= "the context populates existing objects (PreferredObjectCreationHandling.Populate)";
                    break;

                case "RespectRequiredConstructorParameters":
                    optionalCtor = value is not true;
                    break;

                case "Converters" when argument.Value.Kind == TypedConstantKind.Array:
                    foreach ( TypedConstant item in argument.Value.Values )
                    {
                        if ( item.Value is not INamedTypeSymbol converter ) { continue; }

                        string name = TypeShapes.MetadataName(converter.OriginalDefinition);

                        // Enums written as names: Jakar.Json's enum setting covers it (and reads both forms).
                        if ( name == STRING_ENUM_FACTORY )
                        {
                            settings = settings with { Enums = Values.ENUMS_NAME };
                            continue;
                        }

                        if ( name == STRING_ENUM_GENERIC ) { continue; }

                        ITypeSymbol? target = ConverterTarget(converter);
                        if ( target is not null ) { targets.Add(target); }
                        else { blocker ??= $"the context's converter '{converter.Name}' is a factory, so the types it handles can't be known"; }
                    }

                    break;
            }
        }

        return new ContextMirror(settings, includeFields, optionalCtor, targets, blocker);
    }


    /// <summary> <c>T</c> of the <c>JsonConverter&lt;T&gt;</c> a converter derives from; <see langword="null"/> for a factory. </summary>
    private static ITypeSymbol? ConverterTarget( INamedTypeSymbol converter )
    {
        for ( INamedTypeSymbol? current = converter; current is not null; current = current.BaseType )
        {
            if ( current.IsGenericType && TypeShapes.MetadataName(current.OriginalDefinition) == CONVERTER_GENERIC ) { return current.TypeArguments[0]; }
        }

        return null;
    }
}
