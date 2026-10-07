// Jakar.Json.Generator
// 10/07/2026

using Microsoft.CodeAnalysis;



namespace Jakar.Json.Generator;


/// <summary> Diagnostic IDs are public API once shipped: never renumber or reuse them (SPEC.md §11). </summary>
public static class JsonDiagnostics
{
    public const string CATEGORY = "Jakar.Json";
    public const string HELP     = "https://github.com/Jakar510/Jakar.Extensions/blob/master/Jakar.Json/README.md#diagnostics";

    public const string NOT_PARTIAL          = "JJSON001";
    public const string GENERIC_UNSUPPORTED  = "JJSON002";
    public const string UNSUPPORTED_MEMBER   = "JJSON003";
    public const string NO_CONSTRUCTOR       = "JJSON004";
    public const string NAME_COLLISION       = "JJSON005";
    public const string ORDER_AMBIGUOUS      = "JJSON006";
    public const string HAND_WRITTEN         = "JJSON007";
    public const string CONTRADICTORY        = "JJSON008";
    public const string INVALID_CONVERTER    = "JJSON009";
    public const string BOTH_GENERATORS      = "JJSON010";
    public const string NOT_READ             = "JJSON011";
    public const string INVALID_MEMBER_TYPE  = "JJSON012";
    public const string DETERMINISM_WEAKENED = "JJSON013";
    public const string INVALID_DERIVED      = "JJSON014";
    public const string MISSING_ATTRIBUTE    = "JJSON015";

    /// <summary> Diagnostic property for code fixes: the member names that need an <c>Order</c>, comma separated. </summary>
    public const string MEMBERS = "Members";


    private static DiagnosticDescriptor Create( string id, string title, string message, DiagnosticSeverity severity, string? description = null ) => new(id, title, message, CATEGORY, severity, true, description, HELP);


    public static readonly DiagnosticDescriptor NotPartial = Create(NOT_PARTIAL, "[GenerateJson] type must be partial", "'{0}' must be partial (and so must every type containing it) for [GenerateJson] to implement IJsonSerializable", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor GenericUnsupported = Create(GENERIC_UNSUPPORTED, "Type parameter can't be serialized statically", "Member '{0}' uses type parameter '{1}', which needs a constraint the generated code can dispatch on: 'where {1} : IJsonSerializable<{1}>', or 'where {1} : ISpanFormattable, ISpanParsable<{1}>'", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor UnsupportedMember = Create(UNSUPPORTED_MEMBER, "Member type isn't supported", "Member '{0}' has type '{1}', which Jakar.Json can't read or write: {2}. Add [JsonMember(Converter = typeof(...))] with an IJsonConverter<{1}>, or [JsonMember(Ignore = true)].", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor NoConstructor = Create(NO_CONSTRUCTOR, "No usable constructor", "{0}", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor NameCollision = Create(NAME_COLLISION, "Two members have the same JSON name", "Members '{0}' and '{1}' both read and write the JSON name \"{2}\"{3}", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor OrderAmbiguous = Create(ORDER_AMBIGUOUS,
                                                                        "Member order depends on file paths",
                                                                        "'{0}' declares members in several files; without [JsonMember(Order = n)] their JSON order depends on the file paths. Give them explicit orders.",
                                                                        DiagnosticSeverity.Warning,
                                                                        "Members are ordered by Order, then by declaration (file path, then position). Renaming or moving a file would change the output of a type whose members span files.");

    public static readonly DiagnosticDescriptor HandWritten = Create(HAND_WRITTEN, "Member already declared", "'{0}' already declares '{1}', so [GenerateJson] doesn't generate it", DiagnosticSeverity.Info);

    public static readonly DiagnosticDescriptor Contradictory = Create(CONTRADICTORY, "Contradictory settings", "{0}", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor InvalidConverter = Create(INVALID_CONVERTER, "Converter doesn't fit the member", "Converter '{0}' on member '{1}' must implement IJsonConverter<{2}>", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor BothGenerators = Create(BOTH_GENERATORS, "[GenerateJson] and [JsonModel] on one type", "'{0}' has both [GenerateJson] (Jakar.Json) and [JsonModel] (Jakar.SystemTextJson); both generate FromJson/ToJson and neither can see the other's output. Keep one.", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor NotRead = Create(NOT_READ, "Member is written but never read", "Member '{0}' has no setter and no matching constructor parameter, so it's written but ignored when reading", DiagnosticSeverity.Info);

    public static readonly DiagnosticDescriptor InvalidMemberType = Create(INVALID_MEMBER_TYPE, "Member type can't be serialized", "Member '{0}' has type '{1}' ({2}), which has no JSON form; ignore it with [JsonMember(Ignore = true)]", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor DeterminismWeakened = Create(DETERMINISM_WEAKENED, "Output may differ for the same value", "{0}", DiagnosticSeverity.Warning, "SPEC.md §5.4: idempotency guarantee I1 (same value, same bytes) is weakened by this setting.");

    public static readonly DiagnosticDescriptor InvalidDerived = Create(INVALID_DERIVED, "Invalid [JsonDerived]", "{0}", DiagnosticSeverity.Error);

    public static readonly DiagnosticDescriptor MissingAttribute = Create(MISSING_ATTRIBUTE, "IJsonSerializable<T> without [GenerateJson]", "'{0}' lists IJsonSerializable<{0}> but has no [GenerateJson] and doesn't implement it; add [GenerateJson]", DiagnosticSeverity.Error);
}
