// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM


namespace Jakar.Database;


public interface IDbTable : IAsyncDisposable;



public interface IConnectableDb : IDbTable
{
    public IsolationLevel TransactionIsolationLevel { get; }

    public ValueTask<NpgsqlConnection> ConnectAsync( CancellationToken token );
}



public interface IConnectableDbRoot : IConnectableDb
{
    public ref readonly DbOptions Options { get; }
    public IAsyncEnumerable<TSelf> Where<TSelf>( NpgsqlConnection connection, NpgsqlTransaction? transaction, string sql, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>, IDateCreated;
    public IAsyncEnumerable<TValue> Where<TSelf, TValue>( NpgsqlConnection connection, NpgsqlTransaction? transaction, string sql, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
        where TValue : struct
        where TSelf : ITableRecord<TSelf>;
}
