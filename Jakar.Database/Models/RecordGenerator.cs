// Jakar.Extensions :: Jakar.Database
// 10/16/2022  4:51 PM

namespace Jakar.Database;


/// <summary>
///     <see href="https://stackoverflow.com/a/15992856/9530917"/>
/// </summary>
public sealed class RecordGenerator<TSelf>( DbTable<TSelf> table ) : IAsyncEnumerable<TSelf>, IAsyncEnumerator<TSelf>
    where TSelf : class, ITableRecord<TSelf>
{
    private readonly AsyncKeyGenerator<TSelf> __generator = new(table);
    private readonly DbTable<TSelf>           __table     = table;
    private          CancellationToken         __token;
    private          TSelf?                   __current;


    public TSelf Current {  get => __current ?? throw new NullReferenceException(nameof(__current)); }


    public RecordGenerator( DbTable<TSelf> table, CancellationToken token ) : this(table) => __token = token;
    public async ValueTask DisposeAsync()
    {
        __current = null;
        await __generator.DisposeAsync();
    }


    public ValueTask<bool> MoveNextAsync() => __table.Call(MoveNextAsync, __token);
    public async ValueTask<bool> MoveNextAsync( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        if ( token.IsCancellationRequested )
        {
            __current = null;
            return false;
        }

        if ( await __generator.MoveNextAsync(connection, transaction, token) ) { __current = await __table.Get(connection, transaction, __generator.Current, token); }
        else
        {
            __current = null;
            __generator.Reset();
        }

        return __current is not null;
    }


    IAsyncEnumerator<TSelf> IAsyncEnumerable<TSelf>.GetAsyncEnumerator( CancellationToken token ) => WithCancellation(token);
    public RecordGenerator<TSelf> WithCancellation( CancellationToken token )
    {
        __token = token;
        __generator.WithCancellation(token);
        return this;
    }
}
