// Jakar.Json.Generator
// 10/02/2026

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Json.Generator.SystemTextJson;


internal static class Emitter
{
    private const string GENERATED_CODE = $"[global::System.CodeDom.Compiler.GeneratedCode(\"Jakar.Json.Generator\", \"{JsonModelGenerator.VERSION}\")]";



    private readonly record struct ResolvedContext( string FullyQualifiedName, string? MetadataName, string? PropertyName, bool IsRegistered );



    /// <summary> Resolves the model's context and its registration; <see langword="null"/> (with a diagnostic) when there's no usable context. </summary>
    private static ResolvedContext? Resolve( ModelInfo model, ImmutableArray<ContextInfo> contexts, DefaultContextInfo defaultContext, out DiagnosticInfo? diagnostic )
    {
        diagnostic = null;
        string? contextName     = model.ExplicitContext;
        string? contextMetadata = model.ExplicitContextMetadata;

        if ( contextName is null )
        {
            if ( defaultContext.IsValid )
            {
                contextName     = defaultContext.FullyQualifiedName;
                contextMetadata = defaultContext.MetadataName;
            }
            else
            {
                string message = defaultContext.InvalidName is null
                                     ? $"[JsonModel] on '{model.Name}' names no context and the assembly has no [assembly: JsonModelContext(typeof(...))] default; pass the JsonSerializerContext that registers it"
                                     : $"The assembly default [assembly: JsonModelContext(typeof({defaultContext.InvalidName}))] isn't a JsonSerializerContext";

                diagnostic = DiagnosticInfo.Create(JsonModelDiagnostics.InvalidContext, model.AttributeLocation, message);
                return null;
            }
        }

        // Contexts declared in this compilation can be checked; one from a referenced assembly is trusted to use System.Text.Json's default property name.
        ContextInfo? context = contexts.FirstOrDefault(c => c.FullyQualifiedName == contextName);
        if ( context is null ) { return new ResolvedContext(contextName!, contextMetadata, model.Name, true); }

        Registration? registration = context.Registrations.Cast<Registration?>().FirstOrDefault(r => r!.Value.TypeName == model.FullyQualifiedName);
        if ( registration is { } found ) { return new ResolvedContext(contextName!, contextMetadata, found.PropertyName, true); }

        diagnostic = new DiagnosticInfo(JsonModelDiagnostics.NotRegistered, model.AttributeLocation, new EquatableArray<string>([model.Name, contextName!.Replace("global::", "")]), new EquatableArray<DiagnosticProperty>([new DiagnosticProperty(JsonModelDiagnostics.CONTEXT_METADATA_NAME, contextMetadata ?? ""), new DiagnosticProperty(JsonModelDiagnostics.TYPE_NAME, model.FullyQualifiedName)]));

        return new ResolvedContext(contextName!, contextMetadata, null, false);
    }


    public static RegistrationEntry? GetRegistration( ModelInfo model, ImmutableArray<ContextInfo> contexts, DefaultContextInfo defaultContext )
    {
        if ( model.IsFatal || !model.IsAccessibleInAssembly ) { return null; }

        ResolvedContext? resolved = Resolve(model, contexts, defaultContext, out _);

        return resolved is { IsRegistered: true } || ( resolved is not null && !model.EmitJsonTypeInfo )
                   ? new RegistrationEntry(model.FullyQualifiedName)
                   : null;
    }


    /// <param name="hybrid"> Jakar.Json implements the type (it's also <c>IJsonSerializable&lt;T&gt;</c>): the reading members and <c>SerializeJson*</c> call it through <c>JsonCodec</c>; only <c>JsonTypeInfo</c> stays System.Text.Json's. </param>
    public static void EmitModel( SourceProductionContext spc, ModelInfo model, ImmutableArray<ContextInfo> contexts, DefaultContextInfo defaultContext, bool hybrid )
    {
        foreach ( DiagnosticInfo diagnostic in model.Diagnostics ) { spc.ReportDiagnostic(diagnostic.ToDiagnostic()); }

        if ( model.IsFatal ) { return; }

        ResolvedContext? resolved = Resolve(model, contexts, defaultContext, out DiagnosticInfo? contextDiagnostic);
        if ( contextDiagnostic is not null ) { spc.ReportDiagnostic(contextDiagnostic.ToDiagnostic()); }

        if ( resolved is not { } context ) { return; }

        spc.AddSource($"{HintName(model.MetadataName)}.JsonModel.g.cs", SourceText.From(Generate(model, context, hybrid), Encoding.UTF8));
    }


    private static string Generate( ModelInfo model, ResolvedContext context, bool hybrid )
    {
        string self   = model.FullyQualifiedName;
        string info   = $"global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<{self}>";
        string codec  = "global::Jakar.Json.JsonCodec"; // its read errors are JsonReadException : JsonException, as IJsonModel promises
        string stj    = "global::Jakar.Extensions.JsonModel";

        string outType = model.IsValueType
                             ? self
                             : $"{self}?";

        string notNull = model.IsValueType
                             ? ""
                             : "[global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ";

        StringBuilder builder = new();

        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("#pragma warning disable CS1591 // generated members are documented on IJsonModel<TSelf>");
        builder.AppendLine();

        int depth = 0;

        if ( model.Namespace is not null )
        {
            builder.Append("namespace ").AppendLine(model.Namespace);
            builder.AppendLine("{");
            depth++;
        }

        foreach ( TypeDeclInfo containing in model.ContainingTypes )
        {
            Line(builder, depth,   containing.Declaration);
            Line(builder, depth++, "{");
        }

        Line(builder, depth,   $"{model.Self.Declaration} : global::Jakar.Extensions.IJsonModel<{self}>");
        Line(builder, depth++, "{");

        if ( model.EmitJsonTypeInfo )
        {
            Line(builder, depth, $"/// <summary> Source-generated metadata from <c>{context.FullyQualifiedName.Replace("global::", "")}</c>. </summary>");
            Line(builder, depth, GENERATED_CODE);

            Line(builder,
                 depth,
                 context.IsRegistered
                     ? $"public static {info} JsonTypeInfo => {context.FullyQualifiedName}.Default.{context.PropertyName};"
                     : $"public static {info} JsonTypeInfo => throw new global::System.InvalidOperationException(\"{model.Name} isn't registered in {context.FullyQualifiedName.Replace("global::", "")} (JAKAR_JSON001).\");");

            builder.AppendLine();
        }

        if ( model.EmitFromJsonString )
        {
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, hybrid
                                 ? $"public static {self} FromJson( string json ) => {codec}.FromJson<{self}>(json);"
                                 : $"public static {self} FromJson( string json ) => {stj}.FromJson(json, JsonTypeInfo);");
        }

        if ( model.EmitFromJsonUtf8 )
        {
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, hybrid
                                 ? $"public static {self} FromJson( global::System.ReadOnlySpan<byte> utf8Json ) => {codec}.FromJson<{self}>(utf8Json);"
                                 : $"public static {self} FromJson( global::System.ReadOnlySpan<byte> utf8Json ) => {stj}.FromJson(utf8Json, JsonTypeInfo);");
        }

        if ( model.EmitTryFromJsonString )
        {
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, hybrid
                                 ? $"public static bool TryFromJson( [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? json, {notNull}out {outType} result ) => {codec}.TryFromJson<{self}>(json, out result);"
                                 : $"public static bool TryFromJson( [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? json, {notNull}out {outType} result ) => {stj}.TryFromJson(json, JsonTypeInfo, out result);");
        }

        if ( model.EmitTryFromJsonUtf8 )
        {
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, hybrid
                                 ? $"public static bool TryFromJson( global::System.ReadOnlySpan<byte> utf8Json, {notNull}out {outType} result ) => {codec}.TryFromJson<{self}>(utf8Json, out result);"
                                 : $"public static bool TryFromJson( global::System.ReadOnlySpan<byte> utf8Json, {notNull}out {outType} result ) => {stj}.TryFromJson(utf8Json, JsonTypeInfo, out result);");
        }

        if ( model.EmitFromJsonAsync )
        {
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, hybrid
                                 ? $"public static global::System.Threading.Tasks.ValueTask<{self}> FromJsonAsync( global::System.IO.Stream stream, global::System.Threading.CancellationToken token = default ) => {codec}.FromJsonAsync<{self}>(stream, null, token);"
                                 : $"public static global::System.Threading.Tasks.ValueTask<{self}> FromJsonAsync( global::System.IO.Stream stream, global::System.Threading.CancellationToken token = default ) => {stj}.FromJsonAsync(stream, JsonTypeInfo, token);");
        }

        if ( hybrid )
        {
            // value.ToJson() / ToUtf8Json() / WriteToAsync(...) reach these through IJsonModel<TSelf>; explicit, so they don't add to the type's surface.
            builder.AppendLine();
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, $"static string global::Jakar.Extensions.IJsonModel<{self}>.SerializeJson( {self} value, bool? indented ) => indented is {{ }} layout ? {codec}.ToJson(in value, layout) : {codec}.ToJson(in value);");
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, $"static byte[] global::Jakar.Extensions.IJsonModel<{self}>.SerializeJsonUtf8( {self} value ) => {codec}.ToJsonUtf8(in value);");
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, $"static global::System.Threading.Tasks.Task global::Jakar.Extensions.IJsonModel<{self}>.SerializeJsonAsync( {self} value, global::System.IO.Stream stream, global::System.Threading.CancellationToken token ) => {codec}.ToJsonAsync(value, stream, null, token).AsTask();");
        }
        else if ( model.EmitToString ) // with Jakar.Json, its part emits ToString() (GenerateToString = On)
        {
            builder.AppendLine();
            Line(builder, depth, GENERATED_CODE);
            Line(builder, depth, "public override string ToString() => global::System.Text.Json.JsonSerializer.Serialize(this, JsonTypeInfo);");
        }

        while ( depth > 0 ) { Line(builder, --depth, "}"); }

        return builder.ToString();
    }


    public static void EmitRegistrations( SourceProductionContext spc, ImmutableArray<RegistrationEntry> entries )
    {
        if ( entries.IsDefaultOrEmpty ) { return; }

        StringBuilder builder = new();
        builder.AppendLine("// <auto-generated/>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("#pragma warning disable CA2255 // ModuleInitializer in a library: registers source-generated JSON metadata lazily, no other side effects");
        builder.AppendLine();
        builder.AppendLine("namespace Jakar.Extensions.Generated");
        builder.AppendLine("{");
        builder.AppendLine($"    {GENERATED_CODE}");
        builder.AppendLine("    internal static class JsonModelRegistrations");
        builder.AppendLine("    {");
        builder.AppendLine("        [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("        internal static void Register()");
        builder.AppendLine("        {");

        foreach ( string name in entries.Select(static entry => entry.FullyQualifiedName).Distinct().OrderBy(static name => name, System.StringComparer.Ordinal) ) { builder.AppendLine($"            global::Jakar.Extensions.JsonModelRegistry.Register<{name}>(static () => {name}.JsonTypeInfo);"); }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        spc.AddSource("JsonModelRegistrations.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }


    private static void Line( StringBuilder builder, int depth, string text ) => builder.Append(' ', depth * 4).AppendLine(text);


    private static string HintName( string metadataName )
    {
        StringBuilder builder = new(metadataName.Length);

        foreach ( char c in metadataName )
        {
            builder.Append(char.IsLetterOrDigit(c) || c is '.' or '_'
                               ? c
                               : '_');
        }

        return builder.ToString();
    }
}
