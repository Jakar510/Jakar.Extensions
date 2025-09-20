namespace Jakar.Extensions;


[Serializable]
public class JsonModel : ObservableClass, Json.IJsonModel
{
    [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
}



[Serializable]
public abstract class JsonModel<TClass> : ObservableClass<TClass>, Json.IJsonModel
    where TClass : JsonModel<TClass>, IEqualComparable<TClass>, IJsonModel<TClass>
{
    [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
}
