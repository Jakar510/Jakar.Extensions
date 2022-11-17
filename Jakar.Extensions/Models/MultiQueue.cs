#nullable enable
namespace Jakar.Extensions;


public interface IMultiQueue<T> : IEnumerable<T>
{
    public bool IsEmpty { get; }
    public int  Count   { get; }
    public T?   Next    { get; }

    public bool Contains( T value );

    public bool Remove( [NotNullWhen( true )] out T? value );
    public void Add( T                               value );

    public void Clear();
}



/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"> </typeparam>
public class MultiQueue<T> : IMultiQueue<T>
{
    protected readonly ConcurrentQueue<T> _queue;

    public bool IsEmpty => _queue.IsEmpty;
    public int  Count   => _queue.Count;


    public T? Next => _queue.TryPeek( out T? result )
                          ? result
                          : default;


    public MultiQueue() => _queue = new ConcurrentQueue<T>();
    public MultiQueue( IEnumerable<T> items ) => _queue = new ConcurrentQueue<T>( items );


    public bool Contains( T value ) => _queue.Contains( value );
    public void Clear() => _queue.Clear();
    public void Add( T value ) => _queue.Enqueue( value );


    public bool Remove( [NotNullWhen( true )] out T? value )
    {
        bool result = _queue.TryDequeue( out value );

        return result && value is not null;
    }


    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
