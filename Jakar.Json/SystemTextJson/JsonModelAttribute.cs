// Jakar.Json
// 10/02/2026

using System.Text.Json.Serialization;
using Jakar.Json;



namespace Jakar.Extensions;


/// <summary>
///     Source-generates JSON for a <see langword="partial"/> class, struct, record or record struct. What it implements depends on whether the type has a System.Text.Json context:
///     <list type="bullet">
///         <item>
///             <b> With a context </b> (<c> [JsonModel(typeof(AppJsonContext))] </c>, or an assembly default <see cref="JsonModelContextAttribute"/>): <see cref="IJsonModel{TSelf}"/>.
///             <see cref="IJsonModel{TSelf}.JsonTypeInfo"/> is the context's metadata; <c> FromJson </c> / <c> TryFromJson </c> / <c> FromJsonAsync </c> / <c> ToJson </c> / <c> ToUtf8Json </c> / <c> WriteToAsync </c>
///             use whichever serializer is faster for the type: Jakar.Json when it can read and write every member (it then also implements <see cref="IJsonSerializable{TSelf}"/>), otherwise System.Text.Json (JJSON016).
///             The context's <c> [JsonSourceGenerationOptions] </c> are mirrored into Jakar.Json's settings, so both produce JSON that means the same thing.
///         </item>
///         <item> <b> Without one </b>: Jakar.Json only, <see cref="IJsonSerializable{TSelf}"/> with its <c> ToJson </c> / <c> FromJson </c> helpers; no System.Text.Json involved. </item>
///     </list>
///     <code>
///     [JsonSerializable(typeof(Invoice))]
///     public sealed partial class AppJsonContext : JsonSerializerContext;
///
///     [JsonModel(typeof(AppJsonContext))]
///     public sealed partial class Invoice { ... }
///     </code>
///     <para> A context-backed type must also be registered in that context (<c> [JsonSerializable(typeof(Invoice))] </c>): the System.Text.Json generator can't see this generator's output, so the registration can't be added for you (JAKAR_JSON001 has a code fix that does it). </para>
///     <para> The Jakar.Json settings (<see cref="JsonSettingsAttribute"/>) apply here and win over the context's options. </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonModelAttribute : JsonSettingsAttribute
{
    /// <summary> The <see cref="JsonSerializerContext"/> that registers the type, or <see langword="null"/> for the assembly default (<see cref="JsonModelContextAttribute"/>), or none (Jakar.Json only). </summary>
    public Type? Context { get; }

    /// <summary> Which serializer implements the <see cref="IJsonModel{TSelf}"/> members of a context-backed type (default <see cref="JsonModelImplementation.Auto"/>). </summary>
    public JsonModelImplementation Implementation { get; set; }

    /// <summary> Jakar.Json: the member that names the concrete type when <see cref="JsonDerivedAttribute"/>s are present (default <c>"$type"</c>). </summary>
    public string? Discriminator { get; set; }


    /// <summary> Uses the assembly default context (<see cref="JsonModelContextAttribute"/>), or none: Jakar.Json only. </summary>
    public JsonModelAttribute() { }

    /// <param name="context"> The <see cref="JsonSerializerContext"/> that registers the type. </param>
    public JsonModelAttribute( Type context ) => Context = context;
}



/// <summary> Which serializer implements a context-backed <c> [JsonModel] </c>'s <see cref="IJsonModel{TSelf}"/> members. </summary>
public enum JsonModelImplementation : byte
{
    /// <summary> The faster one: Jakar.Json when it supports every member (and the context's options), otherwise System.Text.Json (reported as JJSON016). </summary>
    Auto,

    /// <summary> Always System.Text.Json (the type doesn't implement <see cref="IJsonSerializable{TSelf}"/>). </summary>
    SystemTextJson,

    /// <summary> Always Jakar.Json: what would make <see cref="Auto"/> fall back is an error (JJSON017). </summary>
    JakarJson
}



/// <summary> The default <see cref="JsonSerializerContext"/> for <c> [JsonModel] </c> types in this assembly that don't name one. </summary>
[AttributeUsage(AttributeTargets.Assembly)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonModelContextAttribute( Type context ) : Attribute
{
    public Type Context { get; } = context;
}
