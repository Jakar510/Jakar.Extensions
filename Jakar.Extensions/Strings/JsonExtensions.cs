namespace Jakar.Extensions.Strings;


public static class JsonExtensions
{
    public static string ToJson( this JToken json )                        => json.ToJson(Formatting.Indented);
    public static string ToJson( this JToken json, Formatting formatting ) => json.ToString(formatting);

    public static string ToJson( this string json )                                                   => json.FromJson().ToJson();
    public static string ToJson( this string json, JsonLoadSettings settings )                        => json.FromJson(settings).ToJson();
    public static string ToJson( this string json, Formatting       formatting )                      => json.FromJson().ToJson(formatting);
    public static string ToJson( this string json, JsonLoadSettings settings, Formatting formatting ) => json.FromJson(settings).ToJson(formatting);

    public static string ToPrettyJson( this object s ) => s.ToJson(Formatting.Indented);

    public static string ToJson( this object s )                                                                     => JsonConvert.SerializeObject(s);
    public static string ToJson( this object s, Formatting             formatting )                                  => JsonConvert.SerializeObject(s, formatting);
    public static string ToJson( this object s, JsonSerializerSettings settings )                                    => s.ToJson(Formatting.Indented, settings);
    public static string ToJson( this object s, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject(s, formatting, settings);
    public static string ToJson( this object s, params JsonConverter[] converters )                                    => s.ToJson(Formatting.Indented, converters);
    public static string ToJson( this object s, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject(s, formatting, converters);


    public static TResult FromJson<TResult>( this string s ) => JsonConvert.DeserializeObject<TResult>(s) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));

    public static TResult FromJson<TResult>( this string s, JsonSerializerSettings? settings ) =>
        JsonConvert.DeserializeObject<TResult>(s, settings) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));

    public static TResult FromJson<TResult>( this string s, params JsonConverter[] converters ) =>
        JsonConvert.DeserializeObject<TResult>(s, converters) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));


    public static JObject FromJson( this string s )                            => JObject.Parse(s) ?? throw new NullReferenceException(nameof(JObject.Parse));
    public static JObject FromJson( this string s, JsonLoadSettings settings ) => JObject.Parse(s, settings) ?? throw new NullReferenceException(nameof(JObject.Parse));


    public static JObject FromJson( this object s )                            => JObject.FromObject(s) ?? throw new NullReferenceException(nameof(JObject.Parse));
    public static JObject FromJson( this object s, JsonSerializer serializer ) => JObject.FromObject(s, serializer) ?? throw new NullReferenceException(nameof(JObject.Parse));
}
