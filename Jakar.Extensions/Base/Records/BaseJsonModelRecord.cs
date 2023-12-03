using System.Text.Json;



namespace Jakar.Extensions;


[ Serializable ]
public record BaseJsonNetModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract record BaseJsonNetModelRecord<TClass> : BaseCollectionsRecord<TClass>, JsonModels.IJsonModel
    where TClass : BaseJsonModelRecord<TClass>
{
    [ JsonExtensionData ] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[ Serializable ]
public record BaseJsonModelRecord : ObservableRecord
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract record BaseJsonModelRecord<TClass> : BaseCollectionsRecord<TClass>
    where TClass : BaseJsonModelRecord<TClass>
{
    [ System.Text.Json.Serialization.JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}
