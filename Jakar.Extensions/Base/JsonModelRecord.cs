namespace Jakar.Extensions;


[Serializable]
public record JsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract record JsonModelRecord<TClass, TID> : ObservableRecord<TClass, TID>
    where TClass : JsonModelRecord<TClass, TID>, IComparisonOperators<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
    protected JsonModelRecord() : base() { }
    protected JsonModelRecord( TID   ID ) : base( ID ) { }
    public void Deconstruct( out TID id ) => id = this.ID;
}
