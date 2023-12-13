// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

namespace Jakar.Extensions;


public static class JsonModels
{
    public static bool Contains( this IJsonModel       self, string key ) => self.AdditionalData?.ContainsKey( key )      ?? false;
    public static bool Contains( this IJsonStringModel self, string key ) => self.GetAdditionalData()?.ContainsKey( key ) ?? false;


    public static bool Remove( this IJsonModel self, string key )
    {
        self.AdditionalData ??= new Dictionary<string, JToken?>();
        return self.AdditionalData.Remove( key );
    }
    public static bool Remove( this IJsonModel self, string key, out JToken? item )
    {
        self.AdditionalData ??= new Dictionary<string, JToken?>();
        self.AdditionalData.TryGetValue( key, out item );
        return self.AdditionalData.Remove( key );
    }
    public static bool Remove( this IJsonStringModel self, string key )
    {
        IDictionary<string, JToken?> additionalData = self.GetData();
        bool                         result         = additionalData.Remove( key );
        self.SetAdditionalData( additionalData );
        return result;
    }
    public static bool Remove( this IJsonStringModel self, string key, out JToken? item )
    {
        IDictionary<string, JToken?> additionalData = self.GetData();
        additionalData.TryGetValue( key, out item );
        bool result = additionalData.Remove( key );
        self.SetAdditionalData( additionalData );
        return result;
    }


    public static IDictionary<string, JToken?> GetData( this IJsonModel       model ) => model.GetAdditionalData() ?? new Dictionary<string, JToken?>();
    public static IDictionary<string, JToken?> GetData( this IJsonStringModel model ) => model.GetAdditionalData() ?? new Dictionary<string, JToken?>();


    public static IDictionary<string, JToken?>? GetAdditionalData( this IJsonModel model ) => model.AdditionalData;
    public static IDictionary<string, JToken?>? GetAdditionalData( this IJsonStringModel model ) => string.IsNullOrWhiteSpace( model.AdditionalData )
                                                                                                        ? default
                                                                                                        : model.AdditionalData.FromJson<Dictionary<string, JToken?>>();


    public static JToken? Get( this IJsonModel       self, string key ) => self.AdditionalData?[key];
    public static JToken? Get( this IJsonStringModel self, string key ) => self.GetAdditionalData()?[key];
    public static T? Get<T>( this IJsonModel self, string key )
    {
        JToken? token = self.Get( key );
        if ( token is null ) { return default; }

        return token.ToObject<T>();
    }
    public static T? Get<T>( this IJsonStringModel self, string key )
    {
        JToken? token = self.Get( key );
        if ( token is null ) { return default; }

        return token.ToObject<T>();
    }


    public static void Add( this IJsonModel self, string key, bool           item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, byte           item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, short          item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, int            item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, long           item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, float          item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, double         item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, decimal        item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, string         item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, DateTime       item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, DateTimeOffset item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, TimeSpan       item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, object         item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonModel self, string key, JToken? item )
    {
        self.AdditionalData ??= new Dictionary<string, JToken?>();
        self.AdditionalData.Add( key, item );
    }
    public static void Add( this IJsonStringModel self, string key, bool           item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, byte           item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, short          item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, int            item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, long           item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, float          item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, double         item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, decimal        item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, string         item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, DateTime       item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, DateTimeOffset item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, TimeSpan       item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, object         item ) => self.Add( key, JToken.FromObject( item ) );
    public static void Add( this IJsonStringModel self, string key, JToken? item )
    {
        IDictionary<string, JToken?> additionalData = self.GetData();
        additionalData.Add( key, item );
        self.SetAdditionalData( additionalData );
    }


    public static void SetAdditionalData( this IJsonModel       model, IDictionary<string, JToken?>? data ) => model.AdditionalData = data;
    public static void SetAdditionalData( this IJsonStringModel model, IDictionary<string, JToken?>? data ) => model.AdditionalData = data?.ToPrettyJson();



    public interface IJsonModel
    {
        [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
    }



    public interface IJsonStringModel
    {
        public string? AdditionalData { get; set; }
    }
}
