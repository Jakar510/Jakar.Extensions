// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public IAsyncEnumerable<TSelf> Where( bool           matchAll,   PostgresParameters  parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, matchAll,                        parameters, token);
    public IAsyncEnumerable<TSelf> Where( string         sql,        PostgresParameters? parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, new SqlCommand(sql, parameters), token);
    public IAsyncEnumerable<TSelf> Where<TValue>( string columnName, TValue?            value,      [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, columnName,                      value, token);


    public virtual IAsyncEnumerable<TSelf> Where( NpgsqlConnection connection, NpgsqlTransaction? transaction, string sql, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default ) => Where(connection, transaction, new SqlCommand(sql, parameters), token);
    public virtual async IAsyncEnumerable<TSelf> Where( NpgsqlConnection connection, NpgsqlTransaction? transaction, bool matchAll, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand<TSelf>               command = SqlCommand<TSelf>.Get(matchAll, parameters);
        await using DbDataReader reader  = await _database.ExecuteReaderAsync(connection, transaction, command, token);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TSelf> Where( NpgsqlConnection connection, NpgsqlTransaction? transaction, SqlCommand<TSelf> sql, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync(connection, transaction, sql, token);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TSelf> Where( SqlCommand<TSelf> definition, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync(definition);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }
    public virtual IAsyncEnumerable<TSelf> Where<TValue>( NpgsqlConnection connection, NpgsqlTransaction? transaction, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        PostgresParameters parameters = new();
        parameters.Add(nameof(value), value);

        SqlCommand<TSelf> sql = new($"SELECT * FROM {TSelf.TableName} WHERE {columnName} = @{nameof(value)};", parameters);
        return Where(connection, transaction, sql, token);
    }
}
