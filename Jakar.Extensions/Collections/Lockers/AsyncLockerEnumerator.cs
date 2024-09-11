// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue>( ILockedCollection<TValue> collection, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
{
    private const    int                       START_INDEX = 0;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          bool                      _isDisposed;
    private          CancellationToken         _token = token;
    private          FilterBuffer<TValue>?     _owner;
    private          int                       _index = START_INDEX;


    private ReadOnlyMemory<TValue>  _Memory        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _owner?.Memory ?? ReadOnlyMemory<TValue>.Empty; }
    TValue IAsyncEnumerator<TValue>.Current        => Current;
    public ref readonly TValue      Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _Memory.Span[_index]; }
    internal            bool        ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _token.ShouldContinue() && (uint)_index < (uint)_Memory.Length; }


    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        _isDisposed = true;
        _owner?.Dispose();
        _owner = null;
        return ValueTask.CompletedTask;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();

        if ( _Memory.IsEmpty )
        {
            _owner?.Dispose();
            _owner = await _collection.CopyAsync( _token );
            _index = START_INDEX;
        }

        if ( _token.IsCancellationRequested || (uint)_index >= (uint)_Memory.Length ) { return false; }

        _index++;
        return true;
    }


    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue> GetAsyncEnumerator( CancellationToken token = default )
    {
        ThrowIfDisposed();
        _owner?.Dispose();
        _owner = null;
        _index = START_INDEX;
        _token = token;
        return this;
    }
    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( _isDisposed, this );
}
