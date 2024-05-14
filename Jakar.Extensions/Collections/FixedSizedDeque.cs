namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"> </typeparam>
public class FixedSizedDeque<T>( int size, Locker? locker = null )
{
    protected readonly Deque<T> _q    = new(size);
    protected readonly Locker   _lock = locker ?? Locker.Default;

    public int Size { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = size;


    public bool Contains( T obj )
    {
        using ( _lock.Enter() ) { return _q.Contains( obj ); }
    }
    public async ValueTask<bool> ContainsAsync( T obj, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ).ConfigureAwait( false ) ) { return _q.Contains( obj ); }
    }

    public T Dequeue()
    {
        using ( _lock.Enter() ) { return _q.RemoveFromFront(); }
    }
    public async ValueTask<T> DequeueAsync( CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ).ConfigureAwait( false ) ) { return _q.RemoveFromFront(); }
    }


    public void Enqueue( T obj )
    {
        using ( _lock.Enter() )
        {
            _q.AddToFront( obj );
            while ( _q.Count > Size ) { _q.RemoveFromBack(); }
        }
    }
    public async ValueTask EnqueueAsync( T obj, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ).ConfigureAwait( false ) )
        {
            _q.AddToFront( obj );
            while ( _q.Count > Size ) { _q.RemoveFromFront(); }
        }
    }
}
