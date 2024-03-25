// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue>( ILockedCollection<TValue> collection, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
{
    private const    int                       START_INDEX = -1;
    private readonly ILockedCollection<TValue> _collection = collection;
    private          bool                      _isDisposed;
    private          CancellationToken         _token = token;
    private          int                       _index = START_INDEX;
    private          ReadOnlyMemory<TValue>    _cache;
    private          TValue?                   _current;
    public           TValue                    Current        => _current ?? throw new NullReferenceException( nameof(_current) );
    internal         bool                      ShouldContinue => _token.ShouldContinue() && _index < _cache.Length;


    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize( this );
        _isDisposed = true;
        _cache      = default;
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue>) ); }

        // ReSharper disable once InvertIf
        if ( _cache.IsEmpty )
        {
            _index = START_INDEX;
            _cache = await _collection.CopyAsync( _token );
        }

        _index += 1;

        _current = _index < _cache.Span.Length
                       ? _cache.Span[_index]
                       : default;

        bool result = _index < _cache.Span.Length;
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
        if ( _isDisposed ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue>) ); }

        _cache   = default;
        _current = default;
        _index   = START_INDEX;
    }


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
