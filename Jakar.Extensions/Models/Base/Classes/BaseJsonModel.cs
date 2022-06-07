#nullable enable
namespace Jakar.Extensions.Models.Base.Classes;


[Serializable]
public class BaseJsonModel : ObservableClass, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract class BaseJsonModel<TClass> : BaseCollections<TClass>, JsonModels.IJsonModel where TClass : BaseJsonModel<TClass>
{
    public static TClass FromJson( string json ) => json.FromJson<TClass>();
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}
