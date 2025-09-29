// Jakar.Extensions :: Jakar.Extensions
// 09/15/2022  2:06 PM

namespace Jakar.Extensions;


public sealed class AsyncEnumerator<TValue, TList>( TList list, CancellationToken token = default ) : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    where TList : IReadOnlyList<TValue>
{
    private const    int               START_INDEX = NOT_FOUND;
    private readonly TList             __list       = list;
    private          CancellationToken __token      = token;
    private          int               __index      = START_INDEX;
    public           TValue            Current        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => __list[__index]; }
    internal         bool              ShouldContinue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => __token.ShouldContinue() && __index < __list.Count; }


    public ValueTask DisposeAsync() => default;


    public ValueTask<bool> MoveNextAsync()
    {
        __index++;
        return new ValueTask<bool>( ShouldContinue );
    }
    IAsyncEnumerator<TValue> IAsyncEnumerable<TValue>.GetAsyncEnumerator( CancellationToken token ) => GetAsyncEnumerator( token );
    public AsyncEnumerator<TValue, TList> GetAsyncEnumerator( CancellationToken token = default )
    {
        Reset();
        __token = token;
        return this;
    }
    public void Reset() => __index = START_INDEX;


    public override string ToString() => $"AsyncEnumerator<{typeof(TValue).Name}>( {nameof(__index)} : {__index}, {nameof(ShouldContinue)} : {ShouldContinue} )";
}
