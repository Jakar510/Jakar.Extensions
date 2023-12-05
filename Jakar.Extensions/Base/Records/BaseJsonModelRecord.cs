namespace Jakar.Extensions;


[ Serializable ]
public record BaseJsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract record BaseJsonModelRecord<TClass, TID> : CollectionsRecord<TClass, TID>
    where TClass : BaseJsonModelRecord<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[ Serializable ]
public record BaseMsJsonModelRecord : ObservableRecord, MsJsonModels.IJsonModel
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract record BaseMsJsonModelRecord<TClass, TID> : CollectionsRecord<TClass, TID>
    where TClass : BaseMsJsonModelRecord<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}
