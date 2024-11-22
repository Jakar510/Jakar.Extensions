namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"> </typeparam>
public class FixedSizedQueue<T>( int size, Lock? locker = null )
{
    protected readonly Lock     _lock = locker ?? new Lock();
    protected readonly Queue<T> _q    = new(size);

    public int Size { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = size;


    public bool Contains( T value )
    {
        lock (_lock) { return _q.Contains( value ); }
    }
    public ValueTask<bool> ContainsAsync( T value, CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _q.Contains( value ) ); }
    }


    public T Dequeue()
    {
        lock (_lock) { return _q.Dequeue(); }
    }
    public ValueTask<T> DequeueAsync( CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _q.Dequeue() ); }
    }


    public void Enqueue( T value )
    {
        lock (_lock)
        {
            _q.Enqueue( value );
            while ( _q.Count > Size ) { _q.Dequeue(); }
        }
    }
    public ValueTask EnqueueAsync( T value, CancellationToken token = default )
    {
        lock (_lock)
        {
            _q.Enqueue( value );
            while ( _q.Count > Size ) { _q.Dequeue(); }
        }

        return ValueTask.CompletedTask;
    }
}
