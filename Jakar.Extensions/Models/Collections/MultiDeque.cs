namespace Jakar.Extensions.Models.Collections;


/// <summary>
/// <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class MultiDeque<T>
{
    protected readonly object   _lock = new();
    protected readonly Deque<T> _q;


    public MultiDeque() => _q = new Deque<T>();
    public MultiDeque( int            capacity ) => _q = new Deque<T>(capacity);
    public MultiDeque( IEnumerable<T> items ) => _q = new Deque<T>(items);


    public bool Contains( T obj )
    {
        lock ( _lock ) { return _q.Contains(obj); }
    }

    public T Dequeue()
    {
        lock ( _lock ) { return _q.RemoveFromFront(); }
    }

    public void Enqueue( T obj )
    {
        lock ( _lock ) { _q.AddToFront(obj); }
    }
}
