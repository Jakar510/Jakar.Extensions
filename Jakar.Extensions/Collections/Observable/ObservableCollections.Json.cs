// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     JSON for the observable collections, on Jakar.Json.
///     <list type="bullet">
///         <item> They implement <see cref="IJsonCollection{TSelf,T}"/>, so a <c> [JsonModel] </c> member of such a type writes the <b> unfiltered </b> contents (<see cref="CollectionAlerts{TSelf,TValue}.OverrideFilter"/> is a view concern) and reads with one bulk insert (one notification, one lock). </item>
///         <item> As a root, <c> ToJson() </c> / <c> FromJson(json) </c> do the same for <c> [JsonModel] </c> elements (<see cref="IJsonSerializable{TSelf}"/>). </item>
///     </list>
/// </summary>
public static class ObservableJson
{
    public static string ToJson<TSelf, TValue>( TSelf collection, bool indented = false )
        where TSelf : IJsonCollection<TSelf, TValue>
        where TValue : IJsonSerializable<TValue> => Json.Serialize<TSelf, JsonCollectionConverter<TSelf, TValue, JsonModelConverter<TValue>>>(in collection, indented);


    /// <exception cref="JsonReadException"> Invalid JSON, or JSON that isn't an array of <typeparamref name="TValue"/>. </exception>
    public static TSelf FromJson<TSelf, TValue>( scoped ReadOnlySpan<char> json )
        where TSelf : IJsonCollection<TSelf, TValue>
        where TValue : IJsonSerializable<TValue> => Json.Deserialize<TSelf, JsonCollectionConverter<TSelf, TValue, JsonModelConverter<TValue>>>(json);

    /// <summary> <see langword="false"/> for blank or malformed JSON. </summary>
    public static bool TryFromJson<TSelf, TValue>( [NotNullWhen(true)] string? json, [NotNullWhen(true)] out TSelf? result )
        where TSelf : class, IJsonCollection<TSelf, TValue>
        where TValue : IJsonSerializable<TValue>
    {
        result = null;
        return !string.IsNullOrWhiteSpace(json) && Json.TryDeserialize<TSelf, JsonCollectionConverter<TSelf, TValue, JsonModelConverter<TValue>>>(json, out result);
    }


    /// <summary> Any element type, through <typeparamref name="TConverter"/> (e.g. <c> JsonIntegerConverter&lt;int, ...&gt; </c>). </summary>
    public static string ToJson<TSelf, TValue, TConverter>( TSelf collection, bool indented = false )
        where TSelf : IJsonCollection<TSelf, TValue>
        where TConverter : IJsonConverter<TValue> => Json.Serialize<TSelf, JsonCollectionConverter<TSelf, TValue, TConverter>>(in collection, indented);

    /// <inheritdoc cref="ToJson{TSelf, TValue, TConverter}"/>
    public static TSelf FromJson<TSelf, TValue, TConverter>( scoped ReadOnlySpan<char> json )
        where TSelf : IJsonCollection<TSelf, TValue>
        where TConverter : IJsonConverter<TValue> => Json.Deserialize<TSelf, JsonCollectionConverter<TSelf, TValue, TConverter>>(json);


    /// <summary> The JSON array when <typeparamref name="TValue"/> is a registered <c> [JsonModel] </c> type (<see cref="JsonCodecRegistry"/>), otherwise the type name and count. </summary>
    internal static string ToString<TValue>( IEnumerable<TValue> collection, int count, string typeName )
    {
        if ( !JsonCodecRegistry.TryGet(typeof(TValue), out JsonTypeCodec? codec) ) { return $"{typeName}[{count}]"; }

        JsonWriter writer = new(stackalloc char[256], codec.DefaultWriterOptions);

        try
        {
            writer.WriteStartArray();
            foreach ( TValue value in collection ) { codec.WriteValue(value, ref writer); }

            writer.WriteEndArray();
            return writer.ToString();
        }
        finally { writer.Dispose(); }
    }
}



// ─── Array-shaped bases: unfiltered contents ──────────────────────────────────



public abstract partial class ObservableCollection<TSelf, TValue> : IJsonCollection<TSelf, TValue>
{
    /// <summary> The unfiltered backing list. </summary>
    public virtual void WriteJsonElements<TWriter, TConverter>( ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<TValue>
    {
        foreach ( ref readonly TValue value in CollectionsMarshal.AsSpan(buffer) ) { TConverter.Write(ref writer, in value); }
    }

    /// <summary> Every element at once (one notification). </summary>
    public static TSelf FromJsonElements( ReadOnlySpan<TValue> elements ) => elements;

    public override string ToString() => ObservableJson.ToString(buffer, Count, GetType().Name);
}



public abstract partial class ConcurrentObservableCollection<TSelf, TValue>
{
    /// <summary> Writes a consistent snapshot: the span never escapes the lock. </summary>
    public override void WriteJsonElements<TWriter, TConverter>( ref TWriter writer )
    {
        using ( AcquireLock() ) { base.WriteJsonElements<TWriter, TConverter>(ref writer); }
    }
}



public abstract partial class ObservableHashSet<TSelf, TValue> : IJsonCollection<TSelf, TValue>
{
    /// <summary> The unfiltered backing set, in its enumeration order. </summary>
    public virtual void WriteJsonElements<TWriter, TConverter>( ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<TValue>
    {
        foreach ( TValue value in buffer ) { TConverter.Write(ref writer, in value); }
    }

    /// <summary> Every element at once (one notification). </summary>
    public static TSelf FromJsonElements( ReadOnlySpan<TValue> elements ) => elements;

    public override string ToString() => ObservableJson.ToString(buffer, Count, GetType().Name);
}



// ─── ToJson / FromJson for [JsonModel] elements ───────────────────────────────



public static class ObservableCollectionJsonExtensions
{
    extension<TSelf, TValue>( ObservableCollection<TSelf, TValue> self )
        where TSelf : ObservableCollection<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>
        where TValue : IEquatable<TValue>, IJsonSerializable<TValue>
    {
        /// <summary> The unfiltered contents as a JSON array. </summary>
        public string ToJson( bool indented = false ) => ObservableJson.ToJson<TSelf, TValue>((TSelf)self, indented);
    }



    extension<TSelf, TValue>( ObservableHashSet<TSelf, TValue> self )
        where TSelf : ObservableHashSet<TSelf, TValue>, ICollectionAlerts<TSelf, TValue>
        where TValue : IJsonSerializable<TValue>
    {
        /// <summary> The unfiltered contents as a JSON array. </summary>
        public string ToJson( bool indented = false ) => ObservableJson.ToJson<TSelf, TValue>((TSelf)self, indented);
    }
}
