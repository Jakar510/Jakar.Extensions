// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TSelf>
{
    public IAsyncEnumerable<TSelf> Where( bool           matchAll,   PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, matchAll,                               parameters, token);
    public IAsyncEnumerable<TSelf> Where( string         sql,        PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, new SqlCommand<TSelf>(sql, parameters), token);
    public IAsyncEnumerable<TSelf> Where<TValue>( string columnName, TValue?            value,      [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, columnName,                             value, token);


    public virtual IAsyncEnumerable<TSelf> Where( NpgsqlConnection connection, NpgsqlTransaction? transaction, string sql, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default ) => Where(connection, transaction, new SqlCommand<TSelf>(sql, parameters), token);
    public virtual IAsyncEnumerable<TSelf> Where( NpgsqlConnection connection, NpgsqlTransaction? transaction, bool matchAll, PostgresParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand<TSelf> command = SqlCommand<TSelf>.Get(matchAll, parameters);
        return Where(connection, transaction, command, token);
    }
    public virtual async IAsyncEnumerable<TSelf> Where( NpgsqlConnection connection, NpgsqlTransaction? transaction, SqlCommand<TSelf> command, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using NpgsqlCommand    cmd    = command.ToCommand(connection, transaction);
        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(token);
        await foreach ( TSelf record in reader.CreateAsync<TSelf>(token) ) { yield return record; }
    }
    public virtual IAsyncEnumerable<TSelf> Where( SqlCommand<TSelf> command, [EnumeratorCancellation] CancellationToken token = default ) => this.TryCall(Where, command, token);
    public virtual IAsyncEnumerable<TSelf> Where<TValue>( NpgsqlConnection connection, NpgsqlTransaction? transaction, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        PostgresParameters parameters = PostgresParameters.Create<TSelf>();
        parameters.Add(nameof(value),      value);
        parameters.Add(nameof(columnName), columnName);

        SqlCommand<TSelf> sql = new($"SELECT * FROM {TSelf.TableName} WHERE @{nameof(columnName)} = @{nameof(value)};", parameters);
        return Where(connection, transaction, sql, token);
    }
}
