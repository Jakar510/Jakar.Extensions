// Jakar.SystemTextJson.Generator
// 10/02/2026

using Microsoft.CodeAnalysis;



namespace Jakar.SystemTextJson.Generator;


/// <summary> Diagnostic IDs are public API once shipped: never renumber or reuse them. </summary>
public static class JsonModelDiagnostics
{
    public const string CATEGORY = "Jakar.SystemTextJson";
    public const string HELP     = "https://github.com/Jakar510/Jakar.Extensions/blob/master/Jakar.SystemTextJson/README.md#diagnostics";

    public const string NOT_REGISTERED        = "JAKAR_JSON001";
    public const string NOT_PARTIAL           = "JAKAR_JSON002";
    public const string OPEN_GENERIC          = "JAKAR_JSON003";
    public const string INVALID_CONTEXT       = "JAKAR_JSON004";
    public const string HAND_WRITTEN          = "JAKAR_JSON005";
    public const string MISSING_ATTRIBUTE     = "JAKAR_JSON006";
    public const string MISSING_EXTENSIONDATA = "JAKAR_JSON007";

    /// <summary> Diagnostic property: the context's metadata name (for <c> Compilation.GetTypeByMetadataName </c>). </summary>
    public const string CONTEXT_METADATA_NAME = "ContextMetadataName";

    /// <summary> Diagnostic property: the model's fully qualified name (<c> global::Ns.Type </c>). </summary>
    public const string TYPE_NAME = "TypeName";


    public static readonly DiagnosticDescriptor NotRegistered = new(NOT_REGISTERED,
                                                                   "Type isn't registered in its JsonSerializerContext",
                                                                   "'{0}' isn't registered in '{1}': add [JsonSerializable(typeof({0}))] to the context",
                                                                   CATEGORY,
                                                                   DiagnosticSeverity.Error,
                                                                   true,
                                                                   "The System.Text.Json source generator can't see this generator's output, so a [JsonModel] type must also be registered in its context by hand.",
                                                                   HELP);

    public static readonly DiagnosticDescriptor NotPartial = new(NOT_PARTIAL,
                                                                "[JsonModel] type must be partial",
                                                                "'{0}' must be partial (and so must every type containing it) for [JsonModel] to implement IJsonModel",
                                                                CATEGORY,
                                                                DiagnosticSeverity.Error,
                                                                true,
                                                                helpLinkUri: HELP);

    public static readonly DiagnosticDescriptor OpenGeneric = new(OPEN_GENERIC,
                                                                 "[JsonModel] can't be used on an open generic type",
                                                                 "'{0}' is generic; System.Text.Json can't source-generate open generic types. Put [JsonModel] on the closed derived type, or implement JsonTypeInfo with JsonModel.GetRequiredTypeInfo<T>().",
                                                                 CATEGORY,
                                                                 DiagnosticSeverity.Error,
                                                                 true,
                                                                 helpLinkUri: HELP);

    public static readonly DiagnosticDescriptor InvalidContext = new(INVALID_CONTEXT,
                                                                    "[JsonModel] needs a JsonSerializerContext",
                                                                    "{0}",
                                                                    CATEGORY,
                                                                    DiagnosticSeverity.Error,
                                                                    true,
                                                                    helpLinkUri: HELP);

    public static readonly DiagnosticDescriptor HandWritten = new(HAND_WRITTEN,
                                                                 "Type already implements JsonTypeInfo",
                                                                 "'{0}' declares JsonTypeInfo itself, so [JsonModel] won't generate it",
                                                                 CATEGORY,
                                                                 DiagnosticSeverity.Warning,
                                                                 true,
                                                                 helpLinkUri: HELP);

    public static readonly DiagnosticDescriptor MissingAttribute = new(MISSING_ATTRIBUTE,
                                                                      "IJsonModel<T> without [JsonModel]",
                                                                      "'{0}' lists IJsonModel<{0}> but has no [JsonModel] and doesn't implement JsonTypeInfo; add [JsonModel] to generate the implementation",
                                                                      CATEGORY,
                                                                      DiagnosticSeverity.Error,
                                                                      true,
                                                                      helpLinkUri: HELP);

    public static readonly DiagnosticDescriptor MissingExtensionData = new(MISSING_EXTENSIONDATA,
                                                                          "AdditionalData needs [JsonExtensionData]",
                                                                          "'{0}.AdditionalData' implements IJsonModel.AdditionalData without [JsonExtensionData], so System.Text.Json drops unknown members instead of keeping them",
                                                                          CATEGORY,
                                                                          DiagnosticSeverity.Warning,
                                                                          true,
                                                                          "System.Text.Json reads attributes from the implementing property, not from the interface.",
                                                                          HELP);
}
