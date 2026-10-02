namespace Jakar.Extensions;


public interface IUniqueID<out TID>
    where TID : IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public TID ID { get; }
}
