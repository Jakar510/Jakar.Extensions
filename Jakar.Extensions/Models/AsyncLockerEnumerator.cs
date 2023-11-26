// Jakar.Extensions :: Jakar.Extensions
// 11/25/2023  9:40 PM

namespace Jakar.Extensions;


public sealed class AsyncLockerEnumerator<TValue, TList> : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TList : ILockedCollection<TValue>
{
    private const    int                     START_INDEX = -1;
    private readonly TList                   _collection;
    private          CancellationToken       _token;
    private          int                     _index = START_INDEX;
    private          TValue?                 _current;
    private          ImmutableArray<TValue>? _cache;
    public           TValue                  Current        => _current ?? throw new NullReferenceException( nameof(_current) );
    internal         bool                    ShouldContinue => _token.ShouldContinue() && _index < _cache?.Length;


    public AsyncLockerEnumerator( TList collection, CancellationToken token = default )
    {
        _collection = collection;
        _token      = token;
    }
    public ValueTask DisposeAsync()
    {
        _cache = null;
        return default;
    }


    public async ValueTask<bool> MoveNextAsync()
    {
        // ReSharper disable once InvertIf
        if ( _cache is null )
        {
            _index = START_INDEX;
            _cache = await _collection.CopyAsync( _token );
        }

        return ILockedCollection<TValue>.MoveNext( Reset, out _current, ref _index, _cache.Value.AsSpan() );
    }
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncLockerEnumerator<TValue, TList> GetAsyncEnumerator( CancellationToken token = default )
    {
        Reset();
        if ( token.CanBeCanceled ) { _token = token; }

        return this;
    }
    public void Reset()
    {
        _index = START_INDEX;
        _cache = null;
    }


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
