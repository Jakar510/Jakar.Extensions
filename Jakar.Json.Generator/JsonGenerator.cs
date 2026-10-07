// Jakar.Json.Generator
// 10/07/2026

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace Jakar.Json.Generator;


/// <summary>
///     Implements <c>IJsonSerializable&lt;TSelf&gt;</c> on every <see langword="partial"/> type marked <c>[GenerateJson]</c>: reflection-free readers and writers generic over
///     <c>IJsonWriter</c> / <c>IJsonReader</c>, <c>ISpanFormattable</c> / <c>ISpanParsable</c> (and UTF-8), and <c>ToJson</c> / <c>FromJson</c> helpers. Settings are resolved
///     at compile time (member → type → <c>[assembly: JsonDefaults]</c> → built-in) and baked into the generated code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class JsonGenerator : IIncrementalGenerator
{
    internal const string GENERATE_JSON_ATTRIBUTE = "Jakar.Json.GenerateJsonAttribute";
    internal const string JSON_DEFAULTS_ATTRIBUTE = "Jakar.Json.JsonDefaultsAttribute";
    internal const string JSON_MEMBER_ATTRIBUTE   = "Jakar.Json.JsonMemberAttribute";
    internal const string JSON_DERIVED_ATTRIBUTE  = "Jakar.Json.JsonDerivedAttribute";
    internal const string SERIALIZABLE_INTERFACE  = "Jakar.Json.IJsonSerializable`1";
    internal const string CONVERTER_INTERFACE     = "Jakar.Json.IJsonConverter`1";
    internal const string VERSION                 = "1.0.0";


    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        IncrementalValuesProvider<ModelInfo> models = context.SyntaxProvider.ForAttributeWithMetadataName(GENERATE_JSON_ATTRIBUTE, static ( node, _ ) => node is TypeDeclarationSyntax, static ( ctx, token ) => ModelExtractor.Extract(ctx, token));

        IncrementalValueProvider<Settings> defaults = context.CompilationProvider.Select(static ( compilation, _ ) => Settings.BuiltIn.With(Settings.From(compilation.Assembly.GetAttributes().FirstOrDefault(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == JSON_DEFAULTS_ATTRIBUTE))));

        IncrementalValuesProvider<(ModelInfo Model, Settings Defaults)> combined = models.Combine(defaults);

        context.RegisterSourceOutput(combined, static ( spc, pair ) => Emitter.EmitModel(spc, pair.Model, pair.Defaults));

        // One converter per (enum, naming policy) used anywhere in the assembly.
        IncrementalValueProvider<ImmutableArray<EnumInfo>> enums = combined.SelectMany(static ( pair, _ ) => Emitter.EnumsUsed(pair.Model, pair.Defaults)).Collect();

        context.RegisterSourceOutput(enums, static ( spc, all ) => EnumEmitter.Emit(spc, all));
    }
}
