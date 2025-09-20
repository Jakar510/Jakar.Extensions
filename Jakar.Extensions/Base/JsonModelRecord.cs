namespace Jakar.Extensions;


[Serializable]
public record JsonModelRecord : ObservableRecord, Json.IJsonModel
{
    [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
}



[Serializable]
public abstract record JsonModelRecord<TClass, TID> : ObservableRecord<TClass, TID>, Json.IJsonModel
    where TClass : JsonModelRecord<TClass, TID>, IEqualComparable<TClass>, IJsonModel<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [JsonExtensionData] public JsonObject? AdditionalData { get; set; }
    protected JsonModelRecord() : base() { }
    protected JsonModelRecord( TID   ID ) : base(ID) { }
    public void Deconstruct( out TID id ) => id = ID;
}
