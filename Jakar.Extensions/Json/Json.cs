// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

using Jakar.Extensions.UserGuid;
using Jakar.Extensions.UserLong;



namespace Jakar.Extensions;


public static class Json
{
    private static          JsonDocumentOptions                      __documentOptions;
    private static          JsonNodeOptions                          __jsonNodeOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly ConcurrentDictionary<Type, JsonTypeInfo> __jsonTypeInfos   = new();


    // [RequiresUnreferencedCode(Json.SerializationUnreferencedCode), RequiresDynamicCode(Json.SerializationRequiresDynamicCodeM)]
    public static ref JsonDocumentOptions DocumentOptions => ref __documentOptions;
    public static ref JsonNodeOptions     JsonNodeOptions => ref __jsonNodeOptions;


    public static JsonSerializerOptions Options { get; set; } = CreateOptions();


    public static JsonSerializerOptions CreateOptions( params ReadOnlySpan<IJsonTypeInfoResolver?> resolvers ) => new()
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
                                                                                                                      TypeInfoResolver                     = CombineResolver(resolvers),
                                                                                                                      Converters =
                                                                                                                      {
                                                                                                                          EncodingConverter.Instance,
                                                                                                                          AppVersionJsonConverter.Instance,
                                                                                                                          VersionConverter.Instance
                                                                                                                      }
                                                                                                                  };
    public static IJsonTypeInfoResolver CombineResolver( params ReadOnlySpan<IJsonTypeInfoResolver?> resolvers ) => JsonTypeInfoResolver.Combine([JakarExtensionsContext.Default, JakarModelsGuidContext.Default, JakarModelsLongContext.Default, ..resolvers]);


    public static void                 Register<TValue>( this JsonTypeInfo<TValue> info ) => __jsonTypeInfos[typeof(TValue)] = info;
    public static JsonTypeInfo<TValue> GetTypeInfo<TValue>()                              => (JsonTypeInfo<TValue>)__jsonTypeInfos[typeof(TValue)];


    public static bool GetJsonIsRequired( this PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonRequiredAttribute>() is not null;
    public static string GetJsonKey( this PropertyInfo propInfo ) => propInfo.GetCustomAttribute<JsonPropertyNameAttribute>()
                                                                            ?.Name ??
                                                                     propInfo.Name;


    public static JsonNode FromJson( this         string value )                            => Validate.ThrowIfNull(JsonNode.Parse(value));
    public static TValue   FromJson<TValue>( this string value, JsonTypeInfo<TValue> info ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(value, info));
    public static TValue FromJson<TValue>( this string value )
        where TValue : IJsonModel<TValue> => value.FromJson(TValue.JsonTypeInfo);


    public static bool Contains( this IJsonModel self, string key ) => self.AdditionalData?.ContainsKey(key) ?? false;
    public static bool Contains( this IJsonStringModel self, string key ) => self.GetAdditionalData()
                                                                                 .ContainsKey(key);


    public static bool Remove( this IJsonModel self, string key )
    {
        JsonObject dict = self.GetAdditionalData();
        return dict.Remove(key);
    }
    public static bool Remove( this IJsonModel self, string key, out JsonNode? value )
    {
        JsonObject dict = self.GetAdditionalData();
        dict.TryGetPropertyValue(key, out value);
        return dict.Remove(key);
    }
    public static bool Remove( this IJsonStringModel self, string key )
    {
        JsonObject additionalData = self.GetAdditionalData();
        bool       result         = additionalData.Remove(key);
        self.SetAdditionalData(additionalData);
        return result;
    }
    public static bool Remove( this IJsonStringModel self, string key, out JsonNode? value )
    {
        JsonObject dict = self.GetAdditionalData();
        dict.TryGetPropertyValue(key, out value);
        bool result = dict.Remove(key);
        self.SetAdditionalData(dict);
        return result;
    }


    [Pure] public static JsonObject GetAdditionalData( this IJsonStringModel model ) => model.AdditionalData?.GetAdditionalData() ?? new JsonObject();
    public static        JsonObject GetAdditionalData( this IJsonModel       model ) => model.AdditionalData ??= new JsonObject();
    public static JsonObject? GetAdditionalData( this string json )
    {
        if ( string.IsNullOrWhiteSpace(json) ) { return null; }

        using Buffer<byte> bytes   = json.AsSpanBytes(Encoding.Default);
        Utf8JsonReader     reader  = new(bytes.Values);
        JsonElement        element = JsonElement.ParseValue(ref reader);
        return JsonObject.Create(element);
    }


    public static JsonNode? Get( this IJsonModel       self, string key ) => self.AdditionalData?[key];
    public static JsonNode? Get( this IJsonStringModel self, string key ) => self.GetAdditionalData()[key];


    public static TValue? Get<TValue>( this IJsonModel self, string key )
        where TValue : IJsonModel<TValue> => self.Get(key, TValue.JsonTypeInfo);
    public static TValue? Get<TValue>( this IJsonModel self, string key, JsonTypeInfo<TValue> info )
    {
        JsonNode? token = self.Get(key);
        if ( token is null ) { return default; }

        return token.ToObject(info);
    }

    public static TValue? Get<TValue>( this IJsonStringModel self, string key )
        where TValue : IJsonModel<TValue> => self.Get(key, TValue.JsonTypeInfo);
    public static TValue? Get<TValue>( this IJsonStringModel self, string key, JsonTypeInfo<TValue> info )
    {
        JsonNode? token = self.Get(key);
        return token.ToObject(info);
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
    public static void Add( this IJsonModel self, string key, JsonNode? element ) => self.GetAdditionalData()[key] = element;


    public static void SetAdditionalData( this IJsonModel       model, JsonObject? data ) => model.AdditionalData = data;
    public static void SetAdditionalData( this IJsonStringModel model, JsonObject? data ) => model.AdditionalData = data?.ToJson();


    public static string ToJson( this string[]                value ) => value.ToJson(JakarExtensionsContext.Default.StringArray);
    public static string ToJson( this Int128[]                value ) => value.ToJson(JakarExtensionsContext.Default.Int128Array);
    public static string ToJson( this UInt128[]               value ) => value.ToJson(JakarExtensionsContext.Default.UInt128Array);
    public static string ToJson( this Int128?[]               value ) => value.ToJson(JakarExtensionsContext.Default.NullableInt128Array);
    public static string ToJson( this UInt128?[]              value ) => value.ToJson(JakarExtensionsContext.Default.NullableUInt128Array);
    public static string ToJson( this decimal[]               value ) => value.ToJson(JakarExtensionsContext.Default.DecimalArray);
    public static string ToJson( this double[]                value ) => value.ToJson(JakarExtensionsContext.Default.DoubleArray);
    public static string ToJson( this float[]                 value ) => value.ToJson(JakarExtensionsContext.Default.SingleArray);
    public static string ToJson( this long[]                  value ) => value.ToJson(JakarExtensionsContext.Default.Int64Array);
    public static string ToJson( this int[]                   value ) => value.ToJson(JakarExtensionsContext.Default.Int32Array);
    public static string ToJson( this short[]                 value ) => value.ToJson(JakarExtensionsContext.Default.Int16Array);
    public static string ToJson( this ulong[]                 value ) => value.ToJson(JakarExtensionsContext.Default.UInt64Array);
    public static string ToJson( this uint[]                  value ) => value.ToJson(JakarExtensionsContext.Default.UInt32Array);
    public static string ToJson( this ushort[]                value ) => value.ToJson(JakarExtensionsContext.Default.UInt16Array);
    public static string ToJson( this DateTime[]              value ) => value.ToJson(JakarExtensionsContext.Default.DateTimeArray);
    public static string ToJson( this DateTimeOffset[]        value ) => value.ToJson(JakarExtensionsContext.Default.DateTimeOffsetArray);
    public static string ToJson( this DateOnly[]              value ) => value.ToJson(JakarExtensionsContext.Default.DateOnlyArray);
    public static string ToJson( this TimeOnly[]              value ) => value.ToJson(JakarExtensionsContext.Default.TimeOnlyArray);
    public static string ToJson( this TimeSpan[]              value ) => value.ToJson(JakarExtensionsContext.Default.TimeSpanArray);
    public static string ToJson( this decimal?[]              value ) => value.ToJson(JakarExtensionsContext.Default.NullableDecimalArray);
    public static string ToJson( this double?[]               value ) => value.ToJson(JakarExtensionsContext.Default.NullableDoubleArray);
    public static string ToJson( this float?[]                value ) => value.ToJson(JakarExtensionsContext.Default.NullableSingleArray);
    public static string ToJson( this long?[]                 value ) => value.ToJson(JakarExtensionsContext.Default.NullableInt64Array);
    public static string ToJson( this int?[]                  value ) => value.ToJson(JakarExtensionsContext.Default.NullableInt32Array);
    public static string ToJson( this short?[]                value ) => value.ToJson(JakarExtensionsContext.Default.NullableInt16Array);
    public static string ToJson( this ulong?[]                value ) => value.ToJson(JakarExtensionsContext.Default.NullableUInt64Array);
    public static string ToJson( this uint?[]                 value ) => value.ToJson(JakarExtensionsContext.Default.NullableUInt32Array);
    public static string ToJson( this ushort?[]               value ) => value.ToJson(JakarExtensionsContext.Default.NullableUInt16Array);
    public static string ToJson( this DateTime?[]             value ) => value.ToJson(JakarExtensionsContext.Default.NullableDateTimeArray);
    public static string ToJson( this DateTimeOffset?[]       value ) => value.ToJson(JakarExtensionsContext.Default.NullableDateTimeOffsetArray);
    public static string ToJson( this DateOnly?[]             value ) => value.ToJson(JakarExtensionsContext.Default.NullableDateOnlyArray);
    public static string ToJson( this TimeOnly?[]             value ) => value.ToJson(JakarExtensionsContext.Default.NullableTimeOnlyArray);
    public static string ToJson( this TimeSpan?[]             value ) => value.ToJson(JakarExtensionsContext.Default.NullableTimeSpanArray);
    public static string ToJson( this Uri[]                   value ) => value.ToJson(JakarExtensionsContext.Default.UriArray);
    public static string ToJson( this byte[]                  value ) => value.ToJson(JakarExtensionsContext.Default.ByteArray);
    public static string ToJson( this Memory<byte>[]          value ) => value.ToJson(JakarExtensionsContext.Default.MemoryByteArray);
    public static string ToJson( this ReadOnlyMemory<byte>[]  value ) => value.ToJson(JakarExtensionsContext.Default.ReadOnlyMemoryByteArray);
    public static string ToJson( this ReadOnlyMemory<byte>    value ) => value.ToJson(JakarExtensionsContext.Default.ReadOnlyMemoryByte);
    public static string ToJson( this Memory<byte>            value ) => value.ToJson(JakarExtensionsContext.Default.MemoryByte);
    public static string ToJson( this HashSet<string>         value ) => value.ToJson(JakarExtensionsContext.Default.HashSetString);
    public static string ToJson( this HashSet<double>         value ) => value.ToJson(JakarExtensionsContext.Default.HashSetDouble);
    public static string ToJson( this HashSet<float>          value ) => value.ToJson(JakarExtensionsContext.Default.HashSetSingle);
    public static string ToJson( this HashSet<long>           value ) => value.ToJson(JakarExtensionsContext.Default.HashSetInt64);
    public static string ToJson( this HashSet<int>            value ) => value.ToJson(JakarExtensionsContext.Default.HashSetInt32);
    public static string ToJson( this HashSet<short>          value ) => value.ToJson(JakarExtensionsContext.Default.HashSetInt16);
    public static string ToJson( this HashSet<ulong>          value ) => value.ToJson(JakarExtensionsContext.Default.HashSetUInt64);
    public static string ToJson( this HashSet<uint>           value ) => value.ToJson(JakarExtensionsContext.Default.HashSetUInt32);
    public static string ToJson( this HashSet<ushort>         value ) => value.ToJson(JakarExtensionsContext.Default.HashSetUInt16);
    public static string ToJson( this HashSet<Int128>         value ) => value.ToJson(JakarExtensionsContext.Default.HashSetInt128);
    public static string ToJson( this HashSet<UInt128>        value ) => value.ToJson(JakarExtensionsContext.Default.HashSetUInt128);
    public static string ToJson( this HashSet<Guid>           value ) => value.ToJson(JakarExtensionsContext.Default.HashSetGuid);
    public static string ToJson( this HashSet<DateTimeOffset> value ) => value.ToJson(JakarExtensionsContext.Default.HashSetDateTimeOffset);
    public static string ToJson( this HashSet<DateTime>       value ) => value.ToJson(JakarExtensionsContext.Default.HashSetDateTime);
    public static string ToJson( this HashSet<DateOnly>       value ) => value.ToJson(JakarExtensionsContext.Default.HashSetDateOnly);
    public static string ToJson( this HashSet<TimeOnly>       value ) => value.ToJson(JakarExtensionsContext.Default.HashSetTimeOnly);
    public static string ToJson( this HashSet<TimeSpan>       value ) => value.ToJson(JakarExtensionsContext.Default.HashSetTimeSpan);


    public static string ToJson( this JsonNode value ) => value.ToJsonString(Options);
    public static string ToJson<TValue>( this TValue[] value )
        where TValue : IJsonModel<TValue> => value.ToJson(TValue.JsonArrayInfo);
    public static string ToJson<TValue>( this TValue value )
        where TValue : IJsonModel<TValue> => value.ToJson(TValue.JsonTypeInfo);
    public static string ToJson<TValue>( this TValue value, JsonTypeInfo<TValue> info ) => JsonSerializer.Serialize(value, info);


    [Pure] public static JsonDocumentOptions GetJsonDocumentOptions( this JsonSerializerOptions options ) => new()
                                                                                                             {
                                                                                                                 AllowTrailingCommas = options.AllowTrailingCommas,
                                                                                                                 CommentHandling     = options.ReadCommentHandling,
                                                                                                                 MaxDepth            = options.MaxDepth
                                                                                                             };
    public static       ValueTask<JsonNode?> FromJson( this Stream stream, CancellationToken     token )                            => stream.FromJson(Options, token);
    public static async ValueTask<JsonNode?> FromJson( this Stream stream, JsonSerializerOptions options, CancellationToken token ) => await JsonNode.ParseAsync(stream, JsonNodeOptions, options.GetJsonDocumentOptions(), token);


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


    public static TValue? ToObject<TValue>( this JsonNode? element, JsonTypeInfo<TValue> options ) => element is not null
                                                                                                          ? element.Deserialize(options)
                                                                                                          : default;


    public static TValue? ToObject<TValue>( this JsonElement element, JsonTypeInfo<TValue> options )
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        using ( Utf8JsonWriter writer = new(bufferWriter) ) { element.WriteTo(writer); }

        return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, options);
    }
    public static TValue? ToObject<TValue>( this JsonDocument document, JsonTypeInfo<TValue> options ) => document.RootElement.ToObject(options);


    public static JsonValue ToJsonNode<TValue>( this TValue value )
        where TValue : IJsonModel<TValue> => value.ToJsonNode(TValue.JsonTypeInfo);
    public static JsonValue ToJsonNode<TValue>( this TValue value, JsonTypeInfo<TValue> options ) => Validate.ThrowIfNull(JsonValue.Create(value, options));


    public static JsonElement ToJsonElement<TValue>( this TValue value )
        where TValue : IJsonModel<TValue> => value.ToJsonElement(TValue.JsonTypeInfo);
    public static JsonElement ToJsonElement<TValue>( this TValue value, JsonTypeInfo<TValue> options )
    {
        string             json   = JsonSerializer.Serialize(value, options);
        using Buffer<byte> buffer = json.AsSpanBytes(Encoding.Default);
        Utf8JsonReader     reader = new(buffer.Values);
        return JsonElement.ParseValue(ref reader);
    }


    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this string? value, JsonNodeOptions? options = null ) => !string.IsNullOrEmpty(value)
                                                                                                                                                ? JsonValue.Create(value, options)
                                                                                                                                                : null;


    public static JsonValue ToJsonNode( this bool value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this bool? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                              ? JsonValue.Create(value.Value, options)
                                                                                                                                              : null;


    public static JsonValue ToJsonNode( this byte value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this byte? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                              ? JsonValue.Create(value.Value, options)
                                                                                                                                              : null;


    public static JsonValue ToJsonNode( this char value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this char? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                              ? JsonValue.Create(value.Value, options)
                                                                                                                                              : null;


    public static JsonValue ToJsonNode( this ref readonly DateTime value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ref readonly DateTime? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                               ? JsonValue.Create(value.Value, options)
                                                                                                                                                               : null;


    public static JsonValue ToJsonNode( this ref readonly DateTimeOffset value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ref readonly DateTimeOffset? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                                     ? JsonValue.Create(value.Value, options)
                                                                                                                                                                     : null;


    public static JsonValue ToJsonNode( this ref readonly DateOnly value, JsonNodeOptions? options = null ) => Validate.ThrowIfNull(JsonValue.Create(value, JakarExtensionsContext.Default.DateOnly, options));
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ref readonly DateOnly? value, JsonNodeOptions? options = null )
    {
        if ( value is null ) { return null; }

        DateOnly x = value.Value;
        return x.ToJsonNode(options);
    }


    public static JsonValue ToJsonNode( this ref readonly TimeOnly value, JsonNodeOptions? options = null ) => Validate.ThrowIfNull(JsonValue.Create(value, JakarExtensionsContext.Default.TimeOnly, options));
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ref readonly TimeOnly? value, JsonNodeOptions? options = null )
    {
        if ( value is null ) { return null; }

        TimeOnly x = value.Value;
        return x.ToJsonNode(options);
    }
    public static JsonValue ToJsonNode( this decimal value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this decimal? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                 ? JsonValue.Create(value.Value, options)
                                                                                                                                                 : null;


    public static JsonValue ToJsonNode( this double value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this double? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                ? JsonValue.Create(value.Value, options)
                                                                                                                                                : null;


    public static JsonValue ToJsonNode( this float value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this float? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                               ? JsonValue.Create(value.Value, options)
                                                                                                                                               : null;


    public static JsonValue ToJsonNode( this ref readonly Guid value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ref readonly Guid? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                           ? JsonValue.Create(value.Value, options)
                                                                                                                                                           : null;


    public static JsonValue ToJsonNode( this short value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this short? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                               ? JsonValue.Create(value.Value, options)
                                                                                                                                               : null;


    public static JsonValue ToJsonNode( this int value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this int? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                             ? JsonValue.Create(value.Value, options)
                                                                                                                                             : null;


    public static JsonValue ToJsonNode( this long value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this long? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                              ? JsonValue.Create(value.Value, options)
                                                                                                                                              : null;


    public static JsonValue ToJsonNode( this sbyte value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this sbyte? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                               ? JsonValue.Create(value.Value, options)
                                                                                                                                               : null;


    public static JsonValue ToJsonNode( this ushort value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ushort? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                ? JsonValue.Create(value.Value, options)
                                                                                                                                                : null;


    public static JsonValue ToJsonNode( this uint value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this uint? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                              ? JsonValue.Create(value.Value, options)
                                                                                                                                              : null;


    public static JsonValue ToJsonNode( this ulong value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ulong? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                               ? JsonValue.Create(value.Value, options)
                                                                                                                                               : null;


    public static JsonValue? ToJsonNode( this ref readonly JsonElement value, JsonNodeOptions? options = null ) => JsonValue.Create(value, options);
    [return: NotNullIfNotNull(nameof(value))] public static JsonValue? ToJsonNode( this ref readonly JsonElement? value, JsonNodeOptions? options = null ) => value.HasValue
                                                                                                                                                                  ? JsonValue.Create(value.Value, options)
                                                                                                                                                                  : null;
}



public interface IJsonModel
{
    [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
}



public interface IJsonStringModel
{
    public string? AdditionalData { get; set; }
}
