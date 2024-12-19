namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"> </typeparam>
public class FixedSizedDeque<T>( int size,
                             #if NET8_0
                                 object? locker = null
#else
                                 Lock? locker = null
#endif
)
{
    protected readonly Deque<T> _q = new(size);
#if NET8_0
    protected readonly object _lock = locker ?? new object();
#else
    protected readonly Lock     _lock = locker ?? new Lock();
#endif

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
        lock (_lock) { return _q.RemoveFromFront(); }
    }
    public ValueTask<T> DequeueAsync( CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _q.RemoveFromFront() ); }
    }


    public void Enqueue( T value )
    {
        lock (_lock)
        {
            _q.AddToFront( value );
            while ( _q.Count > Size ) { _q.RemoveFromBack(); }
        }
    }
    public ValueTask EnqueueAsync( T value, CancellationToken token = default )
    {
        lock (_lock)
        {
            _q.AddToFront( value );
            while ( _q.Count > Size ) { _q.RemoveFromFront(); }
        }

        return ValueTask.CompletedTask;
    }
}
