namespace Jakar.Extensions;


public static class JsonNet
{
    private static readonly JsonConverter[]         __converters = [];
    private static          JsonSerializer?         __serializer;
    private static          JsonSerializerSettings? __settings;


    public static JsonLoadSettings LoadSettings { get;                                                                                                                                                      set; } = new();
    public static JsonSerializer   Serializer   { [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] get => __serializer ??= JsonSerializer.Create(Settings); set => __serializer = value; }
    public static JsonSerializerSettings Settings
    {
        get => __settings ??= new JsonSerializerSettings();
        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
        set
        {
            __settings = value;
            Serializer = JsonSerializer.Create(value);
        }
    }
    static JsonNet() => Settings = new JsonSerializerSettings();


    public static                                                                                                  JToken FromJson( this ReadOnlySpan<char> value )                            => Validate.ThrowIfNull(FromJson(value.ToString())); // TODO: optimize?
    public static                                                                                                  JToken FromJson( this string             value )                            => Validate.ThrowIfNull(FromJson(value, LoadSettings));
    public static                                                                                                  JToken FromJson( this string             value, JsonLoadSettings settings ) => Validate.ThrowIfNull(JToken.Parse(value, settings));
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static JToken FromJson( this object             value )                            => Validate.ThrowIfNull(JToken.FromObject(value, Serializer));
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static JToken FromJson( this object             value, JsonSerializer serializer ) => Validate.ThrowIfNull(JToken.FromObject(value, serializer));


    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this JToken value )                                                                       => ToJson(value, Formatting.None, __converters);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this JToken value, params JsonConverter[] converters )                                    => ToJson(value, Formatting.None, converters);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this JToken value, Formatting             formatting )                                    => ToJson(value, formatting,      __converters);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this JToken value, Formatting             formatting, params JsonConverter[] converters ) => value.ToString(formatting, converters);


    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this object value )                                                                     => ToJson(value, Formatting.None, Settings);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this object value, Formatting             formatting )                                  => ToJson(value, formatting,      Settings);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this object value, JsonSerializerSettings settings )                                    => ToJson(value, Formatting.None, settings);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this object value, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject(value, formatting, settings);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this object value, params JsonConverter[] converters )                                    => value.ToJson(Formatting.Indented, converters);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToJson( this object value, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject(value, formatting, converters);


    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToPrettyJson( this JToken value ) => value.ToJson(Formatting.Indented);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static string ToPrettyJson( this object value ) => value.ToJson(Formatting.Indented);


    public static async ValueTask<JToken> FromJson( this Stream jsonStream, Encoding encoding, CancellationToken token )
    {
        using StreamReader reader = new(jsonStream, encoding);
        return await reader.FromJson(token);
    }
    public static async ValueTask<JToken> FromJson( this StreamReader reader, CancellationToken token )
    {
        // ReSharper disable once ConvertToUsingDeclaration
        await using ( JsonTextReader jsonReader = new(reader)
                                                  {
                                                      ArrayPool = JsonArrayPool<char>.Shared,
                                                      Culture   = CultureInfo.CurrentCulture
                                                  } ) { return await JToken.ReadFromAsync(jsonReader, LoadSettings, token); }
    }
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
    public static TResult FromJson<TResult>( this Stream jsonStream, Encoding encoding )
    {
        using StreamReader reader = new(jsonStream, encoding);
        return reader.FromJson<TResult>();
    }
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static TResult FromJson<TResult>( this StreamReader reader ) => Serializer.FromJson<TResult>(reader);
    public static TResult FromJson<TResult>( this JsonSerializer serializer, StreamReader reader )
    {
        // ReSharper disable once ConvertToUsingDeclaration
        using ( JsonTextReader jsonReader = new(reader)
                                            {
                                                ArrayPool = JsonArrayPool<char>.Shared,
                                                Culture   = CultureInfo.CurrentCulture
                                            } )
        {
            TResult? result = serializer.Deserialize<TResult>(jsonReader);
            return Validate.ThrowIfNull(result);
        }
    }
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static TResult FromJson<TResult>( this ReadOnlySpan<char> value )                                     => FromJson<TResult>(value.ToString());
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static TResult FromJson<TResult>( this string             value )                                     => FromJson<TResult>(value, Settings);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static TResult FromJson<TResult>( this string             value, JsonSerializerSettings? settings )   => Validate.ThrowIfNull(JsonConvert.DeserializeObject<TResult>(value, settings));
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public static TResult FromJson<TResult>( this string             value, params JsonConverter[]  converters ) => Validate.ThrowIfNull(JsonConvert.DeserializeObject<TResult>(value, converters));

    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
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
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
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


    [Conditional("DEBUG")]
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
    public static void SaveDebug<TValue>( this TValue value, [CallerMemberName] string? caller = null, [CallerArgumentExpression("value")] string? variableName = null )
        where TValue : notnull =>
        Task.Run(async () =>
                 {
                     using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
                     LocalFile           file          = LocalDirectory.Create("DEBUG").Join($"{caller}__{variableName}.value");
                     await file.WriteAsync(value.ToPrettyJson());
                 });



    public sealed class JsonArrayPool<T>() : IArrayPool<T>
    {
        public static readonly JsonArrayPool<T> Shared  = new();
        private readonly       ArrayPool<T>     __inner = Validate.ThrowIfNull(ArrayPool<T>.Shared);
        public                 T[]              Rent( int    minimumLength ) => __inner.Rent(minimumLength);
        public                 void             Return( T[]? array )         => __inner.Return(Validate.ThrowIfNull(array));
    }
}
