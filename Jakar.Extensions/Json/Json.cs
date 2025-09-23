// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

using System.IO.Pipelines;
using Jakar.Extensions.UserGuid;
using Jakar.Extensions.UserLong;



namespace Jakar.Extensions;


public static class Json
{
    public const            string                                   SerializationRequiresDynamicCodeM = "SerializationRequiresDynamicCodeM";
    public const            string                                   SerializationUnreferencedCode     = "SerializationUnreferencedCode";
    private static          JsonDocumentOptions                      __documentOptions;
    private static readonly ConcurrentDictionary<Type, JsonTypeInfo> __jsonTypeInfos             = new();
    public static readonly  DefaultJsonTypeInfoResolver              DefaultJsonTypeInfoResolver = new();
    private static          JsonNodeOptions                          __jsonNodeOptions           = new() { PropertyNameCaseInsensitive = true };


    // [RequiresUnreferencedCode(Json.SerializationUnreferencedCode), RequiresDynamicCode(Json.SerializationRequiresDynamicCodeM)]
    public static ref JsonDocumentOptions DocumentOptions => ref __documentOptions;
    public static ref JsonNodeOptions     JsonNodeOptions => ref __jsonNodeOptions;

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
                                                                    TypeInfoResolver                     = JsonTypeInfoResolver.Combine(DefaultJsonTypeInfoResolver, JakarExtensionsContext.Default, JakarModelsGuidContext.Default, JakarModelsLongContext.Default),
                                                                    Converters =
                                                                    {
                                                                        AppVersionJsonConverter.Instance,
                                                                        VersionConverter.Instance
                                                                    }
                                                                };


    public static void                 Register<TValue>( this JsonTypeInfo<TValue> info ) => __jsonTypeInfos[typeof(TValue)] = info;
    public static JsonTypeInfo<TValue> GetTypeInfo<TValue>()                              => (JsonTypeInfo<TValue>)__jsonTypeInfos[typeof(TValue)];


    public static bool   GetJsonIsRequired( this PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonRequiredAttribute>() is not null;
    public static string GetJsonKey( this        PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? propInfo.Name;


    public static JsonNode FromJson( this         string value )                            => Validate.ThrowIfNull(JsonNode.Parse(value));
    public static TValue   FromJson<TValue>( this string value, JsonTypeInfo<TValue> info ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(value, info));
    public static TValue FromJson<TValue>( this string value )
        where TValue : IJsonModel<TValue> => value.FromJson<TValue>(TValue.JsonTypeInfo);


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


    public static        JsonObject GetData( this           IJsonModel       model ) => model.GetAdditionalData();
    [Pure] public static JsonObject GetData( this           IJsonStringModel model ) => model.GetAdditionalData() ?? new JsonObject();
    public static        JsonObject GetAdditionalData( this IJsonModel       model ) => model.AdditionalData ??= new JsonObject();
    public static JsonObject? GetAdditionalData( this IJsonStringModel model )
    {
        if ( string.IsNullOrWhiteSpace(model.AdditionalData) ) { return null; }

        using Buffer<byte> bytes   = model.AdditionalData.AsSpanBytes(Encoding.Default);
        Utf8JsonReader     reader  = new(bytes.Values);
        JsonElement        element = JsonElement.ParseValue(ref reader);
        return JsonObject.Create(element);
    }


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
        JsonNode? element = JsonSerializer.SerializeToNode(value, JakarExtensionsContext.Default.TimeSpan);
        self.Add(key, element);
    }
    public static void Add<TValue>( this IJsonModel self, string key, TValue value )
        where TValue : IJsonModel<TValue>
    {
        JsonNode? element = JsonSerializer.SerializeToNode(value, TValue.JsonTypeInfo);
        self.Add(key, element);
    }
    public static void Add( this IJsonModel self, string key, JsonNode? element ) => self.GetData()[key] = element;


    public static void SetAdditionalData( this IJsonModel       model, JsonObject? data ) => model.AdditionalData = data;
    public static void SetAdditionalData( this IJsonStringModel model, JsonObject? data ) => model.AdditionalData = data?.ToJson();


    public static string ToJson( this JsonNode value ) => value.ToJsonString(Options);
    public static string ToJson<TValue>( this TValue value )
        where TValue : IJsonModel<TValue> => value.ToJson(TValue.JsonTypeInfo);
    public static string ToJson<TValue>( this TValue value, JsonTypeInfo<TValue> info ) => JsonSerializer.Serialize(value, info);



    public static JsonDocumentOptions GetJsonDocumentOptions( this JsonSerializerOptions options ) => new()
                                                                                                      {
                                                                                                          AllowTrailingCommas = options.AllowTrailingCommas,
                                                                                                          CommentHandling     = options.ReadCommentHandling,
                                                                                                          MaxDepth            = options.MaxDepth
                                                                                                      };
    public static       ValueTask<JsonNode?> FromJson( this Stream stream, CancellationToken     token )                            => stream.FromJson(Options, token);
    public static async ValueTask<JsonNode?> FromJson( this Stream stream, JsonSerializerOptions options, CancellationToken token ) { return await JsonNode.ParseAsync(stream, JsonNodeOptions, options.GetJsonDocumentOptions(), token); }


    public static ValueTask<TValue?> FromJson<TValue>( this Stream stream, CancellationToken token )
        where TValue : IJsonModel<TValue> => stream.FromJson(TValue.JsonTypeInfo, token);
    public static async ValueTask<TValue?> FromJson<TValue>( this Stream stream, JsonTypeInfo<TValue> info, CancellationToken token )
    {
        using JsonDocument jsonReader = await JsonDocument.ParseAsync(stream, DocumentOptions, token);
        return await JsonSerializer.DeserializeAsync(stream, info, token);
    }


    public static async IAsyncEnumerable<TValue> FromJsonValues<TValue>( this Stream stream, [EnumeratorCancellation] CancellationToken token )
        where TValue : IJsonModel<TValue>
    {
        using JsonDocument jsonReader = await JsonDocument.ParseAsync(stream, DocumentOptions, token);
        await foreach ( TValue? value in JsonSerializer.DeserializeAsyncEnumerable(stream, TValue.JsonTypeInfo, token) ) { yield return Validate.ThrowIfNull(value); }
    }
    public static async IAsyncEnumerable<TValue> FromJsonValues<TValue>( this Stream stream, JsonTypeInfo<TValue> info, [EnumeratorCancellation] CancellationToken token )
    {
        using JsonDocument jsonReader = await JsonDocument.ParseAsync(stream, DocumentOptions, token);
        await foreach ( TValue? value in JsonSerializer.DeserializeAsyncEnumerable(stream, info, token) ) { yield return Validate.ThrowIfNull(value); }
    }


    public static string ToJson<TValue>( this scoped in ReadOnlySpan<TValue> values )
        where TValue : IJsonModel<TValue>
    {
        TValue[] array = ArrayPool<TValue>.Shared.Rent(values.Length);

        try
        {
            values.CopyTo(array);
            return array.ToJson(TValue.JsonArrayInfo);
        }
        finally { ArrayPool<TValue>.Shared.Return(array); }
    }
}



public interface IJsonModel
{
    [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
}



public interface IJsonStringModel
{
    public string? AdditionalData { get; set; }
}
