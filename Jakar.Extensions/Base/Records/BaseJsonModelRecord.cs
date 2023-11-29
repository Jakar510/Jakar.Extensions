namespace Jakar.Extensions;


[ Serializable ]
public record BaseJsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [ JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}



[ Serializable ]
public abstract record BaseJsonModelRecord<TClass> : BaseCollectionsRecord<TClass>, JsonModels.IJsonModel<TClass>
    where TClass : BaseJsonModelRecord<TClass>, JsonModels.IJsonizer<TClass>
{
    [ JsonExtensionData ] public IDictionary<string, JsonElement>? AdditionalData { get; set; }
}
