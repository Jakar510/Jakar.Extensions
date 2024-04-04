namespace Jakar.Extensions;


[Serializable]
public record JsonModelRecord : ObservableRecord, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }
}



[Serializable]
public abstract record JsonModelRecord<TRecord, TID> : ObservableRecord<TRecord, TID>
    where TRecord : JsonModelRecord<TRecord, TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    protected JsonModelRecord() : base() { }
    protected JsonModelRecord( TID ID ) : base( ID ) { }
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData            { get; set; }
    public                     void                          Deconstruct( out TID ID ) { ID = this.ID; }
}
