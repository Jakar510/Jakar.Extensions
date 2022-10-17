// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:52 PM

namespace Jakar.Database;


/// <summary>
///     <see href = "https://stackoverflow.com/a/15992856/9530917" />
/// </summary>
public struct IDGenerator<TRecord> : IAsyncEnumerator<long> where TRecord : TableRecord<TRecord>
{
    private readonly DbTableBase<TRecord> _table;
    public long Current { get; set; } = default;


    public IDGenerator(DbTableBase<TRecord> table) => _table = table;
    public ValueTask DisposeAsync()
    {
        Current = default;
        return ValueTask.CompletedTask;
    }


    public void Reset() => Current = default;
    public ValueTask<bool> MoveNextAsync(CancellationToken token = default) => _table.Call( MoveNextAsync, token );
    public async ValueTask<bool> MoveNextAsync(DbConnection connection, DbTransaction? transaction, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            Current = default;
            return default;
        }

        Current = await _table.NextID( connection, transaction, Current, token );
        return Current.IsValidID();
    }
    ValueTask<bool> IAsyncEnumerator<long>.MoveNextAsync() => MoveNextAsync();
}
