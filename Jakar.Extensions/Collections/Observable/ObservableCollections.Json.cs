// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     JSON for the observable collections (AOT-plan §2.13).
///     <list type="bullet">
///         <item> They always serialize their <b> unfiltered </b> contents (<see cref="CollectionAlerts{TSelf,TValue}.OverrideFilter"/> is a view concern), on every path: <c> ToJson() </c>, <see cref="JsonModel.WriteArray{T}(Utf8JsonWriter, IEnumerable{T}, JsonTypeInfo{T})"/> (through <see cref="IJsonArraySource{T}"/>), and System.Text.Json itself once the converter is attached. </item>
///         <item> Reading uses only the element's metadata and inserts every element at once (one notification, one lock). </item>
///         <item> The open generic types implement <c> JsonTypeInfo </c> with a runtime lookup (<see cref="Json.GetTypeInfo{T}"/>); it's only needed when the collection itself is the root of a System.Text.Json call, so register e.g. <c> ObservableCollection&lt;Group&gt; </c> in a context for that. </item>
///     </list>
///     <para>
///         On .NET 11+ the converter is attached to the open generic types automatically (<c> [JsonConverter(typeof(ObservableCollectionJsonConverter&lt;&gt;))] </c>). On net10.0 that isn't supported,
///         so a collection serialized <i> by System.Text.Json </i> (as a property of a model) only takes this path where the context registers the closed converter,
///         e.g. <c> [JsonSourceGenerationOptions(Converters = [typeof(ObservableCollectionJsonConverter&lt;UserAddress&gt;)])] </c>; otherwise STJ enumerates it, which applies the filter.
///     </para>
/// </summary>
public class CollectionAlertsJsonConverter<TSelf, TValue> : JsonConverter<TSelf>
    where TSelf : CollectionAlerts<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>, IJsonArraySource<TValue>, IEqualComparable<TSelf>
{
    public override TSelf? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType == JsonTokenType.Null ) { return null; }

        using RentedArray<TValue> values = JsonModel.ReadArrayElements(ref reader, ObservableJson.ElementInfo<TValue>(options));
        return values.ReadOnly; // one bulk insert (ICollectionAlerts' implicit conversion from ReadOnlySpan<TValue>)
    }


    public override void Write( Utf8JsonWriter writer, TSelf value, JsonSerializerOptions options ) => value.WriteJsonArray(writer, ObservableJson.ElementInfo<TValue>(options));
}



public sealed class ObservableCollectionJsonConverter<TValue> : CollectionAlertsJsonConverter<ObservableCollection<TValue>, TValue>
    where TValue : IEquatable<TValue>;



public sealed class ConcurrentObservableCollectionJsonConverter<TValue> : CollectionAlertsJsonConverter<ConcurrentObservableCollection<TValue>, TValue>
    where TValue : IEquatable<TValue>;



public sealed class ObservableHashSetJsonConverter<TValue> : CollectionAlertsJsonConverter<ObservableHashSet<TValue>, TValue>;



internal static class ObservableJson
{
    /// <summary> The element's metadata: from the serializer's own options when they have it (inside a converter), otherwise <see cref="Json.GetTypeInfo{T}"/>. </summary>
    public static JsonTypeInfo<TValue> ElementInfo<TValue>( JsonSerializerOptions? options = null ) => options is not null && options.TryGetTypeInfo(typeof(TValue), out JsonTypeInfo? info) && info is JsonTypeInfo<TValue> typed
                                                                                                           ? typed
                                                                                                           : Json.GetTypeInfo<TValue>();


    public static TSelf FromJson<TSelf, TValue>( string json )
        where TSelf : ICollectionAlerts<TSelf, TValue>
    {
        using RentedArray<TValue> values = JsonModel.FromJsonArrayPooled(json, ElementInfo<TValue>());
        return values.ReadOnly;
    }

    public static TSelf FromJson<TSelf, TValue>( ReadOnlySpan<byte> utf8Json )
        where TSelf : ICollectionAlerts<TSelf, TValue>
    {
        using RentedArray<TValue> values = JsonModel.FromJsonArrayPooled(utf8Json, ElementInfo<TValue>());
        return values.ReadOnly;
    }

    public static async ValueTask<TSelf> FromJsonAsync<TSelf, TValue>( Stream stream, CancellationToken token )
        where TSelf : ICollectionAlerts<TSelf, TValue>
    {
        using RentedArray<TValue> values = await JsonModel.FromJsonArrayPooledAsync(stream, ElementInfo<TValue>(), token).ConfigureAwait(false);
        return values.ReadOnly;
    }

    /// <summary> <see langword="false"/> for blank or malformed JSON. A missing registration (<see cref="NotSupportedException"/>) is a configuration error and propagates. </summary>
    public static bool TryFromJson<TSelf, TValue>( string? json, [NotNullWhen(true)] out TSelf? result )
        where TSelf : class, ICollectionAlerts<TSelf, TValue>
    {
        result = null;
        if ( string.IsNullOrWhiteSpace(json) ) { return false; }

        try
        {
            result = FromJson<TSelf, TValue>(json);
            return true;
        }
        catch ( JsonException ) { return false; }
    }

    public static bool TryFromJson<TSelf, TValue>( ReadOnlySpan<byte> utf8Json, [NotNullWhen(true)] out TSelf? result )
        where TSelf : class, ICollectionAlerts<TSelf, TValue>
    {
        result = null;
        if ( utf8Json.IsEmpty ) { return false; }

        try
        {
            result = FromJson<TSelf, TValue>(utf8Json);
            return true;
        }
        catch ( JsonException ) { return false; }
    }


    public static string ToString<TValue>( IEnumerable<TValue> collection, int count )
    {
        try { return JsonModel.ToJson(collection, Json.GetTypeInfo<TValue>()); }
        catch ( NotSupportedException ) { return $"{collection.GetType().Name}[{count}]"; }
    }
}



// ─── Array-shaped bases: unfiltered contents ──────────────────────────────────



public abstract partial class ObservableCollection<TSelf, TValue> : IJsonArraySource<TValue>
{
    /// <summary> Writes the unfiltered backing list. </summary>
    public virtual void WriteJsonArray( Utf8JsonWriter writer, JsonTypeInfo<TValue> info ) => JsonModel.WriteArray(writer, CollectionsMarshal.AsSpan(buffer), info);

    /// <summary> The unfiltered contents as a JSON array (instance members win over the <c> JsonModel.ToJson </c> extensions). </summary>
    public string ToJson( bool? indented = null ) => JsonModel.ToJson((IEnumerable<TValue>)this, Json.GetTypeInfo<TValue>(), indented);

    public void WriteTo( Utf8JsonWriter writer ) => WriteJsonArray(writer, Json.GetTypeInfo<TValue>());

    public override string ToString() => ObservableJson.ToString((IEnumerable<TValue>)this, Count);
}



public abstract partial class ConcurrentObservableCollection<TSelf, TValue>
{
    /// <summary> Writes a consistent snapshot: the span never escapes the lock. </summary>
    public override void WriteJsonArray( Utf8JsonWriter writer, JsonTypeInfo<TValue> info )
    {
        using ( AcquireLock() ) { base.WriteJsonArray(writer, info); }
    }
}



public abstract partial class ObservableHashSet<TSelf, TValue> : IJsonArraySource<TValue>
{
    /// <summary> Writes the unfiltered backing set. </summary>
    public virtual void WriteJsonArray( Utf8JsonWriter writer, JsonTypeInfo<TValue> info )
    {
        writer.WriteStartArray();
        foreach ( TValue value in buffer ) { JsonSerializer.Serialize(writer, value, info); } // HashSet<T>.Enumerator: no allocation, no filter

        writer.WriteEndArray();
    }

    public string ToJson( bool? indented = null ) => JsonModel.ToJson((IEnumerable<TValue>)this, Json.GetTypeInfo<TValue>(), indented);

    public void WriteTo( Utf8JsonWriter writer ) => WriteJsonArray(writer, Json.GetTypeInfo<TValue>());

    public override string ToString() => ObservableJson.ToString((IEnumerable<TValue>)this, Count);
}



// ─── Open generic types: runtime JsonTypeInfo + element-wise FromJson ────────

#if NET11_0_OR_GREATER
[JsonConverter(typeof(ObservableCollectionJsonConverter<>))]
#endif
public sealed partial class ObservableCollection<TValue>
{
    /// <summary> Runtime lookup (an open generic can't be source generated): register <c> ObservableCollection&lt;TValue&gt; </c> in a context if STJ handles it as a root. <c> FromJson </c>/<c> ToJson </c> don't need it. </summary>
    public static JsonTypeInfo<ObservableCollection<TValue>> JsonTypeInfo => Json.GetTypeInfo<ObservableCollection<TValue>>();

    public new static ObservableCollection<TValue>            FromJson( string                         json )                                                                            => ObservableJson.FromJson<ObservableCollection<TValue>, TValue>(json);
    public new static ObservableCollection<TValue>            FromJson( ReadOnlySpan<byte>             utf8Json )                                                                        => ObservableJson.FromJson<ObservableCollection<TValue>, TValue>(utf8Json);
    public new static bool                                    TryFromJson( [NotNullWhen(true)] string? json,     [NotNullWhen(true)] out ObservableCollection<TValue>? result )          => ObservableJson.TryFromJson<ObservableCollection<TValue>, TValue>(json,     out result);
    public new static bool                                    TryFromJson( ReadOnlySpan<byte>          utf8Json, [NotNullWhen(true)] out ObservableCollection<TValue>? result )          => ObservableJson.TryFromJson<ObservableCollection<TValue>, TValue>(utf8Json, out result);
    public new static ValueTask<ObservableCollection<TValue>> FromJsonAsync( Stream                    stream,   CancellationToken                                     token = default ) => ObservableJson.FromJsonAsync<ObservableCollection<TValue>, TValue>(stream, token);
}



#if NET11_0_OR_GREATER
[JsonConverter(typeof(ConcurrentObservableCollectionJsonConverter<>))]
#endif
public sealed partial class ConcurrentObservableCollection<TValue>
{
    /// <inheritdoc cref="ObservableCollection{TValue}.JsonTypeInfo"/>
    public static JsonTypeInfo<ConcurrentObservableCollection<TValue>> JsonTypeInfo => Json.GetTypeInfo<ConcurrentObservableCollection<TValue>>();

    public new static ConcurrentObservableCollection<TValue>            FromJson( string                         json )                                                                                      => ObservableJson.FromJson<ConcurrentObservableCollection<TValue>, TValue>(json);
    public new static ConcurrentObservableCollection<TValue>            FromJson( ReadOnlySpan<byte>             utf8Json )                                                                                  => ObservableJson.FromJson<ConcurrentObservableCollection<TValue>, TValue>(utf8Json);
    public new static bool                                              TryFromJson( [NotNullWhen(true)] string? json,     [NotNullWhen(true)] out ConcurrentObservableCollection<TValue>? result )          => ObservableJson.TryFromJson<ConcurrentObservableCollection<TValue>, TValue>(json,     out result);
    public new static bool                                              TryFromJson( ReadOnlySpan<byte>          utf8Json, [NotNullWhen(true)] out ConcurrentObservableCollection<TValue>? result )          => ObservableJson.TryFromJson<ConcurrentObservableCollection<TValue>, TValue>(utf8Json, out result);
    public new static ValueTask<ConcurrentObservableCollection<TValue>> FromJsonAsync( Stream                    stream,   CancellationToken                                               token = default ) => ObservableJson.FromJsonAsync<ConcurrentObservableCollection<TValue>, TValue>(stream, token);
}



#if NET11_0_OR_GREATER
[JsonConverter(typeof(ObservableHashSetJsonConverter<>))]
#endif
public partial class ObservableHashSet<TValue>
{
    /// <inheritdoc cref="ObservableCollection{TValue}.JsonTypeInfo"/>
    public static JsonTypeInfo<ObservableHashSet<TValue>> JsonTypeInfo => Json.GetTypeInfo<ObservableHashSet<TValue>>();

    public new static ObservableHashSet<TValue>            FromJson( string                         json )                                                                         => ObservableJson.FromJson<ObservableHashSet<TValue>, TValue>(json);
    public new static ObservableHashSet<TValue>            FromJson( ReadOnlySpan<byte>             utf8Json )                                                                     => ObservableJson.FromJson<ObservableHashSet<TValue>, TValue>(utf8Json);
    public new static bool                                 TryFromJson( [NotNullWhen(true)] string? json,     [NotNullWhen(true)] out ObservableHashSet<TValue>? result )          => ObservableJson.TryFromJson<ObservableHashSet<TValue>, TValue>(json,     out result);
    public new static bool                                 TryFromJson( ReadOnlySpan<byte>          utf8Json, [NotNullWhen(true)] out ObservableHashSet<TValue>? result )          => ObservableJson.TryFromJson<ObservableHashSet<TValue>, TValue>(utf8Json, out result);
    public new static ValueTask<ObservableHashSet<TValue>> FromJsonAsync( Stream                    stream,   CancellationToken                                  token = default ) => ObservableJson.FromJsonAsync<ObservableHashSet<TValue>, TValue>(stream, token);
}



// ─── Dictionaries: object-shaped, metadata only ───────────────────────────────



public sealed partial class ObservableDictionary<TKey, TValue>
{
    /// <summary> Runtime lookup (an open generic can't be source generated): register <c> ObservableDictionary&lt;TKey, TValue&gt; </c> in a context to (de)serialize it. </summary>
    public static JsonTypeInfo<ObservableDictionary<TKey, TValue>> JsonTypeInfo => Json.GetTypeInfo<ObservableDictionary<TKey, TValue>>();
}



public sealed partial class ObservableConcurrentDictionary<TKey, TValue>
{
    /// <inheritdoc cref="ObservableDictionary{TKey,TValue}.JsonTypeInfo"/>
    public static JsonTypeInfo<ObservableConcurrentDictionary<TKey, TValue>> JsonTypeInfo => Json.GetTypeInfo<ObservableConcurrentDictionary<TKey, TValue>>();
}
