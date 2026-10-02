// Jakar.Json
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     A type that knows its own source-generated System.Text.Json metadata.
///     <para> Don't implement this by hand: put <see cref="JsonModelAttribute"/> on a <see langword="partial"/> type and the generator implements it. Open generic types (which can't be source generated) implement <see cref="JsonTypeInfo"/> with a runtime lookup instead (<see cref="JsonModel.GetRequiredTypeInfo{T}"/>). </para>
/// </summary>
/// <remarks> Only <see cref="JsonTypeInfo"/> is abstract. Everything else has a default built on it, which generic code reaches through the type parameter (<c> T.FromJson(json) </c>). C# doesn't let callers reach a <see langword="static virtual"/> default through the concrete type name, which is why the generator also emits these methods on the type. </remarks>
public interface IJsonModel<TSelf>
    where TSelf : IJsonModel<TSelf>
{
    /// <summary> Source-generated metadata for <typeparamref name="TSelf"/>. Emitted by [JsonModel]; open generic types implement it with <see cref="JsonModel.GetRequiredTypeInfo{T}"/>. </summary>
    public abstract static JsonTypeInfo<TSelf> JsonTypeInfo { get; }


    /// <summary>
    ///     Resolves metadata for <typeparamref name="TOther"/> from the same source-generated context as <typeparamref name="TSelf"/>,
    ///     typically a shape built around TSelf (Dictionary&lt;string, TSelf&gt;, TSelf?, a wrapper type). Root-level arrays and lists don't need this:
    ///     use <see cref="JsonModel.FromJsonArray{T}(ReadOnlySpan{byte}, JsonTypeInfo{T})"/> / <see cref="JsonModel.WriteArray{T}(Utf8JsonWriter, ReadOnlySpan{T}, JsonTypeInfo{T})"/> with <see cref="JsonTypeInfo"/>, which require no extra registration.
    ///     <para> Unconstrained on purpose: a constraint such as <c> where TOther : IEnumerable&lt;TSelf&gt; </c> can't check registration and would exclude dictionaries, TSelf? and wrappers. </para>
    /// </summary>
    /// <exception cref="NotSupportedException"> <typeparamref name="TOther"/> isn't registered in that context. </exception>
    public static virtual JsonTypeInfo<TOther> GetTypeInfo<TOther>() => TSelf.JsonTypeInfo.Options.GetRequiredTypeInfo<TOther>();


    /// <exception cref="JsonException"> The JSON is malformed, or its root is <c> null </c>. </exception>
    public static virtual TSelf FromJson( string json ) => JsonModel.FromJson(json, TSelf.JsonTypeInfo);

    /// <inheritdoc cref="FromJson(string)"/>
    public static virtual TSelf FromJson( ReadOnlySpan<byte> utf8Json ) => JsonModel.FromJson(utf8Json, TSelf.JsonTypeInfo);

    /// <summary> <see langword="false"/> for <see langword="null"/>/blank input, malformed JSON, or a JSON <c> null </c> root. </summary>
    public static virtual bool TryFromJson( [NotNullWhen(true)] string? json, [NotNullWhen(true)] out TSelf? result ) => JsonModel.TryFromJson(json, TSelf.JsonTypeInfo, out result);

    /// <inheritdoc cref="TryFromJson(string, out TSelf)"/>
    public static virtual bool TryFromJson( ReadOnlySpan<byte> utf8Json, [NotNullWhen(true)] out TSelf? result ) => JsonModel.TryFromJson(utf8Json, TSelf.JsonTypeInfo, out result);

    /// <inheritdoc cref="FromJson(string)"/>
    public static virtual ValueTask<TSelf> FromJsonAsync( Stream stream, CancellationToken token = default ) => JsonModel.FromJsonAsync(stream, TSelf.JsonTypeInfo, token);
}



/// <summary> Carries JSON members the type doesn't declare, so they survive a round trip. </summary>
/// <remarks>
///     Implementations must mark the property <c> [JsonExtensionData] </c>: System.Text.Json reads attributes from the implementing property, not from this interface (the analyzer reports JAKAR_JSON007 otherwise).
///     <para> A <see cref="Dictionary{TKey,TValue}"/> of <see cref="JsonElement"/> rather than a <see cref="JsonObject"/>: on .NET 10, System.Text.Json writes a <see cref="JsonObject"/> extension-data property as a nested object (invalid JSON), with source generation and reflection alike. </para>
/// </remarks>
public interface IJsonModel
{
    public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}



/// <summary> Same bag as <see cref="IJsonModel"/>, stored as a JSON string (e.g. a database column). </summary>
public interface IJsonStringModel
{
    public string? AdditionalData { get; set; }
}
