// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue, TCloser>( ILockedCollection<TValue, TCloser> collection, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TCloser : IDisposable
{
    private const    int                                START_INDEX = 0;
    private readonly ILockedCollection<TValue, TCloser> _collection = collection;
    private          bool                               _isDisposed;
    private          CancellationToken                  _token = token;
    private          FilterBuffer<TValue>?              _buffer;
    private          int                                _index = START_INDEX;


    private ReadOnlyMemory<TValue>  _Memory        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _buffer?.Memory ?? ReadOnlyMemory<TValue>.Empty; }
    TValue IAsyncEnumerator<TValue>.Current        => Current;
    public ref readonly TValue      Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _Memory.Span[_index]; }
    internal            bool        ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _token.ShouldContinue() && (uint)_index < (uint)_Memory.Length; }


    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        _isDisposed = true;
        _buffer?.Dispose();
        _buffer = null;
        return ValueTask.CompletedTask;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();

        if ( _Memory.IsEmpty )
        {
            _buffer?.Dispose();
            _buffer = await _collection.CopyAsync( _token );
            _index  = START_INDEX;
        }

        if ( _token.IsCancellationRequested || (uint)_index >= (uint)_Memory.Length ) { return false; }

        _index++;
        return true;
    }


    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue, TCloser> GetAsyncEnumerator( CancellationToken token = default )
    {
        ThrowIfDisposed();
        _buffer?.Dispose();
        _buffer = null;
        _index  = START_INDEX;
        _token  = token;
        return this;
    }
    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( _isDisposed, this );
}
