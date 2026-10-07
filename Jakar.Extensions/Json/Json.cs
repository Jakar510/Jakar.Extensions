// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

namespace Jakar.Extensions;


/// <summary>
///     System.Text.Json helpers. Everything is AOT-safe: metadata comes from source-generated contexts, never from reflection.
///     <para> Two tiers: members that take a <see cref="JsonTypeInfo{T}"/> (compile-time checked), and members without one that resolve it through <see cref="GetTypeInfo{T}"/>: a [JsonModel] registration first, then <see cref="Options"/>' resolver chain. Those throw <see cref="NotSupportedException"/> naming the type when it isn't registered. </para>
///     <para> <see cref="IJsonModel{TSelf}"/> types also get <c> value.ToJson() </c> / <c> T.FromJson(json) </c> from Jakar.SystemTextJson. </para>
/// </summary>
public static partial class Json
{
    private static readonly Lock                        __lock      = new();
    private static readonly List<IJsonTypeInfoResolver> __resolvers = [];
    private static          JsonSerializerOptions?      __options;


    /// <summary>
    ///     Options for the tier without <see cref="JsonTypeInfo{T}"/>. Matches <see cref="JakarExtensionsContext"/>'s settings: lenient reading (case-insensitive names, numbers in strings,
    ///     comments, trailing commas, public fields) so JSON written by Jakar.Extensions 10.x (Newtonsoft) still reads, PascalCase names, and indented output.
    ///     <para> Frozen on first use; register resolvers before that with <see cref="AddResolver"/>. </para>
    /// </summary>
    public static JsonSerializerOptions Options => Volatile.Read(ref __options) ?? CreateOptions();


    /// <summary> Adds a source-generated context (or any resolver) to <see cref="Options"/>. Resolvers added later are consulted first, so an app can override library metadata. Call at startup, before any serialization. </summary>
    /// <exception cref="InvalidOperationException"> <see cref="Options"/> is already in use. </exception>
    public static void AddResolver( IJsonTypeInfoResolver resolver )
    {
        ArgumentNullException.ThrowIfNull(resolver);

        lock ( __lock )
        {
            if ( __options is not null ) { throw new InvalidOperationException($"{nameof(Json)}.{nameof(Options)} is already in use; call {nameof(AddResolver)} at startup, before any serialization."); }

            if ( !__resolvers.Contains(resolver) ) { __resolvers.Insert(0, resolver); }
        }
    }


    private static JsonSerializerOptions CreateOptions()
    {
        lock ( __lock )
        {
            if ( __options is not null ) { return __options; }

            JsonSerializerOptions options = new(JakarExtensionsContext.Default.Options) { TypeInfoResolver = JsonTypeInfoResolver.Combine([.. __resolvers, JakarExtensionsContext.Default, UserGuid.UserGuidJsonContext.Default, UserLong.UserLongJsonContext.Default]) };
            options.MakeReadOnly();
            Volatile.Write(ref __options, options);
            return options;
        }
    }


    /// <summary> Metadata for <typeparamref name="T"/>: a [JsonModel] registration first (<see cref="JsonModelRegistry"/>), then <see cref="Options"/>' resolver chain. Cached per type. </summary>
    /// <exception cref="NotSupportedException"> <typeparamref name="T"/> isn't registered anywhere. </exception>
    public static JsonTypeInfo<T> GetTypeInfo<T>() => TypeInfoCache<T>.Value ??= JsonModelRegistry.TryGet(out JsonTypeInfo<T>? info)
                                                                                     ? info
                                                                                     : Options.GetRequiredTypeInfo<T>();


    /// <summary> Metadata for a runtime <paramref name="type"/> (e.g. the actual type behind a base-class reference): a [JsonModel] registration first, then <see cref="Options"/>' resolver chain. </summary>
    /// <exception cref="NotSupportedException"> <paramref name="type"/> isn't registered anywhere. </exception>
    public static JsonTypeInfo GetTypeInfo( Type type )
    {
        ArgumentNullException.ThrowIfNull(type);
        if ( JsonModelRegistry.TryGet(type, out JsonTypeInfo? info) || Options.TryGetTypeInfo(type, out info) ) { return info; }

        throw new NotSupportedException($"No JSON metadata is registered for '{type.Name}'. Add [JsonSerializable(typeof({type.Name}))] to a JsonSerializerContext and register it with Json.AddResolver(...) at startup, or put [JsonModel] on the type.");
    }


    public static bool TryGetTypeInfo<T>( [NotNullWhen(true)] out JsonTypeInfo<T>? info )
    {
        try
        {
            info = GetTypeInfo<T>();
            return true;
        }
        catch ( NotSupportedException )
        {
            info = null;
            return false;
        }
    }



    private static class TypeInfoCache<T>
    {
        // Benign race: concurrent first calls resolve the same metadata. Not a static readonly initializer, so a missing registration
        // surfaces as NotSupportedException rather than TypeInitializationException.
        public static JsonTypeInfo<T>? Value;
    }



    // ─── Serialize ────────────────────────────────────────────────────────────

    /// <summary> Serializes with <see cref="GetTypeInfo{T}"/>. For <see cref="IJsonModel{TSelf}"/> types prefer <c> value.ToJson() </c>. </summary>
    public static string      Serialize<T>( T                      value, bool? indented = null ) => JsonModel.ToJson(value, GetTypeInfo<T>(), indented);
    public static byte[]      SerializeToUtf8Bytes<T>( T           value )                                                  => JsonSerializer.SerializeToUtf8Bytes(value, GetTypeInfo<T>());
    public static JsonNode?   SerializeToNode<T>( T                value )                                                  => JsonSerializer.SerializeToNode(value, GetTypeInfo<T>());
    public static JsonElement SerializeToElement<T>( T             value )                                                  => JsonSerializer.SerializeToElement(value, GetTypeInfo<T>());
    public static string      Serialize<T>( scoped ReadOnlySpan<T> values, bool? indented                       = null )    => JsonModel.ToJson(values, GetTypeInfo<T>(), indented);
    public static string      Serialize<T>( IEnumerable<T>         values, bool? indented                       = null )    => JsonModel.ToJson(values, GetTypeInfo<T>(), indented);
    public static Task        SerializeAsync<T>( Stream            stream, T     value, CancellationToken token = default ) => JsonSerializer.SerializeAsync(stream, value, GetTypeInfo<T>(), token);


    public static string ToJson( this JsonNode node, bool indented = true ) => node.ToJsonString(Options.GetIndented(indented));
    public static string ToJson( this JsonElement element, bool indented = true )
    {
        if ( !indented ) { return element.GetRawText(); }

        ArrayBufferWriter<byte> buffer = new(256);
        using ( Utf8JsonWriter writer = new(buffer, Options.GetWriterOptions(true)) ) { element.WriteTo(writer); }

        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }


    private static JsonSerializerOptions? __compact;

    private static JsonSerializerOptions GetIndented( this JsonSerializerOptions options, bool indented )
    {
        if ( indented == options.WriteIndented ) { return options; }

        JsonSerializerOptions? compact = Volatile.Read(ref __compact);
        if ( compact is not null ) { return compact; }

        compact = new JsonSerializerOptions(options) { WriteIndented = indented };
        compact.MakeReadOnly();
        return Interlocked.CompareExchange(ref __compact, compact, null) ?? compact;
    }


    // ─── Deserialize ──────────────────────────────────────────────────────────



    extension( string? self )
    {
        /// <summary> Parses JSON into a <see cref="JsonNode"/>; <see langword="null"/> for blank input or invalid JSON. </summary>
        public JsonNode? TryFromJson()
        {
            if ( string.IsNullOrWhiteSpace(self) ) { return null; }

            try { return JsonNode.Parse(self, NodeOptions, DocumentOptions); }
            catch ( JsonException e )
            {
                SelfLogger.WriteLine("Json parsing error: {Error}", e);
                return null;
            }
        }


        /// <summary> Deserializes with <see cref="GetTypeInfo{T}"/>; <c> default </c> for blank input, invalid JSON, or a JSON <c> null </c> root. </summary>
        public TValue? TryFromJson<TValue>() => self.TryFromJson(GetTypeInfo<TValue>());


        public TValue? TryFromJson<TValue>( JsonTypeInfo<TValue> info ) => JsonModel.TryFromJson(self, info, out TValue? result)
                                                                               ? result
                                                                               : default;
    }



    extension( string self )
    {
        /// <exception cref="JsonException"> Invalid JSON or a JSON <c> null </c> root. </exception>
        public JsonNode FromJson() => JsonNode.Parse(self, NodeOptions, DocumentOptions) ?? throw new JsonException("The JSON root is null.");

        /// <inheritdoc cref="JsonModel.FromJson{T}(string, JsonTypeInfo{T})"/>
        public TValue FromJson<TValue>() => JsonModel.FromJson(self, GetTypeInfo<TValue>());

        /// <inheritdoc cref="JsonModel.FromJson{T}(string, JsonTypeInfo{T})"/>
        public TValue FromJson<TValue>( JsonTypeInfo<TValue> info ) => JsonModel.FromJson(self, info);
    }



    extension( Stream self )
    {
        /// <summary> Reads a JSON document. The caller owns (and disposes) the stream. </summary>
        public async ValueTask<JsonNode> FromJson( CancellationToken token = default ) => await JsonNode.ParseAsync(self, NodeOptions, DocumentOptions, token).ConfigureAwait(false) ?? throw new JsonException("The JSON root is null.");

        public ValueTask<T> FromJson<T>( CancellationToken token = default ) => JsonModel.FromJsonAsync(self, GetTypeInfo<T>(), token);

        public ValueTask<T> FromJson<T>( JsonTypeInfo<T> info, CancellationToken token = default ) => JsonModel.FromJsonAsync(self, info, token);


        /// <summary> Streams the elements of a root-level JSON array, deserializing each as it arrives. </summary>
        public IAsyncEnumerable<T?> FromJsonAsync<T>( CancellationToken token = default ) => JsonSerializer.DeserializeAsyncEnumerable(self, GetTypeInfo<T>(), token);

        /// <inheritdoc cref="FromJsonAsync{T}(Stream, CancellationToken)"/>
        public IAsyncEnumerable<T?> FromJsonAsync<T>( JsonTypeInfo<T> info, CancellationToken token = default ) => JsonSerializer.DeserializeAsyncEnumerable(self, info, token);

        /// <summary> Streams the elements of a root-level JSON array as <see cref="JsonNode"/>s. </summary>
        public IAsyncEnumerable<JsonNode?> FromJsonAsync( CancellationToken token = default ) => JsonSerializer.DeserializeAsyncEnumerable(self, JakarExtensionsContext.Default.JsonNode, token);
    }



    /// <summary> A JSON <c> null </c> element (a <see langword="default"/> <see cref="JsonElement"/> is <see cref="JsonValueKind.Undefined"/> and can't be serialized). </summary>
    public static JsonElement NullElement { get; } = JsonDocument.Parse("null").RootElement.Clone();


    /// <summary> Case-insensitive property lookup, like the serializer options. </summary>
    public static JsonNodeOptions NodeOptions { get; } = new() { PropertyNameCaseInsensitive = true };

    /// <summary> Comments and trailing commas allowed, like the serializer options. </summary>
    public static JsonDocumentOptions DocumentOptions { get; } = new()
                                                                 {
                                                                     AllowTrailingCommas = true,
                                                                     CommentHandling     = JsonCommentHandling.Skip
                                                                 };



    extension( PropertyInfo self )
    {
        public bool   GetJsonIsRequired() => self.GetCustomAttribute<JsonRequiredAttribute>() is not null;
        public string GetJsonKey()        => self.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? self.Name;
    }
}
