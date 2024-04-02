namespace Jakar.Extensions;


[Serializable]
public record BaseJsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract record BaseJsonModelRecord<TRecord, TID> : ObservableRecord<TRecord, TID>
    where TRecord : BaseJsonModelRecord<TRecord, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}
