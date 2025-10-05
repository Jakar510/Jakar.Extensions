// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class AsyncKeyGenerator<TSelf>( DbTable<TSelf> table, CancellationToken token = default ) : IAsyncEnumerator<RecordID<TSelf>>, IAsyncEnumerable<RecordID<TSelf>>
    where TSelf : class, ITableRecord<TSelf>
{
    private readonly DbTable<TSelf>      __table = table;
    private          CancellationToken    __token = token;
    private          KeyGenerator<TSelf> __generator;
    public           RecordID<TSelf>     Current { get; private set; }


    public ValueTask DisposeAsync()
    {
        __generator.Dispose();
        Current = default;
        return ValueTask.CompletedTask;
    }
    public void Reset() => __generator = default;


    public ValueTask<bool> MoveNextAsync() => __table.Call(MoveNextAsync, __token);
    public async ValueTask<bool> MoveNextAsync( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token )
    {
        if ( token.IsCancellationRequested )
        {
            Current = default;
            return false;
        }

        if ( __generator.IsEmpty )
        {
            IEnumerable<RecordPair<TSelf>> pairs = await __table.SortedIDs(connection, transaction, token);
            __generator = KeyGenerator<TSelf>.Create(pairs);
        }

        if ( __generator.MoveNext() ) { Current = __generator.Current; }
        else
        {
            Current     = default;
            __generator = default;
        }

        return Current.Value != Guid.Empty;
    }


    IAsyncEnumerator<RecordID<TSelf>> IAsyncEnumerable<RecordID<TSelf>>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation(token);
    public AsyncKeyGenerator<TSelf> WithCancellation( CancellationToken token )
    {
        __token = token;
        return this;
    }
}
