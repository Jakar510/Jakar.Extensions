// Jakar.SystemTextJson
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Implements <see cref="IJsonModel{TSelf}"/> on a <see langword="partial"/> class, record or struct, using metadata from a source-generated <see cref="JsonSerializerContext"/>.
///     <code>
///     [JsonSerializable(typeof(Invoice))]
///     public sealed partial class AppJsonContext : JsonSerializerContext;
///
///     [JsonModel(typeof(AppJsonContext))]
///     public sealed partial class Invoice { ... }
///     </code>
///     <para> The type must also be registered in that context (<c> [JsonSerializable(typeof(Invoice))] </c>): the System.Text.Json generator can't see this generator's output, so the registration can't be added for you (JAKAR_JSON001 has a code fix that does it). </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonModelAttribute : Attribute
{
    /// <summary> The <see cref="JsonSerializerContext"/> that registers the type, or <see langword="null"/> for the assembly default (<see cref="JsonModelContextAttribute"/>). </summary>
    public Type? Context { get; }

    /// <summary> Also emit <c> public override string ToString() </c> returning the JSON, unless the type or a base type already overrides it. </summary>
    public bool GenerateToString { get; set; }


    /// <summary> Uses the assembly default context (<see cref="JsonModelContextAttribute"/>). </summary>
    public JsonModelAttribute() { }

    /// <param name="context"> The <see cref="JsonSerializerContext"/> that registers the type. </param>
    public JsonModelAttribute( Type context ) => Context = context;
}



/// <summary> The default <see cref="JsonSerializerContext"/> for <c> [JsonModel] </c> types in this assembly that don't name one. </summary>
[AttributeUsage(AttributeTargets.Assembly)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonModelContextAttribute( Type context ) : Attribute
{
    public Type Context { get; } = context;
}
