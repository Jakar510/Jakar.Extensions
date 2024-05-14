namespace Jakar.Extensions;


public interface IQueue<T> : IEnumerable<T>
{
    public int  Count   { get; }
    public bool IsEmpty { get; }
    public T?   Next    { get; }

    public bool Contains( T value );

    public bool Remove( [NotNullWhen( true )] out T? value );
    public void Add( T                               value );

    public void Clear();
}
