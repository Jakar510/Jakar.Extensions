namespace Jakar.Extensions.Models.Base.Classes;


[Serializable]
public class BaseJsonModel : BaseNotifyPropertyModel, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public class BaseJsonModel<TClass> : BaseCollections<TClass> where TClass : BaseJsonModel<TClass>
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }


    public static TClass FromJson( string json ) => json.FromJson<TClass>();
}
