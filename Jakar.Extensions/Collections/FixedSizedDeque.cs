namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
public class FixedSizedDeque<TValue>( int size )
{
    public readonly    Deque<TValue> _values = new(size);
    public readonly    int           Length  = size;
    protected readonly Lock          _lock   = new();


    public int Count
    {
        get
        {
            lock (_lock) { return _values.Count; }
        }
    }
    public bool IsEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Count == 0; }


    public bool Contains( TValue value )
    {
        lock (_lock) { return _values.Contains( value ); }
    }
    public ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _values.Contains( value ) ); }
    }


    public TValue Dequeue()
    {
        lock (_lock) { return _values.RemoveFromFront(); }
    }
    public ValueTask<TValue> DequeueAsync( CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( _values.RemoveFromFront() ); }
    }


    public void Enqueue( TValue value )
    {
        lock (_lock)
        {
            _values.AddToFront( value );
            while ( _values.Count > Length ) { _values.RemoveFromBack(); }
        }
    }
    public ValueTask EnqueueAsync( TValue value, CancellationToken token = default )
    {
        lock (_lock)
        {
            _values.AddToFront( value );
            while ( _values.Count > Length ) { _values.RemoveFromFront(); }
        }

        return ValueTask.CompletedTask;
    }
}
