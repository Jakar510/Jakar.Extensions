namespace Jakar.Extensions.Models.Base.Records;


[Serializable]
public record BaseJsonModelRecord : BaseNotifyPropertyModelRecord, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public record BaseJsonModelRecord<TClass> : BaseCollectionsRecord<TClass> where TClass : BaseJsonModelRecord<TClass>
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }


    public static TClass FromJson( string json ) => json.FromJson<TClass>();
}
