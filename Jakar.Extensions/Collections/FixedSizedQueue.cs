namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
public class FixedSizedQueue<TValue>( int size )
{
    public readonly    int                   Length  = size;
    protected readonly LockFreeStack<TValue> _values = new();


    public int  Count   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _values.Count; }
    public bool IsEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Count == 0; }

    
    public virtual bool Contains( TValue value ) => _values.Contains( value );
    public virtual ValueTask<bool> ContainsAsync( TValue value, CancellationToken token = default ) =>
        token.IsCancellationRequested
            ? ValueTask.FromCanceled<bool>( token )
            : ValueTask.FromResult( _values.Contains( value ) );


    public virtual TValue? Dequeue() => _values.TryPop();
    public virtual ValueTask<TValue?> DequeueAsync( CancellationToken token = default ) =>
        token.IsCancellationRequested
            ? ValueTask.FromCanceled<TValue?>( token )
            : ValueTask.FromResult( _values.TryPop() );


    public virtual void Enqueue( TValue value )
    {
        _values.Push( value );
        while ( _values.Count > Length ) { _values.TryPop( out _ ); }
    }
    public virtual ValueTask EnqueueAsync( TValue value, CancellationToken token = default )
    {
        _values.Push( value );
        while ( _values.Count > Length ) { _values.TryPop( out _ ); }

        return token.IsCancellationRequested
                   ? ValueTask.FromCanceled( token )
                   : ValueTask.CompletedTask;
    }
}
