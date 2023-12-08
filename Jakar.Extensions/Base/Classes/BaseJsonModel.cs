namespace Jakar.Extensions;


[ Serializable ]
public class BaseJsonModel : ObservableClass, JsonModels.IJsonModel
{
    [ Newtonsoft.Json.JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract class BaseJsonModel<TClass> : Collections<TClass>, JsonModels.IJsonModel
    where TClass : BaseJsonModel<TClass>
{
    [ Newtonsoft.Json.JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData          { get; set; }
    public static                TClass                        FromJson( string json ) => json.FromJson<TClass>();
}



[ Serializable ]
public class BaseMsJsonModel : ObservableClass, MsJsonModels.IJsonModel
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public JsonObject? AdditionalData { get; set; }
}



[ Serializable ]
public abstract class BaseMsJsonModel<TClass, TID> : Collections<TClass>
    where TClass : BaseMsJsonModel<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public JsonObject? AdditionalData { get; set; }
}
