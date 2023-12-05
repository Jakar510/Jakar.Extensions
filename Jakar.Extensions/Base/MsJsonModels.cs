using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;



namespace Jakar.Extensions;


public static class MsJsonModels
{
    public static bool Contains<TClass>( this IJsonModel<TClass> self, string key )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass> => self.AdditionalData?.ContainsKey( key ) is true;


    public static bool Remove<TClass>( this IJsonModel<TClass> self, string key )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        self.AdditionalData ??= new Dictionary<string, JsonElement>();
        return self.AdditionalData.Remove( key );
    }
    public static bool Remove<TClass>( this IJsonModel<TClass> self, string key, out JsonElement value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        self.AdditionalData ??= new Dictionary<string, JsonElement>();
        self.AdditionalData.TryGetValue( key, out value );
        return self.AdditionalData.Remove( key );
    }


    public static IDictionary<string, JsonElement> GetData<TClass>( this IJsonModel<TClass> model )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass> => model.GetAdditionalData();


    public static IDictionary<string, JsonElement> GetAdditionalData<TClass>( this IJsonModel<TClass> model )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass> => model.AdditionalData ??= new Dictionary<string, JsonElement>();


    public static JsonElement? Get<TClass>( this IJsonModel<TClass> self, string key )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass> => self.AdditionalData?[key];


    public static void Add<TClass>( this IJsonModel<TClass> self, string key, bool value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, byte value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, short value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, int value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, long value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, float value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, double value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, decimal value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, string value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, DateTime value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, DateTimeOffset value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = value;
        self.Add( key, node );
    }
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, IEnumerable<KeyValuePair<string, JsonNode?>> properties )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonNode node = new JsonObject( properties );
        self.Add( key, node );
    }
    public static void Add<T, TClass>( this IJsonModel<TClass> self, string key, T value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
        where T : IJsonizer => self.Add( key, value.GetProperties() );
    public static void Add<TClass>( this IJsonModel<TClass> self, string key, JsonNode? value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        JsonElement element = value?.Deserialize<JsonElement>() ?? default;
        self.Add( key, element );
    }

    public static void Add<TClass>( this IJsonModel<TClass> self, string key, JsonElement value )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass>
    {
        self.AdditionalData ??= new Dictionary<string, JsonElement>();
        self.AdditionalData.Add( key, value );
    }


    public static void SetAdditionalData<TClass>( this IJsonModel<TClass> model, IDictionary<string, JsonElement>? data )
        where TClass : IJsonModel<TClass>, IJsonizer<TClass> => model.AdditionalData = data;


    public static string ToJson<TClass, TSerializerContext>( this TClass model )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext =>
        JsonSerializer.Serialize( model, model.JsonContext() );
    public static string ToPrettyJson<TClass, TSerializerContext>( this TClass model )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext =>
        JsonSerializer.Serialize( model, model.JsonContextPretty() );


    public static async Task<string> ToJsonAsync<TClass, TSerializerContext>( this TClass model )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync( stream, model, model.JsonContext() );
        using var reader = new StreamReader( stream );
        return await reader.ReadToEndAsync();
    }
    public static async Task<string> ToPrettyJsonAsync<TClass, TSerializerContext>( this TClass model )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync( stream, model, model.JsonContextPretty() );
        using var reader = new StreamReader( stream );
        return await reader.ReadToEndAsync();
    }


    public static TClass FromJson<TClass, TSerializerContext>( this string json )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext => JsonSerializer.Deserialize( json, TClass.JsonTypeInfo ) ?? throw new InvalidOperationException( nameof(json) );


    public static JsonSerializerOptions JsonContext<TClass, TSerializerContext>()
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext => new()
                                                            {
                                                                WriteIndented    = false,
                                                                TypeInfoResolver = TClass.JsonSerializerContext
                                                            };
    public static JsonSerializerOptions JsonContext<TClass, TSerializerContext>( this IJsonizer<TClass, TSerializerContext> _ )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext => JsonContext<TClass, TSerializerContext>();


    public static JsonSerializerOptions JsonContextPretty<TClass, TSerializerContext>()
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext => new()
                                                            {
                                                                WriteIndented    = true,
                                                                TypeInfoResolver = TClass.JsonSerializerContext
                                                            };
    public static JsonSerializerOptions JsonContextPretty<TClass, TSerializerContext>( this IJsonizer<TClass, TSerializerContext> _ )
        where TClass : IJsonizer<TClass, TSerializerContext>
        where TSerializerContext : JsonSerializerContext => JsonContextPretty<TClass, TSerializerContext>();



    public interface IJsonizer
    {
        public IEnumerable<KeyValuePair<string, JsonNode?>> GetProperties();
    }



    public interface IJsonizer<out TClass>
    {
        public abstract static TClass FromJson( string json );
    }



    public interface IJsonizer<TClass, out TSerializerContext> : IJsonizer<TClass>
        where TSerializerContext : JsonSerializerContext
    {
        public abstract static JsonTypeInfo<TClass> JsonTypeInfo          { get; }
        public abstract static TSerializerContext   JsonSerializerContext { get; }
    }



    public interface IJsonModel
    {
        [ JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
    }



    public interface IJsonModel<TClass> : IJsonModel
        where TClass : IJsonModel<TClass>, IJsonizer<TClass> { }
}
