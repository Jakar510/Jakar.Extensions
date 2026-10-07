// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Typed get / add / update / delete helpers for <see cref="JsonNode"/> and <see cref="JsonElement"/> (AOT-plan §2.11).
///     <para> Every member that takes a <c> T </c> has an overload with a <see cref="JsonTypeInfo{T}"/> and one without (resolved by <see cref="GetTypeInfo{T}"/>). </para>
///     <list type="table">
///         <listheader> <term> Case </term> <description> TryGet / Get / GetOrDefault </description> </listheader>
///         <item> <term> key/index missing, or not an object/array </term> <description> <see langword="false"/> / throws <see cref="KeyNotFoundException"/> / <c> defaultValue </c> </description> </item>
///         <item> <term> present, JSON <c> null </c> </term> <description> <see langword="true"/> with <c> default </c> / <c> default </c> / <c> defaultValue </c> </description> </item>
///         <item> <term> present, wrong shape </term> <description> <see langword="false"/> / throws <see cref="JsonException"/> / <c> defaultValue </c> </description> </item>
///     </list>
///     <para> Writing a node that already has a parent clones it. <see cref="JsonElement"/> is immutable, so its write helpers take it by <see langword="ref"/> and replace it (O(n) each: convert to a <see cref="JsonObject"/> for several edits). Not thread-safe. </para>
/// </summary>
public static partial class Json
{
    // ─── Reading: JsonNode ────────────────────────────────────────────────────



    extension( JsonNode? self )
    {
        public bool Contains( string key ) => self is JsonObject obj && obj.ContainsKey(key);


        public bool TryGet<T>( string key, JsonTypeInfo<T> info, out T? value )
        {
            value = default;
            if ( self is not JsonObject obj || !obj.TryGetPropertyValue(key, out JsonNode? node) ) { return false; }

            return TryConvert(node, info, out value);
        }

        public bool TryGet<T>( string key, out T? value ) => self.TryGet(key, GetTypeInfo<T>(), out value);


        public bool TryGet<T>( int index, JsonTypeInfo<T> info, out T? value )
        {
            value = default;
            if ( self is not JsonArray array || (uint)index >= (uint)array.Count ) { return false; }

            return TryConvert(array[index], info, out value);
        }

        public bool TryGet<T>( int index, out T? value ) => self.TryGet(index, GetTypeInfo<T>(), out value);


        /// <exception cref="KeyNotFoundException"> <paramref name="key"/> is missing. </exception>
        /// <exception cref="JsonException"> The value has the wrong shape for <typeparamref name="T"/>. </exception>
        public T? Get<T>( string key, JsonTypeInfo<T> info )
        {
            if ( self is not JsonObject obj || !obj.TryGetPropertyValue(key, out JsonNode? node) ) { throw new KeyNotFoundException(key); }

            return Convert(node, info);
        }

        public T? Get<T>( string key ) => self.Get(key, GetTypeInfo<T>());


        public T? GetOrDefault<T>( string key, JsonTypeInfo<T> info, T? defaultValue = default ) => self.TryGet(key, info, out T? value) && value is not null
                                                                                                        ? value
                                                                                                        : defaultValue;

        public T? GetOrDefault<T>( string key, T? defaultValue = default ) => self.GetOrDefault(key, GetTypeInfo<T>(), defaultValue);


        /// <summary> The node itself as <typeparamref name="T"/>. </summary>
        public T? As<T>( JsonTypeInfo<T> info ) => Convert(self, info);

        public T? As<T>() => self.As(GetTypeInfo<T>());
    }



    // ─── Writing: JsonObject / JsonArray (in place) ───────────────────────────



    extension( JsonObject self )
    {
        /// <summary> <see langword="false"/> if <paramref name="key"/> already exists. </summary>
        public bool TryAdd<T>( string key, T value, JsonTypeInfo<T> info )
        {
            if ( self.ContainsKey(key) ) { return false; }

            self[key] = ToNode(value, info);
            return true;
        }

        public bool TryAdd<T>( string key, T value ) => self.TryAdd(key, value, GetTypeInfo<T>());


        /// <summary> <see langword="false"/> if <paramref name="key"/> is missing. </summary>
        public bool TryUpdate<T>( string key, T value, JsonTypeInfo<T> info )
        {
            if ( !self.ContainsKey(key) ) { return false; }

            self[key] = ToNode(value, info);
            return true;
        }

        public bool TryUpdate<T>( string key, T value ) => self.TryUpdate(key, value, GetTypeInfo<T>());


        /// <summary> Adds or updates. </summary>
        public void Set<T>( string key, T value, JsonTypeInfo<T> info ) => self[key] = ToNode(value, info);

        public void Set<T>( string key, T value ) => self.Set(key, value, GetTypeInfo<T>());


        public bool Remove( string key, out JsonNode? removed ) => self.Remove(key, out removed, out _);

        public bool Remove( string key, out JsonNode? removed, out bool existed )
        {
            existed = self.TryGetPropertyValue(key, out removed);
            if ( !existed ) { return false; }

            self.Remove(key);
            removed = removed?.DeepClone();
            return true;
        }


        public JsonElement ToJsonElement() => JsonSerializer.SerializeToElement(self, JakarExtensionsContext.Default.JsonObject);
    }



    extension( JsonArray self )
    {
        public void Add<T>( T      value, JsonTypeInfo<T> info )                        => self.Add(ToNode(value,           info));
        public void Insert<T>( int index, T               value, JsonTypeInfo<T> info ) => self.Insert(index, ToNode(value, info));
        public void Set<T>( int    index, T               value, JsonTypeInfo<T> info ) => self[index] = ToNode(value, info);

        public void Add<T>( T      value )          => self.Add(value, GetTypeInfo<T>());
        public void Insert<T>( int index, T value ) => self.Insert(index, value, GetTypeInfo<T>());
        public void Set<T>( int    index, T value ) => self.Set(index, value, GetTypeInfo<T>());
    }



    // ─── Reading: JsonElement ─────────────────────────────────────────────────



    extension( ref readonly JsonElement self )
    {
        public bool Contains( string key, StringComparison comparison = StringComparison.Ordinal ) => self.TryFind(key, comparison, out _);


        public bool TryGet<T>( string key, JsonTypeInfo<T> info, out T? value, StringComparison comparison = StringComparison.Ordinal )
        {
            value = default;
            return self.TryFind(key, comparison, out JsonElement element) && TryConvert(element, info, out value);
        }

        public bool TryGet<T>( string key, out T? value, StringComparison comparison = StringComparison.Ordinal ) => self.TryGet(key, GetTypeInfo<T>(), out value, comparison);


        public bool TryGet<T>( int index, JsonTypeInfo<T> info, out T? value )
        {
            value = default;
            if ( self.ValueKind != JsonValueKind.Array || (uint)index >= (uint)self.GetArrayLength() ) { return false; }

            JsonElement element = self[index];
            return element.TryConvert(info, out value);
        }

        public bool TryGet<T>( int index, out T? value ) => self.TryGet(index, GetTypeInfo<T>(), out value);


        /// <exception cref="KeyNotFoundException"> <paramref name="key"/> is missing. </exception>
        /// <exception cref="JsonException"> The value has the wrong shape for <typeparamref name="T"/>. </exception>
        public T? Get<T>( string key, JsonTypeInfo<T> info, StringComparison comparison = StringComparison.Ordinal ) => self.TryFind(key, comparison, out JsonElement element)
                                                                                                                            ? element.Deserialize(info)
                                                                                                                            : throw new KeyNotFoundException(key);

        public T? Get<T>( string key, StringComparison comparison = StringComparison.Ordinal ) => self.Get(key, GetTypeInfo<T>(), comparison);


        public T? GetOrDefault<T>( string key, JsonTypeInfo<T> info, T? defaultValue = default, StringComparison comparison = StringComparison.Ordinal ) => self.TryGet(key, info, out T? value, comparison) && value is not null
                                                                                                                                                                ? value
                                                                                                                                                                : defaultValue;

        public T? GetOrDefault<T>( string key, T? defaultValue = default, StringComparison comparison = StringComparison.Ordinal ) => self.GetOrDefault(key, GetTypeInfo<T>(), defaultValue, comparison);


        public T? As<T>( JsonTypeInfo<T> info ) => self.Deserialize(info);
        public T? As<T>()                       => self.Deserialize(GetTypeInfo<T>());


        public JsonObject? ToJsonObject() => self.ValueKind == JsonValueKind.Object
                                                 ? JsonObject.Create(self, NodeOptions)
                                                 : null;


        /// <summary> Duplicate keys: the last one wins, as in deserialization. </summary>
        private bool TryFind( string key, StringComparison comparison, out JsonElement value )
        {
            value = default;
            if ( self.ValueKind != JsonValueKind.Object ) { return false; }

            if ( comparison == StringComparison.Ordinal )
            {
                bool found = false;

                foreach ( JsonProperty property in self.EnumerateObject() )
                {
                    if ( !property.NameEquals(key) ) { continue; }

                    value = property.Value;
                    found = true;
                }

                return found;
            }

            bool any = false;

            foreach ( JsonProperty property in self.EnumerateObject() )
            {
                if ( !string.Equals(property.Name, key, comparison) ) { continue; }

                value = property.Value;
                any   = true;
            }

            return any;
        }
        /// <summary> <paramref name="removeAt"/> &lt; 0 appends <paramref name="append"/>; otherwise removes that index. </summary>
        private JsonElement RewriteArray( int removeAt, JsonElement? append )
        {
            ArrayBufferWriter<byte> buffer = new(256);

            using ( Utf8JsonWriter writer = new(buffer) )
            {
                writer.WriteStartArray();
                int i = 0;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach ( JsonElement item in self.EnumerateArray() )
                {
                    if ( i++ == removeAt ) { continue; }

                    item.WriteTo(writer);
                }

                append?.WriteTo(writer);
                writer.WriteEndArray();
            }

            return Parse(buffer.WrittenSpan);
        }
    }



    // ─── Writing: JsonElement (immutable, so the receiver is replaced) ────────



    extension( ref JsonElement self )
    {
        /// <summary> <see langword="false"/> if <paramref name="key"/> already exists, or the element isn't an object. </summary>
        public bool TryAdd<T>( string key, T value, JsonTypeInfo<T> info )
        {
            if ( self.ValueKind != JsonValueKind.Object || self.Contains(key) ) { return false; }

            self.RewriteObject(key, JsonSerializer.SerializeToElement(value, info));
            return true;
        }

        /// <summary> <see langword="false"/> if <paramref name="key"/> is missing. Updates every duplicate. </summary>
        public bool TryUpdate<T>( string key, T value, JsonTypeInfo<T> info )
        {
            if ( !self.Contains(key) ) { return false; }

            self.RewriteObject(key, JsonSerializer.SerializeToElement(value, info));
            return true;
        }

        /// <summary> Adds or updates. </summary>
        /// <exception cref="InvalidOperationException"> The element isn't a JSON object. </exception>
        public void Set<T>( string key, T value, JsonTypeInfo<T> info )
        {
            if ( self.ValueKind != JsonValueKind.Object ) { throw new InvalidOperationException($"Expected a JSON object, but found {self.ValueKind}."); }

            self.RewriteObject(key, JsonSerializer.SerializeToElement(value, info));
        }

        /// <summary> Removes every occurrence of <paramref name="key"/>. </summary>
        public bool Remove( string key )
        {
            if ( !self.Contains(key) ) { return false; }

            self.RewriteObject(key, null);
            return true;
        }

        /// <summary> Appends to a JSON array. </summary>
        /// <exception cref="InvalidOperationException"> The element isn't a JSON array. </exception>
        public void Add<T>( T value, JsonTypeInfo<T> info )
        {
            if ( self.ValueKind != JsonValueKind.Array ) { throw new InvalidOperationException($"Expected a JSON array, but found {self.ValueKind}."); }

            self = self.RewriteArray(-1, JsonSerializer.SerializeToElement(value, info));
        }

        public bool RemoveAt( int index )
        {
            if ( self.ValueKind != JsonValueKind.Array || (uint)index >= (uint)self.GetArrayLength() ) { return false; }

            self = self.RewriteArray(index, null);
            return true;
        }

        public bool TryAdd<T>( string    key, T value ) => self.TryAdd(key, value, GetTypeInfo<T>());
        public bool TryUpdate<T>( string key, T value ) => self.TryUpdate(key, value, GetTypeInfo<T>());
        public void Set<T>( string       key, T value ) => self.Set(key, value, GetTypeInfo<T>());
        public void Add<T>( T            value ) => self.Add(value, GetTypeInfo<T>());


        /// <summary> Copies every property except <paramref name="key"/>, then writes <paramref name="replacement"/> under it (or nothing, to remove). The result owns its memory. </summary>
        private void RewriteObject( string key, JsonElement? replacement )
        {
            ArrayBufferWriter<byte> buffer = new(256);

            using ( Utf8JsonWriter writer = new(buffer) )
            {
                writer.WriteStartObject();

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach ( JsonProperty property in self.EnumerateObject() )
                {
                    if ( property.NameEquals(key) ) { continue; }

                    property.WriteTo(writer);
                }

                if ( replacement is { } value )
                {
                    writer.WritePropertyName(key);
                    value.WriteTo(writer);
                }

                writer.WriteEndObject();
            }

            self = Parse(buffer.WrittenSpan);
        }
    }



    // ─── Internals ────────────────────────────────────────────────────────────


    private static JsonElement Parse( ReadOnlySpan<byte> utf8 )
    {
        Utf8JsonReader reader = new(utf8);
        return JsonElement.ParseValue(ref reader);
    }


    /// <summary> Writing a node that already has a parent would throw; clone it instead. </summary>
    private static JsonNode? ToNode<T>( T value, JsonTypeInfo<T> info ) => value switch
                                                                           {
                                                                               null                            => null,
                                                                               JsonNode { Parent: not null } n => n.DeepClone(),
                                                                               JsonNode n                      => n,
                                                                               _                               => JsonSerializer.SerializeToNode(value, info)
                                                                           };


    private static T? Convert<T>( JsonNode? node, JsonTypeInfo<T> info ) => node is null
                                                                                ? default
                                                                                : node.Deserialize(info);


    private static bool TryConvert<T>( JsonNode? node, JsonTypeInfo<T> info, out T? value )
    {
        try
        {
            value = Convert(node, info);
            return true;
        }
        catch ( Exception e ) when ( e is JsonException or FormatException or InvalidOperationException or NotSupportedException )
        {
            value = default;
            return false;
        }
    }


    private static bool TryConvert<T>( this ref readonly JsonElement element, JsonTypeInfo<T> info, out T? value )
    {
        try
        {
            value = element.ValueKind == JsonValueKind.Null
                        ? default
                        : element.Deserialize(info);

            return true;
        }
        catch ( Exception e ) when ( e is JsonException or FormatException or InvalidOperationException )
        {
            value = default;
            return false;
        }
    }
}
