namespace Jakar.Extensions;


/// <summary> <seealso href="https://stackoverflow.com/a/5852926/9530917"/> </summary>
/// <typeparam name="T"> </typeparam>
public class FixedSizedQueue<T>( int limit )
{
    protected readonly object   _lock = new();
    protected readonly Queue<T> _q = new( limit );

    public int Limit { get; init; } = limit;


    public bool Contains( T obj )
    {
        lock (_lock) { return _q.Contains( obj ); }
    }

    public T Dequeue()
    {
        lock (_lock) { return _q.Dequeue(); }
    }


    public void Enqueue( T obj )
    {
        lock (_lock)
        {
            _q.Enqueue( obj );
            while ( _q.Count > Limit ) { _q.Dequeue(); }
        }
    }
}
