namespace Jakar.Extensions.Models.Collections;


/// <summary>
/// <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class FixedSizedDeque<T>
{
    protected readonly object   _lock = new();
    protected readonly Deque<T> _q;

    public int Limit { get; init; }

    public FixedSizedDeque( int limit )
    {
        Limit = limit;
        _q    = new Deque<T>(limit);
    }


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
        lock ( _lock )
        {
            _q.AddToFront(obj);
            while ( _q.Count > Limit ) { _q.RemoveFromBack(); }
        }
    }
}
