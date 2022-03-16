namespace Jakar.Extensions.Models.Collections;


public interface IMultiQueue<T> : IEnumerable<T>
{
    public void Add( T file );

    public bool Remove( [NotNullWhen(true)] out T? file );

    public void Clear();

    public bool Contains( T obj );
    public T?   Next    { get; }
    public bool IsEmpty { get; }
    public int  Count   { get; }
}



/// <summary>
/// <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class MultiQueue<T> : IMultiQueue<T>
{
    protected readonly ConcurrentQueue<T> _queue;


    public T? Next => _queue.TryPeek(out T? result)
                          ? result
                          : default;

    public bool IsEmpty => _queue.IsEmpty;
    public int  Count   => _queue.Count;


    public MultiQueue() => _queue = new ConcurrentQueue<T>();
    public MultiQueue( IEnumerable<T> items ) => _queue = new ConcurrentQueue<T>(items);


    public bool Contains( T obj ) => _queue.Contains(obj);
    public void Clear()           => _queue.Clear();
    public void Add( T file )     => _queue.Enqueue(file);

    public bool Remove( [NotNullWhen(true)] out T? item )
    {
        bool result = _queue.TryDequeue(out item);

        return result && item is null;
    }


    public IEnumerator<T>   GetEnumerator() => _queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
