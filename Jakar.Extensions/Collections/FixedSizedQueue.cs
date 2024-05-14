namespace Jakar.Extensions;


/// <summary>
///     <seealso href="https://stackoverflow.com/a/5852926/9530917"/>
/// </summary>
/// <typeparam name="T"> </typeparam>
public class FixedSizedQueue<T>( int size, Locker? locker = null )
{
    protected readonly Locker   _lock = locker ?? Locker.Default;
    protected readonly Queue<T> _q    = new(size);

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
        using ( _lock.Enter() ) { return _q.Dequeue(); }
    }
    public async ValueTask<T> DequeueAsync( CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ).ConfigureAwait( false ) ) { return _q.Dequeue(); }
    }


    public void Enqueue( T obj )
    {
        using ( _lock.Enter() )
        {
            _q.Enqueue( obj );
            while ( _q.Count > Size ) { _q.Dequeue(); }
        }
    }
    public async ValueTask EnqueueAsync( T obj, CancellationToken token = default )
    {
        using ( await _lock.EnterAsync( token ).ConfigureAwait( false ) )
        {
            _q.Enqueue( obj );
            while ( _q.Count > Size ) { _q.Dequeue(); }
        }
    }
}
