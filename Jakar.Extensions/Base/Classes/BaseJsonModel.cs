namespace Jakar.Extensions;


[ Serializable ]
public class BaseJsonModel : ObservableClass, JsonModels.IJsonModel
{
    [ JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract class BaseJsonModel<TClass> : BaseCollections<TClass>, JsonModels.IJsonModel<TClass>
    where TClass : BaseJsonModel<TClass>, JsonModels.IJsonizer<TClass>
{
    [ JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData          { get; set; }
    public static                TClass                            FromJson( string json ) => json.FromJson<TClass>();
}
