// Jakar.Json.Generator
// 10/07/2026

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Json.Generator;


/// <summary>
///     Jakar.Json's source generator, for every <see langword="partial"/> type marked <c>[JsonModel]</c>. What it implements depends on the type (SPEC.md §7.7):
///     <list type="bullet">
///         <item> <b> No context </b>: Jakar.Json only. <c>IJsonSerializable&lt;TSelf&gt;</c> with its <c>ToJson</c> / <c>FromJson</c> helpers (<see cref="Emitter"/>). </item>
///         <item>
///             <b> A System.Text.Json context </b>: <c>IJsonModel&lt;TSelf&gt;</c> (<see cref="SystemTextJson.Emitter"/>), its members implemented by the faster serializer:
///             Jakar.Json when it supports every member and the context's options (mirrored into its settings, <see cref="ContextMirror"/>), otherwise System.Text.Json (JJSON016).
///             <c>Implementation = SystemTextJson / JakarJson</c> forces one.
///         </item>
///     </list>
///     Jakar.Json code is reflection-free readers and writers generic over <c>IJsonWriter</c> / <c>IJsonReader</c>, plus <c>ISpanFormattable</c> / <c>ISpanParsable</c> (and UTF-8).
///     Settings are resolved at compile time (member → type → context → <c>[assembly: JsonDefaults]</c> → built-in) and baked into the generated code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class JsonGenerator : IIncrementalGenerator
{
    internal const string JSON_MODEL_ATTRIBUTE    = SystemTextJson.JsonModelGenerator.JSON_MODEL_ATTRIBUTE;
    internal const string JSON_DEFAULTS_ATTRIBUTE = "Jakar.Json.JsonDefaultsAttribute";
    internal const string JSON_MEMBER_ATTRIBUTE   = "Jakar.Json.JsonMemberAttribute";
    internal const string JSON_DERIVED_ATTRIBUTE  = "Jakar.Json.JsonDerivedAttribute";
    internal const string SERIALIZABLE_INTERFACE  = "Jakar.Json.IJsonSerializable`1";
    internal const string CONVERTER_INTERFACE     = "Jakar.Json.IJsonConverter`1";
    internal const string VERSION                 = "1.0.0";


    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        // One extraction per type, both views: Jakar.Json's and IJsonModel's.
        IncrementalValuesProvider<JsonModelPair> models = context.SyntaxProvider.ForAttributeWithMetadataName(JSON_MODEL_ATTRIBUTE, static ( node, _ ) => node is TypeDeclarationSyntax, static ( ctx, token ) => new JsonModelPair(ModelExtractor.Extract(ctx, token), SystemTextJson.ModelExtractor.Extract(ctx, token)));

        IncrementalValueProvider<Settings> defaults = context.CompilationProvider.Select(static ( compilation, _ ) => Settings.BuiltIn.With(Settings.From(compilation.Assembly.GetAttributes().FirstOrDefault(static a => a.AttributeClass is not null && TypeShapes.MetadataName(a.AttributeClass) == JSON_DEFAULTS_ATTRIBUTE))));

        IncrementalValueProvider<ImmutableArray<SystemTextJson.ContextInfo>> contexts = context.SyntaxProvider.ForAttributeWithMetadataName(SystemTextJson.JsonModelGenerator.JSON_SERIALIZABLE_ATTRIBUTE, static ( node, _ ) => node is ClassDeclarationSyntax, static ( ctx, token ) => SystemTextJson.ContextExtractor.Extract(ctx, token)).Collect();

        IncrementalValueProvider<SystemTextJson.DefaultContextInfo> defaultContext = context.CompilationProvider.Select(static ( compilation, _ ) => SystemTextJson.ContextExtractor.GetDefault(compilation));

        IncrementalValuesProvider<Input> inputs = models.Combine(defaults).Combine(contexts.Combine(defaultContext)).Select(static ( x, _ ) => new Input(x.Left.Left, x.Left.Right, x.Right.Left, x.Right.Right));

        context.RegisterSourceOutput(inputs, static ( spc, input ) => Emit(spc, input));

        // One converter per (enum, naming policy) used by the Jakar.Json code anywhere in the assembly.
        IncrementalValueProvider<ImmutableArray<EnumInfo>> enums = inputs.SelectMany(static ( input, _ ) => EmitsJakar(input)
                                                                                                               ? Emitter.EnumsUsed(input.Pair.Jakar, input.Defaults)
                                                                                                               : []).Collect();

        context.RegisterSourceOutput(enums, static ( spc, all ) => EnumEmitter.Emit(spc, all));

        // JsonModelRegistry entries for the context-backed types.
        IncrementalValueProvider<ImmutableArray<SystemTextJson.RegistrationEntry>> registrations = inputs.Select(static ( input, _ ) => input.Pair.Jakar.ContextRequested
                                                                                                                                                  ? SystemTextJson.Emitter.GetRegistration(input.Pair.Stj, input.Contexts, input.DefaultContext)
                                                                                                                                                  : null).Where(static entry => entry.HasValue).Select(static ( entry, _ ) => entry!.Value).Collect();

        context.RegisterSourceOutput(registrations, static ( spc, entries ) => SystemTextJson.Emitter.EmitRegistrations(spc, entries));

        // JsonCodecRegistry entries for every type with Jakar.Json code, so frameworks that only know a Type (ASP.NET Core formatters) find its codec.
        IncrementalValueProvider<ImmutableArray<string>> codecs = inputs.Select(static ( input, _ ) => input.Pair.Jakar.Registrable && EmitsJakar(input)
                                                                                                           ? input.Pair.Jakar.FullyQualifiedName
                                                                                                           : null).Where(static name => name is not null).Select(static ( name, _ ) => name!).Collect();

        context.RegisterSourceOutput(codecs, static ( spc, names ) => EmitCodecRegistrations(spc, names));
    }


    private static void EmitCodecRegistrations( SourceProductionContext spc, ImmutableArray<string> names )
    {
        if ( names.IsDefaultOrEmpty ) { return; }

        StringBuilder builder = new();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("#pragma warning disable CA2255 // ModuleInitializer in a library: registers each [JsonModel] type's codec (one small object per type), no other side effects");
        builder.AppendLine();
        builder.AppendLine("namespace Jakar.Json.Generated");
        builder.AppendLine("{");
        builder.AppendLine($"    [global::System.CodeDom.Compiler.GeneratedCode(\"Jakar.Json.Generator\", \"{VERSION}\")]");
        builder.AppendLine("    internal static class JsonCodecRegistrations");
        builder.AppendLine("    {");
        builder.AppendLine("        [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("        internal static void Register()");
        builder.AppendLine("        {");

        foreach ( string name in names.Distinct().OrderBy(static name => name, StringComparer.Ordinal) ) { builder.AppendLine($"            global::Jakar.Json.JsonCodecRegistry.Register<{name}>();"); }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        spc.AddSource("JsonCodecRegistrations.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }


    /// <summary> Whether the Jakar.Json code for the type is emitted (on its own, or behind IJsonModel). </summary>
    private static bool EmitsJakar( Input input )
    {
        ModelInfo jakar = input.Pair.Jakar;

        return !jakar.ContextRequested
                   ? !jakar.IsFatal
                   : jakar.Implementation != Values.IMPLEMENTATION_STJ && Emitter.CanEmit(jakar, input.Defaults, out _);
    }


    private static void Emit( SourceProductionContext spc, Input input )
    {
        ModelInfo                jakar = input.Pair.Jakar;
        SystemTextJson.ModelInfo stj   = input.Pair.Stj;

        // ─── No context: Jakar.Json only ─────────────────────────────────────
        if ( !jakar.ContextRequested )
        {
            if ( jakar.Implementation == Values.IMPLEMENTATION_STJ )
            {
                spc.ReportDiagnostic(SystemTextJson.DiagnosticInfo.Create(SystemTextJson.JsonModelDiagnostics.InvalidContext, stj.AttributeLocation, $"[JsonModel(Implementation = SystemTextJson)] on '{stj.Name}' needs a JsonSerializerContext: pass the one that registers it, or set an assembly default with [assembly: JsonModelContext(typeof(...))]").ToDiagnostic());
                return;
            }

            Emitter.EmitModel(spc, jakar, input.Defaults); // reports Jakar.Json's diagnostics, errors included
            return;
        }

        // ─── A context: IJsonModel, implemented by the faster serializer ─────
        bool jakarCapable = Emitter.CanEmit(jakar, input.Defaults, out string? reason);

        switch ( jakar.Implementation )
        {
            case Values.IMPLEMENTATION_STJ:
                SystemTextJson.Emitter.EmitModel(spc, stj, input.Contexts, input.DefaultContext, false);
                return;

            case Values.IMPLEMENTATION_JAKAR when !jakarCapable:
                spc.ReportDiagnostic(DiagnosticInfo.Create(JsonDiagnostics.JakarJsonRequired, jakar.AttributeLocation, jakar.Name, reason ?? "").ToDiagnostic());
                Emitter.EmitModel(spc, jakar, input.Defaults); // its errors say what to fix
                return;
        }

        if ( jakarCapable && !stj.IsFatal )
        {
            Emitter.EmitModel(spc, jakar, input.Defaults);
            SystemTextJson.Emitter.EmitModel(spc, stj, input.Contexts, input.DefaultContext, true);
            return;
        }

        // Fall back to System.Text.Json: Jakar.Json's errors become the reason, not errors (the type still works).
        if ( !stj.IsFatal ) { spc.ReportDiagnostic(DiagnosticInfo.Create(JsonDiagnostics.SystemTextJsonFallback, jakar.AttributeLocation, jakar.Name, reason ?? "").ToDiagnostic()); }

        SystemTextJson.Emitter.EmitModel(spc, stj, input.Contexts, input.DefaultContext, false);
    }



    /// <summary> Both views of one <c>[JsonModel]</c> type (equatable, so the pipeline caches per type). </summary>
    private sealed record JsonModelPair( ModelInfo Jakar, SystemTextJson.ModelInfo Stj );



    private sealed record Input( JsonModelPair Pair, Settings Defaults, ImmutableArray<SystemTextJson.ContextInfo> Contexts, SystemTextJson.DefaultContextInfo DefaultContext )
    {
        public bool Equals( Input? other ) => other is not null && Pair.Equals(other.Pair) && Defaults.Equals(other.Defaults) && Contexts.SequenceEqual(other.Contexts) && DefaultContext.Equals(other.DefaultContext);

        public override int GetHashCode() => Pair.GetHashCode();
    }
}
