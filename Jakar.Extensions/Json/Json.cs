// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

namespace Jakar.Extensions;


public static class Json
{
    private static JsonDocumentOptions __documentOptions;

    private static readonly ConcurrentDictionary<Type, JsonTypeInfo> __jsonTypeInfos = new();
    public static ref       JsonDocumentOptions                      DocumentOptions => ref __documentOptions;
    public static readonly  DefaultJsonTypeInfoResolver              DefaultJsonTypeInfoResolver = new();


    public static JsonSerializerOptions Options { get; set; } = new()
                                                                {
                                                                    MaxDepth                             = 128,
                                                                    IndentSize                           = 4,
                                                                    NewLine                              = "\n",
                                                                    IndentCharacter                      = ' ',
                                                                    WriteIndented                        = true,
                                                                    RespectNullableAnnotations           = true,
                                                                    AllowTrailingCommas                  = true,
                                                                    AllowOutOfOrderMetadataProperties    = true,
                                                                    IgnoreReadOnlyProperties             = true,
                                                                    IncludeFields                        = true,
                                                                    IgnoreReadOnlyFields                 = false,
                                                                    PropertyNameCaseInsensitive          = false,
                                                                    ReadCommentHandling                  = JsonCommentHandling.Skip,
                                                                    UnknownTypeHandling                  = JsonUnknownTypeHandling.JsonNode,
                                                                    RespectRequiredConstructorParameters = true,
                                                                    TypeInfoResolver                     = JsonTypeInfoResolver.Combine(DefaultJsonTypeInfoResolver, JakarExtensionsContext.Default, UserGuid.JakarModelsGuidContext.Default, UserLong.JakarModelsLongContext.Default),
                                                                    Converters =
                                                                    {
                                                                        AppVersionJsonConverter.Instance,
                                                                        VersionConverter.Instance
                                                                    }
                                                                };


    public static void                 Register<TValue>( this JsonTypeInfo<TValue> info ) => __jsonTypeInfos[typeof(TValue)] = info;
    public static JsonTypeInfo<TValue> GetTypeInfo<TValue>()                              => (JsonTypeInfo<TValue>)__jsonTypeInfos.GetOrAdd(typeof(TValue), static _ => JsonTypeInfo.CreateJsonTypeInfo<TValue>(Options));


    public static bool   GetJsonIsRequired( this PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonRequiredAttribute>() is not null;
    public static string GetJsonKey( this        PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? propInfo.Name;


    public static JsonNode FromJson( this         string value )                                => Validate.ThrowIfNull(JsonSerializer.SerializeToNode(value, Options));
    public static JsonNode FromJson( this         string value, JsonSerializerOptions options ) => Validate.ThrowIfNull(JsonSerializer.SerializeToNode(value, options));
    public static TValue   FromJson<TValue>( this string value, JsonSerializerOptions options ) => Validate.ThrowIfNull(JsonSerializer.Deserialize<TValue>(value, options));
    public static TValue   FromJson<TValue>( this string value, JsonSerializerContext options ) => Validate.ThrowIfNull(JsonSerializer.Deserialize<TValue>(value, options.Options));
    public static TValue   FromJson<TValue>( this string value ) => value.FromJson<TValue>(Options);


    public static bool Contains( this IJsonModel       self, string key ) => self.AdditionalData?.ContainsKey(key)      ?? false;
    public static bool Contains( this IJsonStringModel self, string key ) => self.GetAdditionalData()?.ContainsKey(key) ?? false;


    public static bool Remove( this IJsonModel self, string key )
    {
        JsonObject dict = self.GetData();
        return dict.Remove(key);
    }
    public static bool Remove( this IJsonModel self, string key, out JsonNode? value )
    {
        JsonObject dict = self.GetData();
        dict.TryGetPropertyValue(key, out value);
        return dict.Remove(key);
    }
    public static bool Remove( this IJsonStringModel self, string key )
    {
        JsonObject additionalData = self.GetData();
        bool       result         = additionalData.Remove(key);
        self.SetAdditionalData(additionalData);
        return result;
    }
    public static bool Remove( this IJsonStringModel self, string key, out JsonNode? value )
    {
        JsonObject dict = self.GetData();
        dict.TryGetPropertyValue(key, out value);
        bool result = dict.Remove(key);
        self.SetAdditionalData(dict);
        return result;
    }


    public static JsonObject GetData( this           IJsonModel       model ) => model.GetAdditionalData();
    public static JsonObject GetData( this           IJsonStringModel model ) => model.GetAdditionalData() ?? new JsonObject();
    public static JsonObject GetAdditionalData( this IJsonModel       model ) => model.AdditionalData ??= new JsonObject();
    public static JsonObject? GetAdditionalData( this IJsonStringModel model ) => string.IsNullOrWhiteSpace(model.AdditionalData)
                                                                                      ? null
                                                                                      : model.AdditionalData.FromJson<JsonObject>();


    public static JsonNode? Get( this IJsonModel       self, string key ) => self.AdditionalData?[key];
    public static JsonNode? Get( this IJsonStringModel self, string key ) => self.GetAdditionalData()?[key];


    public static TValue? Get<TValue>( this IJsonModel self, string key )
    {
        JsonNode? token = self.Get(key);
        if ( token is null ) { return default; }

        return token.ToObject<TValue>();
    }

    public static TValue? Get<TValue>( this IJsonStringModel self, string key )
    {
        JsonNode? token = self.Get(key);
        return token.ToObject<TValue>();
    }


    public static TValue? ToObject<TValue>( this JsonNode? element, JsonSerializerOptions? options = null ) => element is not null
                                                                                                                   ? element.Deserialize<TValue>(options)
                                                                                                                   : default;
    public static TValue? ToObject<TValue>( this JsonNode? element, JsonTypeInfo<TValue> options ) => element is not null
                                                                                                          ? element.Deserialize(options)
                                                                                                          : default;


    public static TValue? ToObject<TValue>( this in JsonElement? self, JsonSerializerOptions? options = null ) => self.HasValue
                                                                                                                      ? self.Value.ToObject<TValue>(options)
                                                                                                                      : default;
    public static TValue? ToObject<TValue>( this JsonElement element, JsonSerializerOptions? options = null )
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        using ( Utf8JsonWriter writer = new(bufferWriter) ) { element.WriteTo(writer); }

        return JsonSerializer.Deserialize<TValue>(bufferWriter.WrittenSpan, options);
    }
    public static TValue? ToObject<TValue>( this JsonElement element, JsonTypeInfo<TValue> options )
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        using ( Utf8JsonWriter writer = new(bufferWriter) ) { element.WriteTo(writer); }

        return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, options);
    }
    public static TValue? ToObject<TValue>( this JsonDocument document, JsonTypeInfo<TValue>   options )        => document.RootElement.ToObject(options);
    public static TValue? ToObject<TValue>( this JsonDocument document, JsonSerializerOptions? options = null ) => document.RootElement.ToObject<TValue>(options);


    public static JsonNode? ToJsonNode<TValue>( this TValue value, JsonNodeOptions? nodeOptions = null )
    {
        string             json   = JsonSerializer.Serialize(value, Options);
        using Buffer<byte> buffer = json.AsSpanBytes(Encoding.Default);
        Utf8JsonReader     reader = new(buffer.Values);
        return JsonNode.Parse(ref reader, nodeOptions);
    }


    public static JsonElement ToJsonElement<TValue>( this TValue value, JsonTypeInfo<TValue> options )
    {
        string             json   = JsonSerializer.Serialize(value, options);
        using Buffer<byte> buffer = json.AsSpanBytes(Encoding.Default);
        Utf8JsonReader     reader = new(buffer.Values);
        return JsonElement.ParseValue(ref reader);
    }
    public static JsonElement ToJsonElement<TValue>( this TValue value, JsonSerializerOptions? options = null )
    {
        string             json   = JsonSerializer.Serialize(value, options);
        using Buffer<byte> buffer = json.AsSpanBytes(Encoding.Default);
        Utf8JsonReader     reader = new(buffer.Values);
        return JsonElement.ParseValue(ref reader);
    }


    public static void Add( this IJsonModel self, string key, bool value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, byte value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, short value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, int value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, long value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, float value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, double value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, decimal value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, string value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, DateTime value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, DateTimeOffset value )
    {
        JsonNode element = value;
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, TimeSpan value )
    {
        JsonNode? element = JsonSerializer.SerializeToNode(value);
        self.Add(key, element);
    }
    public static void Add<T>( this IJsonModel self, string key, T value )
    {
        JsonNode? element = JsonSerializer.SerializeToNode(value);
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, JsonNode? element ) => self.GetData()[key] = element;


    public static void SetAdditionalData( this IJsonModel       model, JsonObject? data ) => model.AdditionalData = data;
    public static void SetAdditionalData( this IJsonStringModel model, JsonObject? data ) => model.AdditionalData = data?.ToPrettyJson();


    public static string ToJson( this               JsonNode value ) => value.ToJsonString(Options);
    public static string ToPrettyJson( this         JsonNode value ) => value.ToJsonString(Options.WithIndented(true));
    public static string ToJson<TValue>( this       TValue   value ) => JsonSerializer.Serialize(value, Options);
    public static string ToPrettyJson<TValue>( this TValue   value ) => JsonSerializer.Serialize(value, Options.WithIndented(true));


    public static JsonSerializerOptions WithConverters( this JsonSerializerOptions options, params ReadOnlySpan<JsonConverter> converters )
    {
        if ( !converters.IsEmpty ) { options.Converters.Add(converters); }

        return options;
    }
    public static JsonSerializerOptions WithIndented( this JsonSerializerOptions options, bool isIndented )
    {
        options.WriteIndented = isIndented;
        return options;
    }


    public static ValueTask<JsonNode?> FromJson( this Stream stream, CancellationToken token ) => stream.FromJson(Options, token);
    public static async ValueTask<JsonNode?> FromJson( this Stream stream, JsonSerializerOptions options, CancellationToken token )
    {
        using JsonDocument jsonReader = await JsonDocument.ParseAsync(stream, DocumentOptions, token);
        return await JsonSerializer.DeserializeAsync<JsonNode>(stream, options, token);
    }
    public static ValueTask<TValue?> FromJson<TValue>( this Stream stream, CancellationToken token ) => stream.FromJson<TValue>(Options, token);
    public static async ValueTask<TValue?> FromJson<TValue>( this Stream stream, JsonSerializerOptions options, CancellationToken token )
    {
        using JsonDocument jsonReader = await JsonDocument.ParseAsync(stream, DocumentOptions, token);
        return await JsonSerializer.DeserializeAsync<TValue>(stream, options, token);
    }
    public static async IAsyncEnumerable<TValue?> FromJsonValues<TValue>( this Stream stream, [EnumeratorCancellation] CancellationToken token )
    {
        await foreach ( TValue? value in FromJsonValues<TValue>(stream, Options, token) ) { yield return value; }
    }
    public static async IAsyncEnumerable<TValue?> FromJsonValues<TValue>( this Stream stream, JsonSerializerOptions options, [EnumeratorCancellation] CancellationToken token )
    {
        using JsonDocument jsonReader = await JsonDocument.ParseAsync(stream, DocumentOptions, token);
        await foreach ( TValue? value in JsonSerializer.DeserializeAsyncEnumerable<TValue>(stream, options, token) ) { yield return value; }
    }


    public static string ToJson<TValue>( this ReadOnlySpan<TValue> values )
    {
        TValue[] array = ArrayPool<TValue>.Shared.Rent(values.Length);

        try
        {
            values.CopyTo(array);
            return array.ToJson();
        }
        finally { ArrayPool<TValue>.Shared.Return(array); }
    }

    public static string ToPrettyJson<TValue>( this ReadOnlySpan<TValue> values )
    {
        TValue[] array = ArrayPool<TValue>.Shared.Rent(values.Length);

        try
        {
            values.CopyTo(array);
            return array.ToPrettyJson();
        }
        finally { ArrayPool<TValue>.Shared.Return(array); }
    }



    public interface IJsonModel
    {
        [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
    }



    public interface IJsonStringModel
    {
        public string? AdditionalData { get; set; }
    }
}
