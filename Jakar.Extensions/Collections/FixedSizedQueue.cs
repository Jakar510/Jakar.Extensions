namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
public class FixedSizedQueue<TValue>( int size,  
                             #if NET8_0
                                 object? locker = null
#else
                                 Lock? locker = null
#endif
    )
{ 
    protected readonly Queue<TValue> _q    = new(size);
#if NET8_0
    protected readonly object _lock = locker ?? new object();
#else
    protected readonly Lock     _lock = locker ?? new Lock();
#endif

    public int Size { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = size;


    public bool Contains( TValue value )
    {
        lock (_lock) { return _q.Contains( value ); }
    }
    public ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _q.Contains( value ) ); }
    }


    public TValue Dequeue()
    {
        lock (_lock) { return _q.Dequeue(); }
    }
    public ValueTask<TValue> DequeueAsync( CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _q.Dequeue() ); }
    }


    public void Enqueue( TValue value )
    {
        lock (_lock)
        {
            _q.Enqueue( value );
            while ( _q.Count > Size ) { _q.Dequeue(); }
        }
    }
    public ValueTask EnqueueAsync( TValue value, CancellationToken token = default )
    {
        lock (_lock)
        {
            _q.Enqueue( value );
            while ( _q.Count > Size ) { _q.Dequeue(); }
        }

        return ValueTask.CompletedTask;
    }
}
