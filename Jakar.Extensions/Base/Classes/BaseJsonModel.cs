namespace Jakar.Extensions;


[Serializable]
public class BaseJsonModel : ObservableClass, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract class BaseJsonModel<TClass> : ObservableClass<TClass>, JsonModels.IJsonModel
    where TClass : BaseJsonModel<TClass>
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}
