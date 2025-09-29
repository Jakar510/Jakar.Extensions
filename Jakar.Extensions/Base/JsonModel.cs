namespace Jakar.Extensions;


[Serializable]
public class JsonModel : ObservableClass, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract class JsonModel<TClass> : ObservableClass<TClass>, JsonModels.IJsonModel
    where TClass : JsonModel<TClass>, IEqualComparable<TClass>
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}
