// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:06 PM

namespace Jakar.Extensions;


public struct AsyncEnumerator<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
{
    private readonly IReadOnlyList<T>  _list;
    private          CancellationToken _token;
    private          int               _index = -1;
    public           T                 Current        => _list[_index];
    internal         bool              ShouldContinue => _token.ShouldContinue() && ++_index < _list.Count;


    public AsyncEnumerator( IReadOnlyList<T> list, CancellationToken token = default )
    {
        _list  = list;
        _token = token;
    }
    public ValueTask DisposeAsync() => default;


    public ValueTask<bool> MoveNextAsync() => new(ShouldContinue);
    public IAsyncEnumerator<T> GetAsyncEnumerator( CancellationToken token = default )
    {
        _index = -1;
        _token = token;
        return this;
    }


    public override string ToString() => $"AsyncEnumerator<{typeof(T).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
