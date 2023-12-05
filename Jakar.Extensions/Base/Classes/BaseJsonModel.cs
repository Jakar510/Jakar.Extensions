using System.Text.Json;



namespace Jakar.Extensions;


[ Serializable ]
public class BaseJsonModel : ObservableClass, JsonModels.IJsonModel
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract class BaseJsonModel<TClass> : Collections<TClass>, JsonModels.IJsonModel
    where TClass : BaseJsonModel<TClass>
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData          { get; set; }
    public static                TClass                        FromJson( string json ) => json.FromJson<TClass>();
}



[ Serializable ]
public class BaseMsJsonModel : ObservableClass
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract class BaseMsJsonModel<TClass, TID> : Collections<TClass>
    where TClass : BaseMsJsonModel<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}
