// Jakar.SystemTextJson
// 10/02/2026

namespace Jakar.Extensions;


/// <summary> AOT-safe System.Text.Json helpers. Everything takes source-generated metadata (<see cref="JsonTypeInfo{T}"/>); nothing here uses reflection-based serialization. </summary>
/// <remarks> The signatures used by generated code are public API: generated code compiled into consumer assemblies calls them. </remarks>
public static partial class JsonModel
{
    private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];


    // ─── Single values ────────────────────────────────────────────────────────

    /// <exception cref="JsonException"> The JSON is malformed, or its root is <c> null </c>. </exception>
    public static T FromJson<T>( string json, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(info);
        return JsonSerializer.Deserialize(json, info) ?? throw NullRoot<T>();
    }

    /// <inheritdoc cref="FromJson{T}(string, JsonTypeInfo{T})"/>
    public static T FromJson<T>( ReadOnlySpan<byte> utf8Json, JsonTypeInfo<T> info )
    {
        ArgumentNullException.ThrowIfNull(info);
        return JsonSerializer.Deserialize(SkipBom(utf8Json), info) ?? throw NullRoot<T>();
    }

    /// <inheritdoc cref="FromJson{T}(string, JsonTypeInfo{T})"/>
    public static async ValueTask<T> FromJsonAsync<T>( Stream stream, JsonTypeInfo<T> info, CancellationToken token = default )
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(info);
        return await JsonSerializer.DeserializeAsync(stream, info, token).ConfigureAwait(false) ?? throw NullRoot<T>();
    }


    /// <summary> <see langword="false"/> for <see langword="null"/>/blank input, malformed JSON, or a JSON <c> null </c> root. </summary>
    public static bool TryFromJson<T>( [NotNullWhen(true)] string? json, JsonTypeInfo<T> info, [NotNullWhen(true)] out T? result )
    {
        ArgumentNullException.ThrowIfNull(info);

        if ( !string.IsNullOrWhiteSpace(json) )
        {
            try
            {
                result = JsonSerializer.Deserialize(json, info);
                if ( result is not null ) { return true; }
            }
            catch ( JsonException ) { }
        }

        result = default;
        return false;
    }

    /// <inheritdoc cref="TryFromJson{T}(string, JsonTypeInfo{T}, out T)"/>
    public static bool TryFromJson<T>( ReadOnlySpan<byte> utf8Json, JsonTypeInfo<T> info, [NotNullWhen(true)] out T? result )
    {
        ArgumentNullException.ThrowIfNull(info);
        utf8Json = SkipBom(utf8Json);

        if ( !utf8Json.IsEmpty )
        {
            try
            {
                result = JsonSerializer.Deserialize(utf8Json, info);
                if ( result is not null ) { return true; }
            }
            catch ( JsonException ) { }
        }

        result = default;
        return false;
    }


    /// <summary> Serializes <paramref name="value"/>. <paramref name="indented"/> overrides the context's <see cref="JsonSerializerOptions.WriteIndented"/> for this call. </summary>
    public static string ToJson<T>( T value, JsonTypeInfo<T> info, bool? indented = null )
    {
        ArgumentNullException.ThrowIfNull(info);
        if ( indented is null || indented == info.Options.WriteIndented ) { return JsonSerializer.Serialize(value, info); }

        ArrayBufferWriter<byte> buffer = new(256);
        using ( Utf8JsonWriter writer = new(buffer, info.Options.GetWriterOptions(indented)) ) { JsonSerializer.Serialize(writer, value, info); }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }



    extension<TSelf>( TSelf self )
        where TSelf : IJsonModel<TSelf>
    {
        /// <summary> Serializes with <typeparamref name="TSelf"/>'s source-generated metadata. </summary>
        public string ToJson( bool? indented = null ) => ToJson(self, TSelf.JsonTypeInfo, indented);

        public byte[]      ToUtf8Json()                                                        => JsonSerializer.SerializeToUtf8Bytes(self, TSelf.JsonTypeInfo);
        public JsonNode?   ToJsonNode()                                                        => JsonSerializer.SerializeToNode(self, TSelf.JsonTypeInfo);
        public JsonElement ToJsonElement()                                                     => JsonSerializer.SerializeToElement(self, TSelf.JsonTypeInfo);
        public void        WriteTo( Utf8JsonWriter writer )                                    => JsonSerializer.Serialize(writer, self, TSelf.JsonTypeInfo);
        public Task        WriteToAsync( Stream    stream, CancellationToken token = default ) => JsonSerializer.SerializeAsync(stream, self, TSelf.JsonTypeInfo, token);
    }



    /// <summary>
    ///     Copies every property System.Text.Json reads and writes (including the extension-data bag) from <paramref name="source"/> to <paramref name="destination"/>,
    ///     through the metadata's source-generated getters and setters: no reflection. Properties without a setter are skipped.
    /// </summary>
    /// <exception cref="InvalidOperationException"> <typeparamref name="T"/> isn't serialized as a JSON object (e.g. it has a custom converter). </exception>
    public static void CopyProperties<T>( T source, T destination, JsonTypeInfo<T> info )
        where T : class
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(info);
        if ( info.Kind != JsonTypeInfoKind.Object ) { throw new InvalidOperationException($"{typeof(T).GetDisplayName()} isn't serialized as a JSON object ({info.Kind}), so it has no property metadata to copy."); }

        foreach ( JsonPropertyInfo property in info.Properties )
        {
            if ( property.Get is null || property.Set is null ) { continue; }

            property.Set(destination, property.Get(source));
        }
    }


    // ─── Options bridging ─────────────────────────────────────────────────────



    extension( JsonSerializerOptions options )
    {
        /// <summary> Reader settings matching the serializer's tolerance (trailing commas, comments, max depth), for code that drives a <see cref="Utf8JsonReader"/> itself. </summary>
        public JsonReaderOptions GetReaderOptions() => new()
                                                       {
                                                           AllowTrailingCommas = options.AllowTrailingCommas,
                                                           CommentHandling     = options.ReadCommentHandling,
                                                           MaxDepth            = options.MaxDepth // 0 = default (64) for both
                                                       };


        /// <summary> Writer settings matching the serializer's output (encoder, indentation, new line, max depth). <paramref name="indented"/> overrides <see cref="JsonSerializerOptions.WriteIndented"/>. </summary>
        public JsonWriterOptions GetWriterOptions( bool? indented = null ) => new()
                                                                              {
                                                                                  Encoder         = options.Encoder,
                                                                                  Indented        = indented ?? options.WriteIndented,
                                                                                  IndentCharacter = options.IndentCharacter,
                                                                                  IndentSize      = options.IndentSize,
                                                                                  NewLine         = options.NewLine,
                                                                                  MaxDepth        = options.MaxDepth
                                                                              };


        /// <summary> AOT-safe: resolves from the options' (source-generated) resolver chain, no reflection. </summary>
        /// <exception cref="NotSupportedException"> <typeparamref name="T"/> isn't registered with the options' resolvers. </exception>
        public JsonTypeInfo<T> GetRequiredTypeInfo<T>()
        {
            if ( options.TryGetTypeInfo(typeof(T), out JsonTypeInfo? info) && info is JsonTypeInfo<T> typed ) { return typed; }

            string name = typeof(T).GetDisplayName();
            throw new NotSupportedException($"No JSON metadata is registered for '{name}'. Add [JsonSerializable(typeof({name}))] to a JsonSerializerContext and register it with Json.AddResolver(...) at startup, or put [JsonModel] on the type.");
        }
    }



    extension( Type type )
    {
        /// <summary> C#-style name: <c> ObservableCollection&lt;Group&gt; </c> rather than <c> ObservableCollection`1[[...]] </c>. </summary>
        internal string GetDisplayName()
        {
            if ( type.IsArray ) { return $"{type.GetElementType()!.GetDisplayName()}[]"; }

            if ( !type.IsGenericType ) { return type.Name; }

            if ( type.GetGenericTypeDefinition() == typeof(Nullable<>) ) { return $"{type.GetGenericArguments()[0].GetDisplayName()}?"; }

            string name = type.Name;
            int    tick = name.IndexOf('`');
            if ( tick >= 0 ) { name = name[..tick]; }

            StringBuilder builder = new(name);
            builder.Append('<');
            Type[] arguments = type.GetGenericArguments();

            for ( int i = 0; i < arguments.Length; i++ )
            {
                if ( i > 0 ) { builder.Append(", "); }

                builder.Append(arguments[i].GetDisplayName());
            }

            return builder.Append('>').ToString();
        }
    }



    internal static ReadOnlySpan<byte> SkipBom( ReadOnlySpan<byte> utf8Json ) => utf8Json.StartsWith(Utf8Bom)
                                                                                     ? utf8Json[Utf8Bom.Length..]
                                                                                     : utf8Json;

    private static JsonException NullRoot<T>() => new($"Expected a {typeof(T).GetDisplayName()}, but the JSON root is null.");
}
