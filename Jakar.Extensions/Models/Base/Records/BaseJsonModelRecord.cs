namespace Jakar.Extensions.Models.Base.Records;


[Serializable]
public record BaseJsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract record BaseJsonModelRecord<TClass> : BaseCollectionsRecord<TClass>, JsonModels.IJsonModel where TClass : BaseJsonModelRecord<TClass>
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }


    public static TClass FromJson( string json ) => json.FromJson<TClass>();
}
