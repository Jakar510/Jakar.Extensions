// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public class AsyncLockerEnumerator<TValue, TCloser, TList> : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TList : ILockedCollection<TCloser, TValue>
    where TCloser : IDisposable
{
    private const    long                   DISPOSED     = 1;
    private const    long                   NOT_DISPOSED = 0;
    private const    int                    START_INDEX  = -1;
    private readonly TList                  _collection;
    private          long                   _state = NOT_DISPOSED;
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
        Interlocked.Exchange( ref _state, DISPOSED );
        _cache = default;
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        if ( Interlocked.Read( ref _state ) == DISPOSED ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue, TCloser, TList>) ); }

        // ReSharper disable once InvertIf
        if ( _cache.IsEmpty )
        {
            Interlocked.Exchange( ref _index, START_INDEX );
            _cache = await _collection.CopyAsync( _token );
        }

        bool result = ILockedCollection<TCloser, TValue>.MoveNext( ref _index, _cache.Span, out _current );
        if ( result is false ) { Reset(); }

        return result;
    }
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue, TCloser, TList> GetAsyncEnumerator( CancellationToken token = default )
    {
        Reset();
        _token = token;
        return this;
    }
    public void Reset()
    {
        if ( Interlocked.Read( ref _state ) == DISPOSED ) { throw new ObjectDisposedException( nameof(AsyncLockerEnumerator<TValue, TCloser, TList>) ); }

        _cache   = default;
        _current = default;
        Interlocked.Exchange( ref _index, START_INDEX );
    }


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
