/*
namespace Jakar.Extensions;


public static class JsonModels
{
    static JsonModels() { }
    public static bool Contains<TClass>( this TClass self, string key )
        where TClass : IJsonModel => self.AdditionalData?.ContainsKey( key ) is true;


    public static bool Remove<TClass>( this TClass self, string key )
        where TClass : IJsonModel
    {
        self.AdditionalData ??= new IDictionary<string, JToken?>();
        return self.AdditionalData.Remove( key );
    }


    public static IDictionary<string, JToken?> GetData<TClass>( this TClass model )
        where TClass : IJsonModel => model.GetAdditionalData();


    public static IDictionary<string, JToken?> GetAdditionalData<TClass>( this TClass model )
        where TClass : IJsonModel => model.AdditionalData ??= new IDictionary<string, JToken?>();


    public static JToken? Get<TClass>( this TClass self, string key )
        where TClass : IJsonModel => self.AdditionalData?[key];


    public static TClass Add<TClass>( this TClass self, string key, bool value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, byte value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, short value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, int value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, long value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, float value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, double value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, decimal value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, string value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, DateTime value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, DateTimeOffset value )
        where TClass : IJsonModel
    {
        JToken node = value;
        return self.Add( key, node );
    }
    public static TClass Add<TClass>( this TClass self, string key, IEnumerable<KeyValuePair<string, JToken?>> properties )
        where TClass : IJsonModel
    {
        JToken node = new IDictionary<string, JToken?>( properties );
        return self.Add( key, node );
    }

    public static TClass Add<T, TClass>( this TClass self, string key, T value )
        where TClass : IJsonModel
        where T : IJsonizer => self.Add( key, value.GetProperties() );

    public static TClass Add<TClass>( this TClass self, string key, JToken? value )
        where TClass : IJsonModel
    {
        self.AdditionalData ??= new IDictionary<string, JToken?>();
        self.AdditionalData.Add( key, value );
        return self;
    }


    public static void SetAdditionalData<TClass>( this TClass model, IDictionary<string, JToken?>? data )
        where TClass : IJsonModel => model.AdditionalData = data;


    [ Pure ]
    public static async Task<string> ToJsonAsync<TClass>( this TClass model, JsonTypeInfo<TClass> info, bool writeIndented, CancellationToken token = default )
    {
        info.Options.WriteIndented = writeIndented;
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync( stream, model, info, token );
        using var reader = new StreamReader( stream );
        
    #if NET7_0_OR_GREATER
        return await reader.ReadToEndAsync( token );
    #else
        return await reader.ReadToEndAsync();
    #endif
    }
    [ Pure ]
    public static string ToJson<TClass>( this TClass model, JsonTypeInfo<TClass> info, bool writeIndented )
    {
        info.Options.WriteIndented = writeIndented;
        return JsonSerializer.Serialize( model, info );
    }
    [ Pure ] public static TClass FromJson<TClass>( this string json, JsonTypeInfo<TClass> info ) => JsonSerializer.Deserialize( json, info ) ?? throw new InvalidOperationException( nameof(json) );



    public interface IJsonizer
    {
        public IEnumerable<KeyValuePair<string, JToken?>> GetProperties();
    }



    public interface IJsonModel
    {
        [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
    }



    public interface IJsonModel<TClass> : IJsonModel
        where TClass : IJsonModel<TClass>
#if NET8_0
        , IJsonizer<TClass>
#endif
    { }



#if NET8_0
    public static string ToJson<TClass>( this TClass model )
        where TClass : IJsonizer<TClass> => model.ToJson( TClass.JsonTypeInfo(), false );
    public static string ToPrettyJson<TClass>( this TClass model )
        where TClass : IJsonizer<TClass> => model.ToJson( TClass.JsonTypeInfo(), true );


    public static Task<string> ToJsonAsync<TClass>( this TClass model )
        where TClass : IJsonModel, IJsonizer<TClass> => ToJsonAsync( model, TClass.JsonTypeInfo(), false );
    public static Task<string> ToPrettyJsonAsync<TClass>( this TClass model )
        where TClass : IJsonModel, IJsonizer<TClass> => ToJsonAsync( model, TClass.JsonTypeInfo(), true );


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
*/
