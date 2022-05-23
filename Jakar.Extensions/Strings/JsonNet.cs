namespace Jakar.Extensions.Strings;


public static class JsonNet
{
    public static  JsonLoadSettings       LoadSettings { get; set; } = new();
    public static  JsonSerializer         Serializer   { get; set; } = new();
    private static JsonSerializerSettings _settings = new();
    public static JsonSerializerSettings Settings
    {
        get => _settings;
        set
        {
            _settings  = value;
            Serializer = JsonSerializer.Create(value);
        }
    }


    public static string ToJson( this JToken json, params JsonConverter[] converters ) => json.ToJson(Formatting.Indented, converters);
    public static string ToJson( this JToken json, Formatting             formatting, params JsonConverter[] converters ) => json.ToString(formatting, converters);


    public static string ToJson( this string item ) => item.FromJson().ToJson();
    public static string ToJson( this string item, JsonLoadSettings settings ) => item.FromJson(settings).ToJson();
    public static string ToJson( this string item, Formatting       formatting ) => item.FromJson().ToJson(formatting);
    public static string ToJson( this string item, JsonLoadSettings settings, Formatting formatting ) => item.FromJson(settings).ToJson(formatting);


    public static string ToPrettyJson( this object item ) => item.ToJson(Formatting.Indented);


    public static string ToJson( this object item ) => JsonConvert.SerializeObject(item);
    public static string ToJson( this object item, Formatting             formatting ) => JsonConvert.SerializeObject(item, formatting);
    public static string ToJson( this object item, JsonSerializerSettings settings ) => item.ToJson(Formatting.Indented, settings);
    public static string ToJson( this object item, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject(item, formatting, settings);
    public static string ToJson( this object item, params JsonConverter[] converters ) => item.ToJson(Formatting.Indented, converters);
    public static string ToJson( this object item, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject(item, formatting, converters);


    public static TResult FromJson<TResult>( this ReadOnlySpan<char> json ) => json.ToString().FromJson<TResult>(); // TODO: 
    public static TResult FromJson<TResult>( this string             json ) => JsonConvert.DeserializeObject<TResult>(json) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    public static TResult FromJson<TResult>( this string             json, JsonSerializerSettings? settings ) => JsonConvert.DeserializeObject<TResult>(json,   settings) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    public static TResult FromJson<TResult>( this string             json, params JsonConverter[]  converters ) => JsonConvert.DeserializeObject<TResult>(json, converters) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));


    public static JToken FromJson( this ReadOnlySpan<char> json ) => JToken.Parse(json.ToString()) ?? throw new NullReferenceException(nameof(JToken.Parse)); // TODO: 
    public static JToken FromJson( this string             json ) => JToken.Parse(json) ?? throw new NullReferenceException(nameof(JToken.Parse));
    public static JToken FromJson( this string             json, JsonLoadSettings settings ) => JToken.Parse(json, settings) ?? throw new NullReferenceException(nameof(JToken.Parse));


    public static JToken FromJson( this object json ) => JToken.FromObject(json) ?? throw new NullReferenceException(nameof(JToken.Parse));
    public static JToken FromJson( this object json, JsonSerializer serializer ) => JToken.FromObject(json, serializer) ?? throw new NullReferenceException(nameof(JToken.Parse));

    // TODO: SaveDebug
    // [Conditional("DEBUG")]
    // public static void SaveDebug<T>( this T item, [CallerMemberName] string? caller = default, [CallerArgumentExpression("item")] string? variableName = default ) where T : notnull
    // {
    //     Task.Run(async () =>
    //              {
    //                  LocalFile file = LocalDirectory.Create("DEBUG").Join($"{caller}__{variableName}.json");
    //                  await file.WriteAsync(item.ToPrettyJson());
    //              });
    // }
}
