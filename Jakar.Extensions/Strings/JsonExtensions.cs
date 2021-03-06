namespace Jakar.Extensions.Strings;


public static class JsonExtensions
{
    public static string ToJson( this JToken json, params JsonConverter[] converters )                                    => json.ToJson(Formatting.Indented, converters);
    public static string ToJson( this JToken json, Formatting             formatting, params JsonConverter[] converters ) => json.ToString(formatting, converters);


    public static string ToJson( this string item )                                                   => item.FromJson().ToJson();
    public static string ToJson( this string item, JsonLoadSettings settings )                        => item.FromJson(settings).ToJson();
    public static string ToJson( this string item, Formatting       formatting )                      => item.FromJson().ToJson(formatting);
    public static string ToJson( this string item, JsonLoadSettings settings, Formatting formatting ) => item.FromJson(settings).ToJson(formatting);


    public static string ToPrettyJson( this object item ) => item.ToJson(Formatting.Indented);


    public static string ToJson( this object item )                                                                     => JsonConvert.SerializeObject(item);
    public static string ToJson( this object item, Formatting             formatting )                                  => JsonConvert.SerializeObject(item, formatting);
    public static string ToJson( this object item, JsonSerializerSettings settings )                                    => item.ToJson(Formatting.Indented, settings);
    public static string ToJson( this object item, Formatting             formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject(item, formatting, settings);
    public static string ToJson( this object item, params JsonConverter[] converters )                                    => item.ToJson(Formatting.Indented, converters);
    public static string ToJson( this object item, Formatting             formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject(item, formatting, converters);


    public static TResult FromJson<TResult>( this string json )                                     => JsonConvert.DeserializeObject<TResult>(json) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    public static TResult FromJson<TResult>( this string json, JsonSerializerSettings? settings )   => JsonConvert.DeserializeObject<TResult>(json, settings) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
    public static TResult FromJson<TResult>( this string json, params JsonConverter[]  converters ) => JsonConvert.DeserializeObject<TResult>(json, converters) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));


    public static JObject FromJson( this string json )                            => JObject.Parse(json) ?? throw new NullReferenceException(nameof(JObject.Parse));
    public static JObject FromJson( this string json, JsonLoadSettings settings ) => JObject.Parse(json, settings) ?? throw new NullReferenceException(nameof(JObject.Parse));


    public static JObject FromJson( this object json )                            => JObject.FromObject(json) ?? throw new NullReferenceException(nameof(JObject.Parse));
    public static JObject FromJson( this object json, JsonSerializer serializer ) => JObject.FromObject(json, serializer) ?? throw new NullReferenceException(nameof(JObject.Parse));
}
