// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue>( ILockedCollection<TValue> collection, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
{
    private const    int                       START_INDEX = NOT_FOUND;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          bool                      _isDisposed;
    private          CancellationToken         _token = token;
    private          int                       _index = START_INDEX;
    private          ReadOnlyMemory<TValue>    _memory;


    TValue IAsyncEnumerator<TValue>.Current        => Current;
    public ref readonly TValue      Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _memory.Span[_index]; }
    internal            bool        ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _token.ShouldContinue() && _index < _memory.Length; }


    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        _isDisposed = true;
        _memory     = default;
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        ThrowIfDisposed();
        if ( _memory.IsEmpty ) { _memory = await _collection.CopyAsync( _token ); }

        _index++;
        bool result = ShouldContinue;
        if ( result is false ) { Reset(); }

        return result;
    }
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue> GetAsyncEnumerator( CancellationToken token = default )
    {
        Reset();
        _token = token;
        return this;
    }
    public void Reset()
    {
        ThrowIfDisposed();
        _memory = default;
        _index  = START_INDEX;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ThrowIfDisposed()
    {
    #if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf( _isDisposed, this );
    #else
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue>) ); }
    #endif
    }


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
