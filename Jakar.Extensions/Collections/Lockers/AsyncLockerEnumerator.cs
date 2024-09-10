// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue>( ILockedCollection<TValue> collection, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
{
    private const    int                       START_INDEX = 0;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          bool                      _isDisposed;
    private          CancellationToken         _token = token;
    private          int                       _index = START_INDEX;
    private          ReadOnlyMemory<TValue>    _memory;


    TValue IAsyncEnumerator<TValue>.Current        => Current;
    public ref readonly TValue      Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _memory.Span[_index]; }
    internal            bool        ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _token.ShouldContinue() && (uint)_index < (uint)_memory.Length; }


    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        _isDisposed = true;
        _memory     = ReadOnlyMemory<TValue>.Empty;
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();

        if ( _memory.IsEmpty )
        {
            _memory = await _collection.CopyAsync( _token );
            _index  = START_INDEX;
        }

        if ( _token.IsCancellationRequested || (uint)_index >= (uint)_memory.Length ) { return false; }

        _index++;
        return true;
    }


    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue> GetAsyncEnumerator( CancellationToken token = default )
    {
        ThrowIfDisposed();
        _memory = ReadOnlyMemory<TValue>.Empty;
        _index  = START_INDEX;
        _token  = token;
        return this;
    }
    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";


    [MethodImpl( MethodImplOptions.AggressiveInlining )] private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf( _isDisposed, this );
}
