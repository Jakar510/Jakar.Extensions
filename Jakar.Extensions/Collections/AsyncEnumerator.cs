// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:06 PM

namespace Jakar.Extensions;


public sealed class AsyncEnumerator<TValue, TList>( TList list, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TList : IReadOnlyList<TValue>
{
    private const    int               START_INDEX = NOT_FOUND;
    private readonly TList             _list       = list;
    private          CancellationToken _token      = token;
    private          int               _index      = START_INDEX;
    public           TValue            Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _list[_index]; }
    internal         bool              ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _token.ShouldContinue() && _index < _list.Count; }


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
        _token = token;
        return this;
    }
    public void Reset() => _index = START_INDEX;


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(_index)} : {_index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
