namespace Jakar.Extensions;


[Serializable]
public record BaseJsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract record BaseJsonModelRecord<TClass, TID> : CollectionsRecord<TClass, TID>
    where TClass : BaseJsonModelRecord<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}
