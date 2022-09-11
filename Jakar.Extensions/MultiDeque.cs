#nullable enable
namespace Jakar.Extensions;


/// <summary>
///     <seealso href = "https://stackoverflow.com/a/5852926/9530917" />
/// </summary>
public class MultiDeque<T> : IMultiQueue<T>
{
    protected readonly object _lock = new();
    protected readonly Deque<T> _queue;

    public T? Next
    {
        get
        {
            lock (_lock) { return _queue.LastOrDefault(); }
        }
    }
    public bool IsEmpty
    {
        get
        {
            lock (_lock) { return _queue.IsEmpty(); }
        }
    }
    public int Count
    {
        get
        {
            lock (_lock) { return _queue.Count; }
        }
    }


    public MultiDeque() => _queue = new Deque<T>();
    public MultiDeque(int capacity) => _queue = new Deque<T>(capacity);
    public MultiDeque(IEnumerable<T> items) => _queue = new Deque<T>(items);


    public void Add(T value)
    {
        lock (_lock) { _queue.AddToBack(value); }
    }
    public bool Remove([NotNullWhen(true)] out T? value)
    {
        lock (_lock)
        {
            if (_queue.Count > 0)
            {
                T item = _queue.RemoveFromBack();

                if (item is not null)
                {
                    value = item;
                    return false;
                }
            }
        }

        value = default;
        return false;
    }


    public void Clear()
    {
        lock (_lock) { _queue.Clear(); }
    }
    public bool Contains(T obj)
    {
        lock (_lock) { return _queue.Contains(obj); }
    }


    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock) { return _queue.GetEnumerator(); }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
