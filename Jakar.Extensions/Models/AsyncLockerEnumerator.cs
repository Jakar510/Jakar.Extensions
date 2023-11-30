// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public sealed class AsyncLockerEnumerator<TValue, TList> : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TList : ILockedCollection<TValue>
{
    private const    int                    START_INDEX = -1;
    private readonly TList                  _collection;
    private          bool                   _isDisposed;
    private          CancellationToken      _token;
    private          int                    _index = START_INDEX;
    private          TValue?                _current;
    private          ReadOnlyMemory<TValue> _cache;
    public           TValue                 Current        => _current ?? throw new NullReferenceException( nameof(_current) );
    internal         bool                   ShouldContinue => _token.ShouldContinue() && _index < _cache.Length;


    public AsyncLockerEnumerator( TList collection, CancellationToken token = default )
    {
        _collection = collection;
        _token      = token;
    }
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _cache      = default;
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue, TList>) ); }

        // ReSharper disable once InvertIf
        if ( _cache.IsEmpty )
        {
            _index = START_INDEX;
            _cache = await _collection.CopyAsync( _token );
        }

        return ILockedCollection<TValue>.MoveNext( Reset, out _current, ref _index, _cache.Span );
    }
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue, TList> GetAsyncEnumerator( CancellationToken token = default )
    {
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue, TList>) ); }

        Reset();
        if ( token.CanBeCanceled ) { _token = token; }

        return this;
    }
    public void Reset()
    {
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue, TList>) ); }

        _index = START_INDEX;
        _cache = default;
    }


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
