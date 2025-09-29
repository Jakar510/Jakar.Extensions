namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="TValue"> </typeparam>
public class FixedSizedDeque<TValue>( int size )
{
    public readonly    Deque<TValue> Values = new(size);
    public readonly    int           Length  = size;
    protected readonly Lock          _lock   = new();


    public int Count
    {
        get
        {
            lock (_lock) { return Values.Count; }
        }
    }
    public bool IsEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Count == 0; }


    public bool Contains( TValue value )
    {
        lock (_lock) { return Values.Contains( value ); }
    }
    public ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( Values.Contains( value ) ); }
    }


    public TValue Dequeue()
    {
        lock (_lock) { return Values.RemoveFromFront(); }
    }
    public ValueTask<TValue> DequeueAsync( CancellationToken token = default )
    {
        lock (_lock) { return ValueTask.FromResult( Values.RemoveFromFront() ); }
    }


    public void Enqueue( TValue value )
    {
        lock (_lock)
        {
            Values.AddToFront( value );
            while ( Values.Count > Length ) { Values.RemoveFromBack(); }
        }
    }
    public ValueTask EnqueueAsync( TValue value, CancellationToken token = default )
    {
        lock (_lock)
        {
            Values.AddToFront( value );
            while ( Values.Count > Length ) { Values.RemoveFromFront(); }
        }

        return ValueTask.CompletedTask;
    }
}
