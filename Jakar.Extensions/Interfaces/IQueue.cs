namespace Jakar.Extensions;


public interface IQueue<TValue> : IEnumerable<TValue>
{
    public int     Count   { get; }
    public bool    IsEmpty { get; }
    public TValue? Next    { get; }

    public bool Contains( TValue value );

    public bool Remove( [NotNullWhen( true )] out TValue? value );
    public void Add( TValue                               value );

    public void Clear();
}
