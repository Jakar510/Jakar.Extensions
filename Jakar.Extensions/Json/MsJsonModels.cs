using System.Text.Json.Serialization.Metadata;



namespace Jakar.Extensions;


public static class MsJsonModels
{
    public static bool Contains<TClass>( this TClass self, string key )
        where TClass : IJsonModel => self.AdditionalData?.ContainsKey( key ) is true;


    public static bool Remove<TClass>( this TClass self, string key )
        where TClass : IJsonModel
    {
        self.AdditionalData ??= new JsonObject();
        return self.AdditionalData.Remove( key );
    }
    public static bool Remove<TClass>( this TClass self, string key, out JsonElement value )
        where TClass : IJsonModel
    {
        self.AdditionalData ??= new JsonObject();

        if ( self.AdditionalData.Remove( key, out var node ) )
        {
            value = node?.ToJsonElement() ?? default;
            return true;
        }

        value = default;
        return false;
    }


    public static JsonObject GetData<TClass>( this TClass model )
        where TClass : IJsonModel => model.GetAdditionalData();


    public static JsonObject GetAdditionalData<TClass>( this TClass model )
        where TClass : IJsonModel => model.AdditionalData ??= new JsonObject();


    public static JsonNode? Get<TClass>( this TClass self, string key )
        where TClass : IJsonModel => self.AdditionalData?[key];


    public static TClass Add<TClass>( this TClass self, string key, bool value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, byte value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, short value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, int value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, long value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, float value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, double value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, decimal value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, string value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, DateTime value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, DateTimeOffset value )
        where TClass : IJsonModel
    {
        JsonNode node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, IEnumerable<KeyValuePair<string, JsonNode?>> properties )
        where TClass : IJsonModel
    {
        JsonNode node = new JsonObject( properties );
        return self.Add( key, node );
    }

    public static TClass Add<T, TClass>( this TClass self, string key, T value )
        where TClass : IJsonModel
        where T : IJsonizer => self.Add( key, value.GetProperties() );

    public static TClass Add<TClass>( this TClass self, string key, JsonNode? value )
        where TClass : IJsonModel
    {
        self.AdditionalData ??= new JsonObject();
        self.AdditionalData.Add( key, value );
        return self;
    }
    public static TClass Add<TClass>( this TClass self, string key, JsonElement value )
        where TClass : IJsonModel => self.Add( key, value.ToJsonNode() );


    public static void Add( this IDictionary<string, JsonNode?> self, string key, JsonElement value ) => self.Add( key, value.ToJsonNode() );


    public static JsonNode?   ToJsonNode( this    JsonElement node ) => node.Deserialize<JsonNode>();
    public static JsonElement ToJsonElement( this JsonNode    node ) => node.Deserialize<JsonElement>();


    public static void SetAdditionalData<TClass>( this TClass model, JsonObject? data )
        where TClass : IJsonModel => model.AdditionalData = data;


    [ Pure ]
    public static async Task<string> ToJsonAsync<TClass>( this TClass model, JsonSerializerOptions options )

    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync( stream, model, options );
        using var reader = new StreamReader( stream );
        return await reader.ReadToEndAsync();
    }
    [ Pure ] public static string ToJson<TClass>( this   TClass model, JsonSerializerOptions options ) => JsonSerializer.Serialize( model, options );
    [ Pure ] public static TClass FromJson<TClass>( this string json,  JsonTypeInfo<TClass>  info )    => JsonSerializer.Deserialize( json, info ) ?? throw new InvalidOperationException( nameof(json) );



    public interface IJsonizer
    {
        public IEnumerable<KeyValuePair<string, JsonNode?>> GetProperties();
    }



    public interface IJsonModel
    {
        [ JsonExtensionData ] public JsonObject? AdditionalData { get; set; }
    }



    public interface IJsonModel<TClass> : IJsonModel
        where TClass : IJsonModel<TClass>
    #if NET8_0
        , IJsonizer<TClass>
#endif
    { }



#if NET8_0


    public static string ToJson<TClass>( this TClass model )
        where TClass : IJsonizer<TClass> => model.ToJson( TClass.JsonOptions( false ) );
    public static string ToPrettyJson<TClass>( this TClass model )
        where TClass : IJsonizer<TClass> => model.ToJson( TClass.JsonOptions( true ) );


    public static Task<string> ToJsonAsync<TClass>( this TClass model )
        where TClass : IJsonModel, IJsonizer<TClass> => ToJsonAsync( model, TClass.JsonOptions( false ) );
    public static Task<string> ToPrettyJsonAsync<TClass>( this TClass model )
        where TClass : IJsonModel, IJsonizer<TClass> => ToJsonAsync( model, TClass.JsonOptions( true ) );


    public static TClass FromJson<TClass>( this string json )
        where TClass : IJsonizer<TClass> => json.FromJson( TClass.JsonTypeInfo() ) ?? throw new InvalidOperationException( nameof(json) );



    public interface IJsonizer<TClass>
    {
        [ Pure ] public abstract static JsonTypeInfo<TClass>  JsonTypeInfo();
        [ Pure ] public abstract static TClass                FromJson( string  json );
        [ Pure ] public abstract static JsonSerializerOptions JsonOptions( bool writeIndented );
    }



#endif
}
