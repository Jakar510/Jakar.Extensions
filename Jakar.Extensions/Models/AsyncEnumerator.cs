// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:06 PM

using Newtonsoft.Json.Linq;



namespace Jakar.Extensions;


public sealed class AsyncEnumerator<TValue, TList> : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TList : IReadOnlyList<TValue>
{
    private const    int               START_INDEX = -1;
    private readonly TList             _list;
    private          CancellationToken _token;
    private          int               _index = START_INDEX;
    public           TValue            Current        => _list[_index];
    internal         bool              ShouldContinue => _token.ShouldContinue() && _index < _list.Count;


    public AsyncEnumerator( TList list, CancellationToken token = default )
    {
        _list  = list;
        _token = token;
    }
    public ValueTask DisposeAsync() => default;


    public ValueTask<bool> MoveNextAsync()
    {
        _index++;
        return new ValueTask<bool>( ShouldContinue );
    }
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncEnumerator<TValue, TList> GetAsyncEnumerator( CancellationToken token = default )
    {
        Reset();
        if ( token.CanBeCanceled ) { _token = token; }

        return this;
    }
    public void Reset() => _index = START_INDEX;


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}